//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace FutballSimulator
//{
//    /// <summary>
//    /// A tabellához tartozó osztály, amely egy csapat teljesítményét tárolja.
//    /// </summary>
//    public class Table
//    {
//        /// <summary>
//        /// A csapat neve.
//        /// </summary>
//        public string TeamName { get; set; }

//        /// <summary>
//        /// Lejátszott mérkőzések száma.
//        /// </summary>
//        public int Played { get; set; }

//        /// <summary>
//        /// Győzelmek száma.
//        /// </summary>
//        public int Won { get; set; }

//        /// <summary>
//        /// Döntetlenek száma.
//        /// </summary>
//        public int Drawn { get; set; }

//        /// <summary>
//        /// Vereségek száma.
//        /// </summary>
//        public int Lost { get; set; }

//        /// <summary>
//        /// Lőtt gólok száma.
//        /// </summary>
//        public int GoalsFor { get; set; }

//        /// <summary>
//        /// Kapott gólok száma.
//        /// </summary>
//        public int GoalsAgainst { get; set; }

//        /// <summary>
//        /// Gólkülönbség (lőtt gólok - kapott gólok).
//        /// </summary>
//        public int GoalDifference => GoalsFor - GoalsAgainst;

//        /// <summary>
//        /// Pontszám (győzelemért 3 pont, döntetlenért 1 pont, vereségért 0 pont).
//        /// </summary>
//        public int Points => Won * 3 + Drawn;
//    }


//    public static class TableHandler
//    {
//        /// <summary>
//        /// Tabella frissítése egy mérkőzés eredménye alapján.
//        /// </summary>
//        public static void UpdateTable(Dictionary<string, Table> table, string homeTeam, int homeGoals, string awayTeam, int awayGoals)
//        {
//            if (!table.ContainsKey(homeTeam))
//                table[homeTeam] = new Table { TeamName = homeTeam };

//            if (!table.ContainsKey(awayTeam))
//                table[awayTeam] = new Table { TeamName = awayTeam };

//            var home = table[homeTeam];
//            var away = table[awayTeam];

//            home.Played++;
//            away.Played++;

//            home.GoalsFor += homeGoals;
//            home.GoalsAgainst += awayGoals;

//            away.GoalsFor += awayGoals;
//            away.GoalsAgainst += homeGoals;

//            if (homeGoals > awayGoals)
//            {
//                home.Won++;
//                away.Lost++;
//            }
//            else if (homeGoals < awayGoals)
//            {
//                away.Won++;
//                home.Lost++;
//            }
//            else
//            {
//                home.Drawn++;
//                away.Drawn++;
//            }
//        }

//        /// <summary>
//        /// Tabella megjelenítése konzolon.
//        /// </summary>
//        public static void DisplayTable(Dictionary<string, Table> table)
//        {
//            var sortedTable = table.Values.OrderByDescending(t => t.Points)
//                                          .ThenByDescending(t => t.GoalDifference)
//                                          .ThenByDescending(t => t.GoalsFor)
//                                          .ToList();

//            Console.WriteLine("\nTabella:");
//            Console.WriteLine("Helyezés | Csapat           | M | GY | D | V | LG | KG | GK | Pont");
//            Console.WriteLine("---------------------------------------------------------------");

//            for (int i = 0; i < sortedTable.Count; i++)
//            {
//                var t = sortedTable[i];
//                Console.WriteLine($"{i + 1,8} | {t.TeamName,-15} | {t.Played,2} | {t.Won,2} | {t.Drawn,2} | {t.Lost,2} | {t.GoalsFor,2} | {t.GoalsAgainst,2} | {t.GoalDifference,2} | {t.Points,4}");
//            }
//        }

//        /// <summary>
//        /// Tabella mentése fájlba.
//        /// </summary>
//        public static void SaveTable(Dictionary<string, Table> table, int round)
//        {
//            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Eredmények");
//            if (!Directory.Exists(directoryPath))
//                Directory.CreateDirectory(directoryPath);

//            string filePath = Path.Combine(directoryPath, $"tabella_round_{round}.txt");

//            using (StreamWriter writer = new StreamWriter(filePath))
//            {
//                var sortedTable = table.Values.OrderByDescending(t => t.Points)
//                                              .ThenByDescending(t => t.GoalDifference)
//                                              .ThenByDescending(t => t.GoalsFor)
//                                              .ToList();

//                writer.WriteLine("Tabella:");
//                writer.WriteLine("Helyezés | Csapat           | M | GY | D | V | LG | KG | GK | Pont");
//                writer.WriteLine("------------------------------------------------------------------");

//                for (int i = 0; i < sortedTable.Count; i++)
//                {
//                    var t = sortedTable[i];
//                    writer.WriteLine($"{i + 1,8} | {t.TeamName,-15} | {t.Played,2} | {t.Won,2} | {t.Drawn,2} | {t.Lost,2} | {t.GoalsFor,2} | {t.GoalsAgainst,2} | {t.GoalDifference,2} | {t.Points,4}");
//                }
//            }
//        }
//    }
//}
