using MatrixGame.InGameAreas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Characters.NPC
{
    public class Spider : Character
    {
        private bool missTurn;
        public Spider(int row, int col) : base(row, col, 200)
        {
            Directions = new List<string>() { "up", "down", "left", "right", "up-left", "up-right", "down-left", "down-right" };
        }
        public bool MissTurn { get => missTurn; set { missTurn = value; } }
        public void FindBestDirection(Area area)
        {
            string bestDirection = HuntPlayer(area);
            ValidateDirection(bestDirection, area);
        }
        public string HuntPlayer(Area area)
        {
            Character huntedCharacter = area.Player;
            Cave cave = area as Cave;
            if (cave.Decoy != null)
                huntedCharacter = cave.Decoy;

            int rowDiff = huntedCharacter.Row - this.Row;
            int colDiff = huntedCharacter.Col - this.Col;
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
                if (rowDiff > 0 && colDiff > 0)
                    bestDirection = "down-right";
                else if (rowDiff > 0 && colDiff < 0)
                    bestDirection = "down-left";
                else if (rowDiff < 0 && colDiff < 0)
                    bestDirection = "up-left";
                else if (rowDiff < 0 && colDiff > 0)
                    bestDirection = "up-right";
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
                case "up-left":
                    if (area.IndexIsValid(this.Row - 1, this.Col - 1))
                    {
                        this.Row--;
                        this.Col--;
                        return true;
                    }
                    break;
                case "up-right":
                    if (area.IndexIsValid(this.Row - 1, this.Col + 1))
                    {
                        this.Row--;
                        this.Col++;
                        return true;
                    }
                    break;
                case "down-left":
                    if (area.IndexIsValid(this.Row + 1, this.Col - 1))
                    {
                        this.Row++;
                        this.Col--;
                        return true;
                    }
                    break;
                case "down-right":
                    if (area.IndexIsValid(this.Row + 1, this.Col + 1))
                    {
                        this.Row++;
                        this.Col++;
                        return true;
                    }
                    break;
            }
            return false;
        }

    }
}
