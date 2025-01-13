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

            // Pozíciók szerinti átlagok kiszámítása
            var (defense, midfield, forward, goalkeeper) = TeamEvaluator.EvaluateTeamRatings(new Team { Players = currentTeam });

            foreach (var position in new[] { "DF", "MF", "FW", "GK" })
            {
                double currentAverage;
                switch (position)
                {
                    case "DF":
                        currentAverage = defense;
                        break;
                    case "MF":
                        currentAverage = midfield;
                        break;
                    case "FW":
                        currentAverage = forward;
                        break;
                    case "GK":
                        currentAverage = goalkeeper;
                        break;
                    default:
                        currentAverage = 0;
                        break;
                }

                var potentialPlayers = transferMarket
                    .Where(p => p.Position == position && p.MarketValue <= budget)
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
