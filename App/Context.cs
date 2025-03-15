using System;
using System.IO;
using ClassLibrary.Collections;
using ClassLibrary.FunctionalEnumerableOperations;

namespace App;

public class ExitToMenuException : Exception;
public class ExitProgramException : Exception;

public record Context(TextReader Reader, TextWriter Writer, bool TalkToUser)
{
    public void Print(object message)
    {
        if (TalkToUser) Console.Error.Write(message);
    }

    public void PrintLine(object message)
    {
        if (TalkToUser) Console.Error.WriteLine(message);
    }

    public void Write(object message)
    {
        if (TalkToUser) Writer.Write(message);
    }

    public void WriteLine(object message)
    {
        if (TalkToUser) Writer.WriteLine(message);
    }

    public void Error(string? message = null)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        if (message != null)
            PrintLine($"Error! {message}");
        else
            PrintLine($"Error!");

        Console.ForegroundColor = oldColor;
    }

    public T Request<T>(
            Converter<string, T> converter,
            string? message = null,
            bool inline = false,
            bool bare = false)
    {
        while (true)
        {
            try
            {
                return converter(Request(message, inline, bare));
            }
            catch (FormatException e)
            {
                Error(e.Message);
            }
            catch (OverflowException e)
            {
                Error(e.Message);
            }
        }
    }

    public T Request<T>(string? message = null, bool inline = false, bool bare = false)
        where T : IParsable<T>
    {
        return Request(input => T.Parse(input, null), message, inline, bare);
    }

    public string Request(string? message = null, bool inline = false, bool bare = false)
    {
        if (!bare)
        {
            if (inline)
            {
                if (message != null)
                    Print($"{message}: ");
                else
                    Print($": ");
            }
            else
            {
                if (message != null)
                    Print($"{message}\n> ");
                else
                    Print($"\n> ");
            }
        }

        string input = Reader.ReadLine() ?? "";
        string trimmed = input.Trim().ToLower();

        if (trimmed == "menu")
            throw new ExitToMenuException();

        if (trimmed == "exit")
            throw new ExitProgramException();

        return input;
    }

    public DynArray<T> ReadArrayRandom<T>(
            Func<T> getItem)
        where T : IParsable<T>
    {
        var size = Request<int>("Input number of elements");
        var generatedArray = Generator.GetRandomDynArray(size..size, getItem);

        PrintLine($"Generated array: {generatedArray}");
        return generatedArray;
    }

    public DynArray<T> ReadArrayOneLine<T>()
        where T : IParsable<T>
    {
        return Reader
            .ReadLine()!
            .Split()
            .Map(input => T.Parse(input, null))
            .ToDynArray();
    }

    public DynArray<T> RequestArray<T>(
            Func<T> getItem)
        where T : IParsable<T>
    {
        const string message = """
            Select array input method:
                1. Random
                2. One line
            """;

        while (true) try
            {
                return Request<int>(message) switch
                {
                    1 => ReadArrayRandom(getItem),
                    2 => ReadArrayOneLine<T>(),
                    var x => throw new ArgumentException($"Unknown option: {x}"),
                };
            }
            catch (ArgumentException e) { Error(e.Message); }
            catch (OverflowException e) { Error(e.Message); }
            catch (FormatException e) { Error(e.Message); }
    }
}
