<#
.Synopsis
	Build script, https://github.com/nightroman/Invoke-Build
#>

param(
	$Configuration = (property Configuration Release),
	$FarHome = (property FarHome C:\Bin\Far\x64)
)

Set-StrictMode -Version 2
$ModuleName = 'FarNet.FSharp.Charting'
$ModuleRoot = "$FarHome\FarNet\Lib\$ModuleName"
$Description = 'FarNet friendly FSharp.Charting extension.'

# Synopsis: Remove temp files.
task clean {
	remove src\bin, src\obj, README.htm, *.nupkg, z
}

# Synopsis: Build and Post (post build target).
task build meta, {
	Set-Location src
	exec {dotnet build -c $Configuration}
}

# Synopsis: Post build event.
task publish {
	exec { dotnet publish src\FarNet.FSharp.Charting.fsproj -c $Configuration -o $ModuleRoot --no-build }
	Remove-Item "$ModuleRoot\FarNet.FSharp.Charting.deps.json"
	Copy-Item "src\$ModuleName.ini" $ModuleRoot

	Set-Location $ModuleRoot
	remove FSharp.Core.dll, cs, de, en, es, fr, it, ja, ko, pl, pt-BR, ru, tr, zh-Hans, zh-Hant

	Set-Location runtimes
	remove unix, win-arm64
}

# Get version from release notes.
function Get-Version {
	switch -Regex -File Release-Notes.md {'##\s+v(\d+\.\d+\.\d+)' {return $Matches[1]} }
}

# Synopsis: Set $script:Version.
task version {
	($script:Version = Get-Version)
}

# Synopsis: Convert markdown to HTML.
task markdown {
	assert (Test-Path $env:MarkdownCss)
	exec { pandoc.exe @(
		'README.md'
		'--output=README.htm'
		'--from=gfm'
		'--embed-resources'
		'--standalone'
		"--css=$env:MarkdownCss"
		"--metadata=pagetitle=$ModuleName"
	)}
}

# Synopsis: Generate meta files.
task meta -Inputs .build.ps1, Release-Notes.md -Outputs src/Directory.Build.props -Jobs version, {
	Set-Content src/Directory.Build.props @"
<Project>
	<PropertyGroup>
		<Company>https://github.com/nightroman/$ModuleName</Company>
		<Copyright>Copyright (c) Roman Kuzmin</Copyright>
		<Description>$Description</Description>
		<Product>$ModuleName</Product>
		<Version>$Version</Version>
		<FileVersion>$Version</FileVersion>
		<AssemblyVersion>$Version</AssemblyVersion>
	</PropertyGroup>
</Project>
"@
}

# Synopsis: Collect package files.
task package markdown, {
	remove z
	$toModule = mkdir "z\tools\FarHome\FarNet\Lib\$ModuleName"

	exec { robocopy $ModuleRoot $toModule /s /xf *.pdb } 1

	Copy-Item -Destination z @(
		'README.md'
	)

	Copy-Item -Destination $toModule @(
		"README.htm"
		"LICENSE"
	)

	$result = Get-ChildItem $toModule -Recurse -File -Name | Out-String
	$sample = @'
FarNet.FSharp.Charting.dll
FarNet.FSharp.Charting.ini
FarNet.FSharp.Charting.xml
LICENSE
README.htm
System.Data.OleDb.dll
System.Data.SqlClient.dll
System.Windows.Forms.DataVisualization.dll
runtimes\win\lib\net8.0\System.Data.SqlClient.dll
runtimes\win\lib\net9.0\System.Data.OleDb.dll
runtimes\win-x64\native\sni.dll
runtimes\win-x86\native\sni.dll
'@
	Assert-SameFile.ps1 -Text $sample $result $env:MERGE
}

# Synopsis: Make NuGet package.
task nuget package, version, {
	$dllPath = "$FarHome\FarNet\Lib\$ModuleName\$ModuleName.dll"
	($dllVersion = (Get-Item $dllPath).VersionInfo.FileVersion.ToString())
	assert $dllVersion.StartsWith("$Version.") 'Versions mismatch.'

	Set-Content z\Package.nuspec @"
<?xml version="1.0"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>$ModuleName</id>
		<version>$Version</version>
		<authors>Roman Kuzmin</authors>
		<owners>Roman Kuzmin</owners>
		<license type="expression">Apache-2.0</license>
		<readme>README.md</readme>
		<projectUrl>https://github.com/nightroman/$ModuleName</projectUrl>
		<description>$Description</description>
		<releaseNotes>https://github.com/nightroman/FarNet.FSharp.Charting/blob/main/Release-Notes.md</releaseNotes>
		<tags>FarManager FarNet FSharp Charting</tags>
	</metadata>
</package>
"@

	exec { NuGet.exe pack z\Package.nuspec }
}

task test {
	Invoke-Build ** test
}

task . build, clean
