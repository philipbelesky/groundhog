---
title:      MAX IV Laboratory Landscape
date:       17-11-14
excerpt:    A defensive topography that creates an intricate series of rolling mounds to dampen vibrations from a nearby highway.
year:       2016 (constructed)
location:   Lund, Sweden
designers:  Snøhetta
files:      true
files_text: model and definition that demonstrate a partial recreation of this project
---

{% include elements/figure.html image='7' caption='The MAX Lab facility uses a rippled spiral of topographic form to surround the main building.' credit="Image via Snøhetta website's project page (https://snohetta.com/projects/70-max-iv-laboratory-landscape)" %}

In designing this new scientific facility a major concern was that external vibrations from a nearby highway would disrupt the measurements from sensitive laboratory instruments.[@Snohetta:2016 1] The site's pre-existing topography — a flat slope — heightened this fear as it enabled surface vibrations to travel freely.[@Snohetta:2016 2] Thus a key design goal for Snøhetta's design was to maximise the landscape's surface area through a rippled topography that would, in effect, better scatter the vibrations that might interfere with the work of the laboratory. At the same time, such an exuberant grading could provide some ancillary benefits such as managing  run-off.[@Snohetta:2016 2]

> "3D-modelling proved crucial for several reasons. The design layout was established by extracting the nature of vibrations into rational values inserted in a generic model (Grasshopper; a Rhino plug-in). In plan, intersecting tangents radiating from the major storage ring form the first basis of the wave pattern. These align with the positions of potential future laboratories, and the starting points were defined by 10 to 40m vibration wavelengths and a 4.5m amplitude. ... Our digital model enabled continuous testing of the pattern's effect on mitigating the ground vibrations."

Several Grasshopper definitions were used across the project. In the main definition that drove the base landform, the vibrations from the adjacent roads were implemented as a parametised constraint whose exact value could be honed over many iterations in conjunction with an engineering team.[@Walliss:2016 39] Once set, this constraint allowed the design team to then assess the dampening effects of specific topographic forms and fine-tune them. The resulting topographies could then be further analysed and evaluated according to secondary design criteria that were encapsulated in other definitions that would simulate wind conditions, inform tree planting, visualise a maximum slope gradient, or measure storm-water drainage and retention.[@Walliss:2016 37]

{% include elements/figure.html image='3' alt='Diagram of the Max Lab IV\'s geometry showing the spiralling patterns.' %}
{% include elements/figure.html image='6' caption='The topographic form was designed using an intersecting series of geometric projections that extend as tangents from the outer ring of the main laboratory building.' credit='Image via Snøhetta  press release "The MAX IV Laboratory Landscape Design by Snøhetta to Open Summer 2016."' %}

As compared to other projects discussed, the design process for the MAX Lab IV landscape presents a clearer (or perhaps just more articulated) example of how computational design methods can improve design development. The project makes a case for these tools as a necessary means to achieve crucial levels of precision when producing and testing a complex landscape design against a complex design goal. While the results of this process still need to be assessed by designers and their consultants, the use of parametric models here shows how computational approaches can help to speed that testing and make some of the trade-offs between design criteria more explicit.

{% include elements/figure.html image='model' alt='Rhinoceros model of the MAX IV Laboratory Landscape' %}
{% include elements/figure.html image='definition' caption='Grasshopper definition recreating the basic pattern effect that defines the topographic forms.' credit='Philip Belesky, for https://groundhog.la' %}
