using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookHackerCup2014.BasketballGame
{
    public struct Game
    {
        public int MinutesToPlay { get; private set; }
        public int PlayersOnCourtPerTeam { get; private set; }
        public Team Team1 { get; private set; }
        public Team Team2 { get; private set; }
        public int TotalPlayers
        {
            get
            {
                return Team1.Players.Length + Team2.Players.Length;
            }
        }

        bool played;
        public Game(Team team1, Team team2, int minutesToPlay, int playersOnCourtPerTeam) : this()
        {
            played = false;

            Team1 = team1;
            Team2 = team2;

            MinutesToPlay = minutesToPlay;
            PlayersOnCourtPerTeam = playersOnCourtPerTeam;
        }

        public void Play()
        {
            if (played)
                return;

            for (int i = 1; i <= MinutesToPlay; i++)
            {
                // update how many minutes each player has played
                Team1.UpdateTimePlayed(1);
                Team2.UpdateTimePlayed(1);

                // update who's on the court
                Team1.UpdatePlayerStatuses(PlayersOnCourtPerTeam);
                Team2.UpdatePlayerStatuses(PlayersOnCourtPerTeam);
            }

            played = true;
        }
    }

    public struct Team
    {
        private class PlayingTimeSorter : IComparer<Player>
        {
            public int Compare(Player x, Player y)
            {
                // if the minutes played aren't the same, the person with the most gets benched
                if (x.MinutesPlayed < y.MinutesPlayed)
                    return -1;
                else if (x.MinutesPlayed > y.MinutesPlayed)
                    return 1;

                // whoever has the better draft number plays if they have the same number of minutes played
                if (x.DraftNumber < y.DraftNumber)
                    return -1;
                else if (x.DraftNumber > y.DraftNumber)
                    return 1;

                return 0;
            }
        }

        public Player[] Players { get; private set; }
        public Player[] Bench
        {
            get
            {
                return Players.Where(player => player.CurrentStatus == Player.Status.Bench).ToArray<Player>();
            }
        }
        public Player[] Court
        {
            get
            {
                return Players.Where(player => player.CurrentStatus == Player.Status.Court).ToArray<Player>();
            }
        }

        public Team(Player[] players) : this()
        {
            Players = players;
        }

        public void UpdatePlayerStatuses(int amountToPlay)
        {
            Array.Sort<Player>(Players, new PlayingTimeSorter());

            // sort them based off of playing time, and choose who is playing now
            for (int i = 0; i < Players.Length; i++)
            {
                // once we play the first N, start benching the rest
                if (i < amountToPlay)
                    Players[i].CurrentStatus = Player.Status.Court;
                else
                    Players[i].CurrentStatus = Player.Status.Bench;
            }
        }

        public void UpdateTimePlayed(int minutes)
        {
            // update how much time every player on the court has played
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].CurrentStatus == Player.Status.Court)
                    Players[i].MinutesPlayed += minutes;
            }
        }
    }

    public struct Player
    {
        public enum Status
        {
            Bench,
            Court
        }

        public string Name { get; private set; }
        public int ShotPercentage { get; private set; }
        public int Height { get; private set; }
        public int MinutesPlayed { get; set; }
        public Status CurrentStatus { get; set; }
        public int DraftNumber { get; set; }

        public Player(string name, int shotPercentage, int height) : this()
        {
            Name = name;
            ShotPercentage = shotPercentage;
            Height = height;
        }
    }

    class GameParser
    {
        private class PlayerRankSorter : IComparer<Player>
        {
            public int Compare(Player x, Player y)
            {
                // first see if the shot percentages differ - if so, the higher one wins
                if (x.ShotPercentage > y.ShotPercentage)
                    return -1;
                else if (x.ShotPercentage < y.ShotPercentage)
                    return 1;

                // if the shot percentages are the same, then the taller player wins
                if (x.Height > y.Height)
                    return -1;
                else if (x.Height < y.Height)
                    return 1;

                // they are even (should never happen)
                return 0;
            }
        }

        public static Game[] ParseGameFile(string path)
        {
            // read all the the lines, find out how many boards there are
            string[] inputLines = File.ReadAllLines(path);
            int numberOfGames = Int32.Parse(inputLines[0]);

            Game[] games = new Game[numberOfGames];

            int currentGameIndex = 0;
            for (int i = 1; i < inputLines.Length; i += games[currentGameIndex - 1].TotalPlayers)
            {
                // read game properties
                string[] gameProperties = inputLines[i++].Split(' ');
                int playerCount = Int32.Parse(gameProperties[0]);
                int minutesToPlay = Int32.Parse(gameProperties[1]);
                int playersOnCourtPerTeam = Int32.Parse(gameProperties[2]);

                // load the players
                Player[] players = new Player[playerCount];
                for (int x = 0; x < playerCount; x++)
                {
                    // parse the player's properties
                    string[] playerProperties = inputLines[i + x].Split(' ');
                    string name = playerProperties[0];
                    int shotPercentage = Int32.Parse(playerProperties[1]);
                    int height = Int32.Parse(playerProperties[2]);

                    players[x] = new Player(name, shotPercentage, height);
                }

                // rank the players
                Array.Sort<Player>(players, new PlayerRankSorter());
                for (int x = 0; x < players.Length; x++)
                    players[x].DraftNumber = x;

                // make the teams (odd indexes for 1st team, even for 2nd team)
                Player[] team1Players = players.Where<Player>((item, index) => index % 2 != 0).ToArray<Player>();
                Player[] team2Players = players.Where<Player>((item, index) => index % 2 == 0).ToArray<Player>();

                // set the playes who are currently playing
                for (int x = 0; x < playersOnCourtPerTeam; x++)
                {
                    team1Players[x].CurrentStatus = Player.Status.Court;
                    team2Players[x].CurrentStatus = Player.Status.Court;
                }

                // create the game
                Team team1 = new Team(team1Players);
                Team team2 = new Team(team2Players);
                games[currentGameIndex] = new Game(team1, team2, minutesToPlay, playersOnCourtPerTeam);

                // don't try and exceed the amount of games stated
                if (++currentGameIndex == numberOfGames)
                    break;
            }

            return games;
        }
    }
}
