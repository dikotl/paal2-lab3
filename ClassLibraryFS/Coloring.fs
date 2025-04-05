/// A module that provides support for theming and ANSI color formatting in the console.
module ClassLibraryFS.Coloring

open System
open System.Runtime.CompilerServices

open type System.ConsoleColor

/// Represents a color theme for console output.
type Theme =
    {
        /// The color used for borders.
        Border: ConsoleColor

        /// The color used for headers.
        Header: ConsoleColor

        /// The color used for keys.
        Key:    ConsoleColor

        /// The color used for values.
        Value:  ConsoleColor

        /// The color used for miscellaneous elements.
        Other:  ConsoleColor
    }

    /// A collection of predefined themes accessible by name.
    static member themes : Map<string, Theme> =
        Map.ofList [
            "Default",     Theme.Default
            "Classic",     Theme.Classic
            "BlueAccents", Theme.BlueAccents
            "Hackerman",   Theme.Hackerman
            "Cold",        Theme.Cold
            "Warm",        Theme.Warm
            "Sunset",      Theme.Sunset
            "Forest",      Theme.Forest
            "Ocean",       Theme.Ocean
        ]

    /// Parses a theme by name. If not found, returns the default theme.
    /// <param name="str">The name of the theme.</param>
    /// <returns>The corresponding Theme if found, otherwise Default.</returns>
    static member parseTheme(str: string) =
        match Theme.themes.TryFind str with
        | Some theme -> theme
        | None -> Theme.Default

    static member Default =
        {
            Border = Gray
            Header = Gray
            Key    = Gray
            Value  = Gray
            Other  = Gray
        }

    static member Classic =
        {
            Border = DarkGray
            Header = Green
            Key    = Yellow
            Value  = Gray
            Other  = Magenta
        }

    static member BlueAccents =
        {
            Border = Blue
            Header = Green
            Key    = Yellow
            Value  = White
            Other  = Cyan
        }

    static member Hackerman =
        {
            Border = DarkGray
            Header = Green
            Key    = Green
            Value  = Gray
            Other  = DarkGreen
        }

    static member Cold =
        {
            Border = Cyan
            Header = White
            Key    = Blue
            Value  = Gray
            Other  = DarkCyan
        }

    static member Warm =
        {
            Border = Red
            Header = Yellow
            Key    = Magenta
            Value  = White
            Other  = DarkRed
        }

    static member Sunset =
        {
            Border = DarkRed
            Header = Yellow
            Key    = Red
            Value  = Magenta
            Other  = DarkYellow
        }

    static member Forest =
        {
            Border = DarkGreen
            Header = Green
            Key    = Yellow
            Value  = White
            Other  = DarkGreen
        }

    static member Ocean =
        {
            Border = Blue
            Header = Cyan
            Key    = DarkBlue
            Value  = White
            Other  = DarkCyan
        }


/// Colors a string using ANSI escape codes for a given ConsoleColor.
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

/// Extension method for strings to wrap them in ANSI escape codes for a given ConsoleColor.
[<Extension>]
type WrapEscColor() =
    /// Wraps the string in ANSI escape codes using the specified ConsoleColor.
    /// <param name="str">The string to format.</param>
    /// <param name="color">The ConsoleColor to apply.</param>
    /// <returns>The ANSI-colored string.</returns>
    [<Extension>]
    static member WrapEscColor(str: string, color: ConsoleColor) =
        color =>> str
