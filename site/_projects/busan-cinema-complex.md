---
title:      Busan Cinema Complex
date:       19-03-11
published:  true
excerpt:    A scripted ties topography to tile over the course of this competition entry.
year:       2006 (designed)
location:   Busan, South Korea
designers:  James Corner Field Operations and TEN Arquitectos
files:      true
files_text: model and definition that demonstrate a partial recreation of this project
---

In 2006, James Corner Field Operations worked with architects TEN Arquitectos to develop a competition entry for the design of a new Busan Cinema Centre in South Korea. Although their entry did not win, it is notable as an early example of computational design methods being clearly expressed in a designed landscape.

{% include elements/figure.html image='busan-birds-eye' alt='Perspective of the landscape design' credit="James Corner Field Operations and TEN Arquitectos, from Film and Architecture, p. 215." %}

The competition entry's landscape features an undulating lawn dotted with planting to guide pedestrian circulation and an adaptive paving system that stretched across the site. The differentiated pavers are designed to be responsive to the landform of the proposal, whereby the orientation and size of each individual set of tiles smoothly shifts according to changes in slope. The result is a [field aesthetic]({% link _techniques/field-conditions.md %}) or "ambient surface"[@JoonKang:2006 208] where the tiles are smallest and most offset from their original horizontal orientation where the topography is highest.

{% include elements/figure.html image='busan-field' caption='Details of the paving system and its relationship to landform.' credit='By James Corner Field Operations and TEN Arquitectos, via scenariojournal.com/lu-from-hand-to-land/.' %}

This tie between surface and object suggests an approach that can establish a direct and intuitive link between a primary design driver — landform — and a secondary design feature — paving — that can be explored in an expressive manner. Once developed, the tiling system would be able to quickly respond to different landforms and so create a  feedback loop where the topography and the tile geometries are tied together throughout design development.

{% include elements/figure.html image='busan-pspxv' alt='Perspective of the Busan Cinema competition entry.' credit="By James Corner Field Operations and TEN Arquitectos, adapted from scenariojournal.com/lu-from-hand-to-land/." %}

This tight, or "articulated,"[@Hansen:2011] relationship between the two design features seems especially valuable in the context of developing a competition entry where time constraints often limit the full realisation of a concept. Here, the use of scripting likely allowed the tiling elements to exist at both a conceptual and detailed level of development simultaneously  as changes to the topography, tile shape, tile size, or architectural form could be accommodated with ease because the relationships between each were explicitly defined using computational rules.

{% include elements/figure.html image='busan-layers' alt="A number of plans showing the different layers of the proposal." credit='By James Corner Field Operations and TEN Arquitectos, reproduced from Film and Architecture, p. 216-217.' %}

### Reference Model

While the proposal pre-dated the release of Grasshopper, the software would have offered an easy means of prototyping and iterating upon what was likely developed as a script in Maya or Rhino. Such a parametric model could have proceeded by first referencing contours and `Patch`ing them to become a surface.

{% include elements/figure.html image='step_1' caption='Creating a 3D surface by patching contours allows for later grading adjustments to be easily accommodated.' credit="Albert Rex, for groundhog.la." %}

Once defined, the 2D bounding box of the surface can be used to roughly calculate the number of pavers (given a set spacing interval) that will fit comfortably within the paving area. After creating a corresponding grid of points on the `XY` plane, each point is then `Project`ed onto the surface to establish the origins of each tile shape.

{% include elements/figure.html image='step_2' caption='The origin points of each paver are projected onto the topography.' credit="Albert Rex, for groundhog.la." %}

The vertical distance from the base `XY` plane to each tile-origin is measured and remapped to become the variable data that informs the geometry of the two elements that constitute each individual tile. By controlling the range of numbers that inform the minimum/maximum rotational/scaling factors the designer can 'hone' exactly how the tile system responds to the surface.

{% include elements/figure.html image='step_3' caption='The elevation of each paver is used to define its shape, size and rotation' credit="Albert Rex, for groundhog.la." %}

The result is a paving system that responds to and accentuates the topography of site. Moreover, the form of each tile can be quickly iterated upon in response to topographic manipulation, to the parameters that control the geometry of the paving elements, or to the regions that define where the tiling system is deployed.

{% include elements/figure.html image='step_4' caption='The paving system responds dynamically to topography.' credit="Albert Rex, for groundhog.la." %}

{% include elements/figure.html image='definition' caption='Grasshopper definition recreating the basic tile effect and distribution.' credit='Albert Rex and Philip Belesky, for https://groundhog.la' %}

{% include elements/files.html %}
