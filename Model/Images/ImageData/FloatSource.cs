namespace Model.Images;

public class FloatSource(float f) : IImageDataSource
{
    public float F { get; } = f;
    public List<IDataOperation> Operations { get; } = [];
}