using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using EVEMon.Avalonia.Services;

namespace EVEMon.Avalonia.Converters;

/// <summary>
/// Converts a character ID to a portrait bitmap.
/// </summary>
public class CharacterPortraitConverter : IValueConverter
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static CharacterPortraitConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not long characterId || characterId <= 0)
            return null;

        int size = 128;
        if (parameter is int paramSize)
        {
            size = paramSize;
        }
        else if (parameter is string sizeStr && int.TryParse(sizeStr, out int parsedSize))
        {
            size = parsedSize;
        }

        // Return a task that will be handled by async binding
        // Note: For proper async image loading, use the AsyncImage control instead
        return AvaloniaImageService.Instance.GetCharacterPortraitAsync(characterId, size)
            .GetAwaiter().GetResult();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a corporation ID to a logo bitmap.
/// </summary>
public class CorporationLogoConverter : IValueConverter
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static CorporationLogoConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not long corporationId || corporationId <= 0)
            return null;

        int size = 64;
        if (parameter is string sizeStr && int.TryParse(sizeStr, out int parsedSize))
        {
            size = parsedSize;
        }

        return AvaloniaImageService.Instance.GetCorporationLogoAsync(corporationId, size)
            .GetAwaiter().GetResult();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts an alliance ID to a logo bitmap.
/// </summary>
public class AllianceLogoConverter : IValueConverter
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static AllianceLogoConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not long allianceId || allianceId <= 0)
            return null;

        int size = 64;
        if (parameter is string sizeStr && int.TryParse(sizeStr, out int parsedSize))
        {
            size = parsedSize;
        }

        return AvaloniaImageService.Instance.GetAllianceLogoAsync(allianceId, size)
            .GetAwaiter().GetResult();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Common converters for number formatting.
/// </summary>
public static class FormatConverters
{
    /// <summary>
    /// Formats a number with thousand separators.
    /// </summary>
    public static readonly IValueConverter NumberFormat = new FuncValueConverter<long, string>(
        value => value.ToString("N0"));

    /// <summary>
    /// Formats a decimal as ISK currency.
    /// </summary>
    public static readonly IValueConverter IskFormat = new FuncValueConverter<decimal, string>(
        value => $"{value:N2} ISK");

    /// <summary>
    /// Formats skill points with "SP" suffix.
    /// </summary>
    public static readonly IValueConverter SkillPointsFormat = new FuncValueConverter<long, string>(
        value => $"{value:N0} SP");

    /// <summary>
    /// Formats a percentage.
    /// </summary>
    public static readonly IValueConverter PercentFormat = new FuncValueConverter<double, string>(
        value => $"{value:F1}%");
}

/// <summary>
/// Simple function-based value converter.
/// </summary>
public class FuncValueConverter<TIn, TOut> : IValueConverter
{
    private readonly Func<TIn, TOut> _convert;

    public FuncValueConverter(Func<TIn, TOut> convert)
    {
        _convert = convert;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TIn typedValue)
        {
            return _convert(typedValue);
        }
        return default(TOut);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
