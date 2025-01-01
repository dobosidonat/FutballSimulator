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
        public string Position { get; set; } //Játékos kora
        public int Age { get; set; }    //Játékos kora
        public double Rating { get; set; }  //Játékos értékelése
        public double MarketValue { get; set; } //Játékos piaci értéke

        public override string ToString()
        {
            return $"{Name} ({Position}) - Rating: {Rating}, Value: {MarketValue:C}";
        }
    }
}
