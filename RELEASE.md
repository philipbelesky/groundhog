## Release Check-list

#### Increment Version Numbers

- Version number to be iterated in `plugin.md`
- Version number to be iterated in `download.html`
- Version number to be iterated in `AssemblyInfo.cs`
- Version number to be iterated in `groundhog readme.md`
- Version number to be iterated in the `gh_string` of each definition; search string for *Definition prepared for Groundhog v0.8.0b and depends on*
- Version number to be iterated in `package.json`
- Version number to be iterated in `manifest.yml`
- Release date/notes in `CHANGELOG.md`

#### Update Models/Definitions

- Downsave models to Rhinoceros 5
- Update screenshots used on site
    - Check Full Names are on; fancy wires are on
    - Export as JPG with background 227, 222, 218; at 2X resolution (needs to be done in Rhinoceros 6)

#### Update Plugin on Website

- Rebuild `groundhog readme.pdf` from the Markdown source
- Run `compile_plugin.sh` in the site directory and commit the gha/json/pdf updates to source

#### Update Plugin on Yak

In command prompt (not power shell) executed in the `plugin\release` directory:

    > "C:\Program Files\Rhino 6\System\Yak.exe" build
    > "C:\Program Files\Rhino 6\System\Yak.exe" push groundhog-X.Y.Z.yak
    > "C:\Program Files\Rhino 6\System\Yak.exe" search groundhog

#### Update Plugin on Github

- Upload plugin zip
