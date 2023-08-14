using MatrixGame.Characters.Playable;
using System;
using System.Linq;
using System.Text;

namespace MatrixGame.InGameAreas
{
    public abstract class Area
    {
        private char[,] matrix;
        private int rows;
        private int cols;
        private int enemyCount;
        private int healthPackCount;
        private int turnsPlayed;
        private Player player;
        private bool isRandomGenerated;



        public char[,] Matrix { get => matrix; set { matrix = value; } }
        public int Rows { get => rows; set { rows = value; } }
        public int Cols { get => cols; set { cols = value; } }
        public int EnemyCount
        {
            get { return enemyCount; }
            protected set { enemyCount = value; }
        }

        public int HealthPackCount
        {
            get { return healthPackCount; }
            protected set { healthPackCount = value; }
        }
        public bool IsRandomGenerated
        {
            get { return isRandomGenerated; }
            protected set { isRandomGenerated = value; }
        }
        public Player Player { get => player; protected set { player = value; } }
        public int TurnsPlayed { get => turnsPlayed; set { turnsPlayed = value; } }
        public bool IndexIsValid(int row, int col) => row >= 0 && row < matrix.GetLength(0) && col >= 0 && col < matrix.GetLength(1);
        public int DistanceToPlayer(int row, int col)
        {
            int distance = Math.Abs(row - Player.Row) + Math.Abs(col - Player.Col);
            return distance;
        }
        public char[] GetRow(int rowNumber)
        {
            return Enumerable.Range(0, Matrix.GetLength(1)).Select(x => Matrix[rowNumber, x]).ToArray();
        }
        public char[] GetCol(int colNumber)
        {
            return Enumerable.Range(0, Matrix.GetLength(0)).Select(x => Matrix[x, colNumber]).ToArray();
        }
        public string[] MatrixAsStringArray()
        {
            string[] array = new string[Matrix.GetLength(0)];
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < Matrix.GetLength(0); row++)
            {
                for (int col = 0; col < Matrix.GetLength(1); col++)
                {
                    if (col != Matrix.GetLength(1) - 1)
                        sb.Append(matrix[row, col] + " ");
                    else
                        sb.Append(matrix[row, col]);
                }

                array[row] = sb.ToString().TrimEnd();
                sb.Clear();
            }
            return array;
        }
        public bool CheckVicinity(int row, int col, char element)
        {
            if (IndexIsValid(row, col + 1) && Matrix[row, col + 1] == element)
                return false;
            else if (IndexIsValid(row, col - 1) && Matrix[row, col - 1] == element)
                return false;
            else if (IndexIsValid(row + 1, col) && Matrix[row + 1, col] == element)
                return false;
            else if (IndexIsValid(row - 1, col) && Matrix[row - 1, col] == element)
                return false;
            else if (IndexIsValid(row + 1, col + 1) && Matrix[row + 1, col + 1] == element)
                return false;
            else if (IndexIsValid(row - 1, col + 1) && Matrix[row - 1, col + 1] == element)
                return false;
            else if (IndexIsValid(row - 1, col - 1) && Matrix[row - 1, col - 1] == element)
                return false;
            else if (IndexIsValid(row + 1, col - 1) && Matrix[row + 1, col - 1] == element)
                return false;
            return true;
        }
    }
}
