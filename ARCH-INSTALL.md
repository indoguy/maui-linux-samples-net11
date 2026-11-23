# Installing .NET 11 and OpenMaui on Arch Linux

A step-by-step guide for getting a .NET 11 + OpenMaui development environment
running on Arch Linux (and Arch-based distros such as EndeavourOS, CachyOS,
Manjaro).

This is the environment the samples in this repository are built and tested
against. For what changed to move the samples to .NET 11, see
[`NET11-MIGRATION.md`](NET11-MIGRATION.md).

- **Tested with:** Arch Linux, `.NET SDK 11.0.100`, runtime `11.0.0`,
  `OpenMaui 11.0.10.2`, Wayland session (COSMIC/KDE) and X11.
- **Key point:** OpenMaui apps on Linux build as a plain `net11.0` target and
  do **not** use the official MAUI workload, so you do **not** need
  `dotnet workload install maui`.

---

## 0. Quick start (TL;DR)

```bash
# 1. System libraries
sudo pacman -S --needed mesa fontconfig freetype2 harfbuzz \
    libx11 libxrandr libxcursor libxi libxext libxrender wayland libxkbcommon \
    gtk3 wpewebkit webkit2gtk-4.1 \
    gstreamer gst-plugins-base gst-plugins-good gst-plugins-bad gst-plugins-ugly gst-libav gst-plugin-va \
    libcups

# 2. .NET 11 SDK
curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh
/tmp/dotnet-install.sh --channel 11.0 --install-dir "$HOME/.dotnet"

# 3. Put it on PATH (see section 2 for the non-interactive-shell gotcha)
echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc
echo 'export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"' >> ~/.bashrc
exec bash -l

# 4. Verify
dotnet --list-sdks        # 11.0.100
dotnet --list-runtimes    # Microsoft.NETCore.App 11.0.0
```

Then jump to section 3 to add OpenMaui to a project, or section 5 to run the
samples in this repo.

---

## 1. System dependencies

Arch does not split `-dev`/`-devel` packages as Debian/Fedora do — headers ship
inside the main package — so the list is shorter than the apt/dnf equivalents in
OpenMaui's own README.

### Base (required for any OpenMaui app)

| Purpose | Packages |
|---------|----------|
| EGL / OpenGL (Skia rendering) | `mesa` |
| Text shaping and fonts | `fontconfig` `freetype2` `harfbuzz` |
| X11 support | `libx11` `libxrandr` `libxcursor` `libxi` `libxext` `libxrender` |
| Wayland support (usually already present) | `wayland` `libxkbcommon` |

```bash
sudo pacman -S --needed mesa fontconfig freetype2 harfbuzz \
    libx11 libxrandr libxcursor libxi libxext libxrender wayland libxkbcommon
```

> If you are on a Wayland session, most of these are already installed as
> dependencies of your desktop. `--needed` makes re-running the command a no-op.

### Optional features

Install only the ones you need:

```bash
# WebView / BlazorWebView
sudo pacman -S --needed wpewebkit            # composited WebView (WPE WebKit 2.54+, recommended)
sudo pacman -S --needed gtk3 webkit2gtk-4.1  # GTK-hosted WebKitGTK fallback (OPENMAUI_WEBVIEW=webkitgtk)

# MediaElement (video / audio via GStreamer)
sudo pacman -S --needed gstreamer gst-plugins-base gst-plugins-good \
    gst-plugins-bad gst-plugins-ugly gst-libav gst-plugin-va

# CUPS printing
sudo pacman -S --needed libcups
```

Notes:

- `gst-plugin-va` enables hardware-accelerated decode on Intel/AMD GPUs;
  NVIDIA's `nvcodec` element ships in `gst-plugins-bad`. Software decode works
  with just the GStreamer packages.
- `wpewebkit` in `extra` is 2.54.x and provides `libWPEWebKit-2.0.so.1`, which
  is exactly what OpenMaui looks for.
- Maps (OpenStreetMap tiles) needs no system packages — it uses Skia and the
  network.

---

## 2. Install the .NET 11 SDK

### Important: Arch's `dotnet-sdk` is .NET 10

As of this writing the Arch `extra` repo ships only .NET 10:

```text
extra/dotnet-sdk      10.0.14.sdk114-2
extra/dotnet-runtime  10.0.14.sdk114-2
```

Installing `extra/dotnet-sdk` will **not** give you .NET 11. Use the Microsoft
install script (option A, what the samples use) or the AUR (option B).

### Option A — official `dotnet-install.sh` (recommended)

```bash
curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh

# Latest .NET 11 SDK (currently 11.0.100):
/tmp/dotnet-install.sh --channel 11.0 --install-dir "$HOME/.dotnet"

# ...or pin an exact version:
# /tmp/dotnet-install.sh --version 11.0.100 --install-dir "$HOME/.dotnet"
```

Add it to your shell:

```bash
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"
```

### Option B — AUR

With an AUR helper (`yay`/`paru`):

```bash
yay -S dotnet-sdk-bin      # .NET 11 SDK (11.0.0.sdk100)
```

AUR packages install into `/usr/share/dotnet` (or a package-specific prefix).
After installing, confirm the layout and point `DOTNET_ROOT` at the real root:

```bash
command -v dotnet          # what resolved
dotnet --info | grep -i "base path\|DOTNET_ROOT"
```

### The `~/.bashrc` gotcha (non-interactive shells)

A very common mistake is to put the `export PATH=...dotnet...` lines *after* the
default Arch `.bashrc` guard:

```bash
# ~/.bashrc
[[ $- != *i* ]] && return     # <-- returns for non-interactive shells
...
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"
```

With the exports *after* that `return`, only **interactive** shells see `dotnet`.
Tools, scripts, editors, and `make`-style builds that run the shell
non-interactively get `dotnet: command not found`. Fix it by putting the exports
**before** the guard, or by adding them to `~/.profile` / a
`/etc/profile.d/dotnet.sh`.

### Verify

```bash
dotnet --list-sdks        # expect: 11.0.100 [~/.dotnet/sdk]
dotnet --list-runtimes    # expect: Microsoft.NETCore.App 11.0.0 [~/.dotnet/shared/...]
dotnet --version          # 11.0.100
```

---

## 3. Install OpenMaui

OpenMaui is **not** a system package — it is distributed as NuGet packages. You
either scaffold a new app from its templates or add the package to an existing
project.

### Option A — templates (new app)

```bash
dotnet new install OpenMaui.Linux.Templates

dotnet new openmaui-linux-xaml -n MyApp   # XAML UI (recommended)
# or: dotnet new openmaui-linux -n MyApp  # code-based UI

cd MyApp
dotnet run
```

### Option B — add to an existing project

```bash
dotnet add package OpenMaui.Controls.Linux --version 11.0.10.2
```

Then, in `MauiProgram.cs`:

```csharp
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform.Linux.Hosting;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseLinux();      // or .UseX11() / .UseWayland() to force a backend

var app = builder.Build();
Microsoft.Maui.Platform.Linux.LinuxApplication.Run(app, args);
```

### Optional packages (match MAUI 11)

| Feature | Packages |
|---------|----------|
| Blazor Hybrid | `OpenMaui.Controls.Linux.Blazor` + `Microsoft.AspNetCore.Components.WebView.Maui` `11.0.10` |
| Maps (OpenStreetMap) | `OpenMaui.Controls.Linux.Maps` + `Microsoft.Maui.Controls.Maps` `11.0.10` |
| MediaElement | `OpenMaui.Controls.Linux.MediaElement` + `CommunityToolkit.Maui.MediaElement` `12.0.0` |
| Syncfusion | `OpenMaui.Controls.Linux.Syncfusion` (bring your own license) |
| PDF (PDFium) | `OpenMaui.Controls.Linux.Pdf` |

Keep every `OpenMaui.*` package on the **same** version (`11.0.10.2`) and the
MAUI packages on `11.0.10`.

---

## 4. Verify the environment (OpenMaui doctor)

Any OpenMaui app can print an environment report instead of starting:

```bash
OPENMAUI_DOCTOR=1 ./bin/Debug/net11.0/MyApp
# or
dotnet run -- --openmaui-doctor
```

It reports the display server/compositor, EGL/GL renderer, scale factor, input
method, and each optional native dependency (GStreamer, WPE WebKit, libcups,
fallback fonts), with install hints for **pacman**. Exit code 1 means something
required is missing.

---

## 5. Build and run the samples in this repo

```bash
git clone https://github.com/indoguy/maui-linux-samples-net11.git
cd maui-linux-samples-net11

cd ShellDemo          # or TodoApp, MapsDemo, MediaDemo, BlazorDemo, WebViewDemo
dotnet build          # output: bin/Debug/net11.0/
./run.sh              # launches the net11.0 binary
```

`run.sh` auto-detects `DOTNET_ROOT` and the `net*.0` output folder, so it works
with the .NET 11 build without changes. Each sample restores
`OpenMaui.Controls.Linux 11.0.10.2` from NuGet.

---

## 6. Troubleshooting

| Symptom | Cause / fix |
|---------|-------------|
| `dotnet: command not found` in a script, but fine in your terminal | Exports are after the `.bashrc` interactive guard — see section 2 |
| Build fails with `NU1301 ... index.json ... 404` | Dead NuGet feed; the `OpenMaui.*` source must be `.../packages/open-maui/nuget/...` (fixed here — see `NET11-MIGRATION.md`) |
| `error MSB4019: The imported project ".../maui-linux/build/..." was not found` | You have no sibling `maui-linux` source checkout; `Directory.Build.targets` in this repo already guards this |
| BlazorDemo logs `BlazorWebView on Linux requires WPE WebKit 2.54+` | Install `wpewebkit` (section 1); the app still runs without it, just without the web view |
| WebView shows nothing / uses GTK | WPE missing → install `wpewebkit`, or use the GTK fallback: `OPENMAUI_WEBVIEW=webkitgtk` with `options.UseGtk = true` and `gtk3`+`webkit2gtk-4.1` installed |
| Printing menu reports unavailable | `sudo pacman -S libcups` |
| Video/audio won't play | Install the GStreamer plugin sets (section 1); for HW decode add `gst-plugin-va` (Intel/AMD) or `gst-plugins-bad` (NVIDIA) |
| Wrong window size / blurry on HiDPI | OpenMaui auto-detects scale; force a backend with `UseX11()`/`UseWayland()` if detection picks the wrong one |

Run `OPENMAUI_DOCTOR=1 ./MyApp` first — it almost always names the missing
piece and the exact `pacman` command.
