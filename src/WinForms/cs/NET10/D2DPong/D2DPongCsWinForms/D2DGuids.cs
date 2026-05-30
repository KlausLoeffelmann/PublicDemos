using System.Runtime.InteropServices;

namespace WinFormsPong.DComp;

// Provided GUIDs + necessary DComp/DXGI/D3D/DWrite additions
internal static class D2DGuids
{
    public const string IID_ID2D1Factory = "06152247-6f50-465a-9245-118bfd3b6007";
    public const string IID_ID2D1Device = "2cd90692-12e2-11dc-9fed-001143a055f9";
    public const string IID_ID2D1DeviceContext = "1c51bc64-de61-46fd-9899-63a5d8f03950"; // Alias for ID2D1DeviceContext
    public const string IID_ID2D1Bitmap1 = "a2296057-ea42-4099-983b-539fb6505426";
    public const string IID_ID2D1SolidColorBrush = "2cd906a9-12e2-11dc-9fed-001143a055f9";
    public const string IID_ID2D1RenderTarget = "2cd90694-12e2-11dc-9fed-001143a055f9";

    public const string IID_IDWriteFactory = "b859ee5a-d838-4b5b-a2e8-1adc7d93db48";
    public const string IID_IDWriteTextFormat = "9c906818-31d7-4fd3-a151-7c5e225db55a";
    public const string IID_IDWriteTextLayout = "53737037-6d14-410b-9bfe-0b182bb70961";

    public const string IID_IDCompositionDevice = "95813451-a711-474b-a46d-42f577122a44";
    public const string IID_IDCompositionTarget = "69f2c635-4f8d-4576-b737-0d15a9a47569";
    public const string IID_IDCompositionVisual = "6d446d52-2530-4e6d-b387-f3a8e82e23a7";
    public const string IID_IDCompositionTransform = "5b9532c8-e2e7-4f0d-873e-b1f1d1b0a0d8";
    public const string IID_IDCompositionTranslateTransform = "91a33014-5f22-4869-a0c5-82b8c7c3e2f3";

    public const string IID_IDXGIDevice = "54ec77fa-1377-44e6-8c32-88fd5f44c87c";
    public const string IID_IDXGIFactory2 = "50c83a1c-e074-4c89-8ae1-1804056817b9";
    public const string IID_IDXGISwapChain1 = "798a5d66-959e-42d9-87c7-15b395d4622f";
    public const string IID_IDXGIAdapter = "2411e7e5-12ac-4ccf-bd14-9798e84342ff";

    public const string IID_ID3D11Device = "db6f6ddb-ac77-4e88-8253-819df9bbf140";
    public const int D3D_DRIVER_TYPE_HARDWARE = 0;
    public const int D3D_FEATURE_LEVEL_11_0 = 0x0000;
}

// =========================================================================
// CSWIN32 HELPER TYPES (Required for compilation before source gen)
// =========================================================================
internal enum D2D1_FACTORY_TYPE { D2D1_FACTORY_TYPE_SINGLE_THREADED = 1, D2D1_FACTORY_TYPE_MULTI_THREADED = 2 };
internal enum DXGI_FORMAT { B8G8R8A8_UNORM = 87 };
internal enum DXGI_USAGE { DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x100000 };
internal enum DXGI_SCALING { DXGI_SCALING_STRETCH = 2 };
internal enum DXGI_SWAP_EFFECT { DXGI_SWAP_EFFECT_FLIP_DISCARD = 3 };
internal enum DXGI_ALPHA_MODE { DXGI_ALPHA_MODE_IGNORE = 0 };
internal enum DXGI_SWAP_CHAIN_FLAG { DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH = 0x00000004 };

[StructLayout(LayoutKind.Sequential)]
internal struct D2D1_COLOR_F { public float r, g, b, a; }
[StructLayout(LayoutKind.Sequential)]
internal struct D2D1_RECT_F { public float left, top, right, bottom; }
[StructLayout(LayoutKind.Sequential)]
internal struct D2D1_POINT_2F { public float x, y; }
[StructLayout(LayoutKind.Sequential)]
internal struct D2D1_ELLIPSE { public D2D1_POINT_2F point; public float radiusX, radiusY; }
[StructLayout(LayoutKind.Sequential)]
internal struct DXGI_SAMPLE_DESC { public uint Count, Quality; }
