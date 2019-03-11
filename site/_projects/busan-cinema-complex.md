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

{% include elements/figure.html image='busan-birds-eye' caption='Busan Cinema Centre design concept.' credit="By James Corner Field Operations and TEN Arquitectos, from Film and Architecture: Busan Cinema Complex International Invited Competition, p. 215." %}

In 2006, landscape architects James Corner Field Operations  worked with architects TEN Arquitectos – Mexico City, New York - to submit a proposal for a competition run by the Busan International Architectural Cultural Festival for the design of a new Busan Cinema Centre in South Korea.

Although the competition entry was not successful, it is notable as one of relatively few proposals or projects by Field Operations – perhaps the most well know landscape architecture studio operating today - to incorporate parametric technologies as a central design driver.

{% include elements/figure.html image='busan-layers' credit='By James Corner Field Operations and TEN Arquitectos, reproduced from Film and Architecture: Busan Cinema Complex International Invited Competition, p. 216-217.' %}

The landscape portion of the design concept featured an expansive undulating lawn dotted with planting to guide pedestrian circulation and an adaptive parametric paving system that stretched across the site.

{% include elements/figure.html image='busan-field' caption='Details of paving system from the Busan Cinema Complex competition entry.' credit='By James Corner Field Operations and TEN Arquitectos, reproduced from From Hand to Land.' %}

This parametrically controlled paving system was developed using a rule set where paver size and rotation increased or decreased in response to site topography. The result is a graduated field where the tiles are smallest and most offset from their original horizontal orientation where the surface is highest.

This tie between landform and tiling strategy suggests an approach that can establish a direct and intuitive link between a primary design driver — the landform — and a secondary design feature — tiling — that can be optimised according to a given landscape condition.

This field-like tiling strategy, once developed, would respond to iterations in the underlying landform, creating a feedback loop that could inform both the 'base' geometry of the topography and the tiling strategy itself as the design develops.

{% include elements/figure.html image='busan-pspxv' alt='Perspective of the Busan Cinema competition entry.' credit="By James Corner Field Operations and TEN Arquitectos, adapted from From Hand to Land." %}

The use of parametric tools in the development of Field Operations' submission for the Bussan Cinema Complex competition allowed the design team to work at both a conceptual and detail documentation stage of the design process simultaneously. Changes in topography, tile shape, tile size, or architectural elements could be accommodated with ease because the relationships between each element had been explicitly defined as computational rules. This is a way of working that becomes particularly useful in a competition scenario, where time limits can become significant constraints upon the full realisation of a concept.

In this following section, we give an example of how this proposal could have been put together using tools such as Rhino and Grasshopper. First, contours from Rhino, distributed accurately in 3D space are imported into Grasshopper and 'Patched' into a 3D surface.

{% include elements/figure.html image='step_1' caption='Grasshopper patching a 3D surface with contours imported from Rhino.' credit="Albert Rex, for groundhog.la." %}

A 'bounding box' is used to roughly calculate the number of pavers that will fit comfortably within the paving area. These points are 'projected' onto the site topography with a small planar rectangle generated from each to serve as a base for future operations.

{% include elements/figure.html image='step_2' caption='Origin points for each paver regularly projected across paving area in Grasshopper.' credit="Albert Rex, for groundhog.la." %}

The Vertical offset of each projected point from a standard base plane is calculated. This information, or more specifically the variation between the various points, is 'remapped' into data that informs the rotation and size of the two different tile elements.

NOTE: This element of the definition affords considerable design freedom to the user of the script. The rotation, shape and size of all tiles are controllable from this point, as is the data input used to regulate it.

{% include elements/figure.html image='step_3' caption='Paver height illustrated in section. Relationship to pave shape, size and rotation displayed diagrammatically above.' credit="Albert Rex, for groundhog.la." %}

The result is a paving system that responds to and accentuates the topography of site. It can iterate quickly as topography changes or the rules that control the shape, size and rotation of paving elements are changed.

{% include elements/figure.html image='step_4' caption='A paving system which responds dynamically to topography.' credit="Albert Rex, for groundhog.la." %}
