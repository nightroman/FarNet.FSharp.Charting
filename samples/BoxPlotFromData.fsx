// Example of BoxPlotFromData combined with data points.
// https://fslab.org/FSharp.Charting/BoxPlotCharts.html

open FSharp.Charting
open System.Drawing

let rnd = new System.Random()

let dataSets = [
    ("data 1", [ for i in 0 .. 20 -> float (rnd.Next 20) ])
    ("data 2", [ for i in 0 .. 20 -> float (rnd.Next 15 + 2) ])
    ("data 3", [ for i in 0 .. 20 -> float (rnd.Next 10 + 5) ])
]

let points =
    dataSets
    |> List.mapi (fun i (_, data) -> {| X = i + 1; Y = data |})
    |> List.collect (fun data ->
        data.Y
        |> List.map (fun y -> (data.X, y))
    )

let chartBotPlot = Chart.BoxPlotFromData(dataSets)
let chartPoint = Chart.Point(points, Color=Color.Black)

fun () ->
    [ chartBotPlot; chartPoint ]
    |> Chart.Combine
    |> Chart.WithXAxis(Min=0.0, Max=4.0)

|> Chart.Show
