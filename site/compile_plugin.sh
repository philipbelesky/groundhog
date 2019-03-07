#!/bin/bash

# Note the plugin zip file is intended to be committed to src
# As are the json files

# Build the plugin itself
MSBuild ../plugin/groundhog.csproj /property:Configuration=Release /verbosity:q
echo "Build plugin gha"

# Extract input/output parameters from plugin file to json for Jekyll docs
rm -rf ../site/_data/components/*.json # Delete old files
python ../docs/extract_params.py
echo "Extracted components JSON"

