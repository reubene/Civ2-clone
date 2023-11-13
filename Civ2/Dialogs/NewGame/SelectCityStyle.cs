using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectCityStyle : BaseDialogHandler
{
    public const string Title = "CUSTOMCITY";

    public SelectCityStyle() : base(Title, -0.085, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        if (!res.Dialog.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Dialog.Button.Add(Labels.Cancel);
        }

        res.Dialog.Dialog.Options ??= Labels.Items[247..251];
        
        return res;
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.OptionsImages = activeInterface.CityImages.Sets.Take(4).Select(i => i.Skip(6).First().Image).ToArray();
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
    }
}