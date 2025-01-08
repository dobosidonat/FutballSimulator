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

            // Szimulációs logika: a csapatok átlagos értékelése alapján számolunk
            double homeRating = homeTeam.Players.Average(p => p.Rating);
            double awayRating = awayTeam.Players.Average(p => p.Rating);

            int homeGoals = (int)(random.NextDouble() * (homeRating / 10));
            int awayGoals = (int)(random.NextDouble() * (awayRating / 10));

            return (homeGoals, awayGoals);
        }
    }
}
