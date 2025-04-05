module ClassLibraryFS.Text

open System

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
