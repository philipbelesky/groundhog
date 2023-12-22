---
layout:     page
title:      Plugin
meta:       download latest plugin version
excerpt:    Download the latest version of the Groundhog plugin for Grasshopper.
---

# Download and Install

---

{% include elements/download.html %}
{% include elements/newsletter.html %}

---

## Support, Bug Reports, and Feature Requests

Support is offered — subject to availability — through [this email address](mailto:groundhog@philipbelesky.com). When asking for support please ensure you provide an example Rhinoceros model, Grasshopper definition, and a detailed description of your problem in terms of what you are trying to do, what you expect to happen, and what is going wrong.

The preferred method of submitting bug reports and feature requests is through [Github Issues](https://github.com/philipbelesky/groundhog/issues). As with support enquiries, please ensure any bug reports provide an example model/definition along with a detailed description of your intended use and the resulting problem.

## Changelog

#### [0.13.1] - 2021-06-08

###### Changed

- Altered license to LGPL

#### [0.13.0] - 2021-05-24

###### Fixed

- Various documentation improvements
- Improvement to the catchment boundary cell generation
- Improved handling of flow path lines near edges of meshes and surfaces
- Added further examples to Flow Saturation examples
- Various improvements to precedent projects, particularly Edaphic

###### Changed

- Moved contour components into a 'Utilities' section

###### Added

- Additional output parameters to Flow Saturation component
- New *Mesh Color by Face* utility to assign a single per mesh face must more efficiently

#### [0.12.1] - 2020-08-11

###### Fixed

- Fixed issue that prevented Groundhog from installing on older Rhino versions

#### [0.12.0] - 2020-08-05

###### Added

- Added component to analyse and display saturation gradients given a flow path

#### [0.11.1] - 2020-07-02

###### Fixed

- FieldComponent error

#### [0.11.0] - 2020-05-17

###### Changed

- Definitions and models now default to Rhino 6 compatibility
- Clarified parameter labels in planting components
- Renamed 'Fidelity' parameter to 'Jump' for flow components

###### Added

- Added components to visualise canopy/root masses using meshes
- Flow components can now trace uphill paths with negative distances
- Flow catchments component now guesses a proximity threshold if one is not set
- Plant components actually use variance values
- Added "Volume %" output to catchment component to easily display the % of flow paths terminating within an area

#### [0.10.2] - 2019-04-09

###### Fixed

- Fixed issue where no components would work due to problem in the build process

#### [0.10.1] - 2019-04-07

###### Fixed

- Improved handling of non-planar curves in the ChannelRegion component
- Improved handling of null points in the Flows components

#### [0.10.0] - 2018-11-22

###### Added

- Added the Flow Area component that derives the water level within a defined channel given a defined flow quantity
- Added the Flow Profile component that calculates a series of hydraulic characteristics for a given flow area
- The Slope components will now calculate slope as a ratio

#### [0.9.1] - 2018-05-15

###### Changed

- Plant shower will now do nothing when provided with a negative time value so successional planting schemes are easier to implement
- Random Path component now provides a seed parameter so results can be used deterministically

###### Fixed

- Improved handling of null items and bad inputs across many components

#### [0.9.0] - 2018-05-06

###### Added

- RandomPath component for simulating psuedo random 2D walks
- ShortestPath component for identifying quickest part with a curve network

#### [0.8.0] - 2018-05-01

###### Added

- Surface equivalents of the Slope and Aspect terrain components

###### Fixed

- Flow components will no longer crash when given fewer than three points without a specified Fidelity
- Providing bad input to the Field Mapper component will now reject it rather than proceeding and creating an error
- Improved some error handling on the Contour components

#### [0.7.2b] - 2018-04-15

###### Added

- Exception reporting
- Yak package

###### Changed

- MeshSlope component will now output slopes as a percentage as well as an angle
- Flow components will now compute a sensible fidelity step if not given an exact distance

#### [0.7.1b] - 2018-04-01

###### Fixed

- MeshSlope and MeshAspect components should now properly compute the face normals for quad meshes
- Better handle null items in FlowCatchment component
- Flood components should produce more sensible results

#### [0.7.0b] - 2018-03-25

###### Added

- New MeshAspect component
- Full website added along with basic examples and documentation for some components
- Both FlowPath components now have an optional 'Limit' parameter

###### Changed

- Improved Flows and Terrain example files with more visualisation options
- MeshSlope component will no longer unitise the vector outputs

###### Fixed

- Improved parsing of each component's generic parameters for tables in documentation

#### [0.6.1a] - 2017-11-20

###### Changed

- Improved component Icons slightly (proper icons coming soon!)

###### Removed

- Plant Placement Solver component (needs to be reworked form the C# script)

#### [0.6.0a] - 2017-08-27

###### Changed

- Flow Path component outputs paths as a flat list; not a tree
- Flow Path component now split into separate versions for Surfaces and Meshes
- Switched references to use NuGet

###### Fixed

- Fixed generic plant attributes
- Fixed contour clipped so it excludes contours completely outside the boundary

#### [0.5a]

###### Changed

- Initial release for Skin + Scale Studio
- Tidied up example definitions

<!--
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).
-->
