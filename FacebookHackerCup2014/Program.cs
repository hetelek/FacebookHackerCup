using System;
using FacebookHackerCup2014.SquareDetector;

namespace FacebookHackerCup2014
{
    class Program
    {
        static void Main(string[] args)
        {
            // check for valid argument count
            if (args.Length != 2)
            {
                DisplayUsage();
                return;
            }

            if (args[0] == "-s")
            {
                // square detector

                Board[] boards = BoardParser.ParseBoardFile(args[1]);
                for (int i = 0; i < boards.Length; i++)
                    Console.WriteLine(String.Format("Case #{0}: {1}", i + 1, (boards[i].IsSquare() ? "YES" : "NO")));
            }

            // wait before exiting
            Console.Read();
        }

        private static void DisplayUsage()
        {
            Console.Write("hackercup2014.exe [-s] inputFile");
        }
    }
}
