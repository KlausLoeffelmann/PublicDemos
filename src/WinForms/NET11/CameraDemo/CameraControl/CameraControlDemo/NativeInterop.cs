using System.Runtime.InteropServices;

namespace CameraControlDemo;

/// <summary>
///  Provides raw pointer access to a WinRT memory buffer.
/// </summary>
/// <remarks>
///  CsWinRT does not project this COM contract. The software-frame fallback uses it
///  to upload camera pixels directly into a Direct2D bitmap without first creating a
///  GDI+ bitmap.
/// </remarks>
[ComImport]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal unsafe interface IMemoryBufferByteAccess
{
    /// <summary>
    ///  Gets the address and capacity of the referenced memory buffer.
    /// </summary>
    /// <param name="buffer">Receives the first byte of the buffer.</param>
    /// <param name="capacity">Receives the buffer capacity in bytes.</param>
    void GetBuffer(out byte* buffer, out uint capacity);
}

/// <summary>
///  Exposes the DXGI object wrapped by a WinRT Direct3D surface.
/// </summary>
/// <remarks>
///  This is the standard bridge between <c>IDirect3DSurface</c> and native DXGI.
///  Keeping it local avoids importing unrelated Windows Runtime interop APIs.
/// </remarks>
[ComImport]
[Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IDirect3DDxgiInterfaceAccess
{
    /// <summary>
    ///  Queries the wrapped Direct3D object for a native interface.
    /// </summary>
    /// <param name="iid">The requested interface identifier.</param>
    /// <param name="result">Receives an AddRef'd interface pointer.</param>
    /// <returns>An HRESULT describing the operation.</returns>
    [PreserveSig]
    int GetInterface(in Guid iid, out nint result);
}
