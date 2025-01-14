using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FutballSimulator
{
    public static class SeasonSimulator
    {
        /// <summary>
        /// Manuális szezon szimuláció.
        /// </summary>
        /// <param name="teams">A szezonban szereplő csapatok listája.</param>
        /// <param name="fehervar">Fehérvár FC csapat objektuma.</param>
        public static void SimulateSeason(List<Team> teams, Team fehervar)
        {
            // Tabella inicializálása vagy betöltése
            var table = InitializeTable(teams, fehervar);
            int currentRound = 0;

            // Szezon párosításainak generálása
            var matchups = GenerateSeasonMatchups(teams, fehervar);

            // Fordulók szimulálása
            for (int round = currentRound + 1; round <= 33; round++)
            {
                Console.WriteLine($"\n{round}. forduló eredményei:");

                // Felállás kiválasztása
                var formation = ChooseFormation();

                // Forduló szimulálása
                SimulateRound(matchups[round - 1], table, formation);

                // Tabella megjelenítése és mentése
                DisplayTable(table);
                SaveRoundResults(matchups[round - 1], table, round, "manual");
                SaveTableToFile(table, round, "manual");
                SaveSeasonState(table, round);

                // Kérdés a következő forduló folytatásáról
                Console.Write("\nSzeretnéd folytatni a következő fordulóval? (i/n): ");
                if (Console.ReadLine()?.ToLower() != "i")
                {
                    break;
                }
            }

            Console.WriteLine("\nA szezon mentésre került. Később folytathatja!");
        }

        /// <summary>
        /// Tabella inicializálása.
        /// </summary>
        private static Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> InitializeTable(List<Team> teams, Team fehervar)
        {
            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)>();

            // Minden csapat kezdő adatainak inicializálása
            foreach (var team in teams)
            {
                table[team.Name] = (0, 0, 0, 0);
            }
            table[fehervar.Name] = (0, 0, 0, 0); // Fehérvár hozzáadása

            return table;
        }

        /// <summary>
        /// Szezon párosításainak generálása.
        /// </summary>
        private static List<List<(Team Home, Team Away)>> GenerateSeasonMatchups(List<Team> teams, Team fehervar)
        {
            var allTeams = new List<Team>(teams) { fehervar }; // Fehérvár hozzáadása
            var matchups = new List<List<(Team Home, Team Away)>>();

            for (int cycle = 0; cycle < 3; cycle++) // Három kör a szezonban
            {
                var roundRobinRounds = RoundRobin(allTeams, cycle % 2 == 0); // RoundRobin generálás
                matchups.AddRange(roundRobinRounds);
            }

            return matchups;
        }

        /// <summary>
        /// Round Robin algoritmus, amely az aktuális párosításokat generálja.
        /// </summary>
        private static List<List<(Team Home, Team Away)>> RoundRobin(List<Team> teams, bool homeFirst)
        {
            var rounds = new List<List<(Team Home, Team Away)>>();
            var teamCount = teams.Count;

            // Pihenő csapat hozzáadása, ha páratlan a létszám
            if (teamCount % 2 != 0)
            {
                teams.Add(new Team { Name = "Pihenő" });
                teamCount++;
            }

            // Fordulók generálása
            for (int round = 0; round < teamCount - 1; round++)
            {
                var roundMatchups = new List<(Team Home, Team Away)>();

                for (int i = 0; i < teamCount / 2; i++)
                {
                    var home = teams[i];
                    var away = teams[teamCount - 1 - i];

                    roundMatchups.Add(homeFirst ? (home, away) : (away, home));
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
        /// Forduló szimulálása az adott párosításokkal.
        /// </summary>
        private static void SimulateRound(List<(Team Home, Team Away)> roundMatchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, (int Defenders, int Midfielders, int Forwards) formation)
        {
            var random = new Random();

            foreach (var (home, away) in roundMatchups)
            {
                var homeGoals = random.Next(0, 5); // Hazai gólok
                var awayGoals = random.Next(0, 5); // Vendég gólok
                Console.WriteLine($"{home.Name} {homeGoals} - {awayGoals} {away.Name}");
                UpdateTable(table, home, away, homeGoals, awayGoals);
            }
        }

        /// <summary>
        /// Tabella frissítése a meccseredmények alapján.
        /// </summary>
        private static void UpdateTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, Team home, Team away, int homeGoals, int awayGoals)
        {
            // Győzelem, vereség vagy döntetlen esetén a megfelelő adatok frissítése
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
        private static void SaveRoundResults(List<(Team Home, Team Away)> matchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, int round, string keretNev)
        {
            string resultsFile = $"eredmenyek/automatikus_szezon_eredmenyek_{keretNev}.txt";

            using (StreamWriter writer = new StreamWriter(resultsFile, true))
            {
                writer.WriteLine($"--- {round}. forduló eredményei ---");
                foreach (var (home, away) in matchups)
                {
                    writer.WriteLine($"{home.Name} - {away.Name}");
                }
                writer.WriteLine();
            }

            Console.WriteLine($"Forduló eredményei mentve: {resultsFile}");
        }

        /// <summary>
        /// Tabella mentése fájlba fordulónként.
        /// </summary>
        private static void SaveTableToFile(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches)> table, int round, string keretNev)
        {
            string filePath = $"eredmenyek/automatikus_tabella_{keretNev}.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
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

            Console.WriteLine($"Tabella mentve: {filePath}");
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

            Console.WriteLine($"Szezon állapota mentve: {filePath}");
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

            switch (choice)
            {
                case 1:
                    return (4, 4, 2);
                case 2:
                    return (4, 3, 3);
                case 3:
                    return (3, 5, 2);
                case 4:
                    return (5, 3, 2);
                default:
                    return (4, 4, 2); // Alapértelmezett felállás
            }

        }
        /// <summary>
        /// Automatikus szezon szimuláció az összes forduló végigjátszásával, kiválasztott kerettel.
        /// Az eredmények és a tabella mentésre kerül fájlokba.
        /// </summary>
        /// <param name="teams">A szezonban szereplő csapatok listája.</param>
        /// <param name="fehervar">A Fehérvár FC csapat objektuma.</param>
        public static void SimulateFullSeasonAutomatically(List<Team> teams, Team fehervar)
        {
            // Keretfájlok betöltése
            var keretFiles = Directory.GetFiles("keretek", "*.txt").ToList();

            Console.WriteLine("\nVálassz egy keretet:");
            for (int i = 0; i < keretFiles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(keretFiles[i])}");
            }

            // Keret kiválasztása
            Console.Write("Keret száma: ");
            int keretChoice = int.Parse(Console.ReadLine()) - 1;

            // Érvénytelen választás esetén alapértelmezett keret betöltése
            if (keretChoice < 0 || keretChoice >= keretFiles.Count)
            {
                Console.WriteLine("Érvénytelen választás. Alapértelmezett keret kerül betöltésre.");
                keretChoice = 0;
            }

            // Kiválasztott keret betöltése
            var selectedKeret = FileHandler.LoadPlayersFromFile(keretFiles[keretChoice]);
            fehervar.Players = selectedKeret;

            Console.Clear();
            Console.WriteLine($"Kiválasztott keret: {Path.GetFileNameWithoutExtension(keretFiles[keretChoice])}");

            // Tabella inicializálása
            var table = InitializeTable(teams, fehervar);
            var matchups = GenerateSeasonMatchups(teams, fehervar); // Szezon párosításainak generálása

            // Fordulók szimulálása
            for (int round = 1; round <= 33; round++)
            {
                Console.WriteLine($"\n{round}. forduló eredményei:");

                // Felállás kiválasztása minden fordulóhoz
                var formation = ChooseFormation();

                // Forduló szimulálása
                SimulateRound(matchups[round - 1], table, formation);

                // Eredmények mentése
                SaveRoundResults(matchups[round - 1], table, round, Path.GetFileNameWithoutExtension(keretFiles[keretChoice]));
                SaveTableToFile(table, round, Path.GetFileNameWithoutExtension(keretFiles[keretChoice]));
            }

            Console.WriteLine("\nAz automatikus szezon szimuláció véget ért!");
        }

    }
}
