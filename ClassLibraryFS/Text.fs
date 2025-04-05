module ClassLibraryFS.Text

open System

/// Centers a string within the specified total width.
/// 
/// <param name="s">The string to center.</param>
/// <param name="totalWidth">The total width in which to center the string.</param>
/// <returns>A new string that is centered within the given width.</returns>
let center (s: string) totalWidth =
    let padding = max 0 (totalWidth - s.Length)
    let leftPadding = padding / 2
    s.PadLeft(s.Length + leftPadding).PadRight(totalWidth)

/// Calculates the number of digits in an integer.
/// 
/// <param name="n">The integer whose length is to be calculated.</param>
/// <returns>The number of digits in the integer.</returns>
let numLength n =
    if n = 0 then
        1
    else
        n //
        |> Math.Abs
        |> Math.Log10
        |> int
        |> (+) 1
