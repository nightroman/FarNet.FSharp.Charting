
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
    /// Creates the chart by the specified function and shows its window in a special thread.
    /// // chart: The chart creating function.
    /// // title: Sets the chart window title.
    /// // modal: Tells to wait for closing.
    /// // loop: Tells to recreate the chart in a loop and sets the interval in milliseconds.
    /// // area: Sets the window screen area as (nCol, nRow, iCol, iRow).
    /// // size: Sets the window size as (width, height).
    /// // location: Sets the window location as (x, y).
    /// // topMost: Tells to show on top of other windows.
    static member Show
        (
            chart: unit -> #ChartTypes.GenericChart,
            ?title,
            ?modal,
            ?loop,
            ?area,
            ?size,
            ?location,
            ?topMost
        ) =
        let title = defaultArg title "Chart"
        let modal = defaultArg modal false
        let loop = defaultArg loop 0
        let topMost = defaultArg topMost false

        match area with
        | Some (nX, nY, iX, iY) ->
            if nX < 1 || iX < 0 || iX >= nX || nY < 1 || iY < 0 || iY >= nY then
                invalidArg "area" "Invalid parameter 'area'."
        | None ->
            ()

        let worker () =
            let mutable cc0: IDisposable = null
            use form = new Form ()
            form.Text <- title
            form.TopMost <- topMost
            form.KeyPreview <- true

            match area with
            | Some (nX, nY, iX, iY) ->
                let area = Screen.PrimaryScreen.WorkingArea
                let width = area.Width / nX
                let height = area.Height / nY
                form.StartPosition <- FormStartPosition.Manual
                form.Size <- Size (width, height)
                form.Location <- Point (iX * width, iY * height)
            | None ->
                let width, height = defaultArg size (800, 600)
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
    static member Show (?title, ?modal, ?loop, ?area, ?size, ?location, ?topMost) =
        fun (chart: unit -> #ChartTypes.GenericChart) ->
            Chart.Show (
                chart,
                ?title=title,
                ?modal=modal,
                ?loop=loop,
                ?area=area,
                ?size=size,
                ?location=location,
                ?topMost=topMost
            )
