using FootballManagerLibrary;
using FutballSimulator;
using System;
using System.IO;


class Program
{
    static void Main(string[] args)
    {
        while (true) // Folyamatos futás, amíg a felhasználó nem választja a kilépést
        {
            Console.Clear();
            Console.WriteLine("===== Fehérvár FC Átigazolási Teszt =====");
            Console.WriteLine("Válassz egy tesztesetet a következők közül:");
            Console.WriteLine("1. Budget 1 - 1 millió Ft költségvetés");
            Console.WriteLine("2. Budget 2 - 2 millió Ft költségvetés");
            Console.WriteLine("3. Budget 3 - 3 millió Ft költségvetés");
            Console.WriteLine("4. Budget 4 - 4 millió Ft költségvetés");
            Console.WriteLine("5. Budget 5 - 5 millió Ft költségvetés");
            Console.WriteLine("0. Kilépés");

            Console.Write("Add meg a választott teszteset számát: ");
            string input = Console.ReadLine();

            if (input == "0")
            {
                Console.WriteLine("Kilépés...");
                break;
            }

            string testFile = null;

            switch (input)
            {
                case "1":
                    testFile = "budget1.txt";
                    break;
                case "2":
                    testFile = "budget2.txt";
                    break;
                case "3":
                    testFile = "budget3.txt";
                    break;
                case "4":
                    testFile = "budget4.txt";
                    break;
                case "5":
                    testFile = "budget5.txt";
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    continue;
            }

            RunTestCase(testFile);

            Console.WriteLine("\nSzeretnél újabb tesztesetet futtatni? (i/n)");
            string continueInput = Console.ReadLine()?.ToLower();
            if (continueInput != "i")
            {
                Console.WriteLine("Kilépés...");
                break;
            }
        }
    }

    /// <summary>
    /// Teszteset futtatása a megadott költségvetési fájllal.
    /// </summary>
    /// <param name="testFile">A költségvetési fájl neve.</param>
    static void RunTestCase(string testFile)
    {
        try
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
            var transferMarket = FileHandler.LoadPlayersFromFile("atigazolasi_piac.txt");

            // Optimalizált igazolások
            var bestTransfers = TransferOptimizer.OptimizeTransfers(transferMarket, fehervar.Budget, fehervar.Players);
            foreach (var player in bestTransfers)
            {
                fehervar.AddPlayer(player); // Ez a metódus frissíti a költségvetést és hozzáadja a játékost a kerethez
            }

            Console.WriteLine("\nFehérvár FC az igazolások után:");
            Console.WriteLine(fehervar);

            Console.Write("\nAdd meg a fájl nevét a keret mentéséhez (pl. fehervar_keret_teszt1.txt): ");
            string fileName;

            while (true)
            {
                fileName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "fehervar_updated_keret.txt"; // Alapértelmezett fájlnév
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine($"A '{fileName}' fájl már létezik. Adj meg egy másik nevet:");
                }
                else
                {
                    break; // Kilépünk a ciklusból, ha a fájl még nem létezik
                }
            }

            FileHandler.SavePlayersToFile(fehervar.Players, fileName);
            Console.WriteLine($"\nA frissített keret mentve a '{fileName}' fájlba.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a teszteset futtatása során: {ex.Message}");
        }
    }
}
