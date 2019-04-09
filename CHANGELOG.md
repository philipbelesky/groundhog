## Changelog

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
