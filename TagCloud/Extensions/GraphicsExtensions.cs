using System.Drawing;
using TagCloud.ColorSelectors;
using TagCloudTests;

namespace TagCloud.Extensions;

public static class GraphicsExtensions
{
    public static void DrawStrings(this Graphics graphics, IColorSelector selector, TextRectangle[] rectangles)
    {
        using var brush = new SolidBrush(selector.SetColor());
        foreach (var rectangle in rectangles)
        {
            graphics.DrawString(rectangle.Text, rectangle.Font, brush, rectangle.X, rectangle.Y);
            brush.Color = selector.SetColor();
        }
    }
}