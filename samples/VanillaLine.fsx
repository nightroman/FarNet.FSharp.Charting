// Example of low level charting without FSharp.Charting.
// This way gives full control but it is rather tedious.

open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting

// data
let points = [| for x in 1 .. 100 -> sin (float x / 10.0) |]

// chart
let chart = new Chart()
chart.Dock <- DockStyle.Fill

// area
let area = new ChartArea()
area.Name <- "Area1"
chart.ChartAreas.Add(area)

// series
let series = new Series()
series.ChartArea <- "Area1"
series.ChartType <- SeriesChartType.Line
points |> Array.iter (series.Points.Add >> ignore)
chart.Series.Add(series)

// form
let form = new Form()
form.Text <- "sin"
form.Controls.Add(chart)
form.ShowDialog() |> ignore
