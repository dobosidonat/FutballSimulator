using System;

namespace FutballSimulator
{
    public static class MatchSimulator
    {
        /// <summary>
        /// Mérkőzés szimulálása csapatrészek átlagos értékelése alapján.
        /// </summary>
        /// <param name="home">Hazai csapat.</param>
        /// <param name="away">Vendég csapat.</param>
        /// <returns>A mérkőzés eredménye (hazai és vendég gólok száma).</returns>
        public static (int HomeGoals, int AwayGoals) SimulateMatch(Team home, Team away)
        {
            Random random = new Random();
            int homeGoals = random.Next(0, 4); // Hazai csapat max 3 gól
            int awayGoals = random.Next(0, 3); // Vendégcsapat max 2 gól

            // Hazai előny: növeljük a hazai csapat góljait
            if (random.NextDouble() < 0.2) // 20% esély extra gólra
            {
                homeGoals++;
            }

            return (homeGoals, awayGoals);
        }

    }
}
