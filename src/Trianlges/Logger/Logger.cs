using System;

namespace Trianlges.Logger;

public class Logger
{
    public Logger()
    {
        
    }
    
    public static void Log(string message, LoggerLevel level)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[Error | {0}] ", DateTime.Now);
        Console.WriteLine(message);
    }
}