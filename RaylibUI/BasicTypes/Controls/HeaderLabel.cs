using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.Controls;

public class HeaderLabel : LabelControl
{

    public HeaderLabel(IControlLayout controller, string title) : base(controller, title, eventTransparent: false,alignment: TextAlignment.Center, font: Fonts.BoldFont, fontSize: 26, spacing: 0.0f, colorFront: new Color(135, 135, 135, 255), colorShadow: Color.BLACK, shadowOffset: new Vector2(1, 1))
    {
    }

    public override void OnMouseMove(Vector2 moveAmount)
    {
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
        {
            Controller.Move(moveAmount);
        }
    }
}