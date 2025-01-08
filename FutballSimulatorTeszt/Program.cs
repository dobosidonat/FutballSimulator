﻿using FutballSimulator;
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
            Console.WriteLine();
            Console.WriteLine("Válassz a következő lehetőségek közül:");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Meccsszimuláció");
            Console.WriteLine("3. Meccsszimuláció egyéni felállással");
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
                case "3":
                    Console.Clear();
                    SimulateMatchWithFormationMenu();
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void HandleTransfers()
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
        Console.Clear();

        if (!File.Exists(testFile))
        {
            Console.WriteLine($"Hiba: A költségvetési fájl nem található: {testFile}");
            Console.ReadKey();
            return;
        }

        RunTestCase(testFile);
    }

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
            EvaluateTeamPositions(fehervar);

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
                EvaluateTeamPositions(fehervar);
            }

            Console.Write("\nSzeretnéd elmenteni a keretet? (i/n): ");
            string saveResponse = Console.ReadLine()?.ToLower();

            if (saveResponse == "i")
            {
                Console.Write("\nAdd meg a fájl nevét a keret mentéséhez (pl. ujkeret1.txt): ");
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

    static void SimulateMatchWithFormationMenu()
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

            SimulateMatchWithFormation(fehervar, opponent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a meccsszimuláció során: {ex.Message}");
            Console.ReadKey();
        }
    }

    static void SimulateMatchWithFormation(Team homeTeam, Team awayTeam)
    {
        var (defenders, midfielders, forwards) = FormationHandler.GetFormation();

        var homeBestTeam = FormationHandler.GetBestTeam(homeTeam, defenders, midfielders, forwards);
        var awayBestTeam = FormationHandler.GetBestTeam(awayTeam, defenders, midfielders, forwards);

        Console.WriteLine("\n--- Kezdőcsapatok ---");
        Console.WriteLine($"Hazai csapat ({homeTeam.Name}):");
        foreach (var player in homeBestTeam)
        {
            Console.WriteLine($"{player.Name} ({player.Position}), Értékelés: {player.Rating}");
        }

        Console.WriteLine($"\nVendég csapat ({awayTeam.Name}):");
        foreach (var player in awayBestTeam)
        {
            Console.WriteLine($"{player.Name} ({player.Position}), Értékelés: {player.Rating}");
        }

        var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(
            new Team { Name = homeTeam.Name, Players = homeBestTeam },
            new Team { Name = awayTeam.Name, Players = awayBestTeam }
        );

        Console.WriteLine($"\nEredmény: {homeTeam.Name} {homeGoals} - {awayGoals} {awayTeam.Name}");
        Console.ReadKey();
    }

    static void EvaluateTeamPositions(Team team)
    {
        Console.WriteLine($"Pozíciók kiértékelése a {team.Name} csapatában:");

        var positions = team.Players.GroupBy(player => player.Position);

        foreach (var position in positions)
        {
            Console.WriteLine($"{position.Key}: {position.Count()} játékos, átlagos értékelés: {position.Average(p => p.Rating):F1}");
        }
        Console.WriteLine();
    }
}
