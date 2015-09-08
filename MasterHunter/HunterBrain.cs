using API.LbCore;
using API.LbWrappers;

namespace MasterHunter
{
    internal static class HunterBrain
    {
        internal static LbCard BestPlay(bool coin)
        {
            var cardsInHand = Game.Player.Hand;
            LbCard best = null;
            foreach (var card in cardsInHand)
            {
                if (CanPlay(card) && (best == null || best.Value() < card.Value()))
                    best = card;
            }
            return best;
        }

        // Custom CanPlay for hunter
        internal static bool CanPlay(LbCard card)
        {
            if (!card.CanPlay) return false;
            switch (card.Id)
            {
                // Unleash Hounds
                case "EX1_538":
                    if (Game.Player.HasCardInField("NEW1_019") && Game.Enemy.MinionsInField >= 2)
                        return true;
                    return Game.Enemy.MinionsInField >= 3;
                // Lance Carrier
                case "AT_084":
                    return (Game.Player.MinionsInField > 0);
                // IronBeak Owl
                case "CS2_203":
                    return (Game.Enemy.MinionsInField > 0);
                // Abusive Sergeant
                case "CS2_188":
                    return (Game.Player.MinionsThatCanAttack > 0);
                // Eaglehorn Bow
                case "EX1_536":
                    return !Game.Player.HasWeapon;
                // The Coin
                case "GAME_005":
                    foreach (var handc in Game.Player.Hand)
                    {
                        if (handc.Cost == (Game.Player.CurrentMana + 1) && CanPlay(handc))
                        {
                            return true;
                        }
                    }
                    return false;

                default:
                    return true;
            }
        }


        internal static bool ShouldAttackMinions()
        {
            if (Game.Enemy.HasTauntMinion)
                return true;
            return Game.Player.FieldZoneValue() <= Game.Enemy.FieldZoneValue();
        }
        
        public static LbCard BestKillerFor(LbCard target)
        {
            LbCard best = null;
            foreach (var card in Game.Player.Field)
            {
                if (card.CanKill(target) && card.CanAttack() && (best == null || best.Value() > card.Value()))
                    best = card;
            }
            return best;
        }

        public static void NukeMinion(LbCard target)
        {
            foreach (var card in Game.Player.Field)
            {
                if (!card.CanAttack()) continue;
                card.Attack(target);
            }
        }
    }
}
