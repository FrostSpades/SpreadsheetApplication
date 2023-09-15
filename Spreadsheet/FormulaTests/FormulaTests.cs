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
}