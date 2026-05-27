# PublicDemos project instructions

## BranchComposer guidance

BranchComposer is a WinForms demo that consumes WARP Git services. Keep UI behavior in the demo and reusable Git/composition behavior in WARP.

For Branch-Set composition work:

- Validate selected branches against the current fetched base branch before composing.
- Avoid blanket `git cherry-pick -X theirs` conflict handling because it can lose cumulative file entries.
- Prefer explicit conflict handling: automate known-safe append-only/resource cleanup and surface code conflicts clearly.
- Preserve Designer-compatible patterns in `*.Designer.cs`; place behavior in regular code files.

If a requested approach is likely to hide a Git conflict, discard cumulative data, or rely on stale repository state, push back early with the safer WARP-side design.

## WinForms form review routing

When a user asks whether WinForms Forms/UserControls are "designed according to standard", "not designed properly", "badly designed", "layout looks wrong", or uses similar form-design wording, treat that as both:

- a Designer compatibility review using `winforms-designer-code`; and
- a human/layout/DPI review using `winforms-high-dpi-fluent-layout`.

Check both serialization correctness (`*.Designer.cs`, `InitializeComponent`, event wiring, backing fields, `Dispose`) and layout quality (task flow, high-DPI scaling, responsive containers, accessibility, tab order, and modal dialog ergonomics).
