using Model.Graphics;

namespace Model.Images;

public class DrawRectangleStep : IComputedStep
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Color Color { get; }

    public DrawRectangleStep(int x, int y, int width, int height, Color color)
    {
        this.X = x;
        this.Y = y;
        Width = width;
        Height = height;
        Color = color;
    }

    public string GetKey(int ownerId = -1)
    {
        return $"DrawRectangle({X},{Y},{Width},{Height},{Color})";
    }
}