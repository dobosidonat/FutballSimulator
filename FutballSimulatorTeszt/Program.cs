using FutballSimulator;
using System;
using System.Collections.Generic;

class Program
{
    static Dictionary<string, Table> table = new Dictionary<string, Table>(); // Tabella globálisan elérhető

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t\t\t\t===== Fehérvár FC Menedzser Program =====\n");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Szezon szimuláció");
            Console.WriteLine("3. Tabella megtekintése");
            Console.WriteLine("0. Kilépés");

            Console.Write("\nAdd meg a választásod: ");
            string input = Console.ReadLine();

            if (input == "0")
            {
                Environment.Exit(0); // Azonnali kilépés
            }

            switch (input)
            {
                case "1":
                    TransferHandler.HandleTransfers();
                    break;
                case "2":
                    StartSeasonSimulation();
                    break;
                case "3":
                    DisplayCurrentTable();
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Szezon szimuláció elindítása.
    /// </summary>
    static void StartSeasonSimulation()
    {
        var teams = new List<string>
        {
            "Puskás Akadémia FC",
            "Ferencvárosi TC",
            "Diósgyőri VTK",
            "MTK Budapest",
            "Paks",
            "Újpest FC",
            "Fehérvár FC",
            "Nyíregyháza Spartacus FC",
            "Győri ETO FC",
            "Zalaegerszegi TE FC",
            "Debreceni VSC",
            "Kecskeméti TE",
        };

        var schedule = ScheduleHandler.GenerateSchedule(teams);
        int round = 1;

        foreach (var match in schedule)
        {
            Console.Clear();
            Console.WriteLine($"Forduló {round}: {match.homeTeam} vs {match.awayTeam}");

            var homeTeam = new Team
            {
                Name = match.homeTeam,
                Players = FileHandler.LoadPlayersFromFile($"Csapatok/{match.homeTeam.ToLower().Replace(" ", "_")}_players.txt")
            };

            var awayTeam = new Team
            {
                Name = match.awayTeam,
                Players = FileHandler.LoadPlayersFromFile($"Csapatok/{match.awayTeam.ToLower().Replace(" ", "_")}_players.txt")
            };

            var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(homeTeam, awayTeam);
            Console.WriteLine($"Eredmény: {match.homeTeam} {homeGoals} - {awayGoals} {match.awayTeam}");

            TableHandler.UpdateTable(table, match.homeTeam, homeGoals, match.awayTeam, awayGoals);
            TableHandler.DisplayTable(table);

            TableHandler.SaveTable(table, round);

            round++;

            Console.Write("\nSzeretnéd folytatni a következő meccsel? (i/n): ");
            string continueInput = Console.ReadLine()?.ToLower();
            if (continueInput != "i")
            {
                break; // Kilépünk a szezon szimulációból
            }
        }

        Console.WriteLine("\nA szezon véget ért. Nyomj meg egy gombot a folytatáshoz.");
        Console.ReadKey();
    }

    /// <summary>
    /// Tabella megtekintése.
    /// </summary>
    static void DisplayCurrentTable()
    {
        Console.Clear();
        Console.WriteLine("Aktuális tabella:\n");
        TableHandler.DisplayTable(table);
        Console.WriteLine("\nNyomj meg egy gombot a visszalépéshez.");
        Console.ReadKey();
    }
}
