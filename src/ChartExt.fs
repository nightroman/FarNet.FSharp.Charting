
// FarNet.FSharp.Charting
// Copyright (c) Roman Kuzmin

[<AutoOpen>]
module FSharp.Charting.ChartExt
open FSharp.Charting
open System
open System.Drawing
open System.Threading
open System.Windows.Forms

type Chart with
    /// Creates the chart by the specified function and shows the chart form in a special thread.
    /// // chart: Function making the chart.
    /// // title: Specifies the window title.
    /// // modal: Tells to wait for closing.
    /// // size: Sets the form width and height.
    /// // location: Sets the form width and height.
    /// // topMost: Tells to show on top of other windows.
    /// // loop: Tells to start the timer loop with the specified interval.
    static member Show
        (
            chart: unit -> #ChartTypes.GenericChart,
            ?title,
            ?modal,
            ?size,
            ?location,
            ?topMost,
            ?loop
        ) =
        let title = defaultArg title "Chart"
        let modal = defaultArg modal false
        let width, height = defaultArg size (800, 600)
        let topMost = defaultArg topMost false
        let loop = defaultArg loop 0

        let worker () =
            let mutable cc0: IDisposable = null
            use form = new Form ()
            form.Text <- title
            form.TopMost <- topMost
            form.KeyPreview <- true
            form.Size <- Size (width, height)
            match location with
            | Some (x, y) ->
                form.StartPosition <- FormStartPosition.Manual
                form.Location <- Point (x, y)
            | None -> ()

            let mutable timer = null
            form.Load.Add (fun _ ->
                let cc1 = new ChartTypes.ChartControl (chart (), Dock=DockStyle.Fill)
                cc0 <- cc1
                form.Controls.Add cc1
                form.Activate ()

                // timer setup
                if loop > 0 then
                    timer <- new Timer (Interval=loop)
                    timer.Tick.Add (fun _ ->
                        // add the new chart first to avoid flickering
                        let cc2 = new ChartTypes.ChartControl (chart (), Dock=DockStyle.Fill)
                        form.Controls.Add cc2

                        // then remove the old chart
                        form.Controls.Remove cc1
                        cc0.Dispose ()
                        cc0 <- cc2
                    )
                    timer.Start ()
            )

            form.KeyDown.Add (fun e ->
                if e.KeyCode = Keys.Escape then
                    form.Close ()
            )

            form.ShowDialog () |> ignore
            if not (isNull timer) then
                timer.Dispose ()
            cc0.Dispose ()

        let thread = Thread worker
        thread.Name <- sprintf "UI thread for '%s'" title
        thread.SetApartmentState ApartmentState.STA
        thread.IsBackground <- true
        thread.Start ()
        if modal then
            thread.Join ()

    /// Pipeline friendly version of Show() with parameters:
    /// // fun () -> ... |> Show (title = "Chart", ...)
    /// // Show (title = "Chart", ...) <| fun () -> ...
    /// // Show (title = "Chart", ...) (fun () -> ...)
    static member Show (?title, ?modal, ?size, ?location, ?topMost, ?loop) =
        fun (chart: unit -> #ChartTypes.GenericChart) ->
            Chart.Show (
                chart,
                ?title=title,
                ?modal=modal,
                ?size=size,
                ?location=location,
                ?topMost=topMost,
                ?loop=loop
            )
