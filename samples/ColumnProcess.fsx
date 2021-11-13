// Column chart. It shows top 20 memory consuming processes.
// See ColumnProcessLoop for the dynamically updated chart.

open FSharp.Charting
open System.Diagnostics

let data =
    Process.GetProcesses()
    |> Seq.map (fun x -> x.ProcessName.Substring(0, min 20 x.ProcessName.Length), x.WorkingSet64 / 1048576L)
    |> Seq.sortByDescending snd
    |> Seq.take 20

fun () ->
    Chart.Column(data, Name="Process", Title="WorkingSet (mb)")
    |> Chart.WithXAxis(LabelStyle=ChartTypes.LabelStyle(-45, Interval=1.0))

|> Chart.Show(
    title="Processes",
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
