---
title:      Flow Paths
excerpt:    Components for creating projections of surface water flows.
date:       17-10-14
files:      true
files_text: model and definition that demonstrating the use of these components
---

{% include elements/figure.html image='1' caption='Surface water flow paths across a littoral region' credit='Image via Philip Belesky for the "Processes and Processors" project (http://philipbelesky.com/projects/processes-and-processors/)' %}

## Flow Paths

The "flows" components create naïve projections or simulations of surface water flows and provide further means to analyse the results of these calculations. The key component — the `FlowPath` — accepts a series of 'drop points' on a `Surface` or `Mesh` that become the starting locations of each hypothetical flow path. From there, each point samples the surface or mesh to determine its slope, which becomes a directing vector (i.e. one that points 'downhill'). Each point is then moved along this vector a pre-specified distance, forming a line. The end of this line then becomes the starting point for the next sample; creating a recursive process where flow paths assemble themselves as `Polylines` that grow through this series of descending jumps.

This 'gradient descent' process repeats until a path crosses the edge of the form, a specified quantity of iterations are performed, or until the algorithm determines that the path has reached a point without a viable further downhill path. This halting calculation aims to identify a 'basin' where water might collect and pool rather than continue to flow downhill.

The component then produces as an output a series of `Polylines`, from which the beginnings, ends, and individual segments can be readily extracted. The process provides degrees of flexibility. By accepting any given set of `Points` (rather than enforcing a spatial grid or other formation) it offers the ability to work across a number of contexts, from situations where you may want to simulate a uniform distribution (say rain) or just a particular point-source of water.

The `FlowPath` component takes two forms a `SurfaceFlowPath` and a `MeshFlowPath` depending on the geometric type of the 'landscape' you want to test.

{% include elements/component.html title='SurfaceFlowPath' %}
{% include elements/component.html title='MeshFlowPath' %}

## Workflows

Once calculated, these flow paths can then be used as inputs for two further components:

- See [documentation for the Catchments component]({% link _documentation/flows-catchments.md %})
- See [documentation for the Saturation component]({% link _documentation/flows-saturation.md %})

The example file (linked at the top of this page) demonstrates a number of options for visualisation and extension, such as:

- Using a `Metaball` component to display (a very crude) 'pooling' effect at the end of the flow paths
- Using geometric intersections to test how drainage pits intercept water flows
- Fading the color of the paths as they travel further from their 'source'

{% include elements/figure.html image='model' alt='Example model for the flow paths definition.' %}
{% include elements/figure.html image='definition' caption='Grasshopper definition for the flow paths definition.' credit='Philip Belesky, for https://groundhog.la' %}
