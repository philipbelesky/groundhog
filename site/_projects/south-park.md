---
title:      South Park
date:       18-11-22
thumbnail:  thumbnail.jpg
excerpt:    Computational tools testing the resilience of analogue rules
year:       2017 (constructed)
location:   San Francisco, United States of America
designers:  Fletcher Studio
files:      true
files_text: model and definition that demonstrate a partial recreation of this project
---

{% include elements/figure.html image='south-park/1.jpg' alt='Aerial photograph of South Park.' %}
{% include elements/figure.html image='south-park/2.jpg' caption='The design of the park utilises a \'path finding\' tool which uses data collected on site to draw the path towards important areas.' credit='Image via Fletcher Studio\'s project page (https://www.fletcher.studio/southpark)' %}

When Fletcher Studio first began work on San Francisco's South Park, the initial design not was developed using computational scripting tools but "through iterative analogue diagramming"[@Fletcher:2018 72] made on paper with a focus on "an intuitive understanding of the site and embedded in an analogue rule set."[@Fletcher:2018 72] This rule set was derived from on-the-ground observations. Designers observed and collected data on "land use, park usage, circulation patterns, tree conditions and drainage systems"[@Fletcher:2018 72] as well as "points of entry and desire lines."[@Fletcher:2018 72] This data was aggregated into a "hierarchy of circulation patterns, access points, social nodes, existing trees and structures to retain"[@Fletcher:2018 72] that worked to inform the width and position of the central path running through the park.

While the designers implemented a 'rules based' approach even from the earliest stages of site analysis, they also made of a point of "utilizing a combination of blend tools, manual adjustments, and hand drawings"[@Fletcher:2018 72] which "allowed for idiosyncratic moments while conforming to a robust formal rule set based on environmental, spatial and material logic."[@Fletcher:2018 72] The design team made a special point of working with computational tools only when there was a clear need.

{% include elements/figure.html image='south-park/3.jpg' caption='Control points of varying intensity are parametrised to manipulate the alignment of that path which runs through site.' credit='Fletcher Studio, image from Bentley, Chris, "Follow the Script", Landscape Architecture Magazine 107, no. 7, 2016, p. 72' %}

Analogue rule sets are not — however — without problems of their own. Not only do they take a long time to test, but the very same 'idiosyncratic moments' that allow for the emergence of new and exciting design moves can also lead designers to overlook inconsistencies or failings in their logic.

This is where Grasshopper came in for the designers of South Park. They translated the system of organisation they developed on paper and in the field into a Grasshopper script and used this to test "the design resiliency" of their already developed "tectonic and spatial systems".[@Fletcher:2018 73] Through this process, Fletcher Studio were able to observe their rule set operating around a range of conditions more rapidly than would have been possible manually. This, in turn, allowed the designers not only to easily pick up and iron out any glitches and kinks in their logic, but also to iterate incredibly rapidly, at a high level of detail, all while staying squarely within the already established constraints of their design.

{% include elements/figure.html image='south-park/4.png' caption='Parametric scripting allows for the rapid generation of highly detailed design iterations.' credit='Fletcher Studio, image from David Fletcher, Parametric and Computational Design in Landscape Architecture, Bradley Cantrell & Adam Mekies (Routledge, 2018), p. 72' %}

By translating their rule set into grasshopper and refining it through iterative testing, Fletcher Studio eventually developed what they call a "live model"[@Fletcher:2018a 39] which:

> "was responsive, in the sense that various 3-D parameters could be modified and would universally update the entire model. Paving tablet width, length and distribution could be adjusted by modifying inputs, allowing the entry of exact values, or perhaps more intuitive site specific adjustments."[@Fletcher:2018a 39]

This 'live model', in turn, offered more opportunities later in the design process. After the resilience of the initial analogue design parameters was tested, parametric design techniques went on to serve as a powerful aid for time-saving when working through the implications of design alterations. For example, "introducing a slight slope to improve drainage in one area... would create a problem in the site 200 feet away" and in being able to quickly pinpoint this, "the program helped them \[the designers\] juggle interdependent aspects of the design."[@Bentley:2016a 68]

Parametric methods also presented significant benefits when it came to detailing and the associated rapid production of sheet files for engagement with stake-holders. The landscape architect's noted that the Grasshopper definition had a powerful ability to work both in broad strokes and with minute detail simultaneously: "updates to wall profiles, thickness, edge radii and even the distribution and frequency of skate deterrents were automated."[@Bentley:2016a 68] This then allowed not only for a reduction in time spent on slow and repetitive drawing, but also for the designers to produce outputs even as complex and detailed as full sheet file sets, much faster than would otherwise be possible.

Fletcher Studio's South park is a project that uses Grasshopper to supplement and support analogue logic rather than to drive the design. The designers have seen the huge potential of computational tools, but are wary of these same tools becoming the be all and end all of a project. "Memory, experience, emotion and humour" writes David Fletcher, almost as a warning at the end of a recent article, "are not yet parameters that can be put into a parametric definition."[@Fletcher:2018]

### Reference Model

In this next section we use Grasshopper to speculatively reverse-engineer the path finding rule-set developed by Fletcher Studio for this project. We also divide the remaining space into lawns and garden beds based on the same rules, and give an example of how Groundhog's [planting components]({{ site.baseurl }}{% link _documentation/plants.md %}) might prove useful in this context.

##### Step 1

This first section of the script uses data from site to interpolate a line through the park pulled toward various data points according to their perceived importance. Here, for simplicity's sake, the data points are divided simply into 2 sets for 'more important' and 'less important' and are visualised with a set of larger and smaller circles. Thereafter:

1. The initial 'base line' running through the centre of the park is divided up into points
2. Vectors are created between these points and the centroid of the closest given number of data (attractor) points (circles)
3. The points are then offset along this vector a given distance and the curve is then rebuilt
4. This process is run twice, once with higher amplitude vectors for the 'more important' data points and again with vectors of a smaller amplitude for the 'less important'. The resultant line trends toward the various data points at differing levels of intensity.

{% include elements/figure.html image='south-park/step_1.jpg' caption='A straight base line is drawn toward control circles by vectors which warp and re-align it.'  credit='Albert Rex, for groundhog.la' %}

##### Step 2

This second step generates offsets from the initial line that will define the width of the path. These offsets are also influenced by the same input data or 'attractor points' discussed in step 1; essentially repeating the same operation. However, this iteration only includes the attractor points on 1 side of the park so that one line is offset in one direction and another in the opposite. We see here that the path grows wider when it is closer to more powerful attractor points — doubly so when points are present on both sides.

{% include elements/figure.html image='south-park/step_2.jpg' caption='A path is offset out from the initial line. Offset distances also dependant on control points.'  credit='Albert Rex, for groundhog.la' %}

##### Step 3

Here we use the lines generated in steps 1 and 2 to produce a detailed paving system which is responsive to changes in data input. While straight lines are lofted off the initial base line, the 2 offset lines generated in step 2 are used as cutters to fit these lines to the curve of the path. These lines are then offset, capped and filleted to replicate the shape of the pavers in the original design. The script gives its user control of paver width, spacing and finally trims them to the boundary line of the park.

{% include elements/figure.html image='south-park/step_3.jpg' caption='A grid allows for the introduction of paving over the designated path area. A site boundary curve clips this paving to stay within site.'  credit='Albert Rex, for groundhog.la' %}

##### Step 4

The shape of the paved area of the park is subtracted from the total area, leaving us with an area which we are free to divide up between garden beds and lawns.

{% include elements/figure.html image='south-park/step_4.jpg' caption='Area not taken up by the path is identified.'  credit='Albert Rex, for groundhog.la' %}

##### Step 5

This step divides the remaining area found in step 4 into Garden Beds and Lawns. The size and shape of garden beds is also determined by the initial input data set: the attractor points of this definition. The beds are generated by offsetting the straight segments of the park boundary along the inverse of the vectors used in steps 1 and 2.

*Note: At this point we are stepping away from attempting to replicate the original design process and moving instead toward some of the latent potential we feel presents itself in the script.*

{% include elements/figure.html image='south-park/step_5.jpg' caption='Boundary offsets introduced to divide lawns and garden beds. Lawns: Green. Garden Beds: Green yellow overlap.'  credit='Albert Rex, for groundhog.la' %}

##### Step 6

Here a planting mix spreadsheet is imported and operated on by a series of Groundhog components to generate a rapid visualisation of potential garden bed layouts. This in turn could provide the base for various rapidly iterated light, shade and line of sight studies looking at the effects of growth in the garden beds through time. Default Groundhog grass visualisation have been used for lawns.

{% include elements/figure.html image='south-park/step_6.jpg' caption='Boundary offsets introduced to divide lawns and garden beds. Lawns: Green. Garden Beds: Green yellow overlap.'  credit='Albert Rex, for groundhog.la' %}

While perhaps more symbolic than actually programmatically useful on such a small scale, the ability of this definition to alter the direction and width of paths based on a particular data set has clear implications for larger scale operations. "In response to the observed resiliency of the rule set" writes David Fletcher, "we foresee application of this system to large scale linear open spaces...Potential sites for this application include: urban waterways, waterfronts, corridors, linear open spaces and right of ways".[@Fletcher:2018 75]

One can easily image (for example) how using a script like this, a walking path running along a kilometres long coast-line could automatically give wide girth to a stormwater outlet or road, while at the same time being drawn towards aesthetically pleasing areas or widening where higher densities of people are projected to move through.

{% include elements/figure.html image='south-park/step_7.jpg' caption='The script generates a path, garden beds and lawns, all based on site boundaries and usage data recorded on site'  credit='Albert Rex, for groundhog.la' %}

And of course, the ability to quickly iterate while tweaking data sets and incorporating additional features to a design is even more powerful at scales larger than that of a park. This is because when working at these larger scales, designers simply can't effectively develop 'intuitive' analogue understanding of site like the designers at Fletchers did at South Park. This is not a problem for a set of parametric relationships, which can be applied just as effectively over 100 meters as it can over 100 kilometres. Subsequent rapid iterations can then be curated by designers, who can use tools like Grasshopper as a crutch to help them understand how their designs can apply at scales of operation that would otherwise be near impossible to comprehend.

{% include elements/figure.html image='south-park/iteration_1.jpg' caption='The script still operates when site boundary and path are altered. From this we can infer that it is based on a relatively \'resilient\' rule set.'  credit='Albert Rex, for groundhog.la' %}

{% include elements/figure.html image='south-park/iteration_2.jpg' caption='The script is capable of operating over a wide range of environments. Here it regulates how a path moves through a forest.'  credit='Albert Rex, for groundhog.la' %}

{% include elements/figure.html image='south-park/iteration_3.jpg' caption='The script can be easily altered to accommodate additional, site specific. variations. Here an additional path is influenced by the original set of attractor curves and cuts under the first path, while at the same time the definition rebuilds garden beds around it.'  credit='Albert Rex, for groundhog.la' %}
