using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutballSimulator
{
    /// <summary>
    /// Statikus osztály a játékosigazolások optimalizálására a csapat költségvetése alapján.
    /// </summary>
    public class TransferOptimizer
    {
        /// <summary>
        /// Megkeresi a költségvetés alapján a legjobb játékos-kombinációt.
        /// </summary>
        /// <param name="transferMarket">Az átigazolási piacon elérhető játékosok listája.</param>
        /// <param name="budget">A csapat költségvetése.</param>
        /// <returnsA legjobb játékosok listája.></returns>
        public static List<Player> OptimizeTransfers(List<Player> transferMarket, double budget)
        {
            List<Player> bestTeam = new List<Player>();
            List<Player> currentTeam = new List<Player>();
            double bestStrength = 0;

            void Backtrack(int index, double remainingBudget, double currentStrength)
            {
                if (currentStrength > bestStrength)
                {
                    bestStrength = currentStrength;
                    bestTeam = new List<Player>(currentTeam);
                }

                if (index >= transferMarket.Count || remainingBudget <= 0)
                {
                    return;
                }

                // Az aktuális játékos kihagyása
                Backtrack(index + 1, remainingBudget, currentStrength);

                // Az aktuális játékos hozzáadása, ha belefér a költségvetésbe
                var player = transferMarket[index];
                if (player.MarketValue <= remainingBudget)
                {
                    currentTeam.Add(player);
                    Backtrack(index + 1, remainingBudget - player.MarketValue, currentStrength + player.Rating);
                    currentTeam.RemoveAt(currentTeam.Count - 1);
                }
            }

            Backtrack(0, budget, 0);
            return bestTeam;
        }
    }
}
