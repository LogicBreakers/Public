using API;
using API.LbCore;
using API.LbWrappers;

namespace MasterHunter
{
    internal class HunterBot : LbAssembly
    {
        // Version number
        private const double Ver = 0.1;
        private static LbPlayer MyPlayer { get { return Game.Player; }}
        private LethalHandler _lethalManager;
        private PlayHandler _playHandler;

        public override void OnStart()
        {
            Log.Write("Hunter Master Version: " + Ver + " Loaded");
            var ruleUpdate = new LbRuleUpdater("MasterHunter", true);
            ruleUpdate.Run();

            _lethalManager = new LethalHandler();
            _playHandler = new PlayHandler();
        }

        public override void OnMenu()
        {
            // Automatically handles the menu (starts game with selected deck/ mode)
            Menu.AuntoHandle();
        }

        public override void OnMyTurn()
        {
            _lethalManager.CalculateLethal();
            if (_lethalManager.HasLethal)
            {
                _lethalManager.ExecuteLethals();
            }
            else if (_playHandler.HasPlays)
            {
               _playHandler.ExecutePlays();
            }
            else if(Game.Player.MinionsThatCanAttack > 0)
            {
                AttackHandler.Execute();
            }
            else
            {
                EndTurn();
            }
        }

        private void EndTurn()
        {
            // Always uses my passive before ending turn if enough mana
            if (MyPlayer.CurrentMana >= 2)
                MyPlayer.UseHeroPower();
            GameActions.EndTurn();
        }

        public override void OnGameOver()
        {
            // Automatically handles the endgame (clicks continue, etc)
            Game.HandleGameOver();
        }
    }
}
