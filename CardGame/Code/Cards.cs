namespace CardGame.Code
{
    public class Cards
    {
        public int Attack { get; private set; }
        public int Health { get; private set; }
        public int Accuracy { get; private set; }
        public int Evasion { get; private set; }

        public Cards(int attack, int health, int accuracy, int evasion)
        {
            Attack = attack;
            Health = health;
            Accuracy = accuracy;
            Evasion = evasion;
        }

        public static Cards[] GenerateCards()
        {
            return new Cards[]
            {
                new(attack: 5, health: 1, accuracy: -2, evasion: -5),
                new(attack: 2, health: 5, accuracy: 3, evasion: -2),
                new(attack: 3, health: 15, accuracy: 1, evasion: 2),
                new(attack: 9, health: 0, accuracy: 5, evasion: -3),
                new(attack: 2, health: -20, accuracy: -1, evasion: 4),
                new(attack: 6, health: 5, accuracy: 2, evasion: 0),
                new(attack: 4, health: -12, accuracy: 0, evasion: 3),
                new(attack: 6, health: 0, accuracy: 4, evasion: -1),
                new(attack: 2, health: 3, accuracy: 2, evasion: 1),
                new(attack: 3, health: -18, accuracy: -3, evasion: 5),
                new(attack: 1, health: 7, accuracy: 1, evasion: 1),
                new(attack: 2, health: 10, accuracy: 0, evasion: 0),
                new(attack: 2, health: -5, accuracy: 5, evasion: -5),
                new(attack: 0, health: 0, accuracy: 15, evasion: 15),
                new(attack: 3, health: 6, accuracy: 3, evasion: -1),
                new(attack: 5, health: 4, accuracy: 4, evasion: 0),
                new(attack: 1, health: 2, accuracy: 1, evasion: -3),
                new(attack: 4, health: 14, accuracy: -1, evasion: 3),
                new(attack: 5, health: 15, accuracy: 2, evasion: -2),
                new(attack: 0, health: 10, accuracy: 5, evasion: -5),
                new(attack: 0, health: 8, accuracy: 3, evasion: 1),
                new(attack: -2, health: 6, accuracy: 4, evasion: -2),
                new(attack: -6, health: 12, accuracy: 2, evasion: 3),
                new(attack: 20, health: 20, accuracy: -9, evasion: -20),
                new(attack: 5, health: -20, accuracy: 10, evasion: -1),
                new(attack: 7, health: 15, accuracy: -1, evasion: 2),
                new(attack: 9, health: -5, accuracy: 3, evasion: 3),
                new(attack: -1, health: 18, accuracy: 2, evasion: 5),
                new(attack: -9, health: 12, accuracy: 1, evasion: 0),
                new(attack: -8, health: 14, accuracy: -2, evasion: 4)
                //Здесь можно добавить карты
            };
        }

    }
}