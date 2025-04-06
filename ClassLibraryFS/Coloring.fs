/// <summary>
/// A module that provides support for theming and ANSI color formatting in the console.
/// </summary>
module ClassLibraryFS.Coloring

open System
open System.Runtime.CompilerServices

open type System.ConsoleColor

/// <summary>
/// Represents a color theme for console output.
/// </summary>
type Theme =
    {
        /// <summary>
        /// The color used for borders.
        /// </summary>
        Border: ConsoleColor

        /// <summary>
        /// The color used for headers.
        /// </summary>
        Header: ConsoleColor

        /// <summary>
        /// The color used for keys.
        /// </summary>
        Key:    ConsoleColor

        /// <summary>
        /// The color used for values.
        /// </summary>
        Value:  ConsoleColor

        /// <summary>
        /// The color used for miscellaneous elements.
        /// </summary>
        Other:  ConsoleColor
    }

    /// <summary>
    /// A collection of predefined themes accessible by name.
    /// </summary>
    static member themes : Map<string, Theme> =
        Map.ofList [
            "default",     Theme.Default
            "classic",     Theme.Classic
            "blueaccents", Theme.BlueAccents
            "hackerman",   Theme.Hackerman
            "cold",        Theme.Cold
            "warm",        Theme.Warm
            "sunset",      Theme.Sunset
            "forest",      Theme.Forest
            "ocean",       Theme.Ocean
        ]

    /// <summary>
    /// Parses a theme by name. If not found, returns the default theme.
    /// </summary>
    /// <param name="str">The name of the theme.</param>
    /// <returns>The corresponding Theme if found, otherwise Default.</returns>
    static member parseTheme(str: string) =
        match Theme.themes.TryFind (str.ToLower()) with
        | Some theme -> theme
        | None -> Theme.Default

    /// <summary>
    /// Represents the default color scheme for the application. All elements are displayed in gray.
    /// </summary>
    static member Default =
        {
            Border = Gray
            Header = Gray
            Key    = Gray
            Value  = Gray
            Other  = Gray
        }

    /// <summary>
    /// Represents the classic color scheme for the application with green headers, yellow keys, and magenta for other elements.
    /// </summary>
    static member Classic =
        {
            Border = DarkGray
            Header = Green
            Key    = Yellow
            Value  = Gray
            Other  = Magenta
        }

    /// <summary>
    /// Represents a blue accent color scheme with blue borders and cyan accents for other elements.
    /// </summary>
    static member BlueAccents =
        {
            Border = Blue
            Header = Green
            Key    = Yellow
            Value  = White
            Other  = Cyan
        }

    /// <summary>
    /// Represents a hacker-inspired color scheme with dark green and green accents throughout the interface.
    /// </summary>
    static member Hackerman =
        {
            Border = DarkGray
            Header = Green
            Key    = Green
            Value  = Gray
            Other  = DarkGreen
        }

    /// <summary>
    /// Represents a cold theme with cyan borders and blue accents, creating a cool and fresh visual style.
    /// </summary>
    static member Cold =
        {
            Border = Cyan
            Header = White
            Key    = Blue
            Value  = Gray
            Other  = DarkCyan
        }

    /// <summary>
    /// Represents a warm theme with red borders, yellow headers, and magenta for keys, providing a warm and cozy feel.
    /// </summary>
    static member Warm =
        {
            Border = Red
            Header = Yellow
            Key    = Magenta
            Value  = White
            Other  = DarkRed
        }

    /// <summary>
    /// Represents a sunset-themed color scheme with dark red borders and yellow headers, evoking the colors of a sunset.
    /// </summary>
    static member Sunset =
        {
            Border = DarkRed
            Header = Yellow
            Key    = Red
            Value  = Magenta
            Other  = DarkYellow
        }

    /// <summary>
    /// Represents a forest-inspired theme with dark green and green accents to create an earthy, nature-filled atmosphere.
    /// </summary>
    static member Forest =
        {
            Border = DarkGreen
            Header = Green
            Key    = Yellow
            Value  = White
            Other  = DarkGreen
        }

    /// <summary>
    /// Represents an ocean-inspired color scheme with blue and cyan elements, reflecting the colors of the sea.
    /// </summary>
    static member Ocean =
        {
            Border = Blue
            Header = Cyan
            Key    = DarkBlue
            Value  = White
            Other  = DarkCyan
        }

/// <summary>
/// Colors a string using ANSI escape codes for a given ConsoleColor.
/// </summary>
/// <param name="color">The color to use.</param>
/// <param name="s">The string to wrap in color.</param>
/// <returns>The string wrapped in ANSI color codes.</returns>
let (=>>) color s =
    match color with
    | ConsoleColor.Black       -> "\x1B[30m" + s + "\x1B[0m"
    | ConsoleColor.DarkBlue    -> "\x1B[34m" + s + "\x1B[0m"
    | ConsoleColor.DarkGreen   -> "\x1B[32m" + s + "\x1B[0m"
    | ConsoleColor.DarkCyan    -> "\x1B[36m" + s + "\x1B[0m"
    | ConsoleColor.DarkRed     -> "\x1B[31m" + s + "\x1B[0m"
    | ConsoleColor.DarkMagenta -> "\x1B[35m" + s + "\x1B[0m"
    | ConsoleColor.DarkYellow  -> "\x1B[33m" + s + "\x1B[0m"
    | ConsoleColor.Gray        -> "\x1B[37m" + s + "\x1B[0m"
    | ConsoleColor.DarkGray    -> "\x1B[90m" + s + "\x1B[0m"
    | ConsoleColor.Blue        -> "\x1B[94m" + s + "\x1B[0m"
    | ConsoleColor.Green       -> "\x1B[92m" + s + "\x1B[0m"
    | ConsoleColor.Cyan        -> "\x1B[96m" + s + "\x1B[0m"
    | ConsoleColor.Red         -> "\x1B[91m" + s + "\x1B[0m"
    | ConsoleColor.Magenta     -> "\x1B[95m" + s + "\x1B[0m"
    | ConsoleColor.Yellow      -> "\x1B[93m" + s + "\x1B[0m"
    | ConsoleColor.White       -> "\x1B[97m" + s + "\x1B[0m"
    | _ -> s

/// <summary>
/// Extension method for strings to wrap them in ANSI escape codes for a given ConsoleColor.
/// </summary>
[<Extension>]
type StringExtension() =
    /// <summary>
    /// Wraps the string in ANSI escape codes using the specified ConsoleColor.
    /// </summary>
    /// <param name="str">The string to format.</param>
    /// <param name="color">The ConsoleColor to apply.</param>
    /// <returns>The ANSI-colored string.</returns>
    [<Extension>]
    static member WrapEscColor(str: string, color: ConsoleColor) =
        color =>> str
