namespace BBplus;

public class Color
{
    public static string Name { get; set; } = "";
    public ConsoleColor CColor { get; set; }

    public Color(string name, ConsoleColor color) => (Name, CColor) = (name, color);
}

public static class Functions
{
    public static readonly Color[] Colors = new[]
    {
        new Color("Black", System.ConsoleColor.Black),
        new Color("Blue", System.ConsoleColor.Blue),
        new Color("Cyan", System.ConsoleColor.Cyan),
        new Color("DarkBlue", System.ConsoleColor.DarkBlue),
        new Color("DarkCyan", System.ConsoleColor.DarkCyan),
        new Color("DarkGray", System.ConsoleColor.DarkGray),
        new Color("DarkGreen", System.ConsoleColor.DarkGreen),
        new Color("DarkMagenta", System.ConsoleColor.DarkMagenta),
        new Color("DarkRed", System.ConsoleColor.DarkRed),
        new Color("DarkYellow", System.ConsoleColor.DarkYellow),
        new Color("Gray", System.ConsoleColor.Gray),
        new Color("Green", System.ConsoleColor.Green),
        new Color("Magenta", System.ConsoleColor.Magenta),
        new Color("Red", System.ConsoleColor.Red),
        new Color("White", System.ConsoleColor.White),
        new Color("Yellow", System.ConsoleColor.Yellow)
    };
    
    public static object? Message(object?[] args)
    {
        Console.WriteLine(string.Join(" ", args).Replace("\\n", "\r\n"));
        return null;
    }
    
    public static object? Wait(object?[] args)
    {
        Thread.Sleep((int)(args[0] ?? 1000));
        return null;
    }
    
    public static object? ConsoleColor(object?[] args)
    {
        switch (args)
        {
            case [Color t_color]:
                Console.ForegroundColor = t_color.CColor;
                break;
            case [string t_color]:
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), t_color);
                break;
            case [int t_color]:
                Console.ForegroundColor = (ConsoleColor)t_color;
                break;
            default:
                Console.ResetColor();
                break;
        }
        return null;
    }
    
    public static object? Function(object?[] args) => args[0];
}