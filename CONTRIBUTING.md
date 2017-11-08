## Contributing to Groundhog

Note: this document is still in a draft state.

#### Reporting Bugs

Bugs are reported using Github Issues. Before creating bug reports, please check the existing list as you might find out that you don't need to create one.

When creating a bug report, please include as many details as possible. This should probably include links to a model and definition that display the bug encountered.

#### Suggesting Enhancements

Enhancements are suggested using Github Issues.

#### Writing/Editing/Modeling Contributions

As below with 'Code Contributions'. Note that most of this site's content is written in [Markdown](https://daringfireball.net/projects/markdown/syntax).

#### Code Contributions

Note that this repository uses the 'Gitflow' model. Pull-requests should be submitted against the `develop` branch for small changes; and ideally in a dedicated branch prefixed with `feature/` for major changes.

#### Getting Started

Source related to the Grasshopper plugin is located in the `plugin` directory. Editing and compiling that code is best done in Visual Studio 2017. The [community editions](https://www.visualstudio.com) for Windows or macOS should both work.

Source related to the Website (including it's content) is located in the `site` directory. A seperate `README` file there outlines how to setup and run a local copy of the site.

If modifying Rhinoceros models, note that [Git Large File Storage](https://git-lfs.github.com) is used to track these. Various models are in different locations:

- Demo files are located in the `demos` folder
- Example files for specific components are located in the `examples` folder
- Test files for specific components are located in the `tests` folder
- Recreations of the precedent projects are located in the `projects` folder.
