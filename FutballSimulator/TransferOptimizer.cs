using System;
using System.Collections.Generic;
using System.Linq;

namespace FutballSimulator
{
    public static class TransferOptimizer
    {
        /// <summary>
        /// Optimalizálja az igazolásokat a megadott költségvetés alapján.
        /// Csak olyan játékosokat igazol, akik jelentős javulást hoznak az adott pozícióban.
        /// </summary>
        public static List<Player> OptimizeTransfers(List<Player> transferMarket, double budget, List<Player> currentTeam)
        {
            var bestTransfers = new List<Player>();
            var remainingBudget = budget;

            // Csoportosítsuk a jelenlegi csapatot pozíciók szerint
            var currentTeamByPosition = currentTeam.GroupBy(p => p.Position)
                                                   .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var player in transferMarket.OrderByDescending(p => p.Rating))
            {
                if (remainingBudget < player.MarketValue) continue;

                // Számítsuk ki az adott pozíció átlagát
                if (currentTeamByPosition.TryGetValue(player.Position, out var positionPlayers))
                {
                    double currentAverage = positionPlayers.Average(p => p.Rating);
                    double newAverage = (positionPlayers.Sum(p => p.Rating) + player.Rating) / (positionPlayers.Count + 1);

                    // Csak akkor igazoljunk, ha az átlag legalább 5%-kal javul
                    if (newAverage <= currentAverage * 1.02) continue;
                }

                // Ha nincs ilyen pozíció a csapatban, automatikusan igazolunk
                bestTransfers.Add(player);
                remainingBudget -= player.MarketValue;

                // Frissítsük a pozíciók szerinti csoportot
                if (!currentTeamByPosition.ContainsKey(player.Position))
                {
                    currentTeamByPosition[player.Position] = new List<Player>();
                }
                currentTeamByPosition[player.Position].Add(player);
            }

            return bestTransfers;
        }


    }
}