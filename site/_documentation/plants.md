---
title:      Plants
excerpt:    Components that enable the parametric control of plant selection, distribution, performance, and visualisation.
date:       17-11-08
files:      true
files_text: model and definition that demonstrating the use of these components
thumbnail:  thumbnail.jpg
---

If considered just in terms of their CAD representation, planting design appears to be an exercise in arranging circles. Some circles are smaller or larger, brighter or duller; more round or more frayed. But, removing the metadata that are the sprites, colours, and labels, we start and end with a representation of a particular species' dimensions at maturity: a disc.

It is regrettable that in both digital and analogue mediums the typical representations used poorly reflect their subject matter. Depictions of vegetation are rarely spatially explicit, and often rely on fixed and idealised averages that do not reflect the general nature, or the actual reality, of specific species.[@Elkin:2017ee 60-61][@Raxworthy:2013wa 113] A plan, once planted, will reach the 'mature' state it depicts after years if not decades. This mature state itself is itself an abstraction, as each plant's dimensions vary according to the localised condition that propel or constrain individual growth and are typically altered through ongoing maintenance regimes

{% include elements/figure.html image='plants/1.jpg' caption='Parametric methods of planting design can manage vast quantities of individual species distributed across a site and evaluate how they change over time.' credit='Philip Belesky, for http://groundhog.la' %}

While many options exist for visualising planting plans with a high degree of fidelity (presuming the correct models for a given species are available) these are typically deployed after the concept design stage, given that they are difficult to implement and modify. As a result they are often ill-suited to design exploration, but useful for evaluating aesthetics.

However the lack of 'accurate' models restricts those earlier stages of the design process, if working in orthographic projections, to the most abstracted versions of a planting plan. This constricts the design potential for vegetated elements in a design, both in terms of their broad visual character, but also in terms of any performance metrics available. Particularly important is the nature of plants as one of the most clearly-evolving elements of a landscape, where growth heavily effects the aesthetics of a space, and so must be planned across different time periods if the space is to be successful before each plant reaches maturity.

Several components in the Groundhog plugin work in conjunction to explore how some aspects of planting design can be eased, or approached differently, using parametric methods. To do so, it augments traditional forms of representations (points and circles) with metadata that describes that spatial and performance characteristics of a particular species. These attributes can then be used to inform both the locations of the plants in an automated manner, or be used as analytic tools for better understand how a given plant distribution performs according to specific criteria or over a particular time period.

Its primary input in establishing species attributes is as a spreadsheet; namely namely a CSV file with tables of information that are then read-in to Grasshopper. It contains a number of 'core' and optional parameters for each species that are needed to (later) produce a minimum-viable depiction of species typical geometry. These values (and example species) are available in the `Groundhog - Plants Examples.csv` file within the demo files attached to this post. This spreadsheet also provides option for extensibility, where arbitrary values can be added according to particular design intents, such as a value representing phytoremediation potential, or wind breaking potential, etc.

{% include elements/component.html title='PImport' %}

A second component contains predefined values for generic forms of vegetation, such as 'grass' or 'shrub' that enable rapid design prototyping without the need to specify detailed planting characteristics. Similarly, a component is provided for constructing one-off species representations in Grasshopper using explicit parameters, such as for defining canopy radii.

{% include elements/component.html title='PGeneric' %}

Regardless of which component is used the result is a simple textual representation of the species list, where characteristics use a simple `key:value` format. This allows the list of species to interact with standard list management tools in Grasshopper for adding/removing/combining species depending on a given logic.

The next step is related to how plants are distributed across space. This process can proceed using a number of different methods, given the varying relationships between spatial condition and species attributes that drive different patterns of distribution. At its most simple, the distributor can take a given series of spatial points, developed in Rhinoceros or in Grasshopper, where pre-existing tools allow for common patterns such as grid or radial patterns, as well as more complex options, such as where planter elements are defined elsewhere in the design and planting locations depend directly on these geometries.

Once a location (in the form of a `Point`) has been generated for each instance of a species (in the form of the list) these can be fed into the `Appearance` component. This then allows for key geometric features of each individual plant to be projected at a particular point in time. At present these visualisation methods are limited to basic circular depictions for criteria such as heights or trunks, root, and canopy radii. While offered as flat linework, many existing grasshopper tools can be used to give them volume but filling in interior values, such as when returning canopy circles into spheres can be used to test shading or trunk circles into cylinders to check visual occlusion.

{% include elements/component.html title='PShower' %}

While the components are relatively simple here in their calculations (especially given the currently-released set of components available) their value is in enabling quantitative criteria to be more easily used in designing and assessing vegetation distributions. The tripartite attribute/placement/simulation stages have emerged from extensive iteration in testing how to best support planting design workflows by best allowing each task to easily interface with the existing methods of generation and analysis available in Grasshopper.

{% include elements/figure.html image='plants/definition.jpg' caption='Grasshopper definition demonstrating how to select particular species, place them, and simulate basic growth characteristics.' credit='Philip Belesky, for http://groundhog.la' %}

> ***Coming Soon**: further components that allow for more naturalistic or performance-based planting distribution and 3D visualisation methods.*