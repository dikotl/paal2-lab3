module ClassLibraryFS.Coloring

open System
open System.Runtime.CompilerServices

type Theme =
    {
        Border: ConsoleColor
        Header: ConsoleColor
        Key:    ConsoleColor
        Value:  ConsoleColor
        Other:  ConsoleColor
    }

        static member parseTheme(str: string) =
            match str with
            | "Default"     -> Theme.Default
            | "Classic"     -> Theme.Classic
            | "BlueAccents" -> Theme.BlueAccents
            | "Hackerman"   -> Theme.Hackerman
            | "Cold"        -> Theme.Cold
            | "Warm"        -> Theme.Warm
            | "Sunset"      -> Theme.Sunset
            | "Forest"      -> Theme.Forest
            | "Ocean"       -> Theme.Ocean
            | _             -> Theme.Default

        static member Default =
            {
                Border = ConsoleColor.Gray
                Header = ConsoleColor.Gray
                Key    = ConsoleColor.Gray
                Value  = ConsoleColor.Gray
                Other  = ConsoleColor.Gray
            }

        static member Classic =
            {
                Border = ConsoleColor.DarkGray
                Header = ConsoleColor.Green
                Key    = ConsoleColor.Yellow
                Value  = ConsoleColor.Gray
                Other  = ConsoleColor.Magenta
            }

        static member BlueAccents =
            {
                Border = ConsoleColor.Blue
                Header = ConsoleColor.Green
                Key    = ConsoleColor.Yellow
                Value  = ConsoleColor.White
                Other  = ConsoleColor.Cyan
            }

        static member Hackerman =
            {
                Border = ConsoleColor.DarkGray
                Header = ConsoleColor.Green
                Key    = ConsoleColor.Green
                Value  = ConsoleColor.Gray
                Other  = ConsoleColor.DarkGreen
            }

        static member Cold =
            {
                Border = ConsoleColor.Cyan
                Header = ConsoleColor.White
                Key    = ConsoleColor.Blue
                Value  = ConsoleColor.Gray
                Other  = ConsoleColor.DarkCyan
            }

        static member Warm =
            {
                Border = ConsoleColor.Red
                Header = ConsoleColor.Yellow
                Key    = ConsoleColor.Magenta
                Value  = ConsoleColor.White
                Other  = ConsoleColor.DarkRed
            }

        static member Sunset =
            {
                Border = ConsoleColor.DarkRed
                Header = ConsoleColor.Yellow
                Key    = ConsoleColor.Red
                Value  = ConsoleColor.Magenta
                Other  = ConsoleColor.DarkYellow
            }

        static member Forest =
            {
                Border = ConsoleColor.DarkGreen
                Header = ConsoleColor.Green
                Key    = ConsoleColor.Yellow
                Value  = ConsoleColor.White
                Other  = ConsoleColor.DarkGreen
            }

        static member Ocean =
            {
                Border = ConsoleColor.Blue
                Header = ConsoleColor.Cyan
                Key    = ConsoleColor.DarkBlue
                Value  = ConsoleColor.White
                Other  = ConsoleColor.DarkCyan
            }

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

[<Extension>]
type WrapEscColor() =
    [<Extension>]
    static member WrapEscColor(str: string, color: ConsoleColor) =
        color =>> str
