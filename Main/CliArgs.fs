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

type CliHandler(args: string array) =
    let _help =
        if Array.contains Argument.help args then
            // TODO make help message
            Console.Error.WriteLine("HELP MESSAGE")
            System.Environment.Exit(0)

    let getArgValue (arg: string) =
        // TODO optimize
        if Array.contains arg args then
            args.[Array.IndexOf(args, arg) + 1]
        else null

    let appendExit (original: TextReader) =
        // TODO make normal Reader exit
        let originalText = original.ReadToEnd()
        let finalText = originalText + "\nexit"
        new StringReader(finalText) :> TextReader

    let openReader (filepath: string) =
        try
           appendExit (new StreamReader(filepath) :> TextReader)
        with :? IOException as e ->
            raise (ArgumentException $"Error while processing program arguments. {e.Message}")


    let themeArg = getArgValue Argument.theme
    let inputFileArg = getArgValue Argument.inputFile
    let talkToUserArg = getArgValue Argument.talkToUser


    member val GlobalTheme: Theme = 
        if (isNull themeArg) then
            Theme.Cold
        else
            Theme.parseTheme(themeArg) 
        with get

    member val InputFile: TextReader = 
        if (isNull inputFileArg) then
            Console.In
        else
            openReader inputFileArg 
        with get

    member val TalkToUser: bool = 
        if (isNull talkToUserArg) then
            true
        else
            try
                bool.Parse talkToUserArg
            with ex ->
                raise (ArgumentException $"Error while processing program arguments. {ex.Message}")
        with get

    member this.getContext(): Context = 
        Context(this.InputFile, Console.Out, this.TalkToUser, this.GlobalTheme)
