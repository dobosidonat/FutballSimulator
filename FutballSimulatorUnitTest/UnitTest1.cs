using Microsoft.VisualStudio.TestTools.UnitTesting;
using FutballSimulator;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace FutballSimulatorUnitTest
{

    [TestClass] // 📌 Ez egy teszt osztály
    public class SeasonSimulatorTests
    {
        private readonly string teamFilePath = "D:\\egyetem\\MSc\\1. félév\\Programozási paradigmák és adatszerkezetek\\Beadandó\\FutballSimulatorTeszt\\bin\\Debug\\ellenfelek/csapat_ertekelesek.txt";
        private readonly string budget = "D:\\egyetem\\MSc\\1. félév\\Programozási paradigmák és adatszerkezetek\\Beadandó\\FutballSimulatorTeszt\\bin\\Debug\\budgets/";


        /// <summary>
        /// Van-e 11 csapat az ellenfelek/csapat_ertekelesek fájlban?
        /// </summary>
        [TestMethod]
        public void TestTeamCountInOpponentFile()
        {
            Assert.IsTrue(File.Exists(teamFilePath), "A csapatértékelések fájl nem található!");
            var lines = File.ReadAllLines(teamFilePath);

            // Feltételezzük, hogy minden sor egy csapatot jelöl
            Assert.AreEqual(11, lines.Length, $"A csapatok száma nem megfelelő! Elvárt: 11, Aktuális: {lines.Length}");
        }

        /// <summary>
        /// Olyan csapatot keresünk, ami biztosan nincs benne a fájlban.
        /// </summary>
        [TestMethod]
        public void TestLoadTeams_MissingTeam()
        {
            var teams = FileHandler.LoadTeams(teamFilePath);
            var fakeTeam = teams.FirstOrDefault(t => t.Name == "FC Barcelona");
            Assert.IsNull(fakeTeam, "FC Barcelona nem kellene hogy szerepeljen a fájlban!");
        }

        /// <summary>
        /// Próbáljon meg egy üres vagy rossz formátumú fájlt beolvasni.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestLoadTeams_InvalidFormat()
        {
            var teams = FileHandler.LoadTeams("D:\\egyetem\\MSc\\1. félév\\Programozási paradigmák és adatszerkezetek\\Beadandó\\FutballSimulatorTeszt\\bin\\Debug\\keretek/hibas_keret.txt"); // Nincs ilyen mappa
        }
    }
}
