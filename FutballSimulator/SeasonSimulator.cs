using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FutballSimulator
{
    public static class SeasonSimulator
    {
        /// <summary>
        /// A szezon szimulációjának fő metódusa.
        /// </summary>
        public static void SimulateSeason(List<Team> teams, Team fehervar)
        {
            // Tabella inicializálása
            Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table;
            int currentRound;

            // Szezon állapotának betöltése, ha van mentés
            (table, currentRound) = LoadSeasonState();

            // Ha nincs mentett állapot, új tabella inicializálása
            if (currentRound == 0)
            {
                table = InitializeTable(teams, fehervar);
            }

            // Párosítások generálása a szezonra (33 forduló)
            var matchups = GenerateSeasonMatchups(teams, fehervar);

            // Fordulók szimulálása
            for (int round = currentRound + 1; round <= 33; round++)
            {
                Console.WriteLine($"\n{round}. forduló eredményei:");

                // Felállás kiválasztása a szimulációhoz
                var formation = ChooseFormation();

                // Aktuális forduló szimulálása
                SimulateRound(matchups[round - 1], table, formation);

                // Tabella kiírása a konzolra
                DisplayTable(table);

                // Forduló eredményeinek mentése
                SaveRoundResults(matchups[round - 1], table, round);

                // Tabella mentése fájlba
                SaveTableToFile(table, round);

                // Szezon állapotának mentése
                SaveSeasonState(table, round);

                // Kérdés a következő forduló folytatásáról
                Console.Write("\nSzeretnéd folytatni a következő fordulóval? (i/n): ");
                if (Console.ReadLine()?.ToLower() != "i")
                {
                    break;
                }
            }

            Console.WriteLine("\nA szezon mentésre került. Később folytathatja!");
            Console.ReadKey();
        }

        /// <summary>
        /// Tabella inicializálása az összes csapat számára.
        /// </summary>
        private static Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> InitializeTable(List<Team> teams, Team fehervar)
        {
            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)>();

            // Minden csapat kezdő adatai (0 pont, 0 gól)
            foreach (var team in teams)
            {
                table[team.Name] = (0, 0, 0, 0);
            }

            // Fehérvár hozzáadása a táblához
            table[fehervar.Name] = (0, 0, 0, 0);
            return table;
        }

        /// <summary>
        /// Szezon párosításainak generálása Round Robin logikával.
        /// </summary>
        private static List<List<(Team Home, Team Away)>> GenerateSeasonMatchups(List<Team> teams, Team fehervar)
        {
            var allTeams = new List<Team>(teams) { fehervar }; // Fehérvár hozzáadása
            var matchups = new List<List<(Team Home, Team Away)>>();

            for (int cycle = 0; cycle < 3; cycle++) // Háromszoros kör
            {
                var roundRobinRounds = RoundRobin(allTeams, cycle % 2 == 0); // RoundRobin hívása
                matchups.AddRange(roundRobinRounds);
            }

            return matchups;
        }


        /// <summary>
        /// Round Robin logika egy teljes kör generálásához.
        /// </summary>
        private static List<List<(Team Home, Team Away)>> RoundRobin(List<Team> teams, bool homeFirst)
        {
            var rounds = new List<List<(Team Home, Team Away)>>();
            var teamCount = teams.Count;

            // Ha páratlan a csapatok száma, dummy csapat hozzáadása
            if (teamCount % 2 != 0)
            {
                teams.Add(new Team { Name = "Pihenő" });
                teamCount++;
            }

            for (int round = 0; round < teamCount - 1; round++)
            {
                var roundMatchups = new List<(Team Home, Team Away)>();
                for (int i = 0; i < teamCount / 2; i++)
                {
                    var home = teams[i];
                    var away = teams[teamCount - 1 - i];

                    if (homeFirst)
                    {
                        roundMatchups.Add((home, away));
                    }
                    else
                    {
                        roundMatchups.Add((away, home));
                    }
                }

                rounds.Add(roundMatchups);

                // Csapatok rotálása
                var lastTeam = teams[teamCount - 1];
                teams.RemoveAt(teamCount - 1);
                teams.Insert(1, lastTeam);
            }

            return rounds;
        }


        /// <summary>
        /// Egy forduló szimulálása.
        /// </summary>
        private static void SimulateRound(List<(Team Home, Team Away)> roundMatchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, (int Defenders, int Midfielders, int Forwards) formation)
        {
            var random = new Random();

            // Minden meccs szimulálása az aktuális fordulóban
            foreach (var (home, away) in roundMatchups)
            {
                var homeGoals = random.Next(0, 5);
                var awayGoals = random.Next(0, 5);
                Console.WriteLine($"{home.Name} {homeGoals} - {awayGoals} {away.Name}");
                UpdateTable(table, home, away, homeGoals, awayGoals);
            }
        }

        /// <summary>
        /// Tabella frissítése a meccseredmények alapján.
        /// </summary>
        private static void UpdateTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, Team home, Team away, int homeGoals, int awayGoals)
        {
            // Győzelem, döntetlen vagy vereség esetén a pontok és statisztikák frissítése
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

        /// <summary>
        /// Tabella megjelenítése a konzolon.
        /// </summary>
        private static void DisplayTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table)
        {
            Console.WriteLine("\n--- Tabella ---");
            Console.WriteLine("Helyezés | Csapat            | LM | LG | KG | GK | PSZ");
            Console.WriteLine(new string('-', 50));

            var sortedTable = table.OrderByDescending(t => t.Value.Points)
                                   .ThenByDescending(t => t.Value.GoalsFor - t.Value.GoalsAgainst)
                                   .ThenByDescending(t => t.Value.GoalsFor)
                                   .ToList();

            int rank = 1;
            foreach (var kvp in sortedTable)
            {
                string teamName = kvp.Key;
                var stats = kvp.Value;
                int goalDifference = stats.GoalsFor - stats.GoalsAgainst;

                Console.WriteLine($"{rank,8} | {teamName,-16} | {stats.PlayedMatches,2} | {stats.GoalsFor,2} | {stats.GoalsAgainst,2} | {goalDifference,3} | {stats.Points,3}");
                rank++;
            }
        }

        /// <summary>
        /// Forduló eredményeinek mentése fájlba.
        /// </summary>
        private static void SaveRoundResults(List<(Team Home, Team Away)> matchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, int round)
        {
            string resultsFile = "eredmenyek/fordulo_eredmenyek.txt";

            using (StreamWriter writer = new StreamWriter(resultsFile, true))
            {
                writer.WriteLine($"--- {round}. forduló eredményei ---");
                foreach (var (home, away) in matchups)
                {
                    var homeGoals = table[home.Name].GoalsFor - table[home.Name].GoalsAgainst;
                    var awayGoals = table[away.Name].GoalsFor - table[away.Name].GoalsAgainst;

                    writer.WriteLine($"{home.Name} {homeGoals} - {awayGoals} {away.Name}");
                }
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Tabella mentése fájlba.
        /// </summary>
        private static void SaveTableToFile(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, int round)
        {
            string filePath = "eredmenyek/tabella.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"--- Tabella - {round}. forduló ---");
                writer.WriteLine("Helyezés | Csapat            | LM | LG | KG | GK | PSZ");
                writer.WriteLine(new string('-', 50));

                var sortedTable = table.OrderByDescending(t => t.Value.Points)
                                       .ThenByDescending(t => t.Value.GoalsFor - t.Value.GoalsAgainst)
                                       .ThenByDescending(t => t.Value.GoalsFor)
                                       .ToList();

                int rank = 1;
                foreach (var kvp in sortedTable)
                {
                    string teamName = kvp.Key;
                    var stats = kvp.Value;
                    int goalDifference = stats.GoalsFor - stats.GoalsAgainst;

                    writer.WriteLine($"{rank,8} | {teamName,-16} | {stats.PlayedMatches,2} | {stats.GoalsFor,2} | {stats.GoalsAgainst,2} | {goalDifference,3} | {stats.Points,3}");
                    rank++;
                }
            }

            Console.WriteLine($"\nTabella mentve a következő fájlba: {filePath}");
        }

        /// <summary>
        /// Szezon állapotának mentése fájlba.
        /// </summary>
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

        /// <summary>
        /// Szezon állapotának betöltése fájlból.
        /// </summary>
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
                    int playedMatches = int.Parse(line[4]);

                    table[teamName] = (points, goalsFor, goalsAgainst, playedMatches);
                }
            }

            return (table, round);
        }

        /// <summary>
        /// Felállás kiválasztása a szimulációhoz.
        /// </summary>
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
