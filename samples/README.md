# FarNet.FSharp.Charting samples

## How to run

Scripts in this directory are run by `FSharpFar` or its `fsx.exe` <sup>(1)</sup>.
The configuration [.fs.ini](.fs.ini) provides the required references.

<sup>(1)</sup> Scripts `Far*.fsx` use FarNet and cannot be run by `fsx.exe`.

## Modal or modeless

Most of scripts are designed to show charts modal or modeless depending on the environment.
In interactives they are modeless, charts do not block the UI.
You may keep working and open many charts simultaneously.

On running scripts by `fsx` charts are shown modal.
Otherwise the runner simply exits immediately.

Here is a simple modal or modeless trick:

```fsharp
...
|> Chart.Show(
    modal=fsi.CommandLineArgs[0].EndsWith(".fsx")
)
```
