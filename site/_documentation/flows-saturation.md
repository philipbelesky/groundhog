---
title:      Flow Saturation
excerpt:    Using surface water flow paths to display saturation gradients.
date:       20-08-02
files:      true
files_text: model and definition that demonstrating the use of this component
---

The `FlowCatchment` component uses the collection of [pre-calculated flow paths]({% link _documentation/flows.md %}) to calculate the relative amount of surface infiltration across a given landform.

To do so, it takes the points that make up each of the flow paths and identifies the nearest segment of the mesh of surface. That segment then has a 'volume' of water added to it. Areas of the landform covered by many sets of flow paths will register as having a greater volume of water.

The precision of this volume is controlled by two parameters, The `Start Volume` parameter is a 'budget' of water that each flow path represents. The `Segment Loss` parameter is the rate of loss that occurs at each point within the flow path. So if a path begins with a `Start Volume` of `1000` and a `Segment Loss` of `0.1`, each point within the path will add `10` to the nearest mesh/surface segment and subtract `10` from the volume assigned to that path. When the flow path ends it discharges all of the remaining volume (if any) at its end point.

{% include elements/component.html title='FlowSaturation' %}

> If `Segment Loss` is set to 0 then the `Start Volume` value will always be used at each point.

{% include elements/figure.html image='model' alt='Image of the flow saturation component used across two hypothetical landforms' %}

{% include elements/figure.html image='definition' caption='Grasshopper definition demonstrating how to use and extend the catchment analysis for Surface and Mesh forms.' credit='Philip Belesky, for https://groundhog.la' %}
