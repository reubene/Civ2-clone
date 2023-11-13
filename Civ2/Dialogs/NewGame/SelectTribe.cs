using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectTribe : BaseDialogHandler
{
    public const string Title = "TRIBE";

    public SelectTribe() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        if (!res.Dialog.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Dialog.Button.Add(Labels.Cancel);
        }
        
        res.Dialog.OptionsCols = 3;
        return res;
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.Dialog.Options = Initialization.ConfigObject.Rules.Leaders.Select(l => l.Adjective).ToList();
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }
        else if (result.SelectedButton == Labels.Custom)
        {
            return civDialogHandlers[CustomTribe.Title].Show(civ2Interface);
        }

        // TODO: make civilizations

        return civDialogHandlers[SelectCityStyle.Title].Show(civ2Interface);
    }
}