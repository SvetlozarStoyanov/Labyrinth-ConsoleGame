using MatrixGame.InGameAreas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixGame.Characters.NPC
{
    public class Boss : Character
    {
        private int eatenEnemies;
        private int trapCount;

        public Boss(int row, int col) : base(row, col, 70)
        {
            Directions = new List<string>(new string[] { "up", "down", "left", "right" });
        }
        public int EatenEnemies => eatenEnemies;
        public int TrapCount => trapCount;
        public void EatEnemy()
        {
            eatenEnemies++;
            Health += 3;
        }
        public int PickUpTrap(Area area)
        {
            trapCount++;
            {
                //int pickedUpTraps = 0;
                //if (area.IndexIsValid(Row + 1, Col))
                //{
                //    if (area.Matrix[Row + 1, Col] == 'T')
                //    {
                //        pickedUpTraps++;
                //        area.Matrix[Row + 1, Col] = '-';
                //    }
                //}
                //if (area.IndexIsValid(Row - 1, Col))
                //{
                //    if (area.Matrix[Row - 1, Col] == 'T')
                //    {
                //        pickedUpTraps++;
                //        area.Matrix[Row - 1, Col] = '-';
                //    }

                //}
                //if (area.IndexIsValid(Row, Col + 1))
                //{
                //    if (area.Matrix[Row, Col + 1] == 'T')
                //    {
                //        pickedUpTraps++;
                //        area.Matrix[Row, Col + 1] = '-';
                //    }
                //}
                //if (area.IndexIsValid(Row, Col - 1))
                //{
                //    if (area.Matrix[Row, Col - 1] == 'T')
                //    {
                //        pickedUpTraps++;
                //        area.Matrix[Row, Col - 1] = '-';
                //    }
                //}
                //trapCount += pickedUpTraps;
            }
            return 1;
        }
        public ValueTuple<int, int> PlaceTrap(Area area)
        {
            trapCount--;
            return (Row, Col);
        }
        public void FindBestDirection(Area area)
        {
            string bestDirection = string.Empty;
            if (this.Health > area.Player.Health)
                bestDirection = HuntPlayer(area);
            else if (area.EnemyCount == 0)
            {
                bestDirection = HuntPlayer(area);
            }
            else
            {
                bestDirection = GotoClosestEnemy(FindClosestEnemyAlternate(area), area);
                //old method
                {
                    //string[] directions = new string[] { "up", "down", "left", "right" };
                    //int minTurns = int.MaxValue;
                    //int currRow = 0;
                    //int currCol = 0;

                    //foreach (string direction in directions)
                    //{
                    //    int turns = 0;
                    //    switch (direction)
                    //    {
                    //        case "up":
                    //            currRow = this.Row - 1;
                    //            currCol = this.Col;
                    //            while (area.IndexIsValid(currRow, currCol))
                    //            {
                    //                if (area.Matrix[currRow, currCol] == '*')
                    //                {
                    //                    if (turns < minTurns)
                    //                    {
                    //                        minTurns = turns;
                    //                        bestDirection = direction;
                    //                    }
                    //                    break;
                    //                }
                    //                currRow--;
                    //                turns++;
                    //            }
                    //            break;
                    //        case "down":
                    //            currRow = this.Row + 1;
                    //            currCol = this.Col;
                    //            while (area.IndexIsValid(currRow, currCol))
                    //            {
                    //                if (area.Matrix[currRow, currCol] == '*')
                    //                {
                    //                    if (turns < minTurns)
                    //                    {
                    //                        minTurns = turns;
                    //                        bestDirection = direction;
                    //                    }
                    //                    break;
                    //                }
                    //                currRow++;
                    //                turns++;
                    //            }
                    //            break;
                    //        case "left":
                    //            currRow = this.Row;
                    //            currCol = this.Col - 1;
                    //            while (area.IndexIsValid(currRow, currCol))
                    //            {
                    //                if (area.Matrix[currRow, currCol] == '*')
                    //                {
                    //                    if (turns < minTurns)
                    //                    {
                    //                        minTurns = turns;
                    //                        bestDirection = direction;
                    //                    }
                    //                    break;
                    //                }
                    //                currCol--;
                    //                turns++;
                    //            }
                    //            break;
                    //        case "right":
                    //            currRow = this.Row;
                    //            currCol = this.Col + 1;
                    //            while (area.IndexIsValid(currRow, currCol))
                    //            {
                    //                if (area.Matrix[currRow, currCol] == '*')
                    //                {
                    //                    if (turns < minTurns)
                    //                    {
                    //                        minTurns = turns;
                    //                        bestDirection = direction;
                    //                    }
                    //                    break;
                    //                }
                    //                currCol++;
                    //                turns++;
                    //            }
                    //            break;
                    //    }
                    //}


                    //if (minTurns == int.MaxValue)
                    //    while (!ValidateDirection(bestDirection, area))
                    //    {
                    //        Random random = new Random();
                    //        bestDirection = this.Directions[random.Next(0, this.Directions.Count - 1)];
                    //    }
                }
            }
            ValidateDirection(bestDirection, area);
        }
        private ValueTuple<int, int> FindClosestEnemy(Area area)
        {
            int turnsToReach = 0;
            int minTurnsToReach = int.MaxValue;
            int targetRow = 0;
            int targetCol = 0;
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            for (int row = 0; row < area.Matrix.GetLength(0); row++)
            {
                if (!area.GetRow(row).Contains('*'))
                    continue;
                for (int col = 0; col < area.Matrix.GetLength(1); col++)
                {
                    if (area.Matrix[row, col] == '*')
                    {
                        turnsToReach = Math.Abs(row - this.Row) + Math.Abs(col - this.Col);
                        if (turnsToReach < minTurnsToReach)
                        {
                            minTurnsToReach = turnsToReach;
                            targetRow = row;
                            targetCol = col;
                        }
                    }
                }
            }
            //stopwatch.Stop();
            //Console.WriteLine("Elapsed time:" + stopwatch.Elapsed);
            //Console.ReadKey();

            return (targetRow, targetCol);
        }
        private ValueTuple<int, int> FindClosestEnemyAlternate(Area area)
        {
            int minTurnsToReach = int.MaxValue;
            int targetRow = 0;
            int targetCol = 0;
            int rowChange = 0;
            while (rowChange < minTurnsToReach /*&& minTurnsToReach != 1*/)
            {
                if (area.IndexIsValid(Row - rowChange, 0) && area.GetRow(Row - rowChange).Contains('*'))
                {
                    for (int col = 0; col < area.Matrix.GetLength(1); col++)
                    {
                        if (area.Matrix[Row - rowChange, col] == '*')
                        {
                            int turnsToReach = Math.Abs(rowChange) + Math.Abs(col - this.Col);
                            if (turnsToReach < minTurnsToReach || (turnsToReach == minTurnsToReach && AccessClusterSize(targetRow, targetCol, area) < AccessClusterSize(Row - rowChange, col, area)))
                            {
                                targetRow = Row - rowChange;
                                targetCol = col;
                                minTurnsToReach = turnsToReach;
                            }
                        }
                    }
                }
                if (area.IndexIsValid(Row + rowChange, 0) && area.GetRow(Row + rowChange).Contains('*'))
                {
                    for (int col = 0; col < area.Matrix.GetLength(1); col++)
                    {
                        if (area.Matrix[Row + rowChange, col] == '*')
                        {
                            int turnsToReach = Math.Abs(rowChange) + Math.Abs(col - this.Col);
                            if (turnsToReach < minTurnsToReach || (turnsToReach == minTurnsToReach && AccessClusterSize(targetRow, targetCol, area) < AccessClusterSize(Row + rowChange, col, area)))
                            {
                                targetRow = Row + rowChange;
                                targetCol = col;
                                minTurnsToReach = turnsToReach;
                            }
                        }
                    }
                }
                rowChange++;
            }
            return (targetRow, targetCol);
        }
        private int AccessClusterSize(int row, int col, Area area)
        {
            int clusterSize = 0;
            if (area.IndexIsValid(row + 1, col) && area.Matrix[row + 1, col] == '*')
                clusterSize++;
            if (area.IndexIsValid(row - 1, col) && area.Matrix[row - 1, col] == '*')
                clusterSize++;
            if (area.IndexIsValid(row, col + 1) && area.Matrix[row, col + 1] == '*')
                clusterSize++;
            if (area.IndexIsValid(row, col - 1) && area.Matrix[row, col - 1] == '*')
                clusterSize++;
            if (area.IndexIsValid(row + 1, col + 1) && area.Matrix[row + 1, col + 1] == '*')
                clusterSize++;
            if (area.IndexIsValid(row + 1, col - 1) && area.Matrix[row + 1, col - 1] == '*')
                clusterSize++;
            if (area.IndexIsValid(row - 1, col - 1) && area.Matrix[row - 1, col - 1] == '*')
                clusterSize++;
            if (area.IndexIsValid(row - 1, col + 1) && area.Matrix[row - 1, col + 1] == '*')
                clusterSize++;
            return clusterSize;
        }
        private string GotoClosestEnemy(ValueTuple<int, int> coordinates, Area area)
        {
            int targetRow = coordinates.Item1;
            int targetCol = coordinates.Item2;
            int rowDiff = targetRow - this.Row;
            int colDiff = targetCol - this.Col;
            string bestDirection = string.Empty;
            if (Math.Abs(rowDiff) > Math.Abs(colDiff))
            {
                if (rowDiff < 0)
                    bestDirection = "up";
                else if (rowDiff > 0)
                    bestDirection = "down";
            }
            else if (Math.Abs(rowDiff) < Math.Abs(colDiff))
            {
                if (colDiff < 0)
                    bestDirection = "left";
                else if (colDiff > 0)
                    bestDirection = "right";
            }
            else
            {
                string rowDirection = string.Empty;
                if (rowDiff < 0)
                    rowDirection = "up";
                else if (rowDiff > 0)
                    rowDirection = "down";
                string colDirection = string.Empty;
                if (colDiff < 0)
                    colDirection = "left";
                else if (colDiff > 0)
                    colDirection = "right";
                string[] directions = new string[] { rowDirection, colDirection };
                Random random = new Random();
                bestDirection = directions[random.Next(0, 1)];
            }
            return bestDirection;
        }
        private bool ValidateDirection(string direction, Area area)
        {
            switch (direction)
            {
                case "up":
                    if ((area.IndexIsValid(this.Row - 1, this.Col)))
                    {
                        this.Row--;
                        return true;
                    }
                    break;
                case "down":
                    if (area.IndexIsValid(this.Row + 1, this.Col))
                    {
                        this.Row++;
                        return true;
                    }

                    break;
                case "left":
                    if (area.IndexIsValid(this.Row, this.Col - 1))
                    {
                        this.Col--;
                        return true;
                    }
                    break;
                case "right":
                    if (area.IndexIsValid(this.Row, this.Col + 1))
                    {
                        this.Col++;
                        return true;
                    }
                    break;
            }
            return false;
        }
        private string HuntPlayer(Area area)
        {
            int rowDiff = area.Player.Row - this.Row;
            int colDiff = area.Player.Col - this.Col;
            string bestDirection = string.Empty;
            if (Math.Abs(rowDiff) > Math.Abs(colDiff))
            {
                if (rowDiff < 0)
                    bestDirection = "up";
                else if (rowDiff > 0)
                    bestDirection = "down";
            }
            else if (Math.Abs(rowDiff) < Math.Abs(colDiff))
            {
                if (colDiff < 0)
                    bestDirection = "left";
                else if (colDiff > 0)
                    bestDirection = "right";
            }
            else
            {
                string rowDirection = string.Empty;
                if (rowDiff < 0)
                    rowDirection = "up";
                else if (rowDiff > 0)
                    rowDirection = "down";
                string colDirection = string.Empty;
                if (colDiff < 0)
                    colDirection = "left";
                else if (colDiff > 0)
                    colDirection = "right";
                string[] directions = new string[] { rowDirection, colDirection };
                
                bestDirection = ChooseBestPositionToMoveToWhileChasingPlayer(directions, area);
            }
            return bestDirection;
        }
        private string ChooseBestPositionToMoveToWhileChasingPlayer(string[] directions, Area area)
        {
            string preferredRowDirection = null;

            switch (directions[0])
            {
                case "up":
                    if (area.Matrix[Row - 1, Col] == '*')
                        preferredRowDirection = "up";
                    break;
                case "down":
                    if (area.Matrix[Row + 1, Col] == '*')
                        preferredRowDirection = "down";
                    break;
            }
            string preferredColDirection = null;
            switch (directions[1])
            {
                case "left":
                    if (area.Matrix[Row, Col - 1] == '*')
                        preferredColDirection = "left";
                    break;
                case "right":
                    if (area.Matrix[Row, Col + 1] == '*')
                        preferredColDirection = "right";
                    break;
            }
            Random random = new Random();
            string preferredDirection = preferredRowDirection != null && preferredColDirection == null ? preferredRowDirection : preferredRowDirection == null && preferredColDirection != null ? preferredColDirection : directions[random.Next(0, 1)];
            return preferredDirection;
        }
    }
}


