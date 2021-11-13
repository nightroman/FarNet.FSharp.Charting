// Example of a loop chart, similar to ColumnProcess but with updates.
// It shows top 20 memory consuming processes and updates every 2 seconds.
// Also, with topMost=true it is always on top (still may be minimized).

// Note that we update the chart title as well (current time). This would not
// be possible if we choose LiveChart instead of the loop. Live charts are
// created once with fixed parameters, then only data change.

open FSharp.Charting
open System.Diagnostics

fun () ->
    let data =
        Process.GetProcesses()
        |> Seq.map (fun x -> x.ProcessName.Substring(0, min 20 x.ProcessName.Length), x.WorkingSet64 / 1048576L)
        |> Seq.sortByDescending snd
        |> Seq.take 20

    Chart
        .Column(data, Name="Process", Title=($"WorkingSet (mb) {System.DateTime.Now}"))
        .WithXAxis(LabelStyle=ChartTypes.LabelStyle(-45, Interval=1.0))

|> Chart.Show(
    title="Processes", topMost=true, loop=2000,
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
