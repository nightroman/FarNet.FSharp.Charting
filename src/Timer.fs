
// FarNet.FSharp.Charting
// Copyright (c) Roman Kuzmin

[<RequireQualifiedAccess>]
module FSharp.Charting.Timer
open System.Windows.Forms

/// Maps timer events to the specified function, normally getting data for LiveChart.
/// Mapping should be called in the same thread that creates the chart.
/// // interval: Timer interval in milliseconds.
/// // func: Function getting the data.
let map interval (func: unit -> _) =
    let timer = new Timer(Interval=1)
    let res =
        timer.Tick
        |> Event.choose (fun _ ->
            timer.Interval <- interval
            try
                Some (func ())
            with _ ->
                None
        )
    timer.Start()
    res

/// Maps timer event indexes to the specified function, normally getting data for LiveChart.
/// Mapping should be called in the same thread that creates the chart.
/// // interval: Timer interval in milliseconds.
/// // func: Function getting the data.
let mapi interval (func: int -> _) =
    let timer = new Timer(Interval=1)
    let mutable index = -1
    let res =
        timer.Tick
        |> Event.choose (fun _ ->
            timer.Interval <- interval
            index <- index + 1
            try
                Some (func index)
            with _ ->
                None
        )
    timer.Start()
    res
