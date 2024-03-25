using System.Collections;
using Antlr4.Runtime;
using BBplus.Content;

namespace BBplus;

public class Mod(string name)
{
    public string Name { get; } = name;
    public Dictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> PrivateVariables { get; } = new Dictionary<string, object?>();
}

public class BBplusVisitor : BBplusBaseVisitor<object?>
{
    private Dictionary<string, object?> Variables { get; } = new();
    private Dictionary<string, object?> PrivateVariables { get; } = new();
    
    private Dictionary<string, Func<object?[], object?>> PrivateFunctions { get; } = new();
    private Dictionary<string, Func<object?[], object?>> PublicFunctions { get; } = new();
    
    private Dictionary<string, Mod> Mods { get; } = new Dictionary<string, Mod>();
    private Stack<Mod> ModStack { get; } = new Stack<Mod>();
    
    // Set to keep track of active mods
    private HashSet<Mod> ActiveMods { get; } = new HashSet<Mod>();
    
    private object? ReturnValue { get; set; }

    public BBplusVisitor()
    {
        // Initialize normal variables
        var t_normalVariables = new Dictionary<string, double>
        {
            { "_PI_", Math.PI },
            { "_E_", Math.E }
        };
        foreach (var t_variable in t_normalVariables)
        {
            PrivateVariables[t_variable.Key] = t_variable.Value;
            Variables[t_variable.Key] = t_variable.Value;
        }
        
        // Initialize colors
        var t_colorNames = new[]
        {
            "Black", "DarkBlue", "DarkGreen", "DarkCyan", "DarkRed",
            "DarkMagenta", "DarkYellow", "Gray", "DarkGray", "Blue",
            "Green", "Cyan", "Red", "Magenta", "White", "Yellow"
        };
        for (int t_i = 0; t_i < t_colorNames.Length; t_i++)
        {
            var t_colorName = t_colorNames[t_i];
            PrivateVariables[t_colorName] = Functions.Colors[t_i];
            Variables[t_colorName] = Functions.Colors[t_i];
        }
    
        // Initialize functions
        var t_functions = new Dictionary<string, Func<object?[], object?>>
        {
            { "message", Functions.Message },
            { "wait", Functions.Wait },
            { "throw", Throw },
            { "consoleColor", Functions.ConsoleColor }
        };
        foreach (var t_function in t_functions)
        {
            PrivateFunctions[t_function.Key] = t_function.Value;
        }
    }

    
    private object? Throw(object?[] args)
    {
        // basically a return function
        // Console.WriteLine(args.FirstOrDefault());
        ReturnValue = args.FirstOrDefault();
        return ReturnValue;
    }
    
    private void HandleError(string errorType, string errorMessage, int lineNumber)
    {
        Helper.Error(Program.Filename, errorType, errorMessage, lineNumber);
    }

    private void HandleError(string errorType, string errorMessage, int? lineNumber = null)
    {
        Helper.Error(Program.Filename, errorType, errorMessage, lineNumber);
    }
    
    public override object VisitAssignement(BBplusParser.AssignementContext context)
    {
        var isPrivate = context.PRIVATE() is not null;
        var varName = context.IDENTIFIER().GetText();
        var varValue = Visit(context.expression());

        if (ModStack.Count > 0)
        {
            // Get the current mod from the stack
            var currentMod = ModStack.Peek();

            if (isPrivate)
            {
                if (varValue != null && !currentMod.Variables.TryAdd(varName, varValue))
                    HandleError("Variable error", $"Variable {varName} cannot be defined and/or already exists.", context.Start.Line);
                else
                    currentMod.PrivateVariables[varName] = varValue;
            } 
            else currentMod.Variables[varName] = varValue;
        }
        else
        {
            // No Mod context, operate on normal variables
            if (isPrivate)
            {
                if (varValue != null && !Variables.TryAdd(varName, varValue))
                    HandleError("Variable error", $"Variable {varName} cannot be defined and/or already exists.", context.Start.Line);
                else
                    PrivateVariables[varName] = varValue;
            }
            else Variables[varName] = varValue;
        }
        
        // Console.WriteLine(ModStack.Peek().Name + "." + varName + " = " + varValue);
        // Console.WriteLine($"{varName} = {varValue} is stored in {ModStack.Peek().Name}.");

        return null!;
    }
    public override object? VisitBlockExpr(BBplusParser.BlockExprContext context) => Visit(context.block());

    public override object? VisitIdExpr(BBplusParser.IdExprContext context)
    {
        var varName = context.IDENTIFIER().GetText();

        // Check if the variable exists in the current mod
        if (ModStack.Count > 0)
        {
            foreach (var mod in ModStack)
            {
                if ((mod.Variables.TryGetValue(varName, out var modVariable)
                    || mod.PrivateVariables.TryGetValue(varName, out modVariable)))
                    return modVariable;
            }
        }

        // If not found, check normal variables
        if (Variables.TryGetValue(varName, out var normalVariable)
            || PrivateVariables.TryGetValue(varName, out normalVariable))
            return normalVariable;

        // Handle case when the variable is not found
        Helper.Error(Program.Filename, "Variable error", $"Variable {varName} does not exist.", context.Start.Line);
        return null;
    }


    // Mods are Classes in BB+
    private string _modName = String.Empty;
    public override object? VisitMod(BBplusParser.ModContext context)
    {
        var modName = context.IDENTIFIER().GetText();
        var mod = Mods.TryGetValue(modName, out var existingMod) ? existingMod : new Mod(modName);

        ModStack.Push(mod);

        try
        {
            // Visit each line in the mod
            foreach (var line in context.line())
                Visit(line);

            // Store the mod instance in the Mods dictionary
            Mods.TryAdd(modName, mod);

            // Add the mod to the set of active mods
            ActiveMods.Add(mod);

            return null;
        }
        finally
        {
            // Check if the mod is no longer in use, then pop it from the stack
            if (!IsModInUse(mod))
                ModStack.Pop();
        }
    }

    // Method to check if a mod is in use
    private bool IsModInUse(Mod mod)
    {
        // Iterate through active mods and check if the given mod is in use
        foreach (var activeMod in ActiveMods)
        {
            if (activeMod == mod || activeMod.Variables.Values.Any(value => value == mod))
                return true;
        }
        return false;
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