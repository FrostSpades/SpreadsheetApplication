
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
///
/// This is an evaluation tool for evaluating infix expressions.
/// 
/// @author Ethan Andrews
/// @version September 2, 2023
///

namespace FormulaEvaluator
{
    /// <summary>
    /// This static class includes methods for evaluating infix expressions.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate for returning variable values given a variable name "v".
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// Evaluates the infix expression "exp" using the Lookup function "variableEvaluator".
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            // Converts exp into list of tokens
            string[] substrings = Regex.Split(exp, "(\\s)|(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            // Value used to determine if the previous token processed was an operator
            // Used so that there aren't situations like "+-" or "-*".
            bool operatorLastUsed = true;
            String[] importantOperators = new String[] { "+", "-", "*", "/" };
            String[] otherOperators = new String[] { "(", ")" };

            foreach (String sub in substrings)
            {
                // Allows s to be reassigned if needed
                String s = sub;

                // ignores empty or whitespace strings
                if (s == "" || s == " ")
                {
                    continue;
                }

                // If s is a valid variable name, assign its value to s
                if (isValidString(s))
                {
                    s = variableEvaluator(s).ToString();
                }

                // If s is a non-paranthesis operator
                if (importantOperators.Contains(s))
                {
                    // If the last token processed was also an operator, it is an invalid expression
                    // e.g. "+-", "++", "/*" etc.
                    if (operatorLastUsed)
                    {
                        throw new ArgumentException("Invalid Expression");
                    }
                    else
                    {
                        operatorLastUsed = true;
                    }



                }


                else if (otherOperators.Contains(s))
                {
                    operatorLastUsed = false;
                }


                else if (int.TryParse(s, out int result))
                {
                    operatorLastUsed = false;
                }

                else
                {
                    throw new ArgumentException("Invalid Expression");
                }
            }

            return 0;
        }

        /// <summary>
        /// Helper method for determining whether a string s is valid.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool isValidString(String s)
        {
            // Checks to see if string starts with a letter
            if (!char.IsLetter(s[0]))
            {
                return false;
            }

            // Value that indicates whether we are looking at first half or second half of string.
            // First half is all letters, second half is all numbers.
            bool letterSearch = true;

            foreach (char c in s)
            {
                // Searches for letters
                if (letterSearch)
                {
                    if (char.IsLetter(c))
                    {
                        continue;
                    }

                    else if (char.IsDigit(c))
                    {
                        letterSearch = false;
                    }

                    else
                    {
                        // If neither a letter nor a number, return false
                        return false;
                    }
                }

                // Searches for numbers
                else
                {
                    if (char.IsDigit(c))
                    {
                        continue;
                    }

                    else
                    {
                        return false;
                    }
                }
            }

            // If a digit was never found, return false.
            return !letterSearch;
        }
    }
}