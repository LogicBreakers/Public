using System.Collections.Generic;
using API.LbCore;
using API.LbEnums;
using API.LbWrappers;

namespace MasterHunter
{
    public class LethalHandler
    {
        public int TotalDamage;

        private readonly List<LbCard> _handLethals;
        private readonly List<LbCard> _fieldLethals;

        public LethalHandler()
        {
            _handLethals  = new List<LbCard>();
            _fieldLethals = new List<LbCard>();
        }

        public bool HasLethal
        {
            get { return TotalDamage >= Game.Enemy.Health; }
        }

        public void ExecuteLethals()
        {
            foreach (var card in _handLethals)
            {
                card.SetTarget(Game.Enemy.HeroCard);
                card.PlayCard();
            }
            if (!Game.Enemy.HasTauntMinion && Game.Player.HasWeapon && Game.Player.HeroCard.CanAttack())
            {
                TotalDamage += Game.Player.HeroCard.AttackDamage;
            }
            if (Game.Player.CanUseHeroPower && Game.Player.CurrentMana >= 2)
            {
                Game.Player.UseHeroPower();
            }
            foreach (var card in _fieldLethals)
            {
                card.Attack(Game.Enemy);
            }
        }

        public void CalculateLethal()
        {
            TotalDamage = 0;
            _handLethals.Clear();
            _fieldLethals.Clear();

            var availableMana = Game.Player.CurrentMana;
            foreach (var card in Game.Player.Hand)
            {
                if (card.RealTimeCost > availableMana) continue;
                switch (card.Id)
                {
                    case "BRM_013":
                        if (Game.Player.MinionsThatCanAttack > 0 && !Game.Enemy.HasTauntMinion)
                        {
                            TotalDamage += 2;
                            availableMana -= 1;
                            _handLethals.Add(card);
                        }
                        break;
                    case "AT_084":
                        if (Game.Player.MinionsThatCanAttack > 0 && !Game.Enemy.HasTauntMinion)
                        {
                            TotalDamage += 2;
                            availableMana -= 2;
                            _handLethals.Add(card);
                        }
                        break;
                    case "CS2_188":
                        TotalDamage += 3;
                        availableMana -= 2;
                        _handLethals.Add(card);
                        break;
                    case "EX1_539":
                        if (Game.Player.HasBeastInField)
                        {
                            TotalDamage += 5;
                            availableMana -= 3;
                            _handLethals.Add(card);
                        }
                        else
                        {
                            TotalDamage += 3;
                            availableMana -= 3;
                            _handLethals.Add(card);
                        }
                        break;
                }
            }
            if (Game.Player.CanUseHeroPower && availableMana >= 2)
            {
                TotalDamage += 2;
            }
            if (!Game.Enemy.HasTauntMinion)
            {
                if (Game.Player.HasWeapon && Game.Player.HeroCard.CanAttack())
                {
                    TotalDamage += Game.Player.HeroCard.AttackDamage;
                }
                foreach (var card in Game.Player.Field)
                {
                    if (card.CanAttack())
                    {
                        TotalDamage += card.AttackDamage;
                        _fieldLethals.Add(card);
                    }
                }
            }
            
        }
    }
}
