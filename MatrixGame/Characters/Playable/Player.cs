using MatrixGame.InGameAreas;
using System;

namespace MatrixGame.Characters.Playable
{
    public class Player : Character
    {

        private int groundBashCount;
        private int decoyCount;
        public Player(int row, int col) : base(row, col, 70)
        {
            GroundBashCount = 2;
            DecoyCount = 1;
        }
        public int GroundBashCount { get => groundBashCount; set { groundBashCount = value; } }
        public int DecoyCount { get => decoyCount; set { decoyCount = value; } }

        public ValueTuple<int, int> GroundBash(int row, int col, Area area)
        {
            this.groundBashCount--;
            Console.Beep(200, 60);
            int eliminatedEnemies = 0;
            int destroyedTraps = 0;
            if (area.IndexIsValid(row + 1, col))
            {
                if (area.Matrix[row + 1, col] == '*')
                {
                    area.Matrix[row + 1, col] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row + 1, col] == 'T')
                {
                    area.Matrix[row + 1, col] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row + 1, col] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            if (area.IndexIsValid(row - 1, col))
            {
                if (area.Matrix[row - 1, col] == '*')
                {
                    area.Matrix[row - 1, col] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row - 1, col] == 'T')
                {
                    area.Matrix[row - 1, col] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row - 1, col] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            if (area.IndexIsValid(row, col + 1))
            {
                if (area.Matrix[row, col + 1] == '*')
                {
                    area.Matrix[row, col + 1] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row, col + 1] == 'T')
                {
                    area.Matrix[row, col + 1] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row, col + 1] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            if (area.IndexIsValid(row, col - 1))
            {
                if (area.Matrix[row, col - 1] == '*')
                {
                    area.Matrix[row, col - 1] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row, col - 1] == 'T')
                {
                    area.Matrix[row, col - 1] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row, col - 1] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            if (area.IndexIsValid(row + 1, col + 1))
            {
                if (area.Matrix[row + 1, col + 1] == '*')
                {
                    area.Matrix[row + 1, col + 1] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row + 1, col + 1] == 'T')
                {
                    area.Matrix[row + 1, col + 1] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row + 1, col + 1] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            if (area.IndexIsValid(row - 1, col + 1))
            {
                if (area.Matrix[row - 1, col + 1] == '*')
                {
                    area.Matrix[row - 1, col + 1] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row - 1, col + 1] == 'T')
                {
                    area.Matrix[row - 1, col + 1] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row - 1, col + 1] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            if (area.IndexIsValid(row + 1, col - 1))
            {
                if (area.Matrix[row + 1, col - 1] == '*')
                {
                    area.Matrix[row + 1, col - 1] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row + 1, col - 1] == 'T')
                {
                    area.Matrix[row + 1, col - 1] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row + 1, col - 1] == 'B')
                    (area as GameField).Boss.LoseHealth(30);

            }
            if (area.IndexIsValid(row - 1, col - 1))
            {
                if (area.Matrix[row - 1, col - 1] == '*')
                {
                    area.Matrix[row - 1, col - 1] = '-';
                    eliminatedEnemies++;
                }
                else if (area.Matrix[row - 1, col - 1] == 'T')
                {
                    area.Matrix[row - 1, col - 1] = '-';
                    destroyedTraps++;
                }
                else if (area.Matrix[row - 1, col - 1] == 'B')
                    (area as GameField).Boss.LoseHealth(30);
            }
            return (eliminatedEnemies, destroyedTraps);
        }
        public void IncreaseGroundBashCount()
        {
            GroundBashCount++;
        }
        public ValueTuple<string, int> GetDecoyCoordinates()
        {
            Console.WriteLine("Select direction");
            ConsoleKey pressedKey = Console.ReadKey(true).Key;
            string direction = string.Empty;
            int distance = 0;
            while (direction == string.Empty)
            {
                switch (pressedKey)
                {
                    case ConsoleKey.W:
                        direction = "up";
                        break;
                    case ConsoleKey.S:
                        direction = "down";
                        break;
                    case ConsoleKey.A:
                        direction = "left";
                        break;
                    case ConsoleKey.D:
                        direction = "right";
                        break;
                    default:
                        Console.WriteLine("Select a valid direction!");
                        pressedKey = Console.ReadKey(true).Key;
                        break;
                }
            }
            Console.Write("Input distance for deployment: ");
            while (distance <= 0)
            {
                distance = int.Parse(Console.ReadLine());
                if (distance <= 0)
                {
                    Console.Write("Input valid distance: ");
                }
            }
            return new ValueTuple<string, int>(direction, distance);
        }
    }
}
