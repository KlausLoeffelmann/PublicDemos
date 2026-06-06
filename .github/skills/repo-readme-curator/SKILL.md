---
name: repo-readme-curator
description: >-
 Maintain a repository's central README by detecting newly added or
 substantially extended runnable demo apps, tools, and prototypes from recent
 PRs, and rotating the "currently featured" app into a dated history section.
 Use this skill whenever the user asks to "update the README", "refresh the
 repo readme", "feature the new app/tool/demo", or otherwise asks to bring the
 central README in sync with what landed recently — even if they don't name a
 specific PR. This is for experimentation/demo repos where Tools, TestApps, and
 Prototypes are added frequently and each needs to be surfaced and linked.
---

# Repo README Curator

Keep a repo's central `README.md` current by featuring the newest runnable
addition and demoting the previous feature into a dated history section. Built
for repos where demo apps, tools, and prototypes land continuously and each
should be discoverable from the top-level README.

## When this runs

The user asks to update the central README after one or more PRs merged. The
job is to decide *whether* anything README-worthy landed, and if so, rewrite the
README to feature it.

## What qualifies as a feature

An addition is README-worthy only if it is a **new runnable app** OR a
**substantial extension of an existing runnable app**. A runnable app is a
Tool, TestApp, or Prototype that builds and runs on its own (has an entry point
— e.g. a `.csproj` producing an `Exe`, a launchable executable, a startup
project). Use this bar to filter:

- **Qualifies:** a new project folder with its own runnable entry point; an
 existing app gaining a significant new capability (new screens, new mode, a
 meaningfully new scenario it now demonstrates).
- **Does NOT qualify:** library-only changes, refactors, test-only changes,
 doc tweaks, dependency bumps, formatting, small bugfixes. These may still be
 worth a "Recent Bugfixes/Changes" line (see below) but never become *the*
 feature and never de-throne the current feature.

When unsure whether a change is "substantial", say so and ask the user rather
than guessing — a wrong de-throne is annoying to undo.

## Workflow

Follow these steps in order.

### 1. Gather recent PRs

Identify PRs merged since the README was last updated. Use the available git/PR
tooling (`gh pr list --state merged`, `git log`, or whatever the environment
provides). For each, capture: title, description/body, merge date, PR link, and
the set of added/changed paths.

### 2. Classify each PR

For each PR, decide using the criteria above:

- **New/extended runnable app** → feature candidate.
- **Notable fix or change** (but not a new app) → "Recent Bugfixes/Changes"
 line candidate.
- **Neither** → ignore.

If multiple PRs qualify as feature candidates, the **most recent** one becomes
the new feature; the others, if still app-related, are noted but only the newest
takes the feature slot. Confirm with the user if it's ambiguous which should be
featured.

### 3. Ensure the app has its own README

For each feature candidate, locate the app's subfolder and check for a README
(`README.md` in that folder).

- **Has one** → use it as the link target. Leave it as is.
- **No README, but the PR body has a usable description** → create the app's
 `README.md` from the PR description. "Usable" means it actually explains what
 the app is/does — not just "fixes #123" or a checklist. Clean it into prose:
 a short title (H1 with the app name), what it is, how to run it. Strip PR
 boilerplate (review checklists, CI noise).
- **No README and no usable PR description** → don't fabricate one. Note this
 to the user and ask for a one-line description, or proceed featuring the app
 with a link to its folder instead of a dedicated README.

### 4. Rotate the feature

Only when a *qualifying* new/extended app exists:

1. Take the **current** featured app's section and condense it into a
 brief summary (2–4 sentences capturing what it is).
2. Move that summary into the **history** section as a new entry at the top
 (most recent first), under a heading:
 `## Was new on {ddd, MMMM dd. yyyy}: {FeatureName}`
 followed by the brief summary and, on its own line, a link to that app's own
 README.
3. Promote the new app into the feature slot.

If nothing qualifies, **leave the feature slot untouched** — do not rotate.
You may still append "Recent Bugfixes/Changes" lines from step 2.

### 5. Write the README

Use this exact structure. Preserve any existing intro prose verbatim unless the
user asks to change it.

```markdown
# ReadMe

{Introduction and short explanation of the repo — preserved from existing README.}

## {FeatureName} — published {ddd, MMM dd., yyyy}

{Full description of the currently featured app. Ends with a link to the app's own README.}

## Recent Bugfixes/Changes:

- {ddd, MMM dd. 'yy}: {FeatureName} — [{ChangeDescription}]({LinkToPr})
- {ddd, MMM dd. 'yy}: {FeatureName} — [{ChangeDescription}]({LinkToPr})

## Was new on {ddd, MMMM dd. yyyy}: {FeatureName}

{BriefSummaryOfOriginalFeatureText}

[{FeatureName} README]({LinkToFeatureReadMe})

## Was new on {ddd, MMMM dd. yyyy}: {EarlierFeatureName}

{...}
```

Notes on the structure:

- The **feature** heading uses the short month form: `ddd, MMM dd., yyyy`
 (e.g. `Sat, Jun 06., 2026`).
- The **history** headings use the long month form: `ddd, MMMM dd. yyyy`
 (e.g. `Sat, June 06. 2026`).
- The **bugfix** line dates use the short year form: `ddd, MMM dd. 'yy`
 (e.g. `Sat, Jun 06. '26`).
- History entries are ordered newest-first. Keep the section "brief" — these
 are pointers, not full docs. If it grows long over time, suggest trimming the
 oldest entries (or moving them to a `HISTORY.md`), but ask before deleting.
- Don't duplicate: an app should appear in exactly one of feature / history at
 a time.

### 6. Report

Summarize what you did: which PR was featured, what was de-throned, any READMEs
created, and any bugfix lines added. Flag anything you skipped or were unsure
about (e.g. an app with no description) so the user can fill the gap.

## Date formatting reference

| Slot | Format | Example |
|------|--------|---------|
| Feature heading | `ddd, MMM dd., yyyy` | `Sat, Jun 06., 2026` |
| History heading | `ddd, MMMM dd. yyyy` | `Sat, June 06. 2026` |
| Bugfix line | `ddd, MMM dd. 'yy` | `Sat, Jun 06. '26` |

Use the PR **merge** date, not today's date, when dating a feature or change.
