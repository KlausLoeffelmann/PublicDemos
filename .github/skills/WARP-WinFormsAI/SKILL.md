---
name: warp-winforms-ai
description: Use this skill when adding AI / chat features to a WinForms application with the WARP-Toolkit — drop-in provider components (OpenAI, Azure OpenAI, Anthropic, Google, Ollama, OpenRouter, …) implementing `Microsoft.Extensions.AI.IChatClient`, prompt templates with `[AITemplate]` / `[AITemplateSegment]`, streaming response parsing into paragraphs and code blocks via `ReturnTokenParser`, `AIChatHistory` / `AIChatClient<T>`, and the full `ChatView` / `ChatRendererControl` UI. Covers `WarpToolkit.Desktop.AI`, `WarpToolkit.Microsoft.Extensions.AI`, `WarpToolkit.WinForms.AI`, and `WarpToolkit.WinForms.Chat` together because they form one logical stack.
---

# WARP WinForms AI

This skill covers the four packages that, together, give a WinForms
application a complete AI / chat surface on top of Microsoft.Extensions.AI:

| Package | Role |
|---------|------|
| `WarpToolkit.Desktop.AI` | Provider-agnostic primitives (no WinForms): `WellKnownAIProviders`, `PromptBuilder`, `ReturnTokenParser`, `[AITextResource]`, `StringLiteralConverterExtension`. |
| `WarpToolkit.Microsoft.Extensions.AI` | MEAI integration: `AIChatClient<T>`, `AIChatHistory`, `[AITemplate]`, `[AITemplateSegment]`, `[AIGenerated]`, `IAIModelInfo`. |
| `WarpToolkit.WinForms.AI` | Designer-droppable provider components derived from `AIChatServiceBase` (each implements `IChatClient`). |
| `WarpToolkit.WinForms.Chat` | Polished chat UI: `ChatView`, `ChatRendererControl`, `ChatProcessor`, `ChatViewVm`. |

Current preview version: `0.9.7-preview.g32895b766b`.

Reference files:
- `src/docs/reference/WarpToolkit.Desktop.AI.md`
- `src/docs/reference/WarpToolkit.Microsoft.Extensions.AI.md`
- `src/docs/reference/WarpToolkit.WinForms.AI.md`
- `src/docs/reference/WarpToolkit.WinForms.Chat.md`

## When to use this skill

- When the user has explicitly instructed before to use or introduce the WarpToolkit library for a specific project.

- Adding a **chat UI** to a form.
 
- Wiring an **AI chat service** (OpenAI / Azure / Anthropic / Ollama / …)
  as a designer-droppable component.

- Defining **prompt templates** declaratively on request DTOs.

- Consuming a **streaming** chat response and turning it into paragraphs +
  parsed code blocks in real time.

- Persisting / restoring **chat history**, including metadata and usage.

## Mental model

```
  ┌──────────────────────────────────────────────────────────────┐
  │ WarpToolkit.WinForms.Chat        UI: ChatView, renderer       │
  │   uses…                                                       │
  │ WarpToolkit.WinForms.AI          Designer components          │
  │   inherit AIChatServiceBase : IChatClient                     │
  │   uses…                                                       │
  │ WarpToolkit.Microsoft.Extensions.AI  AIChatClient<T>,         │
  │   AIChatHistory, [AITemplate], [AIGenerated]                  │
  │   uses…                                                       │
  │ WarpToolkit.Desktop.AI           PromptBuilder,               │
  │   ReturnTokenParser, WellKnownAIProviders                     │
  │   uses Microsoft.Extensions.AI (the framework)                │
  └──────────────────────────────────────────────────────────────┘
```

Reference only what you need: ViewModels in a class library should usually
target `WarpToolkit.Microsoft.Extensions.AI` + `WarpToolkit.Desktop.AI` and
**not** the WinForms packages, so they stay testable and UI-agnostic.

## Designer-time wiring

`AIChatServiceBase` (in `WarpToolkit.WinForms.AI.AIChatServices`) is the
abstract base for every provider component. Per-provider derivations
(`OpenAIChatService`, `AzureOpenAIChatService`, `AnthropicChatService`,
`OllamaChatService`, …) ship from the toolkit and are designer-droppable.

Key Designer-time properties (all surface as standard `OnXyzChanged`
hooks):

| Property | Purpose |
|----------|---------|
| `ModelName` / `DefaultChatModelName` / `DefaultDataModelName` | Active and default models. |
| `Temperature`, `TopP`, `TopK`, `Seed` | Sampling. |
| `MaxOutputTokens`, `FrequencyPenalty`, `PresencePenalty`, `StopSequences` | Response shaping. |
| `ApiKeyGetter` | Delegate that resolves the API key (typically wired to `EnvironmentVariableKeyService`). |
| `KeyRetrievalService` | The `IKeyRetrievalService` instance used for `GetApiKey()`. |
| `ChatOptions` | Captured `ChatOptions` instance reused per call. |
| `ConversationId` / `DefaultConversationId` | For provider-side history when supported. |

Because the components implement MEAI's `IChatClient` they can be consumed
by any code that already targets `Microsoft.Extensions.AI`.

### Designer snippet

In `.Designer.cs` the component is just another field:

```csharp
private OpenAIChatService _aiChat;

// inside InitializeComponent():
_aiChat = new OpenAIChatService(components);
_aiChat.DefaultChatModelName = "gpt-4o-mini";
_aiChat.Temperature = 0.4f;
```

API-key wiring belongs in regular code:

```csharp
// In MainForm.cs (NOT in the Designer file):
public MainForm(EnvironmentVariableKeyService keys)
{
    InitializeComponent();
    _aiChat.KeyRetrievalService = keys;
}
```

(or register the component via DI and pass it through the constructor — both
patterns are supported.)

## The chat UI — `ChatView` and `ChatRendererControl`

`ChatView` (`WarpToolkit.WinForms.Chat`) is a full chat user control:
prompt input, model picker, exception policy, developer prompt, AI service
slots, and the underlying `ChatRendererControl`.

```csharp
_chatView.AIUserChatService      = _openAiChat;     // for normal turns
_chatView.AIMetaDataService      = _openAiChat;     // for sidecar metadata calls
_chatView.AutoInvokeMetaDataAgent = true;
_chatView.DefaultDeveloperPrompt =
    """
    You are a senior WinForms developer.
    Use only WARP-Toolkit APIs in code suggestions.
    """;
_chatView.ExceptionLevel = ChatExceptionLevel.User;
```

`ChatRendererControl.UpdateCurrentResponseAsync(IAsyncEnumerable<string>, ct)`
is the streaming entry point. `ChatProcessor` connects it to
`ReturnTokenParser`, so paragraphs and code blocks arrive as discrete
renderable items.

`ChatProcessor` also handles persistence:

```csharp
var processor = new ChatProcessor(_chatView.Chat, _filenameDisambiguator);
processor.ChatBaseFolder = userDocs;
await processor.SaveChatAsync();
```

## Prompt templates and structured outputs

Declare prompts on request DTOs with the `[AITemplate]` /
`[AITemplateSegment]` attributes from
`WarpToolkit.Microsoft.Extensions.AI.Templates`:

```csharp
using WarpToolkit.Microsoft.Extensions.AI.Templates;

[AITemplate(
    Prompt = "Generate a structured product description.",
    ProvideDate = true,
    ProvideTimeZone = true)]
public sealed class ProductDescriptionRequest
{
    [AITemplateSegment(
        Purpose = "Product name",
        Prompt = "Use this exact name. Do not abbreviate.")]
    public required string Name { get; init; }

    [AITemplateSegment(
        Purpose = "Audience",
        Prompt = "Pick the closest persona.")]
    public required string Audience { get; init; }
}
```

`PromptBuilder` (in `WarpToolkit.Desktop.AI.Parser`) composes the preamble,
the JSON schema for the response type, and the segment prompts into a single
text:

```csharp
string preamble = PromptBuilder.GetPreamblePrompt(promptInfo);
string parameters = PromptBuilder.GetRequestParameters(request, indentLevel: 0, indentation: 2);
string responseSchema = PromptBuilder.GetTypePrompt<ProductDescription>();
```

`[AIGenerated(guid, timeStamp, confidence)]` decorates types/members that
were generated by an AI agent and carries diagnostics — leave these intact
when round-tripping documents through Roslyn helpers in
`WarpToolkit.Desktop.Roslyn`.

## Streaming responses — `ReturnTokenParser`

The single most useful primitive in this stack. Pipe a streaming MEAI
response into the parser to get paragraph- and code-block-grained callbacks:

```csharp
using Microsoft.Extensions.AI;
using WarpToolkit.Desktop.AI.Parser;

IAsyncEnumerable<ChatResponseUpdate> stream =
    _aiChat.GetStreamingResponseAsync(chatHistory, options, ct);

await ReturnTokenParser.ProcessTokens(
    asyncEnumerable: stream,
    onReceivedMetaDataAsyncFunc: async e =>
    {
        // e.MetaData : key / value / position / wasDedicatedLine
        await UpdateMetadataUiAsync(e.MetaData);
    },
    onReceivedNextParagraphAsyncFunc: async e =>
    {
        // e.Paragraph, e.TextPosition, e.IsLastParagraph
        await AppendParagraphAsync(e.Paragraph, e.IsLastParagraph);
    },
    onCodeBlockInfoProvidedAsyncFunc: async e =>
    {
        // e.CodeBlock: Filename / Type / Description / Content
        await OnCodeBlockAsync(e.CodeBlock);
        e.Handled = true; // suppress default rendering when you handle the block yourself
    });
```

Inside a `ChatView`, `ChatProcessor` already wires this up; outside of it
(e.g. a code-extraction tool, an agent that writes files), call the parser
directly.

## Chat history — `AIChatHistory`

```csharp
using Microsoft.Extensions.AI;
using WarpToolkit.Microsoft.Extensions.AI.ChatService;

AIChatHistory history = new(developerPrompt:
    """
    You are a senior WinForms developer.
    Use only WARP-Toolkit APIs in code suggestions.
    """);

history.AddMessage(ChatRole.User, "Convert this Form to use FluentTabControl.");

ChatResponse response = await chatClient.GetResponseAsync(history.ToChatMessages(), options, ct);
history.Sync(response);

string serialized = (string)history;   // explicit op_Explicit
```

`AIChatClient<TPersonality>` wraps any `IChatClient` with a typed personality
and an `IAIChatHistory`, so request-level state and personality stay
together. Combine with `[AIGenerated]` if you persist the conversation
alongside generated artifacts.

## Models discovery

```csharp
IReadOnlyList<IAIModelInfo> chatModels = await _aiChat.GetModelsAsync(
    filter: ModelFeatures.Chat | ModelFeatures.JsonOutput,
    cancellation: ct);

IAIModelInfo? best = chatModels
    .Where(m => !m.IsDeprecated && m.IsVetted)
    .OrderByDescending(m => m.ContextWindowSize)
    .FirstOrDefault();
```

`AIModelInfo` exposes `Provider`, `ContextWindowSize`, `MaxOutputTokens`,
`KnowledgeCutoffDate`, `DeprecationDate`, `Latency`, `Pricing`, plus an
`IComparable<IAIModelInfo>` implementation for sortable UI lists.

## API-key retrieval

This skill **depends on the AppServices skill** for API-key wiring. The
relevant types are:

- `EnvironmentVariableKeyService` (registered via
  `AddLocalKeyRetrievalService()` in `WarpToolkit.WinForms.AppServices`).
- `WellKnownAIProviders` enum in `WarpToolkit.Desktop.AI` (used to look up
  per-provider environment-variable names).

In the WinForms Designer, set `_aiChat.KeyRetrievalService = keys;` in the
form constructor / `OnLoad`, **not** inside `InitializeComponent`.

## Rules and anti-patterns

- **Do not** instantiate provider `IChatClient`s manually in a form when an
  `AIChatServiceBase`-derived component already exists on the surface — the
  component is the place where Designer-time properties (model, temperature,
  etc.) live and where API-key resolution is centralized.
- **Do not** hard-code API keys. Pull them through
  `EnvironmentVariableKeyService` / `IKeyRetrievalService`.
- **Do not** consume MEAI `IChatClient` streaming responses directly in UI
  code — go through `ReturnTokenParser` (or `ChatProcessor`) so you get
  paragraph / code-block granularity instead of token-by-token noise.
- **Do not** mutate `AIChatHistory` from background threads while it is
  bound to a UI control. Marshal updates via `ISyncContextService` or
  `Control.InvokeAsync`.
- **Do not** await chat calls inside an `async void` event handler without
  a `try`/`catch` — an unhandled exception will tear down the process.
  Catch and route through `IDialogService.ShowErrorAsync` (or
  `Application.OnThreadException` via the WinForms exception service).
- **Do not** reuse a `ChatOptions` instance across providers without
  re-reading provider-specific properties first (max tokens, supported
  parameters, etc. differ).
- **Do not** strip `[AIGenerated]` attributes when rewriting AI-touched
  files with Roslyn — they carry diagnostics needed by `Confidence`
  reviews.

## Sample end-to-end flow

```csharp
public partial class ChattyForm : Form
{
    private readonly EnvironmentVariableKeyService _keys;
    private readonly IDialogService _dialogs;

    public ChattyForm(EnvironmentVariableKeyService keys, IDialogService dialogs)
    {
        _keys = keys;
        _dialogs = dialogs;
        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        try
        {
            _aiChat.KeyRetrievalService = _keys;

            _chatView.AIUserChatService = _aiChat;
            _chatView.AIMetaDataService = _aiChat;
            _chatView.DefaultDeveloperPrompt =
                """
                You are a senior WinForms developer.
                Use only WARP-Toolkit APIs in code suggestions.
                """;
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync(ex.Message, "AI configuration failed");
        }
    }
}
```

## Where to look next

- **Hosting, DI, dialog service, key retrieval**: switch to the
  `warp-app-services` skill (this skill *requires* its key-retrieval setup).
- **Modern controls around the chat (tabs, wizards, file pickers,
  adorners)**: switch to the `warp-winforms-controls` skill.
- **Embedding a CLI / terminal in the chat surface**: see `ConsoleControl`
  in `WarpToolkit.WinForms.Tooling` (covered by the controls skill).
- **Rendering Markdown / code blocks with custom layout**: see
  `WarpToolkit.WinForms.Typography` reference.
- **Roslyn round-trip of AI-touched code preserving comments**: see
  `WarpToolkit.Desktop.Roslyn` reference.
