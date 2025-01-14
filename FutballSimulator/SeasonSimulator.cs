using System;
using System.Collections.Generic;
using System.IO;

namespace FutballSimulator
{
    public static class SeasonSimulator
    {
        public static void SimulateSeason(List<Team> teams, Team fehervar)
        {
            // Tabella inicializálása
            Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table;
            int currentRound;

            (table, currentRound) = LoadSeasonState();

            if (currentRound == 0)
            {
                table = InitializeTable(teams, fehervar);
            }

            // Párosítások generálása a szezonra
            var matchups = GenerateSeasonMatchups(teams, fehervar);

            for (int round = currentRound + 1; round <= 33; round++)
            {
                Console.WriteLine($"\n{round}. forduló eredményei:");

                var formation = ChooseFormation();
                SimulateRound(matchups[round - 1], table, formation);

                DisplayTable(table);
                SaveRoundResults(matchups[round - 1], table, round);
                SaveSeasonState(table, round);

                Console.Write("\nSzeretnéd folytatni a következő fordulóval? (i/n): ");
                if (Console.ReadLine()?.ToLower() != "i")
                {
                    break;
                }
            }

            Console.WriteLine("\nA szezon mentésre került. Később folytathatja!");
            Console.ReadKey();
        }

        private static Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> InitializeTable(List<Team> teams, Team fehervar)
        {
            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)>();

            foreach (var team in teams)
            {
                table[team.Name] = (0, 0, 0, 0);
            }

            table[fehervar.Name] = (0, 0, 0, 0);
            return table;
        }

        private static List<List<(Team Home, Team Away)>> GenerateSeasonMatchups(List<Team> teams, Team fehervar)
        {
            var allTeams = new List<Team>(teams) { fehervar };
            var matchups = new List<List<(Team Home, Team Away)>>();

            for (int cycle = 0; cycle < 3; cycle++) // Háromszor játszik minden csapat
            {
                var roundMatchups = new List<(Team Home, Team Away)>();
                var remainingTeams = new List<Team>(allTeams);

                while (remainingTeams.Count > 1)
                {
                    var home = remainingTeams[0];
                    var away = remainingTeams[1];

                    if (cycle % 2 == 0)
                    {
                        roundMatchups.Add((home, away));
                    }
                    else
                    {
                        roundMatchups.Add((away, home));
                    }

                    remainingTeams.RemoveAt(0);
                    remainingTeams.RemoveAt(0);
                }

                matchups.Add(roundMatchups);
            }

            return matchups;
        }

        private static void SimulateRound(List<(Team Home, Team Away)> roundMatchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, (int Defenders, int Midfielders, int Forwards) formation)
        {
            var random = new Random();

            foreach (var (home, away) in roundMatchups)
            {
                var homeGoals = random.Next(0, 5); // 0-4 gól a hazai csapatnak
                var awayGoals = random.Next(0, 5); // 0-4 gól a vendégcsapatnak
                Console.WriteLine($"{home.Name} {homeGoals} - {awayGoals} {away.Name}");
                UpdateTable(table, home, away, homeGoals, awayGoals);
            }
        }

        private static void UpdateTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, Team home, Team away, int homeGoals, int awayGoals)
        {
            if (homeGoals > awayGoals)
            {
                table[home.Name] = (table[home.Name].Points + 3, table[home.Name].GoalsFor + homeGoals, table[home.Name].GoalsAgainst + awayGoals, table[home.Name].PlayedMatches + 1);
                table[away.Name] = (table[away.Name].Points, table[away.Name].GoalsFor + awayGoals, table[away.Name].GoalsAgainst + homeGoals, table[away.Name].PlayedMatches + 1);
            }
            else if (homeGoals < awayGoals)
            {
                table[away.Name] = (table[away.Name].Points + 3, table[away.Name].GoalsFor + awayGoals, table[away.Name].GoalsAgainst + homeGoals, table[away.Name].PlayedMatches + 1);
                table[home.Name] = (table[home.Name].Points, table[home.Name].GoalsFor + homeGoals, table[home.Name].GoalsAgainst + awayGoals, table[home.Name].PlayedMatches + 1);
            }
            else
            {
                table[home.Name] = (table[home.Name].Points + 1, table[home.Name].GoalsFor + homeGoals, table[home.Name].GoalsAgainst + awayGoals, table[home.Name].PlayedMatches + 1);
                table[away.Name] = (table[away.Name].Points + 1, table[away.Name].GoalsFor + awayGoals, table[away.Name].GoalsAgainst + homeGoals, table[away.Name].PlayedMatches + 1);
            }
        }

        private static void DisplayTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table)
        {
            Console.WriteLine("\n--- Tabella ---");
            foreach (var kvp in table)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Points} pont, Lőtt gól: {kvp.Value.GoalsFor}, Kapott gól: {kvp.Value.GoalsAgainst}, Meccsek: {kvp.Value.PlayedMatches}");
            }
        }

        private static void SaveRoundResults(List<(Team Home, Team Away)> matchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, int round)
        {
            string resultsFile = "eredmenyek/fordulo_eredmenyek.txt";
            string tableFile = "eredmenyek/tabella.txt";

            using (StreamWriter writer = new StreamWriter(resultsFile, true))
            {
                writer.WriteLine($"--- {round}. forduló eredményei ---");
                foreach (var (home, away) in matchups)
                {
                    writer.WriteLine($"{home.Name} - {away.Name}");
                }
            }

            using (StreamWriter writer = new StreamWriter(tableFile))
            {
                writer.WriteLine("--- Tabella ---");
                foreach (var kvp in table)
                {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value.Points} pont, Lőtt gól: {kvp.Value.GoalsFor}, Kapott gól: {kvp.Value.GoalsAgainst}, Meccsek: {kvp.Value.PlayedMatches}");
                }
            }
        }

        private static void SaveSeasonState(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, int currentRound)
        {
            string filePath = "eredmenyek/season_state.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(currentRound);

                foreach (var kvp in table)
                {
                    writer.WriteLine($"{kvp.Key};{kvp.Value.Points};{kvp.Value.GoalsFor};{kvp.Value.GoalsAgainst};{kvp.Value.PlayedMatches}");
                }
            }

            Console.WriteLine($"\nA szezon állapota mentésre került a következő fájlba: {filePath}");
        }

        private static (Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)>, int) LoadSeasonState()
        {
            string filePath = "eredmenyek/season_state.txt";
            if (!File.Exists(filePath))
            {
                return (new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)>(), 0);
            }

            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)>();
            int round = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                round = int.Parse(reader.ReadLine());
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(';');
                    string teamName = line[0];
                    int points = int.Parse(line[1]);
                    int goalsFor = int.Parse(line[2]);
                    int goalsAgainst = int.Parse(line[3]);
                    int playedMatches = line.Length > 4 ? int.Parse(line[4]) : 0; // Ha nincs meg az ötödik adat, alapértelmezett 0

                    table[teamName] = (points, goalsFor, goalsAgainst, playedMatches);
                }
            }


            return (table, round);
        }

        private static (int Defenders, int Midfielders, int Forwards) ChooseFormation()
        {
            Console.WriteLine("\nVálassz egy felállást a következők közül:");
            Console.WriteLine("1. 4-4-2");
            Console.WriteLine("2. 4-3-3");
            Console.WriteLine("3. 3-5-2");
            Console.WriteLine("4. 5-3-2");

            Console.Write("Választásod: ");
            int choice = int.Parse(Console.ReadLine());

            (int Defenders, int Midfielders, int Forwards) formation;

            switch (choice)
            {
                case 1:
                    formation = (4, 4, 2);
                    break;
                case 2:
                    formation = (4, 3, 3);
                    break;
                case 3:
                    formation = (3, 5, 2);
                    break;
                case 4:
                    formation = (5, 3, 2);
                    break;
                default:
                    formation = (4, 4, 2); // Alapértelmezett felállás
                    break;
            }

            return formation;
        }
    }
}
