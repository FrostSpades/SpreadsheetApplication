// Class that creates immutable formula objects with
// variables.
//
// @author Ethan Andrews
// @version September 15, 2023

using System.Text.RegularExpressions;

namespace SpreadsheetUtilities;

/// <summary>
/// Represents formulas written in standard infix notation using standard precedence
/// rules.  The allowed symbols are non-negative numbers written using double-precision
/// floating-point syntax (without unary preceeding '-' or '+');
/// variables that consist of a letter or underscore followed by
/// zero or more letters, underscores, or digits; parentheses; and the four operator
/// symbols +, -, *, and /.
///
/// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
/// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
/// and "x 23" consists of a variable "x" and a number "23".
///
/// Associated with every formula are two delegates: a normalizer and a validator.  The
/// normalizer is used to convert variables into a canonical form. The validator is used to
/// add extra restrictions on the validity of a variable, beyond the base condition that
/// variables must always be legal: they must consist of a letter or underscore followed
/// by zero or more letters, underscores, or digits.
/// Their use is described in detail in the constructor and method comments.
/// </summary>
public class Formula
{
    private List<String> variables; // Field for storing the variables
    private List<String> tokensList; // Field for storing list of tokens

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.
    /// </summary>
    public Formula(string formula) :
        this(formula, s => s, s => true)
    {
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.
    ///
    /// If the formula contains a variable v such that normalize(v) is not a legal variable,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    ///
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
        // Generates raw tokens list
        IEnumerable<String> tokens = GetTokens(formula);

        // If string contains no valid tokens
        if (tokens.Count() == 0)
        {
            throw new FormulaFormatException("String does not contain any valid tokens");
        }



        variables = new List<String>(); // field for storing variables
        tokensList = new List<String>(); // field for storing valid tokens

        String lastToken = tokens.First(); // Keeps track of the last token processed
        String[] operations = new string[] { "+", "-", "*", "/" }; // list of valid operations

        bool firstToken = true; // True if it is the first token.
        int parenthesesCount = 0; // +1 for left parentheses and -1 for right parentheses
        

        foreach (String token in tokens)
        {
            // Behavior of the first token
            if (firstToken)
            {
                if (operations.Contains(tokens.First()) || lastToken.Equals(")"))
                {
                    throw new FormulaFormatException("Illegal start of formula");
                }

                if (double.TryParse(token, out double value))
                {
                    tokensList.Add(value.ToString());
                }

                else if (token.Equals("("))
                {
                    tokensList.Add("(");
                    parenthesesCount++;
                }

                else
                {
                    VariableHandler(normalize, isValid, token, "(");
                }

                firstToken = false;
                continue;
            }



            // Behavior of operations: +, -, *, /
            if (operations.Contains(token))
            {
                // Checks if last token was also an operation
                if (operations.Contains(lastToken))
                {
                    throw new FormulaFormatException("An operator cannot directly follow another operator");
                }

                // Checks if last token was an open parenthesis
                if (lastToken.Equals("("))
                {
                    throw new FormulaFormatException("An operator cannot directly follow an open parenthesis");
                }

                tokensList.Add(token);
            }



            // Behavior of open: (
            else if (token.Equals("("))
            {
                parenthesesCount++;

                // Checks if last token was an operation or an opening parenthesis
                if (!operations.Contains(lastToken) && !lastToken.Equals("("))
                {
                    throw new FormulaFormatException("Opening parentheses have to follow either an operation or another opening parenthesis");
                }

                tokensList.Add(token);
            }



            // Behavior of close: )
            else if (token.Equals(")"))
            {
                parenthesesCount--;

                // Checks if there are more closing parentheses than opening
                if (parenthesesCount < 0)
                {
                    throw new FormulaFormatException("Parentheses are improperly balanced");
                }

                // Checks if last token was an open parenthesis
                if (lastToken.Equals("("))
                {
                    throw new FormulaFormatException("A close parentheses cannot directly follow an open parenthesis");
                }

                // Checks if last token was an operation
                if (operations.Contains(lastToken))
                {
                    throw new FormulaFormatException("A close parentheses cannot directly follow an operation");
                }

                tokensList.Add(token);
            }


            
            // Behavior of numbers
            else if (double.TryParse(token, out double value))
            {
                
                if (!lastToken.Equals("(") && !operations.Contains(lastToken))
                {
                    throw new FormulaFormatException("Number can only follow opening parenthesis or an operation");
                }

                tokensList.Add(value.ToString());
            }



            // Behavior of variables
            else
            {
                VariableHandler(normalize, isValid, token, lastToken);
            }


            lastToken = token;
        }

        // Checks if parentheses match
        if (parenthesesCount != 0)
        {
            throw new FormulaFormatException("Number of opening parentheses does not match number of closing parentheses");
        }

        // Checks last token
        if (operations.Contains(lastToken) || lastToken.Equals("("))
        {
            throw new FormulaFormatException("Valid formula needs to end with a number, variable, or closing parenthesis");
        }
    }


    /// <summary>
    /// Checks if string is a legal variable.
    /// Throws FormulaFormatException if token is illegal.
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <exception cref="FormulaFormatException"></exception>
    private void checkValidVariable(string token)
    {
        char c = token[0];

        if (c != '_' && !Char.IsLetter(c))
        {
            throw new FormulaFormatException("Formula cannot contain illegal variables");
        }
    }


    /// <summary>
    /// Helper method for handling variables. Normalizes, checks, and adds variables to the lists.
    /// </summary>
    /// <param name="normalize"></param>
    /// <param name="isValid"></param>
    /// <param name="token"></param>
    /// <param name="lastToken"></param>
    /// <exception cref="FormulaFormatException"></exception>
    private void VariableHandler(Func<string, string> normalize, Func<string, bool> isValid, String token, String lastToken)
    {
        String[] operations = new string[] { "+", "-", "*", "/" };

        // Checks if token is legal
        checkValidVariable(token);


        // Checks if token is valid using provided delegate
        String newToken = normalize(token);

        if (isValid(newToken))
        {
            tokensList.Add(newToken);
            variables.Add(newToken);
        }

        else
        {
            throw new FormulaFormatException("Variables are invalid");
        }


        // Checks the last token rules
        if (!lastToken.Equals("(") && !operations.Contains(lastToken))
        {
            throw new FormulaFormatException("Variable can only follow opening parenthesis or an operation");
        }
    }



    /// <summary>
    /// Evaluates this Formula, using the lookup delegate to determine the values of
    /// variables.  When a variable symbol v needs to be determined, it should be looked up
    /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
    /// the constructor.)
    ///
    /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
    /// in a string to upper case:
    ///
    /// new Formula("x+7", N, s => true).Evaluate(L) is 11
    /// new Formula("x+7").Evaluate(L) is 9
    ///
    /// Given a variable symbol as its parameter, lookup returns the variable's value
    /// (if it has one) or throws an ArgumentException (otherwise).
    ///
    /// If no undefined variables or divisions by zero are encountered when evaluating
    /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
    /// The Reason property of the FormulaError should have a meaningful explanation.
    ///
    /// This method should never throw an exception.
    /// </summary>
    public object Evaluate(Func<string, double> lookup)
    {
        string[] substrings = tokensList.ToArray();

        Stack<double> values = new Stack<double>();
        Stack<String> operators = new Stack<String>();

        // Value used to determine if the previous token processed was an operator
        // Used so that there aren't situations like "+-" or "-*".
        String[] importantOperators = new String[] { "+", "-", "*", "/" };
        String[] otherOperators = new String[] { "(", ")" };

        foreach (String sub in substrings)
        {
            // Allows s to be reassigned if needed
            String s = sub;


            // Checks if s is a variable, and looks up value if it is
            if (variables.Contains(s))
            {
                try
                {
                    s = lookup(s).ToString();
                }
                catch (ArgumentException)
                {
                    return new FormulaError("Variable does not have value");
                }
            }

            // If s is a non-paranthesis operator
            if (importantOperators.Contains(s))
            {

                // Checks which operation s belongs to
                if (s.Equals("*") || s.Equals("/"))
                {
                    operators.Push(s);
                }

                else
                {

                    if (operators.Count > 0 && (operators.Peek().Equals("+") || operators.Peek().Equals("-")))
                    {

                        double right = values.Pop();
                        double left = values.Pop();
                        String oper = operators.Pop();

                        if (oper.Equals("+"))
                        {
                            values.Push(left + right);
                        }
                        else
                        {
                            values.Push(left - right);
                        }
                    }

                    operators.Push(s);
                }
            }



            // Checks if s is a parenthesis
            else if (otherOperators.Contains(s))
            {


                if (s.Equals("("))
                {
                    operators.Push("(");
                }

                else
                {
                    // Checks if + or - is at top of stack
                    if (operators.Count > 0 && (operators.Peek().Equals("+") || operators.Peek().Equals("-")))
                    {
                        double right = values.Pop();
                        double left = values.Pop();
                        String oper = operators.Pop();

                        if (oper.Equals("+"))
                        {
                            values.Push(left + right);
                        }
                        else
                        {
                            values.Push(left - right);
                        }

                    }

                    operators.Pop();


                    // Checks if * or / is at top of stack
                    if (operators.Count > 0 && (operators.Peek().Equals("*") || operators.Peek().Equals("/")))
                    {

                        double right = values.Pop();
                        double left = values.Pop();
                        String oper = operators.Pop();

                        if (oper.Equals("*"))
                        {
                            values.Push(left * right);
                        }
                        else
                        {
                            if (right == 0)
                            {
                                return new FormulaError("Divide by Zero");
                            }

                            values.Push(left / right);
                        }
                    }

                }
            }



            // Checks if s is an integer, and if it is, stores the result in "result"
            else if (int.TryParse(s, out int result))
            {

                // Checks if the operator stack has a multiply or divide at the top
                if (operators.Count > 0)
                {

                    if (operators.Peek().Equals("*"))
                    {
                        double left = values.Pop();
                        operators.Pop();

                        values.Push(left * result);
                    }



                    else if (operators.Peek().Equals("/"))
                    {

                        double numerator = values.Pop();
                        operators.Pop();

                        if (result == 0)
                        {
                            return new FormulaError("Divide by Zero");
                        }

                        values.Push(numerator / result);
                    }

                    else
                    {
                        values.Push(result);
                    }
                }

                else
                {
                    values.Push(result);
                }
            }
        }


        // Evaluates final expression if the last operator on stack is + or -
        if (operators.Count == 1)
        {
            double right = values.Pop();
            double left = values.Pop();
            String oper = operators.Pop();

            if (oper.Equals("+"))
            {
                return left + right;
            }

            else
            {
                return left - right;
            }
        }


        // Returns value if operator stack is empty
        else
        {
            return values.Pop();
        }
    }

    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in this
    /// formula.  No normalization may appear more than once in the enumeration, even
    /// if it appears more than once in this Formula.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<string> GetVariables()
    {
        return variables;
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
        String returnString = "";
        
        foreach (String s in tokensList)
        {
            returnString = returnString + s;
        }

        return returnString;
    }

    /// <summary>
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    ///
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" by
    /// using C#'s standard conversion from string to double (and optionally back to a string).
    /// Variable tokens are considered equal if their normalized forms are equal, as
    /// defined by the provided normalizer.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Formula)
        {
            return false;
        }

        return ToString().Equals(obj.ToString());
    }

    /// <summary>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    /// Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !f1.Equals(f2);
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a legal variable token;
    /// a double literal; and anything that doesn't match one of those patterns.
    /// There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(string formula)
    {
        // Patterns for individual tokens
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                yield return s;
            }
        }

    }
}

/// <summary>
/// Used to report syntactic errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(string message) : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public struct FormulaError
{
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(string reason) : this()
    {
        Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}