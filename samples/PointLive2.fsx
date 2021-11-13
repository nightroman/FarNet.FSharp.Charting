// LiveChart.FastPointIncremental example with randomly moving points.
// Note that this case is much easier with LiveChart than Show(loop=...).

open FSharp.Charting

// parameters: point count, square size, step max size
let count = 9
let size = 10.0
let step = 0.11

// generate points with initial random positions
let random = System.Random().NextDouble
let move x = x + random () * step * 2.0 - step
let move2 (x, y) = move x, move y
let points = Array.init count (fun _ -> random () * size, random () * size)

fun () ->
    Array.init count (fun i ->
        Timer.map 50 (fun () ->
            points[i] <- move2 points[i]
            points[i]
        )
        |> LiveChart.FastPointIncremental
    )
    |> Chart.Combine
    |> Chart.WithXAxis(Min=0.0, Max=size)
    |> Chart.WithYAxis(Min=0.0, Max=size)

|> Chart.Show(
    size=(500, 500),
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
