# winvim

Vimium, but for the whole Windows desktop.

Press **Ctrl+Alt+F** in any app and winvim overlays a keyboard hint (like `AS`, `JK`, ...)
on every clickable thing in the foreground window — buttons, links, checkboxes, menu
items, list entries. Type the hint's letters and it's clicked, with no mouse involved.
Escape cancels.

It works by scanning the foreground window with Windows [UI Automation](https://learn.microsoft.com/windows/win32/winauto/entry-uiauto-win32)
for elements exposing an Invoke, Toggle, SelectionItem, or ExpandCollapse pattern, then
invokes that pattern directly (falling back to a synthetic click at the element's center
for anything that doesn't expose one of those).

## Status

Early MVP. One hotkey, one hint style, no configuration UI yet. See
[Known limitations](#known-limitations) and [Roadmap](#roadmap) below.

## Using it

1. Run `winvim.exe` — it sits quietly in the system tray (no window, no taskbar entry).
2. Focus any app window.
3. Press `Ctrl+Alt+F`. Hints appear over clickable elements.
4. Type a hint's letters (case-insensitive) to click it, `Backspace` to correct a typo,
   `Escape` to cancel.
5. Right-click the tray icon to activate hints from there instead, or to exit.

## Building from source

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (10.0+) with the
Windows desktop workload, on Windows.

```bash
dotnet build
```

Run it:

```bash
dotnet run --project src/Winvim
```

Run the tests:

```bash
dotnet test
```

Publish a self-contained single-file exe (same as CI produces for releases):

```bash
dotnet publish src/Winvim -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

## Known limitations

- **DPI**: winvim runs DPI-unaware on purpose, so the coordinates UI Automation reports
  line up with the overlay's own coordinate space without per-monitor scaling math. On
  high-DPI displays this can make winvim's own hint labels render slightly soft; it does
  not affect click accuracy.
- **Deep automation trees**: some apps (browsers especially) build very large UI
  Automation trees. The scanner caps itself at 400 elements per activation to keep things
  responsive, so an extremely dense page may not surface every element.
- **Single hotkey, single hint style**: `Ctrl+Alt+F` is hardcoded; there's no settings UI
  yet.

## Roadmap

- Configurable hotkey and hint alphabet
- Scroll and window-focus navigation (`hjkl`-style), not just clicking
- Settings window instead of tray-menu-only configuration
- Signed, installer-based releases

## Contributing

Changes land via pull request against `main` — see the repo's PR history for the
project's review conventions. CI builds and tests every PR on `windows-latest`.

## License

[MIT](LICENSE)
