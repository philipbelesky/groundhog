---
title:      MAX IV Laboratory Landscape
date:       17-11-14
excerpt:    Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab LIPSUM
thumbnail:  4.jpg
year:       2016 (constructed)
location:   Lund, Sweden
designers:  Snøhetta
files:      true
files_text: model and definition for this project
categories:
- parametric geometry
- simulation
tags:       grasshopper,
---

{% include figure.html name="7.jpg" caption="The MAX Lab facility uses a rippled spiral of topographic form to surround the main building." credit="Image via Snøhetta website's project page (https://snohetta.com/projects/70-max-iv-laboratory-landscape)" %}

In designing this new scientific facility a major concern was that external vibrations from a nearby highway would disrupt the measurements from sensitive laboratory instruments.[@Snohetta:2016us 1] The site's pre-existing topography — a flat slope — heightened this concern as the large planar surface exacerbated the vibrations.[@Snohetta:2016us 2] Thus a design goal to maximising the landscape's surface area through a rippled topography that would scatter vibrations and provide ancillary benefits such as to "manage the water run-off and mass balance on site:"[@Snohetta:2016us 2]

{% include figure.html name="3.jpg" caption="The topographic form was designed using a series of geometric projections offset from the ring-shape of the main building." credit="Image via Snøhetta via press release 'The MAX IV Laboratory Landscape Design by Snøhetta to Open Summer 2016.'" %}

> "3D-modelling proved crucial for several reasons. The design layout was established by extracting the nature of vibrations into rational values inserted in a generic model (Grasshopper; a Rhino plug-in). In plan, intersecting tangents radiating from the major storage ring form the first basis of the wave pattern. These align with the positions of potential future laboratories, and the starting points were defined by 10 to 40m vibration wavelengths and a 4.5m amplitude. ... Our digital model enabled continuous testing of the pattern's effect on mitigating the ground vibrations."

Several Grasshopper definitions were used across the project. In the main definition used to develop the base landform, the adjacent vibrations were implemented as a parametised constraint honed over many iterations in conjunction with an engineering team.[@Walliss:2016vy 39] Once set, this constraint allowed the design team to assess the dampening effects of the topographic form and fine-tune the form to improve it. The results of this form could then be used to analyse and evaluate secondary design criteria that were also heavily dependent on the underlying landform using other definitions that would simulate wind conditions, inform tree planting, visualise a maximum slope gradient, and measure stormwater drainage and retention.[@Walliss:2016vy 37]

{% include figure.html name="4.jpg" caption="A rolling series of troughs and peaks act to damped vibrations from the adjacent roads." credit="Image via Snøhetta  press release 'The MAX IV Laboratory Landscape Design by Snøhetta to Open Summer 2016.'" %}

In this project parametric geometries drive the key feature of the design — its topography — according to quantitative performance assessments.[^4] As compared to the previous project discusses, the design process for the MAX Lab IV landscape presents a clearer (or perhaps just more clearly articulated) example of how computational approaches can improve landscape architectural design development. It makes a case for computational tools as a necessary means to achieve precision when testing and evaluating a complex landscape design against a complex design goal. Using separate definitions to measure each performance criteria does place the designer as an agent that must intuit trade-offs between performance criteria, whereas a cross-functional definition would have opened up the possibility of a pareto-optimal solution that made these trade-offs more explicit, or at least able to be represented by the design's parameters.

{% include figure.html name="6.jpg" credit="Image via Snøhetta  press release 'The MAX IV Laboratory Landscape Design by Snøhetta to Open Summer 2016.'" %}


{% include figure.html name="model.png" %}

{% include figure.html name="definition.png" caption="Grasshopper definition recreating the basic pattern effect and planting distribution." credit="Philip Belesky, for http://groundhog.la" %}

[^4]: There is some uncertainty as to how well Grasshopper could test the magnitude of vibrations — the extensive consultation with engineering suggests it was not as simple as specifying a singular parameter; more likely that the definition's parameters correlated with, but could not precisely model, sonic vibration. Hence the need to iterative tweak and test the underlying model performance rather than creating a definition that could present an optimal outcome from the get-go.
