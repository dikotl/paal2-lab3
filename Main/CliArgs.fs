/// <summary>
/// Provides parsing and handling of command-line arguments for the lab3 application.
/// Supports reading an input file, enabling/disabling console output, and setting a visual theme.
/// </summary>
module CliArgs

open System
open System.IO
open ClassLibraryFS.Coloring
open ClassLibraryVB.IO

/// <summary>
/// Represents the usage guide and available options for the lab3 application.
/// This string provides a detailed description of the command-line interface (CLI) 
/// options and how to use the application, including options for setting input files, 
/// user message output, and theme customization.
/// </summary>
[<Literal>]
let usage = $"""
Usage:
    lab3 [input-file] [options]

Options:
    --help|-h               Show help message
    --input-file FILEPATH    Specify a task command sequence file
    --task-to-user BOOL       Print messages to the console
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
"""

/// <summary>
/// Handles parsing of CLI arguments and provides the configured execution context,
/// including the selected theme, input source, and verbosity setting.
/// </summary>
/// <param name="args">Array of command-line arguments.</param>
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
            exit 1
        | "--input-file" :: filename :: tail ->
            inputFile <- openReader filename
            parseArgs tail
        | "--input-file" :: [] ->
            raise (ArgumentException "--file requires <filepath>")
        | "--talk-to-user" :: cond :: tail ->
            let success, parsedCond = bool.TryParse(cond)

            match success with
            | true  -> talkToUser <- parsedCond
            | false -> raise (ArgumentException($"Unable to parse --talkToUser parameter {cond}"))
            parseArgs tail
        | "--talk-to-user" :: [] ->
            raise (ArgumentException "--talkToUser requires [true/false]")
        | "--theme" :: th :: tail ->
            themeArg <- Theme.parseTheme th
            parseArgs tail
        | "--theme" :: [] ->
            raise (ArgumentException "--theme requires <theme>. TIP see --help")
        | opt :: tail when opt.StartsWith("-") ->
            match opt.StartsWith("--") with
            | true -> raise (ArgumentException $"Unknown option: {opt}")
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
                checkNextSymbol (opt.TrimStart('-') |> List.ofSeq)
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
                Console.Error.WriteLine(
                    sprintf
                        "%s%s"
                        (ConsoleColor.DarkRed =>> "Error occurred while parsing Cli arguments:\n")
                        ex.Message)
                exit 2

    /// <summary>
    /// Gets the global theme specified via CLI arguments.
    /// Defaults to Theme.Cold if not specified.
    /// </summary>
    member val GlobalTheme: Theme = themeArg
        with get

    /// <summary>
    /// Gets the input source for commands.
    /// If an input file is specified, it is used; otherwise, Console input is used.
    /// </summary>
    member val InputFile: TextReader = inputFile
        with get

    /// <summary>
    /// Indicates whether output should be printed to the user (true by default).
    /// Can be controlled via the --talkToUser or -q option.
    /// </summary>
    member val TalkToUser: bool = talkToUser
        with get

    /// <summary>
    /// Constructs and returns a Context instance based on parsed CLI arguments.
    /// </summary>
    /// <returns>Configured Context object for executing tasks.</returns>
    member this.getContext(): Context =
        Context(this.InputFile, Console.Out, this.TalkToUser, this.GlobalTheme)
