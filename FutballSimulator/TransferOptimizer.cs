using FutballSimulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootballManagerLibrary
{
    public static class TransferOptimizer
    {
        /// <summary>
        /// Optimalizálja az igazolásokat backtracking segítségével.
        /// </summary>
        /// <param name="transferMarket">Az átigazolási piacon elérhető játékosok listája.</param>
        /// <param name="budget">A csapat költségvetése.</param>
        /// <param name="currentTeam">A csapat jelenlegi kerete.</param>
        /// <returns>A legjobb igazolások listája.</returns>
        public static List<Player> OptimizeTransfers(List<Player> transferMarket, double budget, List<Player> currentTeam)
        {
            var bestTeam = new List<Player>(currentTeam); // A legjobb csapat, amit találtunk
            double bestAverageRating = currentTeam.Average(p => p.Rating);

            var currentTransfers = new List<Player>();

            void Backtrack(int index, double remainingBudget)
            {
                // Ha az összes játékost megnéztük, ellenőrizzük az átlagos értékelést
                if (index >= transferMarket.Count)
                {
                    var newTeam = new List<Player>(currentTeam);
                    newTeam.AddRange(currentTransfers);
                    double newAverageRating = newTeam.Average(p => p.Rating);

                    if (newAverageRating > bestAverageRating)
                    {
                        bestAverageRating = newAverageRating;
                        bestTeam = new List<Player>(newTeam);
                    }

                    return;
                }

                // Ne vegyük fel az aktuális játékost
                Backtrack(index + 1, remainingBudget);

                // Vegyük fel az aktuális játékost, ha belefér a költségvetésbe
                var player = transferMarket[index];
                if (player.MarketValue <= remainingBudget)
                {
                    currentTransfers.Add(player);
                    Backtrack(index + 1, remainingBudget - player.MarketValue);
                    currentTransfers.RemoveAt(currentTransfers.Count - 1); // Visszalépés
                }
            }

            Backtrack(0, budget);

            // Csak az újonnan igazolt játékosokat adja vissza
            return bestTeam.Except(currentTeam).ToList();
        }
    }
}
