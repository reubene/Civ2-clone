using Model.Images;
using Raylib_CSharp.Colors;

namespace RaylibUtils;

public class PlayerColour
{
    public const int TextColourIndex = 0;
    public const int LightColourIndex = 1;
    public IImageSource Image { get; init; }

    public Color TextColour => Colours[TextColourIndex];

    public List<Color> Colours { get; init; } = new();
}