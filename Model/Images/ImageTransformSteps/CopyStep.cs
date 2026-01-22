namespace Model.Images;

public class CopyStep : IComputedStep
{
    public string GetKey(int ownerId = -1)
    {
        return "Copy";
    }
}