using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutballSimulator
{
    //Csapat osztály
    public class Team
    {
        

        public string Name { get; set; } // A csapat neve
        public double Budget { get; set; } // A csapat költségvetése
        public List<Player> Players { get; set; } = new List<Player>(); // A csapat játékosainak listája

        /// <summary>
        /// A csapat játékosainak átlagos értékelését számítja ki.
        /// </summary>
        public double AverageRating => Players.Count > 0 
            ? Players.Average(p => p.Rating) 
            : 0;

        /// <summary>
        /// Játékos hozzáadása a csapathoz és a költségvetés frissítése.
        /// </summary>
        /// <param name="player">A hozzáadandó játékos.</param>
        public void AddPlayer(Player player)
        {
            Players.Add(player);
            Budget -= player.MarketValue;
            Console.WriteLine($"{player.Name} automatikusan leigazolva a {Name} csapatába!");
        }

        /// <summary>
        /// A csapat adatait tartalmazó szöveges reprezentációt ad vissza.
        /// </summary>
        /// <returns>Szöveg, amely tartalmazza a csapat adatait.</returns>
        public override string ToString()
        {
            return $"{Name} - Költségvetés: {Budget:C}, Átlagos Értékelés: {AverageRating:F1}";
        }
    }
}
