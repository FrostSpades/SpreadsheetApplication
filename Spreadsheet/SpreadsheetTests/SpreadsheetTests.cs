// Class that tests Spreadsheet class.
// @author Ethan Andrews
// @version September 29, 2023

using SpreadsheetUtilities;
using SS;
using System.Text.Json;

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













        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod, Timeout(2000)]
        [TestCategory("1")]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("3")]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod, Timeout(2000)]
        [TestCategory("5")]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("6")]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("7")]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("8")]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod, Timeout(2000)]
        [TestCategory("9")]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("10")]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("11")]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("12")]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("13")]
        public void ChangedAfterModify()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("13b")]
        public void UnChangedAfterSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "C1", "17.5");
            ss.Save("changed.txt");
            Assert.IsFalse(ss.Changed);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("14")]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("15")]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod, Timeout(2000)]
        [TestCategory("16")]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("17")]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("18")]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("19")]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("20")]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod, Timeout(2000)]
        [TestCategory("21")]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod, Timeout(2000)]
        [TestCategory("22")]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("23")]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("24")]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod, Timeout(2000)]
        [TestCategory("25")]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("26")]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("27")]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("28")]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod, Timeout(2000)]
        [TestCategory("29")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save(Path.GetFullPath("/missing/save.txt"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("30")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("31")]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("32")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("33")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("35")]
        public void SaveTest7()
        {
            var sheet = new
            {
                Cells = new
                {
                    A1 = new { StringForm = "hello" },
                    A2 = new { StringForm = "5.0" },
                    A3 = new { StringForm = "4.0" },
                    A4 = new { StringForm = "= A2 + A3" }
                },
                Version = ""
            };

            File.WriteAllText("save5.txt", JsonSerializer.Serialize(sheet));


            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("36")]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");

            string fileContents = File.ReadAllText("save6.txt");
            JsonDocument o = JsonDocument.Parse(fileContents);
            Assert.AreEqual("default", o.RootElement.GetProperty("Version").ToString());
            Assert.AreEqual("hello", o.RootElement.GetProperty("Cells").GetProperty("A1").GetProperty("StringForm").ToString());
            Assert.AreEqual(5.0, double.Parse(o.RootElement.GetProperty("Cells").GetProperty("A2").GetProperty("StringForm").ToString()), 1e-9);
            Assert.AreEqual(4.0, double.Parse(o.RootElement.GetProperty("Cells").GetProperty("A3").GetProperty("StringForm").ToString()), 1e-9);
            Assert.AreEqual("=A2+A3", o.RootElement.GetProperty("Cells").GetProperty("A4").GetProperty("StringForm").ToString().Replace(" ", ""));
        }


        // Fun with formulas
        [TestMethod, Timeout(2000)]
        [TestCategory("37")]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("38")]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("39")]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("40")]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("41")]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("42")]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("43")]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("44")]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("45")]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula. Solutions that re-evaluate 
        // cells on every request, rather than after a cell changes,
        // will timeout on this test.
        // This test is repeated to increase its scoring weight
        [TestMethod, Timeout(6000)]
        [TestCategory("46")]
        public void LongFormulaTest()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("47")]
        public void LongFormulaTest2()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("48")]
        public void LongFormulaTest3()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("49")]
        public void LongFormulaTest4()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        [TestMethod, Timeout(6000)]
        [TestCategory("50")]
        public void LongFormulaTest5()
        {
            object result = "";
            LongFormulaHelper(out result);
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }
    }
}