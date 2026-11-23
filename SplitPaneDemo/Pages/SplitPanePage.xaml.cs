using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace SplitPaneDemo.Pages;

public partial class SplitPanePage : ContentPage
{
    // The divider is clamped so neither pane can be squeezed out of existence.
    private const double MinLeftWidth = 200;
    private const double MinRightWidth = 280;
    private const double DefaultLeftWidth = 300;

    // Width of the middle column (the draggable divider itself). Keep in sync with
    // the ColumnDefinitions in SplitPanePage.xaml.
    private const double DividerWidth = 18;

    // PanGestureRecognizer reports the *total* translation since the press, so we
    // remember the pane width when the gesture starts and add the delta to it.
    private double _leftWidthAtGestureStart;

    // The gesture only begins after a small dead-zone (GestureManager.PanMinDistance,
    // 10px by default). We drop that initial offset so the divider does not jump on
    // the first Running event.
    private double _panBaseline;
    private bool _panBaselineSet;

    public ObservableCollection<Folder> Folders { get; } = new();

    public SplitPanePage()
    {
        InitializeComponent();

        SeedFolders();
        FolderList.ItemsSource = Folders;
        FolderList.SelectedItem = Folders[0];

        // Keep the "N%" readout honest when the window itself is resized.
        SplitGrid.SizeChanged += (_, _) => UpdateStatus();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyDividerCursor();
        UpdateThemeIcon();
        UpdateStatus();
    }

    private void SeedFolders()
    {
        Folders.Add(new Folder { Name = "Inbox",   Description = "Everything delivered to you", Count = 128, Unread = 12, Accent = Color.FromArgb("#2F6FEB") });
        Folders.Add(new Folder { Name = "Starred", Description = "Messages you flagged",        Count = 24,  Unread = 3,  Accent = Color.FromArgb("#F4B400") });
        Folders.Add(new Folder { Name = "Sent",    Description = "Messages you sent",           Count = 87,  Unread = 0,  Accent = Color.FromArgb("#26A69A") });
        Folders.Add(new Folder { Name = "Drafts",  Description = "Unfinished messages",         Count = 6,   Unread = 6,  Accent = Color.FromArgb("#AB47BC") });
        Folders.Add(new Folder { Name = "Archive", Description = "Filed away for later",       Count = 512, Unread = 0,  Accent = Color.FromArgb("#78909C") });
        Folders.Add(new Folder { Name = "Spam",    Description = "Suspicious messages",         Count = 41,  Unread = 41, Accent = Color.FromArgb("#EF5350") });
        Folders.Add(new Folder { Name = "Trash",   Description = "Deleted, recoverable",        Count = 19,  Unread = 0,  Accent = Color.FromArgb("#8D6E63") });
    }

    // ------------------------------------------------------------------
    //  Divider drag
    // ------------------------------------------------------------------
    private void OnDividerPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _leftWidthAtGestureStart = CurrentLeftWidth;
                _panBaselineSet = false;
                Console.WriteLine($"[SplitPane] Drag started at left width {_leftWidthAtGestureStart:F0}px");
                break;

            case GestureStatus.Running:
                if (!_panBaselineSet)
                {
                    _panBaseline = e.TotalX;
                    _panBaselineSet = true;
                }

                SetLeftWidth(_leftWidthAtGestureStart + (e.TotalX - _panBaseline));
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                Console.WriteLine($"[SplitPane] Drag {e.StatusType} at left width {CurrentLeftWidth:F0}px (moved {CurrentLeftWidth - _leftWidthAtGestureStart:F0}px)");
                break;
        }
    }

    private double CurrentLeftWidth => SplitGrid.ColumnDefinitions[0].Width.Value;

    private void SetLeftWidth(double width)
    {
        var total = SplitGrid.Width;
        var maxLeft = total > 0
            ? Math.Max(MinLeftWidth, total - DividerWidth - MinRightWidth)
            : double.PositiveInfinity;

        var clamped = Math.Clamp(width, MinLeftWidth, maxLeft);
        SplitGrid.ColumnDefinitions[0].Width = new GridLength(clamped);
        UpdateStatus();
    }

    private void ApplyPreset(double fraction)
    {
        var total = SplitGrid.Width;
        if (total <= 0)
            return;

        SetLeftWidth((total - DividerWidth) * fraction);
    }

    private void OnPreset25Clicked(object? sender, EventArgs e) => ApplyPreset(0.25);
    private void OnPreset50Clicked(object? sender, EventArgs e) => ApplyPreset(0.50);
    private void OnPreset75Clicked(object? sender, EventArgs e) => ApplyPreset(0.75);
    private void OnResetClicked(object? sender, EventArgs e) => SetLeftWidth(DefaultLeftWidth);

    // ------------------------------------------------------------------
    //  Left pane selection -> right pane content
    // ------------------------------------------------------------------
    private void OnFolderSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Folder folder)
            return;

        DetailTitle.Text = folder.Name;
        DetailSubtitle.Text = folder.Description;
        StatTotal.Text = folder.Count.ToString();
        StatUnread.Text = folder.Unread.ToString();
        StatFlagged.Text = Math.Max(1, folder.Count / 20).ToString();
        StatAttachments.Text = (folder.Count / 7).ToString();

        DetailBody.Text =
            $"{folder.Name} holds {folder.Count} messages, {folder.Unread} of them unread. " +
            "Drag the divider to watch this pane reflow: the text re-wraps, the stat cards " +
            "resize, and the folder list on the left keeps its selection.";

        Console.WriteLine($"[SplitPane] Selected folder: {folder.Name}");
    }

    // ------------------------------------------------------------------
    //  Status bar / theme / cursor
    // ------------------------------------------------------------------
    private void UpdateStatus()
    {
        var left = CurrentLeftWidth;
        var total = SplitGrid.Width;

        if (total > 0 && total > DividerWidth)
        {
            var usable = total - DividerWidth;
            var right = Math.Max(0, total - left - DividerWidth);
            var percent = left / usable * 100;
            StatusLabel.Text =
                $"Left pane {left:F0}px · {percent:F0}%    Right pane {right:F0}px · {100 - percent:F0}%    —  drag the divider to resize";
        }
        else
        {
            StatusLabel.Text = $"Left pane {left:F0}px — drag the divider to resize";
        }
    }

    private void ApplyDividerCursor()
    {
#if OPENMAUI_LINUX
        // Show the horizontal-resize cursor while the pointer is over the divider.
        if (Divider.Handler?.PlatformView is Microsoft.Maui.Platform.SkiaView dividerView)
            dividerView.CursorType = Microsoft.Maui.Platform.Linux.Window.CursorType.SizeWestEast;
#endif
    }

    private void OnThemeToggleClicked(object? sender, EventArgs e)
    {
        var app = Application.Current;
        if (app is null)
            return;

        var isDark = IsDark();
        app.UserAppTheme = isDark ? AppTheme.Light : AppTheme.Dark;
        UpdateThemeIcon();
    }

    private bool IsDark() =>
        Application.Current?.UserAppTheme == AppTheme.Dark ||
        (Application.Current?.UserAppTheme == AppTheme.Unspecified &&
         Application.Current?.RequestedTheme == AppTheme.Dark);

    private void UpdateThemeIcon()
    {
        // Show the icon for the theme you would switch *to*.
        ThemeToggleButton.Source = IsDark() ? "light_mode_white.svg" : "dark_mode_white.svg";
    }
}
