using System.Collections.Generic;
using API.LbCore;
using API.LbWrappers;

namespace MasterHunter
{
    public static class AttackHandler
    {
        private static List<LbCard> enemyCards { get { return Game.Enemy.Field; } }
        private static List<LbCard> allyCards { get { return Game.Player.Field; } }

        public static void Execute()
        {
            if (Game.Player.HasWeapon)
            {
                HandleLocalWeapon();
            }
            HandleLocalAttacks();
        }

        private static void HandleLocalAttacks()
        {
            if (allyCards.Count <= 0) return;
            if (Game.Enemy.HasTauntMinion)
            {
                var bestTauntMinion = Game.Enemy.LowestTauntMinion;
                if (bestTauntMinion != null)
                {
                    var killer = HunterBrain.BestKillerFor(bestTauntMinion);
                    if (killer != null)
                        killer.Attack(bestTauntMinion);
                    else
                        HunterBrain.NukeMinion(bestTauntMinion);
                }
                return;
            }
            Game.Player.NukeEnemy();
        }

        private static void HandleLocalWeapon()
        {
            var weapon = Game.Player.Weapon;
            if (Game.Enemy.HasTauntMinion)
            {
                var bestTauntMinion = Game.Enemy.LowestTauntMinion;
                if (bestTauntMinion != null && bestTauntMinion.RemainingHealth <= weapon.AttackDamage)
                {
                    if (Game.Player.Health >= (Game.Enemy.Health + 2))
                    {
                        Game.Player.HeroCard.Attack(bestTauntMinion);
                    }
                }
            }
            else
            {
                if (Game.Player.FieldZoneValue() >= Game.Enemy.FieldZoneValue())
                {
                    Game.Player.HeroCard.Attack(Game.Enemy);
                }
                else
                {
                    var bestMinion = Game.Enemy.HighestValuedMinionKilledByDamage(Game.Player.HeroCard.AttackDamage);
                    if (bestMinion != null)
                    {
                        Game.Player.HeroCard.Attack(bestMinion);
                    }
                    else
                    {
                        Game.Player.HeroCard.Attack(Game.Enemy);
                    }
                }
            }
        }
    }
}
