using FutballSimulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootballManagerLibrary
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

            var weakestPosition = currentTeamByPosition
                .OrderBy(p => p.Value.Average(player => player.Rating))
                .FirstOrDefault().Key;

            foreach (var player in transferMarket.OrderByDescending(p => p.Rating / p.MarketValue))
            {
                if (remainingBudget < player.MarketValue) continue;

                if (currentTeamByPosition.TryGetValue(player.Position, out var positionPlayers))
                {
                    var bestCurrentPlayer = positionPlayers.Max(p => p.Rating);

                    // Csak akkor igazoljunk, ha az új játékos legalább 5%-kal jobb, és az adott pozíció gyenge
                    if (player.Position == weakestPosition || player.Rating > bestCurrentPlayer * 1.05)
                    {
                        bestTransfers.Add(player);
                        remainingBudget -= player.MarketValue;

                        if (!currentTeamByPosition.ContainsKey(player.Position))
                        {
                            currentTeamByPosition[player.Position] = new List<Player>();
                        }
                        currentTeamByPosition[player.Position].Add(player);
                    }
                }
                else
                {
                    // Pozíció üres, automatikusan igazolunk
                    bestTransfers.Add(player);
                    remainingBudget -= player.MarketValue;
                    currentTeamByPosition[player.Position] = new List<Player> { player };
                }
            }

            return bestTransfers;
        }

    }
}
