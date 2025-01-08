using FootballManagerLibrary;
using FutballSimulator;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        while (true) // Folyamatos futás, amíg a felhasználó nem választja a kilépést
        {
            Console.Clear();
            Console.WriteLine("\t\t\t\t\t===== Fehérvár FC Menedzser Program =====");
            Console.WriteLine("\t\t\t\t\tNyomj egy gombot az induláshoz!");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("Válassz a következő lehetőségek közül:");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Meccsszimuláció");
            Console.WriteLine("0. Kilépés");

            Console.Write("Add meg a választásod: ");
            string input = Console.ReadLine();

            if (input == "0")
            {
                break;
            }

            switch (input)
            {
                case "1":
                    Console.Clear();
                    HandleTransfers();
                    break;
                case "2":
                    Console.Clear();
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
        Console.Clear();

        if (!File.Exists(testFile))
        {
            Console.WriteLine($"Hiba: A költségvetési fájl nem található: {testFile}");
            Console.ReadKey();
            return;
        }

        RunTestCase(testFile);
    }

    /// <summary>
    /// Teszteset futtatása az igazolások optimalizálására.
    /// </summary>
    static void RunTestCase(string testFile)
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
            EvaluateTeamPositions(fehervar); // Pozíciók kiértékelése az igazolások előtt

            // Átigazolási piac betöltése
            var transferMarket = FileHandler.LoadPlayersFromFile("atigazolasi_piac.txt");
            var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget, fehervar.Players);

            if (bestTransfers.Count == 0)
            {
                Console.WriteLine("\nNem történt érdemi igazolás. A keret változatlan marad.");
            }
            else
            {
                Console.WriteLine("\nIgazolt játékosok:");
                foreach (var player in bestTransfers)
                {
                    Console.WriteLine($"- {player.Name} ({player.Position}), Értékelés: {player.Rating}, Piaci érték: {player.MarketValue} Ft");
                    fehervar.AddPlayer(player);
                }

                Console.WriteLine("\nFehérvár FC az igazolások után:");
                EvaluateTeamPositions(fehervar); // Pozíciók kiértékelése az igazolások után
            }

            // Rákérdezés a mentésre
            Console.Write("\nSzeretnéd elmenteni a keretet? (i/n): ");
            string saveResponse = Console.ReadLine()?.ToLower();

            if (saveResponse == "i")
            {
                Console.Write("\nAdd meg a fájl nevét a keret mentéséhez (pl. ujkeret1.txt): ");
                string fileName;

                while (true)
                {
                    fileName = Console.ReadLine();

                    // Automatikusan hozzáadjuk a .txt kiterjesztést, ha hiányzik
                    if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".txt";
                    }

                    // Ellenőrizzük, hogy a fájl létezik-e
                    if (File.Exists(fileName))
                    {
                        Console.WriteLine($"A '{fileName}' fájl már létezik. Adj meg egy másik nevet:");
                    }
                    else
                    {
                        break;
                    }
                }

                FileHandler.SavePlayersToFile(fehervar.Players, fileName);
                Console.WriteLine($"A frissített keret mentve a '{fileName}' fájlba.");
            }
            else
            {
                Console.WriteLine("A keret nem került mentésre.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt: {ex.Message}");
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Meccsszimuláció menü.
    /// </summary>
    static void SimulateMatchMenu()
    {
        try
        {
            Console.Clear();
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

            var opponentFileName = $"{opponents[opponentChoice].ToLower().Replace(" ", "_")}_players.txt";
            var opponentPlayers = FileHandler.LoadPlayersFromFile(opponentFileName);

            var opponent = new Team
            {
                Name = opponents[opponentChoice],
                Players = opponentPlayers
            };

            // Pozíciók kiértékelése mindkét csapatnál
            EvaluateTeamPositions(fehervar);
            EvaluateTeamPositions(opponent);

            SimulateMatch(fehervar, opponent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a meccsszimuláció során: {ex.Message}");
            Console.ReadKey();
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

    /// <summary>
    /// Pozíciók kiértékelése egy csapatnál.
    /// </summary>
    static void EvaluateTeamPositions(Team team)
    {
        Console.WriteLine($"Pozíciók kiértékelése a {team.Name} csapatában:");

        // Csoportosítás pozíciók szerint
        var positions = team.Players.GroupBy(player => player.Position);

        // Pozíciónként kiírás
        foreach (var position in positions)
        {
            Console.WriteLine($"{position.Key}: {position.Count()} játékos, átlagos értékelés: {position.Average(p => p.Rating):F1}");
        }
        Console.WriteLine();
    }
}
