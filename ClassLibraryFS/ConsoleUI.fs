module ClassLibraryFS.ConsoleUI

open System
open System.IO
open System.Collections.Generic
open ClassLibraryFS
open ClassLibraryFS.Coloring

[<Interface>]
type public IContext =
    abstract member Reader: TextReader
    abstract member Writer: TextWriter
    abstract member TalkToUser: Boolean
    abstract member GlobalTheme: Theme
    abstract member HelpMenu: String

let parseTasks<'T when 'T :> IContext>
    (blocks: KeyValuePair<int, struct(Action<'T> * string)> seq)
    =
    blocks
    |> Seq.mapi (fun i x ->
        let (task, desc) = x.Value.ToTuple()
        i + 1, desc, task)

let getIndexedDescriptions<'T when 'T :> IContext>
    (tasks: (int * string * 'T Action) seq)
    =
    tasks
    |> Seq.mapi (fun i (_, description, _) -> (i+1).ToString(), description)

let generateTable
    (header: string) (keyValues: (string * string) seq) (theme: Theme)
    =
    let maxKeyLen =
        keyValues
        |> Seq.map (fun (key, _) -> key.Length)
        |> Seq.max

    let maxValueLen =
        keyValues
        |> Seq.map (fun (_, value) -> value.Length)
        |> Seq.max

    // 3 for corners and 4 for spaces around
    let totalLength = 7 + maxKeyLen + maxValueLen

    let bars n =
        theme.Border =>> String.replicate n "═"

    let top =
        sprintf
            "%s%s%s"
            (theme.Border =>> "╔")
            (bars (totalLength - 2))
            (theme.Border =>> "╗")

    let middle =
        sprintf
            "%s%s%s%s%s"
            (theme.Border =>> "╠")
            (theme.Border =>> bars (maxKeyLen + 2))
            (theme.Border =>> "╦")
            (bars (maxValueLen + 2))
            (theme.Border =>> "╣")

    let bottom =
        sprintf
            "%s%s%s%s%s"
            (theme.Border =>> "╚")
            (bars (maxKeyLen + 2))
            (theme.Border =>> "╩")
            (bars (maxValueLen + 2))
            (theme.Border =>> "╝")

    let header =
        (theme.Border =>> "║")
        + (theme.Header =>> Text.center header (totalLength - 2))
        + (theme.Border =>> "║")

    let row le (ri: string) =
        sprintf
            "%s %s %s %s %s"
            (theme.Border =>> "║")
            (theme.Key =>> le.ToString().PadRight maxKeyLen)
            (theme.Border =>> "║")
            (theme.Value =>> ri.PadRight maxValueLen)
            (theme.Border =>> "║")

    let rows =
        keyValues
        |> Seq.map (fun (key, desc) -> row key desc)
        |> String.concat "\n"

    String.concat "\n" [ top; header; middle; rows; bottom ]
