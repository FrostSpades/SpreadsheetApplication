// Class that tests Spreadsheet class.
// @author Ethan Andrews
// @version September 22, 2023

using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {

        /// <summary>
        /// Tests the basic functionality for the SetCellContents for doubles.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X2", "2");
            ss.SetContentsOfCell("X3", "3");
            ss.SetContentsOfCell("X4", "4");

            Assert.AreEqual((double)1, ss.GetCellContents("X1"));
            Assert.AreEqual((double)2, ss.GetCellContents("X2"));
            Assert.AreEqual((double)3, ss.GetCellContents("X3"));
            Assert.AreEqual((double)4, ss.GetCellContents("X4"));
        }

        /// <summary>
        /// Tests the basic functionality for the SetCellContents for strings.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsString()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X2", "2");
            ss.SetContentsOfCell("X3", "3");
            ss.SetContentsOfCell("X4", "4");

            Assert.AreEqual("1", ss.GetCellContents("X1"));
            Assert.AreEqual("2", ss.GetCellContents("X2"));
            Assert.AreEqual("3", ss.GetCellContents("X3"));
            Assert.AreEqual("4", ss.GetCellContents("X4"));
        }

        /// <summary>
        /// Tests the basic functionality for the SetCellContents for formulas.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X2", "2");
            ss.SetContentsOfCell("X3", "3");
            ss.SetContentsOfCell("X4", "4");

            Assert.AreEqual(new Formula("1"), ss.GetCellContents("X1"));
            Assert.AreEqual(new Formula("2"), ss.GetCellContents("X2"));
            Assert.AreEqual(new Formula("3"), ss.GetCellContents("X3"));
            Assert.AreEqual(new Formula("4"), ss.GetCellContents("X4"));
        }

        /// <summary>
        /// Tests if the correct exception is thrown when given an invalid variable name.
        /// </summary>
        [TestMethod]
        public void TestInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();

            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("X$", "1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("1X", "1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("X1(", "1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("111", "1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.GetCellContents("X$"));
        }

        /// <summary>
        /// Tests the GetCellContents for empty cells.
        /// </summary>
        [TestMethod]
        public void TestGetEmptyCell ()
        {
            Spreadsheet ss = new Spreadsheet ();

            Assert.AreEqual("", ss.GetCellContents("X1"));
            Assert.AreEqual("", ss.GetCellContents("X2"));
            Assert.AreEqual("", ss.GetCellContents("X3"));
            Assert.AreEqual("", ss.GetCellContents("X4"));
        }

        /// <summary>
        /// Tests the replace functionality for the SetCellContents method.
        /// </summary>
        [TestMethod]
        public void TestReplaceCellContents()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X1", "2");
            Assert.AreEqual((double)2, ss.GetCellContents("X1"));

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X1", "2");
            Assert.AreEqual("2", ss.GetCellContents("X1"));

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X1", "2");
            Assert.AreEqual(new Formula("2"), ss.GetCellContents("X1"));
        }

        /// <summary>
        /// Tests the basic functionality of the GetNamesOfAllNonEmptyCells method.
        /// </summary>
        [TestMethod]
        public void TestGetNonEmptyCells()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X2", "1");
            ss.SetContentsOfCell("X3", "1");
            ss.SetContentsOfCell("X4", "1");
            ss.SetContentsOfCell("X1", "1");

            IEnumerable<string> list = ss.GetNamesOfAllNonemptyCells();

            Assert.AreEqual(4, list.Count());
            Assert.IsTrue(list.Contains("X1"));
            Assert.IsTrue(list.Contains("X2"));
            Assert.IsTrue(list.Contains("X3"));
            Assert.IsTrue(list.Contains("X4"));
        }

        /// <summary>
        /// Tests if an exception is thrown if there is a circle in formulas.
        /// </summary>
        [TestMethod]
        public void TestCircularException()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X6", "X5");
            ss.SetContentsOfCell("X5", "X4");
            ss.SetContentsOfCell("X4", "X3");
            ss.SetContentsOfCell("X3", "X2");
            ss.SetContentsOfCell("X1", "X2");

            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "X1"));
            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "X6"));
            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "X5"));
        }

        /// <summary>
        /// Tests if the return value for the SetCellValue is correct.
        /// </summary>
        [TestMethod]
        public void TestReturnValueOfSetCell()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("_X7", "1");
            ss.SetContentsOfCell("X8", "_X7");
            ss.SetContentsOfCell("X9", "X8");

            ss.SetContentsOfCell("X6", "X5");
            ss.SetContentsOfCell("X5", "X4");
            ss.SetContentsOfCell("X4", "X3");
            ss.SetContentsOfCell("X3", "X2");
            ss.SetContentsOfCell("X2", "X1");

            List<string> expectedList = new List<string> { "X1", "X2", "X3", "X4", "X5", "X6"};
            
            // Tests the double SetCellContents
            IList<string> list = ss.SetContentsOfCell("X1", "1");

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], list[i]);
            }


            // Tests the string SetCellContents
            list = ss.SetContentsOfCell("X1", "1");

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], list[i]);
            }


            // Tests the formula SetCellContents
            list = ss.SetContentsOfCell("X1", "1");

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], list[i]);
            }
        }


        /// <summary>
        /// Tests if graph stays unchanged after circular exception is thrown.
        /// </summary>
        [TestMethod]
        public void TestUnchangedValue()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "X2");
            ss.SetContentsOfCell("X2", "1");

            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "X1"));
            Assert.AreEqual(new Formula("1"), ss.GetCellContents("X2"));
        }


        [TestMethod]
        public void Save()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("X1", "=X2");
            ss.SetContentsOfCell("X2", "1");
            ss.SetContentsOfCell("X3", "=X2");
            ss.SetContentsOfCell("X4", "apple");

            ss.Save("SpreadsheetTestFileUsedForTests.txt");

            Spreadsheet newSS = new Spreadsheet("SpreadsheetTestFileUsedForTests.txt", s => true, s => s, "default");

            Assert.Equals(new Formula("X2"), newSS.GetCellContents("X1"));
        }
    }
}