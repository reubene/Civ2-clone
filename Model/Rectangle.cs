namespace Model;

public class Rectangle
{
    public Rectangle()
    {
        
    }

    public Rectangle(int x, int y, int width, int height) : this()
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public int X { get; init; }
    public int Y { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public void Deconstruct(out int X, out int Y, out int Width, out int Height)
    {
        X = this.X;
        Y = this.Y;
        Width = this.Width;
        Height = this.Height;
    }
}

