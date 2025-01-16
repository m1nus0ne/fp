using System.Drawing;
using TagCloud.ColorSelectors;
using TagCloudApplication;

namespace TagCloudTests;

public class GrayScaleColorSelector : IColorSelector
{
    private readonly Random random = new(DateTime.Now.Microsecond);

    public Color SetColor()
    {
        var gray = random.Next(100, 200);
        return Color.FromArgb(gray, gray, gray);
    }

    public static IColorSelector? CreateFromOptions(Options options) =>
        options.ColorScheme == "gray" ? new GrayScaleColorSelector() : null;
}