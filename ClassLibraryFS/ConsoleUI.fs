module ClassLibraryFS.ConsoleUI

open System
open System.IO
open System.Collections.Generic
open ClassLibraryFS
open ClassLibraryFS.Coloring

/// Defines the interface for an execution context that interacts with the user.
[<Interface>]
type public IContext =
    /// Gets the input stream.
    abstract member Reader: TextReader

    /// Gets the output stream.
    abstract member Writer: TextWriter

    /// Indicates whether the context should interact with the user.
    abstract member TalkToUser: Boolean

    /// Gets the current console color theme.
    abstract member GlobalTheme: Theme

    /// Gets the help menu string.
    abstract member HelpMenu: String

/// Parses task definitions from key-value blocks and assigns each a sequential index.
/// 
/// <param name="blocks">A sequence of key-value pairs with an action and description.</param>
/// <returns>A sequence of indexed tuples (index, description, action).</returns>
let parseTasks<'T when 'T :> IContext>
    (blocks: KeyValuePair<int, struct(Action<'T> * string)> seq)
    =
    blocks
    |> Seq.mapi (fun i x ->
        let (task, desc) = x.Value.ToTuple()
        i + 1, desc, task)

/// Extracts indexed descriptions from a sequence.
/// 
/// <param name="tasks">A sequence of (index, description, action) tuples.</param>
/// <returns>A sequence of (string index, description) pairs.</returns>
let getIndexedDescriptions<'T when 'T :> IContext>
    (tasks: (int * string * 'T Action) seq)
    =
    tasks
    |> Seq.mapi (fun i (_, description, _) -> (i+1).ToString(), description)

/// Generates a formatted table string using Unicode box-drawing characters and ANSI colors.
/// 
/// <param name="header">The title displayed at the top of the table.</param>
/// <param name="keyValues">A sequence of key-description pairs to display in the table.</param>
/// <param name="theme">The console color theme used for styling the table.</param>
/// <returns>A formatted string representing the styled table.</returns>
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
