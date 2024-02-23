## Welcome to Luna build

This repository contains the core modules for the luna build system, which is a C# based meta build system. So the first and foremost reason why it exists is to generate code solutions for any IDE, given that a user provides how a solution is structed. For now this repository will provide support for Visual Studio 2022. Down the road the goal is to also provide targets for other IDEs like CLion or XCode and the likes.

## License
MIT

## Supported platforms
|Windows|Linux|MacOS|
|:--:|:--:|:--:|
|Jep|Should work|Should work|

## Supported Targets
|Visual Studio|CLion|XCode|Others|
|:--:|:--:|:--:|:--:|
|2022|Planned|Planned|TBD|

## Requirements
[.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet)
<br>
[Avalonia UI](https://marketplace.visualstudio.com/items?itemName=AvaloniaTeam.AvaloniaVS) [Only for UI project]

## Get started
Clone the repository, extract it and compile the LunaCLI project as well as all plugins and targets which are required. Now it is time to create the config file for luna to work with. It describes where luna should generate the solution, where the code modules are as well as additional meta services. Beyond that, it also contains a list for plugins and targets which should be loaded and the Luna Bridge be linked against.

## Luna Config
```json
{
    "SolutionPath": "%path_to_solution%",  <- Path to where the solution should be generated
    "CodePath": "%path_to_code%", <- Path to where the code modules are located which contain *.build.cs files
    "MetaPath": "%path_to_meta%", <- Path to where the meta sevices are located which contain *.meta.cs files
    "CorePath": "%path_to_core%", <- Path to where LunaCore.dll is located [OPTIONAL]
    "WorkspacePath": "%path_to_workspace%", <- Sets the workspace path for luna. Used for plugin and target look ups
    "Plugins": [
        "Luna.Meta.Cpp.Projects" <- List of plugins that should be loaded from %path_to_workspace%/Plugins/ or %path_to_luna%/LunaCLI/Plugins/
    ],
    "Targets": [
        "Luna.Targets.VisualStudio" <- List of targets that should be loaded from %path_to_workspace%/Targets/ or %path_to_luna%/LunaCLI/Targets/
    ]
}
```

## Luna Bridge
This project is generated and compiled at runtime (unless the `-nocode` flag is given) which contains custom code from meta services as well as code modules which should be include in the solution. It is linked against any plugin and target which is listed in the luna config, which means exported types from those modules can be used explicitly in custom code.<br>
||Extension|Look Up|Interface|
|:--:|:--:|:--:|:--:|
|Meta Services|.meta.cs|%Meta Path%\\**\\*.meta.cs|Luna.Core.IMeta
|Build Services|.build.cs|%Code Path%\\**\\*.build.cs|Luna.Core.IBuild

## Command Line Arguments
```ps
.\LunaCLI.exe -config $Path_to_Config$ [-nocode] [--help]

-nocode // Skips compiliation of LunaBridge.dll and takes a currently available version.
-config // Tells Luna where to find its config file.
--help // Prints the help information.
```

## Order of Intialization
* Core Services
* Targets
* Plugins
* Luna Bridge
    * Meta Services
    * Build Services

## Build Example
```C#
public class BuildableModule : Luna.Core.IBuild
{
    private CommonMeta? _commonMeta;
    private PluginMeta? _pluginMeta;

    // Getting called by the core system to register a single instance of this buildable module.
    public void Register()
    {
        Luna.Core.ServiceProvider.RegistryService.RegisterBuildService(this);

        IOptionService? options = Luna.Core.ServiceProvider.RegistryService.GetMetaService<IOptionService>();
        // Used to register options. Can be used to enable/disable certain modules.
        options?.RegisterOption(Guid.NewGuid(), "New Option", true);

        // Can be used to get custom meta services.
        _commonMeta = Luna.Core.ServiceProvider.RegistryService.GetMetaService<CommenMeta>();

        _pluginMeta = Luna.Core.ServiceProvider.RegistryService.GetMetaService<PluginMeta>();
    }

    // Called when the system needs to be reconfigurated. Might be that some options are no longer available
    // or the flag changed. This is the chance to react to that.
    public void Configurate()
    {
        
    }

    // Called when the solution gets generated and all projects should be generated.
    // Project plugins can be used here to generate specfic flavors of projects.
    public IProject? Generate(ISolution solution)
    {
        // generate project.
	    return someProject;
    }
}
```
## Meta Examples
```C#
public class CommonMeta : Luna.Core.IMeta
{
	public void Register()
	{
		Luna.Core.ServiceProvider.RegistryService.RegisterMetaService(this);
	}

    // Can be anything in here. All custom build services have access to this type and 
    // can therefore cast an IMeta instance into it.
}
```
## Plugin Example
```C#
public class SomePluginMeta : Luna.Core.IMeta
{
    // Basically a meta service that can easily be shared.
    // Custom meta services and build services can cast IMeta instances into specific plugin metas.
	public void Register()
	{
		Luna.Core.ServiceProvider.RegistryService.RegisterMetaService(this);
	}
}
```
Plugins are only scanned for `IMeta` based types. There is no technical limitiation though that blocks registration of targets and build services while meta services are registered. The prefered way though is to create stand alone target modules and build modules for those cases.
## Target Example
```C#
public class CWindowsTarget : Luna.Core.Target.ITarget
{
    // Name of the Target.
	public string Name => "Visual Studio 2022 - Windows x64";

    // Folder name where the solution gets generated in.
	public string SolutionFolder => "Windows";

    // Full Solution Path.
	public string FullSolutionPath => Path.Combine(Path.GetFullPath(LunaConfig.Instance.SolutionPath), SolutionFolder);

    // Registers this target with the core system.
	public void Register()
	{
		ServiceProvider.RegistryService.RegisterTarget(this);
	}

	public bool GenerateSolution()
	{
        // Solution generation logic for the specifc target.
		return true;
	}
}
```
Targets are only scanned for `ITarget` based types. There is no technical limitiation though that blocks registration of meta and build services while targets are registered. The prefered way though is to create stand alone target modules and build modules for those cases.

## Debugging Luna Bridge
Luna creates a standalone project for the LunaBridge that can be used to debug user code without having the complete Luna source code available. It requires a small addition in form of `launchSettings.json`. The project can be found under `%path_to_workspace%/LunaBridge` and to enable debug support `Properties/launchSettings.json` must be created. Working with this approach also always to catch compile issues before luna gets executed.
### launchSettings.json
```json
{
  "profiles": {
    "LunaBridge": {
      "commandName": "Executable",
      "executablePath": "%path_to_lunacli.exe",
      "commandLineArgs": "-config %path_to_config%"
    }
  }
}
```
### Visual Studio Support
Out of the box support when LunaBridge.csproj is opened.
### Visual Studio Code Support
Requires [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
Out of the box support when LunaBridge folder is opened.