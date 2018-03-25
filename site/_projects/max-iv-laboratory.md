---
title:      MAX IV Laboratory Landscape
date:       17-11-14
excerpt:    A defensive topography that that creates an intricate series of rolling mounds to dampen vibrations from a nearby highway.
thumbnail:  4.jpg
year:       2016 (constructed)
location:   Lund, Sweden
designers:  Snøhetta
files:      true
files_text: model and definition that demonstrating a partial recreation of this project
categories:
- parametric geometry
- simulation
tags:       grasshopper,
---

![The MAX Lab facility uses a rippled spiral of topographic form to surround the main building. *Image via Snøhetta website's project page (https://snohetta.com/projects/70-max-iv-laboratory-landscape)*](/assets/projects/max-iv-laboratory/7.jpg)

In designing this new scientific facility a major concern was that external vibrations from a nearby highway would disrupt the measurements from sensitive laboratory instruments.[@Snohetta:2016us 1] The site's pre-existing topography — a flat slope — heightened this fear as the largey planar surface exacerbated the issue.[@Snohetta:2016us 2] Thus a key design goal for Snøhetta was to maximise the landscape's surface area through a rippled topography that would, in effect, scatter the vibrations that might interfere with the laboratory. At the same time, such an exuberant grading could provide some ancillary benefits such as managing the water run-off.[@Snohetta:2016us 2]

> "3D-modelling proved crucial for several reasons. The design layout was established by extracting the nature of vibrations into rational values inserted in a generic model (Grasshopper; a Rhino plug-in). In plan, intersecting tangents radiating from the major storage ring form the first basis of the wave pattern. These align with the positions of potential future laboratories, and the starting points were defined by 10 to 40m vibration wavelengths and a 4.5m amplitude. ... Our digital model enabled continuous testing of the pattern's effect on mitigating the ground vibrations."

Several Grasshopper definitions were used across the project. In the main definition that drove the base landform, the vibrations from the adjacent roads were implemented as a parametised constraint whose exact value could be honed over many iterations in conjunction with an engineering team.[@Walliss:2016vy 39] Once set, this constraint allowed the design team to then assess the dampening effects of specific topographic forms and fine-tune them. The resulting topographies could then be further analysed and evaluated according to secondary design criteria that were encapsulated in other definitions that would simulate wind conditions, inform tree planting, visualise a maximum slope gradient, or measure stormwater drainage and retention.[@Walliss:2016vy 37]

![](/assets/projects/max-iv-laboratory/3.jpg)

![The topographic form was designed using an intersecting series of geometric projections that extend as tangents from the outer ring of the main laboratory building. *Image via Snøhetta  press release 'The MAX IV Laboratory Landscape Design by Snøhetta to Open Summer 2016.'*](/assets/projects/max-iv-laboratory/6.jpg)

As compared to other projects discussed, the design process for the MAX Lab IV landscape presents a clearer (or perhaps just more clearly articulated) example of how computational design methods can improve landscape architectural design development in a quite fundamental manner. It makes a case for particular tools as a necessary means to achieve a crucial level of precision when testing and evaluating a complex landscape design against a complex design goal. The validity and the results of this testing are still just information to be considered by the designers and their consultants, but the use of parametric models here can help to speed that testing and render more clearly the trade-offs between criteria.

![](/assets/projects/max-iv-laboratory/model.png)

![Grasshopper definition recreating the basic pattern effect that defines the topographic forms. *Philip Belesky, for http://groundhog.la*](/assets/projects/max-iv-laboratory/definition.png) d
