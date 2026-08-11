---
name: winforms-cswin32-com-apartments
description: Use this skill when a WinForms app calls CsWin32-generated COM interfaces from background, capture, render, or worker threads, especially when debugging E_NOINTERFACE, InvalidCastException, wrong-thread failures, or deadlocks around DirectComposition, DirectX, media, shell, or other native object trees.
---

# WinForms + CsWin32 COM Apartment Safety

CsWin32 generates strongly typed P/Invoke and classic COM interop definitions. It
does **not** make a native COM object free-threaded or automatically marshal its
runtime callable wrapper (RCW) to the correct apartment.

In WinForms, the UI thread is normally an STA. Media callbacks, render callbacks,
and thread-pool work normally run in the MTA. Treat that boundary as part of the
native object's design.

## The failure pattern

An object may be created and work correctly on a worker callback, then fail when a
WinForms event calls the same RCW:

```text
InvalidCastException
Unable to cast System.__ComObject to IDCompositionVisual
E_NOINTERFACE (0x80004002)
```

This does not always mean that the native object lacks the interface. The interface
may have worked in its creating apartment, while the RCW cannot obtain or marshal
that interface from the calling apartment.

**A C# lock only serializes access. It does not perform COM apartment marshalling.**

## Core rule

Choose an owning apartment for every COM object tree, then perform its complete
lifecycle there:

- creation and `QueryInterface`/casts;
- property and method calls;
- commits, attachment, and detachment;
- teardown and final COM releases.

Do not call a worker-created interface from the UI merely to prepare it for release.
For example, an MTA-created `ID2D1DeviceContext.SetTarget(null)` can fail during a
UI-triggered `Dispose` even when the Direct2D factory is multithreaded. If ownership
cannot be marshalled back, prefer release ordering that destroys the owning context
before the resource it retains, without making another typed COM call.

For HWND-related composition or presentation objects, prefer the WinForms UI
apartment. Keep documented free-threaded graphics work on a render/MTA path.

For example:

| Resource | Typical owner |
|---|---|
| WinForms controls and HWND visual tree | UI STA |
| DirectComposition target and visuals | UI STA |
| Serialized multithreaded D2D/D3D drawing | Render thread or MTA |
| Immutable frame data | Producer thread until ownership transfers |

Check the native API documentation rather than assuming every DirectX or shell
interface has the same threading model.

## Prefer the smallest native object graph

Do not add a DirectComposition visual tree when a single swap chain only needs to
fill one WinForms HWND. `IDXGIFactory2.CreateSwapChainForHwnd` with a flip-model
swap effect is already composed by the Desktop Window Manager.

Use `CreateSwapChainForComposition` plus DirectComposition only when the app needs
composition-specific behavior such as multiple visuals, transforms, opacity,
clipping, animation, or non-HWND composition content. Avoiding unnecessary COM
layers reduces interface casting, apartment ownership, cleanup, and failure paths.

If `E_NOINTERFACE` remains after moving calls to one apartment, do not keep adding
dispatching code. Verify the generated ABI and simplify or remove the unnecessary
COM interface family.

## Safe WinForms pattern

Capture the UI `SynchronizationContext` after the control handle is created:

```csharp
private SynchronizationContext? _uiContext;
private int _uiThreadId;

protected override void OnHandleCreated(EventArgs e)
{
    base.OnHandleCreated(e);

    _uiContext = SynchronizationContext.Current
        ?? throw new InvalidOperationException(
            "The handle must be created on the WinForms UI thread.");

    _uiThreadId = Environment.CurrentManagedThreadId;
}

private void RunOnUiThread(Action callback)
{
    if (Environment.CurrentManagedThreadId == _uiThreadId)
    {
        callback();
        return;
    }

    _uiContext!.Post(
        static state => ((Action)state!).Invoke(),
        callback);
}
```

Create and use UI-owned COM objects inside that dispatch:

```csharp
RunOnUiThread(() =>
{
    // CreateTargetForHwnd, CreateVisual, SetContent, Commit, and release
    // all stay in the same apartment.
});
```

The worker can continue drawing and presenting free-threaded resources without
moving every frame onto the UI thread.

## Avoid deadlocks

Do not synchronously call `Control.Invoke` or `SynchronizationContext.Send` while
holding a render/resource lock. The UI thread may be disposing the same renderer
and waiting for that lock.

Prefer:

1. Finish locked render work.
2. Release the lock.
3. Post the small UI-owned COM operation.
4. Recheck disposed state and resource generation inside the posted callback.

Use a generation number when device loss, camera switching, or handle recreation
can make queued callbacks stale.

## Error handling

Do not swallow exceptions raised by posted COM work, and do not let them become
unhandled UI-loop exceptions. Preserve the first failure and report or rethrow it
through the application's existing error path.

Device-loss recovery and apartment errors are different:

- expected device-loss HRESULTs may rebuild the graphics object graph;
- `E_NOINTERFACE`, invalid casts, and wrong-thread errors require fixing ownership
  or marshalling, not retrying the same cross-apartment call;
- broad catches must not turn a broken renderer into a silent black surface.

## Review checklist

- Identify which thread/apartment creates every COM object.
- Verify the documented threading model for each interface family.
- Keep HWND-bound object creation and lifecycle on the UI thread.
- Never assume an RCW, a C# lock, or CsWin32 provides apartment marshalling.
- Do not read WinForms control properties from render/capture callbacks; snapshot
  simple state on the UI thread.
- Do not synchronously marshal to the UI while holding a lock the UI may need.
- Ignore stale posted work after disposal, device recreation, or handle changes.
- Release COM objects on their owning apartment and in dependency-safe order.
- Surface asynchronous native failures through the normal application error path.
- Exercise camera/device switching, clear/error states, resize, and shutdown, since
  these lifecycle transitions expose apartment bugs more reliably than steady state.
