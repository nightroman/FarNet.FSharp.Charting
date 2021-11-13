// LiveChart.LineIncremental
// Live charts are ideal for data coming from events. But what if we do not
// have events as such? We may use System.Windows.Forms.Timer or its handy
// FarNet.FSharp.Charting helpers Timer.map|mapi, like in this example.

open FSharp.Charting

fun () ->
    Timer.mapi 50 (fun i ->
        i, System.DateTime.Now.Second
    )
    |> LiveChart.LineIncremental

|> Chart.Show(
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
