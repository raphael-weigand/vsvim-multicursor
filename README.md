# VsVim MultiCursor

A small Visual Studio 2022 VSIX extension to add native editor carets above and below the current caret(s), without creating a rectangular/box selection or entering VsVim Visual Block mode.

## Intended commands

- **MultiCursor.AddBelow** — add an independent native caret on the next line (desired shortcut `Alt+J`).
- **MultiCursor.AddAbove** — add an independent native caret on the previous line (desired shortcut `Alt+K`).

### Required behavior

- Use Visual Studio's native `IMultiSelectionBroker` API, not a box selection (`SetBoxSelection`) and not `Edit.LineDownExtendColumn` / `Edit.LineUpExtendColumn`.
- Retain all existing carets and make repeated invocation add more carets in the chosen direction.
- Preserve reasonable character-column / virtual-space behavior on shorter and longer lines.
- Do not replace or interfere with *Select Next Occurrence* or existing VsVim word motions.
- Avoid duplicate carets and handle the first/last document line.
- Expose both operations as Visual Studio commands so shortcuts can be configured in **Tools → Options → Environment → Keyboard**. Check interaction with VsVim key handling before assigning `Alt+J/K`.

## Status

Initial VSIX source implementation committed. **Not yet compiled or tested** in Visual Studio 2022. SDK API compatibility, command registration, and packaging still need verification before installation.

## References

- [Microsoft: IMultiSelectionBroker](https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.text.imultiselectionbroker)
- [Microsoft: Visual Studio 2022 extension compatibility](https://learn.microsoft.com/en-us/visualstudio/extensibility/migration/update-visual-studio-extension?view=visualstudio)
