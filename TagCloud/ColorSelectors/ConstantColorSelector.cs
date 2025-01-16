using System.Drawing;
using TagCloud.ColorSelectors;
using TagCloudApplication;
using TagCloudTests;
using ColorConverter = TagCloud.Extensions.ColorConverter;

namespace TagCloud;

public class ConstantColorSelector : IColorSelector
{
    private Color _color;

    public ConstantColorSelector(Color color)
    {
        _color = color;
    }

    public Color SetColor() => _color;

    public static IColorSelector? CreateFromOptions(Options options) =>
        !ColorConverter.TryConvert(options.ColorScheme, out var convertedColor)
            ? null
            : new ConstantColorSelector(convertedColor);
}