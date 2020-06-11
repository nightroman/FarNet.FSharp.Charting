Scripts in this directory are ready to run by `FarNet.FSharpFar` or its `fsx.exe` <sup>(1)</sup>.
The configuration [.fs.ini](.fs.ini) provides the required references.

<sup>(1)</sup> Scripts `Far*.fsx` use FarNet and cannot be run by `fsx.exe`.

For running by `fsi.exe` scripts need manually added `#r` directives like

```fsharp
#r "System.Windows.Forms"
#r "System.Windows.Forms.DataVisualization"
#r @"C:\Bin\Far\x64\FarNet\Lib\FarNet.FSharp.Charting\FSharp.Charting.dll"
#r @"C:\Bin\Far\x64\FarNet\Lib\FarNet.FSharp.Charting\FarNet.FSharp.Charting.dll"
```

**Modal or modeless**

Most of the scripts are designed to show charts modal or modeless depending on the environment.
In interactives they are modeless, charts do not block the UI.
You may keep working and open many charts simultaneously.

But on running scripts by `fsx` or `fsi` charts are shown modal.
Otherwise the runner exits immediately and charts are not shown.

Here is a simple modal or modeless trick:

```fsharp
...
|> Chart.Show(
    modal=fsi.CommandLineArgs.[0].EndsWith(".fsx")
)
```
