using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FutballSimulator
{
    /// <summary>
    /// Statikus osztály a szezon szimulálásához szükséges metódusokkal
    /// </summary>
    public static class SeasonSimulator
    {
        /// <summary>
        /// Manuális szezon szimuláció.
        /// </summary>
        /// <param name="teams">A szezonban szereplő csapatok listája.</param>
        /// <param name="fehervar">Fehérvár FC csapat objektuma.</param>
        public static void SimulateSeason(List<Team> teams, Team fehervar)
        {
            int currentRound = LoadSeasonState(teams, fehervar, out var table); //szezon aktuális állapota, amely alapján bármikor bárhonnan lehet folytatni egy megkezdett szezont
            if (table == null || table.Count == 0) //megvizsgálja, hogy van-e már táblázat (ha nincs akkor nincs megkezdett szezon)
            {
                Console.WriteLine("Nincs érvényes mentés. Új tabella létrehozása...");
                table = InitializeTable(teams, fehervar);
                currentRound = 0;
            }

            var matchups = GenerateSeasonMatchups(teams, fehervar);
            var matchResults = new Dictionary<(string, string), (int, int)>(); //szótár, amely az meccsek eredményeit tartalmzza

            //bajnokság 33 meccses, adig fut a ciklus maximum
            for (int round = currentRound + 1; round <= 33; round++) 
            {
                Console.Clear();

                var formation = ChooseFormation(); //formáció választása minden meccs előtt

                Console.Clear();
                Console.WriteLine($"\n{round}. forduló eredményei:");

                SimulateRound(matchups[round - 1], table, formation, matchResults); //fordulók szimulálásának meghívása
                SaveRoundResults(matchups[round - 1], table, round, "eredmenyek", "manual", matchResults); //fordulók mentésének meghívása
                SaveSeasonState(table, round); //szezon aktuális állapotának mentése

                //Eredmény kiírása minden forduló után
                foreach (var (home, away) in matchups[round - 1])
                {
                    var (homeGoals, awayGoals) = matchResults[(home.Name, away.Name)];
                    Console.WriteLine($"{home.Name} {homeGoals} - {awayGoals} {away.Name}");
                }

                Console.WriteLine("\nNyomj meg egy gombot a tabella megtekintéséhez...");
                Console.ReadKey();

                Console.Clear();

                DisplayTable(table); //tabella kiírása minden forduló után
                SaveTableToFile(table, round, "manual", false); //tabella mentése fájlba

                Console.Write("\nSzeretnéd folytatni a következő fordulóval? (i/n): ");
                if (Console.ReadLine()?.ToLower() != "i")
                {
                    break;
                }
            }

            Console.Clear();
            Console.WriteLine("\nA szezon mentésre került. Később folytathatja!");
        }







        /// <summary>
        /// Tabella inicializálása.
        /// </summary>
        private static Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> InitializeTable(List<Team> teams, Team fehervar)
        {
            var table = new Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)>();

            foreach (var team in teams)
            {
                if (!table.ContainsKey(team.Name)) // Elkerüljük a duplikációkat
                {
                    table[team.Name] = (0, 0, 0, 0, 0, 0, 0);
                }
            }

            if (!table.ContainsKey(fehervar.Name)) // Fehérvár FC alapból nincs abban a fájlban amiből ez a függvény kivonaja a csapatok neveit, ezért hozzá kell adni
            {
                table[fehervar.Name] = (0, 0, 0, 0, 0, 0, 0);
            }

            return table;
        }




        /// <summary>
        /// Szezon párosításainak generálása.
        /// </summary>
        private static List<List<(Team Home, Team Away)>> GenerateSeasonMatchups(List<Team> teams, Team fehervar)
        {
            var allTeams = new List<Team>(teams) { fehervar }; // Fehérvár hozzáadása
            var matchups = new List<List<(Team Home, Team Away)>>();

            for (int cycle = 0; cycle < 3; cycle++) // Három kör a szezonban
            {
                var roundRobinRounds = RoundRobin(allTeams, cycle % 2 == 0); // RoundRobin generálás
                matchups.AddRange(roundRobinRounds);
            }

            return matchups;
        }

        /// <summary>
        /// Round Robin algoritmus, amely az aktuális párosításokat generálja.
        /// A Round Robin algoritmus egy fix csapatot (az elsőt) helyben tart, és a többit mindig forgatja, hogy minden csapat játszhasson mindenki ellen
        /// Az egymás mellett levő csapatok játszanak egymás ellen
        /// </summary>
        private static List<List<(Team Home, Team Away)>> RoundRobin(List<Team> teams, bool homeFirst)
        {
            var rounds = new List<List<(Team Home, Team Away)>>();
            var teamCount = teams.Count;

            // Pihenő csapat hozzáadása, ha páratlan a létszám
            if (teamCount % 2 != 0)
            {
                teams.Add(new Team { Name = "Pihenő" });
                teamCount++;
            }

            // Fordulók generálása
            for (int round = 0; round < teamCount - 1; round++)
            {
                var roundMatchups = new List<(Team Home, Team Away)>();

                for (int i = 0; i < teamCount / 2; i++)
                {
                    var home = teams[i];
                    var away = teams[teamCount - 1 - i];

                    roundMatchups.Add(homeFirst ? (home, away) : (away, home)); //minden csapat felváltva játszik otthon és idegenben, a ? pedig ezt igyekszik eldönteni minden csapatnál
                }

                rounds.Add(roundMatchups);

                // Csapatok rotálása
                // Ez egy körbeforgatás (rotation) a Round Robin párosítás generálás részeként.
                // Ezzel biztosítjuk, hogy minden csapat más - más csapatok ellen játszhasson a szezonban
                var lastTeam = teams[teamCount - 1]; //megkeresi a sorrendben utolsó csapatot
                teams.RemoveAt(teamCount - 1); //kitörli a megtalált csapatot
                teams.Insert(1, lastTeam); //beszúrja az első indexre (ami sorrendben a második)
            }

            return rounds;
        }

        /// <summary>
        /// Forduló szimulálása az adott párosításokkal.
        /// </summary>
        private static void SimulateRound(List<(Team Home, Team Away)> roundMatchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, (int Defenders, int Midfielders, int Forwards) formation, Dictionary<(string, string), (int, int)> matchResults)
        {
            var random = new Random();

            foreach (var (home, away) in roundMatchups)
            {
                var homeGoals = random.Next(0, 5);
                var awayGoals = random.Next(0, 5);

                
                matchResults[(home.Name, away.Name)] = (homeGoals, awayGoals);

                UpdateTable(table, home, away, homeGoals, awayGoals);
            }
        }

        /// <summary>
        /// Tabella frissítése a meccseredmények alapján.
        /// </summary>
        private static void UpdateTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, Team home, Team away, int homeGoals, int awayGoals)
        {
            if (!table.ContainsKey(home.Name)) // Hibakezelés hozzáadása
            {
                Console.WriteLine($"HIBA: {home.Name} nem található a tabellában!");
                return;
            }

            if (!table.ContainsKey(away.Name)) // Hibakezelés hozzáadása
            {
                Console.WriteLine($"HIBA: {away.Name} nem található a tabellában!");
                return;
            }

            var homeStats = table[home.Name];
            var awayStats = table[away.Name];

            // A szimulált eredmény alapján frissül a táblázat minden cellája
            if (homeGoals > awayGoals)
            {
                table[home.Name] = (homeStats.Points + 3, homeStats.GoalsFor + homeGoals, homeStats.GoalsAgainst + awayGoals, homeStats.PlayedMatches + 1, homeStats.Wins + 1, homeStats.Draws, homeStats.Losses);
                table[away.Name] = (awayStats.Points, awayStats.GoalsFor + awayGoals, awayStats.GoalsAgainst + homeGoals, awayStats.PlayedMatches + 1, awayStats.Wins, awayStats.Draws, awayStats.Losses + 1);
            }
            else if (homeGoals < awayGoals)
            {
                table[away.Name] = (awayStats.Points + 3, awayStats.GoalsFor + awayGoals, awayStats.GoalsAgainst + homeGoals, awayStats.PlayedMatches + 1, awayStats.Wins + 1, awayStats.Draws, awayStats.Losses);
                table[home.Name] = (homeStats.Points, homeStats.GoalsFor + homeGoals, homeStats.GoalsAgainst + awayGoals, homeStats.PlayedMatches + 1, homeStats.Wins, homeStats.Draws, homeStats.Losses + 1);
            }
            else
            {
                table[home.Name] = (homeStats.Points + 1, homeStats.GoalsFor + homeGoals, homeStats.GoalsAgainst + awayGoals, homeStats.PlayedMatches + 1, homeStats.Wins, homeStats.Draws + 1, homeStats.Losses);
                table[away.Name] = (awayStats.Points + 1, awayStats.GoalsFor + awayGoals, awayStats.GoalsAgainst + homeGoals, awayStats.PlayedMatches + 1, awayStats.Wins, awayStats.Draws + 1, awayStats.Losses);
            }
        }




        /// <summary>
        /// Tabella megjelenítése a konzolon.
        /// </summary>
        private static void DisplayTable(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table)
        {
            Console.WriteLine("\n--- Tabella ---");
            Console.WriteLine("Helyezés | Csapat            | LM | GY | D  | V  | LG | KG | GK | PSZ");
            Console.WriteLine(new string('-', 70));

            // Meghatározza, hogy azonos pontsám esetén mi rangsorol
            var sortedTable = table.OrderByDescending(t => t.Value.Points)
                                   .ThenByDescending(t => t.Value.GoalsFor - t.Value.GoalsAgainst) // Gólkülönbség
                                   .ThenByDescending(t => t.Value.GoalsFor) // Lőtt gólok
                                   .ToList();

            int rank = 1;
            foreach (var kvp in sortedTable)
            {
                string teamName = kvp.Key;
                var stats = kvp.Value;
                int goalDifference = stats.GoalsFor - stats.GoalsAgainst;

                //Tábálzat kiiratása
                // A számok meghatározzák, hogy a táblázat egyes sorai milyen szélesek legyenek
                Console.WriteLine($"{rank,8} | {teamName,-16} | {stats.PlayedMatches,2} | {stats.Wins,2} | {stats.Draws,2} | {stats.Losses,2} | {stats.GoalsFor,2} | {stats.GoalsAgainst,2} | {goalDifference,3} | {stats.Points,3}");
                rank++;
            }
        }


        /// <summary>
        /// Fordulók eredményeinek mentése egy fájlba az adott szezonhoz.
        /// </summary>
        /// <param name="matchups">Az aktuális forduló mérkőzéseinek párosításai.</param>
        /// <param name="table">Az aktuális tabella állása.</param>
        /// <param name="round">A forduló száma.</param>
        /// <param name="folder">A mappa neve, ahová az eredményeket mentjük.</param>
        /// <param name="keretNev">A kiválasztott keret neve.</param>
        /// <param name="matchResults">A meccs eredménye</param>
        private static void SaveRoundResults(List<(Team Home, Team Away)> matchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, int round, string folder, string keretNev, Dictionary<(string, string), (int HomeGoals, int AwayGoals)> matchResults)
        {
            string seasonIdentifier = DateTime.Now.ToString("yyyy-MM-dd");
            string resultsFile = $"{folder}/szezon_{seasonIdentifier}_{keretNev}_eredmenyek.txt";

            using (StreamWriter writer = new StreamWriter(resultsFile, true))
            {
                writer.WriteLine($"--- {round}. forduló eredményei ---");

                foreach (var (home, away) in matchups)
                {
                    if (matchResults.ContainsKey((home.Name, away.Name)))
                    {
                        var (homeGoals, awayGoals) = matchResults[(home.Name, away.Name)];
                        writer.WriteLine($"{home.Name} {homeGoals} - {awayGoals} {away.Name}");
                    }
                    else
                    {
                        writer.WriteLine($"{home.Name} ? - ? {away.Name}");
                    }
                }
                writer.WriteLine();
            }
        }



        /// <summary>
        /// Tabella mentése fájlba fordulónként.
        /// Manuális szezon esetén az "eredmenyek/" mappába, automatikus szezon esetén a "szimulalteredmenyek/" mappába.
        /// </summary>
        /// <param name="table">Az aktuális tabella állása.</param>
        /// <param name="round">A forduló száma.</param>
        /// <param name="keretNev">A kiválasztott keret neve.</param>
        /// <param name="isAutomatic">Igaz, ha automatikus szezon szimuláció történik.</param>
        private static void SaveTableToFile(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, int round, string keretNev, bool isAutomatic)
        {
            // Ha automatikus szezon szimuláció fut, a fájl a "szimulalteredmenyek" mappába kerül
            string directory = isAutomatic ? "szimulalteredmenyek" : "eredmenyek";

            // Ha a mappa nem létezik, létrehozzuk
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 🟢 Manuális módban mindig felülírja, automatikus módban hozzáfűzi
            string filePath = $"{directory}/tabella_{keretNev}.txt";
            bool append = isAutomatic;

            using (StreamWriter writer = new StreamWriter(filePath, append)) // Ha manuális, felülírja; ha automatikus, hozzáfűzi
            {
                writer.WriteLine($"--- Tabella - {round}. forduló ---");
                writer.WriteLine("Helyezés | Csapat            | LM | LG | KG | GK | GY | D | V | PSZ");
                writer.WriteLine(new string('-', 60));

                var sortedTable = table.OrderByDescending(t => t.Value.Points)
                                       .ThenByDescending(t => t.Value.GoalsFor - t.Value.GoalsAgainst)
                                       .ThenByDescending(t => t.Value.GoalsFor)
                                       .ToList();

                int rank = 1;
                foreach (var kvp in sortedTable)
                {
                    string teamName = kvp.Key;
                    var stats = kvp.Value;
                    int goalDifference = stats.GoalsFor - stats.GoalsAgainst;

                    writer.WriteLine($"{rank,8} | {teamName,-16} | {stats.PlayedMatches,2} | {stats.GoalsFor,2} | {stats.GoalsAgainst,2} | {goalDifference,3} | {stats.Wins,2} | {stats.Draws,2} | {stats.Losses,2} | {stats.Points,3}");
                    rank++;
                }

                writer.WriteLine();
            }

            Console.WriteLine($"Tabella mentve: {filePath}");
        }




        /// <summary>
        /// Szezon állapotának mentése fájlba.
        /// </summary>
        private static void SaveSeasonState(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, int currentRound)
        {
            string filePath = "eredmenyek/season_state.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(currentRound);

                foreach (var kvp in table)
                {
                    writer.WriteLine($"{kvp.Key};{kvp.Value.Points};{kvp.Value.GoalsFor};{kvp.Value.GoalsAgainst};{kvp.Value.PlayedMatches};{kvp.Value.Wins};{kvp.Value.Draws};{kvp.Value.Losses}");
                }
            }
        }


        /// <summary>
        /// Felállás kiválasztása a szimulációhoz.
        /// </summary>
        private static (int Defenders, int Midfielders, int Forwards) ChooseFormation()
        {
            Console.WriteLine("\nVálassz egy felállást a következők közül:");
            Console.WriteLine("1. 4-4-2");
            Console.WriteLine("2. 4-3-3");
            Console.WriteLine("3. 3-5-2");
            Console.WriteLine("4. 5-3-2");

            Console.Write("Választásod: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    return (4, 4, 2);
                case 2:
                    return (4, 3, 3);
                case 3:
                    return (3, 5, 2);
                case 4:
                    return (5, 3, 2);
                default:
                    return (4, 4, 2); // Alapértelmezett felállás
            }

        }

        /// <summary>
        /// Automatikus szezon szimuláció az összes forduló végigjátszásával.
        /// Az eredmények és a végső tabella a `szimulalteredmenyek/` mappába kerülnek.
        /// </summary>
        /// <param name="teams">A szezonban szereplő csapatok listája.</param>
        /// <param name="fehervar">A Fehérvár FC csapat objektuma.</param>
        public static void SimulateFullSeasonAutomatically(List<Team> teams, Team fehervar)
        {
            var table = InitializeTable(teams, fehervar);
            var matchups = GenerateSeasonMatchups(teams, fehervar);
            var matchResults = new Dictionary<(string, string), (int, int)>(); // meccs eredmények

            for (int round = 1; round <= 33; round++)
            {
                Console.WriteLine($"\n{round}. forduló eredményei szimulálása...");

                SimulateRound(matchups[round - 1], table, (4, 4, 2), matchResults); // fordulók szimulálása, matchResults átadása
                SaveRoundResults(matchups[round - 1], table, round, "szimulalteredmenyek", "auto", matchResults); // fordulók eredményének mentése, matchResults átadása
            }

            SaveFinalTableToFile(table, "szimulalteredmenyek", "auto"); // a file amibe a tabella kerül mindig felülíródik, így a végén az a tabella lesz benne ami a végeredményt tartalmazza
            Console.Clear();
            Console.WriteLine("\n--- Végső Tabella ---");
            DisplayTable(table);
            Console.WriteLine("\nAz automatikus szezon szimuláció véget ért!");
            Console.ReadKey();
        }



        /// <summary>
        /// Végső tabella mentése fájlba a szezon végén.
        /// </summary>
        /// <param name="table">Az aktuális tabella állása.</param>
        /// <param name="folder">A mappa neve, ahová az eredményeket mentjük.</param>
        /// <param name="keretNev">A kiválasztott keret neve.</param>
        private static void SaveFinalTableToFile(Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, string folder, string keretNev)
        {
            // Az aktuális dátum alapján egyedi szezonazonosító generálása
            string seasonIdentifier = DateTime.Now.ToString("yyyy-MM-dd");

            // Fájl elérési útvonalának beállítása
            string filePath = $"{folder}/szezon_{seasonIdentifier}_{keretNev}_veges_tabella.txt";

            using (StreamWriter writer = new StreamWriter(filePath, false)) // Felülírja az előző szezon végét
            {
                writer.WriteLine("--- Végső Tabella ---");
                writer.WriteLine("Helyezés | Csapat            | LM | LG | KG | GK | GY | D | V | PSZ");
                writer.WriteLine(new string('-', 60));

                // Tabella rendezése pontszám, gólkülönbség és lőtt gól alapján
                var sortedTable = table.OrderByDescending(t => t.Value.Points)
                                       .ThenByDescending(t => t.Value.GoalsFor - t.Value.GoalsAgainst)
                                       .ThenByDescending(t => t.Value.GoalsFor)
                                       .ToList();

                int rank = 1;
                foreach (var kvp in sortedTable)
                {
                    string teamName = kvp.Key;
                    var stats = kvp.Value;
                    int goalDifference = stats.GoalsFor - stats.GoalsAgainst;

                    writer.WriteLine($"{rank,8} | {teamName,-16} | {stats.PlayedMatches,2} | {stats.GoalsFor,2} | {stats.GoalsAgainst,2} | {goalDifference,3} | {stats.Wins,2} | {stats.Draws,2} | {stats.Losses,2} | {stats.Points,3}");
                    rank++;
                }
            }

            Console.WriteLine($"Végső tabella mentve: {filePath}");
        }



        /// <summary>
        /// A szezon aktuális állapotának betöltése, hogy ott lehessen folytatni ahol abbahagytuk.
        /// </summary>
        /// <param name="teams"></param>
        /// <param name="fehervar"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private static int LoadSeasonState(List<Team> teams, Team fehervar, out Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table)
        {
            string filePath = "eredmenyek/season_state.txt";
            table = new Dictionary<string, (int, int, int, int, int, int, int)>();

            if (!File.Exists(filePath))
            {
                return 0; // Ha nincs fájl, akkor 0-ról indul a szezon
            }

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length == 0)
            {
                return 0;
            }

            int round = int.Parse(lines[0]); // Az első sor az aktuális forduló

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length == 8)
                {
                    string teamName = parts[0];
                    int points = int.Parse(parts[1]);
                    int goalsFor = int.Parse(parts[2]);
                    int goalsAgainst = int.Parse(parts[3]);
                    int playedMatches = int.Parse(parts[4]);
                    int wins = int.Parse(parts[5]);
                    int draws = int.Parse(parts[6]);
                    int losses = int.Parse(parts[7]);

                    table[teamName] = (points, goalsFor, goalsAgainst, playedMatches, wins, draws, losses);
                }
            }

            // HIÁNYZÓ CSAPATOK AUTOMATIKUS HOZZÁADÁSA
            foreach (var team in teams)
            {
                if (!table.ContainsKey(team.Name))
                {
                    table[team.Name] = (0, 0, 0, 0, 0, 0, 0);
                }
            }

            if (!table.ContainsKey(fehervar.Name))
            {
                table[fehervar.Name] = (0, 0, 0, 0, 0, 0, 0);
            }

            return round; // Visszaadjuk az aktuális forduló számát
        }

    }
}
