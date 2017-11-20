# Groundhog Wiki

Running a local copy of the website requires node.js and ruby, and experience with website development and the command line. The site itself is built using the [Jekyll](https://jekyllrb.com) framework.

However, if you just want to edit text or add new pages, all that requries is a text editor and a basic understanding of the [Markdown syntax](https://daringfireball.net/projects/markdown/syntax).

Markdown files for existing posts are located in `_documentation`, `_projects`, and `_techniques` folders. There are a couple of Jekyll-specific bits of syntax (i.e. the [Front Matter](https://jekyllrb.com/docs/frontmatter/) and [figure include](https://jekyllrb.com/docs/includes/) but if you are interested in contributing to the site's content I can take care of those details during any pull requests. Note that there are also seperate folders under `assets` for the images used in each post.

## Setup

Install dependencies using:

    gem install bundler
    bundler install
    npm install

## Serving

To start a local copy of the website, use:

    npm run build

In addition, if you'd like to use BrowserSync to automatically refresh the browser upon file changes, you can use:

    npm run serve

## Deploying

Note the `compile.sh` script handles compiling JSON representations of components and zipping/shifting the necessary files over to the `_site` for publishing.

    bash compile.sh

Then build Jekyll for deployment:

    JEKYLL_ENV=production bundle exec jekyll build

Deployment is done using [s3_website](https://github.com/laurilehmijoki/s3_website). This requires Java 8 and a set of valid Amazon Web Service credentials in a `.env` file in this directory. Changes can be published with:

    s3_website push

This is packaged together in

    npm run publish