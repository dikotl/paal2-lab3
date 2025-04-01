open System
open System.Collections.Generic
open System.IO
open System.Text
open App


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
                context.PrintLine "Error! Invalid input"
                getTaskNumber ()

        getTaskNumber () - 1

    if taskIndex >= tasks.Count || taskIndex < 0 then
        context.PrintLine "Error! Unknown task"
        selectTask tasks context
    else
        tasks[taskIndex]


let rec runTaskSelector menu tasks (context: Context) =
    context.PrintLine menu

    let task = selectTask tasks context

    context.PrintLine "To return to the menu, type 'menu'"
    context.PrintLine "To exit the program, type 'exit'"

    try
        task.Invoke context
    with
    | :? ExitProgramException -> exit 0
    | :? ExitToMenuException -> ()

    if context.TalkToUser then
        runTaskSelector menu tasks context


[<EntryPoint>]
let main args =
    let menu, tasks = generateMenuAndTasks ()
    let run = runTaskSelector menu tasks

    if args.Length > 0 then
        for arg in args do
            try
                use reader = new StreamReader(arg) :> TextReader
                let context = Context(reader, Console.Out, false)
                run context
            with :? IOException as e ->
                eprintfn $"Error while processing program arguments. {e.Message}"
    else
        let context = Context(Console.In, Console.Out, true)
        run context

    0
