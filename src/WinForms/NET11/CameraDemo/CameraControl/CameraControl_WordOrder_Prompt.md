Build a WinForms app (net10.0-windows10.0.26100.0, C# 13, NRTs on) that displays
a live camera feed fullscreen with no scaling.

Requirements:

1. CameraView : Control (NOT PictureBox).
   - SetStyle(UserPaint | Opaque | AllPaintingInWmPaint, true)
   - Single reusable back buffer Bitmap, PixelFormat.Format32bppPArgb.
     No per-frame allocation.
   - OnPaint: set CompositingMode.SourceCopy, InterpolationMode.NearestNeighbor,
     PixelOffsetMode.Half, SmoothingMode.None BEFORE drawing.
     Draw 1:1 at (0,0) — never scale.

2. Capture via Windows.Media.Capture:
   - MediaFrameSourceGroup.FindAllAsync(), pick first group with a color source.
   - MediaCapture.InitializeAsync: MemoryPreference.Cpu,
     StreamingCaptureMode.Video, SharingMode.ExclusiveControl.
   - CreateFrameReaderAsync(source, MediaEncodingSubtypes.Bgra8) so MF does the
     conversion — do not convert NV12 manually.
   - Declare IMemoryBufferByteAccess yourself (it is not projected).

3. FrameArrived fires off the UI thread:
   - Copy into the back buffer inside the handler while the frame is still alive,
     then marshal with Control.InvokeAsync to Invalidate().
   - Drop frames if a repaint is still pending — never queue.

4. Form: FormBorderStyle.None, WindowState.Maximized, control sized exactly to
   the source resolution. Esc closes.

5. Handle UnauthorizedAccessException from InitializeAsync with a message
   pointing at Settings > Privacy > Camera > "Let desktop apps access your camera".

Generate XML doc comments. Keep prose explanation to a minimum.
