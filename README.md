# OpenMaui Linux Samples

Sample applications demonstrating [OpenMaui Linux](https://github.com/open-maui/maui-linux) - .NET MAUI on Linux desktop with SkiaSharp rendering.

> **This repository is based on [open-maui/maui-linux-samples](https://github.com/open-maui/maui-linux-samples),
> updated to build and run on .NET 11.**

## Table of Contents

- [Overview](#overview)
- [Sample Applications](#sample-applications)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Building and Deployment](#building-and-deployment)
- [Using the OpenMaui source](#using-the-openmaui-source)
- [Sample Details](#sample-details)
- [Project Structure](#project-structure)
- [Development Guide](#development-guide)
- [API Usage Examples](#api-usage-examples)
- [Troubleshooting](#troubleshooting)
- [Related Resources](#related-resources)
- [Support](#support)
- [License](#license)

## Overview

This repository contains production-ready sample applications showcasing **OpenMaui Linux** - a .NET MAUI implementation for Linux desktop environments. These samples demonstrate how to build cross-platform applications using familiar MAUI APIs with native Linux rendering via SkiaSharp and optional GTK integration.

**Key Features:**
- Full .NET MAUI API compatibility
- SkiaSharp-based rendering for high-performance graphics
- Native Wayland window management (preferred) with X11 / XWayland fallback
- Fractional-scale / HiDPI aware (`wp_fractional_scale_v1`)
- GTK integration for WebView and native dialogs
- Light/Dark theme support with dynamic switching
- Navigation (NavigationPage, Shell, push/pop)
- Data binding and MVVM patterns
- Gesture recognizers (tap, pan, pinch, swipe, drag & drop)
- Comprehensive control coverage

## Sample Applications

| Sample | Description | Key Features |
|--------|-------------|--------------|
| [TodoApp](./TodoApp/) | Full-featured task manager | NavigationPage, XAML data binding, CollectionView, value converters, theme switching |
| [ShellDemo](./ShellDemo/) | Comprehensive control showcase | Shell navigation, flyout menu, all core controls, event logging, drag & drop, clipboard/primary selection, system tray icon, CUPS printing |
| [WebViewDemo](./WebViewDemo/) | Web browser with WPE WebKit (WebKitGTK fallback) | WebView in native Wayland/X11 mode, JavaScript evaluation, context menus, clipboard, HTML rendering |
| [BlazorDemo](./BlazorDemo/) | Blazor Hybrid on Linux | BlazorWebView, Razor components, @onclick/@bind round trips, DI-injected services, UrlLoading |
| [MediaDemo](./MediaDemo/) | Video / audio playback via MediaElement | CommunityToolkit.Maui.MediaElement on Linux, GStreamer backend, play/pause/stop/seek/volume/mute |
| [MapsDemo](./MapsDemo/) | OpenStreetMap map view with pins, route, and shape overlays | Microsoft.Maui.Controls.Maps on Linux, OSM raster tile cache, Pin/Polyline/Polygon/Circle overlays, pan/zoom |
| [SplitPaneDemo](./SplitPaneDemo/) | Resizable two-pane layout with a draggable divider | Grid split-pane, PanGestureRecognizer, absolute + star column sizing, preset widths, theme switching |

## Requirements

**Software:**
- .NET 11 SDK (11.0.100 or later)
- Linux with X11 or Wayland
- Supported distributions: Ubuntu 20.04+, Fedora 35+, Debian 11+, Arch Linux, or similar

**System Dependencies:**
```bash
# Ubuntu/Debian
sudo apt-get install libfontconfig1-dev libfreetype6-dev

# Fedora/RHEL
sudo dnf install fontconfig-devel freetype-devel

# Arch Linux
sudo pacman -S --needed fontconfig freetype2 mesa harfbuzz

# For WebView and BlazorWebView (WebViewDemo, BlazorDemo): WPE WebKit 2.54+
sudo pacman -S --needed wpewebkit                                    # Arch (extra, 2.54.x)
sudo apt-get install libwpewebkit-2.0-1                        # Debian sid / Ubuntu
sudo dnf copr enable philn/wpewebkit && sudo dnf install wpewebkit   # Fedora (no official package)
# Without WPE, WebViewDemo falls back to WebKitGTK in GTK mode:
sudo pacman -S --needed gtk3 webkit2gtk-4.1   # Arch
sudo apt-get install libwebkit2gtk-4.1-0    # Ubuntu/Debian
sudo dnf install webkit2gtk4.1              # Fedora/RHEL

# For MediaElement support (MediaDemo)
# Arch Linux:
sudo pacman -S --needed gst-plugins-good gst-plugins-bad gst-plugins-ugly gst-libav
# Ubuntu/Debian:
sudo apt-get install gstreamer1.0-plugins-good gstreamer1.0-plugins-bad \
                     gstreamer1.0-plugins-ugly gstreamer1.0-libav gstreamer1.0-vaapi
# Fedora/RHEL:
sudo dnf install gstreamer1-plugins-good gstreamer1-plugins-bad-free \
                 gstreamer1-plugins-ugly-free gstreamer1-libav gstreamer1-vaapi
# (the vaapi/nvdec plugins add hardware-accelerated decode; software decode works
#  without them)

# For CUPS printing (ShellDemo)
sudo pacman -S --needed libcups              # Arch
sudo apt-get install libcups2-dev            # Ubuntu/Debian
```

## Installation

### Clone the Repository

```bash
git clone https://github.com/indoguy/maui-linux-samples-net11.git
cd maui-linux-samples-net11
```

### Install .NET SDK

If you don't have the .NET 11 SDK installed:

```bash
# Download and install the .NET 11 SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 11.0

# Add to PATH (add to ~/.bashrc for persistence)
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$DOTNET_ROOT:$PATH
```

> On Arch Linux the repositories currently ship .NET 10 only, so use the script
> above to get the .NET 11 SDK.

### Verify Installation

```bash
dotnet --version  # Should show 11.0.x or later
```

## Quick Start

### Run TodoApp

```bash
cd TodoApp
dotnet run
```

The todo application will launch with sample tasks. You can:
- Add new tasks with the "+" button
- Tap tasks to view/edit details
- Mark tasks as completed
- Delete tasks
- Toggle between light and dark themes

### Run ShellDemo

```bash
cd ../ShellDemo
dotnet run
```

The control gallery will open with a flyout menu. Navigate through different pages to explore:
- Button styles and events
- Text input controls (Entry, Editor, SearchBar)
- Selection controls (CheckBox, Switch, Slider)
- Pickers (Picker, DatePicker, TimePicker)
- CollectionView with dynamic data
- Progress indicators
- Grid layouts with various configurations

### Run WebViewDemo

```bash
cd ../WebViewDemo
dotnet run
```

A web browser window will open. You can:
- Navigate to any URL
- Use back/forward buttons
- Load custom HTML content
- Execute JavaScript
- Toggle themes

### Run MediaDemo

```bash
cd ../MediaDemo
dotnet run
```

A video player window will open with the Sintel trailer pre-loaded (Creative Commons, served by Blender Foundation). You can:
- Play / Pause / Stop the video
- Scrub via the position slider
- Adjust volume and mute
- Paste any media URL (mp4, mkv, mp3, http/https streams) or file path into the entry and hit **Load**

Note: requires GStreamer 1.x + the plugin packages listed under System Dependencies above. Hardware-accelerated decode kicks in automatically when `gstreamer1-vaapi` (Intel/AMD) or the nvdec plugin (NVIDIA) is installed.

### Run SplitPaneDemo

```bash
cd ../SplitPaneDemo
dotnet run
```

A two-pane mail layout opens with a draggable divider between the panes. You can:
- Drag the vertical divider left/right to resize the panes
- Jump to 25% / 50% / 75% / Reset with the header buttons
- Select a folder on the left to update the right pane
- Toggle between light and dark themes

## Building and Deployment

### Debug Build

```bash
# Build without running
dotnet build

# Output location: bin/Debug/net11.0/
```

### Release Build

```bash
# Build optimized release version
dotnet build -c Release
```

### Publish for Distribution

Create self-contained executables for specific Linux architectures:

```bash
# Linux x64 (most desktops)
dotnet publish -c Release -r linux-x64 --self-contained

# Linux ARM64 (Raspberry Pi, ARM servers)
dotnet publish -c Release -r linux-arm64 --self-contained

# Framework-dependent (requires .NET runtime installed)
dotnet publish -c Release -r linux-x64 --no-self-contained
```

Published applications will be in `bin/Release/net11.0/linux-x64/publish/` and can be distributed as standalone executables.

### Create Desktop Launcher

```bash
# Make the run script executable
chmod +x TodoApp/run.sh

# Create a .desktop file for your application
cat > ~/.local/share/applications/openmaui-todo.desktop << EOF
[Desktop Entry]
Type=Application
Name=OpenMaui Todo
Exec=/path/to/maui-linux-samples/TodoApp/run.sh
Icon=utilities-terminal
Terminal=false
Categories=Utility;
EOF
```

## Sample Details

Detailed write-ups for each sample - features, key files and code examples - live in
**[SAMPLES.md](./SAMPLES.md)**.

[TodoApp](./SAMPLES.md#todoapp)  ·  [ShellDemo](./SAMPLES.md#shelldemo)  ·  [WebViewDemo](./SAMPLES.md#webviewdemo)  ·  [BlazorDemo](./SAMPLES.md#blazordemo)  ·  [MediaDemo](./SAMPLES.md#mediademo)  ·  [MapsDemo](./SAMPLES.md#mapsdemo)  ·  [SplitPaneDemo](./SAMPLES.md#splitpanedemo)

## Project Structure

Each sample follows a similar structure:

```
SampleApp/
├── Program.cs              # Linux platform entry point
├── MauiProgram.cs          # MAUI app configuration
├── App.xaml                # Application resources and theme definitions
├── App.xaml.cs             # Application lifecycle
├── SampleApp.csproj        # Project configuration
├── Pages/                  # XAML pages and code-behind
│   ├── MainPage.xaml
│   └── MainPage.xaml.cs
├── Resources/              # Images, fonts, icons
│   ├── AppIcon/
│   └── Images/
└── run.sh                  # Launcher script
```

### Project Configuration

All samples use conditional compilation for cross-platform support:

```xml
<!-- SampleApp.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <!-- Linux: Use OpenMaui -->
  <PropertyGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
    <RuntimeIdentifiers>linux-x64;linux-arm64</RuntimeIdentifiers>
  </PropertyGroup>

  <!-- OpenMaui Linux platform (NuGet package) -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
    <PackageReference Include="OpenMaui.Controls.Linux" Version="11.0.10.2" />
  </ItemGroup>
</Project>
```

> The samples always use the OpenMaui **NuGet package**. To build and debug against
> the OpenMaui source code instead, see
> [Using the OpenMaui source](#using-the-openmaui-source).

## Using the OpenMaui source

The samples reference OpenMaui as a **NuGet package**, and that package ships no
`.pdb` or SourceLink data - so you cannot step into the framework from it. To build
and debug *into* OpenMaui's own code, attach the source instead.

**1. Clone the framework next to this repo**, into a folder named `maui-linux`:

```bash
cd ..                                        # parent of maui-linux-samples-net11
git clone https://github.com/open-maui/maui-linux.git
```

> The revision that produced the referenced package is recorded in its `.nuspec`.
> For `OpenMaui.Controls.Linux 11.0.10.2` that is commit `95ff85e`, so
> `git -C ../maui-linux checkout 95ff85e` gives a matching source tree.

**2. Swap the package for project references** in the sample's `.csproj`:

```diff
- <!-- OpenMaui Linux Platform - NuGet package -->
- <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
-   <PackageReference Include="OpenMaui.Controls.Linux" Version="11.0.10.2" />
- </ItemGroup>
+ <!-- OpenMaui Linux Platform - local source -->
+ <ItemGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
+   <ProjectReference Include="../../maui-linux/OpenMaui.Controls.Linux.csproj" />
+ </ItemGroup>
```

Samples that also use a companion package need its source project as well:

| Sample | Project references to add |
|--------|---------------------------|
| `ShellDemo`, `TodoApp`, `WebViewDemo`, `SplitPaneDemo` | `OpenMaui.Controls.Linux` |
| `BlazorDemo` | that plus `Blazor/OpenMaui.Controls.Linux.Blazor.csproj` |
| `MapsDemo` | that plus `Maps/OpenMaui.Controls.Linux.Maps.csproj` |
| `MediaDemo` | that plus `MediaElement/OpenMaui.Controls.Linux.MediaElement.csproj` |

**3. Import the source build targets** (the package's targets do not come along with
a project reference). Add this to the same `.csproj`:

```xml
<Import Project="../../maui-linux/build/OpenMaui.Controls.Linux.targets"
        Condition="$([MSBuild]::IsOSPlatform('Linux')) AND Exists('../../maui-linux/build/OpenMaui.Controls.Linux.targets')" />
```

**4. Build and debug.** `dotnet build` now compiles OpenMaui from source, and F5
steps into it (set `"justMyCode": false` if the debugger skips it). Optionally add the
framework projects to the solution:

```bash
dotnet sln add ../maui-linux/OpenMaui.Controls.Linux.csproj \
               ../maui-linux/Hosting.Abstractions/OpenMaui.Hosting.csproj \
               ../maui-linux/Blazor/OpenMaui.Controls.Linux.Blazor.csproj
```

These edits are local to your checkout - keep them out of commits to leave the
repository package-only.

## Development Guide

### Creating a New Linux MAUI App

1. **Create project structure:**

```bash
mkdir MyApp
cd MyApp
dotnet new console -n MyApp
```

2. **Edit MyApp.csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <EnableDefaultXamlItems>true</EnableDefaultXamlItems>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="OpenMaui.Controls.Linux" Version="11.0.10.2" />
  </ItemGroup>
</Project>
```

3. **Create Program.cs:**

```csharp
using Microsoft.Maui.Platform.Linux;

namespace MyApp;

class Program
{
    static void Main(string[] args)
    {
        var app = MauiProgram.CreateMauiApp();
        LinuxApplication.Run(app, args);
    }
}
```

4. **Create MauiProgram.cs:**

```csharp
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform.Linux.Hosting;

namespace MyApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.UseLinux();
        return builder.Build();
    }
}
```

5. **Create App.xaml and App.xaml.cs** (see samples for examples)

6. **Build and run:**

```bash
dotnet build
dotnet run
```

### Exception Handling

All samples include comprehensive exception handling:

```csharp
// Global exception handlers in Program.cs
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    var ex = e.ExceptionObject as Exception;
    Console.WriteLine($"[FATAL] Unhandled exception: {ex?.Message}");
    Console.WriteLine($"[FATAL] Stack trace: {ex?.StackTrace}");
};

TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Console.WriteLine($"[FATAL] Unobserved task exception: {e.Exception?.Message}");
    e.SetObserved(); // Prevent crash
};
```

### Logging

All samples redirect console output to log files in the user's home directory:

```csharp
var logPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
    "myapp.log");
using var logWriter = new StreamWriter(logPath, append: false) { AutoFlush = true };
var multiWriter = new MultiTextWriter(Console.Out, logWriter);
Console.SetOut(multiWriter);
Console.SetError(multiWriter);
```

### Debugging in VS Code

`.vscode/` is git-ignored, so create the launch configuration yourself. The simplest
way to avoid the apphost runtime problem entirely is to launch the managed `.dll`
rather than the native apphost: the `dotnet` host finds its own runtime, so it does
not need `DOTNET_ROOT`.

Without this, a debug session launches the native .NET **apphost**
(`bin/<Config>/net11.0/<Name>`), which finds the runtime only via `DOTNET_ROOT`,
`/etc/dotnet/install_location(_x64)`, or `/usr/share/dotnet` — **not** `PATH`. On a
per-user SDK under `~/.dotnet` that fails with:

```
You must install .NET to run this application. ... DOTNET_ROOT = <not set>
The program '... SplitPaneDemo' has exited with code 131 (0x83).
```

Create `.vscode/launch.json` (swap the sample path for other samples):

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

If you instead run the apphost as `program`, add an `env` block so it can find the SDK:

```jsonc
"env": {
  "DOTNET_ROOT": "${env:HOME}/.dotnet",
  "PATH": "${env:HOME}/.dotnet:${env:PATH}"
}
```

For a permanent, launcher-independent fix, register the install location:

```bash
# machine-wide (needs root) - the apphost reads this file directly
echo "$HOME/.dotnet" | sudo tee /etc/dotnet/install_location_x64

# or per-user, no root (applies to GUI apps at next login)
mkdir -p ~/.config/environment.d
printf 'DOTNET_ROOT=%s\n' "$HOME/.dotnet" > ~/.config/environment.d/10-dotnet.conf
```

See [Troubleshooting](#troubleshooting).

## API Usage Examples

### Navigation

**NavigationPage (TodoApp):**

```csharp
// App.xaml.cs - Create NavigationPage
protected override Window CreateWindow(IActivationState? activationState)
{
    NavigationPage = new NavigationPage(new TodoListPage())
    {
        BarBackgroundColor = Color.FromArgb("#5C6BC0"),
        BarTextColor = Colors.White
    };
    return new Window(NavigationPage);
}

// Push a page
await Navigation.PushAsync(new TodoDetailPage(todo));

// Pop back
await Navigation.PopAsync();
```

**Shell Navigation (ShellDemo):**

```csharp
// AppShell.xaml.cs - Register routes
Routing.RegisterRoute("detail", typeof(DetailPage));

// Navigate using route
await Shell.Current.GoToAsync("detail?item=MyItem");

// Navigate with parameters
await Shell.Current.GoToAsync($"detail?item={Uri.EscapeDataString(itemName)}");

// Use LinuxViewRenderer for direct navigation
LinuxViewRenderer.NavigateToRoute("Buttons");
LinuxViewRenderer.PushPage(new DetailPage());
LinuxViewRenderer.PopPage();
```

### Data Binding

**Observable Collections:**

```csharp
public class TodoService
{
    public ObservableCollection<TodoItem> Todos { get; } = new();
    
    public TodoItem AddTodo(string title, string notes = "")
    {
        var todo = new TodoItem { Title = title, Notes = notes };
        Todos.Add(todo);
        return todo;
    }
}
```

**INotifyPropertyChanged:**

```csharp
public class TodoItem : INotifyPropertyChanged
{
    private bool _isCompleted;
    
    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### Value Converters

**Theme-Aware Color Converter:**

```csharp
public class CompletedToColorConverter : IValueConverter
{
    private static readonly Color AccentColorLight = Color.FromArgb("#26A69A");
    private static readonly Color AccentColorDark = Color.FromArgb("#4DB6AC");
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isCompleted = value is bool b && b;
        bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        
        if (isCompleted)
            return Color.FromArgb("#9E9E9E");
        
        return isDarkMode ? AccentColorDark : AccentColorLight;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

**Register in App.xaml:**

```xaml
<Application.Resources>
    <local:CompletedToColorConverter x:Key="CompletedToColorConverter"/>
</Application.Resources>
```

### Theme Switching

**Toggle Theme:**

```csharp
private void OnThemeToggleClicked(object? sender, EventArgs e)
{
    if (Application.Current == null) return;
    
    var isDarkMode = Application.Current.UserAppTheme == AppTheme.Dark;
    Application.Current.UserAppTheme = isDarkMode ? AppTheme.Light : AppTheme.Dark;
}
```

**AppThemeBinding in XAML:**

```xaml
<Border BackgroundColor="{AppThemeBinding Light=#FFFFFF, Dark=#1E1E1E}">
    <Label TextColor="{AppThemeBinding Light=#212121, Dark=#FFFFFF}" 
           Text="Theme-aware text"/>
</Border>
```

**Programmatic AppThemeBinding:**

```csharp
label.SetAppThemeColor(Label.TextColorProperty,
    Colors.Black,  // Light theme
    Colors.White); // Dark theme
```

### Dialogs

**DisplayAlert (using LinuxDialogService):**

```csharp
private async void OnDeleteClicked(object? sender, EventArgs e)
{
    var confirmed = await LinuxDialogService.ShowAlertAsync(
        "Delete Task",
        $"Are you sure you want to delete \"{_todo.Title}\"?",
        "Delete",
        "Cancel");
    
    if (confirmed)
    {
        _service.DeleteTodo(_todo);
        await Navigation.PopAsync();
    }
}
```

### WebView

**Load URL:**

```csharp
MainWebView.Source = new UrlWebViewSource { Url = "https://dotnet.microsoft.com" };
```

**Load HTML:**

```csharp
MainWebView.Source = new HtmlWebViewSource 
{ 
    Html = "<html><body><h1>Hello!</h1></body></html>" 
};
```

**Execute JavaScript:**

```csharp
var result = await MainWebView.EvaluateJavaScriptAsync("document.title");
```

**Navigation Events:**

```csharp
private void OnNavigating(object? sender, WebNavigatingEventArgs e)
{
    Console.WriteLine($"Navigating to: {e.Url}");
    // e.Cancel = true; // Cancel navigation if needed
}

private void OnNavigated(object? sender, WebNavigatedEventArgs e)
{
    Console.WriteLine($"Navigated: {e.Result} - {e.Url}");
}
```

## Troubleshooting

**The app exits with "You must install .NET to run this application" (exit code 131).**
The app runs through the generated .NET **apphost**, which finds the runtime only via
`DOTNET_ROOT`, `/etc/dotnet/install_location(_x64)`, or `/usr/share/dotnet` - never
`PATH`. With a per-user SDK under `~/.dotnet`, register the location once (needs root):

```bash
echo "$HOME/.dotnet" | sudo tee /etc/dotnet/install_location_x64
```

Running `dotnet run`, or the `.dll` through the `dotnet` host, works without it. See
[Debugging in VS Code](#debugging-in-vs-code) for the launch-profile details.

**BlazorDemo / WebViewDemo show a blank view.** They need **WPE WebKit 2.54+**
(`libWPEWebKit-2.0`): `sudo pacman -S wpewebkit` (Arch) or
`sudo apt-get install libwpewebkit-2.0-1` (Debian sid / Ubuntu). Without it, BlazorDemo
skips the web view (its status bar stays at "Blazor: starting"), and WebViewDemo falls
back to GTK mode via `OPENMAUI_WEBVIEW=webkitgtk` with `gtk3` + `webkit2gtk-4.1`.

**MediaDemo has no picture or sound.** Install the GStreamer plugin sets from
[Requirements](#requirements).

**MapsDemo is blank.** It fetches OpenStreetMap tiles from `tile.openstreetmap.org`, so
the first view of each tile needs internet access.

**NuGet restore fails.** `NuGet.config` maps `OpenMaui.*` packages to the
`git.marketally.ai` feed; check the URL is reachable and matches `NuGet.config`.

## Related Resources

- **[OpenMaui Linux Framework](https://github.com/open-maui/maui-linux)** - The core framework repository
- **[NuGet Package](https://www.nuget.org/packages/OpenMaui.Controls.Linux)** - Install via NuGet
- **[.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)** - Official Microsoft documentation
- **[SkiaSharp Documentation](https://learn.microsoft.com/xamarin/xamarin-forms/user-interface/graphics/skiasharp/)** - Graphics rendering engine

## Support

If you find this .NET 11 port useful, you can buy me a coffee:

<a href="https://buymeacoffee.com/indoguy" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height="40"></a>

## License

MIT License - See [LICENSE](LICENSE) for details.

Copyright (c) 2024-2026 OpenMaui

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
