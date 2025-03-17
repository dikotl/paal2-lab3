using System;
using System.ComponentModel;
using System.IO;
using ClassLibrary.Collections;
using ClassLibrary.FunctionalEnumerableOperations;

namespace App;

public class ExitToMenuException : Exception;

public class ExitProgramException : Exception;

/// <summary>
/// Specifies a style of the request. Used in <see cref="Context.Request"/> method.
/// </summary>
public enum RequestStyle
{
    /// <summary>
    /// A request message is placed in the previous line.
    ///
    /// <code>Request message
    /// > |
    /// </code>
    /// </summary>
    Default,

    /// <summary>
    /// The request message is placed on the same line.
    ///
    /// <code>Request message: |</code>
    /// </summary>
    Inline,

    /// <summary>
    /// Just a regular <see cref="Console.ReadLine"/>.
    /// </summary>
    Bare,
}

/// <summary>
/// Program\Task context. Should be used for correct IO operations. Contains helper method
/// to read a data from the user.
/// </summary>
/// <param name="Reader">Input data stream.</param>
/// <param name="Writer">Output data stream.</param>
/// <param name="TalkToUser">Specifies whether to talk to the user via stderr.</param>
public record Context(TextReader Reader, TextWriter Writer, bool TalkToUser)
{
    /// <summary>
    /// Prints a message to the user, but doesn't print it as a program output.
    /// </summary>
    /// <param name="message">A message to be printed.</param>
    public void Print(object message)
    {
        if (TalkToUser) Console.Error.Write(message);
    }

    /// <summary>
    /// Prints a message to the user, but doesn't print it as a program output.
    /// Inserts a new line and the end.
    /// </summary>
    /// <param name="message">A message to be printed.</param>
    public void PrintLine(object message)
    {
        if (TalkToUser) Console.Error.WriteLine(message);
    }

    /// <summary>
    /// Writes a data to the the program output.
    /// </summary>
    /// <param name="message">An object to be written.</param>
    public void Write(object message)
    {
        if (TalkToUser) Writer.Write(message);
    }

    /// <summary>
    /// Writes a data to the the program output. Inserts a new line and the end.
    /// </summary>
    /// <param name="message">An object to be written.</param>
    public void WriteLine(object message)
    {
        if (TalkToUser) Writer.WriteLine(message);
    }

    /// <summary>
    /// Prints a message to the user as error. Inserts a new line and the end.
    /// </summary>
    /// <param name="message">An error to be printed.</param>
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

    /// <summary>
    /// Requests input data of a specific type from the user, reading it from the input stream.
    /// If a <see cref="FormatException"/> or <see cref="ExitProgramException"/> is thrown, an error
    /// will be displayed and the request will be retried.
    /// </summary>
    /// <typeparam name="T">Requested value type.</typeparam>
    /// <param name="converter">A function to be invoked on the input string to convert it to the value.</param>
    /// <param name="message">A message describing what the user is supposed to type in.</param>
    /// <param name="style">Specifies a style of the <paramref name="message"/>.</param>
    /// <returns>A value typed by the user and converted by the <paramref name="converter"/>.</returns>
    /// <exception cref="ExitToMenuException"></exception>
    /// <exception cref="ExitProgramException"></exception>
    public T Request<T>(Converter<string, T> converter, string? message = null, RequestStyle style = RequestStyle.Default)
    {
        while (true)
        {
            try
            {
                return converter(Request(message, style));
            }
            catch (Exception e) when (e is FormatException or OverflowException)
            {
                Error(e.Message);
            }
        }
    }

    /// <summary>
    /// Requests input data of a specific type from the user, reading it from the input stream.
    /// If a <see cref="FormatException"/> or <see cref="ExitProgramException"/> is thrown, an error
    /// will be displayed and the request will be retried.
    /// </summary>
    /// <typeparam name="T">Requested value type, that can be parsed from a string.</typeparam>
    /// <param name="message">A message describing what the user is supposed to type in.</param>
    /// <param name="style">Specifies a style of the <paramref name="message"/>.</param>
    /// <returns>A value typed by the user and converted by the <paramref name="converter"/>.</returns>
    /// <exception cref="ExitToMenuException"></exception>
    /// <exception cref="ExitProgramException"></exception>
    public T Request<T>(string? message = null, RequestStyle style = RequestStyle.Default)
        where T : IParsable<T>
    {
        return Request(input => T.Parse(input, null), message, style);
    }

    /// <summary>
    /// Requests input data of a specific type from the user, reading it from the input stream.
    /// If a <see cref="FormatException"/> or <see cref="ExitProgramException"/> is thrown, an error
    /// will be displayed and the request will be retried.
    /// </summary>
    /// <param name="message">A message describing what the user is supposed to type in.</param>
    /// <param name="style">Specifies a style of the <paramref name="message"/>.</param>
    /// <returns>A string, typed by the user</returns>
    /// <exception cref="ExitToMenuException"></exception>
    /// <exception cref="ExitProgramException"></exception>
    public string Request(string? message = null, RequestStyle style = RequestStyle.Default)
    {
        switch (style)
        {
            case RequestStyle.Default:
                if (message != null) Print(message);
                Print("\n> ");
                break;
            case RequestStyle.Inline:
                if (message != null) Print(message);
                Print(": ");
                break;
            case RequestStyle.Bare:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(style), (int)style, typeof(RequestStyle));
        }

        string input = Reader.ReadLine() ?? "";
        string trimmed = input.Trim().ToLower();

        if (trimmed == "menu")
            throw new ExitToMenuException();

        if (trimmed == "exit")
            throw new ExitProgramException();

        return input;
    }

    /// <summary>
    /// Requests the number of items from the user and generates an array with random items as
    /// defined by <paramref name="getRandomItem"/>.
    /// </summary>
    /// <typeparam name="T">Requested value type.</typeparam>
    /// <param name="getRandomItem">Random elements generation function.</param>
    /// <returns>Generated array with the size specified by the user.</returns>
    public DynArray<T> ReadArrayRandom<T>(Func<T> getRandomItem)
    {
        var size = Request<int>("Input number of elements");
        var generatedArray = Generator.GetRandomDynArray(size..size, getRandomItem);

        PrintLine($"Generated array: {generatedArray}");
        return generatedArray;
    }

    /// <summary>
    /// Requests elements from the user and collects them into an array element by element as
    /// defined by the converter <paramref name="converter"/>.
    /// </summary>
    /// <typeparam name="T">Requested value type.</typeparam>
    /// <param name="converter">A function to be invoked on the input string to convert it to the value.</param>
    /// <returns>An array of values typed by the user.</returns>
    public DynArray<T> ReadArrayInline<T>(Converter<string, T> converter)
    {
        return Reader
            .ReadLine()!
            .Split()
            .Map(input => converter(input))
            .ToDynArray();
    }

    /// <summary>
    /// Requests elements from the user and collects them into an array element by element.
    /// </summary>
    /// <typeparam name="T">Requested value type, that can be parsed from a string.</typeparam>
    /// <returns>An array of values typed by the user.</returns>
    public DynArray<T> ReadArrayInline<T>()
        where T : IParsable<T>
    {
        return Reader
            .ReadLine()!
            .Split()
            .Map(input => T.Parse(input, null))
            .ToDynArray();
    }

    /// <summary>
    /// Requests the user to specify which input method to use to enter the array.
    /// </summary>
    /// <typeparam name="T">Requested value type, that can be parsed from a string.</typeparam>
    /// <param name="getRandomItem">Random elements generation function.</param>
    /// <returns>An array of values typed by the user.</returns>
    public DynArray<T> RequestArray<T>(Func<T> getRandomItem)
        where T : IParsable<T>
    {
        return RequestArray(input => T.Parse(input, null), getRandomItem);
    }

    /// <summary>
    /// Requests the user to specify which input method to use to enter the array.
    /// </summary>
    /// <typeparam name="T">Requested value type.</typeparam>
    /// <param name="converter">A function to be invoked on the input string to convert it to the value.</param>
    /// <param name="getRandomItem">Random elements generation function.</param>
    /// <returns>An array of values typed by the user.</returns>
    public DynArray<T> RequestArray<T>(Converter<string, T> converter, Func<T> getRandomItem)
    {
        bool isRandom = ChooseInputMethod();

        if (isRandom)
        {
            return ReadArrayRandom(getRandomItem);
        }
        else
        {
            while (true)
            {
                try
                {
                    return ReadArrayInline(converter);
                }
                catch (Exception e) when (e is FormatException or OverflowException)
                {
                    Error(e.Message);
                }
            }
        }
    }

    public DynArray<DynArray<T>> RequestMatrix<T>(Func<T> getRandomItem)
        where T : IParsable<T>
    {
        return RequestMatrix(input => T.Parse(input, null), getRandomItem);
    }

    /// <summary>
    /// Requests a matrix of dynamic arrays, using a specified converter and random item generator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix, constrained to be parsable.</typeparam>
    /// <param name="converter">A converter that parses a string input into an element of type T.</param>
    /// <param name="getRandomItem">A function that generates random items of type T.</param>
    /// <returns>A dynamically sized matrix (array of arrays) with parsed or randomly generated elements.</returns>
    public DynArray<DynArray<T>> RequestMatrix<T>(Converter<string, T> converter, Func<T> getRandomItem)
    {
        var size = Request<int>("Input number of sub-arrays");
        bool isRandomOrWasError = ChooseInputMethod();

        var result = isRandomOrWasError
            ? RequestMatrixRandom(size, getRandomItem)
            : RequestMatrixInline(size, converter, out isRandomOrWasError);

        if (isRandomOrWasError)
        {
            PrintLine("Gotten matrix:");

            foreach (DynArray<T> item in result)
            {
                WriteLine(item);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads and returns a matrix represented as a dynamic array of rows,  
    /// where each row is a dynamic array of elements of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    /// <param name="size">The number of rows in the matrix.</param>
    /// <param name="converter">A converter function to transform input strings into type <typeparamref name="T"/>.</param>
    /// <param name="show">Outputs a flag indicating whether an error occurred during input processing.</param>
    /// <returns>A dynamic array of dynamic arrays representing the matrix.</returns>
    private DynArray<DynArray<T>> RequestMatrixInline<T>(int size, Converter<string, T> converter, out bool show)
    {
        var typed = new DynArray<DynArray<T>>(length: size);
        show = false;

        for (int i = 0; i < size; i++)
        {
            try
            {
                typed[i] = ReadArrayInline(converter);
            }
            catch (Exception e) when (e is FormatException or OverflowException)
            {
                Error(e.Message);
                i--;
                show = true;
            }
        }

        return typed;
    }

    /// <summary>
    /// Generates and returns a matrix represented as a dynamic array of rows,  
    /// where each row is a dynamic array of randomly generated elements of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="getRandomItem">A function that generates a random item of type <typeparamref name="T"/>.</param>
    /// <returns>A dynamic array of dynamic arrays representing the randomly generated matrix.</returns>

    private DynArray<DynArray<T>> RequestMatrixRandom<T>(int rows, Func<T> getRandomItem)
    {
        // TODO: handle invalid number range.
        var maxColumnSize = Request<int>("Input max number of elements") - 1;
        var generated = new DynArray<DynArray<T>>(length: rows);

        for (int row = 0; row < rows; row++)
        {
            generated[row] = Generator.GetRandomDynArray(2..maxColumnSize, getRandomItem);
        }

        return generated;
    }

    /// <summary>
    /// Requests from the user what input method should be used.
    /// </summary>
    /// <returns>Is input method random.</returns>
    private bool ChooseInputMethod()
    {
        const string message =
            """
            Select input method:
                1. Random
                2. Line by line
            """;

        while (true)
        {
            switch (Request(message).Trim())
            {
                case "1": return true;
                case "2": return false;
            }
            Error("Unknown option");
        }
    }
}