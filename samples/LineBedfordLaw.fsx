// Chart.Line example.
// Benford law: https://en.wikipedia.org/wiki/Benford%27s_law
// Get files in %TEMP% and count first digits of their lengths.

open FSharp.Charting
open System
open System.IO

let data =
    Directory.GetFiles(Environment.GetEnvironmentVariable("TEMP"), "*", SearchOption.AllDirectories)
    |> Array.map (fun x -> FileInfo(x).Length.ToString().Substring(0, 1) |> int)
    |> Array.countBy id
    |> Array.sortBy fst

fun () ->
    Chart
        .Line(data, Name="Digit count")
        .WithXAxis(Title="Digit", Min=1.0, Max=9.0)
        .WithYAxis(Title="Count")

|> Chart.Show(
    title="Benford Law",
    modal=fsi.CommandLineArgs.[0].EndsWith(".fsx")
)
