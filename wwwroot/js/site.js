function initializeCarouselWithRandomBackgrounds() {
    var backgrounds = [
        '/images/bg1.jpg',
        '/images/bg2.jpg',
        '/images/bg3.jpg',
        '/images/bg4.jpg'
    ];

    $('#carouselExampleIndicators').on('slid.bs.carousel', function () {
        var randomIndex = Math.floor(Math.random() * backgrounds.length);
        var newBackground = 'url(' + backgrounds[randomIndex] + ')';
        $('.top-artists-page').css('background-image', newBackground);
    });
}

