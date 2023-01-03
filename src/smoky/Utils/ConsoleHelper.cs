using System;

namespace tomware.Smoky;

public static class ConsoleHelper
{
  public static void Exit(string reason)
  {
    WriteLineError(reason);
    Environment.Exit(1);
  }

  public static void WriteLineYellow(string value)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(value);
    Console.ForegroundColor = ConsoleColor.White;
  }

  public static void WriteLineSuccess(string value)
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(value);
    Console.ForegroundColor = ConsoleColor.White;
  }

  public static void WriteLineError(string value)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(value);
    Console.ForegroundColor = ConsoleColor.White;
  }

  public static void WriteLine(string value)
  {
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(value);
  }
}