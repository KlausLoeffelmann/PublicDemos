using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.DirectComposition;
using Windows.Win32.Graphics.DirectWrite;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;

namespace WinFormsPong;

// =========================================================================
// DCOMP / D2D RENDER HOST
//
// Pipeline: D3D11 device -> DXGI device -> D2D device/context ->
// flip-model composition swap chain -> DirectComposition visual tree.
// All COM access goes through the CsWin32-generated interfaces, which are
// classic [ComImport] RCWs, so QueryInterface is just a cast.
// =========================================================================
internal sealed unsafe class D2DRenderHost : IDisposable
{
    private const DXGI_FORMAT BackBufferFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;

    private ID3D11Device? _d3dDevice;
    private IDXGIDevice? _dxgiDevice;
    private IDXGIFactory2? _dxgiFactory2;
    private IDXGISwapChain1? _swapChain;
    private ID2D1Factory1? _d2dFactory;
    private ID2D1Device? _d2dDevice;
    private ID2D1DeviceContext? _d2dContext;
    private ID2D1Bitmap1? _d2dBitmap;
    private IDCompositionDevice? _dcompDevice;
    private IDCompositionTarget? _dcompTarget;
    private IDCompositionVisual? _rootVisual;
    private IDCompositionVisual? _contentVisual;
    private IDWriteFactory? _dwriteFactory;
    private IDWriteTextFormat? _textFormat;

    private Size _clientSize;

    public void Initialize(IntPtr hwnd, Size clientSize)
    {
        _clientSize = clientSize;

        // 1. D3D11 device (hardware first, WARP fallback). BGRA support is
        //    required for Direct2D interop.
        _d3dDevice = CreateD3DDevice();
        _dxgiDevice = (IDXGIDevice)_d3dDevice;

        // 2. DXGI 1.2 factory for the composition swap chain.
        PInvoke.CreateDXGIFactory2(0, typeof(IDXGIFactory2).GUID, out object factoryObj).ThrowOnFailure();
        _dxgiFactory2 = (IDXGIFactory2)factoryObj;

        // 3. Direct2D factory + device + device context.
        PInvoke.D2D1CreateFactory(
            D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED,
            typeof(ID2D1Factory1).GUID, null, out object d2dFactoryObj).ThrowOnFailure();
        _d2dFactory = (ID2D1Factory1)d2dFactoryObj;
        _d2dFactory.CreateDevice(_dxgiDevice, out _d2dDevice);
        _d2dDevice.CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE, out _d2dContext);

        // 4. DirectComposition device + DirectWrite factory.
        PInvoke.DCompositionCreateDevice(_dxgiDevice, typeof(IDCompositionDevice).GUID, out object dcompObj).ThrowOnFailure();
        _dcompDevice = (IDCompositionDevice)dcompObj;

        PInvoke.DWriteCreateFactory(DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED, typeof(IDWriteFactory).GUID, out object dwObj).ThrowOnFailure();
        _dwriteFactory = (IDWriteFactory)dwObj;
        CreateTextFormat();

        // 5. Swap chain for composition + the D2D render target bitmap.
        CreateSwapChain(clientSize);
        CreateBitmapTarget();

        // 6. DirectComposition visual tree: target(hwnd) -> root -> content(swapchain).
        _dcompDevice.CreateTargetForHwnd((HWND)hwnd, true, out _dcompTarget);
        _dcompDevice.CreateVisual(out _rootVisual);
        _dcompDevice.CreateVisual(out _contentVisual);
        _contentVisual.SetContent(_swapChain);
        _rootVisual.AddVisual(_contentVisual, true, null);
        _dcompTarget.SetRoot(_rootVisual);
        _dcompDevice.Commit();
    }

    private static ID3D11Device CreateD3DDevice()
    {
        ReadOnlySpan<D3D_FEATURE_LEVEL> levels =
        [
            D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
            D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
        ];
        const D3D11_CREATE_DEVICE_FLAG flags = D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        const uint sdkVersion = 7; // D3D11_SDK_VERSION

        HRESULT hr = PInvoke.D3D11CreateDevice(
            null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, default, flags,
            levels, sdkVersion, out ID3D11Device device, out ID3D11DeviceContext _);

        if (hr.Failed)
        {
            hr = PInvoke.D3D11CreateDevice(
                null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_WARP, default, flags,
                levels, sdkVersion, out device, out ID3D11DeviceContext _);
        }

        hr.ThrowOnFailure();
        return device;
    }

    private void CreateSwapChain(Size clientSize)
    {
        var desc = new DXGI_SWAP_CHAIN_DESC1
        {
            Width = (uint)Math.Max(1, clientSize.Width),
            Height = (uint)Math.Max(1, clientSize.Height),
            Format = BackBufferFormat,
            Stereo = false,
            SampleDesc = new DXGI_SAMPLE_DESC { Count = 1, Quality = 0 },
            BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
            BufferCount = 2,
            Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH,
            SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
            AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_IGNORE,
        };

        _dxgiFactory2!.CreateSwapChainForComposition(_d3dDevice!, &desc, null, out _swapChain);
    }

    private void CreateBitmapTarget()
    {
        Guid surfaceIid = typeof(IDXGISurface).GUID;
        _swapChain!.GetBuffer(0, &surfaceIid, out object surfaceObj);
        var surface = (IDXGISurface)surfaceObj;

        var props = new D2D1_BITMAP_PROPERTIES1_unmanaged
        {
            pixelFormat = new D2D1_PIXEL_FORMAT
            {
                format = BackBufferFormat,
                alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_IGNORE,
            },
            dpiX = 96f,
            dpiY = 96f,
            bitmapOptions = D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
            colorContext = null,
        };

        _d2dContext!.CreateBitmapFromDxgiSurface(surface, &props, out _d2dBitmap);
        _d2dContext.SetTarget(_d2dBitmap);
        Marshal.ReleaseComObject(surface);
    }

    private void CreateTextFormat()
    {
        fixed (char* family = "Consolas")
        fixed (char* locale = "en-us")
        {
            _dwriteFactory!.CreateTextFormat(
                new PCWSTR(family), null,
                DWRITE_FONT_WEIGHT.DWRITE_FONT_WEIGHT_BOLD,
                DWRITE_FONT_STYLE.DWRITE_FONT_STYLE_NORMAL,
                DWRITE_FONT_STRETCH.DWRITE_FONT_STRETCH_NORMAL,
                48f, new PCWSTR(locale), out _textFormat);
        }

        _textFormat.SetTextAlignment(DWRITE_TEXT_ALIGNMENT.DWRITE_TEXT_ALIGNMENT_CENTER);
        _textFormat.SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT.DWRITE_PARAGRAPH_ALIGNMENT_NEAR);
    }

    public void Resize(Size clientSize)
    {
        if (_swapChain is null || clientSize.IsEmpty)
            return;

        _clientSize = clientSize;

        _d2dContext!.SetTarget(null);
        if (_d2dBitmap is not null)
        {
            Marshal.ReleaseComObject(_d2dBitmap);
            _d2dBitmap = null;
        }

        _swapChain.ResizeBuffers(2, (uint)clientSize.Width, (uint)clientSize.Height, BackBufferFormat, 0);
        CreateBitmapTarget();
    }

    public void Render(PongGame game)
    {
        if (_d2dContext is null || _swapChain is null)
            return;

        _d2dContext.BeginDraw();

        var background = new D2D1_COLOR_F { r = 0.05f, g = 0.05f, b = 0.10f, a = 1f };
        _d2dContext.Clear(&background);

        var white = new D2D1_COLOR_F { r = 1f, g = 1f, b = 1f, a = 1f };
        _d2dContext.CreateSolidColorBrush(&white, null, out ID2D1SolidColorBrush brush);

        var leftPaddle = new D2D_RECT_F
        {
            left = 0,
            top = game.LeftPaddleY,
            right = PongConfig.PADDLE_WIDTH,
            bottom = game.LeftPaddleY + PongConfig.PADDLE_HEIGHT,
        };
        _d2dContext.FillRectangle(&leftPaddle, brush);

        var rightPaddle = new D2D_RECT_F
        {
            left = _clientSize.Width - PongConfig.PADDLE_WIDTH,
            top = game.RightPaddleY,
            right = _clientSize.Width,
            bottom = game.RightPaddleY + PongConfig.PADDLE_HEIGHT,
        };
        _d2dContext.FillRectangle(&rightPaddle, brush);

        float radius = PongConfig.BALL_SIZE / 2f;
        var ball = new D2D1_ELLIPSE
        {
            point = new D2D_POINT_2F { x = game.BallX + radius, y = game.BallY + radius },
            radiusX = radius,
            radiusY = radius,
        };
        _d2dContext.FillEllipse(&ball, brush);

        if (_textFormat is not null)
        {
            string score = $"{game.LeftScore}  :  {game.RightScore}";
            var layout = new D2D_RECT_F { left = 0, top = 20, right = _clientSize.Width, bottom = 100 };
            fixed (char* text = score)
            {
                _d2dContext.DrawText(
                    new PCWSTR(text), (uint)score.Length, _textFormat, &layout, brush,
                    D2D1_DRAW_TEXT_OPTIONS.D2D1_DRAW_TEXT_OPTIONS_NONE,
                    DWRITE_MEASURING_MODE.DWRITE_MEASURING_MODE_NATURAL);
            }
        }

        Marshal.ReleaseComObject(brush);

        _d2dContext.EndDraw(null, null);
        _swapChain.Present(1, 0);
    }

    public void Dispose()
    {
        void Release(object? com)
        {
            if (com is not null && Marshal.IsComObject(com))
                Marshal.ReleaseComObject(com);
        }

        Release(_textFormat);
        Release(_dwriteFactory);
        Release(_d2dBitmap);
        Release(_contentVisual);
        Release(_rootVisual);
        Release(_dcompTarget);
        Release(_dcompDevice);
        Release(_swapChain);
        Release(_d2dContext);
        Release(_d2dDevice);
        Release(_d2dFactory);
        Release(_dxgiFactory2);
        Release(_dxgiDevice);
        Release(_d3dDevice);

        _textFormat = null;
        _dwriteFactory = null;
        _d2dBitmap = null;
        _contentVisual = null;
        _rootVisual = null;
        _dcompTarget = null;
        _dcompDevice = null;
        _swapChain = null;
        _d2dContext = null;
        _d2dDevice = null;
        _d2dFactory = null;
        _dxgiFactory2 = null;
        _dxgiDevice = null;
        _d3dDevice = null;
    }
}
