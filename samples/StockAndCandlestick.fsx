// Example of Chart.Stock and Chart.Candlestick
// with modal only Chart.Show (FSharp.Charting)

open FSharp.Charting
open System

let prices =
  [ 26.24,25.80,26.22,25.95; 26.40,26.18,26.26,26.20
    26.37,26.04,26.11,26.08; 26.78,26.15,26.60,26.16
    26.86,26.51,26.69,26.58; 26.95,26.50,26.91,26.55
    27.06,26.50,26.64,26.77; 26.86,26.43,26.53,26.59
    27.10,26.52,26.78,26.59; 27.21,26.99,27.13,27.06
    27.37,26.91,26.97,27.21; 27.07,26.60,27.05,27.02
    27.33,26.95,27.04,26.96; 27.27,26.95,27.21,27.23
    27.81,27.07,27.76,27.25; 27.94,27.29,27.93,27.50
    28.26,27.91,28.19,27.97; 28.34,28.05,28.10,28.28
    28.34,27.79,27.80,28.20; 27.84,27.51,27.70,27.77 ]

// Chart 1, just stock prices sequence.

prices
|> Chart.Stock
|> Chart.Show

// Chart 2, stock prices with dates as candlesticks.

prices
|> List.mapi (fun i (hi, lo, op, cl) ->
    (DateTime.Today.AddDays(float i).ToShortDateString(), hi, lo, op, cl)
)
|> Chart.Candlestick
|> Chart.WithYAxis(Max=29.0, Min=25.0)
|> Chart.Show
