using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace EVEMon.Avalonia.Converters;

/// <summary>
/// Converter for skill level boxes - returns true if level >= threshold.
/// </summary>
public class LevelAtLeastConverter : IValueConverter
{
    /// <summary>
    /// Singleton instance for level >= 1.
    /// </summary>
    public static readonly LevelAtLeastConverter Level1 = new(1);

    /// <summary>
    /// Singleton instance for level >= 2.
    /// </summary>
    public static readonly LevelAtLeastConverter Level2 = new(2);

    /// <summary>
    /// Singleton instance for level >= 3.
    /// </summary>
    public static readonly LevelAtLeastConverter Level3 = new(3);

    /// <summary>
    /// Singleton instance for level >= 4.
    /// </summary>
    public static readonly LevelAtLeastConverter Level4 = new(4);

    /// <summary>
    /// Singleton instance for level >= 5.
    /// </summary>
    public static readonly LevelAtLeastConverter Level5 = new(5);

    private readonly int _threshold;

    public LevelAtLeastConverter(int threshold)
    {
        _threshold = threshold;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long level)
        {
            return level >= _threshold;
        }
        if (value is int intLevel)
        {
            return intLevel >= _threshold;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter that returns a filled brush if level >= threshold, otherwise transparent.
/// Uses green for normal skills, yellow/gold for Omega-only skills when character is Alpha.
/// </summary>
public class LevelToBrushConverter : IMultiValueConverter
{
    /// <summary>
    /// Singleton instance for level >= 1.
    /// </summary>
    public static readonly LevelToBrushConverter Level1 = new(1);

    /// <summary>
    /// Singleton instance for level >= 2.
    /// </summary>
    public static readonly LevelToBrushConverter Level2 = new(2);

    /// <summary>
    /// Singleton instance for level >= 3.
    /// </summary>
    public static readonly LevelToBrushConverter Level3 = new(3);

    /// <summary>
    /// Singleton instance for level >= 4.
    /// </summary>
    public static readonly LevelToBrushConverter Level4 = new(4);

    /// <summary>
    /// Singleton instance for level >= 5.
    /// </summary>
    public static readonly LevelToBrushConverter Level5 = new(5);

    // Normal skill colors (green)
    private static readonly IBrush NormalFilledBrush = new SolidColorBrush(Color.Parse("#4ecca3"));
    private static readonly IBrush EmptyBrush = Brushes.Transparent;

    // Restricted skill colors (yellow/gold for Omega-only when Alpha)
    private static readonly IBrush RestrictedFilledBrush = new SolidColorBrush(Color.Parse("#e6b800"));

    private readonly int _threshold;

    public LevelToBrushConverter(int threshold)
    {
        _threshold = threshold;
    }

    /// <summary>
    /// Converts skill level and restriction status to a brush.
    /// Values[0] = Level (long or int)
    /// Values[1] = ShowYellowBoxes (bool) - true for Omega-only skills when character is Alpha
    /// </summary>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count < 2)
            return EmptyBrush;

        // Get the level
        long level = 0;
        if (values[0] is long longLevel)
            level = longLevel;
        else if (values[0] is int intLevel)
            level = intLevel;

        // Get the restriction status
        bool showYellowBoxes = values[1] is bool restricted && restricted;

        // If level is below threshold, return empty
        if (level < _threshold)
            return EmptyBrush;

        // Return appropriate color based on restriction status
        return showYellowBoxes ? RestrictedFilledBrush : NormalFilledBrush;
    }
}

/// <summary>
/// Converter that returns the border brush based on skill restriction status.
/// Yellow/gold for Omega-only skills when Alpha, green for normal.
/// </summary>
public class SkillBorderBrushConverter : IValueConverter
{
    public static readonly SkillBorderBrushConverter Instance = new();

    private static readonly IBrush NormalBorderBrush = new SolidColorBrush(Color.Parse("#4ecca3"));
    private static readonly IBrush RestrictedBorderBrush = new SolidColorBrush(Color.Parse("#e6b800"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool showYellow && showYellow)
        {
            return RestrictedBorderBrush;
        }
        return NormalBorderBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
