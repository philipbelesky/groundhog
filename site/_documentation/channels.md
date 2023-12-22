---
title:      Channels
excerpt:    Components for deriving and analysing water flows within a sectional profile.
date:       19-07-31
files:      true
files_text: model and definition demonstrating the use of these components
---

The first channel component, `Channel Region`, determines a water level for a given section and a given area of water. It allows different channel geometries to be tested against a particular capacity of water and thus quickly gauge the efficacy of different profile geometries.

<!-- - Introduction to general hydraulic principles
- Describe process for deriving level from flow quantity; noting not that water does not strictly follow this process (i.e. settling effects) -->

{% include elements/component.html title='ChannelRegion' %}

<!-- - Description of the calculated attributes and their meaning/purpose
- More detailed discussion of manning formula and link to predefined values for channel materials (noting the uncertainty involved in using them) -->

The second channel component, `Channel Info`, calculates a number of hydraulic calculations from the sectional area determined by the `Channel Region` component. Most of these are geometric (e.g. *Mean Depth*) but a number of others can be calculated if the component is provided with a slope value and a [roughness coefficient](https://www.engineeringtoolbox.com/mannings-roughness-d_799.html) for the channel.

{% include elements/component.html title='ChannelInfo' %}

<!-- - Discussion of applications to design

## Workflows
-->

{% include elements/figure.html image='model' alt='Images of the channel tool applied to various geometries' %}

{% include elements/figure.html image='definition' caption='Grasshopper definition demonstrating how to use the channel region and channel profile components.' credit='Philip Belesky, for https://groundhog.philipbelesky.com' %}
