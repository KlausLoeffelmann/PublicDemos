# D2DPong (`D2DPongCsWinForms`)

A tiny **Direct2D + DirectComposition Pong** hosted inside a WinForms `Form`.

## Origin story

This started as a one-shot prompt to a **local LLM** (Qwen 3.6, running on an
EVO X2 Ryzen AI MAX+ 395 with a Radeon 8060S iGPU) — "write me Pong with
Direct2D and DirectComposition in a WinForms host" — and is kept around as a
demo of how far a local model gets on a non-trivial interop task in a single
shot. The initial output is preserved in history; subsequent commits
**refactored the raw P/Invoke code to use [CsWin32](https://github.com/microsoft/CsWin32)**
for the COM interfaces and types instead of hand-rolled signatures.

## What it does

- Creates a top-level WinForms `PongForm`.
- Spins up a Direct2D / DComp render target on the form's HWND
  (`D2DRenderHost`).
- Runs a game loop on a background task that updates `PongGame` state and
  asks the render host to paint at the configured FPS target.
- Mouse-controlled left paddle, simple AI on the right (toggled via
  `PongConfig.USE_MOUSE_CONTROL`).

## Configuration

All knobs live in [`PongConfig.cs`](D2DPongCsWinForms/PongConfig.cs):
window size, paddle/ball sizes, ball speed and per-rally speed increment,
FPS target, mouse vs self-play.

## Build / run

```powershell
dotnet build src\Prototypes\D2DPong\D2DPong.slnx
```

Then run `D2DPongCsWinForms`. Targets `net10.0-windows`, allows unsafe blocks
(needed for the CsWin32-generated COM interop), and pulls Win32 metadata via
the `Microsoft.Windows.CsWin32` source generator (see `NativeMethods.txt` for
the requested APIs).

## Project layout

```
D2DPongCsWinForms/
├─ Program.cs
├─ PongForm.cs          # WinForms host
├─ D2DRenderHost.cs     # D2D / DComp setup + drawing
├─ PongGame.cs          # Game state & update logic
├─ PongConfig.cs        # All tweakable constants
├─ MouseState.cs
└─ NativeMethods.txt    # CsWin32 API list
```

No WARP dependency — this one is pure stdlib + CsWin32.
