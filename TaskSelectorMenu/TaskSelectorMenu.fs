open System
open System.Collections.Generic
open System.IO
open System.Text
open App
open ClassLibraryVB.IO


type Tasks = Dictionary<int, struct (Action<Context> * string)>
type TaskList = Action<Context> List


let generateMenuAndTasks () =
    let menu = new StringBuilder()
    let tasks = new TaskList()
    let taskIndex = ref 0

    menu.Append "Available tasks" |> ignore

    let inline writeTasksToMenu (block: Tasks) (blockNum: int) (i: int ref) =
        for pair in block do
            let taskNum, taskInfo = pair.Deconstruct()
            let struct (task, desc) = taskInfo

            i.Value <- i.Value + 1
            menu.Append $"\n    {i.Value} - Task {blockNum}.{taskNum} {desc}" |> ignore
            tasks.Add task

    writeTasksToMenu Program.Block1Tasks 1 taskIndex
    writeTasksToMenu Program.Block2Tasks 2 taskIndex

    menu.ToString(), tasks


let rec selectTask (tasks: TaskList) (context: Context) =
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


let rec runTaskSelector menu tasks (context: Context) =
    context.PrintLine(menu, ConsoleColor.Cyan)

    try
        (selectTask tasks context).Invoke context
    with
    | :? ExitProgramException -> exit 0
    | :? ExitToMenuException -> ()

    if context.TalkToUser then
        // We are not in "test mode", so run the menu again
        runTaskSelector menu tasks context


/// Opens a file and runs the action if it's exists.
///
/// A file may contains the following:
///
/// ```txt
/// 1
/// 1
/// 10
/// ```
///
/// ... where the first line contains the task number, and the rest - the sequence of input data.
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

    let menu, tasks = generateMenuAndTasks ()
    let run = runTaskSelector menu tasks

    if args.Length > 0 then
        let runWithReader reader =
            let context = Context(reader, Console.Out, false)
            run context

        for filepath in args do
            openAndRun filepath runWithReader
    else
        let context = Context(Console.In, Console.Out, true)
        context.PrintLine("To return to the menu, type 'menu'", ConsoleColor.Cyan)
        context.PrintLine("To exit the program, type 'exit'", ConsoleColor.Cyan)
        run context

    0
