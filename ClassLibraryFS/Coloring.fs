module ClassLibraryFS.Coloring

open System
open System.Runtime.CompilerServices

open type System.ConsoleColor

type Theme =
    {
        Border: ConsoleColor
        Header: ConsoleColor
        Key:    ConsoleColor
        Value:  ConsoleColor
        Other:  ConsoleColor
    }

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
