using System;
using System.ComponentModel;
using System.IO;
using ClassLibrary.Collections;
using ClassLibrary.FunctionalEnumerableOperations;

namespace ClassLibrary.IO;

/// <summary>
/// Exception thrown to indicate the program should exit to the menu.
/// </summary>
public class ExitToMenuException : Exception;

/// <summary>
/// Exception thrown to indicate the program should exit completely.
/// </summary>
public class ExitProgramException : Exception;

/// <summary>
/// Specifies a style for the request message in the Context.Request* methods.
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
/// Represents the context for program I/O operations. It includes methods to read and write data from/to the user.
/// </summary>
/// <param name="Reader">The input data stream.</param>
/// <param name="Writer">The output data stream.</param>
/// <param name="TalkToUser">Indicates whether to communicate with the user through stderr.</param>
public record Context(TextReader Reader, TextWriter Writer, bool TalkToUser)
{
    /// <summary>
    /// Prints a message to the console without a newline, using the specified color.
    /// If the TalkToUser variable is set to True,
    /// the message will be written to the standard error stream (Console.Error).
    /// </summary>
    /// <param name="message">The message to print.</param>
    /// <param name="color">(Optional) The text color. Default is white.</param>
    public void Print(object message, ConsoleColor color = ConsoleColor.Gray)
    {
        if (TalkToUser)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Error.Write(message);
            Console.ForegroundColor = oldColor;
        }
    }

    /// <summary>
    /// Prints a message to the console with the specified color.
    /// If the TalkToUser variable is set to True,
    /// the message will be written to the standard error stream (Console.Error).
    /// </summary>
    /// <param name="message">The message to print.</param>
    /// <param name="color">(Optional) The text color. Default is white.</param>
    public void PrintLine(object message, ConsoleColor color = ConsoleColor.Gray)
    {
        if (TalkToUser)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }

    /// <summary>
    /// Writes a message to the program output.
    /// </summary>
    /// <param name="message">The message to be written.</param>
    public void Write(object message)
    {
        Writer.Write(message);
    }

    /// <summary>
    /// Writes a message to the program output, appending a new line at the end.
    /// </summary>
    /// <param name="message">The message to be written.</param>
    public void WriteLine(object message)
    {
        Writer.WriteLine(message);
    }

    /// <summary>
    /// Prints an error message to the user, appending a new line at the end.
    /// </summary>
    /// <param name="message">The error message to be printed.</param>
    public void Error(string? message = null)
    {
        PrintLine("Error! " + message ?? "", ConsoleColor.Red);
    }

    /// <summary>
    /// Requests input from the user, attempting to convert it to a specific type.
    /// If an exception occurs, an error message is shown and the request is retried.
    /// </summary>
    /// <typeparam name="T">The type of the requested value.</typeparam>
    /// <param name="converter">A function that converts the input string into the requested type.</param>
    /// <param name="message">A message that guides the user on what to input.</param>
    /// <param name="style">The style of the request message.</param>
    /// <returns>The user input converted to the specified type.</returns>
    /// <exception cref="ExitToMenuException">Thrown when the user chooses to return to the menu.</exception>
    /// <exception cref="ExitProgramException">Thrown when the user chooses to exit the program.</exception>
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
    /// Requests input from the user, attempting to convert it to a specified type that implements IParsable.
    /// If an exception occurs, an error message is shown and the request is retried.
    /// </summary>
    /// <typeparam name="T">The type of the requested value, constrained to be IParsable.</typeparam>
    /// <param name="message">A message that guides the user on what to input.</param>
    /// <param name="style">The style of the request message.</param>
    /// <returns>The user input converted to the specified type.</returns>
    /// <exception cref="ExitToMenuException">Thrown when the user chooses to return to the menu.</exception>
    /// <exception cref="ExitProgramException">Thrown when the user chooses to exit the program.</exception>
    public T Request<T>(string? message = null, RequestStyle style = RequestStyle.Default)
        where T : IParsable<T>
    {
        return Request(input => T.Parse(input, null), message, style);
    }

    /// <summary>
    /// Requests a string input from the user and returns it.
    /// </summary>
    /// <param name="message">A message that guides the user on what to input.</param>
    /// <param name="style">The style of the request message.</param>
    /// <returns>The user input as a string.</returns>
    /// <exception cref="ExitToMenuException">Thrown when the user chooses to return to the menu.</exception>
    /// <exception cref="ExitProgramException">Thrown when the user chooses to exit the program.</exception>
    public string Request(string? message = null, RequestStyle style = RequestStyle.Default)
    {
    begin:
        switch (style)
        {
            case RequestStyle.Default:
                if (message != null) Print(message, ConsoleColor.Magenta);
                Print("\n> ");
                break;
            case RequestStyle.Inline:
                if (message != null) Print(message, ConsoleColor.Magenta);
                Print(": ", ConsoleColor.Magenta);
                break;
            case RequestStyle.Bare:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(style), (int)style, typeof(RequestStyle));
        }

        string input = Reader.ReadLine() ?? "";

        switch (input.Trim().ToLower())
        {
            case "menu":
                throw new ExitToMenuException();
            case "exit":
                throw new ExitProgramException();
            case "clear":
                Console.Clear();
                goto begin; // OMG 🔥 SUPER 🔥 DUPER 🔥 SEMI-UNSAFE 🔥 UNCONDITIONAL 🔥 JUMP
            default:
                return input;
        }
    }

    /// <summary>
    /// Requests the number of elements from the user and generates an array of random elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="getRandomItem">A function that generates random elements of type T.</param>
    /// <returns>An array of randomly generated elements of type T.</returns>
    public DynArray<T> ReadArrayRandom<T>(Func<T> getRandomItem)
    {
        var size = Request(SizeInt, "Input number of elements");
        var generatedArray = Generator.GetRandomDynArray(size..size, getRandomItem);

        PrintLine($"Generated array: {generatedArray}");
        return generatedArray;
    }

    /// <summary>
    /// Requests elements from the user and collects them into an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="converter">A function that converts each input string into the requested type.</param>
    /// <returns>An array of elements of type T entered by the user.</returns>
    public DynArray<T> ReadArrayInline<T>(Converter<string, T> converter)
    {
        return Reader
            .ReadLine()!
            .Split()
            .Map(input => converter(input))
            .ToDynArray();
    }

    /// <summary>
    /// Requests elements from the user and collects them into an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array, constrained to be IParsable.</typeparam>
    /// <returns>An array of elements of type T entered by the user.</returns>
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
    /// Requests the user to specify the input method for entering an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array, constrained to be IParsable.</typeparam>
    /// <param name="getRandomItem">A function that generates random elements of type T.</param>
    /// <returns>An array of elements of type T entered by the user.</returns>
    public DynArray<T> RequestArray<T>(Func<T> getRandomItem)
        where T : IParsable<T>
    {
        return RequestArray(input => T.Parse(input, null), getRandomItem);
    }

    /// <summary>
    /// Requests the user to specify the input method for entering an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="converter">A function that converts each input string into the requested type.</param>
    /// <param name="getRandomItem">A function that generates random elements of type T.</param>
    /// <returns>An array of elements of type T entered by the user.</returns>
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

    /// <summary>
    /// Requests a two-dimensional matrix of elements of type T, where T implements IParsable.
    /// </summary>
    /// <param name="getRandomItem">A function that generates random items of type T.</param>
    /// <typeparam name="T">The type of the elements in the matrix, constrained to be IParsable.</typeparam>
    /// <returns>A dynamically sized matrix (array of arrays) with elements of type T.</returns>
    public DynArray<DynArray<T>> RequestMatrix<T>(Func<T> getRandomItem)
        where T : IParsable<T>
    {
        return RequestMatrix(input => T.Parse(input, null), getRandomItem);
    }

    /// <summary>
    /// Requests a matrix of elements, using a specified converter and random item generator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix, constrained to be parsable.</typeparam>
    /// <param name="converter">A function that converts each input string into the requested type.</param>
    /// <param name="getRandomItem">A function that generates random elements of type T.</param>
    /// <returns>A dynamically sized matrix (array of arrays) with parsed or randomly generated elements.</returns>
    public DynArray<DynArray<T>> RequestMatrix<T>(Converter<string, T> converter, Func<T> getRandomItem)
    {
        var size = Request(SizeInt, "Input number of sub-arrays");
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
    /// Reads and returns a matrix as a dynamic array of rows, each row being a dynamic array of elements of type T.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    /// <param name="size">The number of rows in the matrix.</param>
    /// <param name="converter">A function that converts input strings to the desired type T.</param>
    /// <param name="show">Outputs a flag indicating if any error occurred during input processing.</param>
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
    /// Reads and returns a matrix of random elements based on the given size and generator function.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="getRandomItem">A function that generates a random item of type <typeparamref name="T"/>.</param>
    /// <returns>A dynamic array of dynamic arrays representing the randomly generated matrix.</returns>
    private DynArray<DynArray<T>> RequestMatrixRandom<T>(int rows, Func<T> getRandomItem)
    {
        var maxColumnSize = Request(SizeInt, "Input max number of elements");
        var generated = new DynArray<DynArray<T>>(length: rows);

        for (int row = 0; row < rows; row++)
        {
            generated[row] = Generator.GetRandomDynArray(1..maxColumnSize, getRandomItem);
        }

        return generated;
    }

    /// <summary>
    /// Prompts the user to choose between inputting elements manually or generating them randomly.
    /// </summary>
    /// <returns>A boolean indicating whether the random input method was chosen.</returns>
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

    /// <summary>
    /// Parses <paramref name="input"/> as int and ensures it's positive.
    /// </summary>
    /// <param name="input">The user input as a string.</param>
    /// <returns>A valid array size.</returns>
    private static int SizeInt(string input)
    {
        var size = int.Parse(input);

        if (size < 1)
            throw new FormatException($"Value {size} is not in valid range (1..{int.MaxValue})");

        return size;
    }
}
