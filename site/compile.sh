#!/bin/bash

# Zip up all the files and move them to the Jekyll assets folder for publishing
# Designed to be run from the site/Jekyll directory

# Build
# =====
MSBuild ../plugin/groundhog.csproj /property:Configuration=Release

# Extract Input/Output Parameters
# ===============================
rm -rf ../site/_data/components/ && mkdir ../site/_data/components/ # Delete old files
python ../docs/extract_params.py

# Plugin Files
# ============
zip -r -j ./downloads/plugin/groundhog.zip ../plugin/release/

# Plugin Icons
# ============

cp ../plugin/Resources/**.png ./assets/plugin/icons/

# Project Files
# =============

for i in ../docs/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r -j "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

mv ../docs/**.zip ./downloads/documentation/

# Documentation Files
# =============

for i in ../projects/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r -j "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

mv ../projects/**.zip ./downloads/projects/