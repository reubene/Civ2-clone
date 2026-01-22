namespace Model.Images;

public class HeightSource(IImageSource baseImage) : IImageDataSource
{
    public IImageSource BaseImage { get; } = baseImage;

    public List<IDataOperation> Operations { get; } = [];

    public static HeightSource operator -(HeightSource source, decimal num)
    {
        source.Operations.Add(new Sub(num));
        return source;
    }
    
    public static HeightSource operator /(HeightSource source, int num)
    {
        source.Operations.Add(new Divide(num));
        return source;
    }
}