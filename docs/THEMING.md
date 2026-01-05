# EVEMon Theming System

This document describes the theming and UI standards introduced in EVEMon, starting with the About window as a reference implementation.

## Color Palette

### Dark Theme (EVE-inspired)
```csharp
// Backgrounds
DarkBackground = Color.FromArgb(23, 26, 33);    // Main window background
DarkCard = Color.FromArgb(35, 39, 49);          // Card/panel backgrounds
DarkListBg = Color.FromArgb(28, 31, 38);        // List/scrollable area backgrounds
DarkBorder = Color.FromArgb(50, 55, 65);        // Subtle borders/dividers

// Text
DarkText = Color.FromArgb(230, 230, 230);       // Primary text (white-ish)
DarkSubtext = Color.FromArgb(160, 160, 170);    // Secondary/muted text
DarkAccent = Color.FromArgb(232, 181, 79);      // EVE gold - headers, highlights
DarkLink = Color.FromArgb(100, 180, 255);       // Clickable links
```

### Light Theme
```csharp
// Backgrounds
LightBackground = Color.FromArgb(245, 245, 248);  // Main window background
LightCard = Color.White;                           // Card/panel backgrounds
LightListBg = Color.FromArgb(250, 250, 252);      // List/scrollable area backgrounds
LightBorder = Color.FromArgb(220, 220, 225);      // Subtle borders/dividers

// Text
LightText = Color.FromArgb(30, 30, 35);           // Primary text (near black)
LightSubtext = Color.FromArgb(100, 100, 110);     // Secondary/muted text
LightAccent = Color.FromArgb(180, 130, 50);       // Gold accent - headers, highlights
LightLink = Color.FromArgb(0, 100, 180);          // Clickable links
```

## Layout Architecture

### Three-Column Layout
The About window uses a symmetric 3-column layout:

```csharp
var mainTable = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 3,
    RowCount = 1,
    Padding = new Padding(15),
    BackColor = _bgColor,
    CellBorderStyle = TableLayoutPanelCellBorderStyle.None
};
mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
```

### Full-Height Cards
Each column contains a single Panel that docks to fill, providing visual symmetry:

```csharp
var panel = new Panel
{
    Dock = DockStyle.Fill,
    BackColor = _cardColor,
    Margin = new Padding(4)
};

panel.Paint += (s, e) =>
{
    using (var pen = new Pen(_borderColor, 1))
    {
        e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
    }
};
```

## UI Components

### Typography

| Element | Font | Size | Color |
|---------|------|------|-------|
| App Title | Segoe UI Bold | 18pt | `_accentColor` (gold) |
| Section Header | Segoe UI Bold | 8pt | `_accentColor` (gold) |
| Name/Subtitle | Segoe UI Bold | 12pt | `_textColor` |
| Body Text | Segoe UI | 8-9pt | `_subtextColor` |
| List Item | Segoe UI | 8pt | `_textColor` |
| Link | Segoe UI | 9pt | `_linkColor` |

### Section Headers
All caps, bold, gold accent color:
```csharp
new Label
{
    Text = "SECTION TITLE",
    Font = new Font("Segoe UI", 8, FontStyle.Bold),
    ForeColor = _accentColor,
    BackColor = Color.Transparent
}
```

### Section Dividers
Use thin horizontal lines between sections (not after the last section):

```csharp
private void AddDivider(Panel parent, int y)
{
    var divider = new Panel
    {
        Location = new Point(15, y),
        Size = new Size(parent.Width > 0 ? parent.Width - 30 : 240, 1),
        BackColor = _borderColor,
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
    };
    parent.Controls.Add(divider);
}
```

### Buttons
Flat style with border matching theme:
```csharp
var button = new Button
{
    Text = "Button Text",
    Font = new Font("Segoe UI", 9),
    ForeColor = _textColor,
    BackColor = _borderColor,
    FlatStyle = FlatStyle.Flat,
    Size = new Size(120, 28),
    Cursor = Cursors.Hand
};
button.FlatAppearance.BorderColor = _borderColor;
button.FlatAppearance.MouseOverBackColor = _accentColor;
```

### Links
Hover underline behavior:
```csharp
new LinkLabel
{
    Text = "link text",
    Font = new Font("Segoe UI", 9),
    LinkColor = _linkColor,
    ActiveLinkColor = _accentColor,
    VisitedLinkColor = _linkColor,
    BackColor = Color.Transparent,
    LinkBehavior = LinkBehavior.HoverUnderline
}
```

## Spacing Guidelines

- Window padding: 15px
- Column margin: 4px
- Internal content padding: 15px from edges
- Section divider spacing: 15px below divider
- Line height for body text: 18-20px
- Category header margin: 8px top, 3px bottom
- List item margin: 1-2px vertical

## Theme Toggle Implementation

### State Management
```csharp
private bool _isDarkMode = true;
private bool _isAnimating = false;
private Timer _fadeTimer;
private Color _bgColor, _cardColor, _borderColor, _accentColor;
private Color _textColor, _subtextColor, _linkColor, _listBgColor;
```

### Smooth Fade Transition
Use Form.Opacity for smooth theme switching:
```csharp
private void ToggleThemeWithFade()
{
    if (_isAnimating) return;
    _isAnimating = true;

    int step = 0;
    _fadeTimer = new Timer { Interval = 15 };
    _fadeTimer.Tick += (s, e) =>
    {
        step++;
        if (step <= 10)
        {
            // Fade out
            this.Opacity = 1.0 - (step * 0.1);
        }
        else if (step == 11)
        {
            // Switch theme at minimum opacity
            ToggleTheme();
        }
        else if (step <= 21)
        {
            // Fade in
            this.Opacity = (step - 11) * 0.1;
        }
        else
        {
            // Done
            this.Opacity = 1.0;
            _fadeTimer.Stop();
            _fadeTimer.Dispose();
            _isAnimating = false;
        }
    };
    _fadeTimer.Start();
}
```

## Future Considerations

1. **App-wide Theming**: Extract colors to a central `ThemeManager` class
2. **User Preference**: Store theme preference in settings
3. **System Theme Detection**: Follow Windows dark/light mode
4. **Custom Themes**: Allow user-defined color schemes
5. **Apply to Other Windows**: Use About window as template for other dialogs

## Reference Implementation

See `src/EVEMon/About/AboutWindow.cs` for the complete reference implementation of this theming system.
