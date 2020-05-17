## Release Check-list

#### Increment Version Numbers

- Version number to be iterated in `_config.yml`
- Version number to be iterated in `AssemblyInfo.cs`
- Version number to be iterated in `groundhog readme.md`
- Version number to be iterated in the `gh_string` of each definition; search string for *Definition prepared for Groundhog v0.8.0b and depends on*
- Version number to be iterated in `manifest.yml`
- Release date/notes in `CHANGELOG.md`

#### Update Plugin on Website

- Rebuild `groundhog readme.pdf` from the Markdown source
- Run `compile_plugin.sh` in the site directory and commit the gha/json/pdf updates to source

#### Update Plugin on Yak

In command prompt (not power shell) executed in the `plugin\release` directory:

    > "C:\Program Files\Rhino 6\System\Yak.exe" build
    > "C:\Program Files\Rhino 6\System\Yak.exe" push groundhog-X.Y.Z.yak
    > "C:\Program Files\Rhino 6\System\Yak.exe" search groundhog

#### Update Plugin on Github

- Upload plugin zip (in the built site's downloads/plugin directory)
