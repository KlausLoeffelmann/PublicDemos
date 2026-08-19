# Logical captions, camera motion, and world coordinates — a session postmortem

**Theme:** Logical (stock OLED-oriented family)  
**Area:** caption placement + storm reconstruction animation  
**Outcome:** a clean world / camera / pixel model, validated with composition tests across real design scales  

This note captures what went wrong, why “green tests” were not enough, and how a deliberate multi-model review finally produced the correct architecture.

---

## 1. What we were trying to achieve

The Logical theme periodically stages a dramatic reconstruction:

1. Clock at its original (burn-in-safe) position  
2. Elements begin to shake / flash  
3. The **whole scene** pans and zooms out  
4. Individual clock parts fly, one wave after another, to a far corner  
5. Parts **remain** at their destination-relative places and only settle  
6. The scene pans / zooms back so the reconstructed clock is recentered (OLED offsets considered)  
7. Weekday, date, and optional timezone captions then fly into their resting places  

Caption placement was refined twice in the same effort:

### Requirement set A — scenic captions

- Weekday and date live **outside** the clock face, alternating upper corners.  
- Timezone (when shown) may use upper-left, top-center, or upper-right.  
- Scene pan/zoom must affect captions the same way it affects the clock — even when that carries them **off-screen**.  
- After recentering, captions that are partially or fully off-screen fly back into place relative to the new clock position.

### Requirement set B — world-fixed screen-top anchors (clarification)

This superseded the “clock-face top” reading of placement:

- At rest, labels align to the **top of the whole screen**, not the top of the dial:  
  - weekday → padded screen-top **left**  
  - date → padded screen-top **right**  
  - timezone (optional) → padded screen-top **center** (or another upper slot)  
- Those positions are **world coordinates**. They stay put in the scene.  
- When we pan/zoom, labels only *appear* to move because the **camera** moves. They can leave the visible rectangle without ever changing their world anchors.  
- Separately, the **clock actually moves**: parts fly and reconstruct at a new physical place.  
- Only after recentering do captions optionally relocate in world space (fly to newly chosen padded top slots).

In short: **camera motion is not the same as object motion**, and captions are scenery in the scene, not ornaments glued to the dial.

---

## 2. Why this was hard

WarpClock already has three implicit spaces that are easy to conflate:

| Space | Meaning |
|--------|---------|
| **World / design units** | Theme-authored positions (dial radius 500, Logical’s normalized viewport with min side 1000). |
| **Camera** | Pan + scale applied to the whole scene during staging and recenter. |
| **Pixels** | Engine layout anchors + `AnchorOffset * DesignScale` (+ OLED origin). |

A theme can only set design-unit levers (`AnchorOffset`, scale, etc.). The engine multiplies offsets by `DesignScale = min(W,H) / 1000`. Any time a theme mixes **design homes into pixel anchors**, or treats **clock destination** as **camera offset**, the math looks fine on a 1000-short-side surface and fails everywhere else.

Logical’s animation also overloads one snapshot field (`SceneOffset`) to mean different things in different phases: sometimes “where the camera is,” sometimes “where the reconstructed clock sits.” That overload is exactly where the worst bugs hid.

---

## 3. Failed and partial attempts (in order)

### Attempt 1 — Clock-relative caption slots

Captions were placed with fixed design vectors near the dial (upper-left / upper-middle / upper-right of the **clock**).

- Matched an early reading of “upper corners.”  
- Failed the clarification: labels sat on the face, not on the **screen** top.  
- Pan/zoom “participation” was faked by fading or parking labels rather than transforming true scene anchors.

### Attempt 2 — Scene-attached slots with fade / hide during flight

Labels followed `SceneOffset` during escalate/zoom-out, then were hidden during flight and faded back after recenter.

- Still clock-ish geometry, not padded viewport-top.  
- Hiding broke the “same pan/zoom even off-screen” rule.  
- Arrival still felt like a UI fade, not scenery carried by a camera.

### Attempt 3 — “World anchors” that were still clock-relative

We introduced `_labelWorldAnchors` and spoke about world coordinates, but targets were still derived as:

`camera-ish offset + clock-relative slot vector`

- Tests asserted animator formulas with `home + AnchorOffset` on surfaces where `DesignScale == 1`.  
- On real windows (800×600, 1920×1080, 4K), pixel composition drifted by tens to hundreds of pixels.  
- `LogicalLayout` added **design-unit** homes to a **pixel** center — correct only when the short side was exactly 1000 px.

### Attempt 4 — Viewport-top anchors without a true camera split

Padded screen-top placement was added, and labels were said to be world-fixed, but:

- **Reassembly** still seeded part offsets from `SceneOffset` (destination) and then treated the source→destination delta as “jitter,” double-applying the corner jump and throwing the assembled clock off-screen for the reassembly window.  
- **Initialize** ran before the engine wrote the real `SurfaceSize` (default 1000×1000), so production captions started wrong and stayed wrong until a full cycle.  
- **Resize / DPI** did not re-pin settled captions.  
- **Timezone** could share a corner with weekday/date at nearly the same Y and overdraw text.  
- **Cycle-boundary rebase** used the previous frame’s snapshot, so hitches could pop labels.  
- Tests never crossed the FlyingOff→Reassembling boundary through the live animator path for movers, and never simulated engine pixel composition at `DesignScale ≠ 1`.

Net: the product looked “almost right” in the happy path the tests modeled, and wrong under the conditions the dual review later enumerated.

---

## 4. What finally worked: dedicated multi-model review

After several implementation iterations, we stopped coding and asked for **independent architecture reviews** against both requirement texts (A and B), with explicit instructions **not** to treat passing tests as proof of correctness.

### Models involved

| Model | Role |
|--------|------|
| **Claude Opus 5** (`claude-opus-5`, high reasoning) | Deep read of animator, layout, engine `ResolveAnchor` / `BuildSceneIfReady`, and tests. Produced severity-ordered defects with file/line references. |
| **Grok 4.6** (`grok-4.6`) | Second independent pass on the same corpus and the same two requirement blocks. Converged on the same root causes and the same recommended model. |

*(A first Grok launch failed only because an unsupported reasoning-effort setting was requested; the retry completed normally.)*

### What the reviewers agreed on (high confidence)

1. **Layout unit bug** — `LogicalLayout` mixed design homes into pixel anchors; engine then scaled `AnchorOffset` again. Wrong whenever `DesignScale ≠ 1`.  
2. **Init order bug** — animator `Initialize` saw default `1000×1000` before the engine set the real surface.  
3. **Reassembly double-offset** — camera/destination overload caused arrived parts to leave the screen during Reassembling.  
4. **Missing composition tests** — suite validated animator algebra, not final pixels.  
5. **Timezone collision** — shared upper corners needed stacking or exclusion.  
6. **Recommended architecture** — name three spaces and stop overloading one offset field:

```text
screenDesign = CameraOffset + world × CameraScale
pixel        = layoutCenterPx + screenDesign × DesignScale   (+ OLED origin)
```

- Captions: persistent **world** anchors; inverse-camera map from padded screen-top slots when pinning.  
- Escalate / zoom-out / zoom-in: change **camera only**.  
- Flight / reassembly: freeze camera at source staging; **physically** interpolate part worlds to destination-relative positions; hold and settle.  
- After recenter: optional caption **world** relocation (fly), not another camera cheat.

They also called out the only real **wording ambiguity**: request A’s “relative to the new clock” vs request B’s “padded screen top.” Both treated B as the later clarification for resting placement, while keeping those anchors fixed in world space until an explicit post-recenter flight.

### Implementation after the review (short)

The implementation pass that followed the reviews did not invent a new story; it **executed** that model:

- Camera helper stays at **source** during FlyingOff and Reassembling; ZoomingIn back-solves camera so a rigid reconstructed dial recenters.  
- Flight samples absolute screen positions; no destination seed + jitter double count.  
- Label layout returns **pixel center only**; `AnchorOffset` is full design-space screen position.  
- Engine sets `SurfaceSize` before `Initialize`.  
- Calm / pinned captions re-anchor on viewport change.  
- Cycle rebase subtracts the completed cycle’s physical clock offset using **pre-transition** staging (not the last frame’s progress).  
- Timezone stacks on a second row when it shares a corner with weekday/date.  
- New tests: `DesignScale` composition at 800×600 / 1080p / 4K, reassembly without double-offset, camera-at-source during reassembly, center-only layout.

**Validation after the fix:** full WarpClock solution build with 0 warnings/errors; full test pass (including the new composition cases).

---

## 5. Lessons worth keeping

1. **Name the spaces in code.** If camera and clock world share one field, bugs will look like “animation glitches” and test as green on unit squares.  
2. **DesignScale = 1 is a lie.** Any layout/offset test that only uses `min(surface) == 1000` is not a placement test.  
3. **Initialize is part of the contract.** Host must publish real surface size before theme init; themes must re-pin viewport-relative content on resize when still “pinned.”  
4. **Passing tests can encode the wrong model.** Prefer at least one test that walks the same composition path the engine uses (`layout anchor px + AnchorOffset × DesignScale`).  
5. **When stuck, change the process, not only the code.** Two strong models reviewing the *requirements + implementation* without edit rights produced a sharper diagnosis in one pass than several implement-and-patch loops.  
6. **Clarify superseding requirements explicitly.** “Screen top in world coordinates” vs “upper corners of the clock” is not a polish difference; it is a different coordinate story.

---

## 6. Suggested mental model (final)

```text
Calm:        camera = safeOffset, clock world = 0, captions = padded top worlds
Escalate:    camera → source staging (zoom out), worlds frozen
Zoom out:    camera finishes at source, worlds frozen
Fly:         camera frozen at source; parts bezier source→destination (screen)
Reassemble:  camera still source; parts hold destination, settle only
Zoom in:     camera recenters around rigid reconstructed dial; captions ride camera
Calm':       rebase caption worlds into new origin; fly to new padded top slots
```

Captions never “move because the clock moved.”  
They move when the **camera** moves, or when we **explicitly** change their world anchors after recenter.

---

## 7. Related files

- `LogicalTheme.cs` — center-only caption layout  
- `LogicalThemeAnimator.cs` — camera, flight, world anchors, caption return  
- `LogicalThemeTests.cs` — composition / reassembly / camera regressions  
- `WarpClockControl.cs` — `SurfaceSize` before animator `Initialize`; pixel composition  

---

*Document distilled from the WarpClock platform-extension session that produced the Logical reconstruction animation and the subsequent caption/camera/world-coordinate corrections.*
