---
title:      Keio University Roof Garden
date:       17-09-14
excerpt:    A contemporary rooftop garden that uses a tiled field to recreate natural planting patterns.
year:       2012 (constructed)
location:   Tokyo, Japan
designers:  Michel Desvigne Paysagiste
files:      true
files_text: model and definition that demonstrates a partial recreation of this project
---

{% include elements/figure.html image='1' alt='Photographs of the Keio University Roof Garden' credit="Image via MBP website's project page (http://micheldesvignepaysagiste.com/en/keio-university-慶應義塾)" %}

The most visible impact of computational design techniques on the design of landscapes is often in the formal treatment of 'hard' surfaces or structures — street furniture, paving elements, pavilions, and other items. As manufactured and constructed artefacts, these elements can draw from the design and fabrication techniques typically developed in other disciplines.

For example, the Keio University Roof Garden employs a 'field' technique whereby the radii of a  parametrically-defined grid of circles grows in response to an image. This image is initially read as 2D grid of pixels that are then transposed on to the site model so that each grid point can 'sample' the brightness of the corresponding pixels on the original image. The apertures that result from these grid-samples become voids within the paving grid and are either used for planting beds or extruded as cylinders that function as seating.[@Gilles:2009 173]

The resulting aesthetic is one of a smoothly differentiated surface with semi-enclosed areas that reflects the form of a braided river in plan; an image used to drive the geometric layout:

> "One slips into this space, drifting along on the feelings aroused by the water and the light, playing on the same logic. There is no clear separation here (nor was there in Noguchi's garden) between voids and solids. This composition plays with successive planes and textures of variable densities. The even punctuation of the ground gives cadence to these variations. This is a small structure that organizes textures, porosities, densities, and transparencies—the material and the complex spaces, just as in a natural landscape." [@Gilles:2009 175]

{% include elements/figure.html image='2' caption='The different types of granite slab in terms of their dimensions and appearance in the resulting design.' credit='Image via "Intermediate Natures, The Landscapes of Michel Desvigne" (2009) p172' %}

The project's goals are a productive contradiction: a desire for a roof garden — a tightly bounded and highly sculpted landscape — that at the same time displays some of the rich variety and dynamism that characterise a traditional Japanese garden. The definition and model provided also demonstrate some of the capacity for variation inherent to the parametric model itself, as basic variables (such as tile depth,  dimensions, planting palette, etc) are easily modified. At the same time the use of the interpolated image map allows for a more expressive mode whereby the tile pattern can be altered by manipulating the source image by applying either filter effects (i.e. tweaking the overall brightness or contrast) or through specific edits (i.e. using brush tools in Photoshop).

{% include elements/figure.html image='model' alt='Rhinoceros model of the Keio University Roof Garden' %}
{% include elements/figure.html image='definition' caption='Grasshopper definition recreating the basic pattern effect and planting distribution.' credit='Philip Belesky, for https://groundhog.la' %}

{% include elements/files.html %}
