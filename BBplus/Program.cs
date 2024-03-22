using System;
using Antlr4.Runtime;
using BBplus.Content;

namespace BBplus;

static class Program
{
    public static string Filename = "";
    
    static void Main(string[] args)
    {
        /*if (args.Length != 1)
        {
            Console.WriteLine("Interpreter for BrainBox+ / Bb+.\n" +
                              "Usage: BBplus.exe <filename>\n" +
                              "Example: BBplus.exe test.bbp");
            return;
        }*/
        
        // var t_filename = args[0];
        Filename = "Content/test.bbp";
        var t_file = File.ReadAllText(Filename);
        
        AntlrInputStream t_input = new(t_file);
        BBplusLexer t_lexer = new(t_input);
        CommonTokenStream t_tokens = new(t_lexer);
        BBplusParser t_parser = new(t_tokens);
        BBplusParser.ProgramContext t_context = t_parser.program();
        BBplusVisitor t_visitor = new();
        t_visitor.Visit(t_context);
    }
}