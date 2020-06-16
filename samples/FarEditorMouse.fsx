// The ideal use case for live charts: data come from events.
// This example visualizes mouse moves in an editor.
// Also, it opens two charts at the same time.
// How to use:
// - run the script, it shows two charts
// - switch to a Far Manager editor
// - move the mouse, see the charts

open FSharp.Charting
open FarNet

// Transform mouse move events to (x, y) observable.
let editorMouseXY =
    far.AnyEditor.MouseMove
    |> Observable.map (fun e -> e.Mouse.Where.X, - e.Mouse.Where.Y)

// Chart 1 shows all mouse moves using LiveChart.LineIncremental
// and feeding one pair of mouse coordinate at a time.

fun () ->
    editorMouseXY
    |> LiveChart.LineIncremental

|> Chart.Show(
    title="All mouse moves",
    area=(3, 2, 0, 0)
)

// Chart 2 shows last 50 mouse moves. It uses the usual LiveChart.Line,
// so we maintain and feed the queue of last coordinates ourselves.

fun () ->
    let queue = System.Collections.Generic.Queue ()

    editorMouseXY
    |> Observable.map (fun xy ->
        queue.Enqueue xy
        if queue.Count > 50 then
            queue.Dequeue () |> ignore
        queue
    )
    |> LiveChart.Line

|> Chart.Show(
    title="Last 50 moves",
    area=(3, 2, 0, 1)
)
