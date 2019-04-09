#!/bin/bash

# Zip up all the files and move them to the Jekyll assets folder for publishing
# Designed to be run from the site/Jekyll directory


# Plugin Icons
rm -rf ./assets/plugin/icons/*.png
cp ../plugin/Resources/**.png ./assets/plugin/icons/
ICONS=$(ls -1 "./assets/plugin/icons/" | wc -l | xargs)
echo "Extracted ${ICONS} Component Icons"


# Documentation Files
for i in ../docs/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r -q -j "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

rm -rf ./downloads/documentation/*.zip
mv ../docs/**.zip ./downloads/documentation/
DOCS=$(ls -1 "./downloads/documentation/" | wc -l | xargs)
echo "Moved ${DOCS} Documentation Files"


# Project Files
# Need to keep inner folders but not outer folders so cant use -j option; instead do some CDing
cd "../projects/"
for i in ./*;
    # Only do stuff if they are a directory
    do if [ -d $i ]; then zip -rq "${i%/}.zip" "$i"/* -x "*.DS_Store" -x "*Thumbs.db" && echo "$i"; fi
done
cd "../site/"

rm -rf ./downloads/projects/*.zip
mv ../projects/**.zip ./downloads/projects/
PROJECTS=$(ls -1 "./downloads/projects/" | wc -l | xargs)
echo "Moved ${PROJECTS} Project Files"


# NPM Dependencies
cp ./node_modules/@ibm/type/fonts/Sans/web/woff2/IBMPlexSans-Regular*.woff2 ./assets/fonts/
cp ./node_modules/@ibm/type/fonts/Sans/web/woff/IBMPlexSans-Regular*.woff ./assets/fonts/
echo "Moved Asset Files"


# Move plugin files
rm -f ./downloads/plugin/groundhog.zip
# Zip for distribution
zip -r -q -j ./downloads/plugin/groundhog.zip "../plugin/release/groundhog.gha" "../plugin/groundhog readme.pdf" -x "*.DS_Store*" -x "*manifest.yml*"
echo "Built and Moved Plugin Files"

