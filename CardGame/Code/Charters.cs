using System;


namespace CardGame.Code
{
    public class Charters
    {
        public class Character
        {
            public int Accuracy { get;  set; }
            public int Evasion { get;  set; }
            public int Attack { get;  set; }
            public int Health { get;  set; }
            public int MaxHealth { get;  set; }

            public Character(int accuracy, int evasion, int attack, int health, int maxHealth)
            {
                Accuracy = accuracy;
                Evasion = evasion;
                Attack = attack;
                Health = health;
                MaxHealth = maxHealth;
            }

            public void Turn(Character opponent, Random random)
            {
                if (Accuracy > opponent.Evasion)
                {
                    if (random.Next(0, 3) == 2)
                    {
                        opponent.Health -= Attack * 2;
                    }
                    else
                    {
                        opponent.Health -= Attack;
                    }
                }
                else if (Accuracy == opponent.Evasion)
                {
                    opponent.Health -= Attack;
                }
                else
                {
                    if (random.Next(0, 3) != 0)
                    {
                        opponent.Health -= Attack;
                    }

                    // Иначе урон не наносится

                }
            }
            public void IncreaseStats(Cards card)
            {
                Attack += card.Attack;
                Health += card.Health;
                MaxHealth += card.Health;
                Accuracy += card.Accuracy;
                Evasion += card.Evasion;
                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }
            }
        }
    }
}
