namespace Model.Graphics;

public record Drawing (DrawingType type, Rectangle bounds, float thickness, Color color);

public enum DrawingType
{
    HollowRect
}
