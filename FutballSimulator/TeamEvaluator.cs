using System;
using System.Linq;

namespace FutballSimulator
{
    public static class TeamEvaluator
    {
        /// <summary>
        /// Pozíciók kiértékelése egy csapatnál.
        /// </summary>
        /// <param name="team">A csapat, amelyet kiértékelünk.</param>
        public static void EvaluateTeamPositions(Team team)
        {
            Console.WriteLine($"Pozíciók kiértékelése a {team.Name} csapatában:");

            // Játékosok csoportosítása pozíciók szerint
            var positions = team.Players.GroupBy(player => player.Position);

            // Pozíciónként kiírás
            foreach (var position in positions)
            {
                Console.WriteLine($"{position.Key}: {position.Count()} játékos, átlagos értékelés: {position.Average(p => p.Rating):F1}");
            }

            Console.WriteLine();
        }
    }
}
