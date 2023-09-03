
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
                if (s.Equals("") || s.Equals(" "))
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


                    // Checks which operation s belongs to
                    if (s.Equals("*") || s.Equals("/"))
                    {
                        operators.Push(s);
                    }

                    else
                    {

                        if (operators.Count > 0 && (operators.Peek().Equals("+") || operators.Peek().Equals("-")))
                        {
                            // If the values stack does not have at least two values,
                            // throw an exception.
                            if (values.Count < 2)
                            {
                                throw new ArgumentException("Invalid Expression");
                            }

                            int right = values.Pop();
                            int left = values.Pop();
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
                    operatorLastUsed = false;


                    if (s.Equals("("))
                    {
                        operators.Push("(");
                    }

                    else
                    {
                        // Checks if + or - is at top of stack
                        if (operators.Count > 0 && (operators.Peek().Equals("+") || operators.Peek().Equals("-")))
                        {
                            if (values.Count < 2)
                            {
                                throw new ArgumentException("Invalid Expression");
                            }

                            int right = values.Pop();
                            int left = values.Pop();
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

                        // If the next item on operators stack isnt "(" throw an exception
                        if (operators.Count == 0 || !operators.Pop().Equals("("))
                        {
                            throw new ArgumentException("Invalid Expression");
                        }


                        // Checks if * or / is at top of stack
                        if (operators.Count > 0 && (operators.Peek().Equals("*") || operators.Peek().Equals("/")))
                        {
                            if (values.Count < 2)
                            {
                                throw new ArgumentException("Invalid Expression");
                            }

                            int right = values.Pop();
                            int left = values.Pop();
                            String oper = operators.Pop();

                            if (oper.Equals("*"))
                            {
                                values.Push(left * right);
                            }
                            else
                            {
                                if (right == 0)
                                {
                                    throw new ArgumentException("Divide by Zero");
                                }

                                values.Push(left / right);
                            }
                        }

                    }
                }



                // Checks if s is an integer, and if it is, stores the result in "result"
                else if (int.TryParse(s, out int result))
                {
                    operatorLastUsed = false;

                    // Checks if the operator stack has a multiply or divide at the top
                    if (operators.Count > 0)
                    {
                       
                        if (operators.Peek().Equals("*"))
                        {
                            if (values.Count == 0)
                            {
                                throw new ArgumentException("Invalid Expression");
                            }

                            int left = values.Pop();
                            operators.Pop();

                            values.Push(left * result);
                        }



                        else if (operators.Peek().Equals("/"))
                        {
                            if (values.Count == 0)
                            {
                                throw new ArgumentException("Invalid Expression");
                            }

                            int numerator = values.Pop();
                            operators.Pop();

                            if (result == 0)
                            {
                                throw new ArgumentException("Divide by Zero");
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

                else
                {
                    throw new ArgumentException("Invalid Expression");
                }
            }


            // Evaluates final expression if the last operator on stack is + or -
            if (operators.Count == 1)
            {
                if (values.Count != 2)
                {
                    throw new ArgumentException("Invalid Expression");
                }

                int right = values.Pop();
                int left = values.Pop();
                String oper = operators.Pop();

                if (oper.Equals("+"))
                {
                    return left + right;
                }

                else if (oper.Equals("-"))
                {
                    return left - right;
                }

                else
                {
                    throw new ArgumentException("Invalid Expression");
                }
            }


            // Returns value if operator stack is empty
            else if (operators.Count == 0)
            {
                if (values.Count != 1)
                {
                    throw new ArgumentException("Invalid Expression");
                }

                else
                {
                    return values.Pop();
                }
            }

            // If operator stack has >1 values left, throw an exception
            else
            {
                throw new ArgumentException("Invalid Expression");
            }
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