using Model.Graphics;
using Raylib_CSharp.Images;

namespace Model.Images;

public interface IImageSource
{
    string GetKey(int ownerId = -1);
}