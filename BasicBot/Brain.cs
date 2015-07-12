using System.Collections.Generic;
using API.LbCore;
using API.LbEnums;
using API.LbWrappers;

namespace BasicBot
{
    internal static class Brain
    {
        internal static void DoSmartTrades()
        {
            foreach (var card in Game.Player.Field)
            {
                if (!card.CanAttack()) continue;
                foreach (var enemy in Game.Enemy.Field)
                {
                    if ((Game.Enemy.HasTauntMinion && !enemy.HasTaunt) || enemy.Value() >= card.Value() ||
                        enemy.RemainingHealth > card.AttackDamage) continue;
                    card.Attack(enemy);
                }
            }
        }

        internal static LbCard BestKillerFor(LbCard target)
        {
            if (Game.Enemy.HasTauntMinion && !target.HasTaunt)
                return null;
            LbCard best = null;
            var myField = Game.Player.Field;
            foreach (var card in myField)
            {
                if (!card.CanAttack() || !card.CanKill(target))
                    continue;
                if (best == null || (best.Value() > card.Value()) ||
                    (best.RemainingHealth <= target.AttackDamage && card.RemainingHealth > target.AttackDamage))
                    best = card;
            }
            return best;
        }

        internal static LbCard BestCardToPlay()
        {
            var cardsInHand = Game.Player.Hand;
            LbCard best = null;
            foreach (var card in cardsInHand)
            {
                if (CheckCard(card, best))
                    best = card;
            }
            return best;
        }

        private static List<LbCard> KillersFor(LbCard target)
        {
            var myField = Game.Player.Field;
            var killers = new List<LbCard>();
            foreach (var card in myField)
            {
                if (!card.CanKill(target)) continue;
                killers.Add(card);
            }
            return killers;
        }

        internal static bool ShouldAttackMinions()
        {
            if (Game.Enemy.HasTauntMinion)
                return true;
            return Game.Player.FieldZoneValue() <= Game.Enemy.FieldZoneValue();
        }

        internal static bool UseHeroPower()
        {
            if (!Game.Player.CanUseHeroPower) return false;
            switch (Game.Player.HeroClass)
            {
                case HeroClass.Hunter:
                case HeroClass.Warrior:
                    return Game.Player.PlayableMinions() == 0;
                case HeroClass.Warlock:
                    return Game.Player.PlayableMinions() == 0 &&
                           (Game.Player.Health > 22 || (Game.Player.Health > (Game.Enemy.Health + 3)));
                default:
                    return false;
            }
        }

        internal static void ReplaceCards()
        {
            foreach (var card in Game.Player.Hand)
            {
                if (card.Cost <= 3) continue;
                API.Log.Write("[REPLACING] => " + card);
                card.ToggleReplace();
            }
            GameActions.ConfirmMulligan();
        }

        internal static void NukeEnemy()
        {
            foreach (var card in Game.Player.Field)
            {
                if (!card.CanAttack()) continue;
                card.Attack(Game.Enemy);
            }
        }

        internal static LbCard GetLowestEnemyTaunt()
        {
            LbCard lowestTaunt = null;
            foreach (var card in Game.Enemy.Field)
            {
                if (!card.HasTaunt)
                    continue;
                if (lowestTaunt == null || (card.RemainingHealth < lowestTaunt.RemainingHealth))
                    lowestTaunt = card;
                else if (lowestTaunt.RemainingHealth == card.RemainingHealth &&
                         lowestTaunt.AttackDamage < card.AttackDamage)
                    lowestTaunt = card;
            }
            return lowestTaunt;
        }

        private static bool CheckCard(LbCard card, LbCard best)
        {
            if (card.Rule != null)
                return card.CanPlay && card.Rule.Met && (best == null || best.Value() < card.Value());
            API.Utility.DumpCustomRule(card);
            return false;
        }
    }
}
