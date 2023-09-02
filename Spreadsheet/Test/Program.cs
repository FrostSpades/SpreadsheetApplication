using FormulaEvaluator;

int SampleEvaluator(String s)
{
    return 0;
}


void testRandomExpressions()
{
    Console.WriteLine(Evaluator.Evaluate("1+4", SampleEvaluator)); // 5
    Console.WriteLine(Evaluator.Evaluate("1+4*5-7", SampleEvaluator)); // 14
    Console.WriteLine(Evaluator.Evaluate("(2+3)/2 + 2", SampleEvaluator)); // 4
    Console.WriteLine(Evaluator.Evaluate("2+5/2-7", SampleEvaluator)); // -3
}

void testExceptions()
{
    //Console.WriteLine(Evaluator.Evaluate("2 + 7 * 4 / (2 + 5 - 7)", SampleEvaluator));

}


testRandomExpressions();
testExceptions();