namespace Model.Images;

public class ImageDataVector(IImageDataSource x, IImageDataSource y)
{
    public ImageDataVector(float x, int y) : this(new FloatSource(x), new FloatSource(y))
    {
    }
    
    public ImageDataVector(IImageDataSource x, int y) : this(x, new FloatSource(y))
    {
    }
    
    public ImageDataVector(float x, IImageDataSource y) : this(new FloatSource(x), y)
    {
    } 
    public IImageDataSource X { get; } = x;
    public IImageDataSource Y { get; } = y;
}