# Groundhog Plugin

To develop the plugin you will need a copy of Rhinoceros installed, and some knowledge working with [C# code](https://docs.microsoft.com/en-us/dotnet/csharp/) and the [Rhinoceros/Grasshopper APIs](http://developer.rhino3d.com).

## Setup

Editing and compiling that code is best done in Visual Studio 2017. The [community editions](https://www.visualstudio.com) for Windows or macOS should both work.

Upon first build it should fetch the required RhinoCommon and Grasshopper references from NuGet (an internet connection is required).

You will need to add the `build` folder to your Grasshopper Folders. To do so use the `GrasshopperDeveloperSettings` command in Rhinoceros.