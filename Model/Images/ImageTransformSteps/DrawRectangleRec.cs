using Model.Graphics;

namespace Model.Images.ImageTransformSteps;

public class DrawRectangleRec(Rectangle rectangle, float thickness, Color color) : IComputedStep
{
    public Rectangle Rectangle { get; } = rectangle;
    public float Thickness { get; } = thickness;
    public Color Color { get; } = color;

    public string GetKey(int ownerId = -1)
    {
        return $"DrawRectangleRec{Rectangle.X}{Rectangle.Y}{Rectangle.Width}{Rectangle.Height}{Thickness}{Color.ToHex()}";
    }
}