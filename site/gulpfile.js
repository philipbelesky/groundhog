var gulp = require('gulp');
var responsive = require('gulp-responsive');

gulp.task('images', function () {
  return gulp.src([
      // './assets/documentation/**/*.{png,jpg,jpeg}', // OK
      './assets/projects/**/*.{png,jpg,jpeg}',
      // './assets/techniques/**/*.{png,jpg,jpeg}' // OK
    ]).pipe(responsive({
      '**/*.*': [
      {
        // original image (for linking to)
        rename: {
          suffix: '-original',
          extname: '.jpg',
        },
      },
      {
        width: 800,
        rename: {
          suffix: '-small',
          extname: '.jpg',
        },
      },
      {
        width: 800 * 2,
        rename: {
          suffix: '-small@2x',
          extname: '.jpg',
        },
      },
      {
        width: 960,
        rename: {
          suffix: '-large',
          extname: '.jpg',
        },
      },
      {
        width: 960 * 2,
        rename: {
          suffix: '-large@2x',
          extname: '.jpg',
        },
      },
      {
        width: 1344,
        rename: {
          suffix: '-extralarge',
          extname: '.jpg',
        },
      },
      {
        width: 1344 * 2,
        rename: {
          suffix: '-extralarge@2x',
          extname: '.jpg',
        },
      },
      {
        width: 800,
        rename: {
          suffix: '-small',
          extname: '.webp',
        },
      },
      {
        width: 800 * 2,
        rename: {
          suffix: '-small@2x',
          extname: '.webp',
        },
      },
      {
        width: 960,
        rename: {
          suffix: '-large',
          extname: '.webp',
        },
      },
      {
        width: 960 * 2,
        rename: {
          suffix: '-large@2x',
          extname: '.webp',
        },
      },
      {
        width: 1200,
        rename: {
          suffix: '-extralarge',
          extname: '.webp',
          // Skip enlargment
        },
      },
      {
        width: 1200 * 2,
        rename: {
          suffix: '-extralarge@2x',
          extname: '.webp',
        },
      }
    ],
    }, {
      // Global configuration for all images
      // The output quality for JPEG, WebP and TIFF output formats
      quality: 70,
      // Use progressive (interlace) scan for JPEG and PNG output
      progressive: true,
      // Zlib compression level of PNG output format
      compressionLevel: 6,
      // Strip all metadata
      withMetadata: false,
      // Skip sizes that are too big
      withoutEnlargement: true,
      skipOnEnlargement: false,
      errorOnEnlargement: false
    }))
    .pipe(gulp.dest('_site/assets/img/'));
});
