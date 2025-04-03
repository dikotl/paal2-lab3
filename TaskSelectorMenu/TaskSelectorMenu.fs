open System
open System.IO
open System.Text
open System.Collections.Generic
open Tasks
open ClassLibraryVB.IO
open ClassLibraryFS.ConsoleUI
open ClassLibraryFS.Coloring

open type System.ConsoleColor

let rec selectTask (tasks: Context Action List) (context: Context) =
    let taskIndex =
        let rec getTaskNumber () =
            try
                context.Request<int> "Select task"
            with
            | :? FormatException
            | :? OverflowException ->
                context.Error "Invalid input"
                getTaskNumber ()

        getTaskNumber () - 1

    if taskIndex >= tasks.Count || taskIndex < 0 then
        context.Error "Unknown task"
        selectTask tasks context
    else
        tasks[taskIndex]

let rec runTaskSelector (menu: string) tasks (context: Context) =
    context.PrintLine(menu)

    try
        (selectTask tasks context).Invoke context
    with
    | :? ExitProgramException -> exit 0
    | :? ExitToMenuException -> ()

    if context.TalkToUser then
        // We are not in "test mode", so run the menu again
        runTaskSelector menu tasks context

/// <summary>
///  Opens a file and runs the action if it's exists.
/// <code>
/// 1
/// 1
/// 10
/// </code>
/// ... where the first line contains the task number, and the rest - the sequence of input data.
/// </summary>
/// <param name="filepath">The path to the file to open.</param>
/// <param name="applyReader">The function to run with the reader as an argument.</param>
let openAndRun (filepath: string) applyReader =
    try
        use reader = new StreamReader(filepath) :> TextReader
        applyReader reader
    with :? IOException as e ->
        eprintfn $"Error while processing program arguments. {e.Message}"


[<EntryPoint>]
let main args =
    Console.InputEncoding <- Encoding.UTF8
    Console.OutputEncoding <- Encoding.UTF8

    let globalTheme = Theme.Cold

    let parsedTasks =
        seq {
            yield! Tasks.Block1
            yield! Tasks.Block2
        }
        |> parseTasks


    let helpMenu = 
        generateTable 
            "Available Commands" 
            [ 
                ("menu", "return to the menu")
                ("exit", "exit the program")
                ("help", "show this table") 
                ("clear", "clear the console") 
            ] 
            globalTheme

    let tasksMenu = generateTable "Available tasks" (getIndexedDescriptions parsedTasks) globalTheme

    let tasks = 
        parsedTasks
        |> Seq.map (fun (_, _, task) -> task)
        |> List

    let run = runTaskSelector tasksMenu tasks

    if args.Length > 0 then
        let runWithReader reader =
            let context = Context(reader, Console.Out, false, globalTheme.ToVBTheme(), helpMenu)
            run context

        for filepath in args do
            openAndRun filepath runWithReader
    else
        let context = Context(Console.In, Console.Out, true, globalTheme.ToVBTheme(), helpMenu)

        context.PrintLine(helpMenu)
        run context

    0
