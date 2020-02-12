# Expression
**Afk.Expression** is an expression parser/evaluation. Expression is parse only once and use to evaluate differents inputs (variables, constants, functions).
This library can be used to generate SQL Expression to be used with Entity Framework.

# Version
The library is compatible with netstandard2.0

# NUGET
The easiest way to install is by using [NuGet](https://www.nuget.org/packages/Afk.Expression/).

# HowTo
This library allows you to enter complex expressions which will be evaluated and calculated on demand.
```csharp
ExpressionEval eval = new ExpressionEval("5 * (3 + 8)");
Console.WriteLine(eval.Evaluate());
>>> 55
```

String and boolean expression can be evaluated too
```csharp
ExpressionEval eval = new ExpressionEval("'one ' + 'and' + ' two'");
Console.WriteLine(eval.Evaluate());
>>> one and two
```

```csharp
ExpressionEval eval = new ExpressionEval("true or false");
Console.WriteLine(eval.Evaluate());
>>> true
```

## Operators
By default the ^ is the logical exclusive OR operator.
You can change its behavior by using the operator type ***Arithmetic***

```csharp
ExpressionEval expr1 = new ExpressionEval("4^2", OperatorType.Arithmetic);
Console.WriteLine(eval.Evaluate());
>>> 16
```
 
## Variables
Expression allows you to use variables evaluated on execution
```csharp
ExpressionEval eval = new ExpressionEval("5 * x2 + y");
eval.AddVariable("x"); eval.AddVariable("y");
eval.UserExpressionEventHandler += (s, e) => {
    if (e.Name == "x")
        e.Result = 8d;
    else if (e.Name == "y")
        e.Result = 5d;
}; 
Console.WriteLine(eval.Evaluate());
>>> 325
```

## Constants
Constants can be added to the expression
```csharp
ExpressionEval eval = new ExpressionEval("pi");
eval.AddConstant("pi", 3.14);
Console.WriteLine(eval.Evaluate());
>>> 3.14
```

## Functions
External function can be defined
```csharp
    private void OnFunctionHandler(object sender, UserFunctionEventArgs e)
    {
        if (e.Name == "Concat")
        {
            e.Result = string.Join("", e.Parameters);
        }
    }

ExpressionEval eval = new ExpressionEval("Concat('The ', 'dogs ', 'barks') + '.'");
eval.AddFunctions("Concat");

eval.UserFunctionEventHandler += OnFunctionHandler;
Console.WriteLine(eval.Evaluate());
>>> The dogs barks
```

## EF Core
Expression can be used to generate *System.Linq.Expressions.Expression* to request *DbContext*.
You need to define a **ILambdaExpressionProvider** which provides expression on your entity.

```csharp
class MyLambdaProvider : ILambdaExpressionProvider
{
    public Expression GetExpression(ParameterExpression parameter, string propertyName)
    {
        if (propertyName.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
        {
            return System.Linq.Expressions.Expression.Property(parameter, nameof(Student.Name));
        }
        else if (propertyName.Equals("Age", StringComparison.InvariantCultureIgnoreCase))
        {
            ...
        }
        return null;
    }
```

Gets the expression with the extension method ToLambda on your expression :

```csharp
ExpressionEval eval = new ExpressionEval("name='john'", CaseSensitivity.None);
eval.UserExpressionEventHandler += evalProperties.Evaluate;

var lambda = eval.ToLambda<Student, bool>(new MyLambdaProvider());

var result = myContext.Students.Where(lambda).ToList();
```

### Overide operators
The *ILambdaExpressionProvider* enables you to override operators. 

By default the *like* operator use the *Contains* method translate to CHARINDEX sql function. To overide this operator use the following definition :

```csharp
class MyLambdaProvider : ILambdaExpressionProvider
{
    public Expression GetExpression(Expression left, string operand, Expression right)
    {
        if (operand == "like")
        {
            var method = typeof(DbFunctionsExtensions).GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
            MethodCallExpression like = System.Linq.Expressions.Expression.Call(method, Expression.Constant(EF.Functions), left, right);
            return like;
        }
        return null;
    }
}

ExpressionEval eval = new ExpressionEval("name like '8000%'", CaseSensitivity.None);
eval.UserExpressionEventHandler += evalProperties.Evaluate;

var lambda = eval.ToLambda<Student, bool>(new MyLambdaProvider());

Console.WriteLine(lambda.ToString());
>>> "s => value(Microsoft.EntityFrameworkCore.DbFunctions).Like(s.Name, \"8000%\")"
```

# External Links

