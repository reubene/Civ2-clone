namespace Model.Images;

public class ComputedImage : IImageSource
{
    private readonly string? _name;
    private readonly IImageSource _baseImage;

    public ComputedImage(IImageSource baseImage, string? name) : this(baseImage)
    {
        _name = name;
    }
    
    public ComputedImage(IImageSource baseImage, Rectangle subsection, IImageSource? mask = null) : this(baseImage)
    {
        Steps.Add(new SubsectionStep(subsection));
        if (mask != null)
        {
            Steps.Add(new MaskStep(mask));
        }
    }

    public ComputedImage(IImageSource baseImage)
    {
        if (baseImage is not ComputedImage computedImage)
        {
            _baseImage = baseImage;
            return;
        }
        _baseImage = computedImage.BaseImage;
        Steps.AddRange(computedImage.Steps);
    }

    public ImageStorage Type => ImageStorage.Computed;
    
    public IImageSource BaseImage => _baseImage;
    public List<IComputedStep> Steps { get; } = [];

    public string GetKey(int ownerId = -1)
    {
        if (!string.IsNullOrWhiteSpace(_name))
        {
            return ownerId != -1 ? $"{_name}_{ownerId}" : $"{_name}";
        }
        return $"{_baseImage.GetKey(ownerId)}-{string.Join("-", Steps.Select(s => s.GetKey(ownerId)))}";
    }
}