using Model.Graphics;
using Raylib_CSharp.Images;

namespace Model.Images;

public class MemoryStorage : IImageSource
{
    private readonly string _name;

    public MemoryStorage(Image baseImage, string name)
    {
        Image = baseImage;
        _name = name;
    }

    public ImageStorage Type => ImageStorage.Memory;
    public Image Image { get; }

    public string GetKey(int ownerId = -1)
    {
        if (ownerId == -1)
        {
            return _name;
        }

        return _name + "-" + ownerId;
    }
}