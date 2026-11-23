# Sample Details

Detailed write-ups for each sample: what it demonstrates, key files, and code
examples. For requirements, build/run instructions and troubleshooting, see the
[README](./README.md).

- [TodoApp](#todoapp)
- [ShellDemo](#shelldemo)
- [WebViewDemo](#webviewdemo)
- [BlazorDemo](#blazordemo)
- [MediaDemo](#mediademo)
- [MapsDemo](#mapsdemo)
- [SplitPaneDemo](#splitpanedemo)

## TodoApp

A complete task management application demonstrating production-ready patterns.

**Features:**
- **NavigationPage** with toolbar and back navigation
- **CollectionView** with item selection and data templates
- **XAML data binding** with INotifyPropertyChanged
- **Value converters** for dynamic styling (alternating rows, completed tasks)
- **DisplayAlert** dialogs for confirmations
- **Theme switching** (Light/Dark) with AppThemeBinding
- **Grid layouts** with star sizing
- **Entry and Editor** for text input
- **Alternating row colors** with theme support

**Architecture:**
- `TodoItem.cs` - Data model with property change notifications
- `TodoService.cs` - Business logic with ObservableCollection
- `TodoListPage.xaml` - Main list view with data templates
- `TodoDetailPage.xaml` - Detail/edit view
- `NewTodoPage.xaml` - Create new task view
- Value converters for visual state management

**Code Example - Data Binding:**

```xaml
<!-- TodoListPage.xaml -->
<CollectionView ItemsSource="{Binding Todos}" SelectionMode="Single">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Grid Padding="15,10" 
                  BackgroundColor="{Binding Index, Converter={StaticResource AlternatingRowColorConverter}}">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="4"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                
                <!-- Status indicator -->
                <BoxView Grid.Column="0" 
                         BackgroundColor="{Binding IsCompleted, Converter={StaticResource CompletedToColorConverter}, ConverterParameter=indicator}"/>
                
                <!-- Task content -->
                <VerticalStackLayout Grid.Column="1" Spacing="4">
                    <Label Text="{Binding Title}" 
                           FontSize="16"
                           TextColor="{Binding IsCompleted, Converter={StaticResource CompletedToColorConverter}}"
                           TextDecorations="{Binding IsCompleted, Converter={StaticResource CompletedToTextDecorationsConverter}}"/>
                    <Label Text="{Binding Notes}" 
                           FontSize="12"
                           MaxLines="2"
                           LineBreakMode="TailTruncation"
                           TextColor="{Binding IsCompleted, Converter={StaticResource CompletedToColorConverter}, ConverterParameter=notes}"/>
                </VerticalStackLayout>
            </Grid>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

**Logging:**
Application logs are written to `~/todoapp.log` for debugging.

## ShellDemo

A comprehensive control gallery showcasing all supported MAUI controls with Shell navigation.

**Features:**
- **Shell navigation** with flyout menu
- **All core controls**: Button, Entry, Editor, SearchBar, CheckBox, Switch, Slider, Stepper
- **Pickers**: Picker, DatePicker, TimePicker
- **CollectionView** with various data types and selection modes
- **ProgressBar and ActivityIndicator**
- **Grid layouts** with demonstrations of:
  - Auto, Star, and Absolute sizing
  - Row and column spanning
  - Spacing and padding
  - Mixed sizing strategies
- **Real-time event logging** for all interactions
- **Push/pop navigation** examples
- **Theme switching** support
- **Drag & drop** (XDND / Wayland data-device) with a drop zone, drag source, and event log
- **Clipboard and primary selection** (middle-click paste buffer)
- **System tray icon** (StatusNotifierItem / XEmbed) with menu actions
- **CUPS printing**: printer enumeration, GTK print dialog, Skia-rendered test page

**Navigation Structure:**

```csharp
// AppShell.xaml.cs - Shell with flyout menu
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for push navigation
        Routing.RegisterRoute("detail", typeof(DetailPage));
    }
}
```

**Code Example - Grid Layouts:**

```csharp
// GridsPage.xaml.cs - Programmatic grid creation
private View CreateStarSizingDemo()
{
    var grid = new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
        }
    };
    
    // Add cells with proportional sizing: 25% | 50% | 25%
    grid.Children.Add(CreateCell("1*", "#BBDEFB"));
    grid.Children.Add(CreateCell("2* (double)", "#C8E6C9"));
    grid.Children.Add(CreateCell("1*", "#FFECB3"));
    
    return grid;
}
```

**Logging:**
Application logs are written to `~/shelldemo.log` for debugging.

## WebViewDemo

A web browser application demonstrating the WebView. With WPE WebKit installed the page is composited inside the Skia render tree and the app runs in native Wayland/X11 mode; without it the sample falls back to the GTK-hosted WebKitGTK view. `OPENMAUI_WEBVIEW=wpe|webkitgtk` forces a backend.

**Features:**
- **WebView** with full HTML5 support
- **WPE WebKit** backend (WebKitGTK fallback), context menus, clipboard
- **Navigation controls** (back, forward, reload)
- **URL entry** with automatic https:// prefix
- **JavaScript evaluation** via `EvaluateJavaScriptAsync`
- **HTML content loading** from strings
- **Progress indicator** with animations
- **Theme switching** support
- **GTK mode** for native WebView rendering

**Important:** Requires GTK mode enabled in `Program.cs`:

```csharp
// WebViewDemo/Program.cs
LinuxApplication.Run(app, args, options =>
{
    options.UseGtk = true;  // Required for WebView
});
```

**Code Example - JavaScript Evaluation:**

```csharp
private async void OnEvalJsClicked(object? sender, EventArgs e)
{
    try
    {
        var result = await MainWebView.EvaluateJavaScriptAsync("document.title");
        StatusLabel.Text = $"JS Result: {result ?? "(null)"}";
    }
    catch (Exception ex)
    {
        StatusLabel.Text = $"JS Error: {ex.Message}";
    }
}
```

**Code Example - Loading HTML:**

```csharp
private void OnLoadHtmlClicked(object? sender, EventArgs e)
{
    var html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial; margin: 40px; }
        button { padding: 15px 30px; font-size: 1.1em; }
    </style>
</head>
<body>
    <h1>Hello from OpenMaui!</h1>
    <button onclick=""alert('Hello from JavaScript!')"">Click Me!</button>
</body>
</html>";

    MainWebView.Source = new HtmlWebViewSource { Html = html };
}
```

**Logging:**
Application logs are written to `~/webviewdemo.log` for debugging.

## BlazorDemo

Blazor Hybrid on Linux: a MAUI (Skia) toolbar above a `BlazorWebView` hosted in WPE WebKit, in native Wayland/X11 mode. Requires WPE WebKit 2.54+ and the `OpenMaui.Controls.Linux.Blazor` package (`.UseLinuxBlazorWebView()`).

**Features:**
- **Razor components** rendered by WebKit, served through the `app://localhost/` scheme
- **Counter** (`@onclick`): JavaScript to .NET to JavaScript round trip
- **Two-way binding** (`@bind`) on an input
- **DI-injected .NET service** (`WeatherService`) consumed by a component
- **External links** opened in the system browser through `UrlLoading`
- **`BlazorWebViewInitialized`** surfacing in the MAUI toolbar

```bash
cd BlazorDemo
dotnet build && ./run.sh
```

## MediaDemo

A video and audio player demonstrating `CommunityToolkit.Maui.MediaElement` on Linux via the **OpenMaui.Controls.Linux.MediaElement** sibling package (GStreamer backend).

**Features:**
- **MediaElement** with full playback (play / pause / stop / seek)
- **Position slider** with live time display, drag-to-seek
- **Volume slider** + mute toggle
- **URL entry** for loading arbitrary streams or local files (mp4, mkv, mp3, http/https, rtsp)
- **Aspect-correct video rendering** in a Skia surface (AspectFit by default)
- **Hardware-accelerated decode** via VAAPI / NVDEC when those GStreamer plugins are installed

**Runtime requirements (Linux):**
- GStreamer 1.x core (almost always preinstalled on desktop Linux)
- Plugin sets: `gstreamer1-plugins-good`, `-bad-free`, `-ugly-free`, `-libav` (broad codec coverage)
- Optional: `gstreamer1-vaapi` (Intel/AMD HW decode) or `gstreamer1-nvdec` (NVIDIA)

See the **System Dependencies** section near the top of this README for distro-specific install commands.

**Scrub-accuracy note:** For local files, seeks land frame-accurately. For HTTP-streamed sources, backward seeks can land 1–2 seconds off the requested position because the pipeline must re-request earlier byte ranges and decode-and-discard back to the target — this is a fundamental property of streaming over HTTP and matches the behavior of YouTube / Vimeo. Forward scrubs and click-to-position are typically accurate within a frame.

**Important:** The sample's `MauiProgram.cs` opts in to the Linux backend explicitly. Cross-platform projects keep `.UseLinuxMediaElement()` in their builder chain — it's a safe no-op on Windows / Android / iOS / macCatalyst where the toolkit's own handlers serve the same MediaElement control:

```csharp
// MediaDemo/MauiProgram.cs
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkitMediaElement(isAndroidForegroundServiceEnabled: false)
    .UseLinux()
    .UseLinuxMediaElement();   // Linux backend; no-op elsewhere
```

**Code Example - playback controls:**

```xaml
<toolkit:MediaElement x:Name="Player"
                      Aspect="AspectFit"
                      Source="https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/720/Big_Buck_Bunny_720_10s_1MB.mp4"
                      Volume="0.8" />
```

```csharp
private void OnPlayClicked(object? sender, EventArgs e)  => Player.Play();
private void OnPauseClicked(object? sender, EventArgs e) => Player.Pause();
private void OnStopClicked(object? sender, EventArgs e)  => Player.Stop();

private async void OnPositionDragCompleted(object? sender, EventArgs e)
{
    await Player.SeekTo(TimeSpan.FromSeconds(PositionSlider.Value));
}
```

**Logging:**
Application logs are written to `~/mediademo.log` for debugging.

## MapsDemo

A map view demonstrating `Microsoft.Maui.Controls.Maps` on Linux via the **OpenMaui.Controls.Linux.Maps** sibling package. Renders OpenStreetMap raster tiles in SkiaSharp.

**Features:**
- **`<maps:Map>` cross-platform XAML** — same control as Apple Maps / Google Maps / Bing on the other platforms
- **OpenStreetMap raster tiles** fetched from `tile.openstreetmap.org` and cached under `$XDG_CACHE_HOME/openmaui/osm-tiles`
- **Six pins** placed on major Asian capitals (Tokyo, Seoul, Beijing, Hanoi, Bangkok, Singapore)
- **Polyline overlay** tracing the Trans-Siberian Railway approximation (Moscow → Yekaterinburg → Novosibirsk → Irkutsk → Vladivostok)
- **Polygon overlay** outlining Central Park, New York, with a semi-transparent fill and visible stroke
- **Circle overlay** — a 5 km radius (`Distance.FromKilometers`) around central Paris, semi-transparent fill + stroke
- **Runtime overlay toggles** — Add/Remove Polygon and Add/Remove Circle buttons mutate `Map.MapElements` on the fly, exercising the handler's element change-notification path
- **Pan and zoom** with mouse drag + scroll wheel; zoom toward cursor position
- **Quick-jump buttons** to Paris / London / New York / Tokyo / Singapore / Sydney / Full World
- **Attribution overlay** rendered per the OSM tile usage policy

**Runtime requirements (Linux):**
- Internet access to `tile.openstreetmap.org` for the first view of each tile (subsequent views serve from disk cache)
- No native libraries beyond what the main `OpenMaui.Controls.Linux` package already requires

**Cross-platform note:** Like MediaDemo, the same `MauiProgram.cs` works on every platform — the Linux opt-in lines are no-ops elsewhere where the platform-native Maps backend serves the same control:

```csharp
// MapsDemo/MauiProgram.cs
builder
    .UseMauiApp<App>()
    .UseMauiMaps()       // upstream MAUI Maps
    .UseLinux()          // base Linux handlers
    .UseLinuxMaps();     // Linux Maps backend (OSM); no-op elsewhere
```

**Code example — pin, route, polygon + circle:**

```xaml
<maps:Map x:Name="Map" IsScrollEnabled="True" IsZoomEnabled="True" />
```

```csharp
Map.Pins.Add(new Pin
{
    Label = "Tokyo",
    Location = new Location(35.6762, 139.6503),
    Type = PinType.Place,
});

var route = new Polyline { StrokeColor = Color.FromArgb("#1976D2"), StrokeWidth = 4 };
route.Geopath.Add(new Location(55.7558, 37.6173));  // Moscow
route.Geopath.Add(new Location(43.1198, 131.8869)); // Vladivostok
Map.MapElements.Add(route);

// Filled polygon — Geopath closes automatically
var park = new Polygon
{
    FillColor = Color.FromArgb("#4D2E7D32"),   // semi-transparent green
    StrokeColor = Color.FromArgb("#2E7D32"),
    StrokeWidth = 3,
};
park.Geopath.Add(new Location(40.7677, -73.9816)); // Central Park SW
park.Geopath.Add(new Location(40.8003, -73.9580)); // NW
park.Geopath.Add(new Location(40.7969, -73.9494)); // NE
park.Geopath.Add(new Location(40.7642, -73.9730)); // SE
Map.MapElements.Add(park);

// Circle — Radius is a ground distance, so it scales with zoom
Map.MapElements.Add(new Circle
{
    Center = new Location(48.8566, 2.3522),    // Paris
    Radius = Distance.FromKilometers(5),
    FillColor = Color.FromArgb("#40E64A19"),   // semi-transparent orange
    StrokeColor = Color.FromArgb("#E64A19"),
    StrokeWidth = 3,
});

Map.MoveToRegion(MapSpan.FromCenterAndRadius(
    new Location(45, 100), Distance.FromKilometers(5000)));
```

**Customizing the tile source:** OSM's tile policy throttles heavy traffic. Apps that produce significant load should swap to a self-hosted or commercial tile server by overriding `OsmTileService.Default.UrlTemplate` (uses `{z}/{x}/{y}` placeholders) early in startup.

**Logging:**
Application logs are written to `~/mapsdemo.log` for debugging.

## SplitPaneDemo

A two-pane (left / right) layout whose divider can be dragged to resize the panes, showing how to build a split view without a dedicated splitter control.

**Features:**
- **Draggable split-pane divider** — a thin middle `Grid` column carrying a `PanGestureRecognizer`
- **Mixed sizing** — the left pane is an absolute `GridLength` rewritten on each drag; the right pane is `*` and takes the remainder
- **Clamping** — minimum widths keep either pane from collapsing
- **Preset widths** — 25% / 50% / 75% / Reset set the left column directly
- **Input-transparent chrome** — the visible rail and grab handle never steal the drag
- **Resize cursor** — horizontal-resize cursor over the divider (Linux)
- **Light / dark themes** with `AppThemeBinding`
- **Selection binding** — selecting a folder in the left pane updates the right pane

**Navigation structure:** a single page; the layout is a three-column `Grid` (`ColumnDefinitions="300,18,*"`) whose middle column is the divider.

**Code example - the divider and its gesture:**

```xml
<Grid x:Name="SplitGrid" ColumnDefinitions="300,18,*" ColumnSpacing="0">

    <!-- left pane (absolute width, rewritten while dragging) -->
    <Grid Grid.Column="0"> ... </Grid>

    <!-- the draggable divider: a full-height, transparent hit target that owns the gesture -->
    <BoxView Grid.Column="1" x:Name="Divider" Color="Transparent">
        <BoxView.GestureRecognizers>
            <PanGestureRecognizer PanUpdated="OnDividerPanUpdated" />
        </BoxView.GestureRecognizers>
    </BoxView>

    <!-- the visible rail + grab handle, drawn on top; InputTransparent keeps it draggable -->
    <BoxView Grid.Column="1" InputTransparent="True" WidthRequest="2" HorizontalOptions="Center" Color="..." />

    <!-- right pane (star = remaining space) -->
    <Grid Grid.Column="2"> ... </Grid>
</Grid>
```

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
            // TotalX is the translation since the press: add it to the width at gesture start.
            SetLeftWidth(_leftWidthAtGestureStart + e.TotalX);
            break;
    }
}

private void SetLeftWidth(double width)
{
    var maxLeft = Math.Max(MinLeftWidth, SplitGrid.Width - DividerWidth - MinRightWidth);
    SplitGrid.ColumnDefinitions[0].Width = new GridLength(Math.Clamp(width, MinLeftWidth, maxLeft));
}
```

**Display server:** uses whatever OpenMaui detects, preferring Wayland when `WAYLAND_DISPLAY` is set (native Wayland, including `wp_fractional_scale_v1`, so it stays crisp on fractional-scale/HiDPI displays). Set `MAUI_PREFER_X11=1 dotnet run` to force the X11 / XWayland backend.

**Logging:**
Application logs are written to `~/splitpanedemo.log` for debugging.
