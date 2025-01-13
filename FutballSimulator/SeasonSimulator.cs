using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FutballSimulator
{
    /// <summary>
    /// A bajnokság szimulációját kezeli.
    /// </summary>
    public static class SeasonSimulator
    {
        private static Dictionary<string, int> teamRatings;
        private static List<TableRow> leagueTable;

        /// <summary>
        /// A szezon szimulálása fordulóról fordulóra.
        /// </summary>
        public static void SimulateSeason()
        {
            LoadTeamRatings("csapat_ertekelesek.txt");
            InitializeLeagueTable();

            var teams = new List<string>(teamRatings.Keys);
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    var homeTeam = teams[i];
                    var awayTeam = teams[j];
                    var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(
                        new Team { Name = homeTeam, Players = new List<Player>() },
                        new Team { Name = awayTeam, Players = new List<Player>() }
                    );

                    Console.WriteLine($"{homeTeam} {homeGoals} - {awayGoals} {awayTeam}");
                    UpdateTable(homeTeam, awayTeam, homeGoals, awayGoals);
                    Console.WriteLine("Nyomj meg egy gombot a következő meccshez...");
                    Console.ReadKey();
                }
            }

            SaveTableToFile("eredmenyek/tabella.txt");
            Console.WriteLine("A szezon véget ért. Tabella mentve az eredmenyek/tabella.txt fájlba.");
            Console.ReadKey();
        }

        /// <summary>
        /// Csapatszintű értékelések betöltése fájlból.
        /// </summary>
        private static void LoadTeamRatings(string filePath)
        {
            teamRatings = new Dictionary<string, int>();
            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(';');
                if (parts.Length == 2 && int.TryParse(parts[1], out int rating))
                {
                    teamRatings[parts[0]] = rating;
                }
            }
        }

        /// <summary>
        /// Tabella inicializálása.
        /// </summary>
        private static void InitializeLeagueTable()
        {
            leagueTable = new List<TableRow>();
            foreach (var team in teamRatings.Keys)
            {
                leagueTable.Add(new TableRow { TeamName = team, Points = 0, GoalsFor = 0, GoalsAgainst = 0 });
            }
        }

        /// <summary>
        /// Tabella frissítése egy mérkőzés eredménye alapján.
        /// </summary>
        private static void UpdateTable(string homeTeam, string awayTeam, int homeGoals, int awayGoals)
        {
            var homeRow = leagueTable.Find(row => row.TeamName == homeTeam);
            var awayRow = leagueTable.Find(row => row.TeamName == awayTeam);

            homeRow.GoalsFor += homeGoals;
            homeRow.GoalsAgainst += awayGoals;

            awayRow.GoalsFor += awayGoals;
            awayRow.GoalsAgainst += homeGoals;

            if (homeGoals > awayGoals)
            {
                homeRow.Points += 3;
            }
            else if (homeGoals < awayGoals)
            {
                awayRow.Points += 3;
            }
            else
            {
                homeRow.Points += 1;
                awayRow.Points += 1;
            }
        }

        /// <summary>
        /// Tabella megjelenítése a konzolon.
        /// </summary>
        public static void DisplayTable()
        {
            Console.Clear();
            Console.WriteLine("===== NB I Tabella =====");
            Console.WriteLine("Helyezés | Csapat              | Pont | Lőtt | Kapott | Gólkülönbség");
            int rank = 1;
            foreach (var row in leagueTable.OrderByDescending(r => r.Points).ThenByDescending(r => r.GoalDifference))
            {
                Console.WriteLine($"{rank++,2}. {row.TeamName,-20} {row.Points,5} {row.GoalsFor,5} {row.GoalsAgainst,7} {row.GoalDifference,11}");
            }
            Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
            Console.ReadKey();
        }

        /// <summary>
        /// Tabella mentése fájlba.
        /// </summary>
        private static void SaveTableToFile(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Helyezés;Csapat;Pont;Lőtt gól;Kapott gól;Gólkülönbség");
                int rank = 1;
                foreach (var row in leagueTable.OrderByDescending(r => r.Points).ThenByDescending(r => r.GoalDifference))
                {
                    writer.WriteLine($"{rank++};{row.TeamName};{row.Points};{row.GoalsFor};{row.GoalsAgainst};{row.GoalDifference}");
                }
            }
        }
    }
}
