using System.Drawing;
using TagCloudApplication;

namespace TagCloud.ColorSelectors;

public interface IColorSelector
{
    Color SetColor();
    static IColorSelector? CreateFromOptions(Options options)
    {
        return null;
    }
}