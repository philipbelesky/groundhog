## Release Check-list

#### Version Numbers

- Version number to be iterated in `plugin.md`
- Version number to be iterated in `download.html`
- Version number to be iterated in `AssemblyInfo.cs`
- Version number to be iterated in `groundhog readme.md`
- Version number to be iterated in the `gh_string` of each definition; search string for *Definition prepared for Groundhog v0.8.0b and depends on*
- Version number to be iterated in `package.json` + `package-lock.json`
- Version number to be iterated in `manifest.yml`
- Release date/notes in `CHANGELOG.md`

#### Models/Definitions

- Downsave models to Rhinoceros 5
- Update screenshots used on site
    - Check Full Names are off; fancy wires are on
    - Export as JPG with background 227, 222, 218; at 2X resolution (needs to be done in Rhinoceros 6)

#### Plugin/Website

- Rebuild `groundhog readme.pdf` from the Markdown source
- Run `compile_plugin.sh` in the site directory and commit the gha/pdf updates to source

#### Yak

In command prompt (not power shell):

    > "C:\Program Files\Rhino WIP\System\Yak.exe" build
    > "C:\Program Files\Rhino WIP\System\Yak.exe" push groundhog-X.Y.Z.yak
    > "C:\Program Files\Rhino WIP\System\Yak.exe" search groundhog

#### Github

- Upload plugin zip
