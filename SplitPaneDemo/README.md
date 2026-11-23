# SplitPaneDemo

A two-pane layout with a **draggable split-pane divider**, built on the OpenMaui
Linux platform. It answers a common question: how do you make a resizable
left/right layout in .NET MAUI when there is no built-in splitter control?

The trick is a three-column `Grid` whose **middle column is a thin, draggable
column**. A `PanGestureRecognizer` on that column rewrites the left column's
width as you drag.

```
+------------------+----+--------------------------------+
|                  |    |                                |
|   LEFT pane      | || |         RIGHT pane             |
|   (absolute px)  |    |         (star = remainder)     |
|                  |    |                                |
+------------------+----+--------------------------------+
   ColumnDefinitions="300,18,*"
                        ^
                        PanGestureRecognizer lives here
```

## Running

```bash
dotnet build
dotnet run          # or ./run.sh
```

The app logs to `~/splitpanedemo.log`.

### Debugging in VS Code

`.vscode/` is git-ignored, so create a `.vscode/launch.json` yourself, then press
<kbd>F5</kbd>. Point `program` at the managed **`.dll`** (not the apphost): the
`dotnet` host finds its own runtime, so no `DOTNET_ROOT` is required.

```jsonc
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "SplitPaneDemo (dotnet)",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/SplitPaneDemo/bin/Debug/net11.0/SplitPaneDemo.dll",
      "cwd": "${workspaceFolder}/SplitPaneDemo",
      "console": "internalConsole"
    }
  ]
}
```

This matters because if the debugger runs the native **apphost**
(`.../SplitPaneDemo`, no `.dll`) instead, that apphost looks for the runtime only in
`DOTNET_ROOT` / `/etc/dotnet/install_location` / `/usr/share/dotnet` - never on
`PATH`. With a per-user SDK under `~/.dotnet` and no `DOTNET_ROOT`, the app exits
with:

```
You must install .NET to run this application. ... DOTNET_ROOT = <not set>
The program '... SplitPaneDemo' has exited with code 131 (0x83).
```

To run the apphost anyway, add an `env` block with `DOTNET_ROOT` set to
`${env:HOME}/.dotnet`, or register the install location once (needs root):

```bash
echo "$HOME/.dotnet" | sudo tee /etc/dotnet/install_location_x64
```

### Display server

The app uses whatever OpenMaui detects, which **prefers Wayland** whenever
`WAYLAND_DISPLAY` is set (native Wayland, including `wp_fractional_scale_v1`, so
it is crisp on HiDPI/fractional-scale displays). It falls back to X11 / XWayland
automatically. To force X11 for testing:

```bash
MAUI_PREFER_X11=1 dotnet run
```

## How the divider works

**XAML** — the middle column holds a full-height, transparent `BoxView` that owns
the gesture, plus some input-transparent chrome:

```xml
<Grid Grid.Row="1" x:Name="SplitGrid" ColumnDefinitions="300,18,*" ColumnSpacing="0">

    <!-- left pane -->
    <Grid Grid.Column="0"> ... </Grid>

    <!-- the draggable divider -->
    <BoxView Grid.Column="1" x:Name="Divider" Color="Transparent">
        <BoxView.GestureRecognizers>
            <PanGestureRecognizer PanUpdated="OnDividerPanUpdated" />
        </BoxView.GestureRecognizers>
    </BoxView>

    <!-- chrome, drawn on top; InputTransparent keeps the whole column draggable -->
    <BoxView Grid.Column="1" InputTransparent="True"
             Color="{AppThemeBinding Light=..., Dark=...}" />
    <BoxView Grid.Column="1" InputTransparent="True" WidthRequest="2"
             HorizontalOptions="Center" Color="..." />

    <!-- right pane -->
    <Grid Grid.Column="2"> ... </Grid>
</Grid>
```

**Code-behind** — `PanGestureRecognizer` reports the *total* translation since the
press, so remember the width at gesture start and add the delta:

```csharp
private double _leftWidthAtGestureStart;

private void OnDividerPanUpdated(object? sender, PanUpdatedEventArgs e)
{
    switch (e.StatusType)
    {
        case GestureStatus.Started:
            _leftWidthAtGestureStart = SplitGrid.ColumnDefinitions[0].Width.Value;
            break;

        case GestureStatus.Running:
            SetLeftWidth(_leftWidthAtGestureStart + e.TotalX);
            break;
    }
}

private void SetLeftWidth(double width)
{
    var maxLeft = Math.Max(MinLeftWidth,
                           SplitGrid.Width - DividerWidth - MinRightWidth);
    SplitGrid.ColumnDefinitions[0].Width = new GridLength(Math.Clamp(width, MinLeftWidth, maxLeft));
}
```

### Notes / gotchas

- **Absolute + star sizing.** The left pane is an absolute `GridLength` that the
  drag rewrites; the right pane is `*`, so it absorbs the remainder.
- **Hit-testing.** The gesture lives on a full-column `BoxView` (a leaf view), not
  on a layout. In OpenMaui a press on a layout's own empty area does not deliver
  move events; a leaf view covers the whole column so any press on the divider
  starts the drag. The visual chrome is `InputTransparent` so it never steals the
  press.
- **Clamping.** `MinLeftWidth` / `MinRightWidth` keep either pane from collapsing.
- **Cursor.** On Linux the divider shows the horizontal-resize cursor
  (`CursorType.SizeWestEast`), set behind the `OPENMAUI_LINUX` compile symbol so
  the file still compiles for other MAUI platforms.
- **Presets.** The 25% / 50% / 75% / Reset buttons set the left column directly,
  showing the same `SetLeftWidth` path without a gesture.
