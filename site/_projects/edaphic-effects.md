---
title:      Edaphic Effects
date:       19-03-10
excerpt:    Parametric patterns produce 'incremental infrastructures' in-grade
year:       2011 (constructed)
location:   Philadelphia, United States of America
designers:  PEG Office of Landscape + Architecture
files:      true
files_text: model and definition that demonstrate a partial recreation of this project
tags:
---

{% include elements/figure.html image='edaphic-materials' caption='The parametrically-controlled geo-textile system following initial regrowth.' credit='PEG Office of Landscape + Architecture, via www.suckerpunchdaily.com/2011/12/26/edaphic-effects/' %}

The *Edaphic Effects* project demonstrates [PEG's](http://www.peg-ola.com/) fascination with patterns as a design driver and the value of parametric methods in rendering those patterns as form and material. "If ecology and systems are common frameworks used to describe the constellations of relationships that we see in the world" write Karen McCloskey and Keith VanDerSys, "then patterns are the 'how', or the means by which we come to know, understand or express these relationships."[@MCloskey:2017 p. 6] They "exist outside such categorical distinctions as nature versus culture"[@MCloskey:2017 p. 6] while offering a framework that can tie ecological and infrastructural requirements without sacrificing aesthetic qualities.[@MCloskey:2017 p. 38]

The project aimed to demonstrate the potential of "incremental infrastructures" as a means of reducing the loads on Philadelphia's stormwater infrastructure. To do so, a dispersed network of geo-textiles was proposed as a means of increasing water retention across vacant lots. Unlike the straightforward layering of traditional geo-textiles, the prototype design uses a more complex pattern that sees the geometry and infill of each geo-cell carefully controlled on an individual basis.[@Walliss:2016 170] This, in turn, allows the material profile to be tailored to the needs of each site while also producing a uniquely expressive surface.

{% include elements/figure.html image='edaphic-iso' alt='The geotextiles are fitted to the surface water flows of the site.' credit='PEG Office, from Dynamic Patterns p. 70.' %}

To achieve this, PEG began by defining two consecutive forms of patterning.

First, they worked with a GIS-derived dataset to develop "surface patterns that are mapping forces"[@MCloskey:2016b] — a loosely topographic model designed to indicate areas on site where water would likely collect after rain. The usefulness of this process lies largely in its ability to easily translate a large quantitative data set into a clear and compelling representations.

Subsequently, the team developed a more geometric pattern based on the shape of standard plastic geo-cell organisations. Unlike standard geo-cells however, the size and shape of these cells was parametrically defined in a way that allowed them to gradually change shape and size in a manner corresponding to the above flow analysis. This direct link between a quantitative analysis and a subsequent qualitative geometric pattern serves to express "something about the flow on the surface through the top layer".[@MCloskey:2016b] The geo-cell structure is able to respond dynamically to areas where more or less water is expected to collect, potentially increasing water retention and at the very least graphically highlighting where it is likely to occur.

{% include elements/figure.html image='edaphic-fields' caption='A script calculateds the rough paths of surface water flows across site' credit='PEG Office, from Dynamic Patterns p. 70.' %}

It is interesting to note that PEG also increased the geometric complexity of this pattern by offsetting the cell shapes inwards. This introduction of this 'double wall' allowed for a higher fidelity in the pattern's response to its environment, but without requiring a huge amount of extra scripting.

In this project PEG give a clear example of how parametric patterning can prove a powerful aid in quickly translating valuable – but alone difficult to decipher – quantitative data sets into simple and responsive patterning systems that can be applied directly to site in a way that allows them to interact with and often times reveal something of these initial data sets and their effects. This was achieved through two consecutive parametric patterning exercises, where "one is a flow pattern based on structuring relationships" and "the other is a module based unit that could be used for construction"[@MCloskey:2016b]. Taken together, these work to define an inter-related rule set where quantitative and qualitative information sets are related recursively.

### Reference Model

The reference model and definition for this project (downloaded from the link at the top of the page) attempts to reverse-engineer the broader pattern developed for this project.

To begin, a surface is constructed by lofting a series of curves. The script then divides the resultant surface into a grid of point and generates a diamond grid between them. The cells of this grid warp to conform to the surface and thus provide an initial layer of site-specific geometry.

{% include elements/figure.html image='step_1' caption='A surface is lofted through a set of 4 base curves and a diamond grid is fitted across it.' credit='Albert Rex, for groundhog.la' %}
{% include elements/figure.html image='step_2' caption="The offset of each cell\'s inner wall (left) is controlled by each cell's vertical range (right)." credit='Albert Rex, for groundhog.la' %}

The script then measures the vertical distance between the highest and lowest point of each cell. This difference is taken as a rough interpolation of slope and the value is 'remapped' to influence the width of each cell and thus control cell dilation. That is to say, the steeper the angle of each cell, the more that its inner wall becomes offset from its outer. Note: this mechanism is used speculatively and the actual Edaphic project likely had a different means of articulating the surface that seems to have incorporated the results of a [gradient-descent method]({{ site.baseurl }}{% link _documentation/flows.md %}) alongside other analytics.

{% include elements/figure.html image='step_3' caption='While the first layer of patterning conforms to the topography of a 3D surface, the second responds in size to a specific environmental input (in this case, slope).' credit='Albert Rex, for groundhog.la' %}

The script has a versatility that allows it to be applied to a range of landscape conditions by re-configuring the underlying surface. This without even taking into consideration the additional variation possible when different data sets are introduced to influence the secondary offset pattern.

{% include elements/figure.html image='step_4' caption='The pattern has a versatility that allows it be applied to a wide range of sites/ shapes.' credit='Albert Rex, for groundhog.la' %}
