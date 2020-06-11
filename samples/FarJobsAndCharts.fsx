// This script shows how to program a flow with non blocking charts and dialogs
// between their shows. You can work in Far when charts are shown. On closing
// charts the flow resumes and shows input boxes for chart parameters.

open FSharp.Charting
open FarNet
open FarNet.FSharp

async {
    let mutable factor = 1.0
    let mutable loop = true

    // show charts with changing factor
    while loop do
        // switch thread to avoid blocking Far
        do! Async.SwitchToThreadPool()

        // show modal chart (modal here but not blocking Far)
        fun () ->
            Seq.init 100 (fun i -> sin (0.1 * float i * factor))
            |> Chart.Line
        |> Chart.Show(modal=true, location=(0, 0))

        // Far job: input box for a new factor
        match! job {return far.Input ("Try different factor", null, "Chart", string factor)} with
        | null ->
            // stop
            loop <- false
        | text ->
            // continue with the new factor
            try factor <- float text with _ -> ()
}
|> Async.StartImmediate
