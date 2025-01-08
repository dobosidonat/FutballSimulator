using System;
using System.Linq;

namespace FutballSimulator
{
    public static class TeamEvaluator
    {
        public static void EvaluateTeamPositions(Team team)
        {
            Console.WriteLine($"Pozíciók kiértékelése a {team.Name} csapatában:");
            var positions = team.Players.GroupBy(p => p.Position);

            foreach (var position in positions)
            {
                Console.WriteLine($"{position.Key}: {position.Count()} játékos, átlagos értékelés: {position.Average(p => p.Rating):F1}");
            }
        }
    }
}