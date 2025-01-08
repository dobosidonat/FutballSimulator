using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FutballSimulator
{
    public static class TransferHandler
    {
        public static void HandleTransfers()
        {
            Console.WriteLine("\nVálassz egy költségvetési tesztesetet:");
            Console.WriteLine("1. 20 millió Ft");
            Console.WriteLine("2. 40 millió Ft");
            Console.WriteLine("3. 60 millió Ft");
            Console.WriteLine("4. 80 millió Ft");
            Console.WriteLine("5. 100 millió Ft");

            Console.Write("Add meg a választásod: ");
            string budgetChoice = Console.ReadLine();
            string testFile = $"budget{budgetChoice}.txt";

            if (!File.Exists(testFile))
            {
                Console.WriteLine($"Hiba: A költségvetési fájl nem található: {testFile}");
                Console.ReadKey();
                return;
            }

            RunTestCase(testFile);
        }

        private static void RunTestCase(string testFile)
        {
            try
            {
                var fehervarPlayers = FileHandler.LoadPlayersFromFile("fehervar_players.txt");
                var fehervar = new Team
                {
                    Name = "Fehérvár FC",
                    Budget = BudgetHandler.LoadBudgetFromFile(testFile),
                    Players = fehervarPlayers
                };

                Console.WriteLine("Kezdő Fehérvár FC:");
                TeamEvaluator.EvaluateTeamPositions(fehervar);

                var transferMarket = FileHandler.LoadPlayersFromFile("atigazolasi_piac.txt");
                var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget, fehervar.Players);

                if (bestTransfers.Count == 0)
                {
                    Console.WriteLine("\nNem történt érdemi igazolás.");
                }
                else
                {
                    Console.WriteLine("\nIgazolt játékosok:");
                    foreach (var player in bestTransfers)
                    {
                        Console.WriteLine($"{player.Name} ({player.Position}), Értékelés: {player.Rating}");
                        fehervar.AddPlayer(player);
                    }

                    TeamEvaluator.EvaluateTeamPositions(fehervar);
                }

                SaveUpdatedTeam(fehervar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba: {ex.Message}");
            }
            Console.ReadKey();
        }

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
                    if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".txt";
                    }

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
