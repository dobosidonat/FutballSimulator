using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FutballSimulator
{
    public static class MatchHandler
    {
        /// <summary>
        /// Mérkőzés szimulálása egyéni felállással.
        /// </summary>
        public static void SimulateMatchWithFormation(Team homeTeam, Team awayTeam)
        {
            var (defenders, midfielders, forwards) = FormationHandler.GetFormation();

            var homeBestTeam = FormationHandler.GetBestTeam(homeTeam, defenders, midfielders, forwards);
            var awayBestTeam = FormationHandler.GetBestTeam(awayTeam, defenders, midfielders, forwards);

            // Hazai csapat felállásának megjelenítése
            Console.Clear();
            Console.WriteLine("--- Hazai csapat kezdőcsapata ---");
            DisplayFormation(homeBestTeam, defenders, midfielders, forwards);
            Console.WriteLine("\nNyomj meg egy gombot a vendégcsapat kezdőcsapatának megtekintéséhez...");
            Console.ReadKey();

            // Vendég csapat felállásának megjelenítése
            Console.Clear();
            Console.WriteLine("--- Vendég csapat kezdőcsapata ---");
            DisplayFormation(awayBestTeam, defenders, midfielders, forwards);
            Console.WriteLine("\nNyomj meg egy gombot az eredmény megtekintéséhez...");
            Console.ReadKey();

            // Meccseredmény kiírása
            Console.Clear();
            var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(
                new Team { Name = homeTeam.Name, Players = homeBestTeam },
                new Team { Name = awayTeam.Name, Players = awayBestTeam }
            );

            Console.WriteLine($"\n--- Eredmény ---");
            Console.WriteLine($"{homeTeam.Name} {homeGoals} - {awayGoals} {awayTeam.Name}");

            SaveMatchResult(homeTeam, awayTeam, homeGoals, awayGoals, defenders, midfielders, forwards);
            Console.WriteLine("\nAz eredmény mentésre került az eredmenyek mappába.");
            Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
            Console.ReadKey();
        }

        /// <summary>
        /// Csapat felállásának megjelenítése.
        /// </summary>
        private static void DisplayFormation(List<Player> team, int defenders, int midfielders, int forwards)
        {
            var goalkeeper = team.FirstOrDefault(p => p.Position == "GK");
            Console.WriteLine($"Kapus: {goalkeeper?.Name} ({goalkeeper?.Rating})");

            Console.WriteLine("\nVédők:");
            foreach (var defender in team.Where(p => p.Position == "DF").Take(defenders))
            {
                Console.WriteLine($"- {defender.Name} ({defender.Rating})");
            }

            Console.WriteLine("\nKözéppályások:");
            foreach (var midfielder in team.Where(p => p.Position == "MF").Take(midfielders))
            {
                Console.WriteLine($"- {midfielder.Name} ({midfielder.Rating})");
            }

            Console.WriteLine("\nCsatárok:");
            foreach (var forward in team.Where(p => p.Position == "FW").Take(forwards))
            {
                Console.WriteLine($"- {forward.Name} ({forward.Rating})");
            }
        }

        /// <summary>
        /// Mérkőzés eredményének mentése fájlba.
        /// </summary>
        private static void SaveMatchResult(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals, int defenders, int midfielders, int forwards)
        {
            try
            {
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "eredmenyek");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string fileName = $"eredmeny_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(directoryPath, fileName);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"Mérkőzés: {homeTeam.Name} {homeGoals} - {awayGoals} {awayTeam.Name}");
                    writer.WriteLine($"Felállás: {defenders}-{midfielders}-{forwards}");

                    writer.WriteLine("\nHazai kezdőcsapat:");
                    var homeBestTeam = FormationHandler.GetBestTeam(homeTeam, defenders, midfielders, forwards);
                    foreach (var player in homeBestTeam)
                    {
                        writer.WriteLine($"- {player.Name} ({player.Position}, {player.Rating})");
                    }

                    writer.WriteLine("\nVendég kezdőcsapat:");
                    var awayBestTeam = FormationHandler.GetBestTeam(awayTeam, defenders, midfielders, forwards);
                    foreach (var player in awayBestTeam)
                    {
                        writer.WriteLine($"- {player.Name} ({player.Position}, {player.Rating})");
                    }

                    writer.WriteLine(new string('-', 40));
                }

                Console.WriteLine($"\nAz eredmény mentésre került a következő fájlba: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba az eredmény mentése során: {ex.Message}");
            }
        }
    }
}
