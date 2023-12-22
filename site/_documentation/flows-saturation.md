---
title:      Flow Saturation
excerpt:    Using surface water flow paths to calculate gradients of infiltration and saturation.
date:       20-08-02
files:      true
files_text: model and definition demonstrating the use of this component
---

The `FlowCatchment` component uses the collection of [pre-calculated flow paths]({% link _documentation/flows.md %}) to calculate surface infiltration across a given landform. These calculations allow for estimates of infiltration volumes across a given landform and the flow volumes collected in a landform's basins or edges. In many cases, the same methods will also be applicable to roofs and other surfaces.

To perform this calculation, the component:

1. Extracts all of the points that make up each of the provided flow paths.
2. For each path-point it identifies the nearest segment of provided mesh or surface.
3. That segment has a 'volume' of water added to a running tally (minus any loss).
4. As all the points are processed, segments adjacent to more flow paths will have higher tallies.
5. The segment boundaries and volume tallies are provided as outputs for visualisation or further processing.

The amount of volume that is added to each segment is controlled by two parameters: a *start volume* and a *segment loss*.

The `Start Volume` parameter is a 'budget' of water that each flow path draws from. Assuming that flows paths are evenly-distributed across the landform, this 'volume' is the (metaphorical) rain drop's size and so represents the duration and intensity of the water load.

The `Segment Loss` parameter is the rate of loss that occurs at each point within the flow path. So if a path begins with a `Start Volume` of `1000` and a `Segment Loss` of 0.01, each point within the path will add 10 (i.e. 1%) of its volume to the nearest mesh/surface segment and subtract 10 from the volume allocated to that flow path. When the flow path ends it discharges all of the remaining volume (if any) at its end point.

This parameter essentially models the infiltration rate of runoff across the landform. If the loss rate is set to 0, the surface is treated as impermeable — all of the `Start Volume` will be allocated to the end-point of each flow path.

> If `Segment Loss` is set to 0 then the `Start Volume` value will always be used at each point. E.g. the `Start Volume` is not treated as a budget, but rather as a per-point water load.

{% include elements/component.html title='FlowSaturation' %}

## Workflows and Examples

{% include elements/files.html %}

The example file for this component demonstrates a number of options for visualisation and extension, such as:



{% include elements/figure.html image='model' alt='Image of the flow saturation component used across two hypothetical landforms' %}

{% include elements/figure.html image='definition' caption='Grasshopper definition demonstrating how to use and extend the catchment analysis for Surface and Mesh forms.' credit='Philip Belesky, for https://groundhog.philipbelesky.com' %}
