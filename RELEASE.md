## Release Checklist

#### Version Numbers

- Version number to be iterated in `landing_home`
- Version number to be iterated in `download.html`
- Version number to be iterated in `AssemblyInfo.cs`
- Version number to be iterated in `groundhog.csproj`
- Version number to be iterated in `groundhog readme.md`
- Version number to be iterated in the `gh_string` of each definition; search string for *Definition prepared for Groundhog v0.7.1b and depends on*
- Version number to be iterated in `package.json`
- Release date/notes in `CHANGELOG.md`

#### Models/Definitions

- Downsave to Rhinoceros 5 versions
- Update screenhots used on site
    - Check Full Names are off; fancy wires are on
    - Export as JPG with background 227, 222, 218; at 2X resolution (needs to be done in Rhinoceros 6)

#### Plugin/Website

- Rebuild `groundhog readme.pdf` from the Markdown source
- Follow `compile.sh` and `s3_website` steps in `site/README.md`
