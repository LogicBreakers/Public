using API.LbCore;
using API.LbEnums;
using API.LbWrappers;

namespace BasicBot
{
    // Class necessary to be recognized by the client
    internal static class Entry
    {
        public static void Init()
        {
            // Adds the custom bot class the scene manager
            SceneMgr.Get().gameObject.AddComponent<Bot>();
        }
    }

    // Custom Bot Class
    internal class Bot : LbAssembly
    {
        // OnStart is called once when the class is initiated
        public override void OnStart()
        {
            // Writes to the application log that the bot was loaded
            API.Log.Write("Basic Bot Loaded!");
            // Auto Updater for customrules from our server
            var ruleUpdater = new LbRuleUpdater("BasicBot", true);
            // Runs the auto updater
            ruleUpdater.Run();
        }

        // OnMenu is called every tick while we're in the menu
        public override void OnMenu()
        {
            var currentMode = Menu.GetCurrentMode();
            switch (currentMode)
            {
                case ModeStatus.Login:
                    // Releases quest dialog if needed
                    Menu.ReleaseQuests();
                    break;
                case ModeStatus.MainMenu:
                    // Sets the mode selected in the client
                    Client.SetSelectedMode();
                    break;
                // If we're in adventure/play mode find game with default settings
                case ModeStatus.Adventure:
                case ModeStatus.Play:
                    Menu.FindGame();
                    break;
            }
        }

        // OnGameOver gets called once after the game has finished.
        public override void OnGameOver()
        {
            GameActions.EndGame();
            switch (Game.GetGameResult())
            {
                case GameResult.Win:
                    API.Log.Write("Nice Won The Game! " + Game.Player.Health + " To " +
                              Game.Enemy.Health);
                    Client.Score.AddWin();
                    break;
                case GameResult.Loss:
                     API.Log.Write("Ohh Got Rekked! " + Game.Player.Health + " To " +
                              Game.Enemy.Health);
                    Client.Score.AddLoss();
                    break;
                case GameResult.Draw:
                    API.Log.Write("DRAW? or FUCKED? " + Game.Player.Health + " To " +
                              Game.Enemy.Health);
                    break;
            }
        }

        // Mulligan gets called once during the mulligan phase
        public override void OnMulligan()
        {
            Brain.ReplaceCards();
        }

        // OnMyTurn gets called every tick during the local player's turn
        public override void OnMyTurn()
        {
            if (Game.Player.HasLethal() && !Game.Enemy.HasTauntMinion)
                Brain.NukeEnemy();
            else if (Game.Player.CurrentMana >= 2 && Brain.UseHeroPower())
            {
                Game.Player.UseHeroPower();
            }
            else if (Brain.BestCardToPlay() != null)
            {
                Brain.BestCardToPlay().PlayCard();
            }
            else if (Game.Player.MinionsThatCanAttack > 0)
            {
                ProcessAttacks();
            }
            GameActions.EndTurn();
        }

        private void ProcessAttacks()
        {
            // Lists of enemy cards in field
            var enemyCards = Game.Enemy.Field;
            // List of our local player cards in field
            var myCards = Game.Player.Field;
            if (myCards.Count <= 0) return;
            if (Brain.ShouldAttackMinions())
            {
                Brain.DoSmartTrades();
                foreach (var enemy in enemyCards)
                {
                    var bestKiller = Brain.BestKillerFor(enemy);
                    if (bestKiller != null)
                        bestKiller.Attack(enemy);
                }

                foreach (var card in myCards)
                {
                    if (!card.CanAttack())
                        continue;
                    if (Game.Player.HasTauntMinion)
                    {
                        var bestTaunt = Brain.GetLowestEnemyTaunt();
                        card.Attack(bestTaunt);
                        continue;
                    }
                    if (enemyCards.Count > 0)
                    {
                        foreach (var enemy in enemyCards)
                        {
                            card.Attack(enemy);
                            break;
                        }
                    }
                }
            }
            else
                Brain.NukeEnemy();
        }
    }
}
