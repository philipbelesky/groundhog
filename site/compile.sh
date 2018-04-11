#!/bin/bash

# Zip up all the files and move them to the Jekyll assets folder for publishing
# Designed to be run from the site/Jekyll directory


# Extract Input/Output Parameters
# ===============================
rm -rf ../site/_data/components/*.json # Delete old files
python ../docs/extract_params.py


# Plugin Icons
# ============
rm -rf ./assets/plugin/icons/*.png
cp ../plugin/Resources/**.png ./assets/plugin/icons/


# Project Files
# =============
for i in ../docs/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r -j "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

rm -rf ./downloads/documentation/*.zip
mv ../docs/**.zip ./downloads/documentation/


# Documentation Files
# ===================
for i in ../projects/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r -j "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

rm -rf ./downloads/projects/*.zip
mv ../projects/**.zip ./downloads/projects/


# NPM Dependencies
# ================
cp ./node_modules/@ibm/type/fonts/Sans/web/woff2/IBMPlexSans-Regular*.woff2 ./assets/fonts/
cp ./node_modules/@ibm/type/fonts/Sans/web/woff/IBMPlexSans-Regular*.woff ./assets/fonts/


# Build
# =====
MSBuild ../plugin/groundhog.csproj /property:Configuration=Release /verbosity:m


# Plugin Files
# ============
rm -f ../plugin/release/groundhog.gha.mdb
rm -f ../plugin/release/groundhog.pdb # Comes from VS build using release config
mv -f ../plugin/release/groundhog.dll ../plugin/release/groundhog.gha
rm -f ./downloads/plugin/groundhog.zip

zip -r -j ./downloads/plugin/groundhog.zip ../plugin/release/ -x "*.DS_Store*" -x "*manifest.yml*"
