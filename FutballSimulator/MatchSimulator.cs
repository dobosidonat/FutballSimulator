using System;

namespace FutballSimulator
{
    public static class MatchSimulator
    {
        /// <summary>
        /// Mérkőzés szimulálása csapatrészek átlagos értékelése alapján.
        /// </summary>
        /// <param name="homeTeam">Hazai csapat.</param>
        /// <param name="awayTeam">Vendég csapat.</param>
        /// <returns>A mérkőzés eredménye (hazai és vendég gólok száma).</returns>
        public static (int homeGoals, int awayGoals) SimulateMatch(Team homeTeam, Team awayTeam)
        {
            // Csapatrészenkénti értékelések kiértékelése mindkét csapatnál
            var (homeDefense, homeMidfield, homeForward, homeGoalkeeper) = TeamEvaluator.EvaluateTeamRatings(homeTeam);
            var (awayDefense, awayMidfield, awayForward, awayGoalkeeper) = TeamEvaluator.EvaluateTeamRatings(awayTeam);

            Random random = new Random();

            // Hazai és vendég csapat támadási és védekezési erőssége
            double homeAttackStrength = (homeMidfield + homeForward) / 2 + 5; // Hazai előny
            double awayAttackStrength = (awayMidfield + awayForward) / 2;

            // Gólok szimulálása
            int homeGoals = (int)(random.NextDouble() * homeAttackStrength / (awayDefense + awayGoalkeeper) * 2);
            int awayGoals = (int)(random.NextDouble() * awayAttackStrength / (homeDefense + homeGoalkeeper) * 2);

            return (homeGoals, awayGoals);
        }
    }
}
