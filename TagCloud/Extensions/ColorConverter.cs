using System.Drawing;
using System.Text.RegularExpressions;

namespace TagCloud.Extensions;

public static class ColorConverter
{
    public static bool TryConvert(string hexString, out Color color)
    {
        color = default;
        var colorHexRegExp = new Regex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
        
        if (colorHexRegExp.Count(hexString) != 1)
            return false;

        var hex = hexString.Replace("#", "");
        
        if (hex.Length == 3)
        {
            hex = string.Concat(hex.Select(c => $"{c}{c}"));
        }
        
        var rgbValue = hex
            .Chunk(2)
            .Select(chars => Convert.ToInt32(new string(chars), 16))
            .ToArray();
            
        color = Color.FromArgb(rgbValue[0], rgbValue[1], rgbValue[2]);
        return true;
    }
}