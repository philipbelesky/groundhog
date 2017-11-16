---
---

// Masonry Include
{% include_absolute /node_modules/masonry-layout/dist/masonry.pkgd.min.js %}

// Feather Include
{% include_absolute /node_modules/feather-icons/dist/feather.min.js %}

// Icons
feather.replace()

// Menu Toggle; via https://bulma.io/documentation/components/navbar/
document.addEventListener('DOMContentLoaded', function () {

  // Get all "navbar-burger" elements
  var $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0);

  // Check if there are any navbar burgers
  if ($navbarBurgers.length > 0) {

    // Add a click event on each of them
    $navbarBurgers.forEach(function ($el) {
      $el.addEventListener('click', function () {

        // Get the target from the "data-target" attribute
        var target = $el.dataset.target;
        var $target = document.getElementById(target);

        // Toggle the class on both the "navbar-burger" and the "navbar-menu"
        $el.classList.toggle('is-active');
        $target.classList.toggle('is-active');

      });
    });
  }

});

// Masonry Grid
var grid = document.querySelector('.masonry-grid');
if (grid !== null) {
  var msnry = new Masonry( grid, {
    // options...
    itemSelector: '.masonry-item',
    horizontalOrder: true,
    // percentPosition: true,
    // columnWidth: '.test',
  });
}