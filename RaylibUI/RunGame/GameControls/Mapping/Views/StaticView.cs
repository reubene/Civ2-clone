using Civ2engine.MapObjects;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class StaticView : BaseGameView
{
    public StaticView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, gameScreen.Game.ActivePlayer.ActiveTile,
        currentView, viewHeight, viewWidth, true, 2000, Array.Empty<Tile>(), forceRedraw)
    {
        SetAnimation(Array.Empty<TextureElement>());
    }
}