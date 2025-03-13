open System
open System.IO
open App


let rec selectTask (reader: TextReader) (writer: TextWriter) talkToUser =
    if talkToUser then
        eprintfn
            """Select task:
    1 - ...
    2 - ...
    3 - ..."""

    let input =
        let rec getTaskNumber () : int =
            if talkToUser then
                eprintf "Task: "

            let parsed, result =
                reader //
                |> _.ReadLine()
                |> _.Trim()
                |> Int32.TryParse

            if parsed then
                result
            else
                if talkToUser then
                    eprintfn "Error! Invalid input"

                getTaskNumber ()

        getTaskNumber ()

    match input with
    | 1 -> App.Task1
    | 2 -> App.Task2
    | 3 -> App.Task3
    | _ ->
        if talkToUser then
            eprintfn "Error! Unknown task"

        selectTask reader writer talkToUser


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

    let task = selectTask reader writer (args.Length = 0)
    task (reader, writer)

    0
