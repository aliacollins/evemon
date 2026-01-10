using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace EVEMon.Avalonia.Controls;

/// <summary>
/// A custom progress bar styled for skill training display.
/// </summary>
public class SkillProgressBar : Control
{
    /// <summary>
    /// Defines the <see cref="Value"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<SkillProgressBar, double>(nameof(Value), 0);

    /// <summary>
    /// Defines the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<SkillProgressBar, double>(nameof(Maximum), 100);

    /// <summary>
    /// Defines the <see cref="ProgressBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ProgressBrushProperty =
        AvaloniaProperty.Register<SkillProgressBar, IBrush?>(nameof(ProgressBrush));

    /// <summary>
    /// Defines the <see cref="BackgroundBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundBrushProperty =
        AvaloniaProperty.Register<SkillProgressBar, IBrush?>(nameof(BackgroundBrush));

    /// <summary>
    /// Defines the <see cref="CornerRadius"/> property.
    /// </summary>
    public static readonly StyledProperty<double> CornerRadiusProperty =
        AvaloniaProperty.Register<SkillProgressBar, double>(nameof(CornerRadius), 2);

    /// <summary>
    /// Defines the <see cref="ShowPercentage"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowPercentageProperty =
        AvaloniaProperty.Register<SkillProgressBar, bool>(nameof(ShowPercentage), false);

    /// <summary>
    /// Defines the <see cref="SkillLevel"/> property.
    /// </summary>
    public static readonly StyledProperty<int> SkillLevelProperty =
        AvaloniaProperty.Register<SkillProgressBar, int>(nameof(SkillLevel), 0);

    /// <summary>
    /// Defines the <see cref="ShowLevelMarkers"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLevelMarkersProperty =
        AvaloniaProperty.Register<SkillProgressBar, bool>(nameof(ShowLevelMarkers), false);

    /// <summary>
    /// Gets or sets the current progress value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush for the progress fill.
    /// </summary>
    public IBrush? ProgressBrush
    {
        get => GetValue(ProgressBrushProperty);
        set => SetValue(ProgressBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush for the background.
    /// </summary>
    public IBrush? BackgroundBrush
    {
        get => GetValue(BackgroundBrushProperty);
        set => SetValue(BackgroundBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public double CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the percentage text.
    /// </summary>
    public bool ShowPercentage
    {
        get => GetValue(ShowPercentageProperty);
        set => SetValue(ShowPercentageProperty, value);
    }

    /// <summary>
    /// Gets or sets the current skill level (0-5).
    /// </summary>
    public int SkillLevel
    {
        get => GetValue(SkillLevelProperty);
        set => SetValue(SkillLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show level markers (5 segments).
    /// </summary>
    public bool ShowLevelMarkers
    {
        get => GetValue(ShowLevelMarkersProperty);
        set => SetValue(ShowLevelMarkersProperty, value);
    }

    static SkillProgressBar()
    {
        AffectsRender<SkillProgressBar>(ValueProperty, MaximumProperty, ProgressBrushProperty,
            BackgroundBrushProperty, CornerRadiusProperty, ShowPercentageProperty,
            SkillLevelProperty, ShowLevelMarkersProperty);
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        var radius = CornerRadius;

        // Default brushes
        var bgBrush = BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(30, 30, 50));
        var progressBrush = ProgressBrush ?? new SolidColorBrush(Color.FromRgb(68, 136, 255));

        // Draw background
        var bgGeometry = new RoundedRect(bounds, radius, radius, radius, radius);
        context.DrawRectangle(bgBrush, null, bgGeometry);

        // Calculate progress width
        double percentage = Maximum > 0 ? Value / Maximum : 0;
        percentage = System.Math.Clamp(percentage, 0, 1);

        if (percentage > 0)
        {
            double progressWidth = bounds.Width * percentage;
            var progressRect = new Rect(0, 0, progressWidth, bounds.Height);
            var progressGeometry = new RoundedRect(progressRect, radius, radius, radius, radius);
            context.DrawRectangle(progressBrush, null, progressGeometry);
        }

        // Draw level markers if enabled
        if (ShowLevelMarkers)
        {
            var markerPen = new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)), 1);
            double segmentWidth = bounds.Width / 5;

            for (int i = 1; i < 5; i++)
            {
                double x = segmentWidth * i;
                context.DrawLine(markerPen, new Point(x, 0), new Point(x, bounds.Height));
            }

            // Fill completed levels
            if (SkillLevel > 0)
            {
                var completedBrush = new SolidColorBrush(Color.FromRgb(68, 255, 68));
                for (int i = 0; i < SkillLevel && i < 5; i++)
                {
                    var levelRect = new Rect(i * segmentWidth + 1, 1, segmentWidth - 2, bounds.Height - 2);
                    context.FillRectangle(completedBrush, levelRect);
                }
            }
        }

        // Draw percentage text
        if (ShowPercentage)
        {
            string percentText = $"{percentage * 100:F1}%";
            var formattedText = new FormattedText(
                percentText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                10,
                Brushes.White);

            var textPoint = new Point(
                (bounds.Width - formattedText.Width) / 2,
                (bounds.Height - formattedText.Height) / 2);

            context.DrawText(formattedText, textPoint);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // Default height if not specified
        double height = double.IsInfinity(availableSize.Height) ? 8 : availableSize.Height;
        double width = double.IsInfinity(availableSize.Width) ? 100 : availableSize.Width;
        return new Size(width, height);
    }
}

/// <summary>
/// A skill level indicator showing 5 boxes for skill levels I-V.
/// </summary>
public class SkillLevelIndicator : Control
{
    /// <summary>
    /// Defines the <see cref="Level"/> property.
    /// </summary>
    public static readonly StyledProperty<int> LevelProperty =
        AvaloniaProperty.Register<SkillLevelIndicator, int>(nameof(Level), 0);

    /// <summary>
    /// Defines the <see cref="TrainingLevel"/> property (partially trained level).
    /// </summary>
    public static readonly StyledProperty<int> TrainingLevelProperty =
        AvaloniaProperty.Register<SkillLevelIndicator, int>(nameof(TrainingLevel), 0);

    /// <summary>
    /// Defines the <see cref="TrainingProgress"/> property (0-1 for partial level).
    /// </summary>
    public static readonly StyledProperty<double> TrainingProgressProperty =
        AvaloniaProperty.Register<SkillLevelIndicator, double>(nameof(TrainingProgress), 0);

    /// <summary>
    /// Gets or sets the current trained level (0-5).
    /// </summary>
    public int Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the level currently being trained (0-5).
    /// </summary>
    public int TrainingLevel
    {
        get => GetValue(TrainingLevelProperty);
        set => SetValue(TrainingLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the training progress for the current level (0-1).
    /// </summary>
    public double TrainingProgress
    {
        get => GetValue(TrainingProgressProperty);
        set => SetValue(TrainingProgressProperty, value);
    }

    static SkillLevelIndicator()
    {
        AffectsRender<SkillLevelIndicator>(LevelProperty, TrainingLevelProperty, TrainingProgressProperty);
    }

    public override void Render(DrawingContext context)
    {
        var bounds = Bounds;
        double boxSize = System.Math.Min(bounds.Width / 5 - 2, bounds.Height - 2);
        double spacing = 2;
        double totalWidth = (boxSize * 5) + (spacing * 4);
        double startX = (bounds.Width - totalWidth) / 2;
        double y = (bounds.Height - boxSize) / 2;

        var emptyBrush = new SolidColorBrush(Color.FromRgb(40, 40, 60));
        var trainedBrush = new SolidColorBrush(Color.FromRgb(68, 255, 68));
        var trainingBrush = new SolidColorBrush(Color.FromRgb(68, 136, 255));
        var borderPen = new Pen(new SolidColorBrush(Color.FromRgb(80, 80, 100)), 1);

        for (int i = 0; i < 5; i++)
        {
            double x = startX + (i * (boxSize + spacing));
            var rect = new Rect(x, y, boxSize, boxSize);
            int levelNum = i + 1;

            IBrush fillBrush;
            if (levelNum <= Level)
            {
                fillBrush = trainedBrush;
            }
            else if (levelNum == TrainingLevel && TrainingProgress > 0)
            {
                // Partially trained - show progress
                context.FillRectangle(emptyBrush, rect);
                var progressRect = new Rect(x, y + boxSize * (1 - TrainingProgress), boxSize, boxSize * TrainingProgress);
                context.FillRectangle(trainingBrush, progressRect);
                context.DrawRectangle(borderPen, rect);
                continue;
            }
            else
            {
                fillBrush = emptyBrush;
            }

            context.FillRectangle(fillBrush, rect);
            context.DrawRectangle(borderPen, rect);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double height = double.IsInfinity(availableSize.Height) ? 16 : availableSize.Height;
        double width = double.IsInfinity(availableSize.Width) ? 90 : availableSize.Width;
        return new Size(width, height);
    }
}
