// Chart.Point example with a loop and randomly moving points.
// This case is slightly simpler than live chart in PointLive1.

open FSharp.Charting

// parameters: point count, square size, step max size
let count = 1000
let size = 10.0
let step = 0.05

// generate points with initial random positions
let random = System.Random().NextDouble
let move x = x + random () * step * 2.0 - step
let points = Array.init count (fun _ -> random () * size, random () * size)

fun () ->
    // move each point
    points |> Array.iteri (fun i (x, y) -> points.[i] <- move x, move y)

    // create the chart
    Chart.Point(points, Name="Points")
    |> Chart.WithXAxis(Min=0.0, Max=size)
    |> Chart.WithYAxis(Min=0.0, Max=size)

|> Chart.Show(
    title="Random moves", loop=50, size=(500, 500),
    modal=fsi.CommandLineArgs.[0].EndsWith(".fsx")
)
