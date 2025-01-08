using System;
using System.Collections.Generic;
using System.Linq;

namespace FutballSimulator
{
    public static class TransferOptimizer
    {
        /// <summary>
        /// Átigazolások optimalizálása a megadott költségvetés és csapat alapján.
        /// </summary>
        public static List<Player> OptimizeTransfers(List<Player> transferMarket, double budget, List<Player> currentTeam, double improvementThreshold)
        {
            var bestTransfers = new List<Player>();

            foreach (var position in currentTeam.GroupBy(p => p.Position))
            {
                double currentAverage = position.Average(p => p.Rating);
                var potentialPlayers = transferMarket
                    .Where(p => p.Position == position.Key && p.MarketValue <= budget)
                    .OrderByDescending(p => p.Rating)
                    .ToList();

                foreach (var player in potentialPlayers)
                {
                    if (player.Rating > currentAverage * (1 + improvementThreshold))
                    {
                        bestTransfers.Add(player);
                        budget -= player.MarketValue;
                        break;
                    }
                }
            }

            return bestTransfers;
        }

        /// <summary>
        /// Javulási tűréshatár meghatározása a költségvetés alapján.
        /// </summary>
        public static double GetImprovementThreshold(double budget)
        {
            if (budget <= 20_000_000) return 0.02; // 2%
            if (budget <= 50_000_000) return 0.03; // 3%
            return 0.05; // 5%
        }
    }

}