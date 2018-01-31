---
title:      Sony Forest
date:       18-01-31
published:  false
excerpt:    lorum
thumbnail:
year:       2012 (constructed)
location:
designers:  ANS Studio
files:      true
files_text: model and definition that demonstrating a partial recreation of this project
tags:
- analog computation
---

ANS Studio developed a constraints-based parametric model — a "Seed Scattering System" — as a means to create a 'natural' distribution of plants for the garden surrounding an office building. The natural stipulation here is not understood in terms of an untended garden or of a naturalistic formal pattern, but as a distribution that locates plants and chooses species in a manner that replicates the end-result of the localised growth and succession processes that occur in a forest. This was not the sole focus though — the model made heavy use of parametised goals and constraints to allow for other design criteria to affect the distribution.

What distinguishes this project from many other approaches — such of that of Snohetta — is the sophistication of the modelling process and the use of the model as the key design driver spanning from site analysis to design documentation. Rather than using landscape form as the key site of design investigation (and have analysis performed in response to changes) the model itself embodied the process of form development, with the designer instead choosing amongst possible solutions and adjusting input weights.

![A model of plant growth was used to project the expected plant morphology over time. *(image from paper)*](/assets/projects/sony-forest/1.png)

The model itself performed a number of steps when creating a possible design. Broadly speaking the first phase was in identifying how environmental conditions, such as soil composition, building shading, and wind sheltering, affected different portions of the site. Follow from this the design logic was developed, whereby the designer could adjust parameter's values and possible layout patterns for how the plant placement would respond to the environmental conditions. Finally the system would take all of these into account to create the planting plan, with the algorithm's primary outputs being the  'seed' points that represented a plant with a particular spacing and species optimised to the given site conditions and design criteria.[@Takenaka:2012vn 431] The location of the pathway system occurs after this distribution (optimising to work around root systems).

![The design logic was able to reformulate the tiling and vegetation distributions according to desired entry paths. *(image from paper)*](/assets/projects/sony-forest/2.png)

This system had a number of benefits:

- By developing the bulk of the design within a parameter system the way in which planting plans were developed changed where they were not looking to "manipulate geometries or compositions of tree groupings but to design the fundamental rules that underlie them."[@Takenaka:2012vn 434-435]
- Doing so also forces an explicit tradeoff between performance-driven and aesthetic-driven design criteria through the weighted parameters of the model.
- Modifications to the design crtieria later on in the process can be easily accomodated by regenerate the design solution with the new parameters, such as when the entrance spaces needed to become more prominent.[@Takenaka:2012vn 432]

Because the design criteria could be encompassed in a relatively complete manner by choices in the vegetation (and their direct relationship to pathing) the design logic could be encapsulated neatly in a weighted model, particularly when both the initial and final site conditions were flat in formal terms. However, the use of a singular model becomes much more difficult as the design criteria become more multivalent, and come to encompass wider ranges of criteria. In these cases, and in cases where the design has more formal effects, the use of formal representations to link between different performances criteria becomes key. That is to say rather than have a single model encompass and generate a design from the first to last stages of the design process, formal representations (says surfaces in a CAD program) are used to store the results of these models or to negotiate between multiple models investigating different criteria.

Similarly, it is unclear how generalisable this particular model is to other projects. Often highly advanced models become overly tied to project-specific criteria, and so are difficult to transplant into other situations. That this particular model does not seem to be implemented in a common CAD environment means it is likely to require a large degree of expertise to adapt and deploy within other projects.