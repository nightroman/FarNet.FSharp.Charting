// Mix or loop and live: LiveChart.LineIncremental, Chart.Combine, Chart.Show (loop)
// We are watching evolution of N processes. Every M seconds one process dies
// and its chart is replaced with a new process chart.

open FSharp.Charting

let count = 5
let size = 10.0
let step = 0.33

let rand = System.Random()
let initY () = rand.NextDouble() * size
let moveY y = y + step * (rand.NextDouble() - 0.5)

let mutable charts = None
fun () ->
    // make a new process chart
    let chart index =
        let mutable y = initY ()
        let data =
            Timer.mapi 100 (fun i ->
                y <- moveY y
                i, y
            )
        LiveChart.LineIncremental(data, Name=(string index))

    // make all charts once or replace one random with new
    match charts with
    | None ->
        charts <- Some (Array.init count chart)
    | Some charts ->
        let index = rand.Next count
        charts.[index] <- chart index

    // combine charts
    Chart.Combine(charts.Value)
    |> Chart.WithYAxis(Min=0.0, Max=size)
    |> Chart.WithLegend(Docking=ChartTypes.Docking.Left)

|> Chart.Show(
    loop=5000,
    title=sprintf "%i processes" count,
    modal=fsi.CommandLineArgs.[0].EndsWith(".fsx")
)
