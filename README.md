[NuGet]: https://www.nuget.org/packages/FarNet.FSharp.Charting
[GitHub]: https://github.com/nightroman/FarNet.FSharp.Charting
[/samples]: https://github.com/nightroman/FarNet.FSharp.Charting/tree/master/samples
[FSharp.Charting]: https://fslab.org/FSharp.Charting/index.html

# FarNet.FSharp.Charting

FarNet friendly [FSharp.Charting] extension

## Package

The NuGet package [FarNet.FSharp.Charting][NuGet] may be used as usual in F# projects.

The package is also designed for [FarNet.FSharpFar](https://github.com/nightroman/FarNet/tree/master/FSharpFar).
To install FarNet packages, follow [these steps](https://github.com/nightroman/FarNet#readme).

## Features

In addition to the original library features:

- The library may be called from MTA threads.
- Loop charts, often easier than live charts.
- Chart windows are modeless by default.
- Chart windows provide more options.
- Timer helpers for live charts.

## How to use

Instead of the original `Chart.Show` with a chart instance, use the new
method `Chart.Show` with a function which creates and configures a chart.

Example, instead of the original method:

```fsharp
<data>
|> Chart.Line
|> Chart.Show
```

use this new:

```fsharp
fun () ->
    <data>
    |> Chart.Line
|> Chart.Show(title="My chart", ...)
```

The new method is slightly more verbose but it has some advantages:

- It is modeless by default, the original is always modal.
- It works well in Far Manager, unlike the original.
- It provides handy features via parameters.
- It may be called from MTA threads.

The original `Chart.Show` is still available and partially works in FarNet. It
shows charts. But some features may not work, for example context menu commands
"copy to clipboard", "save as", etc.

## "Loop" and "Live" charts

There are two ways of showing charts with dynamic content:
new loop charts and original live charts.

Loop charts are very easy, just specify the required timer interval as the
parameter `loop`:

```fsharp
fun () ->
    <data>
    |> Chart.X
|> Chart.Show(loop=2000)
```

Live charts are slightly more difficult, they require input data as special
streams of events or observables. If data do not naturally come from events
then some conversion is needed. See the original manual for the details.

Unlike loop charts, live charts cannot change chart parameters dynamically.

Live charts are still useful:

- Live charts may be more effective because only data change on updates.
- Live charts provide useful incremental charts right out of the box.
- Live charts are easier to code when data come from events.

If you accidentally (!) use live charts with loop shows then the result
may be unexpected. But this is allowed and in some cases used effectively.
See [LoopLiveCharts.fsx](https://github.com/nightroman/FarNet.FSharp.Charting/blob/master/samples/LoopLiveCharts.fsx)

## Timers for live charts

For generating live chart events you may use `System.Windows.Forms.Timer` or
its handy helpers `Timer.map` and `Timer.mapi`. Important:

- Timers and mappings must be created in chart making functions.
- Do not use other timer classes like `System.Timers.Timer`.

## Using with FSharpFar

In your script directory create the configuration `*.fs.ini`:

```ini
[use]
%FARHOME%\FarNet\Lib\FarNet.FSharp.Charting\FarNet.FSharp.Charting.ini
```

This is it. See [/samples].

## Notes

The project is suitable for cloning and building with dotnet.

*FSharpFar* development and tools are optional. They require:

- *Far Manager* in `C:\Bin\Far\x64`
- FarNet module *FarNet.FSharpFar*
- *Invoke-Build* with *.build.ps1*
