using MatrixGame.Characters.Playable;
using MatrixGame.InGameAreas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixGame.Characters
{
    public abstract class Character
    {
        private int health;
        private int row;
        private int col;
        private List<string> directions;
        public Character(int row, int col, int health)
        {
            Row = row;
            Col = col;
            Health = health;
        }
        public int Col
        {
            get { return col; }
            set { col = value; }
        }
        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public int Health { get => health; set { health = value; } }
        public List<string> Directions { get => directions; set { directions = value; } }
        public bool IsDead => Health <= 0;
        public void LoseHealth(int amount)
        {
            Health -= amount;
            if (Health < 0)
                Health = 0;
        }
        public void GainHealth(int amount)
        {
            Health += amount;
        }
        public void Fight(Character character)
        {
            int temp = character.Health;
            character.LoseHealth(this.Health);
            LoseHealth(temp);
        }
        public int MeasureDistanceToCharacter(Character character)
        {
            int rowDiff = Math.Abs(Row - character.Row);
            int colDiff = Math.Abs(Col - character.Col);
            return rowDiff + colDiff;
        }
    }
}
