using Antlr4.Runtime;
using BBplus.Content;

namespace BBplus;

public class BBplusVisitor : BBplusBaseVisitor<object?>
{
    private Dictionary<string, object?> Variables { get; } = new();
    private Dictionary<string, object?> PrivateVariables { get; } = new();
    
    private Dictionary<string, Func<object?[], object?>> PrivateFunctions { get; } = new();
    private Dictionary<string, Func<object?[], object?>> PublicFunctions { get; } = new();
    
    private Dictionary<string, object?> Mods { get; } = new();
    private Stack<string> ModStack { get; } = new Stack<string>();
    
    private object? ReturnValue { get; set; }

    public BBplusVisitor()
    {
        PrivateVariables["_PI_"] = Math.PI;
        PrivateVariables["_E_"] = Math.E;
        
        // Colors
        // i could've done this better but uh it works
        PrivateVariables["Black"] = Functions.Colors[0];
        PrivateVariables["DarkBlue"] = Functions.Colors[1];
        PrivateVariables["DarkGreen"] = Functions.Colors[2];
        PrivateVariables["DarkCyan"] = Functions.Colors[3];
        PrivateVariables["DarkRed"] = Functions.Colors[4];
        PrivateVariables["DarkMagenta"] = Functions.Colors[5];
        PrivateVariables["DarkYellow"] = Functions.Colors[6];
        PrivateVariables["Gray"] = Functions.Colors[7];
        PrivateVariables["DarkGray"] = Functions.Colors[8];
        PrivateVariables["Blue"] = Functions.Colors[9];
        PrivateVariables["Green"] = Functions.Colors[10];
        PrivateVariables["Cyan"] = Functions.Colors[11];
        PrivateVariables["Red"] = Functions.Colors[12];
        PrivateVariables["Magenta"] = Functions.Colors[13];
        PrivateVariables["White"] = Functions.Colors[14];
        PrivateVariables["Yellow"] = Functions.Colors[15];
        
        Variables["_PI_"] = PrivateVariables["_PI_"];
        Variables["_E_"] = PrivateVariables["_E_"];
        
        Variables["Black"] = PrivateVariables["Black"];
        Variables["DarkBlue"] = PrivateVariables["DarkBlue"];
        Variables["DarkGreen"] = PrivateVariables["DarkGreen"];
        Variables["DarkCyan"] = PrivateVariables["DarkCyan"];
        Variables["DarkRed"] = PrivateVariables["DarkRed"];
        Variables["DarkMagenta"] = PrivateVariables["DarkMagenta"];
        Variables["DarkYellow"] = PrivateVariables["DarkYellow"];
        Variables["Gray"] = PrivateVariables["Gray"];
        Variables["DarkGray"] = PrivateVariables["DarkGray"];
        Variables["Blue"] = PrivateVariables["Blue"];
        Variables["Green"] = PrivateVariables["Green"];
        Variables["Cyan"] = PrivateVariables["Cyan"];
        Variables["Red"] = PrivateVariables["Red"];
        Variables["Magenta"] = PrivateVariables["Magenta"];
        Variables["White"] = PrivateVariables["White"];
        Variables["Yellow"] = PrivateVariables["Yellow"];
        
        PrivateFunctions["message"] = Functions.Message;
        PrivateFunctions["wait"] = Functions.Wait;
        PrivateFunctions["throw"] = Throw;
        PrivateFunctions["consoleColor"] = Functions.ConsoleColor;
    }
    
    private object? Throw(object?[] args)
    {
        // basically a return function
        // Console.WriteLine(args.FirstOrDefault());
        ReturnValue = args.FirstOrDefault();
        return ReturnValue;
    }
    
    public override object? VisitAssignement(BBplusParser.AssignementContext context)
    {
        var t_varPrv = context.PRIVATE() is { };
        var t_varName = context.IDENTIFIER().GetText();
        var t_varValue = Visit(context.expression());

        if (t_varPrv)
        {
            if (!PrivateVariables.TryAdd(t_varName, t_varValue))
                // throw new Exception($"Variable {t_varName} cannot be defined and/or already exists.");
                Helper.Error(Program.Filename, "Variable error", $"Variable {t_varName} cannot be defined and/or already exists.", context.Start.Line);
        }
        else
            if (PrivateVariables.ContainsKey(t_varName))
                // throw new Exception($"Variable {t_varName} already exists and is private.");
                Helper.Error(Program.Filename, "Variable error", $"Variable {t_varName} already exists and is private.", context.Start.Line);
        
        // put it in the variables
        Variables[t_varName] = t_varValue;
        
        return null;
    }

    public override object? VisitBlockExpr(BBplusParser.BlockExprContext context) => Visit(context.block());

    public override object? VisitIdExpr(BBplusParser.IdExprContext context)
    {
        var t_varName = context.IDENTIFIER().GetText();
        
        if (ReturnValue != null && Variables.ContainsKey(t_varName) 
                && Variables[t_varName] is null)
            return ReturnValue;
        
        if (!Variables.ContainsKey(t_varName)
            && (!PrivateFunctions.ContainsKey(t_varName) && !PublicFunctions.ContainsKey(t_varName)))
            // throw new Exception($"Variable {t_varName} does not exist." +
                                 // $"\nVariables: {Variables.Keys}");
            Helper.Error(Program.Filename, "Variable error", $"Variable {t_varName} does not exist.", context.Start.Line);
        
        if (PrivateFunctions.TryGetValue(t_varName, out var t_expr))
            return t_expr;
        if (PublicFunctions.TryGetValue(t_varName, out var t_idExpr))
            return t_idExpr;
        
        return Variables[t_varName];
    }

    // Mods are Classes in BB+
    private string _modName = String.Empty;
    public override object? VisitMod(BBplusParser.ModContext context)
    {
        _modName = context.IDENTIFIER().GetText();
        var t_modContent = context.line();

        // Push the current mod onto the stack
        ModStack.Push(_modName);

        try
        {
            // Initialize a result object
            object? t_result = null;

            // Visit each line in the mod
            foreach (var t_line in t_modContent)
                t_result = Visit(t_line);

            // Handle storing the result in the appropriate mod
            // For simplicity, let's assume mods are stored in a dictionary
            // You might want to adjust this based on your application's needs
            var t_currentMod = GetCurrentMod();
            if (!Mods.TryAdd(t_currentMod, t_result))
                Helper.Error(Program.Filename, "Module error", $"Module {t_currentMod} already exists.", context.Start.Line);

            return null;
        }
        finally
        {
            // Pop the current mod from the stack
            // Console.WriteLine("current line number: " + context.Stop.Line);
            _modName = String.Empty;
            ModStack.Pop();
        }
    }
    
    private string GetCurrentMod()
    {
        return ModStack.Count > 0 ? string.Join(".", ModStack.Reverse()) : "";
    }

    public override object? VisitConstant(BBplusParser.ConstantContext context)
    {
        if (context.INTEGER() is { } t_i)
            return int.Parse(t_i.GetText());
        
        if (context.FLOAT() is { } t_f)
            return float.Parse(t_f.GetText());
        
        if (context.STRING() is { } t_s)
            return t_s.GetText()[1..^1];
        
        if (context.BOOL() is { } t_b)
            return bool.Parse(t_b.GetText());

        if (context.NULL() is { })
            return null;

        // throw new NotImplementedException();
        Helper.Error(Program.Filename, "Conversion error", "Cannot convert " + context.GetText() + " to bool", context.Start.Line);
        return null;
    }

    public override object? VisitFunctionCall(BBplusParser.FunctionCallContext context)
    {
        string t_funcName;
        if (context.MOD() is { })
            t_funcName = context.MOD().GetText() + "." + context.IDENTIFIER().GetText();
        else
            t_funcName = context.IDENTIFIER().GetText();
        var t_funcArgs = context.expression().Select(Visit).ToArray();

        if (t_funcName != null && PrivateFunctions.TryGetValue(t_funcName, out var t_expr))
            return t_expr(t_funcArgs);
        if (t_funcName != null && PublicFunctions.TryGetValue(t_funcName, out var t_idExpr))
            return t_idExpr(t_funcArgs);
        
        // throw new Exception($"Function {t_funcName} does not exist.");
        Helper.Error(Program.Filename, "Function error", $"Function {t_funcName} does not exist.", context.Start.Line);
        return null;
    }

    public override object? VisitAddExpr(BBplusParser.AddExprContext context)
    {
        var t_left = Visit(context.expression(0));
        var t_right = Visit(context.expression(1));

        var t_op = context.add().GetText();
        return t_op switch
        {
            "+" => Helper.Add(t_left, t_right),
            "-" => Helper.Subtract(t_left, t_right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitMultExpr(BBplusParser.MultExprContext context)
    {
        var t_left = Visit(context.expression(0));
        var t_right = Visit(context.expression(1));
        
        var t_op = context.mult().GetText();
        return t_op switch
        {
            "*" => Helper.Multiply(t_left, t_right),
            "/" => Helper.Divide(t_left, t_right),
            _ => throw new NotImplementedException()
        };
    }

    // public override object? VisitComment(BBplusParser.CommentContext context) => null;

    public override object? VisitWhileBlock(BBplusParser.WhileBlockContext context)
    {
        Func<object?, bool> t_condition = (context.WHILE().GetText() == "while" || context.WHILE().GetText() == "until")
            ? IsTrue
            : IsFalse;

        if (t_condition(Visit(context.expression())))
        {
            do
            {
                Visit(context.block());
            } while (t_condition(Visit(context.expression())));
        }
        else Visit(context.elseIfBlock());
        
        // return value
        return context.ELSE() is { } ? null : 0;
    }
    
    public override object? VisitIfBlock(BBplusParser.IfBlockContext context)
    {
        if (IsTrue(Visit(context.expression())))
            Visit(context.block());
        else Visit(context.elseIfBlock());
        
        // return value
        return context.ELSE() is { } ? null : 0;
    }

    public override object? VisitCmpExpr(BBplusParser.CmpExprContext context)
    {
        var t_left = Visit(context.expression(0));
        var t_right = Visit(context.expression(1));
        
        var t_op = context.cmp().GetText();
        
        return t_op switch
        {
            "==" => Helper.IsEquals(t_left, t_right),
            "!=" => !Helper.IsEquals(t_left, t_right),
            ">" => Helper.GreaterThan(t_left, t_right),
            "<" => Helper.LessThan(t_left, t_right),
            ">=" => Helper.GreaterThanOrEqual(t_left, t_right),
            "<=" => Helper.LessThanOrEqual(t_left, t_right),
            _ => throw new NotImplementedException()
        };
    }

    public override object? VisitFunctionBlock(BBplusParser.FunctionBlockContext context)
    {
        var t_funcMod = context.MOD() is { };
        var t_funcName = t_funcMod ? context.MOD().GetText() + "." + context.IDENTIFIER().First().GetText()
            : context.IDENTIFIER().First().GetText();
        var t_funcPrv = context.PRIVATE() is { };
        var t_funcArgs = context.IDENTIFIER().Skip(t_funcMod ? 2 : 1).Select(x => x.GetText()).ToArray();
        var t_funcCode = context.block();

        if (t_funcName != null && PrivateFunctions.ContainsKey(t_funcName))
        {
            // throw new Exception($"Function {t_funcName} already exists.");
            Helper.Error(Program.Filename, "Function error", $"Function {t_funcName} already exists and is private.",
                context.Start.Line);
            return null;
        }

        if (t_funcPrv)
        {
            if (t_funcName != null)
                PrivateFunctions[t_funcName] = args =>
                {
                    // Create a dictionary to store function arguments
                    var t_arguments = new Dictionary<string, object?>();

                    // Assign provided arguments to their respective names
                    for (int t_i = 0; t_i < t_funcArgs.Length; t_i++)
                        t_arguments[t_funcArgs[t_i]] = args[t_i];

                    // Visit the function code block while passing the arguments
                    return VisitWithNewVariables(t_funcCode, t_arguments);
                };
        }
        else
        {
            /*if (PublicFunctions.ContainsKey(t_funcName))
                Console.WriteLine("Warning: Function " + t_funcName + " already exists. Replacing.");*/

            if (t_funcName != null)
                PublicFunctions[t_funcName] = args =>
                {
                    // Create a dictionary to store function arguments
                    var t_arguments = new Dictionary<string, object?>();

                    // Assign provided arguments to their respective names
                    for (int t_i = 0; t_i < t_funcArgs.Length; t_i++)
                        t_arguments[t_funcArgs[t_i]] = args[t_i];

                    var t_variables = VisitWithNewVariables(t_funcCode, t_arguments);

                    // check for return
                    if (ReturnValue != null)
                    {
                        // Clear the return value for subsequent function calls
                        var t_returnValue = ReturnValue;
                        return t_returnValue;
                    }

                    return t_variables;
                };
        }
        
        return null;
    }
    
    private object? VisitWithNewVariables(ParserRuleContext context, Dictionary<string, object?> variables)
    {
        // Store current variables
        var t_oldVariables = Variables.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        // Add new variables
        foreach (var t_variable in variables)
            Variables[t_variable.Key] = t_variable.Value;
        
        // Visit the context
        var t_result = Visit(context);
        
        // Restore old variables
        Variables.Clear();
        
        foreach (var t_variable in t_oldVariables)
            Variables[t_variable.Key] = t_variable.Value;
        
        return t_result;
    }

    private bool IsTrue(object? value)
    {
        if (value is bool t_b) return t_b;
        // throw new Exception("Cannot convert " + value?.GetType() + " to bool");
        Helper.Error(Program.Filename, "Conversion error", "Cannot convert " + value?.GetType() + " to bool", null);
        return false;
    }
    
    private bool IsFalse(object? value) => !IsTrue(value);
}