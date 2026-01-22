using Model.Graphics;
using Model.Images.ImageTransformSteps;

namespace Model.Images;

public static class IImageSourceExtensions
{

    public static IImageSource LoadColors(this IImageSource baseImage, int index)
    {
        ArgumentNullException.ThrowIfNull(baseImage);

        var step = new LoadColourStep(index);

        return AssignStep(baseImage, step);
    }

    private static ComputedImage AssignStep(IImageSource baseImage, IComputedStep step)
    {
        var computedImage = baseImage as ComputedImage ?? new ComputedImage(baseImage);
        computedImage.Steps.Add(step);
        return computedImage;
    }

    public static IImageSource ReplaceColor(this IImageSource baseImage, Color original, Color replacement)
    {
        var step = (new ReplaceColour(original, replacement));

        return AssignStep(baseImage, step);
    }

    public static IImageSource ReplaceWithPlayerColor(this IImageSource baseImage, Color toReplace,
        int playerColourIndex)
    {
        var step = (new ReplacePlayerColourStep(playerColourIndex, toReplace));

        return AssignStep(baseImage, step);
    }

    public static IImageSource ReplaceColor(this IImageSource baseImage, int index, Color replacement)
    {
        var step = (new ReplaceColour(index, replacement));
        return AssignStep(baseImage, step);
    }

    public static IImageSource Copy(this IImageSource baseImage, string? name = null)
    {
        // We create a new computed image so we seperate the list of steps
        var image = new ComputedImage(baseImage, name);
        image.Steps.Add(new CopyStep());
        return image;
    }
    
    
    public static IImageSource DrawRectangle(this IImageSource baseImage, int x, int y, int width, int height,
        Color color)
    {
        var step = new DrawRectangleStep(x, y, width, height, color);
        return AssignStep(baseImage, step);
    }

    public static WidthSource Width(this IImageSource baseImage)
    {
        return new WidthSource(baseImage);
    }
    
    public static HeightSource Height(this IImageSource baseImage)
    {
        return new HeightSource(baseImage);
    }
}