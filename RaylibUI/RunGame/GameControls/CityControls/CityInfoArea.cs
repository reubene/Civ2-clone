using System.Numerics;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping;
using Model;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityInfoArea : BaseControl
{
    private readonly CityWindow _cityWindow;
    private CityDisplayMode _mode;
    private IUserInterface _active; 

    public CityInfoArea(CityWindow controller, Rectangle infoPanel) : base(controller, false)
    {
        AbsolutePosition = infoPanel;
        _cityWindow = controller;
        _mode = CityDisplayMode.Info;
        _active = controller.MainWindow.ActiveInterface;
    }

    public override void OnResize()
    {
        base.OnResize();
        SetActiveMode(_mode);
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawRectangleLines((int)Bounds.x, (int)Bounds.y, Width,Height,Color.MAGENTA);
        if (Children != null)
        {
            foreach (var control in Children)
            {
                control.Draw(pulse);
            }
        }
        else
        {
            Raylib.DrawTextEx(_active.Look.DefaultFont,  Mode.ToString(), Location, 20,1,Color.MAGENTA );
        }
    }

    public CityDisplayMode Mode => _mode;
    

    public void SetActiveMode(CityDisplayMode mode)
    {
        _mode = mode;
        var activeInterface = _cityWindow.CurrentGameScreen.Main.ActiveInterface;
        var activPos = new Vector2(Bounds.x, Bounds.y);
        var children = new List<IControl>();
        if (mode == CityDisplayMode.Info)
        {
            foreach (var unit in _cityWindow.City.UnitsInCity)
            {
                var unitDisplay = new UnitDisplay(_cityWindow, unit, activPos, activeInterface);
                activPos = activPos with { X = activPos.X + unitDisplay.Width };
                if (activPos.X + unitDisplay.Width > Bounds.x + Bounds.width)
                {
                    activPos = new Vector2(Bounds.x, activPos.Y + unitDisplay.Height);
                }

                children.Add(unitDisplay);
            }
        }

        Children = children;
    }
}