using System.Runtime.InteropServices;

namespace WinFormsPong.DComp;

[StructLayout(LayoutKind.Sequential)]
internal struct DXGI_SWAP_CHAIN_DESC1
{
    public uint Width, Height; public DXGI_FORMAT Format; public bool Stereo;
    public DXGI_SAMPLE_DESC SampleDesc; public DXGI_USAGE BufferUsage; public uint BufferCount;
    public DXGI_SCALING Scaling; public DXGI_SWAP_EFFECT SwapEffect; public DXGI_ALPHA_MODE AlphaMode;
    public uint Flags;
}
