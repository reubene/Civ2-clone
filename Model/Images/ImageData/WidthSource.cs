namespace Model.Images;

public class WidthSource(IImageSource baseImage) : IImageDataSource
{
    public IImageSource BaseImage { get; } = baseImage;

    public List<IDataOperation> Operations { get; } = [];
    
    public static WidthSource operator -(WidthSource source, int num)
    {
        source.Operations.Add(new Sub(num));
        return source;
    }
    
    public static WidthSource operator /(WidthSource source, int num)
    {
        source.Operations.Add(new Divide(num));
        return source;
    }
}