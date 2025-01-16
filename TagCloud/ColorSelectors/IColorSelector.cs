using System.Drawing;
using TagCloudApplication;

namespace TagCloud.ColorSelectors;

public interface IColorSelector
{
    Color SetColor();
    bool IsMatch(Options options);
}