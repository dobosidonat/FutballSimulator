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
        public string Name { get; set; }    //Játékos neve
        public string Position { get; set; } //Játékos pozíciója
        public int Age { get; set; }    //Játékos kora
        public double Rating { get; set; }  //Játékos értékelése (0-100)
        public double MarketValue { get; set; } //Játékos piaci értéke


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
