// Chart.Line example with "foxes and rabbits" dynamic system trajectory.
// http://fsharpnews.blogspot.com/2010/07/f-vs-mathematica-parametric-plots.html

open FSharp.Charting

let trajectory g k t =
    let evolve (r, f) =
        let dtrf = 0.0001 * r * f
        r + (1.0 - r / k) * r * g - dtrf, dtrf + (1.0 - g) * f
    List.scan (fun s _ -> evolve s) (50.0, 10.0) [ 1 .. t ]

fun () ->
    trajectory 0.02 5e2 1500
    |> Chart.Line
    |> Chart.WithXAxis(Title="Rabbits", Min=0.0)
    |> Chart.WithYAxis(Title="Foxes")

|> Chart.Show(
    title="Foxes and Rabbits",
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
