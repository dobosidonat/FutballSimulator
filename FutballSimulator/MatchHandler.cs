using System;
using System.Collections.Generic;
using System.IO;

namespace FutballSimulator
{
    /// <summary>
    /// Kezeli a felhasználóval való interakciókat és a meccsek szimulációjának folyamatát
    /// 
    /// </summary>
    public static class MatchHandler
    {
        /// <summary>
        /// Meccsszimuláció menü.
        /// </summary>
        public static void SimulateMatchMenu()
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

                TeamEvaluator.EvaluateTeamPositions(fehervar);
                TeamEvaluator.EvaluateTeamPositions(opponent);

                SimulateMatch(fehervar, opponent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a meccsszimuláció során: {ex.Message}");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Meccsszimuláció egyéni felállással.
        /// </summary>
        public static void SimulateMatchWithFormationMenu()
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

        /// <summary>
        /// Mérkőzés szimulálása egyéni felállással.
        /// </summary>
        public static void SimulateMatchWithFormation(Team homeTeam, Team awayTeam)
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

        /// <summary>
        /// Mérkőzés szimulálása alapértelmezett kezdőcsapatokkal.
        /// </summary>
        public static void SimulateMatch(Team homeTeam, Team awayTeam)
        {
            var (homeGoals, awayGoals) = MatchSimulator.SimulateMatch(homeTeam, awayTeam);
            Console.WriteLine($"{homeTeam.Name} - {awayTeam.Name}: {homeGoals} - {awayGoals}");
            Console.ReadKey();
        }
    }
}
