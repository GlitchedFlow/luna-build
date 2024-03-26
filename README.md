[![Generate Solution](https://github.com/GlitchedFlow/luna-build/actions/workflows/dotnet_cli.yml/badge.svg?branch=main)](https://github.com/GlitchedFlow/luna-build/actions/workflows/dotnet_cli.yml)
[![Build CLI](https://github.com/GlitchedFlow/luna-build/actions/workflows/dotnet_build_cli.yml/badge.svg?branch=main)](https://github.com/GlitchedFlow/luna-build/actions/workflows/dotnet_build_cli.yml)

## Welcome to Luna build

This repository contains the core modules for the luna build system, which is a C# based meta build system. it can be used to generate Visual Studio 2022 solutions (other IDEs are planned) based on `*.build.cs` files. It provides a CLI, which has support for simple luna scripts (`*.lusc`), as well as a simple UI to allow quick solution generation.

## License
MIT

## Supported platforms
|Windows|Linux|MacOS|
|:--:|:--:|:--:|
|Jep|Should work|Should work|

## Supported Targets
|Visual Studio|CLion|XCode|Others|
|:--:|:--:|:--:|:--:|
|2022|Planned|Planned|TBA|

## Requirements
[.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet)
<br>
[Avalonia UI](https://marketplace.visualstudio.com/items?itemName=AvaloniaTeam.AvaloniaVS) [Only for UI project]

## Get started
Make sure the requirements are installed then download the [1.0 release](https://github.com/GlitchedFlow/luna-build/releases/tag/v1.0), extract it and execute the CLI with the path to the `%RepoRoot%\Tools\Luna\Scripts\build_windows.lusc` script as argument. This will generate the solution for Visual Studio 2022 under `%RepoRoot%\solution\Windows\Luna_Windows.sln`, which can be used to compile luna-build.

## Luna Config
The luna config can be used to make configurate steps for the luna CLI easier. It can be loaded via the `load` command in the core module.
```json
{
	"Name": "Luna", <- Main name of the solution that gets generated
	"SolutionPath": "%path_to_solution%",  <- Path to where the solution should be generated
	"CodePath": "%path_to_code%", <- Path to where the code modules are located which contain *.build.cs files
	"MetaPath": "%path_to_meta%", <- Path to where the meta sevices are located which contain *.meta.cs files
	"CorePath": "%path_to_core%", <- Path to where LunaCore.dll is located [OPTIONAL]
	"WorkspacePath": "%path_to_workspace%", <- Sets the workspace path for luna. Used for plugin and target look ups
	"Plugins": [
		"Sample" <- List of plugins that should be loaded from %path_to_workspace%/Plugins/ or %path_to_luna%/Plugins/
	],
	"Targets": [
		"VisualStudio" <- List of targets that should be loaded from %path_to_workspace%/Targets/ or %path_to_luna%/Targets/
	]
}
```

## Luna Bridge
Luna Bridge is the dynamically compiled project that contains all the user code to generate projects and provide custom services for user specific scenarios. The project will be generated in the provided workspace, which can be set via luna config. With it, it is possible to debug user code very easily. Simply open the generated `*.csproj` file from the project in Visual Studio or an IDE which supports `*.csproj` files.

||Extension|Look Up|Interface|
|:--:|:--:|:--:|:--:|
|Meta Services|.meta.cs|%Meta Path%\\**\\*.meta.cs|Luna.Core.IMeta
|Build Services|.build.cs|%Code Path%\\**\\*.build.cs|Luna.Core.IBuild

## Command Line Arguments
```ps
.\CLI.exe %Path_to_luna_script% [--help]
```
|Argument|Description|
|:--:|:--:|
|%Path_to_luna_script%|Path to a script file [Optional]|
|--help| Prints help information|

## Suggested Configuration Steps
Luna allows to set up custom workflows and steps to configurate Luna to custom needs its suggested to do the following configuration steps:

|Step|CLI Command|
|:--:|:--:|
|Load luna config|`load %path_to_confg%`|
|Compile|`compile`|
|Targets|`init targets`|
|Plugins|`init plugins`|
|Luna Bridge|`init bridge`|

## Examples
The best place to check out how the different systems work and how to set them up is by checking out the repo itself,
since it in itself use a version of Luna to generate the solution. Easy look up for certain approaches:

|Type|Example location|
|:--:|:--:|
|Meta Service|[`Tools\Luna\Meta\ProjectService.meta.cs`](https://github.com/GlitchedFlow/luna-build/blob/main/Tools/Luna/Meta/ProjectService.meta.cs)|
|Build Service|[`Code\CLI\CLI.build.cs`](https://github.com/GlitchedFlow/luna-build/blob/main/Code/CLI/CLI.build.cs)|
|Target|[`Code/Targets/Luna.Targets.VisualStudio/Target.cs`](https://github.com/GlitchedFlow/luna-build/blob/main/Code/Targets/Luna.Targets.VisualStudio/Target.cs)|
|Plugin|[`Code/Plugins/Luna.Plugins.Sample/SamplePlugin.cs`](https://github.com/GlitchedFlow/luna-build/blob/main/Code/Plugins/Luna.Plugins.Sample/SamplePlugin.cs)
|CLI Plugin|[`Code/Plugins/Luna.CLI.Plugins.Sample/SamplePlugin.cs`](https://github.com/GlitchedFlow/luna-build/blob/main/Code/Plugins/Luna.CLI.Plugins.Sample/SamplePlugin.cs)
|CLI Script|[`Tools/Luna/Scripts/build_windows.lusc`](https://github.com/GlitchedFlow/luna-build/blob/main/Tools/Luna/Scripts/build_windows.lusc)

## Debugging Luna Bridge
As mentioned, Luna creates a standalone project for the bridge project that can be used to debug user code without having the complete Luna source code available. It requires a small addition in form of `launchSettings.json`. The project can be found under `%path_to_workspace%/Bridge` and to enable debugging support `Properties/launchSettings.json` must be created. Once the bridge project was created, it can also be compiled by itself without the need to execute the CLI to check if all of the linked user code compiles.

### launchSettings.json
```json
{
  "profiles": {
	"LunaBridge": {
	  "commandName": "Executable",
	  "executablePath": "%path_to_cli.exe%"
	}
  }
}
```
### Visual Studio Support
Out of the box support when `Bridge.csproj` is opened.
### Visual Studio Code Support
Requires [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
Out of the box support when Bridge folder is opened.
