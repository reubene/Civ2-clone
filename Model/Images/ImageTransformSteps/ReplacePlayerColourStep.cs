using Model.Graphics;

namespace Model.Images;

public class ReplacePlayerColourStep(int index, Color replacement) : IComputedStep
{
    public int Index { get; } = index;
    public Color Replacement { get; } = replacement;

    public string GetKey(int ownerId = -1)
    {
        return "ReplacePlayerColour:" + Index + "-" + Replacement.ToHex() + "-" + ownerId;
    }
}