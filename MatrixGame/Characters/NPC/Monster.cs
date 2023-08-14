using MatrixGame.InGameAreas;
using System;

namespace MatrixGame.Characters.NPC
{
    public class Monster : Character
    {
        private string direction;
        private char lastEncounteredElement;
        private int previousRow;
        private int previousCol;
        public Monster(int row, int col, string direction) : base(row, col, 400)
        {
            Direction = direction;
            lastEncounteredElement = ' ';
        }
        public string Direction
        {
            get { return direction; }
            private set { direction = value; }
        }

        public char LastEncounteredElement { get => lastEncounteredElement; set => lastEncounteredElement = value; }
        public int PreviousRow { get => previousRow; set => previousRow = value; }
        public int PreviousCol { get => previousCol; set => previousCol = value; }
    }
}
