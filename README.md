<div align="center">
<img width=200 src="https://cdn.rawgit.com/philipbelesky/groundhog/develop/site/assets/logo.svg">

# Groundhog

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![GitHub release](https://img.shields.io/github/release/philipbelesky/groundhog.svg)](https://github.com/philipbelesky/groundhog/releases)
[![Component Build status](https://ci.appveyor.com/api/projects/status/v54xuu2rea3q4r3p/branch/develop?svg=true)](https://ci.appveyor.com/project/philipbelesky/groundhog/branch/develop)
[![Website Build Status](https://travis-ci.org/philipbelesky/groundhog.svg?branch=develop)](https://travis-ci.org/philipbelesky/groundhog)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/86683403554e426baad9225687d5ca00)](https://www.codacy.com/app/philipbelesky/groundhog?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=philipbelesky/groundhog&amp;utm_campaign=Badge_Grade)

</div>

Groundhog is a [Grasshopper plugin](http://grasshopper3d.com), a set of reference models, and wiki exploring the applications of computational design in landscape architecture. Groundhog is currently in beta. The plugin and associated site will be updated with more features and information over the course of 2018.

Please visit [www.groundhog.la](http://groundhog.la) for documentation and examples of how to use the plugin and for resources on computational approaches to landscape architectural design in general.

## Documentation, Demos, and Reference Definitions

See the provided documentation and demo, example, and reference files at [groundhog.la](http://groundhog.la).

## Support, Bug Reports, and Feature Requests

Refer to [groundhog.la/plugin/](http://groundhog.la/plugin/).

## Contributing

Feedback and pull requests welcome; see [`CONTRIBUTING.md` file](https://github.com/philipbelesky/groundhog/blob/develop/CONTRIBUTING.md).

## Plugin Installation

See the [`README.md` file](https://github.com/philipbelesky/groundhog/blob/develop/plugin/README.md) located in the `plugin` folder.

## Plugin Development

To develop the plugin you will need a copy of Rhinoceros installed, and some knowledge working with [C# code](https://docs.microsoft.com/en-us/dotnet/csharp/) and the [Rhinoceros/Grasshopper APIs](http://developer.rhino3d.com).

Editing and compiling that code is best done in Visual Studio 2017. The [community editions](https://www.visualstudio.com) for Windows or macOS should both work. Upon first build it should fetch the required RhinoCommon, Grasshopper, and third-party references from NuGet (an internet connection is required).

You will need to add the `build` folder to your Grasshopper Folders once you have compiled the project. To do so use the `GrasshopperDeveloperSettings` command in Rhinoceros.

## Wiki Development

See the [`README.md` file](https://github.com/philipbelesky/groundhog/blob/develop/site/README.md) located in the `site` folder.

## License

This project is licensed under the GPL v3 License - see the [`LICENSE` file](https://github.com/philipbelesky/groundhog/blob/develop/LICENSE) for details.
