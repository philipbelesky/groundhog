#!/bin/bash

# Build the plugin itself and move/bundle the files over

# Build
# =====
MSBuild ../plugin/groundhog.csproj /property:Configuration=Release /verbosity:q


# Plugin Files
# ============
rm -f ../plugin/release/groundhog.gha.mdb
rm -f ../plugin/release/groundhog.pdb # Comes from VS build using release config
mv -f ../plugin/release/groundhog.dll ../plugin/release/groundhog.gha
rm -f ./downloads/plugin/groundhog.zip

zip -r -q -j ./downloads/plugin/groundhog.zip ../plugin/release/ -x "*.DS_Store*" -x "*manifest.yml*"
echo "Built and Moved Plugin Files"
