using System;
using System.Collections.Generic;
using System.IO;

namespace FutballSimulator
{
    /// <summary>
    /// Statikus osztály a csapatok fájljainak kezeléséhez
    /// </summary>
    public static class FileHandler
    {
        /// <summary>
        /// Csapatok betöltése a megadott fájlból, kivéve a Fehérvár FC-t.
        /// </summary>
        public static List<Team> LoadTeams(string filePath)
        {
            var teams = new List<Team>();

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(';');
            
                teams.Add(new Team
                {
                    Name = parts[0],
                    Players = GeneratePlayersFromRatings(
                        double.Parse(parts[1]), // Védelem
                        double.Parse(parts[2]), // Középpálya
                        double.Parse(parts[3]), // Támadók
                        double.Parse(parts[4])  // Kapus
                    )
                });
                
            }

            return teams;
        }

        /// <summary>
        /// Játékosok generálása a csapatrészek értékelései alapján.
        /// </summary>
        private static List<Player> GeneratePlayersFromRatings(double defense, double midfield, double forward, double goalkeeper)
        {
            var players = new List<Player>();

            // Védelem (DF)
            for (int i = 0; i < 4; i++)
            {
                players.Add(new Player { Name = $"Védő {i + 1}", Position = "DF", Rating = defense });
            }

            // Középpálya (MF)
            for (int i = 0; i < 4; i++)
            {
                players.Add(new Player { Name = $"Középpályás {i + 1}", Position = "MF", Rating = midfield });
            }

            // Támadók (FW)
            for (int i = 0; i < 3; i++)
            {
                players.Add(new Player { Name = $"Támadó {i + 1}", Position = "FW", Rating = forward });
            }

            // Kapus (GK)
            players.Add(new Player { Name = "Kapus", Position = "GK", Rating = goalkeeper });

            return players;
        }

        /// <summary>
        /// Játékosok beolvasás egy fájlból egy listába
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Player> LoadPlayersFromFile(string filePath)
        {
            var players = new List<Player>();

            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"A fájl nem található: {filePath}");
                }

                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 5)
                    {
                        players.Add(new Player
                        {
                            Name = parts[0],
                            Position = parts[1],
                            Age = int.Parse(parts[2]),
                            Rating = double.Parse(parts[3]),
                            MarketValue = double.Parse(parts[4])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a játékosok betöltésekor: {ex.Message}");
            }

            return players;
        }

        /// <summary>
        /// Ezt az igazolásoknál fogjuk meghívni, a program által igazolt játékosokat ez alapján fogjuk menteni egy fájlba
        /// </summary>
        /// <param name="players"></param>
        /// <param name="filePath"></param>
        public static void SavePlayersToFile(List<Player> players, string filePath)
        {
            try
            {
                // Automatikusan hozzáadjuk a .txt kiterjesztést, ha hiányzik
                if (!filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    filePath += ".txt";
                }

                var lines = new List<string>();
                foreach (var player in players)
                {
                    //A :F1 egy formátumsztring, az F (fixed) biztosítja, hogy tizedes vesző/ponttal fogja kiírni a számot
                    //Az 1 pedig azt jelenti, hogy az egész rész, után 1 tizedes jegyet tart meg
                    lines.Add($"{player.Name};{player.Position};{player.Age};{player.Rating:F1};{player.MarketValue}");
                }   

                File.WriteAllLines(filePath, lines);
                Console.WriteLine($"A játékosok sikeresen mentve lettek a következő fájlba: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a fájl mentésekor: {ex.Message}");
            }
        }

    }
}
