var gulp = require('gulp');
var responsive = require('gulp-responsive');


gulp.task('figures', function () {
  return gulp.src([
      './assets/documentation/**/!(thumbnail)*.{png,jpg,jpeg}',
      './assets/projects/**/!(thumbnail)*.{png,jpg,jpeg}',
      './assets/techniques/**/!(thumbnail)*.{png,jpg,jpeg}'
    ]).pipe(responsive({
      '**/*.*': [
        {
          rename: {
            suffix: '-original', // original image (for linking to)
            extname: '.jpg',
          },
        },
        // JPEG VARIATIONS
        {
          width: 960, // 1x Mobile;
          rename: {
            suffix: '-smaller',
            extname: '.jpg',
          },
        },
        {
          width: 1152, // 1x Desktops at widths less than 1472 (from bpoint)
          rename: {
            suffix: '-small',
            extname: '.jpg',
          },
        },
        {
          width: 1344, // 1x Desktops at widths greater than 1472 (from bpoint)
          rename: {
            suffix: '-medium',
            extname: '.jpg',
          },
        },
        {
          width: 1536, // 2x Mobile (768 * 2x); where 768 is assumed max
          rename: {
            suffix: '-large',
            extname: '.jpg',
          },
        },
        {
          width: 1920, // 2X Tablets and 2X Desktops at low width (960 * 2x)
          rename: {
            suffix: '-larger',
            extname: '.jpg',
          },
        },
        {
          width: 2304, // 2X Desktops at medium width and above (1152 * 2x)
          rename: {
            suffix: '-huge',
            extname: '.jpg',
          },
        },
        // WEBP variations
        {
          width: 960, // 1x Mobile;
          rename: {
            suffix: '-smaller',
            extname: '.webp',
          },
        },
        {
          width: 1152, // 1x Desktops at widths less than 1472 (from bpoint)
          rename: {
            suffix: '-small',
            extname: '.webp',
          },
        },
        {
          width: 1344, // 1x Desktops at widths greater than 1472 (from bpoint)
          rename: {
            suffix: '-medium',
            extname: '.webp',
          },
        },
        {
          width: 1536, // 2x Mobile (768 * 2x); where 768 is assumed max
          rename: {
            suffix: '-large',
            extname: '.webp',
          },
        },
        {
          width: 1920, // 2X Tablets and 2X Desktops at low width (960 * 2x)
          rename: {
            suffix: '-larger',
            extname: '.webp',
          },
        },
        {
          width: 2304, // 2X Desktops at medium width and above (1152 * 2x)
          rename: {
            suffix: '-huge',
            extname: '.jpg',
          },
        },
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


// These have different quality and size formats (lower/smaller/singular)
gulp.task('thumbnails', function () {
  return gulp.src([
    './assets/documentation/**/thumbnail.{png,jpg,jpeg}',
    './assets/projects/**/thumbnail.{png,jpg,jpeg}',
    './assets/techniques/**/thumbnail.{png,jpg,jpeg}'
  ]).pipe(responsive({
    '**/*.*': [
      {
        width: 690,
        rename: {
          suffix: '-medium',
          extname: '.jpg',
        },
      },
      {
        width: 690,
        rename: {
          suffix: '-medium',
          extname: '.webp',
        },
      }
    ]
  }, {
    // Global configuration for all images
    // The output quality for JPEG, WebP and TIFF output formats
    quality: 70,
    // Zlib compression level of PNG output format
    compressionLevel: 6,
    // Strip all metadata
    withMetadata: false,
  }))
  .pipe(gulp.dest('_site/assets/img/'));
});


gulp.task('images', gulp.series('thumbnails', 'figures'));
