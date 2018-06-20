---
title:      Terrain
excerpt:    Components for quantifying different elements of topographic form.
date:       18-05-01
files:      true
files_text: model and definition that demonstrating the use of these components
thumbnail:  thumbnail.jpg
---

Topography is heterogeneous in a more complex manner than contours or a 3D visualisation often suggest. Different and seemingly-similar areas of landform can posses quite distinct characteristics depending on relatively small variations in their grading and wider position within the landscape. 3D models in particular often make it difficult to determine or apprehend these characteristics of digital terrain models given the  artificial manner in which they are rendered.

Groundhog provides a number of components for measuring particular characteristics of a given landform. However its worth noting that, as above, such tools for classifying topographic features are only as good as their underpinning 3D representations. Representing landform as (say) either a `Mesh` or a `Surface` has different trade-offs in terms of the different types of accuracy and level of detailed offered.

{% include elements/figure.html image='terrain/1.png' caption="Visualisations of slope analysis across a mesh, showing each face's grade as a vector, fill, and label" credit='Philip Belesky, for http://groundhog.la' %}

The slope analysis component operates by identifying the normals of each face within the form and measuring those against the Z-axis to produce a measure of steepness that can be output as either an angle or percentile. This can then be filtered and visualised to assist in grading tasks that may need to ensure slopes stay within a particular range (say to ensure accessible circulation) or to highlight areas that are vulnerable to erosion or require stabilisation.

{% include elements/component.html title='MeshSlope' %}

The aspect analysis component operates in a similar fashion but measures the faces' normals relative to a specified vector. This vector defaults to the Y-axis (assuming that is the North direction) and so produces a measure of which direction a slope faces. This can be used to identify areas within the landform that have a particular aspect, such as those that are predominantly north-easterly. This can be used to help determine the micro-climates of different areas of the landscape (based on their different levels of solar insolation) or to determine their visibility relative to a given vantage point.

{% include elements/component.html title='MeshAspect' %}

Each component calculates the specified values but does not visualise them in the model itself. For visualisation purposes you will typically want to assign the range of slope or aspect values to a colour using the `Gradient` component then use a `Preview` component to apply these colours to each individual mesh face. The reference definition provided at the top of this page shows an example of this process and several other visualisation options.

Components are provided (as a convenience) for employing the same analytics on a `Surface` rather than a `Mesh` although note that the former will be converted to the latter during the calculation.

{% include elements/component.html title='SurfaceSlope' %}
{% include elements/component.html title='SurfaceAspect' %}

> ***Coming Soon**: further components that provide other metrics for assessing different terrain characteristics.*