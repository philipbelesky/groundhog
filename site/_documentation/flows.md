---
title:      Flows
excerpt:    Tools for projecting and analysis surface water flows
date:       17-10-14
files:      true
files_text: model and definition that demonstrating a partial recreation of this project
thumbnail:  1.jpg
---

Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. LIPSUM

{% assign component = site.data.components["SurfaceFlowPath"] %}
{% include elements/component.html %}
{% assign component = site.data.components["MeshFlowPath"] %}
{% include elements/component.html %}

{% include elements/figure.html name="1.jpg" caption="Surface water flow paths across a littoral region." credit="Image via Philip Belesky for the 'Processes and Processors' project (http://philipbelesky.com/projects/processes-and-processors/)" %}

Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. LIPSUM

{% assign component = site.data.components["FlowCatchment"] %}
{% include elements/component.html %}

Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. LIPSUM
