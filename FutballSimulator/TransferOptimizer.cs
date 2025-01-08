using System;
using System.Collections.Generic;
using System.Linq;

namespace FutballSimulator
{
    public static class TransferOptimizer
    {
        /// <summary>
        /// Átigazolások optimalizálása a megadott költségvetés és csapat alapján, prioritást adva a leggyengébb pozícióknak.
        /// </summary>
        public static List<Player> OptimizeTransfers(List<Player> transferMarket, double budget, List<Player> currentTeam, double improvementThreshold)
        {
            var bestTransfers = new List<Player>();

            // Csoportosítsuk a játékosokat pozíció szerint, és priorizáljuk a leggyengébb pozíciókat
            var positionsByWeakness = currentTeam
                .GroupBy(p => p.Position)
                .OrderBy(positionGroup => positionGroup.Average(p => p.Rating)) // A leggyengébb pozíció kerül az elejére
                .ToList();

            foreach (var position in positionsByWeakness)
            {
                double currentAverage = position.Average(p => p.Rating);
                var potentialPlayers = transferMarket
                    .Where(p => p.Position == position.Key && p.MarketValue <= budget)
                    .OrderByDescending(p => p.Rating) // Legjobb játékos először
                    .ToList();

                foreach (var player in potentialPlayers)
                {
                    if (player.Rating > currentAverage * (1 + improvementThreshold))
                    {
                        bestTransfers.Add(player);
                        budget -= player.MarketValue;
                        break; // Csak egy játékost igazolunk egy pozícióra
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
