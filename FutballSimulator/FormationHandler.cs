using System;
using System.Collections.Generic;
using System.Linq;

namespace FutballSimulator
{
    public static class FormationHandler
    {
        public static (int defenders, int midfielders, int forwards) GetFormation()
        {
            Console.WriteLine("Add meg a kívánt felállást (pl. 4-4-2):");
            string input = Console.ReadLine();
            var parts = input.Split('-');

            if (parts.Length != 3 ||
                !int.TryParse(parts[0], out int defenders) ||
                !int.TryParse(parts[1], out int midfielders) ||
                !int.TryParse(parts[2], out int forwards))
            {
                Console.WriteLine("Érvénytelen formátum. Alapértelmezett: 4-4-2.");
                return (4, 4, 2);
            }

            return (defenders, midfielders, forwards);
        }

        public static List<Player> GetBestTeam(Team team, int defenders, int midfielders, int forwards)
        {
            var bestDefenders = team.Players.Where(p => p.Position == "DF")
                                            .OrderByDescending(p => p.Rating)
                                            .Take(defenders).ToList();

            var bestMidfielders = team.Players.Where(p => p.Position == "MF")
                                              .OrderByDescending(p => p.Rating)
                                              .Take(midfielders).ToList();

            var bestForwards = team.Players.Where(p => p.Position == "FW")
                                           .OrderByDescending(p => p.Rating)
                                           .Take(forwards).ToList();

            var bestGoalkeeper = team.Players.Where(p => p.Position == "GK")
                                             .OrderByDescending(p => p.Rating)
                                             .FirstOrDefault();

            var bestTeam = new List<Player> { bestGoalkeeper };
            bestTeam.AddRange(bestDefenders);
            bestTeam.AddRange(bestMidfielders);
            bestTeam.AddRange(bestForwards);

            return bestTeam;
        }
    }
}
