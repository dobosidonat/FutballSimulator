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
            Console.WriteLine("\t\t===== Fehérvár FC Menedzser Program =====");
            Console.WriteLine();
            Console.WriteLine("Válassz a következő lehetőségek közül:");
            Console.WriteLine("1. Igazolások végrehajtása");
            Console.WriteLine("2. Meccsszimuláció");
            Console.WriteLine("3. Meccsszimuláció egyéni felállással");
            Console.WriteLine("0. Kilépés");

            Console.Write("Add meg a választásod: ");
            string input = Console.ReadLine();

            if (input == "0") break;

            switch (input)
            {
                case "1":
                    TransferHandler.HandleTransfers();
                    break;
                case "2":
                    MatchHandler.SimulateMatchMenu();
                    break;
                case "3":
                    MatchHandler.SimulateMatchWithFormationMenu();
                    break;
                default:
                    Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot a folytatáshoz.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
