using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutballSimulator
{
    /// <summary>
    /// Csapat osztály
    /// </summary>
    public class Team
    {

        /// <summary>
        /// A csapat neve
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A csapat költségvetése 
        /// </summary>
        public double Budget { get; set; }

        /// <summary>
        /// A csapat játékosainak listája
        /// </summary>
        public List<Player> Players { get; set; } = new List<Player>();

        /// <summary>
        /// A csapat játékosainak átlagos értékelését számítja ki.
        /// Ez egy kifejezés alapú tulajdonság, amely LINQ lambda kifejezést használ a számításban
        /// if-else használatával is meg lehetett volna oldani de ez így rövidebb
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
            Console.WriteLine($"{player.Name} leigazolva!");
        }
    }
}
