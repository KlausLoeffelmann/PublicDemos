Build a WinForms app (net11.0-windows10.0.26100.0, C# latest, NRTs on) that displays
a live camera feed and lets the user pick the camera and the capture resolution.

How to work on this (read first):

- Work straight through, no exploration detours. The scaffold project already exists;
  just retarget it and add the files.
- DO NOT run, launch or otherwise test the app, and do not take screenshots. A single
  `dotnet build` at the very end to confirm it compiles is enough. Testing is the
  user's job.
- Do not ask clarifying questions; the spec below is complete. Prefer the shortest
  path that satisfies it.
- Keep prose explanation to a minimum, but generate XML doc comments on all public
  and private members.

Project setup (do this first, it is easy to get wrong):

- TargetFramework `net11.0-windows10.0.26100.0`,
  `SupportedOSPlatformVersion` `10.0.19041.0` — without the Windows SDK version in the
  TFM the Windows.Media.* namespaces do not exist at all.
- `AllowUnsafeBlocks` = true (needed for the frame copy).
- `Nullable` = enable, `UseWindowsForms` = true, `ImplicitUsings` = enable.

Requirements:

1. CameraView : Control (NOT PictureBox).
   - SetStyle(UserPaint | Opaque | AllPaintingInWmPaint, true)
   - Single reusable back buffer Bitmap, PixelFormat.Format32bppPArgb.
     No per-frame allocation.
   - KeepAspectRatio property (default true):
     - false: set CompositingMode.SourceCopy, InterpolationMode.NearestNeighbor,
       PixelOffsetMode.Half, SmoothingMode.None BEFORE drawing and draw 1:1 at (0,0)
       — never scale.
     - true: draw the frame centered and letterboxed into the client area, preserving
       the source aspect ratio (HighQualityBilinear when the size differs, otherwise
       NearestNeighbor). Uncovered area is filled with BackColor.
   - GetPreferredSize returns the source resolution.

2. Capture via Windows.Media.Capture:
   - MediaFrameSourceGroup.FindAllAsync(), expose all groups with a color source as
     selectable devices. Note MediaFrameSourceKind lives in
     Windows.Media.Capture.Frames but MediaStreamType lives in Windows.Media.Capture —
     both usings are needed wherever source infos are filtered.
   - MediaCapture.InitializeAsync: MemoryPreference.Cpu,
     StreamingCaptureMode.Video, SharingMode.ExclusiveControl.
   - Expose MediaFrameSource.SupportedFormats as selectable capture formats
     (resolution / frame rate / subtype) and apply the pick via SetFormatAsync
     before the reader is created.
   - CreateFrameReaderAsync(source, MediaEncodingSubtypes.Bgra8) so MF does the
     conversion — do not convert NV12 manually. Set
     AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime.
   - Declare IMemoryBufferByteAccess yourself (it is not projected):
     [ComImport, Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D"),
      InterfaceType(ComInterfaceType.InterfaceIsIUnknown)].
   - IMPORTANT, this is the classic trap: under CsWinRT the old UWP pattern
     `((IMemoryBufferByteAccess)reference)` compiles but throws at runtime
     ("Invalid cast from 'WinRT.IInspectable'"). Use
     `reference.As<IMemoryBufferByteAccess>()` (`using WinRT;`) instead.
   - Copy row by row with Buffer.MemoryCopy, honoring BitmapPlaneDescription.StartIndex
     and .Stride versus the BitmapData.Stride of the back buffer — the two strides
     differ.
   - StartAsync/StopAsync fully tear down the previous reader and MediaCapture, so
     switching device or format is race free.

3. FrameArrived fires off the UI thread:
   - Copy into the back buffer inside the handler while the frame is still alive,
     then marshal with Control.InvokeAsync to Invalidate().
   - Drop frames if a repaint is still pending — never queue. (Interlocked gate that
     is released at the end of OnPaint.)
   - Do not silently swallow exceptions in the frame handler: report the first failure
     per session through the error event / status text, otherwise the control just
     stays black with no clue why.

4. Demo form: normal resizable window with a control bar (camera ComboBox,
   resolution ComboBox, "Keep aspect ratio" CheckBox, Refresh button) and the
   CameraView hosted in a single 100% cell of a TableLayoutPanel, docked so it always
   sizes optimally while KeepAspectRatio is on. Esc closes.
   Suppress the ComboBox SelectedIndexChanged handlers while their DataSource is being
   filled, otherwise filling the lists restarts the camera.

5. Handle UnauthorizedAccessException from InitializeAsync with a message
   pointing at Settings > Privacy > Camera > "Let desktop apps access your camera".
   Also paint a status/error message inside the control while no frame is available.
