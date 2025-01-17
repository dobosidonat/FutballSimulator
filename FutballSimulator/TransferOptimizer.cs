using System;
using System.Collections.Generic;
using System.Linq;

namespace FutballSimulator
{
    /// <summary>
    /// Átigazolásokat optimalizálja
    /// </summary>
    public static class TransferOptimizer
    {
        /// <summary>
        /// Dinamikus programozás (DP) alapú Knapsack-algoritmust használ az átigazolások optimalizálására
        /// A cél az, hogy adott költségvetésből a legjobb értékelésű játékosokat válassza ki, és ne csak egy drága játékost vegyen
        /// </summary>
        /// <param name="transferMarket"></param>
        /// <param name="budget"></param>
        /// <param name="currentTeam"></param>
        /// <returns>bestTransfers => Legjobb játékosok az adott költségvetésből</returns>
        public static List<Player> OptimizeTransfers(List<Player> transferMarket, double budget, List<Player> currentTeam)
        {
            int n = transferMarket.Count; // A játékospiacon lévő játékosok száma
            int maxBudget = (int)budget;  // A költségvetést egész számra alakítjuk (int típusra kényszerítjük), mert a dinamikus programozás nem tud tizedes értékekkel jól dolgozni

            // DP tárolók
            double[] dp = new double[maxBudget + 1];    // az összegre elérhető legnagyobb összesített játékoserősséget tárolja
            List<int>[] chosenPlayers = new List<int>[maxBudget + 1];   // Egy lista, amely tartalmazza azoknak a játékosoknak az indexét a transferMarket-ben, akik ezt az értéket adják ki

            // Minden költségszinthez egy üres lista tartozik, amit később kitöltünk a választott játékosokkal
            for (int i = 0; i <= maxBudget; i++)
            {
                chosenPlayers[i] = new List<int>(); // Alapból üres lista minden költségpontra
            }

            // **Dinamikus Programozás (KnapSack)**
            // célunk, hogy minden játékost megnézzünk, és eldöntsük, befér-e az adott költségvetésbe
            // Végigmegyünk minden játékoson és megszerezzük az árát (cost) és értékelését (value)
            for (int i = 0; i < n; i++)
            {
                var player = transferMarket[i];
                int cost = (int)player.MarketValue;
                double rating = player.Rating;

                for (int j = maxBudget; j >= cost; j--) //Visszafelé iterálunk, megakadályozva, hogy 1 játékost többször is kiválasszunk
                {
                    double newRating = dp[j - cost] + rating; // Ha egy j - cost összegért már van egy jó csapatunk, akkor megnézzük, hogy ha ezt a játékost hozzáadjuk, jobb lesz-e az összértékelés

                    if (newRating > dp[j]) // Ha jobb kombinációt találunk
                    {
                        dp[j] = newRating;   // Frissítjük a maximális értékelést
                        chosenPlayers[j] = new List<int>(chosenPlayers[j - cost]) { i }; // Új listát hozunk létre, amely a korábbi csapatból áll, plusz az új játékos indexével
                    }
                }
            }


            // **Legjobb játékosok kiválasztása**
            // Miután az összes lehetőséget végignéztük, ki kell választanunk azt az értéket, amely a legjobb teljesítményt adja
            // Végigmegyünk az összes lehetséges költségszinten és kiválasztjuk azt, amely a legnagyobb értékelést adja ki
            int bestBudget = 0;
            for (int i = 0; i <= maxBudget; i++)
            {
                if (dp[i] > dp[bestBudget])
                {
                    bestBudget = i;
                }
            }

            //Az indexek alapján kiszedjük a legjobb játékosokat a transferMarket-ből
            List<Player> bestTransfers = chosenPlayers[bestBudget].Select(index => transferMarket[index]).ToList();
            return bestTransfers;
        }
    }
}
