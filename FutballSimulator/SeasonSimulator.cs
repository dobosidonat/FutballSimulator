using System;
using System.Collections.Generic;
using System.IO;

namespace FutballSimulator
{
    /// <summary>
    /// A szezon szimulációjáért felelős osztály.
    /// </summary>
    public static class SeasonSimulator
    {
        /// <summary>
        /// A teljes szezon szimulációja, beleértve a fordulók és a tabella kezelését.
        /// </summary>
        /// <param name="teams">A szezonban szereplő csapatok listája.</param>
        /// <param name="fehervar">A Fehérvár FC csapat objektuma.</param>
        public static void SimulateSeason(List<Team> teams, Team fehervar)
        {
            // Betöltjük az aktuális szezon állapotát, ha van mentés
            var (table, currentRound) = LoadSeasonState();

            // Ha nincs mentett állapot, inicializáljuk a táblázatot
            if (currentRound == 0)
            {
                table = InitializeTable(teams, fehervar); // Új tabella inicializálása
            }

            for (int round = currentRound + 1; round <= 33; round++) // 33 forduló van az NB1-ben
            {
                Console.WriteLine($"\n{round}. forduló eredményei:");

                // Felállás kiválasztása
                var formation = ChooseFormation();

                // Forduló szimulálása
                SimulateRound(teams, fehervar, table, formation);

                // Tabella megjelenítése
                DisplayTable(table);

                // Tabella és szezon állapotának mentése
                SaveTableToFile(table, round);
                SaveSeasonState(table, round);

                Console.Write("\nSzeretnéd folytatni a következő fordulóval? (i/n): ");
                if (Console.ReadLine()?.ToLower() != "i")
                {
                    break; // Kilépünk, de az állapot mentve van, így később folytatható
                }
            }

            Console.WriteLine("\nA szezon véget ért!");
            Console.ReadKey();
        }


        /// <summary>
        /// Tabella inicializálása.
        /// </summary>
        private static Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)> InitializeTable(List<Team> teams, Team fehervar)
        {
            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)>();

            // Csapatok a fájlból
            foreach (var team in teams)
            {
                table[team.Name] = (0, 0, 0);
            }

            // Fehérvár hozzáadása
            table[fehervar.Name] = (0, 0, 0); //A Fehérvár FC alapból nem szerepel a beolvasott listában, mert önmagának nem ellenfele, de a table szótárba

            return table;
        }



        /// <summary>
        /// Egy forduló szimulálása és az eredmények frissítése.
        /// </summary>
        private static void SimulateRound(List<Team> teams, Team fehervar, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)> table, (int Defenders, int Midfielders, int Forwards) formation)
        {
            foreach (var homeTeam in teams)
            {
                foreach (var awayTeam in teams)
                {
                    if (homeTeam.Name != awayTeam.Name)
                    {
                        var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(homeTeam, awayTeam);
                        UpdateTable(table, homeTeam, awayTeam, homeGoals, awayGoals);
                    }
                }
            }

            // Fehérvár FC meccsének szimulálása az aktuális fordulóban
            Console.WriteLine("\n--- Fehérvár FC meccs ---");
            var opponent = SelectOpponent(teams, fehervar);
            var (fehervarGoals, opponentGoals) = MatchSimulator.SimulateMatch(fehervar, opponent);
            Console.WriteLine($"{fehervar.Name} {fehervarGoals} - {opponentGoals} {opponent.Name}");
            UpdateTable(table, fehervar, opponent, fehervarGoals, opponentGoals);
        }

        /// <summary>
        /// Tabella frissítése az eredmények alapján.
        /// </summary>
        private static void UpdateTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)> table, Team homeTeam, Team awayTeam, int homeGoals, int awayGoals)
        {
            if (homeGoals > awayGoals)
            {
                table[homeTeam.Name] = (table[homeTeam.Name].Points + 3, table[homeTeam.Name].GoalsFor + homeGoals, table[homeTeam.Name].GoalsAgainst + awayGoals);
            }
            else if (homeGoals < awayGoals)
            {
                table[awayTeam.Name] = (table[awayTeam.Name].Points + 3, table[awayTeam.Name].GoalsFor + awayGoals, table[awayTeam.Name].GoalsAgainst + homeGoals);
            }
            else
            {
                table[homeTeam.Name] = (table[homeTeam.Name].Points + 1, table[homeTeam.Name].GoalsFor + homeGoals, table[homeTeam.Name].GoalsAgainst + awayGoals);
                table[awayTeam.Name] = (table[awayTeam.Name].Points + 1, table[awayTeam.Name].GoalsFor + awayGoals, table[awayTeam.Name].GoalsAgainst + homeGoals);
            }
        }

        /// <summary>
        /// Tabella megjelenítése a konzolon.
        /// </summary>
        public static void DisplayTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)> table)
        {
            Console.WriteLine("\n--- Tabella ---");
            foreach (var kvp in table)
            {
                string teamName = kvp.Key;
                var stats = kvp.Value;
                Console.WriteLine($"{teamName}: {stats.Points} pont, Lőtt gól: {stats.GoalsFor}, Kapott gól: {stats.GoalsAgainst}");
            }
        }

        /// <summary>
        /// Tabella mentése fájlba.
        /// </summary>
        private static void SaveTableToFile(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)> table, int round)
        {
            string filePath = $"eredmenyek/tabella_round_{round}.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"--- Tabella - {round}. forduló ---");
                foreach (var kvp in table)
                {
                    string teamName = kvp.Key;
                    var stats = kvp.Value;
                    writer.WriteLine($"{teamName}: {stats.Points} pont, Lőtt gól: {stats.GoalsFor}, Kapott gól: {stats.GoalsAgainst}");
                }
            }
        }

        /// <summary>
        /// Szezon állapotának mentése.
        /// </summary>
        private static void SaveSeasonState(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)> table, int currentRound)
        {
            string filePath = "eredmenyek/season_state.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(currentRound);
                foreach (var kvp in table)
                {
                    writer.WriteLine($"{kvp.Key};{kvp.Value.Points};{kvp.Value.GoalsFor};{kvp.Value.GoalsAgainst}");
                }
            }
        }

        /// <summary>
        /// Szezon állapotának betöltése.
        /// </summary>
        private static (Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)>, int) LoadSeasonState()
        {
            string filePath = "eredmenyek/season_state.txt";
            if (!File.Exists(filePath))
            {
                return (new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)>(), 0);
            }

            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst)>();
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
                    table[teamName] = (points, goalsFor, goalsAgainst);
                }
            }

            return (table, round);
        }

        /// <summary>
        /// Ellenfél kiválasztása a Fehérvár FC számára.
        /// </summary>
        private static Team SelectOpponent(List<Team> teams, Team fehervar)
        {
            var random = new Random();
            Team opponent;
            do
            {
                opponent = teams[random.Next(teams.Count)];
            } while (opponent.Name == fehervar.Name); // Nem lehet saját magával játszani
            return opponent;
        }

        /// <summary>
        /// Felállás kiválasztása.
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
