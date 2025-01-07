using FootballManagerLibrary;
using FutballSimulator;
using System;

class Program
{
    static void Main(string[] args)
    {
        string[] testFiles = { "teszt1_budget.txt", "teszt2_budget.txt", "teszt3_budget.txt", "teszt4_budget.txt" };

        foreach (var testFile in testFiles)
        {
            Console.WriteLine($"\n--- Teszt: {testFile} ---");

            // Fehérvár FC játékosok betöltése
            var fehervarPlayers = FileHandler.LoadPlayersFromFile("fehervar_players.txt");
            var fehervar = new Team
            {
                Name = "Fehérvár FC",
                Budget = BudgetHandler.LoadBudgetFromFile(testFile),
                Players = fehervarPlayers
            };

            Console.WriteLine("Kezdő Fehérvár FC:");
            Console.WriteLine(fehervar);

            // Átigazolási piac betöltése
            var transferMarket = FileHandler.LoadPlayersFromFile("transfer_market.txt");

            // Optimalizált igazolások
            var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget);
            foreach (var player in bestTransfers)
            {
                fehervar.AddPlayer(player);
            }

            Console.WriteLine("\nFehérvár FC az igazolások után:");
            Console.WriteLine(fehervar);
        }
    }
}
