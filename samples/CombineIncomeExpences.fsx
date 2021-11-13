// Combining several charts together and using the legend.

open FSharp.Charting
open System

let rand = Random().NextDouble
let futureDate days = DateTime.Today.AddDays(float days)

let expectedIncome =
  [ for x in 1 .. 100 ->
      futureDate x, 1000.0 + rand() * 100.0 * exp (float x / 40.0) ]

let expectedExpenses =
  [ for x in 1 .. 100 ->
      futureDate x, rand() * 500.0 * sin (float x / 50.0) ]

let computedProfit =
  (expectedIncome, expectedExpenses)
  ||> List.map2 (fun (d1, i) (d2, e) -> (d1, i - e))

fun () ->
    Chart.Combine [
        Chart.Line(expectedIncome, Name="Income")
        Chart.Line(expectedExpenses, Name="Expenses")
        Chart.Line(computedProfit, Name="Profit")
    ]
    |> Chart.WithLegend(Docking=ChartTypes.Docking.Left)

|> Chart.Show(
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
