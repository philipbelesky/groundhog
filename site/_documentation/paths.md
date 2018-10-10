---
title:      Paths
excerpt:    Components for ...
date:       18-07-31
files:      true
files_text: model and definition that demonstrating the use of these components
thumbnail:  false
---

- Introduction to general hydraulic principles
- Describe process for deriving level from flow quantity; noting not that water does not strictly follow this process (i.e. settling effects)

{% include elements/component.html title='ShortestPath' %}

- Description of topology / shortest path algorithm

{% include elements/component.html title='RandomPath' %}

- Description of random path component
- Note link to ecologist's behavioural models

{% include elements/figure.html image='channels/model.jpg' %}

- Discussion of uses within design process
    - Understanding connectivity across a landscape/city etc
    - Understanding more abstracted 'connections' — i.e. river systems
    - Random path as useful for things like seed scattering etc

{% include elements/figure.html image='channels/definition.jpg' caption='Grasshopper definition demonstrating how to use the channel region and channel profile components.' credit='Philip Belesky, for http://groundhog.la' %}
