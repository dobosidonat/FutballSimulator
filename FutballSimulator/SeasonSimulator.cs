using System;
using System.Collections.Generic;

namespace FutballSimulator
{
    public static class SeasonSimulator
    {
        public static void SimulateSeason(List<string> teams)
        {
            var schedule = ScheduleHandler.GenerateSchedule(teams);
            var table = new Dictionary<string, Table>();

            int round = 1;

            foreach (var match in schedule)
            {
                Console.Clear();
                Console.WriteLine($"Forduló {round}: {match.homeTeam} vs {match.awayTeam}");

                var homeTeam = new Team
                {
                    Name = match.homeTeam,
                    Players = FileHandler.LoadPlayersFromFile($"Csapatok/{match.homeTeam.ToLower().Replace(" ", "_")}_players.txt")
                };

                var awayTeam = new Team
                {
                    Name = match.awayTeam,
                    Players = FileHandler.LoadPlayersFromFile($"Csapatok/{match.awayTeam.ToLower().Replace(" ", "_")}_players.txt")
                };

                var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(homeTeam, awayTeam);
                Console.WriteLine($"Eredmény: {match.homeTeam} {homeGoals} - {awayGoals} {match.awayTeam}");

                TableHandler.UpdateTable(table, match.homeTeam, homeGoals, match.awayTeam, awayGoals);
                TableHandler.DisplayTable(table);

                TableHandler.SaveTable(table, round);

                round++;
                Console.ReadKey();
            }
        }
    }
}
