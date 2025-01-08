using FootballManagerLibrary;
using FutballSimulator;
using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        while (true) // Folyamatos futás, amíg a felhasználó nem választja a kilépést
        {
            Console.Clear();
            Console.WriteLine("===== Fehérvár FC Menedzser Program =====");
            Console.WriteLine("Válassz a következő lehetőségek közül:");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Meccsszimuláció");
            Console.WriteLine("0. Kilépés");

            Console.Write("Add meg a választásod: ");
            string input = Console.ReadLine();

            if (input == "0")
            {
                Console.WriteLine("Kilépés...");
                break;
            }

            switch (input)
            {
                case "1":
                    HandleTransfers();
                    break;
                case "2":
                    SimulateMatchMenu();
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Igazolások végrehajtása a felhasználó által választott költségvetéssel.
    /// </summary>
    static void HandleTransfers()
    {
        Console.WriteLine("\nVálassz egy költségvetési tesztesetet:");
        Console.WriteLine("1. Budget 1 - 1 millió Ft");
        Console.WriteLine("2. Budget 2 - 2 millió Ft");
        Console.WriteLine("3. Budget 3 - 3 millió Ft");
        Console.WriteLine("4. Budget 4 - 4 millió Ft");
        Console.WriteLine("5. Budget 5 - 5 millió Ft");

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

    /// <summary>
    /// Meccsszimuláció menü.
    /// </summary>
    static void SimulateMatchMenu()
    {
        try
        {
            // Keretfájlok automatikus keresése ujkeret# formátumban
            var keretFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "ujkeret*.txt");

            if (keretFiles.Length == 0)
            {
                Console.WriteLine("Nincs elérhető keretfájl. Előbb hajts végre igazolásokat!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nVálassz egy keretet a következő fájlok közül:");
            for (int i = 0; i < keretFiles.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(keretFiles[i])}");
            }

            Console.Write("Keret száma: ");
            int keretChoice = int.Parse(Console.ReadLine()) - 1;

            if (keretChoice < 0 || keretChoice >= keretFiles.Length)
            {
                Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                Console.ReadKey();
                return;
            }

            var fehervarPlayers = FileHandler.LoadPlayersFromFile(keretFiles[keretChoice]);
            if (fehervarPlayers.Count == 0)
            {
                Console.WriteLine("Hiba: Az adott keret üres vagy nem tölthető be.");
                Console.ReadKey();
                return;
            }

            var fehervar = new Team
            {
                Name = "Fehérvár FC",
                Players = fehervarPlayers
            };

            var opponents = new List<string> { "Paksi FC", "Ferencváros", "Debreceni VSC", "Újpest FC" };
            Console.WriteLine("\nVálassz egy ellenfelet:");
            for (int i = 0; i < opponents.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {opponents[i]}");
            }

            Console.Write("Ellenfél száma: ");
            int opponentChoice = int.Parse(Console.ReadLine()) - 1;
            if (opponentChoice < 0 || opponentChoice >= opponents.Count)
            {
                Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                Console.ReadKey();
                return;
            }

            var opponentFileName = $"{opponents[opponentChoice].ToLower().Replace(" ", "_")}_players.txt";
            var opponentPlayers = FileHandler.LoadPlayersFromFile(opponentFileName);
            if (opponentPlayers.Count == 0)
            {
                Console.WriteLine($"Hiba: Az ellenfél játékoslistája nem található vagy üres: {opponentFileName}");
                Console.ReadKey();
                return;
            }

            var opponent = new Team
            {
                Name = opponents[opponentChoice],
                Players = opponentPlayers
            };

            SimulateMatch(fehervar, opponent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a meccsszimuláció során: {ex.Message}");
            Console.ReadKey();
        }
    }




    /// <summary>
    /// Teszteset futtatása az igazolások optimalizálására.
    /// </summary>
    static void RunTestCase(string testFile)
    {
        try
        {
            var fehervarPlayers = FileHandler.LoadPlayersFromFile("fehervar_players.txt");
            if (fehervarPlayers.Count == 0)
            {
                Console.WriteLine("Hiba: A Fehérvár FC játékoslistája nem található vagy üres.");
                return;
            }

            var fehervar = new Team
            {
                Name = "Fehérvár FC",
                Budget = BudgetHandler.LoadBudgetFromFile(testFile),
                Players = fehervarPlayers
            };

            Console.WriteLine("Kezdő Fehérvár FC:");
            Console.WriteLine(fehervar);

            var transferMarket = FileHandler.LoadPlayersFromFile("atigazolasi_piac.txt");
            if (transferMarket.Count == 0)
            {
                Console.WriteLine("Hiba: Az átigazolási piac listája üres vagy nem található.");
                return;
            }

            var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget, fehervar.Players);

            foreach (var player in bestTransfers)
            {
                fehervar.AddPlayer(player);
            }

            Console.WriteLine("\nFehérvár FC az igazolások után:");
            Console.WriteLine(fehervar);

            Console.Write("\nAdd meg a fájl nevét a keret mentéséhez (pl. fehervar_keret_teszt1.txt): ");
            string fileName = Console.ReadLine();

            FileHandler.SavePlayersToFile(fehervar.Players, fileName);
            Console.WriteLine($"A frissített keret mentve a '{fileName}' fájlba.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt: {ex.Message}");
        }
    }

    /// <summary>
    /// Mérkőzés szimulálása két csapat között.
    /// </summary>
    static void SimulateMatch(Team homeTeam, Team awayTeam)
    {
        Console.WriteLine($"\n--- Mérkőzés: {homeTeam.Name} vs {awayTeam.Name} ---");

        var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(homeTeam, awayTeam);

        Console.WriteLine($"{homeTeam.Name} - {awayTeam.Name}: {homeGoals} - {awayGoals}");

        if (homeGoals > awayGoals)
        {
            Console.WriteLine($"{homeTeam.Name} győzött!");
        }
        else if (homeGoals < awayGoals)
        {
            Console.WriteLine($"{awayTeam.Name} győzött!");
        }
        else
        {
            Console.WriteLine("A mérkőzés döntetlennel zárult.");
        }

        Console.WriteLine("Nyomj meg egy gombot a folytatáshoz...");
        Console.ReadKey();
    }

}
