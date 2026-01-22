namespace Model.Images;

public class Blank(int width, int height) : IImageSource
{
    public int Height { get; set; } = height;

    public int Width { get; set; } = width;
    public ImageStorage Type => ImageStorage.Blank;
    public string GetKey(int ownerId = -1)
    {
        return "Blank";
    }
}