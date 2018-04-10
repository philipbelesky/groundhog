## Changelog

#### [0.7.3b] -
###### Added
- Exception reporting
- Yak package

###### Changed
- MeshSlope component will now output slopes as a percentage as well as an angle

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