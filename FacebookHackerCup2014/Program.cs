using System;
using FacebookHackerCup2014.SquareDetector;
using FacebookHackerCup2014.BasketballGame;

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
            else if (args[0] == "-b")
            {
                Game[] games = GameParser.ParseGameFile(args[1]);

                for (int i = 0; i < games.Length; i++)
                {
                    Game game = games[i];
                    game.Play();

                    Player[] player1FinalPlayers = game.Team1.Court;
                    Player[] player2FinalPlayers = game.Team2.Court;

                    string[] playerNames = new string[player1FinalPlayers.Length + player2FinalPlayers.Length];
                    for (int x = 0; x < player1FinalPlayers.Length; x++)
                        playerNames[x] = player1FinalPlayers[x].Name;
                    for (int x = 0; x < player2FinalPlayers.Length; x++)
                        playerNames[player1FinalPlayers.Length + x] = player2FinalPlayers[x].Name;

                    Array.Sort(playerNames);
                    Console.WriteLine(String.Format("Case #{0}: {1}", i + 1, String.Join(" ", playerNames)));
                }
            }

            // wait before exiting
            Console.Read();
        }

        private static void DisplayUsage()
        {
            Console.Write("hackercup2014.exe [-s|-b] inputFile");
        }
    }
}
