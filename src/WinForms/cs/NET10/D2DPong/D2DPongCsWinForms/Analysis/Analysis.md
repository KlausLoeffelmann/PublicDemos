# Analysis of D2D Pong in C# WinForms implemented by QWen 3.6

Qwen 3.6 on LMStudio was given th following task:

## Hardware running LMStudio

Processor	AMD RYZEN AI MAX+ 395 w/ Radeon 8060S (3.00 GHz)
Installed RAM	128 GB (63.6 GB usable)
Graphics card	AMD Radeon(TM) 8060S Graphics (64 GB)
Storage	1.13 TB of 1.86 TB used
System type	64-bit operating system, x64-based processor

## Analysis of the Approach in general

The model picked a genuinely *correct and modern* high-level architecture, and that is
the most impressive part of the result. The intended pipeline is exactly what a
seasoned graphics developer would draw on a whiteboard:

```
D3D11 device  ->  IDXGIDevice  ->  ID2D1Device / ID2D1DeviceContext
      |                                   |
      +--> IDXGIFactory2 ---------------- + ----> swap chain for composition
                                          |
                       IDCompositionDevice -> Target(hwnd) -> root visual -> content visual(swap chain)
```

- It chose a **flip-model composition swap chain** (`CreateSwapChainForComposition`,
  `DXGI_SWAP_EFFECT_FLIP_DISCARD`, `B8G8R8A8`, 2 buffers) instead of the legacy
  `ID2D1HwndRenderTarget`. That is the right, current way to put Direct2D on screen.
- It correctly wired a **DirectComposition visual tree** (device → target-for-hwnd →
  root visual → content visual → `SetContent(swapChain)` → `Commit`).
- It separated concerns cleanly into small files: `PongGame` (pure game state/physics),
  `PongConfig` (constants), `MouseState`, `D2DRenderHost` (all the native rendering),
  and the WinForms `PongForm` host. The game-logic file (`PongGame.cs`) is essentially
  correct and needed **no** fixing.
- The render-loop design — drive updates from an `async` loop and let the WinForms
  `SynchronizationContext` marshal the continuations back onto the UI thread — happens
  to keep all COM access single-threaded, which is the behavior you want.
- It knew the *names* of every API that matters: `D3D11CreateDevice`,
  `CreateDXGIFactory2`, `D2D1CreateFactory`, `DCompositionCreateDevice`,
  `DWriteCreateFactory`, `CreateBitmapFromDxgiSurface`, `SetTarget`, `BeginDraw` /
  `EndDraw`, `Present`. The skeleton, the ordering, and the resource lifetimes were all
  in the right place.

So conceptually: a solid B+. If you only read the comments and the call sequence, it
looks like someone who understands DComp wrote it.

## Errors, which needed to be fixed

Where it fell apart was the **interop reality**. The model clearly never "saw" what
CsWin32 actually generates and invented an API surface that does not exist. The whole
thing did not compile (61+ errors). The substantive problems:

1. **Fundamental COM misconception.** It treated every COM method as a flat C-style
   free function hanging off a `PInvoke` class, passing the interface pointer as the
   first argument, e.g.
   `PInvoke.ID2D1DeviceContext_CreateSolidColorBrush(_d2dContext, ref white, out brush)`
   or `PInvoke.IDCompositionVisual_SetContent(visual, swapChain)`. None of those exist.
   CsWin32 generates real **`[ComImport]` interfaces** whose methods you call on the
   object: `_d2dContext.CreateSolidColorBrush(&white, null, out var brush)`. Every
   single COM call had to be rewritten. This is the single biggest "completely off"
   item — it is the difference between knowing *about* COM and knowing *how the binding
   works*.

2. **Wrong `using`s / namespaces.** `using Microsoft.Windows.CsWin32;` and
   `Microsoft.Windows.CsWin32.Interop;` were imported as if they were runtime
   namespaces. CsWin32 is a **compile-time source generator**; the generated code lives
   under `Windows.Win32.*` (e.g. `Windows.Win32.Graphics.Direct2D`). Everything was
   stored as raw `IntPtr` fields with manual `Marshal.Release`, instead of using the
   generated RCW interface types and letting QueryInterface be a simple cast.

3. **A `NativeMethods.txt` full of invented symbols.** It listed things like
   `IDCompositionDevice_CreateVisual`, `IDXGISwapChain_GetBuffer`,
   `ID2D1DeviceContext_SetTarget` — these are not Win32 API names, so CsWin32 generated
   nothing for them. The file had to be replaced with the real function and **interface**
   names (`D3D11CreateDevice`, `CreateDXGIFactory2`, `ID2D1DeviceContext`,
   `IDCompositionDevice`, …).

4. **Hand-rolled duplicate types that fight the generator.** It re-declared
   `D2D1_COLOR_F`, `D2D1_RECT_F`, `DXGI_SWAP_CHAIN_DESC1`, `DXGI_*` enums and a table of
   GUID **string** constants in `D2DGuids.cs`. With CsWin32 generating the authoritative
   versions these only cause clashes; they were deleted. The invented constants were
   also simply wrong — e.g. `D3D_FEATURE_LEVEL_11_0 = 0x0000` (the real value is
   `0xB000` / 45056), and it conflated `D3D_DRIVER_TYPE` "WARP" with a magic `1`.

5. **Project plumbing didn't match.** `PongForm.Designer.cs` was emitted in a *different*
   namespace (`D2DPongCsWinForms`) than the form itself (`WinFormsPong`), declared a
   second `partial PongForm` with its own `Dispose(bool)`, and was never wired up
   (`InitializeComponent` is never called) — producing `CS0115` and a duplicate-class
   conflict. `MouseState` lived in `WinFormsPong.DComp` while `PongGame` referenced it
   unqualified from `WinFormsPong`. Namespaces had to be unified and the orphaned
   designer file removed. `<AllowUnsafeBlocks>` was also required, since the real
   interop is pointer-based.

6. **Smaller API-shape mistakes** that only surface once the real signatures are known:
   `int`/`uint` mismatches on the swap-chain description, passing a string GUID where an
   `in Guid` is expected, a bitmap target created without
   `D2D1_BITMAP_OPTIONS_TARGET` (which would fail `SetTarget` at runtime), and an
   `EndDraw` whose signature it guessed.

In short: **the plan was right, the binding was fiction.** Roughly 100% of the native
interop layer (`D2DRenderHost.cs`) was rewritten, while the game logic, config, and the
overall structure survived almost untouched.

## What effort would a human have had...

...with a fair understanding of Direct2D, WinForms, C# and rudimentary DComp?

- **Designing it from scratch:** a few hours. The hard knowledge is exactly what the
  model already supplied for free — *which* APIs, in *what order*, and the DComp visual
  tree. A developer who has done this once would not have to look much up.
- **Doing the interop correctly:** this is where the human wins decisively. An
  experienced dev knows that COM methods are called *on* the generated interface, knows
  CsWin32 emits into `Windows.Win32.*`, and would have the swap-chain/bitmap-target
  details (BGRA support flag, `D2D1_BITMAP_OPTIONS_TARGET`, premultiplied vs. ignore
  alpha) in muscle memory. They would not have produced a single line of the
  `PInvoke.Interface_Method(ptr, …)` pattern.
- **Fixing *this* output to a running app** (what was actually done here): about half a
  day, and the fastest route was to ignore the invented binding entirely, let CsWin32
  generate the real interfaces, and inspect the generated `*.g.cs` to copy the exact
  signatures. That last step — reading the generated code — is precisely the feedback
  loop the local model did not have.

## Conclusion

The local model is a strong **architect** and a weak **mechanic**. It understood the
problem domain — Direct2D-on-DirectComposition with a flip-model swap chain — at a level
that many human developers do not, and it produced clean, well-factored, idiomatic-looking
C#. But it hallucinated the entire CsWin32/COM binding layer, because it was reasoning
from the *idea* of the APIs rather than from their actual generated shape.

The practical takeaway: for native-interop work, an LLM (local or not) is most valuable
as a *scaffolding and planning* partner, and least reliable on the exact marshalling
details. Pairing it with a tool feedback loop — generate the bindings, read what the
generator actually produced, and let the compiler drive the corrections — turns a
non-compiling sketch into a working app surprisingly quickly, because the expensive,
creative 80% (the correct architecture) was already there. Almost everything that had to
change was the boilerplate-but-unforgiving 20% at the COM boundary.

