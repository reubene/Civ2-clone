using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;

namespace RaylibUtils;

public static class Converter
{
    public static Color AsRl(this Model.Graphics.Color color)
    {
        return new  Color(color.R, color.G, color.B, color.A);
    }

    public static Rectangle AsRl(this Model.Rectangle? s)
    {
        return new Rectangle(s.X, s.Y, s.Width, s.Height);
    }
}