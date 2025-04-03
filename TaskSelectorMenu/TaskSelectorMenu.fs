open System
open System.Collections.Generic
open System.IO
open System.Text
open App
open ClassLibraryVB.IO


let center (s: string) totalWidth =
    let padding = max 0 (totalWidth - s.Length)
    let leftPadding = padding / 2
    s.PadLeft(s.Length + leftPadding).PadRight(totalWidth)

let numLength n =
    if n = 0 then
        1
    else
        n //
        |> Math.Abs
        |> Math.Log10
        |> int
        |> (+) 1

type Color =
    | Black
    | DarkBlue
    | DarkGreen
    | DarkCyan
    | DarkRed
    | DarkMagenta
    | DarkYellow
    | Gray
    | DarkGray
    | Blue
    | Green
    | Cyan
    | Red
    | Magenta
    | Yellow
    | White


let (=>>) color s =
    match color with
    | Black       -> "\x1B[30m" + s + "\x1B[0m"
    | DarkBlue    -> "\x1B[34m" + s + "\x1B[0m"
    | DarkGreen   -> "\x1B[32m" + s + "\x1B[0m"
    | DarkCyan    -> "\x1B[36m" + s + "\x1B[0m"
    | DarkRed     -> "\x1B[31m" + s + "\x1B[0m"
    | DarkMagenta -> "\x1B[35m" + s + "\x1B[0m"
    | DarkYellow  -> "\x1B[33m" + s + "\x1B[0m"
    | Gray        -> "\x1B[37m" + s + "\x1B[0m"
    | DarkGray    -> "\x1B[90m" + s + "\x1B[0m"
    | Blue        -> "\x1B[94m" + s + "\x1B[0m"
    | Green       -> "\x1B[92m" + s + "\x1B[0m"
    | Cyan        -> "\x1B[96m" + s + "\x1B[0m"
    | Red         -> "\x1B[91m" + s + "\x1B[0m"
    | Magenta     -> "\x1B[95m" + s + "\x1B[0m"
    | Yellow      -> "\x1B[93m" + s + "\x1B[0m"
    | White       -> "\x1B[97m" + s + "\x1B[0m"


let generateTable
    (header: string)
    (keyValues: (string * string) seq)
    borderColor
    headerColor
    keyColor
    valueColor //
    =
    let maxKeyLen =
        keyValues //
        |> Seq.map (fun (key, _) -> key.Length)
        |> Seq.max

    let maxValueLen =
        keyValues //
        |> Seq.map (fun (_, value) -> value.Length)
        |> Seq.max

    // 3 for corners and 4 for spaces around
    let totalLength = 7 + maxKeyLen + maxValueLen


    let bars n = //
        borderColor =>> String.replicate n "═"

    let top =
        sprintf //
            "%s%s%s"
            (borderColor =>> "╔")
            (bars (totalLength - 2))
            (borderColor =>> "╗")

    let middle =
        sprintf //
            "%s%s%s%s%s"
            (borderColor =>> "╠")
            (borderColor =>> bars (maxKeyLen + 2))
            (borderColor =>> "╦")
            (bars (maxValueLen + 2))
            (borderColor =>> "╣")

    let bottom =
        sprintf //
            "%s%s%s%s%s"
            (borderColor =>> "╚")
            (bars (maxKeyLen + 2))
            (borderColor =>> "╩")
            (bars (maxValueLen + 2))
            (borderColor =>> "╝")

    let header =
        (borderColor =>> "║") //
        + (headerColor =>> center header (totalLength - 2))
        + (borderColor =>> "║")

    let row le (ri: string) =
        sprintf //
            "%s %s %s %s %s"
            (borderColor =>> "║")
            (keyColor =>> le.ToString().PadRight maxKeyLen)
            (borderColor =>> "║")
            (valueColor =>> ri.PadRight maxValueLen)
            (borderColor =>> "║")

    let rows =
        keyValues //
        |> Seq.map (fun (key, desc) -> row key desc)
        |> String.concat "\n"

    String.concat "\n" [ top; header; middle; rows; bottom ]


let generateMenuAndTasks () =
    let tasks =
        seq {
            yield! Program.Block1Tasks
            yield! Program.Block2Tasks
        }

    let keyValues =
        tasks //
        |> Seq.mapi (fun i task -> (i+1).ToString(), snd (task.Value.ToTuple()))

    let actions =
        tasks //
        |> Seq.map (fun task -> fst (task.Value.ToTuple()))

    let menu =
        generateTable "Available tasks" keyValues Cyan White Blue Gray

    menu, actions


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
    context.PrintLine(menu, ConsoleColor.Cyan)

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

    let menu, tasks = generateMenuAndTasks ()
    let run = runTaskSelector menu (List tasks)

    if args.Length > 0 then
        let runWithReader reader =
            let context = Context(reader, Console.Out, false)
            run context

        for filepath in args do
            openAndRun filepath runWithReader
    else
        let context = Context(Console.In, Console.Out, true)

        let data = seq [
            ("menu", "return to the menu")
            ("exit", "exit the program")
            ("clear", "clear the console")
        ]

        context.PrintLine(generateTable "Available Commands" data Cyan White Blue Gray)
        run context

    0


(*
Good themes


Classic
# Task
DarkGray  Green  Yellow  Gray
# Commands
DarkGray  Cyan  Yellow  Gray
# Other
Magenta


Blue Accents
# Task
Blue  Green  Yellow  White
# Commands
Blue  Cyan  Yellow  White
# Other
Cyan


Hackerman
# Task
DarkGray  Green  Green  Gray
# Commands
DarkGray  Green  Green  Gray
# Other
DarkGreen


Cold(Not Max)
# Task
Cyan  White  Blue  Gray
# Commands
Cyan  White  Blue  Gray
# Other
DarkCyan
*)
