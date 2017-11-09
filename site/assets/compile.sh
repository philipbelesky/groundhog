# Zip up all the files and move them to the Jekyll assets folder for publishing
# Designed to be run from the site/Jekyll directory

# Project Files
# =============

for i in ../projects/*;
    # Only do stuff i fthey are a directory
    do if [ -d $i ]; then zip -r "${i%/}.zip" "$i" -x "*.DS_Store"; fi
done

mv ../projects/**.zip _site/assets/projects/downloads