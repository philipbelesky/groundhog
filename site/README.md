# Groundhog Wiki

Running a local copy of the website requires node.js and ruby, and experience with website development and the command line. The site itself is built using the [Jekyll](https://jekyllrb.com) framework.

However, if you just want to edit text or add new pages, all that requries is a text editor and a basic understanding of the [Markdown syntax](https://daringfireball.net/projects/markdown/syntax).

Markdown files for existing posts are located in `_documentation`, `_projects`, and `_techniques` folders. There are a couple of Jekyll-specific bits of syntax (i.e. the [Front Matter](https://jekyllrb.com/docs/frontmatter/) and [figure include](https://jekyllrb.com/docs/includes/) but if you are interested in contributing to the site's content I can take care of those details during any pull requests. Note that there are also separate folders under `assets` for the images used in each post.

## Setup

Install dependencies using:

    gem install bundler
    bundler install
    npm install

## Serving

To start a local copy of the website, use:

    npm run serve

Then open `http://localhost:4000/`. If using a [Livereload plugin](https://chrome.google.com/webstore/detail/livereload/jnihajbhpnppcggbcgedagnkighmdlei?hl=en) changes to files (layout; markdown; css; etc) should trigger browser updates live.

## Deploying

Note the `compile_assets.sh` script handles zipping/shifting the necessary files over to the `_site` for publishing. This is designed to be run on-server as part of deployment.

The `compile_plugin.sh` script creates JSON representations of components and builds/zips the plugin files. This depends on python and C# and is designed to be run locally with the results committed to source control.

Images are compiled into responsive variants using gulp and `npm run images`. This automatically triggered during deployment, but needs to be manually re-run during development.
