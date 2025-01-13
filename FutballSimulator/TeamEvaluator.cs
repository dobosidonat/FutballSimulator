using System.Linq;

namespace FutballSimulator
{
    public static class TeamEvaluator
    {
        /// <summary>
        /// Kiértékeli a csapatrészek (védelem, középpálya, támadósor, kapus) átlagos értékelését.
        /// </summary>
        /// <param name="team">A kiértékelendő csapat.</param>
        /// <returns>Csapatrészenkénti átlagos értékelések.</returns>
        public static (double DefenseRating, double MidfieldRating, double ForwardRating, double GoalkeeperRating) EvaluateTeamRatings(Team team)
        {
            // Védelem (DF) átlagos értékelése
            double defense = team.Players.Where(p => p.Position == "DF").Average(p => p.Rating);

            // Középpálya (MF) átlagos értékelése
            double midfield = team.Players.Where(p => p.Position == "MF").Average(p => p.Rating);

            // Támadósor (FW) átlagos értékelése
            double forward = team.Players.Where(p => p.Position == "FW").Average(p => p.Rating);

            // Kapus (GK) átlagos értékelése
            double goalkeeper = team.Players.Where(p => p.Position == "GK").Average(p => p.Rating);

            // Visszaadjuk a csapatrészek értékelését
            return (defense, midfield, forward, goalkeeper);
        }
    }
}
