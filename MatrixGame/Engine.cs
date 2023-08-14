using MatrixGame.InGameAreas;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame
{
    public class Engine
    {
        public void Run()
        {
            Console.SetWindowSize(100, 40);
            Console.Title = "Labyrinth";
            while (true)
            {
                Console.Clear();
                Console.CursorVisible = true;

                Console.WriteLine("====Labyrinth====");
                Console.WriteLine("1.Play all levels");
                Console.WriteLine("2.Choose a level");
                Console.WriteLine("3.Play random generated level");
                Console.WriteLine("4.View Controls");
                Console.WriteLine("5.Map Legend");
                Console.WriteLine("6.Game Instructions");
                Console.WriteLine("7.Exit");
                Console.Write("Choose an option [1-7]: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        PlayLevelsInOrder();
                        break;
                    case "2":
                        ChooseLevel();
                        break;
                    case "3":
                        GenerateLevel();
                        break;
                    case "4":
                        Console.WriteLine(ViewControls());
                        Console.ReadKey();
                        break;
                    case "5":
                        Console.WriteLine(MapLegend());
                        Console.ReadKey();
                        break;
                    case "6":
                        Console.WriteLine(GameInstructions());
                        Console.ReadKey();
                        break;
                    case "7":
                        Environment.Exit(0);
                        break;
                    default:

                        break;
                }
            }
        }
        private void PlayLevelsInOrder()
        {
            for (int i = 0; i < 8; i++)
            {
                ConsoleKey pressedKey = ConsoleKey.Clear;
                if (!PlayLevel(i + 1))
                {
                    Console.Beep(3500, 30);
                    Console.WriteLine($"You lose!{Environment.NewLine}Press space to try again or press escape to return to the menu.");
                    while (true)
                    {
                        pressedKey = Console.ReadKey(true).Key;
                        switch (pressedKey)
                        {
                            case ConsoleKey.Spacebar:
                                PlayLevel(i + 1);
                                return;
                            case ConsoleKey.Escape:
                                return;
                            default:
                                Console.WriteLine("Invalid Key! Press space to try again or press escape to return to the menu.");
                                break;
                        }
                    }
                }
                Console.Beep();
                Console.WriteLine($"You win!{Environment.NewLine}Press space to continue or press escape to return to the menu.");
                while (pressedKey != ConsoleKey.Spacebar)
                {
                    pressedKey = Console.ReadKey(true).Key;
                    switch (pressedKey)
                    {
                        case ConsoleKey.Spacebar:
                            break;
                        case ConsoleKey.Escape:
                            return;
                        default:
                            Console.WriteLine("Invalid Key! Press space to continue or press escape to return to the menu.");
                            break;
                    }
                }
            }
        }
        private void ChooseLevel()
        {
            string choice = string.Empty;
            int levelNumber;
            while (true)
            {
                Console.Write("Choose level [1-7]: ");
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                        levelNumber = int.Parse(choice);
                        ConsoleKey pressedKey = ConsoleKey.NoName;
                        while (true)
                        {
                            if (!PlayLevel(levelNumber))
                            {
                                Console.Beep(3500, 30);
                                Console.WriteLine($"You lose!{Environment.NewLine}Press space to try again! Press space to try again or press escape to return to the menu.");
                            }
                            else
                            {
                                Console.Beep();
                                Console.WriteLine($"You win!{Environment.NewLine}Press space to try again or press escape to return to the menu.");
                            }
                            pressedKey = Console.ReadKey(true).Key;
                            while (pressedKey != ConsoleKey.Spacebar)
                            {
                                switch (pressedKey)
                                {
                                    case ConsoleKey.Spacebar:
                                        break;
                                    case ConsoleKey.Escape:
                                        return;
                                    default:
                                        Console.WriteLine("Invalid Key! Press space to play again or press escape to return to the menu.");
                                        break;
                                }
                                pressedKey = Console.ReadKey(true).Key;

                            }
                        }
                    default:
                        Console.WriteLine("Invalid level!");
                        break;
                }
            }
        }
        private void GenerateLevel()
        {

            if (File.Exists($"../../../Levels/level_0.txt"))
                File.Delete($"../../../Levels/level_0.txt");

            //File.Create($"../../../Levels/level_0.txt");

            int size = 0;
            while (true)
            {
                Console.Write("Select labyrinth size [10-30]: ");
                string choice = Console.ReadLine();
                size = int.Parse(choice);
                if (size >= 10 && size <= 30)
                    break;
                Console.WriteLine("Invalid size!");
            }
            GameField gameField = new GameField(size);
            string[] array = gameField.MatrixAsStringArray();
            File.WriteAllLines($"../../../Levels/level_0.txt", array);
            while (true)
            {
                if (!PlayLevel(0))
                {
                    Console.Beep(3500, 30);
                    Console.WriteLine($"You lose!{Environment.NewLine}Press space to try again! Press space to try again or press escape to return to the menu.");
                }
                else
                {
                    Console.Beep();
                    Console.WriteLine($"You win!{Environment.NewLine}Press space to try again or press escape to return to the menu.");
                }
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                while (pressedKey != ConsoleKey.Spacebar)
                {
                    switch (pressedKey)
                    {
                        case ConsoleKey.Spacebar:
                            break;
                        case ConsoleKey.Escape:
                            return;
                        default:
                            Console.WriteLine("Invalid Key! Press space to play again or press escape to return to the menu.");
                            break;
                    }
                    pressedKey = Console.ReadKey(true).Key;
                }
            }
        }
        private bool PlayLevel(int levelNumber)
        {
            GameField gameField = new GameField($"../../../Levels/level_{levelNumber}.txt");
            gameField.PrintMatrix();
            while (!gameField.Player.IsDead && !gameField.Boss.IsDead)
            {
                gameField.TurnsPlayed++;
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                if (pressedKey == ConsoleKey.Escape)
                {
                    PauseMenu();
                }
                gameField.PlayerAction(pressedKey);
                gameField.PrintMatrix();
                if (gameField.Player.IsDead)
                {
                    return false;
                }
                if (gameField.Boss.IsDead)
                {
                    return true;
                }
                gameField.BossAction();
                gameField.PrintMatrix();
                if (gameField.Player.IsDead)
                {
                    return false;
                }
                if (gameField.Boss.IsDead)
                {
                    return true;
                }
            }
            return true;
        }
        private string ViewControls()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Movement W,A,S,D");
            sb.AppendLine("G: Ground bash (Only when in overworld!)");
            sb.AppendLine("F: Decoy(Only when in cave!)");
            return sb.ToString().TrimEnd();
        }
        private string GameInstructions()
        {
            string explanation = File.ReadAllText("../../../instructions.txt");
            return explanation;
        }
        private string MapLegend()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("P: Player");
            sb.AppendLine("-: Empty space");
            sb.AppendLine("*: Enemy");
            sb.AppendLine("T: Trap");
            sb.AppendLine("G: Ground bash pickup");
            sb.AppendLine("+: Health kit");
            sb.AppendLine("B: Boss");
            sb.AppendLine("C: Cave");
            sb.AppendLine("E: Exit (Only in cave)");
            sb.AppendLine("S: Spider (Only in cave)");
            sb.AppendLine("D: Decoy (Only in cave)");
            sb.AppendLine($"W: Web (Only in cave)");
            sb.AppendLine($"^: Spider nest (Only in cave)");
            return sb.ToString().TrimEnd();
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


