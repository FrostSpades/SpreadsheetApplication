// Test class used to test the formulas class
//
// @author Ethan Andrews
// @version September 15, 2023

using Newtonsoft.Json.Linq;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace FormulaTests;

[TestClass]
public class FormulaTests
{
    /// <summary>
    /// Tests the parsing rule.
    /// </summary>
    [TestMethod]
    public void ParsingRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("4+$r"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("5r+1"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("_rrr1%"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("4.00.0 + 1"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("5..0"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("_ab%"));
    }


    /// <summary>
    /// Tests the one token rule.
    /// </summary>
    [TestMethod]
    public void OneTokenRuleTest()
    {
        Formula f = new Formula("1");

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula(""));
    }


    /// <summary>
    /// Tests the right parentheses rule.
    /// </summary>
    [TestMethod]
    public void RightParenthesesRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(6+4))+(4+(5)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("((2)))+(4"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("((4+7)))+((2)"));
    }


    /// <summary>
    /// Tests the balanced parentheses rule.
    /// </summary>
    [TestMethod]
    public void BalancedParenthesesRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(((1))))"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("((1+2)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(((1*2"));
    }


    /// <summary>
    /// Tests the starting token rule.
    /// </summary>
    [TestMethod]
    public void StartingTokenRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula(")*5"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("+1"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("-0+1"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("*7+2"));
    }

    /// <summary>
    /// Tests the ending token rule.
    /// </summary>
    [TestMethod]
    public void EndingTokenRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("2+3-"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("1-2+"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("1+2+("));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("1+2*"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("1+2/"));
    }

    /// <summary>
    /// Tests the parenthesis/operator following rule.
    /// </summary>
    [TestMethod]
    public void ParOperFollowingRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("4+-1"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(+1)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("-(1+2)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("4*-1"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(4+2-)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(4+2)_1"));
    }

    /// <summary>
    /// Tests the extra following rule.
    /// </summary>
    [TestMethod]
    public void ExtraFollowingRuleTest()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("4(5+2)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("(4)(4)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("A4(1)"));
        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("67 23"));
    }

    /// <summary>
    /// Tests special cases.
    /// </summary>
    [TestMethod]
    public void SpecialTests()
    {
        Formula f;

        Assert.ThrowsException<FormulaFormatException>(() => f = new Formula("()"));
    }


    /// <summary>
    /// Uppercase method used for the normalization function.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public String toUpper(String s)
    {
        String newString = "";

        foreach (char c in s)
        {
            newString += char.ToUpper(c);
        }

        return newString;
    }



    /// <summary>
    /// Tests the get variables method.
    /// </summary>
    [TestMethod]
    public void GetVariablesTest()
    {
        Formula f = new Formula("_1+at+v");
        List<String> list = (List<String>)f.GetVariables();
        List<String> expectedList = new List<string> { "_1", "at", "v"};
        
        for (int i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(list[i], expectedList[i]);
        }


        f = new Formula("_1+at+v", toUpper, s => true);
        list = (List<String>)f.GetVariables();
        expectedList = new List<string> { "_1", "AT", "V" };

        for (int i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(list[i], expectedList[i]);
        }
    }


    /// <summary>
    /// Tests the equals methods.
    /// </summary>
    [TestMethod]
    public void EqualsTest()
    {
        Formula f = new Formula("1   +     2");
        Formula g = new Formula("1+2");
        Assert.IsTrue(f.Equals (g));

        f = new Formula("A1 + 2 +_1");
        g = new Formula("a1+2+_1", toUpper, s => true);
        Assert.IsTrue(f.Equals (g));
        
        Assert.IsTrue(f == g);
        Assert.IsFalse(f != g);
        Assert.AreEqual(f.GetHashCode(), g.GetHashCode());

        Assert.IsFalse(f.Equals(1));
        Assert.IsFalse(f.Equals(null));
    }


    /// <summary>
    /// Tests the evaluation method for no variable formulas.
    /// </summary>
    [TestMethod]
    public void NoVariableEvaluationTest()
    {
        Formula f = new Formula("1 + 4 * 5 - 2");
        Assert.AreEqual((double)f.Evaluate(s => 1), 19, 1e-6);

        f = new Formula("(2+3) / 6");
        Assert.AreEqual((double)f.Evaluate(s => 1), .833333333333, 1e-6);

        f = new Formula("1-1-1-1-1-1-1");
        Assert.AreEqual((double)f.Evaluate(s => 1), -5, 1e-6);

        f = new Formula("1+1+1+1+1+1");
        Assert.AreEqual((double)f.Evaluate(s => 1), 6, 1e-6);

        f = new Formula("(1-2-3)-1+4-5");
        Assert.AreEqual((double)f.Evaluate(s => 1), -6, 1e-6);

        f = new Formula("(1*2*3)*1*(4*5)");
        Assert.AreEqual((double)f.Evaluate(s => 1), 120, 1e-6);

        f = new Formula("(2/3)/1/(4/4)");
        Assert.AreEqual((double)f.Evaluate(s => 1), .666666666666, 1e-6);

        f = new Formula("(2/3)/1/(4-4)");
        Assert.AreEqual(((FormulaError)f.Evaluate(s => 1)).Reason, "Divide by Zero");

        f = new Formula("1/0");
        Assert.AreEqual(((FormulaError)f.Evaluate(s => 1)).Reason, "Divide by Zero");
    }


    /// <summary>
    /// Tests the evaluation method with variables.
    /// </summary>
    [TestMethod]
    public void EvaluationWithVariablesTest()
    {
        Formula f = new Formula("1+s+s+2");
        Assert.AreEqual((double)(f.Evaluate(s => 1)), 5, 1e-6);

        f = new Formula("1+s+s+2");
        Assert.AreEqual(((FormulaError)f.Evaluate(s => throw new ArgumentException())).Reason, "Variable does not have value");
    }
}