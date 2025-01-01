using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace FutballSimulator
{
    /// <summary>
    /// Statikus osztály a fájlkezeléshez.
    /// </summary>
    public static class FileHandler
    {
        /// <summary>
        /// Játékosok betöltése szöveges fájlból.
        /// </summary>
        /// <param name="filePath">A fájl elérési útvonala.</param>
        /// <returns>A játékosok listája.</returns>
        public static List<Player> LoadPlayersFromFile(string filePath)
        {
            var players = new List<Player>();

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(';'); // Az adatok elválasztása
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

            return players;
        }

    }
}
