// Chart.Line example.
// Benford law: https://en.wikipedia.org/wiki/Benford%27s_law
// Get files in FARHOME and count first digits of their lengths.

open FSharp.Charting
open System
open System.IO

let data =
    Directory.GetFiles(Environment.GetEnvironmentVariable("FARHOME"), "*", SearchOption.AllDirectories)
    |> Seq.map (fun x -> FileInfo(x).Length.ToString().Substring(0, 1) |> int)
    |> Seq.groupBy id
    |> Seq.map (fun (k, v) -> k, Seq.length v)
    |> Seq.sortBy fst

fun () ->
    Chart
        .Line(data, Name="Digit count")
        .WithXAxis(Title="Digit")
        .WithYAxis(Title="Count")

|> Chart.Show(
    title="Benford Law",
    modal=fsi.CommandLineArgs.[0].EndsWith(".fsx")
)
