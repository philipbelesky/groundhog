---
title:      Flow Catchments
excerpt:    TODO
date:       18-10-14
files:      true
files_text: model and definition that demonstrating the use of this component
---

{% include elements/figure.html image='1' caption='Surface water flow paths across a littoral region' credit='Image via Philip Belesky for the "Processes and Processors" project (http://philipbelesky.com/projects/processes-and-processors/)' %}

The first component for this is `FlowCatchment`. It uses the collection of pre-calculated flow paths to identify different catchment areas. To do so, it classifies each flow path into groups depending upon which paths finish or 'drain' into the same approximate location. This grouping is visually represented using a Voronoi diagram with each cell centred on the original `Pts` used as the 'start' of each flow path. Once formed, adjacent Voronoi cells whose origins drain to the same end location are joined together to form 'catchment' boundaries. Additionally, the different catchment groups are provided with a distinct color code and their cells/paths are output as distinct branches to aid further visualisation or analysis.

{% include elements/component.html title='FlowCatchment' %}

{% include elements/figure.html image='model' alt='Image of the flows component used across two hypothetical landforms' %}

{% include elements/figure.html image='definition' caption='Grasshopper definition demonstrating how to use the flow and catchment analysis for Surface and Mesh form.' credit='Philip Belesky, for https://groundhog.la' %}
