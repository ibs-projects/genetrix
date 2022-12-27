;(function ($) {
    "use strict";

    /*============= preloader js css =============*/
    var cites = [];
    cites[0] = "Veuillez patienter, chargement en cours...";
    cites[1] = "Veuillez patienter, chargement en cours...";
    cites[2] = "Veuillez patienter, chargement en cours...";
    cites[3] = "Veuillez patienter, chargement en cours...";
    var cite = cites[Math.floor(Math.random() * cites.length)];
    $('#preloader p').text(cite);
    $('#preloader').addClass('loading');

    $(window).on( 'load', function() {
        setTimeout(function () {
            $('#preloader').fadeOut(500, function () {
                $('#preloader').removeClass('loading');
            });
        }, 500);
    })

})(jQuery)