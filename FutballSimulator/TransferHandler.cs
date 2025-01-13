using System;
using System.Collections.Generic;
using System.IO;

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

                // Csapatrészek értékelése
                Console.WriteLine("Kezdő Fehérvár FC:");
                var (defense, midfield, forward, goalkeeper) = TeamEvaluator.EvaluateTeamRatings(fehervar);
                Console.WriteLine($"Védelem: {defense:F1}, Középpálya: {midfield:F1}, Támadósor: {forward:F1}, Kapus: {goalkeeper:F1}");

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

                // Csapat kiértékelése az igazolások után
                Console.WriteLine("\nFehérvár FC az igazolások után:");
                var (updatedDefense, updatedMidfield, updatedForward, updatedGoalkeeper) = TeamEvaluator.EvaluateTeamRatings(fehervar);
                Console.WriteLine($"Védelem: {updatedDefense:F1}, Középpálya: {updatedMidfield:F1}, Támadósor: {updatedForward:F1}, Kapus: {updatedGoalkeeper:F1}");

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
        /// Frissített csapat keretének mentése fájlba.
        /// </summary>
        private static void SaveUpdatedTeam(Team team)
        {
            Console.Write("\nSzeretnéd elmenteni a keretet? (i/n): ");
            string saveResponse = Console.ReadLine()?.ToLower();

            if (saveResponse == "i")
            {
                Console.Write("\nAdd meg a fájl nevét (pl. ujkeret1.txt): ");
                string fileName;
                while (true)
                {
                    fileName = Console.ReadLine();
                    // Automatikusan hozzáadjuk a .txt kiterjesztést, ha hiányzik
                    if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".txt";
                    }

                    // Ellenőrizzük, hogy létezik-e a fájl
                    if (File.Exists(fileName))
                    {
                        Console.WriteLine($"A '{fileName}' fájl már létezik. Adj meg egy másik nevet:");
                    }
                    else
                    {
                        break;
                    }
                }
                FileHandler.SavePlayersToFile(team.Players, fileName);
                Console.WriteLine($"A keret mentve: {fileName}");
            }
        }
    }
}
