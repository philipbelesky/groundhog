---
title:      Busan Cinema Complex
date:       19-03-11
published:  true
excerpt:    lorum
year:       2006 (designed)
location:   Busan, South Korea
designers:  James Corner Field Operations and TEN Arquitectos
files:      true
files_text: model and definition that demonstrate a partial recreation of this project
tags:
---

In 2006, landscape architects James Corner Field Operations worked with architects TEN Arquitectos to develop a competition entry for the design of a new Busan Cinema Centre in South Korea. Although their entry was not successful, it is notable as an early example of a computational design method driving landscape architectural design.

{% include elements/figure.html image='busan-birds-eye' caption='' credit="James Corner Field Operations and TEN Arquitectos, from Film and Architecture, p. 215." %}

The entry's landscape concept featured an undulating lawn dotted with planting to guide pedestrian circulation and an adaptive parametric paving system that stretched across the site. The differentiated paving systems is designed to be responsive to the landform of the proposal, whereby the orientation and size of each individual paver smoothly shifts according to gradual changes in slope. The result is a [field aesthetic]({{ site.baseurl }}{% link _techniques/field-conditions.md %}) or "ambient surface"[@JoonKang:2006 208] where the tiles are smallest and most offset from their original horizontal orientation where the surface is highest.

{% include elements/figure.html image='busan-field' caption='Details of the paving system and its relationship to landform.' credit='By James Corner Field Operations and TEN Arquitectos, via scenariojournal.com/lu-from-hand-to-land/.' %}

This tie between topography and tile suggests an approach that can establish a direct and intuitive link between a primary design driver — landform — and a secondary design feature — paving — that can be explored in an expressive manner. Once developed, the tiling system would respond to iterations upon the grading of the site, creating a feedback loop that could inform both the 'base' topography and the geometry of the tiles as the design develops. This tight relationship between the two features seems especially valuable in the context of developing a competition entry where time constraints often limit the full realisation of a concept. However, the use of scripting here allowed some element of the design to exist at both a conceptual and detailed level of development simultaneously. Changes in topography, tile shape, tile size, or architectural elements could be accommodated with ease because the relationships between each element had been explicitly defined as computational rules.

{% include elements/figure.html image='busan-pspxv' alt='Perspective of the Busan Cinema competition entry.' credit="By James Corner Field Operations and TEN Arquitectos, adapted from scenariojournal.com/lu-from-hand-to-land/." %}

{% include elements/figure.html image='busan-layers' credit='By James Corner Field Operations and TEN Arquitectos, reproduced from Film and Architecture, p. 216-217.' %}

### Reference Model

While the proposal pre-dated the release of Grasshopper, it would have offered an easy means of prototyping and iterating upon what was likely a script in Maya or Rhino. Such a parametric model could have proceeding by first referencing contours and `Patch`ing them to become a surface.

{% include elements/figure.html image='step_1' caption='Creating a 3D surface by patching contours' credit="Albert Rex, for groundhog.la." %}

Once defined, the 2D bounding box of the surface can be used to roughly calculate the number of pavers (given a set spacing interval) that will fit comfortably within the paving area. These origins of these grid point can then be `Project`ed onto the landform to establish the centers of each tile.

{% include elements/figure.html image='step_2' caption='The origin points of each paver  projected onto the topography.' credit="Albert Rex, for groundhog.la." %}

The vertical offset from the base plane to each tile-center is calculated and 'remapped' into data that informs the rotation and size of the two different elements that constitute each individual tile.

{% include elements/figure.html image='step_3' caption='The elevation of each paver is used to define its shape, size and rotation' credit="Albert Rex, for groundhog.la." %}

The result is a paving system that responds to and accentuates the topography of site. Moreover, the form of each tile can be quickly iterated upon in response to topographic manipulation or to the parameters that control the shape, size and rotation of paving elements.

{% include elements/figure.html image='step_4' caption='The paving system responds dynamically to topography.' credit="Albert Rex, for groundhog.la." %}
