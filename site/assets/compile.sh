#!/bin/bash

# Zip up all the files and move them to the Jekyll assets folder for publishing
# Designed to be run from the site/Jekyll directory


# Plugin Files
# ============
zip -r -j ./downloads/plugin/groundhog.zip ../plugin/release/


# Project Files
# =============

for i in ../projects/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r -j "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

mv ../projects/**.zip ./downloads/projects/