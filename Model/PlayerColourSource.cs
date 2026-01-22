using Model.Images;

namespace Model;

public class PlayerColourSource
{
    public IImageSource Image { get; set; }
    
    public IList<ColourIndex> Colours { get; set; }
    public int? Alpha { get; set; }
}

public class ColourIndex(IImageSource imageSource, int row = 0, int column = 0, int? div = null)
{
    public IImageSource ImageSource { get; } = imageSource;
    public int Row { get; } = row;
    public int Column { get; } = column;
    
    public int? Div { get; } = div;
}