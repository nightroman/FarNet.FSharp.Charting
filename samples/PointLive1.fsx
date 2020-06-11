// LiveChart.Point example with randomly moving points.
// This case does not have to use LiveChart, see PointLoop.

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
    let data =
        Timer.map 50 (fun () ->
            points |> Array.iteri (fun i (x, y) -> points.[i] <- move x, move y)
            points
        )

    LiveChart.Point(data, Name="Points")
    |> Chart.WithXAxis(Min=0.0, Max=size)
    |> Chart.WithYAxis(Min=0.0, Max=size)

|> Chart.Show(
    size=(500, 500),
    modal=fsi.CommandLineArgs.[0].EndsWith(".fsx")
)
