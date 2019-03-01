---
title:      Terrain
excerpt:    Components for analysing different elements of landform.
date:       18-05-01
files:      true
files_text: model and definition that demonstrating the use of these components
thumbnail:  thumbnail.jpg
---

Landform is more heterogeneous and complex than contour lines suggest. Seemingly-similar areas of terrain can possess quite distinct characteristics depending on relatively small variations in their grading and position within the wider landscape. Digital models of landscapes often obscure this variety and make it difficult to determine or apprehend these characteristics given the artificial manner in which they are rendered.

Groundhog provides a number of components for measuring particular characteristics of a given landform. However its worth noting that, as above, such tools for classifying topographic features are only as good as their underpinning 3D representations. Representing a landform as (say) either a `Mesh` or a `Surface` will create different trade-offs in the types of accuracy and detail offered.

{% include elements/figure.html image='1' caption="Visualisations of slope analysis across a `Mesh`, showing each face's grade as a vector, fill, and label" credit='Philip Belesky, for http://groundhog.la' %}

The slope analysis components operate by identifying the normals of each face within the (land)form and measuring those vectors against the Z-axis. This produces a measure of steepness that can be output as either an angle or percentile. Either output can then be filtered and visualised to assist in grading tasks that may need to ensure slopes stay within a particular range (say to ensure accessible circulation) or to highlight areas that are vulnerable to erosion or require stabilisation.

{% include elements/component.html title='MeshSlope' %}

The aspect analysis components operate in a similar fashion, but measure the faces' normals relative to a specified vector. This vector defaults to the Y-axis (assuming this is the North direction) and so produces a measure of which direction a slope faces. This can be used to identify areas within the landform that have a particular aspect, e.g. those that are predominantly north-easterly, and help determine the micro-climates of different areas of the landscape (based on their different levels of solar insolation) or to determine their visibility relative to a given vantage point.

{% include elements/component.html title='MeshAspect' %}

Note that each component calculates the specified values, but does not visualise them within the model itself. For visualisation purposes, you typically want to translate the raw outputs of the slope or aspect components into colors by using the `Gradient` component and then a `Preview` component that matches the list to each individual mesh face. The reference definition provided at the top of this page shows an example of this process and several other visualisation options.

Components are provided (as a convenience) for employing the same analytics on a `Surface` rather than a `Mesh`, although note that the former will be converted to the latter during the actual calculation.

{% include elements/component.html title='SurfaceSlope' %}
{% include elements/component.html title='SurfaceAspect' %}

> ***Coming Soon**: further components that provide other metrics for assessing different terrain characteristics.*
