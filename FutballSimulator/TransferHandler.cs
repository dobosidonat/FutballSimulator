using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FutballSimulator
{
    /// <summary>
    /// Átigazolások kezeléséért felelős osztály.
    /// </summary>
    public static class TransferHandler
    {
        /// <summary>
        /// Átigazolási folyamat indítása, költségvetés kiválasztása alapján.
        /// </summary>
        public static void HandleTransfers()
        {
            Console.WriteLine("\nVálassz egy költségvetési tesztesetet:");
            Console.WriteLine("1. 500 ezer Ft");
            Console.WriteLine("2. 1 millió Ft");
            Console.WriteLine("3. 1,5 millió Ft");
            Console.WriteLine("4. 2 millió Ft");
            Console.WriteLine("5. 2,5 millió Ft");
            Console.WriteLine("6. 10 millió Ft");

            Console.Write("Add meg a választásod: ");
            string budgetChoice = Console.ReadLine();
            string testFile = $"budgets/budget{budgetChoice}.txt";

            // Ellenőrizzük, hogy létezik-e a kiválasztott fájl
            if (!File.Exists(testFile))
            {
                Console.WriteLine($"Hiba: A költségvetési fájl nem található: {testFile}");
                Console.ReadKey();
                return;
            }

            // Teszteset futtatása
            RunTestCase(testFile);
        }

        /// <summary>
        /// Teszteset futtatása, amely optimalizálja az igazolásokat a megadott költségvetés alapján.
        /// </summary>
        static void RunTestCase(string testFile)
        {
            try
            {
                // Fehérvár FC keretének betöltése
                var fehervarPlayers = FileHandler.LoadPlayersFromFile("keretek/fehervar_players.txt");
                var fehervar = new Team
                {
                    Name = "Fehérvár FC",
                    Budget = BudgetHandler.LoadBudgetFromFile(testFile),
                    Players = fehervarPlayers
                };

                // Jelenlegi csapat posztonkénti átlagainak kiírása
                Console.WriteLine("\nEredeti Fehérvár FC keret posztonkénti átlagai:");
                DisplayPositionAverages(fehervar);

                // Átigazolási piac betöltése
                var transferMarket = FileHandler.LoadPlayersFromFile("atigazolasi_piac.txt");

                // Javulási tűréshatár meghatározása a költségvetés alapján
                double improvementThreshold = TransferOptimizer.GetImprovementThreshold(fehervar.Budget);

                // Optimális igazolások meghatározása
                var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget, fehervar.Players, improvementThreshold);

                Console.WriteLine("\nIgazolt játékosok:");
                foreach (var player in bestTransfers)
                {
                    Console.WriteLine($"- {player.Name} ({player.Position}), Értékelés: {player.Rating}, Piaci érték: {player.MarketValue} Ft");
                    fehervar.AddPlayer(player);
                }

                // Frissített csapat posztonkénti átlagainak kiírása
                Console.WriteLine("\nFehérvár FC keret az igazolások után, posztonkénti átlagok:");
                DisplayPositionAverages(fehervar);

                // Frissített keret mentése
                SaveUpdatedTeam(fehervar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt: {ex.Message}");
            }
            Console.ReadKey();
        }


        /// <summary>
        /// Frissített csapat keretének mentése fájlba a "keretek" mappába.
        /// </summary>
        private static void SaveUpdatedTeam(Team team)
        {
            Console.Write("\nSzeretnéd elmenteni a keretet? (i/n): ");
            string saveResponse = Console.ReadLine()?.ToLower();

            if (saveResponse == "i")
            {
                Console.Write("\nAdd meg a fájl nevét (pl. ujkeret1.txt): ");
                string fileName;
                string fullPath; // Definiáljuk itt a változót, hogy elérhető legyen a ciklus után is.

                while (true)
                {
                    fileName = Console.ReadLine();

                    // Automatikusan hozzáadjuk a .txt kiterjesztést, ha hiányzik
                    if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".txt";
                    }

                    // "keretek" mappa útvonalának létrehozása
                    string directory = "keretek";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory); // Mappa létrehozása, ha nem létezik
                    }

                    // Teljes fájl elérési útvonal összeállítása
                    fullPath = Path.Combine(directory, fileName);

                    // Ellenőrizzük, hogy létezik-e a fájl
                    if (File.Exists(fullPath))
                    {
                        Console.WriteLine($"A '{fileName}' fájl már létezik a 'keretek' mappában. Adj meg egy másik nevet:");
                    }
                    else
                    {
                        break;
                    }
                }

                // Fájl mentése a "keretek" mappába
                FileHandler.SavePlayersToFile(team.Players, fullPath);
                Console.WriteLine($"A keret mentve: {fullPath}");
            }
        }

        /// <summary>
        /// Csapat posztonkénti átlagainak kiírása.
        /// </summary>
        private static void DisplayPositionAverages(Team team)
        {
            var positions = new[] { "GK", "DF", "MF", "FW" }; // Kapus, védő, középpályás, csatár

            foreach (var position in positions)
            {
                var playersInPosition = team.Players.Where(p => p.Position == position).ToList();
                double averageRating = playersInPosition.Any() ? playersInPosition.Average(p => p.Rating) : 0.0;

                Console.WriteLine($"{position}: Átlagos értékelés: {averageRating:F2}");
            }
        }


    }
}
