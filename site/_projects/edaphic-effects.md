---
title:      Edaphic Effects
date:       19-03-10
published:  true
excerpt:    lorum
year:       2011 (constructed)
location:   Philadelphia, United States of America
designers:  PEG Office of Landscape Architecture
files:      true
files_text: model and definition that demonstrating a partial recreation of this project
tags:
---

PEG Office of Landscape + Architecture are well know for a belief in the benefits of pattern-making as a design driver and for the use of scripting tools like grasshopper as a means of controlling and manipulating these patterns.

"If ecology and systems are common frameworks used to describe the constellations of relationships that we see in the world" write Karen McCloskey and Keith VanDerSys, "then patterns are the 'how', or the means by which we come to know, understand or express these relationships."[@MCloskey:2017 p. 6] They "exist outside such categorical distinctions as nature versus culture"[@MCloskey:2017 p. 6] and "can link ecological and infrastructural mandates placed on landscapes without forsaking formal and perceptual coherence".[@MCloskey:2017 p. 38]

Edaphic Effects is a landscape architecture project that clearly implements pattern in a compelling way. The project aims to increase water retention over the city's many vacant lots by implementing geo-textiles, where plastic sheets hold stones and pebbles in the ground to increase water retention. Not content however with standard and uniformly shaped geo-textiles readily available, PEG believed they could increase the efficiency of these patterns by defining and manipulating them parametrically.

<!-- TODO: replace with images from sucker punch? -->
{% include elements/figure.html image='edaphic-materials' caption='The parametrically-controlled geo-textile system following initial regrowth.' credit='TODO' %}

PEG began by defining two consecutive forms of patterning they intended to use in their design process.

First, they worked with a GIS derived dataset to develop "surface patterns that are mapping forces" lecture, a loosely topographic model/map designed to indicate areas on site where water would mostly likely collect during a rain event. The usefulness of this process lies largely in its ability to easily translate a large quantitative data set into a simple, visual representations.

{% include elements/figure.html image='edaphic-fields' caption='An initial scipt calculates rough storm-water flow paths over site. This will inform the shape and size of the geo-cells in the second section of the script.' credit='PEG Office, from Dynamic Patterns p. 70.' %}

Subsequently the team developed a more geometric pattern based on the shape of standard plastic geo-cell organisations.Unlike standard geo-cells however, the size and shape of these cells was parametrically defined in a way that allowed them to gradually change shape and size in a manner corresponding to the above flow analysis. This direct link between a quantitative analysis and a subsequent qualitative geometric pattern serves to express "something about the flow on the surface through the top layer" Lecture. The geo-cell structure is able to respond dynamically to areas where more or less water is expected to collect, potentially increasing water retention and at the very least graphically highlighting where it is likely to occur.

It is interesting to note that PEG also increased the geometric complexity of this pattern by offsetting the cell shapes inwards. This introduction of this 'double wall' allowed for a higher fidelity in the pattern's response to its environment, but without requiring a huge amount of extra scripting.

{% include elements/figure.html image='edaphic-iso' caption='A plan illustrates the secondary geometric pattern as it will be applied on site.' credit='PEG Office, from Dynamic Patterns p. 70.' %}

In this project PEG give a clear example of how parametric patterning can prove a powerful aid in quickly translating valuable – but alone difficult to decipher – quantitative data sets into simple and responsive patterning systems that can be applied directly to site in a way that allows them to interact with and often times reveal something of these initial data sets and their effects.  This was achieved through two consecutive parametric patterning exercises. "one is a flow pattern based on structuring relationships. The other is a module based unit that could be used for construction" Lecture, and together they work to define an inter-related rule set where quantitative and qualitative information sets are related recursively.

In the reverse engineering process below, we have simplified how we believe PEG may have developed their second, cell distributing pattern. We have also included a simple way to manipulate cell shape and size based on a range in input data sets.

### Reference Model

The reference model and definition for this project (link at the top of the page) attempts to reverse-engineer the ...

First, the script first lofts a surface through a series of curves. These curves do not have to be planar and can be representative of a real, 3d site. The script then divides the resultant surface into a grid of point and generates a diamond grid between them. The cells of this grid warp to conform to the surface, thus providing an initial layer of site-specific geometry.

{% include elements/figure.html image='step_1' caption='A surface is lofted through a set of 4 base curves, a diamond grid is then stretched over the top of this surface.' credit='Albert Rex, for groundhog.la' %}

The script then measures the vertical distance between the highest and lowest point of each cell. This is taken as a rough interpolation of slope. The value for this distance is 'remapped' to the width of each cell and used to control cell dilation. The greater the vertical range, the greater the dilation.

NOTE: It is at this stage where different environmental data sets can be substituted in for varying results.

{% include elements/figure.html image='step_2' caption="Dilation of each cell\'s inner wall (left) is controlled by a remapping of each cells vertical range (right). Cells with a greater vertical range (slope) will have a greater dilation" credit='Albert Rex, for groundhog.la' %}

After vertical range is calculated for each cell, this process generates a second layer of grid structure. This layer is responsive not only to the initial lofted surface input, but also to a secondary environmental data set, in this case, slope.

{% include elements/figure.html image='step_3' caption='While the first layer of patterning conforms  to the topography of a 3D surface, the second responds in size to a specific environmental input, in this case slope.' credit='Albert Rex, for groundhog.la' %}

The script has a versatility that allows it to be applied to a range of landscape conditions by re-configuring the underlying surface. This without even taking into consideration the additional variation possible when different data sets are introduced to influence the secondary offset pattern.

{% include elements/figure.html image='step_4' caption='The pattern has a versatility that allows it be applied to a wide range of sites/ shapes.' credit='Albert Rex, for groundhog.la' %}

{% include elements/figure.html image='model' caption='Rhinoceros model of the Edaphic Effects installation ' %}
{% include elements/figure.html image='definition' caption='Grasshopper definition recreating the general pattern used to create the geo-textile.' credit='Albert Rex and Philip Belesky, for https://groundhog.la' %}
