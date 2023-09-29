// Class that tests Spreadsheet class.
// @author Ethan Andrews
// @version September 29, 2023

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

            ss.SetContentsOfCell("X1", "a");
            ss.SetContentsOfCell("X2", "b");
            ss.SetContentsOfCell("X3", "c");
            ss.SetContentsOfCell("X4", "d");

            Assert.AreEqual("a", ss.GetCellContents("X1"));
            Assert.AreEqual("b", ss.GetCellContents("X2"));
            Assert.AreEqual("c", ss.GetCellContents("X3"));
            Assert.AreEqual("d", ss.GetCellContents("X4"));
        }

        /// <summary>
        /// Tests the basic functionality for the SetCellContents for formulas.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("X1", "=1");
            ss.SetContentsOfCell("X2", "=2");
            ss.SetContentsOfCell("X3", "=3");
            ss.SetContentsOfCell("X4", "=4");

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

            ss.SetContentsOfCell("X1", "");
            ss.SetContentsOfCell("X1", "2");
            Assert.AreEqual((double)2, ss.GetCellContents("X1"));

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X1", "a");
            Assert.AreEqual("a", ss.GetCellContents("X1"));

            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X1", "=2");
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

            ss.SetContentsOfCell("X6", "=X5");
            ss.SetContentsOfCell("X5", "=X4");
            ss.SetContentsOfCell("X4", "=X3");
            ss.SetContentsOfCell("X3", "=X2");
            ss.SetContentsOfCell("X1", "=X2");

            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "=X1"));
            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "=X6"));
            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "=X5"));
        }

        /// <summary>
        /// Tests if the return value for the SetCellValue is correct.
        /// </summary>
        [TestMethod]
        public void TestReturnValueOfSetCell()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetContentsOfCell("_X7", "1");
            ss.SetContentsOfCell("X8", "=_X7");
            ss.SetContentsOfCell("X9", "=X8");

            ss.SetContentsOfCell("X6", "=X5");
            ss.SetContentsOfCell("X5", "=X4");
            ss.SetContentsOfCell("X4", "=X3");
            ss.SetContentsOfCell("X3", "=X2");
            ss.SetContentsOfCell("X2", "=X1");

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

            ss.SetContentsOfCell("X1", "=X2");
            ss.SetContentsOfCell("X2", "=1");

            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("X2", "=X1"));
            Assert.AreEqual(new Formula("1"), ss.GetCellContents("X2"));
        }

        /// <summary>
        /// Tests the basic save functionality.
        /// </summary>
        [TestMethod]
        public void TestSave()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("X1", "=X2");
            ss.SetContentsOfCell("X2", "1");
            ss.SetContentsOfCell("X3", "=X2");
            ss.SetContentsOfCell("X4", "apple");

            ss.Save("SpreadsheetTestFileUsedForTests.txt");

            Spreadsheet newSS = new Spreadsheet("SpreadsheetTestFileUsedForTests.txt", s => true, s => s, "default");

            Assert.AreEqual(new Formula("X2"), newSS.GetCellContents("X1"));
            Assert.AreEqual((double)1, newSS.GetCellContents("X2"));
            Assert.AreEqual(new Formula("X2"), newSS.GetCellContents("X3"));
            Assert.AreEqual("apple", newSS.GetCellContents("X4"));

            Assert.AreEqual((double)1, newSS.GetCellValue("X1"));
            Assert.AreEqual((double)1, newSS.GetCellValue("X2"));
            Assert.AreEqual((double)1, newSS.GetCellValue("X3"));
            Assert.AreEqual("apple", newSS.GetCellValue("X4"));
        }

        /// <summary>
        /// Tests if exception is thrown when given a fake path.
        /// </summary>
        [TestMethod]
        public void TestInvalidFilePath()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.ThrowsException<SpreadsheetReadWriteException>(() =>ss.Save("/some/nonsense/path.txt"));
        }

        /// <summary>
        /// Tests changed functionality.
        /// </summary>
        [TestMethod]
        public void TestChanged()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);

            ss.SetContentsOfCell("X1", "=X2");
            ss.SetContentsOfCell("X2", "1");
            ss.SetContentsOfCell("X3", "=X2");
            ss.SetContentsOfCell("X4", "apple");

            Assert.IsTrue(ss.Changed);

            ss.Save("SpreadsheetTestFileUsedForTests.txt");

            Assert.IsFalse(ss.Changed);
        }

        /// <summary>
        /// Tests basic functionality of get value method.
        /// </summary>
        [TestMethod]
        public void TestGetValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X2", "=X1*5");
            ss.SetContentsOfCell("X3", "=X2+X1");

            Assert.AreEqual((double)1, ss.GetCellValue("X1"));
            Assert.AreEqual((double)5, ss.GetCellValue("X2"));
            Assert.AreEqual((double)6, ss.GetCellValue("X3"));
        }

        /// <summary>
        /// Tests get value functionality when changes are made to the spreadsheet.
        /// </summary>
        [TestMethod]
        public void TestGetValueAfterChange()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("X1", "1");
            ss.SetContentsOfCell("X2", "=X1*5");
            ss.SetContentsOfCell("X3", "=X2+X1");

            Assert.AreEqual((double)1, ss.GetCellValue("X1"));
            Assert.AreEqual((double)5, ss.GetCellValue("X2"));
            Assert.AreEqual((double)6, ss.GetCellValue("X3"));

            ss.SetContentsOfCell("X1", "2");

            Assert.AreEqual((double)2, ss.GetCellValue("X1"));
            Assert.AreEqual((double)10, ss.GetCellValue("X2"));
            Assert.AreEqual((double)12, ss.GetCellValue("X3"));
        }

        /// <summary>
        /// Tests if formula error is returned with invalid formulas.
        /// </summary>
        [TestMethod]
        public void TestFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            
            ss.SetContentsOfCell("X3", "=X2+X1");
            Assert.IsTrue(ss.GetCellValue("X3") is FormulaError);

            ss.SetContentsOfCell("X2", "=X1*5");
            Assert.IsTrue(ss.GetCellValue("X2") is FormulaError);

            ss.SetContentsOfCell("X1", "1");

            Assert.AreEqual((double)1, ss.GetCellValue("X1"));
            Assert.AreEqual((double)5, ss.GetCellValue("X2"));
            Assert.AreEqual((double)6, ss.GetCellValue("X3"));

            ss.SetContentsOfCell("X1", "yes");
            Assert.IsTrue(ss.GetCellValue("X2") is FormulaError);
            Assert.IsTrue(ss.GetCellValue("X3") is FormulaError);
        }

        /// <summary>
        /// Tests basic functionality with normalizer.
        /// </summary>
        [TestMethod]
        public void TestNormalizer()
        {
            Func<string, bool> validator = s => true;
            Func<string, string> normalizer = s => s.ToUpper();

            Spreadsheet ss = new Spreadsheet(validator, normalizer, "default");

            ss.SetContentsOfCell("x1", "1");
            Assert.AreEqual((double)1, ss.GetCellContents("X1"));
            Assert.AreEqual((double)1, ss.GetCellContents("x1"));
        }

        /// <summary>
        /// Validator function that only accepts a few strings.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool ValidatorFunction(string s)
        {
            if (s.Equals("X1"))
            {
                return true;
            }

            else if (s.Equals("X2"))
            {
                return true;
            }

            else if (s.Equals("X3"))
            {
                return true;
            }

            else if (s.Equals("X4"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tests basic functionality of validator.
        /// </summary>
        [TestMethod]
        public void TestWithValidator()
        {
            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();

            Spreadsheet ss = new Spreadsheet(validator, normalizer, "default");

            ss.SetContentsOfCell("x1", "1");
            Assert.AreEqual((double)1, ss.GetCellContents("X1"));
            ss.SetContentsOfCell("x2", "2");
            Assert.AreEqual((double)2, ss.GetCellContents("X2"));
            ss.SetContentsOfCell("x3", "3");
            Assert.AreEqual((double)3, ss.GetCellContents("X3"));
            ss.SetContentsOfCell("x4", "4");
            Assert.AreEqual((double)4, ss.GetCellContents("X4"));

            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("A1", "=X1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.GetCellContents("X5"));
            Assert.ThrowsException<InvalidNameException>(() => ss.GetCellValue("_1"));
        }

        /// <summary>
        /// Tests if exception is thrown with invalid path name.
        /// </summary>
        [TestMethod]
        public void TestInvalidPathNameConstructor()
        {
            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();
            Spreadsheet ss;

            Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new Spreadsheet("/some/nonsense/path.txt", validator, normalizer, "default"));
        }

        /// <summary>
        /// Tests if exception is thrown when file is empty.
        /// </summary>
        [TestMethod]
        public void TestNullConstructor()
        {
            try
            {
                File.WriteAllText("TestNullConstructorTestingFile.txt", "");
            }
            catch
            {
                return;
            }

            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();
            Spreadsheet ss;

            Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new Spreadsheet("TestNullConstructorTestingFile.txt", validator, normalizer, "default"));
        }

        /// <summary>
        /// Tests if spreadsheet can save with nothing in it.
        /// </summary>
        [TestMethod]
        public void TestEmptySave()
        {
            Spreadsheet ss = new Spreadsheet();

            ss.Save("SpreadsheetTestFileUsedForTests.txt");

            Spreadsheet newSS = new Spreadsheet("SpreadsheetTestFileUsedForTests.txt", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests if exception is thrown when given invalid formulas.
        /// </summary>
        [TestMethod]
        public void TestFormulaFormatExceptionConstructor()
        {
            try
            {
                File.WriteAllText("TestTestingFile.txt", "{\"Cells\":{\"X4\":{\"StringForm\":\"=**\"},\"X3\":{\"StringForm\":\"=X2\"},\"X2\":{\"StringForm\":\"1\"},\"X1\":{\"StringForm\":\"=X2\"}},\"Version\":\"default\"}");
            }
            catch
            {
                return;
            }

            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();
            Spreadsheet ss;

            Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new Spreadsheet("TestTestingFile.txt", validator, normalizer, "default"));
        }

        /// <summary>
        /// Tests if exception is thrown when given circular formulas.
        /// </summary>
        [TestMethod]
        public void TestCircularExceptionConstructor()
        {
            try
            {
                File.WriteAllText("TestTestingFile.txt", "{\"Cells\":{\"X4\":{\"StringForm\":\"=X3\"},\"X3\":{\"StringForm\":\"=X4\"},\"X2\":{\"StringForm\":\"1\"},\"X1\":{\"StringForm\":\"=X2\"}},\"Version\":\"default\"}");
            }
            catch
            {
                return;
            }

            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();
            Spreadsheet ss;

            Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new Spreadsheet("TestTestingFile.txt", validator, normalizer, "default"));
        }

        /// <summary>
        /// Tests if exception is thrown when given invalid names.
        /// </summary>
        [TestMethod]
        public void TestInvalidNameConstructor()
        {
            try
            {
                File.WriteAllText("TestTestingFile.txt", "{\"Cells\":{\"4X4\":{\"StringForm\":\"=X3\"},\"X3\":{\"StringForm\":\"=X4\"},\"X2\":{\"StringForm\":\"1\"},\"X1\":{\"StringForm\":\"=X2\"}},\"Version\":\"default\"}");
            }
            catch
            {
                return;
            }

            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();
            Spreadsheet ss;

            Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new Spreadsheet("TestTestingFile.txt", validator, normalizer, "default"));
        }

        /// <summary>
        /// Tests if exception is thrown when given invalid version.
        /// </summary>
        [TestMethod]
        public void TestIncorrectVersionConstructor()
        {
            try
            {
                File.WriteAllText("TestTestingFile.txt", "{\"Cells\":{\"4X4\":{\"StringForm\":\"=X3\"},\"X3\":{\"StringForm\":\"=X4\"},\"X2\":{\"StringForm\":\"1\"},\"X1\":{\"StringForm\":\"=X2\"}},\"Version\":\"1\"}");
            }
            catch
            {
                return;
            }

            Func<string, bool> validator = ValidatorFunction;
            Func<string, string> normalizer = s => s.ToUpper();
            Spreadsheet ss;

            Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new Spreadsheet("TestTestingFile.txt", validator, normalizer, "default"));
        }

        /// <summary>
        /// Tests if exception is thrown when given invalid names to different methods.
        /// </summary>
        [TestMethod]
        public void TestInvalidNames()
        {
            Spreadsheet ss = new Spreadsheet();

            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("%X1", "1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("1X1", "apple"));
            Assert.ThrowsException<InvalidNameException>(() => ss.SetContentsOfCell("&&&&", "=1"));
            Assert.ThrowsException<InvalidNameException>(() => ss.GetCellValue("&&&&"));
        }
    }
}