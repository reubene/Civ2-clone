namespace Model.Images;

public class MaskStep(IImageSource mask) : IComputedStep
{
    public IImageSource Mask { get; } = mask;

    public string GetKey(int ownerId = -1)
    {
        return "Mask:" + Mask.GetKey(ownerId);
    }
}