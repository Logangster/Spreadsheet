//Author: Logan Gore

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Diagnostics;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNonEmptyCells()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "Test");
            ss.SetContentsOfCell("C3", "7.6");

            HashSet<string> expected = new HashSet<string> { "A1", "C3" };
            HashSet<string> results = new HashSet<string>(ss.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(expected.SetEquals(results));
        }

        //Test that the method returns an empty set if there are no cells
        [TestMethod]
        public void TestNonEmptyCells2()
        {
            Spreadsheet ss = new Spreadsheet();

            HashSet<string> expected = new HashSet<string>{"A1"};
            Assert.IsFalse(expected.SetEquals(ss.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        public void TestGetEmptyCell()
        {
            Spreadsheet ss = new Spreadsheet();

            Assert.AreEqual("", ss.GetCellContents("A1"));
        }

        //Test that the method sets double AND returns proper dependencies
        [TestMethod]
        public void TestSetCellDoubleWithFormula()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("B1", "=A1 * 2");
            ss.SetContentsOfCell("C1", "=B1 + A1");

            HashSet<string> expected = new HashSet<string> { "A1", "B1", "C1" };

            ISet<string> results = ss.SetContentsOfCell("A1", "7.8");

            Assert.IsTrue(expected.SetEquals(results));
            Assert.AreEqual(ss.GetCellContents("A1"), "7.8");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellDoubleInvalidCell()
        {
            Spreadsheet ss = new Spreadsheet();

            ISet<string> results = ss.SetContentsOfCell("7A", "7.8");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellDoubleInvalidCell2()
        {
            Spreadsheet ss = new Spreadsheet();

            ISet<string> results = ss.SetContentsOfCell("A^", "7.8");
        }

        // Test method sets cell with texxt AND returns proper dependencies
        [TestMethod]
        public void TestSetCellTextWithFormula()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B1", "=A1 + 2");
            ss.SetContentsOfCell("E2", "=3 + A1");

            HashSet<string> expected = new HashSet<string> { "A1", "B1", "E2" };

            ISet<string> results = ss.SetContentsOfCell("A1", "Test works?");

            Assert.IsTrue(expected.SetEquals(results));
            Assert.AreEqual(ss.GetCellContents("A1"), "Test works?");
        }

        [TestMethod]
        public void TestSetCellFormula()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("B1", "2");
            ISet<string> results = ss.SetContentsOfCell("A1", "=(6+3)*2 + (5+2) - 2/3");

            Assert.AreEqual(ss.GetCellContents("A1"), new Formula("(6+3)*2+(5+2)-2/3"));
        }

        [TestMethod]
        public void TestSetCellFormula2()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("B1", "=A1 + 2");
            ss.SetContentsOfCell("C1", "=3 + A1");
            ss.SetContentsOfCell("E2", "8");

            

            HashSet<string> expected = new HashSet<string> { "A1", "B1", "C1"};

            ISet<string> results = ss.SetContentsOfCell("A1", "=(6+E2)*2 + (5+2) - 2/3");

            Assert.IsTrue(expected.SetEquals(results));
            Assert.AreEqual(ss.GetCellContents("A1"), new Formula("(6+E2)*2+(5+2)-2/3"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellFormulaCircularException()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("B1", "=A1 + 2");
            ss.SetContentsOfCell("A1", "=3 + B1");

            HashSet<string> expected = new HashSet<string> { "A1", "B1", "C1" };

            ISet<string> results = ss.SetContentsOfCell("A1", "=(6+3)*2 + (5+2) - 2/3");

            Assert.IsTrue(expected.SetEquals(results));
            Assert.AreEqual(ss.GetCellContents("A1"), "=(6+3)*2+(5+2) - 2/3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetContentsInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("ac_22");
        }

        // New methods implemented from abstract spread sheet

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ConstructorWithValidator()
        {
            // Only accepts uppercase variable names
            Spreadsheet ss = new Spreadsheet(s => s == s.ToUpper(), s => s, "default");

            ss.SetContentsOfCell("b1", "test");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("wawt2g+", "3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsInvalidFormulaVariables()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("wawt2g+", "=3 + 324A21B");
        }

        //Tests that setContents with a formula, also evaluates it into a value
        [TestMethod]
        public void GetValuewithFormula()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("B1", "1");
            ss.SetContentsOfCell("A1", "=2 + B1 * 2");

            Assert.AreEqual((double) ss.GetCellValue("A1"), 4);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellValue("af@g1");
        }

        [TestMethod]
        public void GetValuewithDouble()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("A1", "22");

            Assert.AreEqual((double)ss.GetCellValue("A1"), 22.0);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void OpenInvalidFile()
        {
            Spreadsheet s2 = new Spreadsheet(@"C:\spreadsheet.xml",
                s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveInvalidFile()
        {
            Spreadsheet s2 = new Spreadsheet(@"C:\spreadsheet.cs",
                s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedInvalidFileVersion()
        {
            Spreadsheet s2 = new Spreadsheet();
            s2.GetSavedVersion(@"C:/spreadsheet");
        }

        [TestMethod]
        public void Normalize()
        {
            Spreadsheet s1 = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            s1.SetContentsOfCell("a1", "testing");

            Assert.AreEqual(s1.GetCellContents("A1"), "testing");
            Assert.AreEqual(s1.GetCellContents("a1"), "testing");
        }

        // Methods taken from PS4 grading test that I felt were helpful for testing, I take no credit for these

        [TestMethod()]
        public void Test43()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            HashSet<string> firstCells = new HashSet<string>();
            HashSet<string> lastCells = new HashSet<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.Add("A1" + i);
                lastCells.Add("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SetEquals(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SetEquals(lastCells));
        }

        [TestMethod()]
        public void Test47()
        {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test48()
        {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test49()
        {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test50()
        {
            RunRandomizedTest(50, 2521);
        }

        public void RunRandomizedTest (int seed, int size) 
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26),1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }
    }
}
