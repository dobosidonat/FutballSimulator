using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FutballSimulator
{
    /// <summary>
    /// Statikus osztály a csapat költségvetésének kezeléséhez.
    /// </summary>
    public static class BudgetHandler
    {
        /// <summary>
        /// Betölti a költségvetést egy szöveges fájlból.
        /// </summary>
        /// <param name="filePath">A fájl elérési útvonala.</param>
        /// <returns>A betöltött költségvetés.</returns>
        public static double LoadBudgetFromFile(string filePath)
        {
            return double.Parse(File.ReadAllText(filePath).Trim());
        }

        /// <summary>
        /// Ment egy költségvetési értéket egy szöveges fájlba.
        /// </summary>
        /// <param name="budget">A mentendő költségvetés.</param>
        /// <param name="filePath">A fájl elérési útvonala.</param>
        public static void SaveBudgetToFile(double budget, string filePath)
        {
            File.WriteAllText(filePath, budget.ToString());
        }
    }
}
