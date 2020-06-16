<#
.Synopsis
	Build script (https://github.com/nightroman/Invoke-Build)
#>

param(
	$Configuration = (property Configuration Release)
)

Set-StrictMode -Version 2
$ModuleName = 'FarNet.FSharp.Charting'
$env:FarDevHome = $FarDevHome = if (Test-Path 'C:\Bin\Far\x64') {'C:\Bin\Far\x64'} else {''}

# Synopsis: Remove temp files.
task Clean {
	remove src\bin, src\obj, README.htm, *.nupkg, z
}

# Synopsis: Build and Post (post build target).
task Build {
	Set-Location src
	exec {dotnet build -c $Configuration}
}

# Synopsis: Post build target. Copy stuff.
task Post -If:$FarDevHome {
	$to = "$FarDevHome\FarNet\Lib\$ModuleName"
	Copy-Item "src\$ModuleName.ini" $to

	$xml = [xml](Get-Content "src\$ModuleName.fsproj")
	$node = $xml.SelectSingleNode('Project/ItemGroup/PackageReference[@Include="FSharp.Charting"]')
	Copy-Item "$env:USERPROFILE\.nuget\packages\FSharp.Charting\$($node.Version)\lib\net45\FSharp.Charting.xml" $to
}

# Get version from release notes.
function Get-Version {
	switch -Regex -File Release-Notes.md {'##\s+v(\d+\.\d+\.\d+)' {return $Matches[1]} }
}

# Synopsis: Set $script:Version.
task Version {
	($script:Version = Get-Version)
}

# Synopsis: Convert markdown to HTML.
task Markdown {
	assert (Test-Path $env:MarkdownCss)
	exec { pandoc.exe @(
		'README.md'
		'--output=README.htm'
		'--from=gfm'
		'--self-contained', "--css=$env:MarkdownCss"
		'--standalone', "--metadata=pagetitle=$ModuleName"
	)}
}

# Synopsis: Generate meta files.
task Meta -Inputs .build.ps1, Release-Notes.md -Outputs src/Directory.Build.props -Jobs Version, {
	Set-Content src/Directory.Build.props @"
<Project>
	<PropertyGroup>
		<Company>https://github.com/nightroman/$ModuleName</Company>
		<Copyright>Copyright (c) Roman Kuzmin</Copyright>
		<Description>FarNet friendly FSharp.Charting extension</Description>
		<Product>$ModuleName</Product>
		<Version>$Version</Version>
		<FileVersion>$Version</FileVersion>
		<AssemblyVersion>$Version</AssemblyVersion>
	</PropertyGroup>
</Project>
"@
}

# Synopsis: Collect package files.
task Package -If:$FarDevHome Markdown, {
	remove z
	$toLib = mkdir "z\lib\net45"
	$toModule = mkdir "z\tools\FarHome\FarNet\Lib\$ModuleName"
	$fromModule = "$FarDevHome\FarNet\Lib\$ModuleName"

	Copy-Item -Destination $toLib @(
		"$fromModule\FarNet.FSharp.Charting.dll"
		"$fromModule\FarNet.FSharp.Charting.xml"
	)

	Copy-Item -Destination $toModule @(
		'README.htm'
		'LICENSE.txt'
		"$fromModule\FarNet.FSharp.Charting.dll"
		"$fromModule\FarNet.FSharp.Charting.ini"
		"$fromModule\FarNet.FSharp.Charting.xml"
		"$fromModule\FSharp.Charting.dll"
		"$fromModule\FSharp.Charting.xml"
	)
}

# Synopsis: Make NuGet package.
task NuGet -If:$FarDevHome Package, Version, {
	# test versions
	$dllPath = "$FarDevHome\FarNet\Lib\$ModuleName\$ModuleName.dll"
	($dllVersion = (Get-Item $dllPath).VersionInfo.FileVersion.ToString())
	assert $dllVersion.StartsWith("$Version.") 'Versions mismatch.'

	$text = @'
FarNet friendly FSharp.Charting extension

---

The package may be used as usual in F# projects.

It is also configured for FarNet.FSharpFar.
To install FarNet packages, follow these steps:

https://raw.githubusercontent.com/nightroman/FarNet/master/Install-FarNet.en.txt

---
'@
	# nuspec
	Set-Content z\Package.nuspec @"
<?xml version="1.0"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>$ModuleName</id>
		<version>$Version</version>
		<authors>Roman Kuzmin</authors>
		<owners>Roman Kuzmin</owners>
		<projectUrl>https://github.com/nightroman/$ModuleName</projectUrl>
		<license type="expression">Apache-2.0</license>
		<requireLicenseAcceptance>false</requireLicenseAcceptance>
		<summary>$text</summary>
		<description>$text</description>
		<releaseNotes>https://github.com/nightroman/FarNet.FSharp.Charting/blob/master/Release-Notes.md</releaseNotes>
		<tags>FarManager FarNet FSharp Charting</tags>
		<dependencies>
			<group targetFramework=".NETFramework4.5">
				<dependency id="FSharp.Charting" version="2.1.0" />
			</group>
		</dependencies>
	</metadata>
</package>
"@
	# pack
	exec { NuGet.exe pack z\Package.nuspec }
}

task . Build, Clean
