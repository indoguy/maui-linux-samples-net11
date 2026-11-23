# .NET 11 Migration Notes

This document records exactly what was changed to retarget every sample in this
repository from **.NET 10** to **.NET 11**, and why. It is a companion to the
top-level [`README.md`](README.md) and [`GUIDE.md`](GUIDE.md).

- **Before:** `net10.0` + `OpenMaui 10.0.x`
- **After:** `net11.0` + `OpenMaui 11.0.10.2` (built on MAUI 11.0.10)

## Prerequisites

- **.NET SDK 11.0.100** (or newer). Verify with:
  ```bash
  dotnet --list-sdks     # expect 11.0.100
  dotnet --list-runtimes # expect Microsoft.NETCore.App 11.0.0
  ```
- A desktop session (X11 or Wayland). Each sample has a `run.sh` that
  auto-detects the runtime root and the `net11.0` output folder.

## Summary of changes

| Area | Change |
|------|--------|
| `NuGet.config` | Fixed a dead package feed URL (`AuroraOSS` → `open-maui`), which was returning **404** and blocking restore |
| `Directory.Build.targets` | The local-dev `maui-linux` import is now existence-guarded so package-reference builds work without a sibling source checkout |
| Every `*.csproj` | `net10.0` → `net11.0`; OpenMaui packages pinned to `11.0.10.2`; ecosystem packages bumped to their .NET 11 builds; app version metadata `10.0.x` → `11.0.x` |
| `ShellDemo` sources | Adapted to two v11 API renames (`LinuxDragEventArgs`/`LinuxDropEventArgs`, `Browser.Default`) |
| `.gitignore` | Added (none existed), so `bin/`/`obj/` stay out of the repo |

All six samples build with **0 errors** and launch:

| Sample | Linux TFM | Extra packages |
|--------|-----------|----------------|
| `ShellDemo` | `net11.0` | OpenMaui.Controls.Linux |
| `TodoApp` | `net11.0` | OpenMaui.Controls.Linux |
| `WebViewDemo` | `net11.0` | OpenMaui.Controls.Linux |
| `MapsDemo` | `net11.0` | Microsoft.Maui.Controls.Maps, OpenMaui…Maps |
| `MediaDemo` | `net11.0` | CommunityToolkit.Maui.MediaElement, OpenMaui…MediaElement |
| `BlazorDemo` | `net11.0` | Microsoft.AspNetCore.Components.WebView.Maui, OpenMaui…Blazor |

---

## 1. `NuGet.config` — dead feed

Restore failed before anything else could build because the `OpenMaui.*`
package source returned **404 Not Found**. The Gitea organization was renamed,
so the feed path changed:

```diff
- <add key="gitea" value="https://git.marketally.ai/api/packages/AuroraOSS/nuget/index.json" />
+ <add key="gitea" value="https://git.marketally.ai/api/packages/open-maui/nuget/index.json" />
```

By default each sample uses a `PackageReference` to `OpenMaui.Controls.Linux`
(via `packageSourceMapping`, `OpenMaui.*` resolves from this feed). The same
packages are also published on nuget.org if you prefer to point there instead.

## 2. `Directory.Build.targets` — guarded local-dev import

The repo auto-imports OpenMaui MSBuild targets from a **sibling** `maui-linux`
source checkout when present. That import was unconditional, so it failed with
`MSB4019` whenever the sibling directory did not exist (i.e. the normal
package-reference workflow). It is now guarded:

```diff
- <Import Project="../maui-linux/build/OpenMaui.Controls.Linux.targets" Condition="$([MSBuild]::IsOSPlatform('Linux'))" />
+ <PropertyGroup>
+   <_OpenMauiLocalTargets>$(MSBuildThisFileDirectory)../maui-linux/build/OpenMaui.Controls.Linux.targets</_OpenMauiLocalTargets>
+ </PropertyGroup>
+ <Import Project="$(_OpenMauiLocalTargets)" Condition="$([MSBuild]::IsOSPlatform('Linux')) AND Exists('$(_OpenMauiLocalTargets)')" />
```

When a `../maui-linux` checkout *is* present, the local build still works
exactly as before.

## 3. Project files (`*.csproj`)

Applied identically to all six samples.

### 3.1 Target frameworks

```diff
- <TargetFramework>net10.0</TargetFramework>
+ <TargetFramework>net11.0</TargetFramework>

  <!-- non-Linux (only built on a Mac/Windows host) -->
- <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0</TargetFrameworks>
+ <TargetFrameworks>net11.0-android;net11.0-ios;net11.0-maccatalyst;net11.0-windows10.0.19041.0</TargetFrameworks>
```

### 3.2 Package versions

OpenMaui packages were pinned to the exact `net11.0` build instead of the
floating `*`:

```diff
- <PackageReference Include="OpenMaui.Controls.Linux" Version="*" />
+ <PackageReference Include="OpenMaui.Controls.Linux" Version="11.0.10.2" />
```

The optional packages used by individual samples:

| Package | Before | After |
|---------|--------|-------|
| `OpenMaui.Controls.Linux` | `*` | `11.0.10.2` |
| `OpenMaui.Controls.Linux.Blazor` | `*` | `11.0.10.2` |
| `OpenMaui.Controls.Linux.Maps` | `*` | `11.0.10.2` |
| `OpenMaui.Controls.Linux.MediaElement` | `*` | `11.0.10.2` |
| `Microsoft.AspNetCore.Components.WebView.Maui` | `10.0.110` | `11.0.10` |
| `Microsoft.Maui.Controls.Maps` | `10.0.*` | `11.0.10` |
| `CommunityToolkit.Maui.MediaElement` | `10.0.0` | `12.0.0` |

> `OpenMaui 11.0.x` targets `net11.0` and MAUI 11 (11.0.10). Apps staying on
> .NET 10 use `OpenMaui 10.0.x` / MAUI 10.

### 3.3 App version metadata

```diff
- <ApplicationVersion>10.0.0</ApplicationVersion>
- <Version>10.0.110</Version>
+ <ApplicationVersion>11.0.0</ApplicationVersion>
+ <Version>11.0.10</Version>
```

## 4. `ShellDemo` source changes

These are the only code changes required; the other five samples compiled
unchanged once retargeted (their `UseLinux` / `UseLinuxMaps` /
`UseLinuxMediaElement` / `UseLinuxBlazorWebView` calls are unchanged in v11).

### 4.1 Drag/drop event args renamed

`ShellDemo/Pages/DragDropPage.xaml.cs` — in the v11 assembly the drag/drop event
argument types gained a `Linux` prefix (`DragEventArgs` → `LinuxDragEventArgs`,
`DropEventArgs` → `LinuxDropEventArgs`). Their members (`Data`, `X`, `Y`,
`DroppedData`) are unchanged.

```diff
- using LinuxDragEventArgs = Microsoft.Maui.Platform.Linux.Services.DragEventArgs;
- using LinuxDropEventArgs = Microsoft.Maui.Platform.Linux.Services.DropEventArgs;
+ using LinuxDragEventArgs = Microsoft.Maui.Platform.Linux.Services.LinuxDragEventArgs;
+ using LinuxDropEventArgs = Microsoft.Maui.Platform.Linux.Services.LinuxDropEventArgs;
```

### 4.2 Browser helper replaced

`ShellDemo/Pages/AboutPage.xaml.cs` — the static OpenMaui
`…Platform.Linux.Services.Browser` helper no longer exists. Use the
cross-platform MAUI API, which is backed by OpenMaui's `BrowserService`:

```diff
- await Microsoft.Maui.Platform.Linux.Services.Browser.OpenAsync("https://www.openmaui.net", BrowserLaunchMode.SystemPreferred);
+ await Microsoft.Maui.ApplicationModel.Browser.Default.OpenAsync("https://www.openmaui.net", BrowserLaunchMode.SystemPreferred);
```

## 5. `.gitignore`

No `.gitignore` existed, so build output was untracked noise. A standard .NET
ignore file was added (`bin/`, `obj/`, `*.log`, IDE/OS cruft).

---

## How to build and run

Each sample builds and runs the same way:

```bash
cd ShellDemo          # or TodoApp, MapsDemo, MediaDemo, BlazorDemo, WebViewDemo
dotnet build          # produces bin/Debug/net11.0/
./run.sh              # launches the compiled net11.0 binary
```

`run.sh` auto-detects `DOTNET_ROOT` and picks the highest `net*.0` folder, so it
works with the new `net11.0` output without modification.

## Verification

```text
dotnet build   →  Build succeeded (0 errors)   [all six samples]
./run.sh       →  launches and stays running
runtimeconfig  →  "tfm": "net11.0", Microsoft.NETCore.App 11.0.0
```

## Known environment requirements (not migration issues)

- **`BlazorDemo` / `WebViewDemo` WebView:** the WPE-backed WebView requires
  **WPE WebKit 2.54+** (`libWPEWebKit-2.0`). If it is missing, `BlazorDemo`
  still runs but skips the `BlazorWebView` control and logs a clear message.
  - Debian testing/sid: `apt install libwpewebkit-2.0-1`
  - Fedora: `dnf copr enable philn/wpewebkit && dnf install wpewebkit`
- **`maui-linux-samples.sln`** references a sibling `../maui-linux` source
  checkout and is therefore only usable in the local-development workflow.
  In package-reference mode, build the individual sample projects.
