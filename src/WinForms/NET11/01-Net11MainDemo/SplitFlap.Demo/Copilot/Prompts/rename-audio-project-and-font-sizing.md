# Rename the Audio Project and Add Font Sizing

For the `WinFormsNet11Demo.slnx` solution:

- Rename the `SplitFlap.Audio` project to `WinForms.Audio`.
- In `DrumMachine.Demo\DrumMachine.Demo.csproj`, add a font-size option with these choices:
  - Small (WinForms standard)
  - Normal
  - Large
  - XXL
- Increase the font size in two-point steps relative to the currently selected font:
  - Small uses the current WinForms standard size.
  - Normal adds 2 points.
  - Large adds 4 points.
  - XXL adds 6 points.

The sizing must remain relative when different controls use different font sizes.
Do not replace those deliberate size differences with one absolute font size.

As far as possible, commit and push the completed, validated changes.
