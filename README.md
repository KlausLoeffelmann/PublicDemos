# Public Demos

Repository for all sorts of quick, simple, sometimes also a bit more extensive Demos for conferences, .NET Blog Posts, LinkedIn- or other social media posts, or just for public Prototyping.

## WARP DirectX Wall Clock — published Fri, Jun 19., 2026

The current featured demo is the **WARP DirectX Wall Clock** — a WinForms
.NET 11 analog clock where **every part (face, numerals, ticks, hands, arbour)
is its own DirectComposition visual**, composed by a small engine and styled by
**drop-in plug-in themes**. It doubles as a demo of the new .NET 11
`KioskModeManager` component (F11 fullscreen, taskbar hiding, sleep suppression,
wake events).

The headline trick: the engine alone owns time and hand *pointing*, so a theme
can relocate anchors and restyle visuals but **can never show the wrong time** —
move the hour numerals into a column on the left and the hour hand's tip simply
follows them (with an adjustable 1–30 s grace catch-up). Building it added a
per-visual transform API (`Transform` / `SetRotation`) to WARP's `D2DVisual`.
Two sample plug-ins ship: a free-floating *Lose-Hour* theme and a *NERD* theme
whose single second hand encodes the hour and minute in binary. Author your own
from a natural-language description with the `warpclock-theme-authoring` skill.

[WARP DirectX Wall Clock README](src/WinForms/NET11/WarpClock/README.md)

## Was new on Sat, Jun 06. 2026: Layout Tester

The **Layout Tester** is a WinForms .NET 10
prototype for poking at **High-DPI cascaded layout**: how `AutoScaleMode`
propagates through nested `UserControl` containers when the host form moves
between monitors at different DPI under `PerMonitorV2`.

You build up a **probe set** of nested *carrier containers* in the editor,
configure each one's scaling parameters individually (CTor vs Lazy
construction, scale apply phase, design resolution, font, percent), save the
set to disk, then launch a separate **carrier form** that actually
instantiates the hierarchy. Drag that carrier form between monitors of
different scaling factors and watch how each level of the cascade behaves.

Same scenario as the screenshot below — we have been ending up at this kind
of layout exploration session. (OK — I did go in one more time, and asked
the WinForms Expert Agent to do _one_ more tweak.)

How did we get here? Well — wait for the Episode, and tune in! :-)

<img width="1977" height="1534" alt="image" src="https://github.com/user-attachments/assets/4c5ce283-ba26-4d08-b04d-221445948404" />

[Layout Tester README](src/Prototypes/LayoutTests/README.md)

## Was new on Sat, May 30. 2026: BranchComposer

WinForms .NET 10 demo / tool built on the WARP Git services that **composes a
single working branch from a chosen set of feature branches** on top of a
fetched base branch. Validates each candidate against the fetched base before
composing (no stale-state surprises), surfaces all `git` activity in an
embedded console, and handles conflicts explicitly rather than via blanket
`-X theirs` so cumulative append-only files don't get silently truncated.

[BranchComposer README](src/WinForms/NET10/BranchComposer/README.md)

## Was new on Sat, May 30. 2026: D2DPong

A tiny **Direct2D + DirectComposition Pong** hosted in a WinForms `Form`.
Started life as a one-shot prompt to a **local LLM** (Qwen 3.6 on an EVO X2
Ryzen AI MAX+ 395 / Radeon 8060S iGPU) and was later refactored from
hand-rolled P/Invoke to **CsWin32**-generated COM interop. Kept around as a
demo of what a local model can produce on a non-trivial interop task in a
single shot.

[D2DPong README](src/Prototypes/D2DPong/README.md)

## Was new on Wed, May 13. 2026: Northwind (Visual Studio Toolbox, May 2026)

Companion demo from the **May 2026 Visual Studio Toolbox** episode: a
WinForms .NET 10 app over the classic Northwind sample database via
Entity Framework Core. Ships the EF Core data layer, the WinForms front-end
with a startup LocalDB smoke-test, and the actual Copilot prompts used during
the episode so the build-up is reproducible.

[Northwind README](src/WinForms/NET10/VSToolBox/2026-05/Northwind/README.md)
