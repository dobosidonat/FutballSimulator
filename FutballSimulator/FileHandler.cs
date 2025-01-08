using FutballSimulator;
using System;
using System.Collections.Generic;
using System.IO;

namespace FootballManagerLibrary
{
    public static class FileHandler
    {
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
