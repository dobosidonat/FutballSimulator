using FutballSimulator;
using System;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t===== Fehérvár FC Menedzser Program =====");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Bajnokság szimulálása fordulóról fordulóra");
            Console.WriteLine("3. Tabella megtekintése");
            Console.WriteLine("0. Kilépés");
            Console.Write("Válassz egy lehetőséget: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    TransferHandler.HandleTransfers();
                    break;
                case "2":
                    SeasonSimulator.SimulateSeason();
                    break;
                case "3":
                    SeasonSimulator.DisplayTable();
                    break;
                case "0":
                    Environment.Exit(0); // Kilépés a programból
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás! Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
