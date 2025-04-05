module CliArgs

open System
open System.IO
open ClassLibraryFS.Coloring
open ClassLibraryVB.IO

[<Literal>]
let usage = $"""
Usage: 
lab3 [options|arguments]
lab3 [path-to-input-file]
lab3 [path-to-input-file] [options]

Options & arguments:
    --help|-h               Show help message
    --inputFile FILEPATH    Specify a task command sequence file
    --taskToUser BOOL       Print messages to the console
    --theme THEME           Set color theme, available themes:
                            - Default
                            - Classic
                            - BlueAccents
                            - Hackerman
                            - Cold
                            - Warm
                            - Sunset
                            - Forest
                            - Ocean

    -q                      Set talkToUser to false
    -td                     Set theme to Default
    -tcl                    Set theme to Classic
    -tb                     Set theme to BlueAccents
    -th                     Set theme to Hackerman
    -tc                     Set theme to Cold
    -tw                     Set theme to Warm
    -ts                     Set theme to Sunset
    -tf                     Set theme to Forest
    -fo                     Set theme to Ocean

path-to-input-file:
    The path to the input file for the program.
"""

type CliHandler(args: string array) =
    let openReader (filepath: string) =
        try
            new StreamReader(filepath)
            :> TextReader
            |> _.ReadToEnd()
            |> (fun x -> new StringReader(x + "\nexit"))
            :> TextReader
        with :? IOException as e ->
            raise (ArgumentException $"Error while processing program arguments. {e.Message}")

    let mutable themeArg = Theme.Cold
    let mutable inputFile = Console.In
    let mutable talkToUser = true

    let rec parseArgs args =
        match args with
        | [] -> ()
        | "-h" :: _ | "--help" :: _ ->
            Console.Error.WriteLine usage
            System.Environment.Exit(1)
        | "--inputFile" :: filename :: tail ->
            inputFile <- openReader filename
            parseArgs tail
        | "--inputFile" :: [] ->
            raise (ArgumentException "--file requires <filepath>")
        | "--talkToUser" :: cond :: tail ->
            let success, parsedCond = bool.TryParse(cond)

            match success with
            | true  -> talkToUser <- parsedCond
            | false -> raise (ArgumentException($"Unable to parse --talkToUser parameter {cond}"))
            parseArgs tail
        | "--talkToUser" :: [] ->
            raise (ArgumentException "--talkToUser requires [true/false]")
        | "--theme" :: th :: tail ->
            themeArg <- Theme.parseTheme th
            parseArgs tail
        | "--theme" :: [] ->
            raise (ArgumentException "--theme requires <theme>. TIP see --help")
        | arg :: tail when arg.StartsWith("-") ->
            match arg.StartsWith("--") with
            | true -> raise (ArgumentException $"Unknown argument: {arg}")
            | false ->
                let rec checkNextSymbol remainingChars =
                    match remainingChars with
                    | [] -> ()
                    | ch :: rest ->
                        match ch with
                        | 'q' -> talkToUser <- false
                        | 't' ->
                            match rest with
                            | 'd' :: _ -> 
                                themeArg <- Theme.Default
                            | 'c' :: 'l' :: _ -> 
                                themeArg <- Theme.Classic
                            | 'b' :: _ -> 
                                themeArg <- Theme.BlueAccents
                            | 'h' :: _ -> 
                                themeArg <- Theme.Hackerman
                            | 'c' :: _ -> 
                                themeArg <- Theme.Cold
                            | 'w' :: _ -> 
                                themeArg <- Theme.Warm
                            | 's' :: _ -> 
                                themeArg <- Theme.Sunset
                            | 'f' :: _ -> 
                                themeArg <- Theme.Forest
                            | 'o' :: _ -> 
                                themeArg <- Theme.Ocean
                            | _ -> ()
                        | _ -> ()
                        checkNextSymbol rest
                checkNextSymbol (arg.TrimStart('-') |> List.ofSeq)
                parseArgs tail
        | unknown :: tail ->
            raise (ArgumentException $"Unknown argument: {unknown}")
            parseArgs tail
    
    do
        if args.Length > 0 then
            let argsList = args |> Array.toList

            try
                match args.[0].StartsWith("-") with
                | true -> parseArgs argsList
                | false ->
                    parseArgs (List.tail argsList)
                    inputFile <- openReader args.[0]
            with ex ->
                Console.Error.WriteLine (sprintf "%s%s" (ConsoleColor.DarkRed =>> "Error occured while parsing Cli arguments:\n") ex.Message)
                System.Environment.Exit(2)

            

    member val GlobalTheme: Theme = themeArg with get

    member val InputFile: TextReader = inputFile with get

    member val TalkToUser: bool = talkToUser with get

    member this.getContext(): Context = 
        Context(this.InputFile, Console.Out, this.TalkToUser, this.GlobalTheme)
