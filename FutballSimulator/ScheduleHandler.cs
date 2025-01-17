//using System;
//using System.Collections.Generic;

//namespace FutballSimulator
//{
//    public static class ScheduleHandler
//    {
//        /// <summary>
//        /// Sorsolás generálása hazai és idegenbeli mérkőzésekkel.
//        /// </summary>
//        public static List<(string homeTeam, string awayTeam)> GenerateSchedule(List<string> teams)
//        {
//            var schedule = new List<(string homeTeam, string awayTeam)>();

//            for (int i = 0; i < teams.Count; i++)
//            {
//                for (int j = 0; j < teams.Count; j++)
//                {
//                    if (i != j)
//                    {
//                        schedule.Add((teams[i], teams[j]));
//                    }
//                }
//            }

//            return schedule;
//        }
//    }
//}
