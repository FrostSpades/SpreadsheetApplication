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
            return 0;
        }
    }
}