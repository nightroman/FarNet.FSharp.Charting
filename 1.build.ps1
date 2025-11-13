<#
.Synopsis
	Build script, https://github.com/nightroman/Invoke-Build
#>

param(
	$Configuration = (property Configuration Release),
	$FarHome = (property FarHome C:\Bin\Far\x64)
)

Set-StrictMode -Version 3
$_name = 'FarNet.FSharp.Charting'
$_root = "$FarHome\FarNet\Lib\$_name"
$_description = 'FarNet friendly FSharp.Charting extension.'

# Synopsis: Remove temp files.
task clean {
	remove src\bin, src\obj, README.htm, *.nupkg, z
}

# Synopsis: Build and Post (post build target).
task build meta, {
	Set-Location src
	exec { dotnet build -c $Configuration -tl:off }
}

# Synopsis: Post build event.
task publish {
	exec { dotnet publish src\FarNet.FSharp.Charting.fsproj -c $Configuration -o $_root --no-build }
	Remove-Item "$_root\FarNet.FSharp.Charting.deps.json"
	Copy-Item "src\$_name.ini" $_root

	Set-Location $_root
	remove FSharp.Core.dll

	Set-Location runtimes
	remove unix, win-arm64, win-x86
}

# Synopsis: Set $Script:Version.
task version {
	($Script:_version = Get-BuildVersion Release-Notes.md '##\s+v(\d+\.\d+\.\d+)')
}

# Synopsis: Convert markdown to HTML.
task markdown {
	requires -Path $env:MarkdownCss
	exec { pandoc.exe @(
		'README.md'
		'--output=README.htm'
		'--from=gfm'
		'--embed-resources'
		'--standalone'
		"--css=$env:MarkdownCss"
		"--metadata=pagetitle=$_name"
	)}
}

# Synopsis: Generate meta files.
task meta -Inputs 1.build.ps1, Release-Notes.md -Outputs src/Directory.Build.props -Jobs version, {
	Set-Content src/Directory.Build.props @"
<Project>
	<PropertyGroup>
		<Company>https://github.com/nightroman/$_name</Company>
		<Copyright>Copyright (c) Roman Kuzmin</Copyright>
		<Description>$_description</Description>
		<Product>$_name</Product>
		<Version>$_version</Version>
		<FileVersion>$_version</FileVersion>
		<AssemblyVersion>$_version</AssemblyVersion>
	</PropertyGroup>
</Project>
"@
}

# Synopsis: Collect package files.
task package markdown, {
	remove z
	$toModule = mkdir "z\tools\FarHome\FarNet\Lib\$_name"

	exec { robocopy $_root $toModule /s } 1

	Copy-Item -Destination z @(
		'README.md'
	)

	Copy-Item -Destination $toModule @(
		"README.htm"
		"LICENSE"
	)

	Assert-SameFile.ps1 -Result (Get-ChildItem $toModule -Recurse -File -Name) -Text -View $env:MERGE -Sample @'
FarNet.FSharp.Charting.dll
FarNet.FSharp.Charting.ini
FarNet.FSharp.Charting.pdb
FarNet.FSharp.Charting.xml
LICENSE
README.htm
System.Data.OleDb.dll
System.Data.SqlClient.dll
System.Windows.Forms.DataVisualization.dll
runtimes\win\lib\net10.0\System.Data.OleDb.dll
runtimes\win\lib\net8.0\System.Data.SqlClient.dll
runtimes\win-x64\native\sni.dll
'@
}

# Synopsis: Make NuGet package.
task nuget package, version, {
	$dllPath = "$FarHome\FarNet\Lib\$_name\$_name.dll"
	($dllVersion = (Get-Item $dllPath).VersionInfo.FileVersion.ToString())
	assert $dllVersion.StartsWith("$_version.") 'Versions mismatch.'

	Set-Content z\Package.nuspec @"
<?xml version="1.0"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>$_name</id>
		<version>$_version</version>
		<authors>Roman Kuzmin</authors>
		<owners>Roman Kuzmin</owners>
		<license type="expression">Apache-2.0</license>
		<readme>README.md</readme>
		<projectUrl>https://github.com/nightroman/$_name</projectUrl>
		<description>$_description</description>
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
