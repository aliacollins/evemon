using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using EVEMon.Avalonia.Services;

namespace EVEMon.Avalonia.Controls;

/// <summary>
/// An image control that loads images asynchronously with placeholder support.
/// </summary>
public class AsyncImage : Control
{
    private Bitmap? _loadedImage;
    private bool _isLoading;
    private CancellationTokenSource? _loadCts;

    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> SourceProperty =
        AvaloniaProperty.Register<AsyncImage, object?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PlaceholderProperty =
        AvaloniaProperty.Register<AsyncImage, IBrush?>(nameof(Placeholder), Brushes.Gray);

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<AsyncImage, string?>(nameof(PlaceholderText));

    /// <summary>
    /// Defines the <see cref="CornerRadius"/> property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<AsyncImage, CornerRadius>(nameof(CornerRadius));

    /// <summary>
    /// Defines the <see cref="Stretch"/> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<AsyncImage, Stretch>(nameof(Stretch), Stretch.Uniform);

    /// <summary>
    /// Defines the <see cref="ImageSize"/> property for character portraits.
    /// </summary>
    public static readonly StyledProperty<int> ImageSizeProperty =
        AvaloniaProperty.Register<AsyncImage, int>(nameof(ImageSize), 128);

    /// <summary>
    /// Gets or sets the image source.
    /// Can be a Bitmap, Uri, string URL, or long (character ID for portrait).
    /// </summary>
    public object? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder brush shown while loading.
    /// </summary>
    public IBrush? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text shown while loading.
    /// </summary>
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius for rounded images.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets how the image is stretched.
    /// </summary>
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Gets or sets the requested image size for character portraits.
    /// </summary>
    public int ImageSize
    {
        get => GetValue(ImageSizeProperty);
        set => SetValue(ImageSizeProperty, value);
    }

    static AsyncImage()
    {
        AffectsRender<AsyncImage>(SourceProperty, PlaceholderProperty, PlaceholderTextProperty,
            CornerRadiusProperty, StretchProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            LoadImageAsync();
        }
    }

    private async void LoadImageAsync()
    {
        // Cancel any pending load
        _loadCts?.Cancel();
        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;

        _loadedImage = null;
        _isLoading = true;
        InvalidateVisual();

        try
        {
            var source = Source;
            Bitmap? image = null;

            if (source is Bitmap bitmap)
            {
                image = bitmap;
            }
            else if (source is long characterId && characterId > 0)
            {
                // Load character portrait
                image = await AvaloniaImageService.Instance.GetCharacterPortraitAsync(characterId, ImageSize);
            }
            else if (source is Uri uri)
            {
                image = await AvaloniaImageService.Instance.GetImageAsync(uri);
            }
            else if (source is string urlString && !string.IsNullOrEmpty(urlString))
            {
                if (Uri.TryCreate(urlString, UriKind.Absolute, out var parsedUri))
                {
                    image = await AvaloniaImageService.Instance.GetImageAsync(parsedUri);
                }
            }

            if (token.IsCancellationRequested)
                return;

            _loadedImage = image;
            _isLoading = false;

            await Dispatcher.UIThread.InvokeAsync(InvalidateVisual);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"AsyncImage load error: {ex.Message}");
            _isLoading = false;
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);

        // Apply corner radius clipping if needed
        if (CornerRadius != default)
        {
            var clipGeometry = new RectangleGeometry(bounds) { Rect = bounds };
            // For rounded corners, we'd need a more complex geometry
            // For now, just draw with the geometry
        }

        if (_loadedImage != null)
        {
            // Calculate destination rect based on stretch mode
            var destRect = CalculateDestRect(bounds, _loadedImage.Size);
            context.DrawImage(_loadedImage, destRect);
        }
        else
        {
            // Draw placeholder
            if (Placeholder != null)
            {
                context.FillRectangle(Placeholder, bounds);
            }

            // Draw placeholder text
            if (!string.IsNullOrEmpty(PlaceholderText) && !_isLoading)
            {
                var formattedText = new FormattedText(
                    PlaceholderText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    Typeface.Default,
                    12,
                    Brushes.White);

                var textPoint = new Point(
                    (bounds.Width - formattedText.Width) / 2,
                    (bounds.Height - formattedText.Height) / 2);

                context.DrawText(formattedText, textPoint);
            }
            else if (_isLoading)
            {
                // Draw loading indicator (simple text for now)
                var loadingText = new FormattedText(
                    "...",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    Typeface.Default,
                    14,
                    Brushes.LightGray);

                var textPoint = new Point(
                    (bounds.Width - loadingText.Width) / 2,
                    (bounds.Height - loadingText.Height) / 2);

                context.DrawText(loadingText, textPoint);
            }
        }
    }

    private Rect CalculateDestRect(Rect bounds, Size imageSize)
    {
        return Stretch switch
        {
            Stretch.None => new Rect(
                (bounds.Width - imageSize.Width) / 2,
                (bounds.Height - imageSize.Height) / 2,
                imageSize.Width,
                imageSize.Height),
            Stretch.Fill => bounds,
            Stretch.Uniform => CalculateUniformRect(bounds, imageSize),
            Stretch.UniformToFill => CalculateUniformToFillRect(bounds, imageSize),
            _ => bounds
        };
    }

    private static Rect CalculateUniformRect(Rect bounds, Size imageSize)
    {
        double scaleX = bounds.Width / imageSize.Width;
        double scaleY = bounds.Height / imageSize.Height;
        double scale = Math.Min(scaleX, scaleY);

        double width = imageSize.Width * scale;
        double height = imageSize.Height * scale;

        return new Rect(
            (bounds.Width - width) / 2,
            (bounds.Height - height) / 2,
            width,
            height);
    }

    private static Rect CalculateUniformToFillRect(Rect bounds, Size imageSize)
    {
        double scaleX = bounds.Width / imageSize.Width;
        double scaleY = bounds.Height / imageSize.Height;
        double scale = Math.Max(scaleX, scaleY);

        double width = imageSize.Width * scale;
        double height = imageSize.Height * scale;

        return new Rect(
            (bounds.Width - width) / 2,
            (bounds.Height - height) / 2,
            width,
            height);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // If we have a loaded image, use its size (respecting available size)
        if (_loadedImage != null)
        {
            var imageSize = _loadedImage.Size;
            if (Stretch == Stretch.None)
            {
                return new Size(
                    Math.Min(imageSize.Width, availableSize.Width),
                    Math.Min(imageSize.Height, availableSize.Height));
            }
        }

        // Otherwise, use available size or a default
        return availableSize;
    }
}
