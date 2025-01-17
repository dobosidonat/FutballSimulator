using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutballSimulator
{
    /// <summary>
    /// Játékos osztáy
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Játékos neve 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///Játékos pozíciója
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Játékos kora 
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Játékos értékelése (0-100)
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// //Játékos piaci értéke
        /// </summary>
        public double MarketValue { get; set; } 


        /// <summary>
        /// A játékos adatait tartalmazó szöveges reprezentációt ad vissza.
        /// </summary>
        /// <returns>Szöveg, amely tartalmazza a játékos adatait.</returns>
        public override string ToString()
        {
            return $"{Name} ({Position}) - Rating: {Rating}, Value: {MarketValue:C}";
        }
    }
}
