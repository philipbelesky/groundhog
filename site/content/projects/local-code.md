+++
title=      "Local Code"
[extra]
date=       18-01-31
published=  false
excerpt=    "lorum"
year=       "TODO"
location=   "TODO"
designers=  "Nicholas de Mochaux and..."
files=      true
files_text= "model and definition that demonstrate a partial recreation of this project"
+++

{% include "elements/figure.html" image='3' caption='TODO' credit='(Image from competition boards http://wpa2.aud.ucla.edu/info/index.php?/theprojects/)' %}

Over the three years of his *Fake Estates* project, Gordon Matta-Clark identified 15 marginalised lots in New York; selecting largely vacant parcels that could be appropriated as community facilities.[@Mochaux:2010vq 89] Nicholas de Mochaux's employs a broadly similar methodology using geospatial analysis and parametric design to identifying 1625 vacant lots in San Francisco and propose new uses for each.[@Mochaux:2010vq 90] Such lots are often 'unaccepted' streets that the municipality owns and nominally uses as thoroughfares, but are not actively maintained and are often untraversable.[@Mochaux:2010vq 90] Taken collectively, *Local Code* investigates how such a large and distributed collection of small sites can nevertheless act to address city-wide issues.

{% include "elements/figure.html" image='4' caption='TODO' credit='(Image from competition boards http://wpa2.aud.ucla.edu/info/index.php?/theprojects/)' %}

The mapping process starts with a database identifying the unused lots, and relating this site data in relation to existing datasets for the city.[@Mochaux:2010vq 90] From there the conditions local to each particular site Werner investigated using a variety of simulation techniques for understanding thermodynamics, drainage, wind, and insolation phenomena at a local level. [@Mochaux:2010wx 238]

This data then informs a subsequent process of parametric design development that produces a proposal unique to each site and optimised to address its local conditions. Working within Rhinoceros/Grasshopper, the parametric system operates primarily through small topographic manipulations, distributing hard- and soft- scape surfaces, and distributing vegetation. These act to precisely mediate "air quality, drainage, and energy loads"[@Mochaux:2010wx 238] and feed into a secondary model that quantifies the funding opportunities available at each location as well as the benefits that each site offers as compared to traditional infrastructures.[@Mochaux:2010wx 240] As a whole the network acts as "an archipelago of opportunity, resistant to traditional forms of design, but open to more novel modes of speculation."[@Mochaux:2010vq 90]

{% include "elements/figure.html" image='1' caption='TODO' credit='Image from Mochaux, Nicholas de. “Local Code: Real Estates.” *Architectural Design* 80, no. 3 (May 30, 2010): 91' %}
{% include "elements/figure.html" image='2' caption='TODO' credit='(Image from competition boards http://wpa2.aud.ucla.edu/info/index.php?/theprojects/)' %}

While the proposal here aims to address pressing needs such as poor stormwater drainage and the urban heat island effect, the methodology (since applied to other cities) also aims to provoke a more thorough consideration of geospatial technologies within design practice.[@Mochaux:2010vq 90] Mochaux highlights that while data-driven architectural design often makes use of environmental data and analysis (such as say solar conditions) there use of cartographic data is under-appreciated relative to other fields where it is a crucial and ubiquitous resource.[@Mochaux:2010wx 237] While the project begins with an established process of GIS-enabled data gathering and analysis, its novelty is in connecting this large-scale data set to much smaller-scale design outputs.

That is to say the shift from a GIS platform to a parametric design platform here highlights a number of issues and opportunities. Firstly, that GIS platforms as-are — or assuming an imagined 'geodesign' capacities — are still primarily tools for very large scale considerations that at some point require a transition to different software in order to develop more concrete proposals at a human scale. In a similar manner, it seems like this transition (to Rhino/Grasshopper in this case) was also not just an opportunity to develop design features, but also to run a subsequent series of simulations at that more local level. Many methods exist for transitioning the large-scale information of GIS into design environments, however there are fewer options and less demand for transferring data in the other direction. <!-- TODO: check this --> This in turn raises the possibility of developing this large-scale analysis within smaller-scale software (which seems more viable than doing the reverse) and having a single environment in which data, analysis, and design generation can occur simultaneously across scales. While Local Code makes heavy use of GIS-as-software it also highlights how its limitations, and points towards the potential instead of GIS-as-methodology that can build feedback loops across all scales relevant to a design.
