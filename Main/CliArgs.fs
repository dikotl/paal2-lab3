module CliArgs

open System
open System.IO
open ClassLibraryFS.Coloring
open ClassLibraryVB.IO

type Argument = 
    static member help: string       = "--help"
    static member theme: string      = "--theme"
    static member inputFile: string  = "--inputFile"
    static member talkToUser: string = "--talkToUser"

[<Literal>]
let usage = """Usage: lab3 [options]

Options:
    --help                  Show help message
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
"""

type CliHandler(args: string array) =
    let _help =
        if Array.contains Argument.help args then
            Console.Error.WriteLine(usage)
            System.Environment.Exit(1)

    let getArgValue (arg: string) =
        args
        |> Array.skipWhile ((<>) arg)
        |> Array.tryItem 1        

    let openReader (filepath: string) =
        try
            new StreamReader(filepath)
            :> TextReader
            |> _.ReadToEnd()
            |> (fun x -> new StringReader(x + "\nexit"))
            :> TextReader
        with :? IOException as e ->
            raise (ArgumentException $"Error while processing program arguments. {e.Message}")

    let themeArg = getArgValue Argument.theme
    let inputFileArg = getArgValue Argument.inputFile
    let talkToUserArg = getArgValue Argument.talkToUser

    member val GlobalTheme: Theme = 
        themeArg
        |> Option.map Theme.parseTheme
        |> Option.defaultValue Theme.Cold
        with get

    member val InputFile: TextReader = 
        inputFileArg
        |> Option.map openReader
        |> Option.defaultValue Console.In
        with get

    member val TalkToUser: bool = 
        talkToUserArg
        |> Option.map (fun talkToUser ->
            try
                bool.Parse talkToUser
            with :? FormatException as e ->
                raise (ArgumentException $"Error while processing program arguments. {e.Message}"))
        |> Option.defaultValue true
        with get

    member this.getContext(): Context = 
        Context(this.InputFile, Console.Out, this.TalkToUser, this.GlobalTheme)
