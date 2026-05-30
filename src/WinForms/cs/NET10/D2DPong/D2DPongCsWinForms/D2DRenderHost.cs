using System.Runtime.InteropServices;
using Windows.Win32;

namespace WinFormsPong.DComp;

// =========================================================================
// DCOMP/D2D RENDER HOST
// =========================================================================
internal class D2DRenderHost : IDisposable
{
    private IntPtr _d3dDevice = IntPtr.Zero;
    private IntPtr _dxgiDevice = IntPtr.Zero;
    private IntPtr _dxgiFactory2 = IntPtr.Zero;
    private IntPtr _swapChain = IntPtr.Zero;
    private IntPtr _d2dFactory = IntPtr.Zero;
    private IntPtr _d2dDevice = IntPtr.Zero;
    private IntPtr _d2dContext = IntPtr.Zero;
    private IntPtr _d2dBitmap = IntPtr.Zero;
    private IntPtr _dcompDevice = IntPtr.Zero;
    private IntPtr _dcompTarget = IntPtr.Zero;
    private IntPtr _rootVisual = IntPtr.Zero;
    private IntPtr _contentVisual = IntPtr.Zero;
    private IntPtr _dwriteFactory = IntPtr.Zero;
    private IntPtr _textFormat = IntPtr.Zero;
    private IntPtr _textLayout = IntPtr.Zero;

    public bool TreeChanged { get; set; }

    public void Initialize(IntPtr hwnd, Size clientSize)
    {
        // 1. D3D11CreateDevice — HW first, then WARP fallback
        var driverTypes = new[] { D2DGuids.D3D_DRIVER_TYPE_HARDWARE, 1 }; // 1 = REF (WARP fallback in D3D11CreateDevice context)
        var featureLevels = new[] { D2DGuids.D3D_FEATURE_LEVEL_11_0 };

        var hr = PInvoke.D3D11CreateDevice(
            IntPtr.Zero, driverTypes[0], IntPtr.Zero, 0, D2DGuids.D3D_FEATURE_LEVEL_11_0, (uint)featureLevels.Length,
            11, out IntPtr immediateContext, IntPtr.Zero, out _d3dDevice);
        if (hr < 0) throw new InvalidOperationException($"D3D11CreateDevice failed: 0x{hr:X}");

        // 2. QI IDXGIDevice
        PInvoke.ID3D11Device_QueryInterface(_d3dDevice, D2DGuids.IID_IDXGIDevice, out _dxgiDevice);

        // 3. CreateDXGIFactory2 → IDXGIFactory2
        PInvoke.CreateDXGIFactory2(0, D2DGuids.IID_IDXGIFactory2, out _dxgiFactory2);

        // 4. D2D1CreateFactory (MULTI_THREADED)
        PInvoke.D2D1CreateFactory(
            D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED,
            ref D2DGuids.IID_ID2D1Factory, IntPtr.Zero, out _d2dFactory);

        // 5. ID2D1Factory1::CreateDevice(dxgiDevice) → ID2D1Device
        PInvoke.ID2D1Factory_CreateDevice(_d2dFactory, _dxgiDevice, out _d2dDevice);

        // 6. DCompositionCreateDevice(dxgiDevice, …) → IDCompositionDevice
        PInvoke.DCompositionCreateDevice(IntPtr.Zero, D2DGuids.IID_IDCompositionDevice, out _dcompDevice);

        // 7. DWriteCreateFactory(ISOLATED)
        PInvoke.DWriteCreateFactory(0, D2DGuids.IID_IDWriteFactory, out _dwriteFactory);

        // === Per-host wiring ===
        _d2dDevice.QueryInterface(D2DGuids.IID_ID2D1DeviceContext, out _d2dContext);

        // 2. IDXGIFactory2::CreateSwapChainForComposition
        var swapDesc = new DXGI_SWAP_CHAIN_DESC1
        {
            Width = clientSize.Width,
            Height = clientSize.Height,
            Format = DXGI_FORMAT.B8G8R8A8_UNORM,
            Stereo = false,
            SampleDesc = new DXGI_SAMPLE_DESC { Count = 1, Quality = 0 },
            BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
            BufferCount = 2,
            Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH,
            SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
            AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_IGNORE,
            Flags = (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH
        };
        PInvoke.IDXGIFactory2_CreateSwapChainForComposition(_dxgiFactory2, _dcompDevice, ref swapDesc, out _swapChain);

        // 3. IDCompositionDevice::CreateTargetForHwnd
        PInvoke.IDCompositionDevice_CreateTargetForHwnd(_dcompDevice, hwnd, true, out _dcompTarget);

        // 4. CreateVisual (root + content visual)
        PInvoke.IDCompositionDevice_CreateVisual(_dcompDevice, out _rootVisual);
        PInvoke.IDCompositionDevice_CreateVisual(_dcompDevice, out _contentVisual);

        // 5. visual.SetContent(swapChain), root.AddVisual(visual), target.SetRoot(root)
        PInvoke.IDCompositionVisual_SetContent(_contentVisual, _swapChain);
        PInvoke.IDCompositionVisual_AddVisual(_rootVisual, _contentVisual, true, IntPtr.Zero);
        PInvoke.IDCompositionTarget_SetRoot(_dcompTarget, _rootVisual);
        TreeChanged = true;

        // 6. IDCompositionDevice::Commit()
        PInvoke.IDCompositionDevice_Commit(_dcompDevice);

        // 7. swapChain.GetBuffer(0) → IDXGISurface → deviceContext.CreateBitmapFromDxgiSurface → ID2D1Bitmap1
        PInvoke.IDXGISwapChain_GetBuffer(_swapChain, 0, D2DGuids.IID_ID2D1Bitmap1, out IntPtr surface);
        PInvoke.ID2D1DeviceContext_CreateBitmapFromDxgiSurface(_d2dContext, surface, IntPtr.Zero, out _d2dBitmap);
        Marshal.Release(surface);

        // 8. deviceContext.SetTarget(bitmap)
        PInvoke.ID2D1DeviceContext_SetTarget(_d2dContext, _d2dBitmap);

        // DirectWrite initialization
        var width = (float)clientSize.Width;
        var height = (float)clientSize.Height;
        PInvoke.IDWriteFactory_CreateTextFormat(_dwriteFactory, "Arial", IntPtr.Zero, 700, 0, "en-US", out _textFormat);
        PInvoke.IDWriteTextFormat_SetTextAlignment(_textFormat, 1); // DWRITE_TEXT_ALIGNMENT_LEADING
        PInvoke.IDWriteTextFormat_SetParagraphAlignment(_textFormat, 1); // DWRITE_PARAGRAPH_ALIGNMENT_NEAR
        PInvoke.IDWriteTextFormat_SetWordWrapping(_textFormat, 0); // DWRITE_WORD_WRAPPING_NO_WRAP
        PInvoke.IDWriteFactory_CreateTextLayout(_dwriteFactory, "00 : 00", 10, _textFormat, width, height, out _textLayout);
        PInvoke.IDWriteTextLayout_SetDrawTextParams(_textLayout, null, 0);
        PInvoke.IDWriteTextLayout_SetFontSize(_textLayout, 64f, new uint[] { 0, 2 }, new uint[] { 2 });
    }

    public void Resize(Size clientSize)
    {
        if (clientSize.IsEmpty) return;

        // Release old bitmap & swapchain buffers
        if (_d2dBitmap != IntPtr.Zero) { Marshal.Release(_d2dBitmap); _d2dBitmap = IntPtr.Zero; }
        PInvoke.IDXGISwapChain_ResizeBuffers(_swapChain, 2, clientSize.Width, clientSize.Height, DXGI_FORMAT.B8G8R8A8_UNORM, 0);

        // Get new buffer
        PInvoke.IDXGISwapChain_GetBuffer(_swapChain, 0, D2DGuids.IID_ID2D1Bitmap1, out IntPtr surface);
        PInvoke.ID2D1DeviceContext_CreateBitmapFromDxgiSurface(_d2dContext, surface, IntPtr.Zero, out _d2dBitmap);
        Marshal.Release(surface);
        PInvoke.ID2D1DeviceContext_SetTarget(_d2dContext, _d2dBitmap);
    }

    public void Render(PongGame game)
    {
        // Per-frame: SetTarget (already done, but ensure context)
        PInvoke.ID2D1DeviceContext_SetTarget(_d2dContext, _d2dBitmap);
        PInvoke.ID2D1DeviceContext_BeginDraw(_d2dContext);

        // Clear
        var clearColor = new D2D1_COLOR_F { r = 0.05f, g = 0.05f, b = 0.1f, a = 1.0f };
        PInvoke.ID2D1DeviceContext_Clear(_d2dContext, ref clearColor);

        // Draw Paddles & Ball
        var white = new D2D1_COLOR_F { r = 1f, g = 1f, b = 1f, a = 1f };
        var pen = PInvoke.ID2D1DeviceContext_CreateSolidColorBrush(_d2dContext, ref white, out var brush);

        var leftRect = new D2D1_RECT_F { left = 0, top = game.LeftPaddleY, right = PongConfig.PADDLE_WIDTH, bottom = game.LeftPaddleY + PongConfig.PADDLE_HEIGHT };
        PInvoke.ID2D1DeviceContext_FillRectangle(_d2dContext, ref leftRect, brush);

        var rightRect = new D2D1_RECT_F { left = PongConfig.WINDOW_WIDTH - PongConfig.PADDLE_WIDTH, top = game.RightPaddleY, right = PongConfig.WINDOW_WIDTH, bottom = game.RightPaddleY + PongConfig.PADDLE_HEIGHT };
        PInvoke.ID2D1DeviceContext_FillRectangle(_d2dContext, ref rightRect, brush);

        var ballRect = new D2D1_RECT_F { left = game.BallX, top = game.BallY, right = game.BallX + PongConfig.BALL_SIZE, bottom = game.BallY + PongConfig.BALL_SIZE };
        PInvoke.ID2D1DeviceContext_FillEllipse(_d2dContext, new D2D1_ELLIPSE { point = new D2D1_POINT_2F { x = ballRect.left + ballRect.right / 2, y = ballRect.top + ballRect.bottom / 2 }, radiusX = ballRect.right - ballRect.left, radiusY = ballRect.bottom - ballRect.top }, brush);

        // Draw Score with DirectWrite
        var scoreStr = $"{game.LeftScore,2} : {game.RightScore,2}";
        PInvoke.IDWriteTextLayout_SetText(_textLayout, scoreStr, (uint)scoreStr.Length);
        PInvoke.IDWriteTextLayout_SetFontSize(_textLayout, 48f, new uint[] { 0, scoreStr.Length }, new uint[] { 2 });

        var textBrush = PInvoke.ID2D1DeviceContext_CreateSolidColorBrush(_d2dContext, ref white, out var textBrushPtr);
        var origin = new D2D1_POINT_2F { x = PongConfig.WINDOW_WIDTH / 2 - 50, y = PongConfig.WINDOW_HEIGHT / 2 - 25 };
        PInvoke.ID2D1DeviceContext_DrawText(_d2dContext, scoreStr, (uint)scoreStr.Length, _textLayout, ref origin, textBrush);
        Marshal.Release(textBrushPtr);

        PInvoke.ID2D1DeviceContext_EndDraw(_d2dContext, out _, out _);

        // Present
        PInvoke.IDXGISwapChain1_Present1(_swapChain, 1, 0, IntPtr.Zero, IntPtr.Zero);

        // DComp Commit only when tree changed
        if (TreeChanged)
        {
            PInvoke.IDCompositionDevice_Commit(_dcompDevice);
            TreeChanged = false;
        }
    }

    public void Dispose()
    {
        if (_d2dBitmap != IntPtr.Zero) Marshal.Release(_d2dBitmap);
        if (_d2dContext != IntPtr.Zero) Marshal.Release(_d2dContext);
        if (_d2dDevice != IntPtr.Zero) Marshal.Release(_d2dDevice);
        if (_d2dFactory != IntPtr.Zero) Marshal.Release(_d2dFactory);
        if (_dcompDevice != IntPtr.Zero) Marshal.Release(_dcompDevice);
        if (_dcompTarget != IntPtr.Zero) Marshal.Release(_dcompTarget);
        if (_rootVisual != IntPtr.Zero) Marshal.Release(_rootVisual);
        if (_contentVisual != IntPtr.Zero) Marshal.Release(_contentVisual);
        if (_swapChain != IntPtr.Zero) Marshal.Release(_swapChain);
        if (_dxgiFactory2 != IntPtr.Zero) Marshal.Release(_dxgiFactory2);
        if (_dxgiDevice != IntPtr.Zero) Marshal.Release(_dxgiDevice);
        if (_d3dDevice != IntPtr.Zero) Marshal.Release(_d3dDevice);
        if (_textLayout != IntPtr.Zero) Marshal.Release(_textLayout);
        if (_textFormat != IntPtr.Zero) Marshal.Release(_textFormat);
        if (_dwriteFactory != IntPtr.Zero) Marshal.Release(_dwriteFactory);
    }
}
