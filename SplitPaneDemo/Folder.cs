using Microsoft.Maui.Graphics;

namespace SplitPaneDemo;

/// <summary>
/// A selectable entry in the left ("navigation") pane. Kept deliberately small:
/// this sample is about the draggable divider, not about the data.
/// </summary>
public class Folder
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Count { get; set; }
    public int Unread { get; set; }

    /// <summary>Accent used for the item's leading colour bar.</summary>
    public Color Accent { get; set; } = Colors.Gray;
}
