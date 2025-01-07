using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutballSimulator
{
    public static class MatchSimulator
    {
        /// <summary>
        /// Egy mérkőzés eredményét szimulálja a csapatok átlagos értékelése alapján.
        /// </summary>
        /// <param name="homeTeam">Hazai csapat.</param>
        /// <param name="awayTeam">Vendég csapat.</param>
        /// <returns>Gólok száma: hazai és vendég.</returns>
        public static (int homeGoals, int awayGoals) SimulateMatch(Team homeTeam, Team awayTeam)
        {
            Random random = new Random();

            // Gólok számának generálása a csapatok átlagos értékelése alapján
            int homeGoals = random.Next(0, (int)(homeTeam.AverageRating / 10)) + random.Next(0, 2);
            int awayGoals = random.Next(0, (int)(awayTeam.AverageRating / 10)) + random.Next(0, 2);

            return (homeGoals, awayGoals);
        }
    }
}
