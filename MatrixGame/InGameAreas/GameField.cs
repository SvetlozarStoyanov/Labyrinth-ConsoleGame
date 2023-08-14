using MatrixGame.Characters.NPC;
using MatrixGame.Characters.Playable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatrixGame.InGameAreas
{
    public class GameField : Area
    {

        private int trapCount;
        private int groundBashPickupCount;
        private Boss boss;
        private Cave cave;
        private Monster monster;
        private string matrixFilePath;
        private bool bossIsInCave;
        private int stalemateCounterRows;
        private int stalemateCounterCols;
        //Reason the monster spawned, can either be "Rows" or "Cols"
        private string stalemateCause;

        public GameField(string path)
        {
            matrixFilePath = path;
            ReadMatrixFromFile(path);
        }
        public GameField(int size)
        {
            IsRandomGenerated = true;
            GenerateMatrix(size);
        }

        public int TrapCount
        {
            get { return trapCount; }
            protected set { trapCount = value; }
        }
        public Boss Boss { get => boss; private set { boss = value; } }
        public Cave Cave => cave;

        public int GroundBashPickupCount { get => groundBashPickupCount; private set => groundBashPickupCount = value; }
        public Monster Monster { get => monster; set => monster = value; }

        private void ReadMatrixFromFile(string path)
        {
            string[] matrixLineByLine = File.ReadAllLines(path);
            Matrix = new char[matrixLineByLine.Length, matrixLineByLine.Length];
            Rows = Cols = matrixLineByLine.Length;
            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                char[] rowInput = matrixLineByLine[row].Split(' ').Select(char.Parse).ToArray();
                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    Matrix[row, col] = rowInput[col];
                    if (Matrix[row, col] == '*')
                        EnemyCount++;
                    else if (Matrix[row, col] == '+')
                        HealthPackCount++;
                    else if (Matrix[row, col] == 'T')
                        HealthPackCount++;
                    else if (Matrix[row, col] == 'P')
                        Player = new Player(row, col);
                    else if (Matrix[row, col] == 'B')
                        boss = new Boss(row, col);
                    else if (Matrix[row, col] == 'G')
                        GroundBashPickupCount++;
                }
            }
        }
        public void PlayerAction(ConsoleKey pressedKey)
        {
            Matrix[Player.Row, Player.Col] = '-';
            bool actionIsValid = false;
            switch (pressedKey)
            {
                case ConsoleKey.W:
                    if (IndexIsValid(Player.Row - 1, Player.Col))
                    {
                        actionIsValid = true;
                        Player.Row--;
                    }
                    break;
                case ConsoleKey.S:
                    if (IndexIsValid(Player.Row + 1, Player.Col))
                    {
                        actionIsValid = true;
                        Player.Row++;
                    }
                    break;
                case ConsoleKey.A:
                    if (IndexIsValid(Player.Row, Player.Col - 1))
                    {
                        actionIsValid = true;
                        Player.Col--;
                    }
                    break;
                case ConsoleKey.D:
                    if (IndexIsValid(Player.Row, Player.Col + 1))
                    {
                        actionIsValid = true;
                        Player.Col++;
                    }
                    break;
                case ConsoleKey.G:
                    if (Player.GroundBashCount < 1)
                    {
                        Console.WriteLine($"No ground bashes left!");
                        pressedKey = Console.ReadKey(true).Key;
                        PlayerAction(pressedKey);
                    }
                    else
                    {
                        ValueTuple<int, int> groundBashEliminatedEnemiesAndDestroyedTraps = Player.GroundBash(Player.Row, Player.Col, this);
                        EnemyCount -= groundBashEliminatedEnemiesAndDestroyedTraps.Item1;
                        TrapCount -= groundBashEliminatedEnemiesAndDestroyedTraps.Item2;
                        actionIsValid = true;
                    }
                    break;
                case ConsoleKey.Escape:
                    break;
                default:
                    Console.WriteLine("Press a valid button!");
                    pressedKey = Console.ReadKey(true).Key;
                    PlayerAction(pressedKey);
                    break;
            }
            if (actionIsValid)
                CheckPlayerPosition();
            if (Boss.IsDead)
                Matrix[Boss.Row, Boss.Col] = '-';
            if (!Player.IsDead)
                Matrix[Player.Row, Player.Col] = 'P';
            else
                Matrix[Player.Row, Player.Col] = 'X';
        }
        public void BossAction()
        {

            if (Monster != null)
            {
                if (Player.Health >= Boss.Health || ShouldBreakStalemate(stalemateCause))
                    DespawnMonster();
                else
                    MoveMonster();
            }
            ValueTuple<int, int> placedTrap = (-1, -1);
            if (boss.Health < Player.Health && boss.MeasureDistanceToCharacter(Player) <= 10 && boss.TrapCount > 0 && !bossIsInCave)
            {
                placedTrap = boss.PlaceTrap(this);
                TrapCount++;
            }
            if (bossIsInCave)
            {
                Matrix[boss.Row, boss.Col] = 'C';
                bossIsInCave = false;
            }
            else
                Matrix[boss.Row, boss.Col] = '-';
            if (placedTrap != (-1, -1))
                Matrix[placedTrap.Item1, placedTrap.Item2] = 'T';
            boss.FindBestDirection(this);
            CheckBossPosition();
            if (Boss.Health > Player.Health && Monster == null)
                DetermineStalemate();
        }
        private void MoveMonster()
        {
            if (Monster.LastEncounteredElement != 'T')
                Matrix[Monster.Row, Monster.Col] = Monster.LastEncounteredElement;
            else
                Matrix[Monster.Row, Monster.Col] = '-';
            switch (monster.Direction)
            {
                case "up":
                    if (IndexIsValid(monster.Row - 1, monster.Col))
                        monster.Row--;
                    break;
                case "down":
                    if (IndexIsValid(monster.Row + 1, monster.Col))
                        monster.Row++;
                    break;
                case "left":
                    if (IndexIsValid(monster.Row, monster.Col - 1))
                        monster.Col--;
                    break;
                case "right":
                    if (IndexIsValid(monster.Row, monster.Col + 1))
                        monster.Col++;
                    break;
            }
            switch (Monster.LastEncounteredElement)
            {
                case '*':
                    EnemyCount++;
                    break;
                case '+':
                    HealthPackCount++;
                    break;
                case 'G':
                    GroundBashPickupCount++;
                    break;
            }
            CheckMonsterPosition();
        }
        private void CheckPlayerPosition()
        {
            if (Matrix[Player.Row, Player.Col] == '*')
            {
                Player.LoseHealth(5);
                EnemyCount--;
            }
            else if (Matrix[Player.Row, Player.Col] == '+')
            {
                Player.GainHealth(10);
                HealthPackCount--;
            }
            else if (Matrix[Player.Row, Player.Col] == 'T')
            {
                Player.LoseHealth(40);
                trapCount--;
                SurroundWithEnemies(Player.Row, Player.Col);
            }
            else if (Matrix[Player.Row, Player.Col] == 'G')
            {
                GroundBashPickupCount--;
                Player.IncreaseGroundBashCount();
            }
            else if (Matrix[Player.Row, Player.Col] == 'B')
                Player.Fight(boss);
            else if (Matrix[Player.Row, Player.Col] == 'M')
                Player.Fight(Monster);
            else if (Matrix[Player.Row, Player.Col] == 'C')
            {
                if (!IsRandomGenerated)
                    cave = new Cave(matrixFilePath.Substring(0, matrixFilePath.Length - 4) + "_cave.txt", Player);
                else
                {
                    cave.GenerateMatrix(10);
                }
                Player = cave.Player;
            }
        }
        private void CheckBossPosition()
        {
            if (Matrix[boss.Row, boss.Col] == '*')
            {
                boss.EatEnemy();
                EnemyCount--;
            }
            else if (Matrix[boss.Row, boss.Col] == '+')
                HealthPackCount--;
            else if (Matrix[boss.Row, boss.Col] == 'G')
                GroundBashPickupCount--;
            else if (Matrix[boss.Row, boss.Col] == 'T')
                TrapCount -= boss.PickUpTrap(this);
            else if (Matrix[boss.Row, boss.Col] == 'P')
                boss.Fight(Player);
            else if (Matrix[boss.Row, boss.Col] == 'C')
                bossIsInCave = true;
            if (!boss.IsDead && Matrix[boss.Row, boss.Col] != 'C')
                Matrix[boss.Row, boss.Col] = 'B';
            if (Player.IsDead)
                Matrix[Player.Row, Player.Col] = 'X';
        }
        public void CheckMonsterPosition()
        {
            Monster.LastEncounteredElement = Matrix[Monster.Row, Monster.Col];
            switch (Matrix[Monster.Row, Monster.Col])
            {
                case '*':
                    EnemyCount--;
                    break;
                case '+':
                    HealthPackCount--;
                    break;
                case 'G':
                    GroundBashPickupCount--;
                    break;
                case 'T':
                    TrapCount--;
                    SurroundWithEnemies(Monster.Row, Monster.Col);
                    break;
                case 'P':
                    Monster.Fight(Player);
                    break;
            }
            Matrix[Monster.Row, Monster.Col] = 'M';
        }
        private void GenerateMatrix(int size)
        {
            List<char> elementsOfTheGame = new List<char> { '-', '+', '*', 'P', 'T' };
            Matrix = new char[size, size];
            if (size >= 15)
                elementsOfTheGame.Add('G');
            if (size >= 10)
                elementsOfTheGame.Add('C');
            Random random = new Random();
            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                if (Boss == null && row == size - random.Next(2, 4))
                    elementsOfTheGame.Add('B');

                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    int randomIndex = random.Next(0, elementsOfTheGame.Count);

                    if (elementsOfTheGame[randomIndex] == 'P' && CheckVicinity(row, col, 'B') /*&& CheckVicinity(row, col, '+')*/)
                    {
                        Player = new Player(row, col);
                    }
                    else if (elementsOfTheGame[randomIndex] == '*' && GetRow(row).Count(x => x == '*') < 6)
                    {
                        EnemyCount++;
                        //if (EnemyCount == 80)
                        //    elementsOfTheGame.Remove('*');
                    }
                    else if (elementsOfTheGame[randomIndex] == 'G' && GetRow(row).Count(x => x == 'G') == 0 && GetCol(col).Count(x => x == 'G') == 0 && IndexIsValid(row - 1, col - 1) && GetRow(row - 1).Count(x => x == 'G') == 0 && GetCol(col - 1).Count(x => x == 'G') == 0 && IndexIsValid(row - 2, col - 2) && GetRow(row - 2).Count(x => x == 'G') == 0 && GetCol(col - 2).Count(x => x == 'G') == 0 && IndexIsValid(row - 3, col - 3) && GetRow(row - 3).Count(x => x == 'G') == 0 && GetCol(col - 3).Count(x => x == 'G') == 0)
                    {
                        GroundBashPickupCount++;
                    }
                    else if (elementsOfTheGame[randomIndex] == 'T' && CheckVicinity(row, col, 'T') && GetRow(row).Count(x => x == 'T') < 5 && GetCol(col).Count(x => x == 'T') < 5/*&& CheckVicinity(row, col, '+')*/)
                    {
                        TrapCount++;
                        //if (TrapCount == 35)
                        //    elementsOfTheGame.Remove('T');
                    }
                    else if (elementsOfTheGame[randomIndex] == '+' && CheckVicinity(row, col, '+') && GetRow(row).Count(x => x == '+') < 2)
                    {
                        HealthPackCount++;
                        //if (HealthPackCount == 25)
                        //    elementsOfTheGame.Remove('+');
                    }
                    else if (elementsOfTheGame[randomIndex] == 'B' && CheckVicinity(row, col, 'P'))
                    {
                        Boss = new Boss(row, col);
                    }
                    else if (elementsOfTheGame[randomIndex] == 'C' && Player != null && DistanceToPlayer(row, col) > 5)
                    {
                        if (File.Exists($"../../../Levels/level_0_cave.txt"))
                            File.Delete($"../../../Levels/level_0_cave.txt");

                        cave = new Cave(random.Next(10, 20));
                    }
                    else
                    {
                        Matrix[row, col] = '-';
                        continue;
                    }
                    Matrix[row, col] = elementsOfTheGame[randomIndex];
                    if (Boss != null)
                        elementsOfTheGame.Remove('B');
                    if (Player != null)
                        elementsOfTheGame.Remove('P');
                    if (cave != null)
                        elementsOfTheGame.Remove('C');
                }
            }
            if (Boss == null)
            {
                if (Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1] == '*')
                    EnemyCount--;
                if (Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1] == '+')
                    HealthPackCount--;
                if (Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1] == 'T')
                    TrapCount--;
                Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1] = 'B';
                Boss = new Boss(Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1);
            }
        }
        public bool DetermineStalemate()
        {
            if (Math.Abs(Boss.Row - Player.Row) == 1 && Math.Abs(Boss.Col - Player.Col) != 1)
                stalemateCounterRows++;
            else
                stalemateCounterRows = 0;
            if (Math.Abs(Boss.Row - Player.Row) != 1 && Math.Abs(Boss.Col - Player.Col) == 1)
                stalemateCounterCols++;
            else
                stalemateCounterCols = 0;
            if (stalemateCounterRows == 3)
            {
                SpawnMonster("Horizontal");
                stalemateCause = "Cols";
                return true;
            }
            else if (stalemateCounterCols == 3)
            {
                SpawnMonster("Vertical");
                stalemateCause = "Rows";
                return true;
            }
            return false;
        }
        private bool ShouldBreakStalemate(string stalemateCause)
        {
            switch (stalemateCause)
            {
                case "Rows":
                    if (Math.Abs(Boss.Row - Player.Row) != 1)
                    {
                        stalemateCounterCols = 0;
                        return true;
                    }
                    break;
                case "Cols":
                    if (Math.Abs(Boss.Col - Player.Col) != 1)
                    {
                        stalemateCounterRows = 0;
                        return true;
                    }
                    break;
            }
            return false;
        }
        private void SpawnMonster(string monsterWayOfMoving)
        {
            string direction = string.Empty;
            int monsterRow = 0;
            int monsterCol = 0;
            switch (monsterWayOfMoving)
            {
                case "Horizontal":
                    if (Matrix.GetLength(1) - 1 - Player.Col <= 0 + Player.Col)
                    {
                        monsterCol = Matrix.GetLength(1) - 1;
                        direction = "left";
                    }
                    else
                        direction = "right";
                    monsterRow = Player.Row;
                    break;
                case "Vertical":
                    if (Matrix.GetLength(0) - 1 - Player.Row <= 0 + Player.Row)
                    {
                        monsterRow = Matrix.GetLength(0) - 1;
                        direction = "up";
                    }
                    else
                        direction = "down";
                    monsterCol = Player.Col;
                    break;
            }
            Monster = new Monster(monsterRow, monsterCol, direction);
            //Monster.LastEncounteredElement = Matrix[monsterRow, monsterCol];
            //Matrix[Monster.Row, Monster.Col] = 'M';
            CheckMonsterPosition();

        }
        private void DespawnMonster()
        {
            Matrix[Monster.Row, Monster.Col] = Monster.LastEncounteredElement;
            Monster = null;
        }

        public void SurroundWithEnemies(int row, int col)
        {
            if (IndexIsValid(row + 1, col))
            {
                if (Matrix[row + 1, col] == '-')
                {
                    Matrix[row + 1, col] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row + 1, col] == '+')
                {
                    HealthPackCount--;
                    Matrix[row + 1, col] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row + 1, col] == 'T')
                {
                    Matrix[row + 1, col] = '*';
                    SurroundWithEnemies(row + 1, col);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row + 1, col] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row + 1, col] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row - 1, col))
            {
                if (Matrix[row - 1, col] == '-')
                {
                    Matrix[row - 1, col] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row - 1, col] == '+')
                {
                    HealthPackCount--;
                    Matrix[row - 1, col] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row - 1, col] == 'T')
                {
                    Matrix[row - 1, col] = '*';
                    SurroundWithEnemies(row - 1, col);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row - 1, col] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row - 1, col] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row, col + 1))
            {
                if (Matrix[row, col + 1] == '-')
                {
                    Matrix[row, col + 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row, col + 1] == '+')
                {
                    HealthPackCount--;
                    Matrix[row, col + 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row, col + 1] == 'T')
                {
                    Matrix[row , col + 1] = '*';
                    SurroundWithEnemies(row, col + 1);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row, col + 1] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row, col + 1] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row, col - 1))
            {
                if (Matrix[row, col - 1] == '-')
                {
                    Matrix[row, col - 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row, col - 1] == '+')
                {
                    HealthPackCount--;
                    Matrix[row, col - 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row, col - 1] == 'T')
                {
                    Matrix[row, col - 1] = '*';
                    SurroundWithEnemies(row, col - 1);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row, col - 1] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row, col - 1] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row + 1, col + 1))
            {
                if (Matrix[row + 1, col + 1] == '-')
                {
                    Matrix[row + 1, col + 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row + 1, col + 1] == '+')
                {
                    HealthPackCount--;
                    Matrix[row + 1, col + 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row + 1, col + 1] == 'T')
                {
                    Matrix[row + 1, col + 1] = '*';
                    SurroundWithEnemies(row + 1, col + 1);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row + 1, col + 1] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row + 1, col + 1] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row - 1, col + 1))
            {
                if (Matrix[row - 1, col + 1] == '-')
                {
                    Matrix[row - 1, col + 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row - 1, col + 1] == '+')
                {
                    HealthPackCount--;
                    Matrix[row - 1, col + 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row - 1, col + 1] == 'T')
                {
                    Matrix[row - 1, col + 1] = '*';
                    SurroundWithEnemies(row - 1, col + 1);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row - 1, col + 1] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row - 1, col + 1] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row + 1, col - 1))
            {
                if (Matrix[row + 1, col - 1] == '-')
                {
                    Matrix[row + 1, col - 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row + 1, col - 1] == '+')
                {
                    HealthPackCount--;
                    Matrix[row + 1, col - 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row + 1, col - 1] == 'T')
                {
                    Matrix[row + 1, col - 1] = '*';
                    SurroundWithEnemies(row + 1, col - 1);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row + 1, col - 1] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row + 1, col - 1] == 'B')
                    boss.EatEnemy();
            }
            if (IndexIsValid(row - 1, col - 1))
            {
                if (Matrix[row - 1, col - 1] == '-')
                {
                    Matrix[row - 1, col - 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row - 1, col - 1] == '+')
                {
                    HealthPackCount--;
                    Matrix[row - 1, col - 1] = '*';
                    EnemyCount++;
                }
                else if (Matrix[row - 1, col - 1] == 'T')
                {
                    Matrix[row - 1, col - 1] = '*';
                    SurroundWithEnemies(row - 1, col - 1);
                    EnemyCount++;
                    TrapCount--;
                }
                else if (Matrix[row - 1, col - 1] == 'P')
                    Player.LoseHealth(5);
                else if (Matrix[row - 1, col - 1] == 'B')
                    boss.EatEnemy();
            }
        }
        public void PrintMatrix()
        {
            Console.Clear();
            Console.CursorVisible = false;
            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    bool isHidden = false;
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (Matrix[row, col] == 'P')
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    else if (Matrix[row, col] == 'M')
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    else if (row == boss.Row && col == boss.Col && !Boss.IsDead)
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    if (Matrix[row, col] == 'X')
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    //if ((Matrix[row, col] == 'T' || Matrix[row, col] == 'C') && DistanceToPlayer(row, col) > 1)
                    //    isHidden = true;
                    //else if (Matrix[row, col] == '+' && DistanceToPlayer(row, col) > 5)
                    //    isHidden = true;
                    if (!isHidden)
                        Console.Write($"{Matrix[row, col]}");
                    else
                        Console.Write('-');
                    if (col < Matrix.GetLength(1) - 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;

                Console.WriteLine();
            }
            Console.WriteLine($"Player health: {Player.Health}, ground bashes: {Player.GroundBashCount}{Environment.NewLine}");
            Console.WriteLine($"Boss health: {Boss.Health}");
            Console.WriteLine($"Turns played: {TurnsPlayed}");
        }

    }
}
