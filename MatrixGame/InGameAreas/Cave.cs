using MatrixGame.Characters.Playable;
using MatrixGame.Characters.NPC;
using MatrixGame.Characters;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.InGameAreas
{
    public class Cave : Area
    {
        private bool hasEscaped;
        private int playerRowBeforeEntering;
        private int playerColBeforeEntering;
        private Spider spider;
        private Decoy decoy;
        private bool isInWeb;
        private int turnsInWeb;
        private bool spiderStandingOnExit;
        public Cave(string path, Player player)
        {
            Player = player;
            playerRowBeforeEntering = player.Row;
            playerColBeforeEntering = player.Col;
            ReadMatrixFromFile(path);
            MoveInCave();
        }
        public Cave(int size)
        {
            GenerateMatrix(size);
        }
        public Spider Spider { get => spider; private set { spider = value; } }
        public Decoy Decoy { get => decoy; private set { decoy = value; } }
        private void ReadMatrixFromFile(string path)
        {
            string[] matrixLineByLine = File.ReadAllLines(path);
            Matrix = new char[matrixLineByLine.Length, matrixLineByLine.Length];
            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                char[] rowInput = matrixLineByLine[row].Split().Select(char.Parse).ToArray();
                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    Matrix[row, col] = rowInput[col];
                    if (Matrix[row, col] == 'P')
                    {
                        Player.Row = row;
                        Player.Col = col;
                    }
                    if (Matrix[row, col] == 'S')
                    {
                        Spider = new Spider(row, col);
                    }
                    else if (Matrix[row, col] == '*')
                        EnemyCount++;
                    else if (Matrix[row, col] == '+')
                        HealthPackCount++;
                }
            }
        }
        private void MoveInCave()
        {
            PrintMatrix();
            hasEscaped = false;
            while (!Player.IsDead)
            {
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                if (pressedKey == ConsoleKey.Escape)
                {
                    PauseMenu();
                    PrintMatrix();
                    pressedKey = Console.ReadKey(true).Key;
                }

                PlayerAction(pressedKey);
                PrintMatrix();
                if (hasEscaped)
                    break;
                SpiderAction();
                PrintMatrix();
            }
            Player.Row = playerRowBeforeEntering;
            Player.Col = playerColBeforeEntering;
        }
        public void PlayerAction(ConsoleKey pressedKey)
        {
            if (isInWeb)
            {
                turnsInWeb--;
                if (turnsInWeb == 0)
                    isInWeb = false;
                return;
            }
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
                    Console.WriteLine("Cannot bash underground!");
                    pressedKey = Console.ReadKey(true).Key;
                    PlayerAction(pressedKey);
                    break;
                case ConsoleKey.F:
                    if (Player.DecoyCount == 0)
                    {
                        Console.WriteLine("No decoys left!");
                        pressedKey = Console.ReadKey(true).Key;
                        PlayerAction(pressedKey);
                    }
                    else
                    {
                        DeployDecoy();
                        if (Decoy == null)
                        {
                            pressedKey = Console.ReadKey(true).Key;
                            PlayerAction(pressedKey);
                        }

                    }
                    break;
                default:
                    Console.WriteLine("Press a valid button!");
                    pressedKey = Console.ReadKey(true).Key;
                    PlayerAction(pressedKey);
                    break;
            }
            if (actionIsValid)
                CheckPlayerPosition();
            if (!Player.IsDead)
                Matrix[Player.Row, Player.Col] = 'P';
            else
                Matrix[Player.Row, Player.Col] = 'X';
        }
        private void DeployDecoy()
        {
            ValueTuple<string, int> decoyCoordinates = Player.GetDecoyCoordinates();
            string direction = decoyCoordinates.Item1;
            int distance = decoyCoordinates.Item2;
            Decoy = new Decoy(Player.Row, Player.Col);
            switch (direction)
            {
                case "up":
                    ThrowDecoy(Player.Row - distance, Player.Col);
                    break;
                case "down":
                    ThrowDecoy(Player.Row + distance, Player.Col);
                    break;
                case "left":
                    ThrowDecoy(Player.Row, Player.Col - distance);
                    break;
                case "right":
                    ThrowDecoy(Player.Row, Player.Col + distance);
                    break;
            }
            Player.DecoyCount--;
        }

        private void ThrowDecoy(int targetRow, int targetCol)
        {
            int currRow = Player.Row;
            int currCol = Player.Col;
            while (true)
            {
                if (currRow < targetRow)
                    currRow++;
                else if (currRow > targetRow)
                    currRow--;
                if (currCol < targetCol)
                    currCol++;
                else if (currCol > targetCol)
                    currCol--;
                if (!IndexIsValid(currRow, currCol) || !DecoyThrowCheckForObstacle(currRow, currCol))
                    break;
                if (currRow == targetRow && currCol == targetCol)
                    break;
            }
            if (Decoy.Row == Player.Row && Decoy.Col == Player.Col)
            {
                Decoy = null;
                Player.DecoyCount++;
            }
            if (Decoy != null)
                Matrix[Decoy.Row, Decoy.Col] = 'D';
        }

        private bool DecoyThrowCheckForObstacle(int row, int col)
        {
            CheckVicinity(row, col);
            Decoy.Row = row;
            Decoy.Col = col;
            if (Matrix[row, col] == '*')
            {
                EnemyCount--;
                return false;
            }
            else if (Matrix[row, col] == '+')
            {
                HealthPackCount--;
                return false;
            }
            else if (Matrix[row, col] == 'W')
            {
                return false;
            }
            else if (Matrix[row, col] == 'S')
            {
                Decoy.Fight(Spider);
                Decoy = null;
                return false;
            }

            return true;
        }
        private void CheckPlayerPosition()
        {
            CheckVicinity(Player.Row, Player.Col);
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
            else if (Matrix[Player.Row, Player.Col] == 'W')
            {
                isInWeb = true;
                turnsInWeb = 3;
            }
            else if (Matrix[Player.Row, Player.Col] == 'S')
                Player.Fight(Spider);
            else if (Matrix[Player.Row, Player.Col] == 'D')
            {
                Decoy = null;
                Player.DecoyCount++;
            }
            else if (Matrix[Player.Row, Player.Col] == 'E')
                hasEscaped = true;
            if (!Player.IsDead)
                Matrix[Player.Row, Player.Col] = 'P';
            else
                Matrix[Player.Row, Player.Col] = 'X';
        }
        private void CheckVicinity(int row, int col)
        {
            if (IndexIsValid(row, col + 1) && Matrix[row, col + 1] == '^')
            {
                SpiderTeleport(row, col + 1);
            }
            else if (IndexIsValid(row, col - 1) && Matrix[row, col - 1] == '^')
            {
                SpiderTeleport(row, col - 1);
            }
            else if (IndexIsValid(row + 1, col) && Matrix[row + 1, col] == '^')
            {
                SpiderTeleport(row + 1, col);
            }
            else if (IndexIsValid(row - 1, col) && Matrix[row - 1, col] == '^')
            {
                SpiderTeleport(row - 1, col);
            }
            else if (IndexIsValid(row + 1, col + 1) && Matrix[row + 1, col + 1] == '^')
            {
                SpiderTeleport(row + 1, col + 1);
            }
            else if (IndexIsValid(row - 1, col + 1) && Matrix[row - 1, col + 1] == '^')
            {
                SpiderTeleport(row - 1, col + 1);
            }
            else if (IndexIsValid(row - 1, col - 1) && Matrix[row - 1, col - 1] == '^')
            {
                SpiderTeleport(row - 1, col - 1);
            }
            else if (IndexIsValid(row + 1, col - 1) && Matrix[row + 1, col - 1] == '^')
            {
                SpiderTeleport(row + 1, col - 1);
            }
        }
        public void SpiderAction()
        {
            if (Spider.MissTurn)
            {
                Spider.MissTurn = false;
                return;
            }
            if (!spiderStandingOnExit)
                Matrix[Spider.Row, Spider.Col] = 'W';
            else
            {
                spiderStandingOnExit = false;
                Matrix[Spider.Row, Spider.Col] = 'E';
            }
            Spider.FindBestDirection(this);
            CheckSpiderPosition();
        }
        public void CheckSpiderPosition()
        {
            if (Matrix[Spider.Row, Spider.Col] == '*')
                EnemyCount--;
            else if (Matrix[Spider.Row, Spider.Col] == '+')
                HealthPackCount--;
            else if (Matrix[Spider.Row, Spider.Col] == 'P')
                Spider.Fight(Player);
            else if (Matrix[Spider.Row, Spider.Col] == 'D')
            {
                Spider.Fight(Decoy);
                Decoy = null;
            }
            else if (Matrix[Spider.Row, Spider.Col] == 'E')
            {
                spiderStandingOnExit = true;
            }
            if (!Spider.IsDead)
                Matrix[Spider.Row, Spider.Col] = 'S';
            if (Player.IsDead)
                Matrix[Player.Row, Player.Col] = 'X';
        }
        private void SpiderTeleport(int row, int col)
        {
            Matrix[Spider.Row, Spider.Col] = 'W';
            Spider.Row = row;
            Spider.Col = col;
            Matrix[Spider.Row, Spider.Col] = 'S';
            Spider.MissTurn = true;
        }
        public void GenerateMatrix(int size)
        {
            List<char> elementsOfTheGame = new List<char> { '-', '+', '*', 'E', 'W', 'P', 'S' };
            Matrix = new char[size, size];
            if (size >= 15)
                elementsOfTheGame.Add('G');
            if (size >= 10)
                elementsOfTheGame.Add('C');
            Random random = new Random();
            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    int randomIndex = random.Next(0, elementsOfTheGame.Count);

                    if (elementsOfTheGame[randomIndex] == 'P' && CheckVicinity(row, col, 'S') /*&& CheckVicinity(row, col, '+')*/)
                    {
                        Player = new Player(row, col);
                    }
                    else if (elementsOfTheGame[randomIndex] == '*' && GetRow(row).Count(x => x == '*') < 3)
                    {
                        EnemyCount++;
                        //if (EnemyCount == 80)
                        //    elementsOfTheGame.Remove('*');
                    }
                    else if (elementsOfTheGame[randomIndex] == 'W' && GetRow(row).Count(x => x == 'W') < 1 && GetCol(col).Count(x => x == 'W') < 1)
                    {

                    }
                    else if (elementsOfTheGame[randomIndex] == '^' && GetRow(row).Count(x => x == '^') < 2 && GetCol(col).Count(x => x == '^') < 2 && MatrixAsStringArray().Count(x => x == "^") < 2)
                    {

                    }
                    else if (elementsOfTheGame[randomIndex] == '+' && CheckVicinity(row, col, '+') && GetRow(row).Count(x => x == '+') < 1)
                    {
                        HealthPackCount++;
                    }
                    else if (elementsOfTheGame[randomIndex] == 'S' && CheckVicinity(row, col, 'P'))
                    {

                    }
                    else if (elementsOfTheGame[randomIndex] == 'E' && Player != null && DistanceToPlayer(row, col) > 10)
                    {

                    }
                    else
                    {
                        Matrix[row, col] = '-';
                        continue;
                    }
                    Matrix[row, col] = elementsOfTheGame[randomIndex];
                    if (Matrix[row, col] == 'S')
                        elementsOfTheGame.Remove('S');
                    if (Matrix[row, col] == 'P')
                        elementsOfTheGame.Remove('P');
                    if (Matrix[row, col] == 'E')
                        elementsOfTheGame.Remove('E');
                }
            }
            string[] array = MatrixAsStringArray();
            File.WriteAllLines($"../../../Levels/level_0_cave.txt", array);
        }
        public void PrintMatrix()
        {
            Console.Clear();
            Console.CursorVisible = false;

            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    if (Matrix[row, col] == 'P')
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    if (Matrix[row, col] == 'S')
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    else if (Matrix[row, col] == 'X')
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write($"{Matrix[row, col]}");
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (col < Matrix.GetLength(1) - 1)
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Player health: {Player.Health}, Decoys left : {Player.DecoyCount}");
            Console.WriteLine($"Spider health: {Spider.Health}");
            if (isInWeb)
                Console.WriteLine($"Turns until web breaks : {turnsInWeb}");
        }
        private void PauseMenu()
        {
            Console.Clear();
            Console.WriteLine("Press Space to continue/Enter to go back to the menu/Escape to exit to desktop.");
            while (true)
            {
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                switch (pressedKey)
                {
                    case ConsoleKey.Spacebar:
                        return;
                    case ConsoleKey.Enter:
                        System.Diagnostics.Process.Start(@"MatrixGame.exe");
                        Environment.Exit(0);
                        return;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Press a valid key!");
                        break;
                }
            }
        }
    }

}