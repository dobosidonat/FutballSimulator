using FutballSimulator;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t===== Fehérvár FC Menedzser Program =====");
            Console.WriteLine();
            Console.WriteLine("Válassz a következő lehetőségek közül:");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Szezon szimuláció");
            Console.WriteLine("3. Tabella megtekintése");
            Console.WriteLine("0. Kilépés");

            Console.Write("Add meg a választásod: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    TransferHandler.HandleTransfers();
                    break;
                case "2":
                    SimulateSeason();
                    break;
                case "3":
                    DisplayCurrentTable();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// A szezon szimuláció elindítása.
    /// </summary>
    static void SimulateSeason()
    {
        // Csapatok betöltése fájlból
        var teams = FileHandler.LoadTeams("ellenfelek/csapat_ertekelesek.txt");

        // Fehérvár FC keret betöltése
        var fehervarPlayers = FileHandler.LoadPlayersFromFile("keretek/fehervar_players.txt");
        var fehervar = new Team
        {
            Name = "Fehérvár FC",
            Players = fehervarPlayers
        };

        // Szezon szimuláció
        SeasonSimulator.SimulateSeason(teams, fehervar);
    }

    /// <summary>
    /// Tabella megtekintése.
    /// </summary>
    static void DisplayCurrentTable()
    {
        string tableFile = "eredmenyek/tabella.txt";

        if (!File.Exists(tableFile))
        {
            Console.WriteLine("Nincs elérhető tabella. Előbb szimulálj le egy szezont!");
            Console.ReadKey();
            return;
        }

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
