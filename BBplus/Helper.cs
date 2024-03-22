namespace BBplus;

public class Helper
{
    public static object? Add(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l + t_r;
        
        if (left is float t_lf && right is float t_rf)
            return t_lf + t_rf;
        
        if (left is int t_lint && right is float t_rfloat)
            return t_lint + t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat + t_rint;
        
        if (left is string || right is string) return $"{left}{right}";
        
        throw new Exception("Cannot add " + left?.GetType() + " and " + right?.GetType());
    }

    public static object? Subtract(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l - t_r;

        if (left is float t_lf && right is float t_rf)
            return t_lf - t_rf;

        if (left is int t_lint && right is float t_rfloat)
            return t_lint - t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat - t_rint;
        
        throw new Exception("Cannot subtract " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static object? Multiply(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l * t_r;
        
        if (left is float t_lf && right is float t_rf)
            return t_lf * t_rf;
        
        if (left is int t_lint && right is float t_rfloat)
            return t_lint * t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat * t_rint;
        
        throw new Exception("Cannot multiply " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static object? Divide(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l / t_r;
        
        if (left is float t_lf && right is float t_rf)
            return t_lf / t_rf;
        
        if (left is int t_lint && right is float t_rfloat)
            return t_lint / t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat / t_rint;
        
        throw new Exception("Cannot divide " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static bool IsEquals(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l == t_r;
        
        if (left is float t_lf && right is float t_rf)
            return Math.Abs(t_lf - t_rf) < 0.01f;
        
        if (left is int t_lint && right is float t_rfloat)
            return Math.Abs(t_lint - t_rfloat) < 0.01f;
        
        if (left is float t_lfloat && right is int t_rint)
            return Math.Abs(t_lfloat - t_rint) < 0.01f;
        
        if (left is string t_ls && right is string t_rs)
            return t_ls == t_rs;
        
        if (left is bool t_lb && right is bool t_rb)
            return t_lb == t_rb;
        
        if (left is null && right is null)
            return true;
        
        throw new Exception("Cannot compare " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static bool GreaterThan(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l > t_r;
        
        if (left is float t_lf && right is float t_rf)
            return t_lf > t_rf;
        
        if (left is int t_lint && right is float t_rfloat)
            return t_lint > t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat > t_rint;
        
        throw new Exception("Cannot compare " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static bool LessThan(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l < t_r;
        
        if (left is float t_lf && right is float t_rf)
            return t_lf < t_rf;
        
        if (left is int t_lint && right is float t_rfloat)
            return t_lint < t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat < t_rint;
        
        throw new Exception("Cannot compare " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static bool GreaterThanOrEqual(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l >= t_r;
        
        if (left is float t_lf && right is float t_rf)
            return t_lf >= t_rf;
        
        if (left is int t_lint && right is float t_rfloat)
            return t_lint >= t_rfloat;
        
        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat >= t_rint;
        
        throw new Exception("Cannot compare " + left?.GetType() + " and " + right?.GetType());
    }

    public static bool LessThanOrEqual(object? left, object? right)
    {
        if (left is int t_l && right is int t_r)
            return t_l <= t_r;

        if (left is float t_lf && right is float t_rf)
            return t_lf <= t_rf;

        if (left is int t_lint && right is float t_rfloat)
            return t_lint <= t_rfloat;

        if (left is float t_lfloat && right is int t_rint)
            return t_lfloat <= t_rint;

        throw new Exception("Cannot compare " + left?.GetType() + " and " + right?.GetType());
    }
    
    public static void Error(string file, string title, string message, int? line)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(title + ": " + message);

        if (line is not null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("File: " + file + ", Line: " + line);
        }
        
        Console.ResetColor();
        Environment.Exit(1);
    }
}