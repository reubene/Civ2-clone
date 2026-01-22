namespace Model.Images;

public interface IImageDataSource
{
    List<IDataOperation> Operations { get; }
}