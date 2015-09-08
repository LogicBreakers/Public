using API.LbCore;
using API.LbWrappers;

namespace MasterHunter
{
    public class PlayHandler
    {
        public PlayHandler(){}

        public void ExecutePlays()
        {
            if (Game.Player.HasCoin)
            {
                HandleCoin();
            }
            var bestPlay = BestPlay();
            if (bestPlay != null)
            {
                bestPlay.PlayCard();
            }
        }

        private void HandleCoin()
        {
            foreach (var card in Game.Player.Hand)
            {
                if (card.RealTimeCost == (Game.Player.CurrentMana + 1) && CanPlay(card, true))
                {
                    Game.Player.PlayCoin();
                    card.PlayCard();
                }

            }
        }

        public bool HasPlays
        {
            get { return PlayableCards > 0; }
        }


        private int PlayableCards
        {
            get
            {
                var playable = 0;
                foreach (var card in Game.Player.Hand)
                {
                    if (HunterBrain.CanPlay(card))
                        playable++;
                }
                return playable;
            }
        }

        private LbCard BestPlay()
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
        private bool CanPlay(LbCard card, bool ignorePlay = false)
        {
            if (!card.CanPlay && !ignorePlay) return false;
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
                default:
                    return true;
            }
        }
    }
}
