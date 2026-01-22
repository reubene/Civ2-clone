using Model.Graphics;

namespace Model.Images;

public class ReplaceColour(Color? original, Color replacement) : IComputedStep
{
    public int Index { get; } = -1;
    public Color? Original { get; } = original;
    public Color Replacement { get; } = replacement;
    public string GetKey(int ownerId = -1)
    {
        return "ReplaceColour:" + (Index != -1 ? Index : Original?.ToHex()) + "-" + Replacement.ToHex();
    }

    public ReplaceColour(int index, Color replacement) : this(null, replacement)
    {
        Index = index;
    }
}