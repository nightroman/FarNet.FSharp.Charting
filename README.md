[NuGet]: https://www.nuget.org/packages/FarNet.FSharp.Charting
[GitHub]: https://github.com/nightroman/FarNet.FSharp.Charting
[/samples]: https://github.com/nightroman/FarNet.FSharp.Charting/tree/master/samples

# FarNet.FSharp.Charting

FarNet friendly FSharp.Charting extension

- [FSharp.Charting](https://fslab.org/FSharp.Charting/index.html)
- [FarNet.FSharpFar](https://github.com/nightroman/FarNet/tree/master/FSharpFar)

## Package

The NuGet package [FarNet.FSharp.Charting][NuGet] may be used as usual in F# projects.

The package is also designed for [FarNet.FSharpFar](https://github.com/nightroman/FarNet/tree/master/FSharpFar).
To install FarNet packages, follow [these steps](https://raw.githubusercontent.com/nightroman/FarNet/master/Install-FarNet.en.txt).

## Features

*FarNet.FSharp.Charting* does not require FarNet or Far Manager.
The library may be used in usual F# projects for its features:

- The library may be used in MTA thread environments.
- Optional timer loop, often easier than live charts.
- Chart windows are modeless by default.
- Chart windows provide more options.
- Some helpers for live charts.

But the library is specifically designed for FarNet F# modules and FSharpFar F#
scripts. In this environment apart from adding new features the library works
around FSharp.Charting STA requirements.

## How to use

Instead of the original `Chart.Show` taking a chart instance, use the new
method `Chart.Show` which takes a chart making function and other useful
parameters.

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

- It is modeless by default. The original is modal, always.
- It works well in Far Manager, unlike the original.
- It adds handy features using extra parameters.
- It may be used in MTA threads.

Note that the original `Chart.Show` partially works in FarNet. It shows charts.
So you may use it for easier prototyping. But some features may not work, for
example context menu commands "copy to clipboard", "save as", etc.

## "Loop" and "Live" charts

There are two ways of showing charts with dynamic content:
new loop charts and the original live charts.

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

Why do we need live charts then? There are at least three reasons:

- Live charts may be more effective because only data change on updates.
- Live charts provide useful incremental charts right out of the box.
- Live charts are easier to code when data come from events.

The last note. If you accidentally (!) use live charts with loop shows, i.e.
`LiveChart.X |> Chart.Show(loop=...)` then the result may be unexpected.
But this is allowed and in some cases may be used very effectively.
See *LoopLiveCharts* in samples.

## Timers for live charts

For generating live chart events you may use `System.Windows.Forms.Timer` or
its handy helpers `Timer.map` and `Timer.mapi`. Important:

- Timers and mappings must be used inside chart making functions.
- Do not use other timer classes, e.g. `System.Timers.Timer`.

## Using with FSharpFar

In the script directory create the configuration `*.fs.ini`:

```ini
[use]
%FARHOME%\FarNet\Lib\FarNet.FSharp.Charting\FarNet.FSharp.Charting.ini
```

This is it. Scripts normally contain `open FSharp.Charting`.
This "opens" both `FSharp.Charting` types and `FarNet.FSharp.Charting` extensions.

See [/samples]. They are ready to use examples of various scenarios and
features, focusing on added by *FarNet.FSharp.Charting*.

## Notes

Features and API may change before v1.0.

The project is suitable for cloning and building with dotnet.

*FSharpFar* development and tools are optional.
They require:

- *Far Manager* in `C:\Bin\Far\x64`
- FarNet module *FarNet.FSharpFar*
- *Invoke-Build* with *.build.ps1*
