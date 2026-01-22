namespace Model.Graphics;

public struct Color(byte r, byte g, byte b, byte a)
{
    public static readonly Color LightGray = new(200, 200, 200, byte.MaxValue);
    public static readonly Color Gray = new(130, 130, 130, byte.MaxValue);
    public static readonly Color DarkGray = new(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/, byte.MaxValue);
    public static readonly Color Yellow = new(253, 249, 0, byte.MaxValue);
    public static readonly Color Gold = new(byte.MaxValue, 203, 0, byte.MaxValue);
    public static readonly Color Orange = new(byte.MaxValue, 161, 0, byte.MaxValue);
    public static readonly Color Pink = new(byte.MaxValue, 109, 194, byte.MaxValue);
    public static readonly Color Red = new(230, 41, 55, byte.MaxValue);
    public static readonly Color Maroon = new(190, 33, 55, byte.MaxValue);
    public static readonly Color Green = new(0, 228, 48 /*0x30*/, byte.MaxValue);
    public static readonly Color Lime = new(0, 158, 47, byte.MaxValue);
    public static readonly Color DarkGreen = new(0, 117, 44, byte.MaxValue);
    public static readonly Color SkyBlue = new(102, 191, byte.MaxValue, byte.MaxValue);
    public static readonly Color Blue = new(0, 121, 241, byte.MaxValue);
    public static readonly Color DarkBlue = new(0, 82, 172, byte.MaxValue);
    public static readonly Color Purple = new(200, 122, byte.MaxValue, byte.MaxValue);
    public static readonly Color Violet = new(135, 60, 190, byte.MaxValue);
    public static readonly Color DarkPurple = new(112 /*0x70*/, 31 /*0x1F*/, 126, byte.MaxValue);
    public static readonly Color Beige = new(211, 176 /*0xB0*/, 131, byte.MaxValue);
    public static readonly Color Brown = new(127 /*0x7F*/, 106, 79, byte.MaxValue);
    public static readonly Color DarkBrown = new(76, 63 /*0x3F*/, 47, byte.MaxValue);
    public static readonly Color White = new(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
    public static readonly Color Black = new(0, 0, 0, byte.MaxValue);
    public static readonly Color Blank = new(0, 0, 0, 0);
    public static readonly Color Magenta = new(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
    public static readonly Color RayWhite = new(245, 245, 245, byte.MaxValue);
    /// <summary>Color red value.</summary>
    public byte R = r;
    /// <summary>Color green value.</summary>
    public byte G = g;
    /// <summary>Color blue value.</summary>
    public byte B = b;
    /// <summary>Color alpha value.</summary>
    public byte A = a;

    public string ToHex()
    {
        return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    }
}