using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;
using WinRT;

namespace CameraControlDemo;

/// <summary>
///  Presents camera frames through Direct2D and a flip-model DXGI swap chain.
/// </summary>
/// <remarks>
///  <para>
///   A GPU-backed camera frame already owns an <c>IDXGISurface</c>. The renderer
///   discovers that surface's D3D11 device and creates Direct2D and the swap chain on
///   the same device. Direct2D can then wrap and scale the camera surface without
///   copying its pixels through managed memory.
///  </para>
///  <para>
///   Some cameras expose only a <see cref="SoftwareBitmap"/>. For those frames the
///   renderer creates its own D3D11 device and updates one reusable Direct2D bitmap
///   before drawing it through the same swap-chain path. The swap chain targets the
///   child control HWND directly and is composed by the Desktop Window Manager.
///   Releasing it reveals normal WinForms status and error painting.
///  </para>
///  <para>
///   Rendering, resizing, and disposal are serialized because frame callbacks do not
///   run on the UI thread. A non-blocking gate drops a frame when the previous one is
///   still rendering, which favors a current preview over queued latency.
///  </para>
/// </remarks>
internal sealed unsafe class DirectXCameraRenderer : IDisposable
{
    /// <summary>
    ///  Pixel format shared by BGRA camera frames and the swap-chain back buffers.
    /// </summary>
    private const DXGI_FORMAT BackBufferFormat =
        DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;

    /// <summary>
    ///  HRESULT returned when Direct2D requires its render target to be recreated.
    /// </summary>
    private const int D2DERR_RECREATE_TARGET = unchecked((int)0x8899000C);

    /// <summary>
    ///  HRESULT returned when the graphics device was removed.
    /// </summary>
    private const int DXGI_ERROR_DEVICE_REMOVED = unchecked((int)0x887A0005);

    /// <summary>
    ///  HRESULT returned when the graphics device was reset.
    /// </summary>
    private const int DXGI_ERROR_DEVICE_RESET = unchecked((int)0x887A0007);

    /// <summary>
    ///  Serializes all native graphics object access.
    /// </summary>
    private readonly Lock _sync = new();

    /// <summary>
    ///  HWND whose client area receives the composition target.
    /// </summary>
    private readonly nint _windowHandle;

    /// <summary>
    ///  Non-zero while a frame owns the non-blocking render gate.
    /// </summary>
    private int _rendering;

    /// <summary>
    ///  Most recent client size requested by the control.
    /// </summary>
    private Size _requestedSize;

    /// <summary>
    ///  Current swap-chain size.
    /// </summary>
    private Size _swapChainSize;

    /// <summary>
    ///  Indicates whether the current device came from a camera surface.
    /// </summary>
    private bool _usesCameraDevice;

    /// <summary>
    ///  Indicates that disposal has permanently stopped this renderer.
    /// </summary>
    private bool _disposed;

    /// <summary>
    ///  D3D11 device used by both Direct2D and DXGI.
    /// </summary>
    private ID3D11Device? _d3dDevice;

    /// <summary>
    ///  Immediate D3D11 context retained with an internally created fallback device.
    /// </summary>
    private ID3D11DeviceContext? _d3dContext;

    /// <summary>
    ///  DXGI view of <see cref="_d3dDevice"/>.
    /// </summary>
    private IDXGIDevice? _dxgiDevice;

    /// <summary>
    ///  Factory that creates the composition swap chain.
    /// </summary>
    private IDXGIFactory2? _dxgiFactory;

    /// <summary>
    ///  Flip-model composition swap chain presented by this renderer.
    /// </summary>
    private IDXGISwapChain1? _swapChain;

    /// <summary>
    ///  Multi-threaded Direct2D factory.
    /// </summary>
    private ID2D1Factory1? _d2dFactory;

    /// <summary>
    ///  Direct2D device layered on the D3D11 device.
    /// </summary>
    private ID2D1Device? _d2dDevice;

    /// <summary>
    ///  Direct2D device context used to clear, scale, and draw each frame.
    /// </summary>
    private ID2D1DeviceContext? _d2dContext;

    /// <summary>
    ///  Direct2D target bitmap wrapping the current swap-chain back buffer.
    /// </summary>
    private ID2D1Bitmap1? _targetBitmap;

    /// <summary>
    ///  Reusable source bitmap used only by the software-frame fallback.
    /// </summary>
    private ID2D1Bitmap? _softwareBitmap;

    /// <summary>
    ///  Pixel dimensions of <see cref="_softwareBitmap"/>.
    /// </summary>
    private Size _softwareBitmapSize;

    /// <summary>
    ///  Initializes a renderer for a created WinForms control handle.
    /// </summary>
    /// <param name="windowHandle">The child control HWND.</param>
    /// <param name="clientSize">The initial client size in pixels.</param>
    public DirectXCameraRenderer(nint windowHandle, Size clientSize)
    {
        if (windowHandle == 0)
        {
            throw new ArgumentException("A created control handle is required.", nameof(windowHandle));
        }

        _windowHandle = windowHandle;
        _requestedSize = clientSize;
    }

    /// <summary>
    ///  Updates the client size that will be applied before the next presentation.
    /// </summary>
    /// <param name="clientSize">The control's client size in pixels.</param>
    public void Resize(Size clientSize)
    {
        lock (_sync)
        {
            _requestedSize = clientSize;
        }
    }

    /// <summary>
    ///  Releases the HWND swap chain so WinForms status painting is visible.
    /// </summary>
    public void Hide()
    {
        lock (_sync)
        {
            if (_disposed || _swapChain is null)
            {
                return;
            }

            ReleaseGraphicsResources();
        }
    }

    /// <summary>
    ///  Attempts to present a camera frame without waiting for an earlier frame.
    /// </summary>
    /// <param name="surface">A GPU-backed camera surface, when available.</param>
    /// <param name="softwareBitmap">A CPU-backed camera bitmap used as fallback.</param>
    /// <param name="keepAspectRatio">
    ///  Whether to center and letterbox the frame instead of drawing it 1:1.
    /// </param>
    /// <param name="backgroundColor">The color used outside the destination rectangle.</param>
    /// <param name="frameSize">Receives the presented source dimensions.</param>
    /// <returns>
    ///  <see langword="true"/> after a successful present; otherwise
    ///  <see langword="false"/> when the frame was dropped or the device will be
    ///  recreated on the next frame.
    /// </returns>
    public bool TryPresent(
        IDirect3DSurface? surface,
        SoftwareBitmap? softwareBitmap,
        bool keepAspectRatio,
        Color backgroundColor,
        out Size frameSize)
    {
        frameSize = Size.Empty;
        if (Interlocked.CompareExchange(ref _rendering, 1, 0) != 0)
        {
            return false;
        }

        try
        {
            lock (_sync)
            {
                if (_disposed
                    || _requestedSize.Width <= 0
                    || _requestedSize.Height <= 0)
                {
                    return false;
                }

                try
                {
                    if (surface is not null)
                    {
                        frameSize = PresentGpuFrame(
                            surface,
                            keepAspectRatio,
                            backgroundColor);
                    }
                    else if (softwareBitmap is not null)
                    {
                        frameSize = PresentSoftwareFrame(
                            softwareBitmap,
                            keepAspectRatio,
                            backgroundColor);
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                catch (COMException ex) when (IsDeviceLoss(ex.HResult))
                {
                    ReleaseGraphicsResources();
                    return false;
                }
            }
        }
        finally
        {
            Interlocked.Exchange(ref _rendering, 0);
        }
    }

    /// <summary>
    ///  Presents a camera frame already stored in a Direct3D texture.
    /// </summary>
    /// <param name="surface">The WinRT wrapper around the camera's DXGI surface.</param>
    /// <param name="keepAspectRatio">Whether to letterbox the frame.</param>
    /// <param name="backgroundColor">The letterbox color.</param>
    /// <returns>The source frame dimensions.</returns>
    private Size PresentGpuFrame(
        IDirect3DSurface surface,
        bool keepAspectRatio,
        Color backgroundColor)
    {
        using NativeSurface nativeSurface = NativeSurface.FromWinRtSurface(surface);
        ID3D11Device cameraDevice = nativeSurface.GetDevice();
        bool deviceOwnedByRenderer = false;

        try
        {
            if (!_usesCameraDevice || !IsSameComObject(_d3dDevice, cameraDevice))
            {
                ReleaseGraphicsResources();
                Initialize(cameraDevice, usesCameraDevice: true);
                deviceOwnedByRenderer = true;
            }

            EnsureSwapChainSize();

            var properties = CreateBitmapProperties(
                D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_NONE);

            _d2dContext!.CreateBitmapFromDxgiSurface(
                nativeSurface.Surface,
                &properties,
                out ID2D1Bitmap1 sourceBitmap);

            try
            {
                D2D_SIZE_U pixelSize = sourceBitmap.GetPixelSize();
                Size sourceSize = new((int)pixelSize.width, (int)pixelSize.height);
                DrawAndPresent(sourceBitmap, sourceSize, keepAspectRatio, backgroundColor);
                return sourceSize;
            }
            finally
            {
                ReleaseComObject(sourceBitmap);
            }
        }
        finally
        {
            if (!deviceOwnedByRenderer)
            {
                ReleaseComObject(cameraDevice);
            }
        }
    }

    /// <summary>
    ///  Uploads and presents a CPU-backed camera frame.
    /// </summary>
    /// <param name="bitmap">The BGRA8 software bitmap.</param>
    /// <param name="keepAspectRatio">Whether to letterbox the frame.</param>
    /// <param name="backgroundColor">The letterbox color.</param>
    /// <returns>The source frame dimensions.</returns>
    private Size PresentSoftwareFrame(
        SoftwareBitmap bitmap,
        bool keepAspectRatio,
        Color backgroundColor)
    {
        if (_d3dDevice is null)
        {
            Initialize(CreateFallbackDevice(), usesCameraDevice: false);
        }

        EnsureSwapChainSize();

        using BitmapBuffer buffer = bitmap.LockBuffer(BitmapBufferAccessMode.Read);
        using Windows.Foundation.IMemoryBufferReference reference = buffer.CreateReference();

        IMemoryBufferByteAccess byteAccess = reference.As<IMemoryBufferByteAccess>();
        byteAccess.GetBuffer(out byte* rawBuffer, out _);

        BitmapPlaneDescription plane = buffer.GetPlaneDescription(0);
        Size sourceSize = new(plane.Width, plane.Height);

        EnsureSoftwareBitmap(sourceSize);

        byte* source = rawBuffer + plane.StartIndex;
        _softwareBitmap!.CopyFromMemory(null, source, (uint)plane.Stride);

        DrawAndPresent(
            _softwareBitmap,
            sourceSize,
            keepAspectRatio,
            backgroundColor);

        return sourceSize;
    }

    /// <summary>
    ///  Creates a BGRA-capable D3D11 hardware device, falling back to WARP.
    /// </summary>
    /// <returns>The device used for software-frame presentation.</returns>
    private ID3D11Device CreateFallbackDevice()
    {
        ReadOnlySpan<D3D_FEATURE_LEVEL> levels =
        [
            D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
            D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0
        ];

        const D3D11_CREATE_DEVICE_FLAG flags =
            D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;

        HRESULT result = PInvoke.D3D11CreateDevice(
            null,
            D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
            default,
            flags,
            levels,
            PInvoke.D3D11_SDK_VERSION,
            out ID3D11Device device,
            out ID3D11DeviceContext context);

        if (result.Failed)
        {
            result = PInvoke.D3D11CreateDevice(
                null,
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_WARP,
                default,
                flags,
                levels,
                PInvoke.D3D11_SDK_VERSION,
                out device,
                out context);
        }

        result.ThrowOnFailure();
        _d3dContext = context;
        return device;
    }

    /// <summary>
    ///  Builds the Direct2D, DXGI, and HWND swap-chain object graph.
    /// </summary>
    /// <param name="device">The D3D11 device at the root of the graph.</param>
    /// <param name="usesCameraDevice">
    ///  Whether <paramref name="device"/> belongs to a GPU camera surface.
    /// </param>
    private void Initialize(ID3D11Device device, bool usesCameraDevice)
    {
        _d3dDevice = device;
        _usesCameraDevice = usesCameraDevice;

        try
        {
            _dxgiDevice = (IDXGIDevice)device;

            PInvoke.CreateDXGIFactory2(
                0,
                typeof(IDXGIFactory2).GUID,
                out object factoryObject).ThrowOnFailure();
            _dxgiFactory = (IDXGIFactory2)factoryObject;

            PInvoke.D2D1CreateFactory(
                D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED,
                typeof(ID2D1Factory1).GUID,
                null,
                out object d2dFactoryObject).ThrowOnFailure();
            _d2dFactory = (ID2D1Factory1)d2dFactoryObject;
            _d2dFactory.CreateDevice(_dxgiDevice, out _d2dDevice);
            _d2dDevice.CreateDeviceContext(
                D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
                out _d2dContext);

            CreateSwapChain();
        }
        catch
        {
            ReleaseGraphicsResources();
            throw;
        }
    }

    /// <summary>
    ///  Creates the flip-model composition swap chain and its Direct2D target.
    /// </summary>
    private void CreateSwapChain()
    {
        Size size = NormalizeSize(_requestedSize);

        var description = new DXGI_SWAP_CHAIN_DESC1
        {
            Width = (uint)size.Width,
            Height = (uint)size.Height,
            Format = BackBufferFormat,
            Stereo = false,
            SampleDesc = new DXGI_SAMPLE_DESC { Count = 1, Quality = 0 },
            BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
            BufferCount = 2,
            Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH,
            SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
            AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_IGNORE
        };

        _dxgiFactory!.CreateSwapChainForHwnd(
            _d3dDevice!,
            (HWND)_windowHandle,
            &description,
            null,
            null,
            out _swapChain);

        _swapChainSize = size;
        CreateTargetBitmap();
    }

    /// <summary>
    ///  Wraps the current swap-chain back buffer as the Direct2D render target.
    /// </summary>
    private void CreateTargetBitmap()
    {
        Guid surfaceIdentifier = typeof(IDXGISurface).GUID;
        _swapChain!.GetBuffer(0, &surfaceIdentifier, out object surfaceObject);
        var surface = (IDXGISurface)surfaceObject;

        try
        {
            D2D1_BITMAP_PROPERTIES1_unmanaged properties = CreateBitmapProperties(
                D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_TARGET
                    | D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_CANNOT_DRAW);

            _d2dContext!.CreateBitmapFromDxgiSurface(
                surface,
                &properties,
                out _targetBitmap);
            _d2dContext.SetTarget(_targetBitmap);
        }
        finally
        {
            ReleaseComObject(surface);
        }
    }

    /// <summary>
    ///  Resizes the swap chain when the control's requested size has changed.
    /// </summary>
    private void EnsureSwapChainSize()
    {
        Size requestedSize = NormalizeSize(_requestedSize);

        if (requestedSize == _swapChainSize)
        {
            return;
        }

        _d2dContext!.SetTarget(null);
        ReleaseComObject(_targetBitmap);
        _targetBitmap = null;

        _swapChain!.ResizeBuffers(
            2,
            (uint)requestedSize.Width,
            (uint)requestedSize.Height,
            BackBufferFormat,
            0);

        _swapChainSize = requestedSize;
        CreateTargetBitmap();
    }

    /// <summary>
    ///  Creates or resizes the reusable CPU-upload bitmap.
    /// </summary>
    /// <param name="size">Required source dimensions.</param>
    private void EnsureSoftwareBitmap(Size size)
    {
        if (_softwareBitmap is not null && _softwareBitmapSize == size)
        {
            return;
        }

        ReleaseComObject(_softwareBitmap);
        _softwareBitmap = null;

        var pixelSize = new D2D_SIZE_U
        {
            width = (uint)size.Width,
            height = (uint)size.Height
        };

        var properties = new D2D1_BITMAP_PROPERTIES
        {
            pixelFormat = new D2D1_PIXEL_FORMAT
            {
                format = BackBufferFormat,
                alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_IGNORE
            },
            dpiX = 96f,
            dpiY = 96f
        };

        _d2dContext!.CreateBitmap(
            pixelSize,
            null,
            0,
            &properties,
            out _softwareBitmap);
        _softwareBitmapSize = size;
    }

    /// <summary>
    ///  Clears, draws, and presents one source bitmap.
    /// </summary>
    /// <param name="source">The Direct2D source bitmap.</param>
    /// <param name="sourceSize">The source dimensions in pixels.</param>
    /// <param name="keepAspectRatio">Whether to letterbox the source.</param>
    /// <param name="backgroundColor">The clear color.</param>
    private void DrawAndPresent(
        ID2D1Bitmap source,
        Size sourceSize,
        bool keepAspectRatio,
        Color backgroundColor)
    {
        D2D_RECT_F destination = GetDestinationRectangle(
            sourceSize,
            _swapChainSize,
            keepAspectRatio);

        var clearColor = new D2D1_COLOR_F
        {
            r = backgroundColor.R / 255f,
            g = backgroundColor.G / 255f,
            b = backgroundColor.B / 255f,
            a = 1f
        };

        _d2dContext!.BeginDraw();
        _d2dContext.Clear(&clearColor);
        _d2dContext.DrawBitmap(
            source,
            &destination,
            1f,
            keepAspectRatio
                ? D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR
                : D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_NEAREST_NEIGHBOR,
            null);

        _d2dContext.EndDraw(null, null);
        _swapChain!.Present(1, 0);
    }

    /// <summary>
    ///  Creates common Direct2D bitmap properties for BGRA pixel data.
    /// </summary>
    /// <param name="options">How Direct2D will use the bitmap.</param>
    /// <returns>Bitmap properties using pixel coordinates at 96 DPI.</returns>
    private static D2D1_BITMAP_PROPERTIES1_unmanaged CreateBitmapProperties(
        D2D1_BITMAP_OPTIONS options)
        => new()
        {
            pixelFormat = new D2D1_PIXEL_FORMAT
            {
                format = BackBufferFormat,
                alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_IGNORE
            },
            dpiX = 96f,
            dpiY = 96f,
            bitmapOptions = options,
            colorContext = null
        };

    /// <summary>
    ///  Computes either a centered letterbox rectangle or an unscaled origin rectangle.
    /// </summary>
    /// <param name="source">Source dimensions.</param>
    /// <param name="target">Target dimensions.</param>
    /// <param name="keepAspectRatio">Whether the source should be scaled to fit.</param>
    /// <returns>The Direct2D destination rectangle.</returns>
    private static D2D_RECT_F GetDestinationRectangle(
        Size source,
        Size target,
        bool keepAspectRatio)
    {
        if (!keepAspectRatio)
        {
            return new D2D_RECT_F
            {
                left = 0,
                top = 0,
                right = source.Width,
                bottom = source.Height
            };
        }

        double scale = Math.Min(
            target.Width / (double)source.Width,
            target.Height / (double)source.Height);

        float width = (float)(source.Width * scale);
        float height = (float)(source.Height * scale);
        float left = (target.Width - width) / 2f;
        float top = (target.Height - height) / 2f;

        return new D2D_RECT_F
        {
            left = left,
            top = top,
            right = left + width,
            bottom = top + height
        };
    }

    /// <summary>
    ///  Replaces empty dimensions with the smallest valid swap-chain size.
    /// </summary>
    /// <param name="size">The requested size.</param>
    /// <returns>A size whose dimensions are both at least one pixel.</returns>
    private static Size NormalizeSize(Size size)
        => new(Math.Max(1, size.Width), Math.Max(1, size.Height));

    /// <summary>
    ///  Compares COM identity rather than managed wrapper identity.
    /// </summary>
    /// <param name="left">The first COM object.</param>
    /// <param name="right">The second COM object.</param>
    /// <returns><see langword="true"/> when both wrappers represent one COM object.</returns>
    private static bool IsSameComObject(object? left, object right)
    {
        if (left is null)
        {
            return false;
        }

        nint leftIdentity = Marshal.GetIUnknownForObject(left);
        nint rightIdentity = Marshal.GetIUnknownForObject(right);

        try
        {
            return leftIdentity == rightIdentity;
        }
        finally
        {
            Marshal.Release(leftIdentity);
            Marshal.Release(rightIdentity);
        }
    }

    /// <summary>
    ///  Determines whether a failed draw should recreate the graphics object graph.
    /// </summary>
    /// <param name="hresult">The failing HRESULT.</param>
    /// <returns><see langword="true"/> for recoverable render-target/device loss.</returns>
    private static bool IsDeviceLoss(int hresult)
        => hresult is D2DERR_RECREATE_TARGET
            or DXGI_ERROR_DEVICE_REMOVED
            or DXGI_ERROR_DEVICE_RESET;

    /// <summary>
    ///  Releases one owned reference to a generated COM interface.
    /// </summary>
    /// <param name="value">The interface to release, if present.</param>
    private static void ReleaseComObject(object? value)
    {
        if (value is not null && Marshal.IsComObject(value))
        {
            Marshal.ReleaseComObject(value);
        }
    }

    /// <summary>
    ///  Releases the native graphics object graph in dependency-safe order.
    /// </summary>
    /// <remarks>
    ///  Teardown may be initiated by the WinForms UI thread. It intentionally makes no
    ///  typed calls on the MTA-created Direct2D context. Releasing the context first
    ///  drops its reference to the target bitmap without a cross-apartment
    ///  <c>SetTarget</c> call.
    /// </remarks>
    private void ReleaseGraphicsResources()
    {
        ReleaseComObject(_d2dContext);
        ReleaseComObject(_softwareBitmap);
        ReleaseComObject(_targetBitmap);
        ReleaseComObject(_swapChain);
        ReleaseComObject(_d2dDevice);
        ReleaseComObject(_d2dFactory);
        ReleaseComObject(_dxgiFactory);
        ReleaseComObject(_dxgiDevice);
        ReleaseComObject(_d3dContext);
        ReleaseComObject(_d3dDevice);

        _softwareBitmap = null;
        _targetBitmap = null;
        _swapChain = null;
        _d2dContext = null;
        _d2dDevice = null;
        _d2dFactory = null;
        _dxgiFactory = null;
        _dxgiDevice = null;
        _d3dContext = null;
        _d3dDevice = null;
        _softwareBitmapSize = Size.Empty;
        _swapChainSize = Size.Empty;
        _usesCameraDevice = false;
    }

    /// <summary>
    ///  Releases all native resources owned by the renderer.
    /// </summary>
    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            ReleaseGraphicsResources();
        }
    }

    /// <summary>
    ///  Owns the native DXGI surface reference obtained from a WinRT surface.
    /// </summary>
    private sealed class NativeSurface : IDisposable
    {
        /// <summary>
        ///  Initializes an owner for a native surface interface.
        /// </summary>
        /// <param name="surface">The owned surface interface.</param>
        private NativeSurface(IDXGISurface surface)
        {
            Surface = surface;
        }

        /// <summary>
        ///  Gets the wrapped native DXGI surface.
        /// </summary>
        public IDXGISurface Surface { get; }

        /// <summary>
        ///  Queries a WinRT surface for its underlying DXGI surface.
        /// </summary>
        /// <param name="surface">The projected WinRT surface.</param>
        /// <returns>An owner for the queried native surface.</returns>
        public static NativeSurface FromWinRtSurface(IDirect3DSurface surface)
        {
            IDirect3DDxgiInterfaceAccess access =
                surface.As<IDirect3DDxgiInterfaceAccess>();

            try
            {
                Guid identifier = typeof(IDXGISurface).GUID;
                int result = access.GetInterface(in identifier, out nint pointer);
                Marshal.ThrowExceptionForHR(result);

                try
                {
                    return new NativeSurface(
                        (IDXGISurface)Marshal.GetObjectForIUnknown(pointer));
                }
                finally
                {
                    Marshal.Release(pointer);
                }
            }
            finally
            {
                ReleaseComObject(access);
            }
        }

        /// <summary>
        ///  Gets the D3D11 device that owns <see cref="Surface"/>.
        /// </summary>
        /// <returns>The owning D3D11 device.</returns>
        public ID3D11Device GetDevice()
        {
            var texture = (ID3D11Texture2D)Surface;
            texture.GetDevice(out ID3D11Device device);
            return device;
        }

        /// <summary>
        ///  Releases the native surface interface.
        /// </summary>
        public void Dispose()
            => ReleaseComObject(Surface);
    }
}
