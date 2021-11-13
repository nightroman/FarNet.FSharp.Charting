// Example of Chart.Histogram with normally distributed random numbers.

open FSharp.Charting
open System

/// Generator of uniform random numbers.
let random = Random()

/// Generator of approximately normal random numbers.
// https://en.wikipedia.org/wiki/Normal_distribution
let approximateRandomNormal () =
    let mutable sum = 0.0
    for _ in 1 .. 12 do
        sum <- sum + random.NextDouble()
    sum - 6.0

// generate data, show the histogram, normally bell shaped
fun () ->
    Seq.init 10000 (fun _ -> approximateRandomNormal ())
    |> Chart.Histogram
    |> Chart.WithXAxis(LabelStyle=ChartTypes.LabelStyle(Format="F0"))

|> Chart.Show(
    title="Random normal samples",
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
