# Expression
**Afk.Expression** is an expression parser/evaluation.

# Version
The library is compatible with netstandard, net40, net45 and net46

# NUGET
The easiest way to install is by using [NuGet](https://www.nuget.org/packages/Afk.Expression/).

# HowTo
This library allows you to enter complex expressions which will be evaluated and calculated on demand.
```
ExpressionEval eval = new ExpressionEval("5 * (3 + 8)");
Console.WriteLine(eval.Evaluate());
>>> 55
```

String and boolean expression can be evaluated too
```
ExpressionEval eval = new ExpressionEval("'one ' + 'and' + ' two'");
Console.WriteLine(eval.Evaluate());
>>> one and two
```

```
ExpressionEval eval = new ExpressionEval("true or false");
Console.WriteLine(eval.Evaluate());
>>> true
```

## Variables
Expression allows you to use variables evaluated on execution
```
ExpressionEval eval = new ExpressionEval("5 * x + x");
eval.AddVariable("x"); eval.AddVariable("y");
eval.UserExpressionEventHandler += (s, e) => {
    if (e.Name == "x")
        e.Result = 8d;
    else if (e.Name == "y")
        e.Result = 5d;
}; 
Console.WriteLine(eval.Evaluate());
>>> 45
```

## Constants

## Functions

# External Links

