using System.Drawing;

using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Abstractions;

/// <summary>
///  Draws the cached content of clock elements into their per-element Direct2D
///  surface. A single renderer instance typically handles all of a theme's
///  element kinds, switching on <see cref="IClockRenderContext.Id"/>.
/// </summary>
public interface IClockElementRenderer
{
    /// <summary>
    ///  Draws the element identified by <paramref name="context"/> into
    ///  <paramref name="graphics"/>. The surface has already been cleared to
    ///  transparent and a <c>BeginDraw</c> issued; the implementation must not call
    ///  <c>BeginDraw</c>/<c>EndDraw</c>. Draw in element-local pixel space
    ///  (origin at top-left of <see cref="IClockRenderContext.ContentSize"/>).
    /// </summary>
    void DrawElement(ID2DGraphics graphics, IClockRenderContext context);
}
