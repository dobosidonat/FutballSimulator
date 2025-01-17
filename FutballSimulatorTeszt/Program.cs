using FutballSimulator;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Csapatok betöltése fájlból
        var teams = FileHandler.LoadTeams("ellenfelek/csapat_ertekelesek.txt");

        // Fehérvár FC alapértelmezett keretének betöltése
        var fehervarPlayers = FileHandler.LoadPlayersFromFile("keretek/fehervar_players.txt");
        var fehervar = new Team
        {
            Name = "Fehérvár FC",
            Players = fehervarPlayers
        };

        // Főmenü megjelenítése és felhasználó által választott opcióra írt funkció meghívása
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t\t\t\t\t===== Fehérvár FC Menedzser Program =====");
            Console.WriteLine();
            Console.WriteLine("Válassz a következő lehetőségek közül:");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Szezon szimuláció (manuális)");
            Console.WriteLine("3. Tabella megtekintése");
            Console.WriteLine("4. Automatikus szezon szimuláció kiválasztott kerettel");
            Console.WriteLine("0. Kilépés");

            Console.Write("Add meg a választásod: ");
            string input = Console.ReadLine();

            // Választás alapján a megfelelő funkció meghívása
            switch (input)
            {
                case "1":
                    TransferHandler.HandleTransfers(); // Igazolások kezelése
                    break;
                case "2":
                    SimulateSeason(teams, fehervar); //Azezon szimulációja egyesével
                    break;
                case "3":
                    DisplayCurrentTable(); // Tabella megtekintése
                    break;
                case "4":
                    SeasonSimulator.SimulateFullSeasonAutomatically(teams, fehervar); // Teljes szezon leszimulálása
                    break;
                case "0":
                    Environment.Exit(0); // Kilépés a programból
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Manuális szezon szimuláció indítása.
    /// </summary>
    static void SimulateSeason(List<Team> teams, Team fehervar)
    {
        SeasonSimulator.SimulateSeason(teams, fehervar); // Szezon szimuláció futtatása
    }

    /// <summary>
    /// Tabella megtekintése.
    /// </summary>
    static void DisplayCurrentTable()
    {
        string tableFile = "eredmenyek/tabella_manual.txt"; // Tabella fájl elérési útja

        if (!File.Exists(tableFile))
        {
            Console.WriteLine("Nincs elérhető tabella. Előbb szimulálj le egy szezont!");
            Console.ReadKey();
            return;
        }

        // Tabella megjelenítése
        Console.Clear();
        Console.WriteLine("--- Aktuális Tabella ---");
        foreach (var line in File.ReadAllLines(tableFile))
        {
            Console.WriteLine(line);
        }

        Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
        Console.ReadKey();
    }
}
