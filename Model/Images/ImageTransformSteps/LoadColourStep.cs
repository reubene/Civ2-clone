namespace Model.Images;

public class LoadColourStep(int index) : IComputedStep
{
    public int Index { get; } = index;

    public string GetKey(int ownerId = -1)
    {
        return  "LoadColour:"  + Index;
    }
}