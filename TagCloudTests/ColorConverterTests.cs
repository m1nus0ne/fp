using System.Drawing;
using ColorConverter = TagCloud.Extensions.ColorConverter;

namespace TagCloudTests;

public class ColorConverterTests
{
    public static IEnumerable<TestCaseData> RightCases
    {
        get
        {
            yield return new TestCaseData("#FFFFFF").Returns(Color.FromArgb(255, 255, 255));
            yield return new TestCaseData("#FF0000").Returns(Color.FromArgb(255, 0, 0));
            yield return new TestCaseData("#808080").Returns(Color.FromArgb(128, 128, 128));
            yield return new TestCaseData("#123456").Returns(Color.FromArgb(18, 52, 86));
        }
    }

    [TestCaseSource(typeof(ColorConverterTests), nameof(RightCases))]
    public Color Converter_ShouldConvertStringToColor_WhenStringInRightFormat(string hexString)
    {
        ColorConverter.TryConvert(hexString, out var color).Should().BeTrue();
        return color;
    }
}