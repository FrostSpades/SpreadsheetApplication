using FormulaEvaluator;

int SampleEvaluator(String s)
{
    switch (s)
    {
        case "A1":
            return 1;

        case "B2":
            return 2;
    }

    return 0;
}


void testRandomExpressions()
{
    Console.WriteLine(Evaluator.Evaluate("1+4", SampleEvaluator)); // 5
    Console.WriteLine(Evaluator.Evaluate("1+4*5-7", SampleEvaluator)); // 14
    Console.WriteLine(Evaluator.Evaluate("(2+3)/2 + 2", SampleEvaluator)); // 4
    Console.WriteLine(Evaluator.Evaluate("2+5/2-7", SampleEvaluator)); // -3
    Console.WriteLine(Evaluator.Evaluate("4-54*3+7", SampleEvaluator)); // -151
}

void testExceptions()
{
    //Console.WriteLine(Evaluator.Evaluate("2 + 7 * 4 / (2 + 5 - 7)", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("2+(3-7", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("2+(4-7))", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("54*23-(+7)", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("5*4+(+7)", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("-4*5+1", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("A4A", SampleEvaluator));
    //Console.WriteLine(Evaluator.Evaluate("", SampleEvaluator));
}

void testVariables()
{
    Console.WriteLine(Evaluator.Evaluate("A1+4", SampleEvaluator)); // 5
    Console.WriteLine(Evaluator.Evaluate("A1+4*5-7", SampleEvaluator)); // 14
    Console.WriteLine(Evaluator.Evaluate("(B2+3)/B2 + B2", SampleEvaluator)); // 4
    Console.WriteLine(Evaluator.Evaluate("B2+5/B2-7", SampleEvaluator)); // -3
    Console.WriteLine(Evaluator.Evaluate("4-54*3+7+A1", SampleEvaluator)); // -150
}

//testRandomExpressions();
//testExceptions();
//testVariables();