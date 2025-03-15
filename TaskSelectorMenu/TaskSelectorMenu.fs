﻿open System
open System.Collections.Generic
open System.IO
open System.Text
open App
open ClassLibrary.FunctionalEnumerableOperations


type Tasks = Dictionary<int, struct (Action<Context> * string)>
type TaskList = Action<Context> List


let taskMenu () =
    let menu = new StringBuilder()
    let tasks = new TaskList()

    menu.Append "Available tasks" |> ignore

    let inline writeTasksToMenu (block: Tasks) (blockNum: int) =
        for pair, i in block.ToIndexedEnumerable() do
            let taskNum, taskInfo = pair.Deconstruct()
            let task, desc = taskInfo.ToTuple()

            menu.Append $"\n    {i + 1} - Task {blockNum}.{taskNum} {desc}" |> ignore
            tasks.Add task

    writeTasksToMenu Program.Block1Tasks 1
    writeTasksToMenu Program.Block2Tasks 2

    menu.ToString(), tasks


let rec selectTask (tasks: TaskList) (context: Context) =
    let taskIndex =
        let rec getTaskNumber () =
            try
                context.Request<int> "Select task"
            with
            | :? FormatException
            | :? OverflowException ->
                context.PrintLine "Error! Invalid input"
                getTaskNumber ()

        getTaskNumber () - 1

    if taskIndex >= tasks.Count || taskIndex < 0 then
        context.PrintLine "Error! Unknown task"
        selectTask tasks context
    else
        tasks[taskIndex]


[<EntryPoint>]
let main args =
    use reader =
        if args.Length > 0 then
            new StreamReader(args[0]) :> TextReader
        else
            Console.In

    use writer =
        if args.Length > 1 then
            new StreamWriter(args[1]) :> TextWriter
        else
            Console.Out

    let context = Context(reader, writer, args.Length = 0)
    let menu, tasks = taskMenu ()

    let rec runTaskSelector () =
        try
            context.PrintLine menu

            let task = selectTask tasks context

            context.PrintLine "To return to the menu, type 'menu'"
            context.PrintLine "To exit the program, type 'exit'"
            task.Invoke context

            runTaskSelector ()
        with
        | :? ExitToMenuException -> runTaskSelector ()
        | :? ExitProgramException -> ()

    runTaskSelector ()
    0
