namespace Model.Images;

public class SubsectionStep(Rectangle rectangle) : IComputedStep
{
    public Rectangle Rectangle { get; } = rectangle;

    public string GetKey(int ownerId = -1)
    {
        return Rectangle.X + "-" + Rectangle.Y + "-" + Rectangle.Width + "-" + Rectangle.Height;
    }
}