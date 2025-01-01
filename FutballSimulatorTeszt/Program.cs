using System;
using FutballSimulator;

class Program
{
    static void Main(string[] args)
    {
        // Fehérvár FC játékosok betöltése fájlból
        var fehervarPlayers = FileHandler.LoadPlayersFromFile("fehervar_players.txt");

        // Fehérvár FC csapat létrehozása
        var fehervar = new Team
        {
            Name = "Fehérvár FC",
            Budget = 2000000,
            Players = fehervarPlayers
        };

        Console.WriteLine("Fehérvár FC kezdő állapota:");
        Console.WriteLine(fehervar);

        // Átigazolási piac betöltése
        var transferMarket = FileHandler.LoadPlayersFromFile("transfer_market.txt");

        // Optimalizált igazolások
        var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget);
        foreach (var player in bestTransfers)
        {
            fehervar.AddPlayer(player);
        }

        Console.WriteLine("\nFehérvár FC állapota az igazolások után:");
        Console.WriteLine(fehervar);

        Console.WriteLine("\nIgazolt játékosok:");
        foreach (var player in bestTransfers)
        {
            Console.WriteLine(player);
        }
    }
}
