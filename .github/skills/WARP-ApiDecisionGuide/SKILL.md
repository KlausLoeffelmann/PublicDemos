---
name: warp-api-decision-guide
description: Brief decision guide that maps a WinForms development task to the correct WARP-Toolkit NuGet package(s) and entry-point types. Use this skill as the FIRST step when the user asks "how do I do X with WARP?" or "which WARP package should I use for X?" before diving into a topic-specific WARP skill.
---

# WARP-Toolkit API Decision Guide

You are working in a .NET WinForms codebase that uses the **WARP-Toolkit**
(`WarpToolkit.*` packages). When the user describes a task, your first
job is to pick the **right package(s) and entry-point types** before writing
any code, and then either continue yourself or hand off to a more specific
WARP skill.

For deep context on each package see
`src/docs/GettingStarted/API-Overview.md` and the per-package files under
`src/docs/reference/` in this repo.

The current preview version of every WARP NuGet is:
`0.9.7-preview.g32895b766b`.

## How to use this skill

1. Read the user's request and identify the **primary intent** (UI control,
   AI/chat, app hosting, layout, async, rendering, …).
2. Map that intent to a row in the table below.
3. If a more specific WARP skill exists for that area (e.g. `warp-app-services`,
   `warp-winforms-controls`, `warp-winforms-ai`), invoke / consult it before
   coding. Otherwise read the linked reference file.
4. Verify the package is already referenced in the target project. If not,
   add it with the version above.

## Decision table

| If the user wants to… | Reach for | Reference |
|-----------------------|-----------|-----------|
| Bootstrap a WinForms app with DI, configuration, logging, dark-mode, fluent `Use*` setup | `WarpToolkit.WinForms.AppServices` (+ `WarpToolkit.Desktop.AppServices`). Use `WinFormsApplication.CreateBuilder(args)` and `WinFormsApplicationBuilder` | `WarpToolkit.WinForms.AppServices.md`, `WarpToolkit.Desktop.AppServices.md` |
| Show a message box / confirmation / input prompt **from a ViewModel** | `IDialogService` (contract in `Desktop.AppServices`, implementation registered via `AddWinFormsDialogService()` in `WinForms.AppServices`) | `WarpToolkit.Desktop.AppServices.md` |
| Persist form bounds, splitter distances or grid column widths between runs | `IUserSettingsService` from `WarpToolkit.ComponentModel` + extensions in `WarpToolkit.WinForms.Extensions.UI.UIServiceExtensions` (`SaveFormBounds`, `TryApplyFormBounds`, `SaveDataGridViewColumnWidths`, …) | `WarpToolkit.WinForms.Extensions.md` |
| Read an API key (OpenAI, Azure, Anthropic, …) from an environment variable | `EnvironmentVariableKeyService` from `WarpToolkit.WinForms.AppServices` with `WellKnownAIProviders` enum from `WarpToolkit.Desktop.AI`. Register via `services.AddLocalKeyRetrievalService()` | `WarpToolkit.WinForms.AppServices.md` |
| A modern tab control / message box / wizard / file picker / bindable combo | `WarpToolkit.WinForms` (`FluentTabControl`, `FluentMessageBox`, `WizardContainer`, `FilePathPicker`, `BindableComboBox`, `TransparentPanel`, …) | `WarpToolkit.WinForms.md` |
| Decorate a TableLayoutPanel with borders / error signalling / per-cell padding | `AdornerPanel` / `AdornerTableLayoutPanel` / `AdornerCellStyle` from `WarpToolkit.WinForms` (with layout primitives from `WarpToolkit.ComponentModel.GridLayouting`) | `WarpToolkit.WinForms.md` |
| Walk the control tree, plug an `IValueConverter` into a `Binding`, apply dark-mode to a `DataGridView`, show a modal dialog async | `WarpToolkit.WinForms.Extensions` (`ControlExtensions`, `BindableComponentExtensions`, `DataGridViewExtensions.ApplyDarkMode`, `FormExtensions.ShowDialogAsync<T>`) | `WarpToolkit.WinForms.Extensions.md` |
| An interactive in-process terminal pane, or collision-free filename generation | `WarpToolkit.WinForms.Specialized` (`ConsoleControl`, `FilenameDisambiguator`) | `WarpToolkit.WinForms.Specialized.md` |
| GDI+-style drawing but **hardware accelerated** (Direct2D/DirectWrite) | Inherit from `D2DPanel` and draw via `D2DGraphics` in `WarpToolkit.WinForms.DirectX`. Each panel owns its own device-manager bundle. | `WarpToolkit.WinForms.DirectX.md` |
| Render Markdown / formatted text with custom layout | `WarpToolkit.WinForms.Typography` (`ITypographyRenderer`, `Block`/`Inline`/`BulletedParagraph`, MarkDig integration) | `WarpToolkit.WinForms.Typography.md` |
| Wire up `async` button clicks, await a form closing, freeze content with a spinner | `WarpToolkit.WinForms.Async` (`AsyncButton`, `AwaitableForm`, `AwaitableEvent<T>`, `ContentFreezePanel`) | `WarpToolkit.WinForms.Async.md` |
| Format typed input (date, decimal, AI-described) bound to a `TextBox` | `WarpToolkit.WinForms.Async.TypedInputExtenders` (`DateFormatterComponent`, `DecimalFormatterComponent`, `AITypedFormatter<T>`) | `WarpToolkit.WinForms.Async.md` |
| Add AI chat to a form (designer-droppable, provider-agnostic) | Drag an `AIChatServiceBase`-derived component from `WarpToolkit.WinForms.AI` onto the form. The full chat UI is `ChatView` from `WarpToolkit.WinForms.Chat` | `WarpToolkit.WinForms.AI.md`, `WarpToolkit.WinForms.Chat.md` |
| Build prompts / parse streamed responses / declare `[AITemplate]` request types | `WarpToolkit.Desktop.AI` (`PromptBuilder`, `ReturnTokenParser`) and `WarpToolkit.Microsoft.Extensions.AI` (`AIChatClient<T>`, `AIChatHistory`, `[AITemplate]`, `[AITemplateSegment]`, `[AIGenerated]`) | `WarpToolkit.Desktop.AI.md`, `WarpToolkit.Microsoft.Extensions.AI.md` |
| Inspect / rewrite C# source with Roslyn, preserving comments | `WarpToolkit.Desktop.Roslyn` (`DocumentExtensions`, `MemberDeclarationExtensions`, `ReplaceCommentsByGuids` / `ReplaceGuidsByComments`) | `WarpToolkit.Desktop.Roslyn.md` |
| Show a Roslyn-classified source document in a control | `RoslynDocumentView` from `WarpToolkit.WinForms.Roslyn` | `WarpToolkit.WinForms.Roslyn.md` |
| Add a designer-aware logger to the app | `WarpToolkit.Microsoft.Extensions` (`AddTimeStampedDebug`, `AddWinFormsFileLogger`, `UseWinFormsLogging`) | `WarpToolkit.Microsoft.Extensions.md` |
| A sub-millisecond timer for animations or media | `HighPrecisionTimer` from `WarpToolkit.Windows.Interop.PrecisionTimer` | `WarpToolkit.Windows.Interop.md` |
| A DI-aware component that receives the host form / container as `IServiceProvider` at designer time | Implement `IServiceProviderAssignable` (`WarpToolkit.ComponentModel`); attach `ServiceProviderAssignableComponentCodeDomSerializer` from `WarpToolkit.WinForms.Design` | `WarpToolkit.WinForms.Design.md`, `WarpToolkit.ComponentModel.md` |
| Inspect local GitHub repos, list branches/commits, compose integration branches, or get an authenticated Octokit client | `WarpToolkit.WinForms.Github` (`ILocalGitRepositoryService`, `IGitBranchCompositionService`, `IGitHubClientFactoryService`) | `warp-github-git` skill |

## Hand-off targets

When the chosen topic has its own dedicated skill, prefer that skill's
guidance over generic reasoning. Available WARP topic skills (in this repo
under `src/CopilotSkills/`):

- **`warp-app-services`** — `WarpToolkit.Desktop.AppServices` +
  `WarpToolkit.WinForms.AppServices` (hosting, DI, dialogs, settings, keys).

- **`warp-winforms-application-builder`** — Bootstrapping
  `WinFormsApplication` for new apps **and** retrofitting an existing
  `Application.Run(...)` codebase, plus the Form-as-`IServiceProvider`
  preparation required by every `BindableServiceProviderComponent`-based
  component (AI services, WebView2-backed `ChatView`, custom DI-aware
  components). Consult this skill first whenever a feature requires DI.

- **`warp-winforms-controls`** — `WarpToolkit.WinForms`,
  `WarpToolkit.WinForms.Extensions`, `WarpToolkit.WinForms.Specialized`.

- **`warp-fluent-tab-control`** — The mandatory "one UserControl per
  tab page" pattern, host-form sizing caps (≤ 75% of screen, ≤ 90% of
  parent), and when a tab UserControl needs its own scrolling layer.

- **`warp-winforms-ai`** — `WarpToolkit.Desktop.AI`,
  `WarpToolkit.Microsoft.Extensions.AI`, `WarpToolkit.WinForms.AI`,
  `WarpToolkit.WinForms.Chat`.

- **`warp-github-git`** — `WarpToolkit.WinForms.Github`
  (GitHub auth/client helpers, local Git repository inspection, branch metadata,
  and safe branch composition through temporary worktrees).

## Anti-patterns to avoid

- **Do not** roll your own `Application.Run(new MainForm())` startup when an
  existing app uses `WinFormsApplicationBuilder` — extend the builder
  pipeline instead.

- **Do not** use `MessageBox.Show` directly from view-models. Use
  `IDialogService` so the view-model stays testable and UI-framework agnostic.

- **Do not** instantiate `IChatClient` providers directly when an
  `AIChatServiceBase`-derived component already exists on the form — the
  component is a registered service in the DI container and the place where
  Designer-time properties (model, temperature, etc.) live.

- **Do not** share an `IDWriteFactory` or `D2DDeviceManager` between
  `D2DPanel` instances on different threads. Each panel owns its own bundle
  by design.

- **Do not** create a `Program.cs` for VB. WARP follows the VB Application
  Framework conventions instead.
