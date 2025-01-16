using System.Drawing;
using TagCloud.ColorSelectors;
using TagCloudApplication;
using TagCloudTests;
using ColorConverter = TagCloud.Extensions.ColorConverter;

namespace TagCloud;

public class ConstantColorSelector(Color color) : IColorSelector
{
    private Color _color = color;
    public Color SetColor() => _color;

    public bool IsMatch(Options options)
    {
        if (!ColorConverter.TryConvert(options.ColorScheme, out var convertedColor)) return false;
        _color = convertedColor;
        return true;
    }
}