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
        public static Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> InitializeTable(List<Team> teams, Team fehervar)
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

        // Egyetlen Random objektum az egész szezonra, így minden fordulóban valódi véletlenszerűség lesz
        private static readonly Random random = new Random();

        /// <summary>
        /// Egy forduló (játékhet) szimulációja, ahol minden csapat lejátszik egy mérkőzést.
        /// A gólokat a csapatok értékelése alapján generálja, és frissíti a tabellát.
        /// </summary>
        /// <param name="roundMatchups">A forduló párosításai</param>
        /// <param name="table">Az aktuális bajnoki tabella</param>
        /// <param name="formation">A csapatok választott formációja (nem használt jelenleg, de később bővíthető)</param>
        /// <param name="matchResults">A szótár, amely eltárolja a mérkőzések eredményeit</param>
        public static void SimulateRound(List<(Team Home, Team Away)> roundMatchups, Dictionary<string, (int Points, int GoalsFor, int GoalsAgainst, int PlayedMatches, int Wins, int Draws, int Losses)> table, (int Defenders, int Midfielders, int Forwards) formation, Dictionary<(string, string), (int, int)> matchResults)
        {
            foreach (var (home, away) in roundMatchups) // Végigmegyünk minden meccspárosításon
            {
                // Csapatok értékelésének meghatározása (játékosok átlagos értékelése alapján)
                double homeRating = home.Players.Average(p => p.Rating);
                double awayRating = away.Players.Average(p => p.Rating);

                // Hazai pálya előnyének figyelembevétele
                double homeAdvantage = 1.1;  // 10%-os erősítés hazai csapatnak
                double adjustedHomeRating = homeRating * homeAdvantage;
                double adjustedAwayRating = awayRating;

                // Gólok számának előrejelzése a csapatok teljesítménye alapján
                double baseGoals = 1.5; // Egy csapat átlagosan ennyi gólt szerez egy meccsen
                double homeExpectedGoals = baseGoals * (adjustedHomeRating / 100); // Hazai gólok várható száma
                double awayExpectedGoals = baseGoals * (adjustedAwayRating / 100); // Vendég gólok várható száma

                // Valódi gólok generálása (Poisson-eloszlás vagy más módszerrel)
                int homeGoals = GenerateGoals(homeExpectedGoals);
                int awayGoals = GenerateGoals(awayExpectedGoals);

                // Túl sok döntetlen csökkentése (25% eséllyel módosítunk rajta)
                if (homeGoals == awayGoals && random.NextDouble() < 0.25)
                {
                    if (random.NextDouble() < 0.5) homeGoals++; // Hazai csapat kap egy extra gólt
                    else awayGoals++; // Vendég csapat kap egy extra gólt
                }

                // Meccs eredményének mentése a `matchResults` szótárba
                matchResults[(home.Name, away.Name)] = (homeGoals, awayGoals);

                // Tabella frissítése a meccs eredménye alapján
                UpdateTable(table, home, away, homeGoals, awayGoals);
            }
        }





        /// <summary>
        /// Poisson-alapú gólgenerálás, hogy reálisabb meccseredményeket kapjunk.
        /// </summary>
        /// <param name="averageGoals">A csapat átlagos góltermése.</param>
        /// <returns>A generált gólok száma.</returns>
        private static int GenerateGoals(double averageGoals)
        {
            Random random = new Random();
            double lambda = averageGoals; // Átlagos gólok száma
            double L = Math.Exp(-lambda);
            int k = 0;
            double p = 1.0;

            do
            {
                k++;
                p *= random.NextDouble();
            } while (p > L);

            return k - 1;
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
        /// <param name="teams">A csapatok kivéve a Fehérvár FC</param>
        /// <param name="fehervar">Fehérvár FC</param>
        public static void SimulateFullSeasonAutomatically(List<Team> teams, Team fehervar)
        {
            // 🔹 1. Keretek mappa beolvasása
            string keretMappa = "keretek";
            var availableKits = Directory.GetFiles(keretMappa, "*.txt").Select(Path.GetFileNameWithoutExtension).ToList();

            if (availableKits.Count == 0)
            {
                Console.WriteLine("Nincsenek elérhető keretek!");
                return;
            }

            // 🔹 2. Keretek listázása a konzolon
            Console.WriteLine("\nVálassz egy keretet a következők közül:");
            for (int i = 0; i < availableKits.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableKits[i]}");
            }

            // 🔹 3. Felhasználó input bekérése
            Console.Write("\nAdd meg a választott keret számát: ");
            int selectedKitIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedKitIndex) || selectedKitIndex < 1 || selectedKitIndex > availableKits.Count)
            {
                Console.Write("Hibás bemenet! Adj meg egy érvényes számot: ");
            }

            // 🔹 4. Kiválasztott keret beolvasása (csak a Fehérvár FC-re vonatkozóan)
            string chosenKit = availableKits[selectedKitIndex - 1];
            Console.WriteLine($"\nA választott keret: {chosenKit}");

            // 🔹 5. Fehérvár játékoskeretének frissítése
            fehervar.Players = FileHandler.LoadPlayersFromFile($"{keretMappa}/{chosenKit}.txt");

            // 🔹 6. Szezon szimuláció a kiválasztott kerettel
            var table = InitializeTable(teams, fehervar);
            var matchups = GenerateSeasonMatchups(teams, fehervar);
            var matchResults = new Dictionary<(string, string), (int, int)>();

            for (int round = 1; round <= 33; round++)
            {
                Console.WriteLine($"\n{round}. forduló eredményei szimulálása...");

                SimulateRound(matchups[round - 1], table, (4, 4, 2), matchResults);
                SaveRoundResults(matchups[round - 1], table, round, "szimulalteredmenyek", chosenKit, matchResults);
            }

            SaveFinalTableToFile(table, "szimulalteredmenyek", chosenKit);
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
