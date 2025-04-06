open System
open System.Text
open System.Collections.Generic
open Tasks
open Cli
open ClassLibraryVB.IO
open ClassLibraryFS.ConsoleUI


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
    | :? ДоПобаченняException -> exit 0
    | :? ДавайПоНовійException -> ()

    if context.Reader = Console.In then
        runTaskSelector menu tasks context


[<EntryPoint>]
let main args =
    Console.InputEncoding <- Encoding.UTF8
    Console.OutputEncoding <- Encoding.UTF8

    let argsHandler = Args.Handler(args)

    let parsedTasks =
        seq {
            yield! Tasks.Block1
            yield! Tasks.Block2
        }
        |> parseTasks

    let tasksMenu =
        generateTable "Available tasks" (getIndexedDescriptions parsedTasks) argsHandler.GlobalTheme

    let tasks = parsedTasks |> Seq.map (fun (_, _, task) -> task) |> List

    let context = argsHandler.getContext ()

    runTaskSelector tasksMenu tasks context

    0
