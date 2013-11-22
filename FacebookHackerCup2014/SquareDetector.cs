using System;
using System.IO;
using System.Drawing;

namespace FacebookHackerCup2014.SquareDetector
{
    public struct Board
    {
        enum Color
        {
            Black = '#',
            White = '.'
        }

        public int Size
        {
            get { return Characters.Length; }
        }

        public char[][] Characters
        {
            get;
            set;
        }

        public Board(char[][] characters) : this()
        {
            Characters = characters;
        }

        public bool IsSquare()
        {
            Point topLeft = new Point(Int32.MaxValue, Int32.MaxValue);
            Point topRight = new Point(Int32.MinValue, Int32.MaxValue);
            Point bottomLeft = new Point(Int32.MaxValue, Int32.MinValue);
            Point bottomRight = new Point(Int32.MinValue, Int32.MinValue);

            // get the corners of the square
            for (int y = 0; y < Characters.Length; y++)
            {
                for (int x = 0; x < Characters[y].Length; x++)
                {
                    char chararcter = Characters[y][x];
                    bool isBlack = (chararcter == (char)Color.Black);

                    if (isBlack)
                    {
                        // update top left
                        if (y < topLeft.Y || (y == topLeft.Y && x < topLeft.X))
                            topLeft = new Point(x, y);

                        // update top right
                        if (y < topRight.Y || (y == topRight.Y && x > topRight.X))
                            topRight = new Point(x, y);

                        // update bottom left
                        if (y > bottomLeft.Y || (y == bottomLeft.Y && x < bottomLeft.X))
                            bottomLeft = new Point(x, y);

                        // update bottom right
                        if (y > bottomRight.Y || (y == bottomRight.Y && x > bottomRight.X))
                            bottomRight = new Point(x, y);
                    }
                }
            }

            // get the lengths of each side
            int topDifference = (topRight.X - topLeft.X);
            int bottomDifference = (bottomRight.X - bottomLeft.X);
            int leftDifference = (bottomLeft.Y - topLeft.Y);
            int rightDifference = (bottomRight.Y - topRight.Y);

            // make sure all sides of the square are of equal length
            if (topDifference + bottomDifference + leftDifference + rightDifference != topDifference * 4)
                return false;

            // make sure the square is filled
            for (int y = topLeft.Y; y <= bottomRight.Y; y++)
            {
                for (int x = bottomLeft.X; x <= bottomRight.X; x++)
                {
                    char chararcter = Characters[y][x];
                    bool isBlack = (chararcter == (char)Color.Black);

                    // if it's not black, then the square is not filled
                    if (!isBlack)
                        return false;
                }
            }

            return true;
        }
    }

    public class BoardParser
    {
        public static Board[] ParseBoardFile(string path)
        {
            // read all the the lines, find out how many boards there are
            string[] inputLines = File.ReadAllLines(path);
            int numberOfBoards = Int32.Parse(inputLines[0]);

            Board[] boards = new Board[numberOfBoards];

            int currentBoardIndex = 0;
            for (int i = 1; i < inputLines.Length; i += boards[currentBoardIndex - 1].Size)
            {
                // find the size of the board
                int size = Int32.Parse(inputLines[i++]);
                char[][] characters = new char[size][];

                // load all the lines
                for (int x = 0; x < size; x++)
                    characters[x] = inputLines[i + x].ToCharArray();

                // create the board
                boards[currentBoardIndex] = new Board(characters);

                // don't try and exceed the amount of boards stated
                if (++currentBoardIndex == numberOfBoards)
                    break;
            }

            return boards;
        }
    }
}
