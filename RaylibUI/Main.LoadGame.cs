
using Civ2engine.IO;
using Civ2engine;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Core.Units;
using RaylibUI.RunGame;
using RaylibUtils;

namespace RaylibUI
{
    public partial class Main
    {
        private Unit _activeUnit;

        
        public void StartGame(IGame game, IDictionary<string, string?>? viewData)
        {
            BuildPlayerColours();
            game.UpdatePlayerViewData();
            
            _activeScreen = new GameScreen(this, game, Soundman, viewData, PlayerColours);

            if (game.TurnNumber == 0)
            {
                game.StartNextTurn();
            }
            else
            {
                // If we're not on turn one start with active player
                game.StartPlayerTurn(game.ActivePlayer);
            }
        }

        public IUserInterface SetActiveRulesetFromFile(string root, string subDirectory,
            Dictionary<string, string> extendedMetadata)
        {
            var maxScore = -1;
            var selected = AllRuleSets.First();
            foreach (var set in AllRuleSets)
            {
                var score = extendedMetadata
                    .Where(thing => set.Metadata.ContainsKey(thing.Key) && set.Metadata[thing.Key] == thing.Value)
                    .Sum(thing => 1000);

                if (set.Paths.Contains(subDirectory))
                {
                    score += 100;
                }

                if (set.Root == root)
                {
                    score += 10;
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    selected = set;
                }
            }

            if (!selected.Paths.Contains(subDirectory))
            {
                selected = new Ruleset(selected, subDirectory);
                AllRuleSets = AllRuleSets.Append(selected).ToArray();
            }

            ActiveRuleSet = selected;
            if (selected.InterfaceIndex != Interfaces.IndexOf(ActiveInterface))
            {
                ActiveInterface = Interfaces[selected.InterfaceIndex];
            }

            TextureCache.Clear();
            ImageUtils.SetLook(ActiveInterface.Look, selected.Paths);
            
           BuildPlayerColours();
            
            return ActiveInterface;
        }

        public Ruleset[] AllRuleSets { get; set; }
        public Ruleset? ActiveRuleSet { get; private set; }

        public IUserInterface SetActiveRuleSet(int ruleSetIndex)
        {
            if (ruleSetIndex < 0 || ruleSetIndex >= AllRuleSets.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(ruleSetIndex));
            }
            if (ActiveRuleSet == AllRuleSets[ruleSetIndex])
            {
                // Do nothing we're already good
                return ActiveInterface;
            }
            
            ActiveRuleSet = AllRuleSets[ruleSetIndex];
            if (ActiveRuleSet.InterfaceIndex != Interfaces.IndexOf(ActiveInterface))
            {
                ActiveInterface = Interfaces[ActiveRuleSet.InterfaceIndex];
            }
            BuildPlayerColours();
        
            return ActiveInterface;
        }

        private void BuildPlayerColours()
        {
            PlayerColours = ActiveInterface.GetPlayerColours().Select((  pcs) =>
            {
                return new PlayerColour { Image = pcs.Image, 
                    Colours =  pcs.Colours.Select(ci => Images.ExtractBitmapColor(ci.ImageSource,ci.Column, ci.Row, ci.Div, pcs.Alpha)).ToList()};
            }).ToArray();
        }
    }
}
