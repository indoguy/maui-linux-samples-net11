using Microsoft.Maui.Controls;

namespace SplitPaneDemo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new Pages.SplitPanePage())
        {
            Title = "OpenMaui Split Pane Demo",
            Width = 1100,
            Height = 720,
        };
    }
}
