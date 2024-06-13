document.getElementById('next-icon').onclick = function () {
    changeBackground();
    slideOutCurrentSlide('left');
    slideInNextSlide();
}

document.getElementById('prev-icon').onclick = function () {
    changeBackground();
    slideOutCurrentSlide('right');
    slideInPrevSlide();
}

function changeBackground() {
    var backgrounds = [
        '/images/bg1.jpg',
        '/images/bg2.jpg',
        '/images/bg3.jpg',
        '/images/bg4.jpg'
    ];

    var randomIndex = Math.floor(Math.random() * backgrounds.length);
    var newBackground = 'url(' + backgrounds[randomIndex] + ')';
    var $background = $('.background');
    var $nextBackground = $('.next-background');

    $nextBackground.css('background-image', newBackground);
    $nextBackground.css('animation', 'fade-in 2s forwards');
    $nextBackground.on('animationend', function () {
        $background.css('background-image', newBackground);
        $nextBackground.css('animation', '');
        $nextBackground.css('opacity', 0);
    });
}

function slideOutCurrentSlide(direction) {
    var $currentSlide = $('.carousel-item.active');
    var transformValue = direction === 'left' ? 'translateX(-100%)' : 'translateX(100%)';
    $currentSlide.css({
        opacity: 0,
        transform: transformValue
    });
}

function slideInNextSlide() {
    var $nextSlide = $('.carousel-item.active').next();
    if ($nextSlide.length === 0) {  // if there's no next slide, go back to the first one
        $nextSlide = $('.carousel-item').first();
    }
    $nextSlide.css({
        opacity: 0,
        transform: 'translateX(100%)'
    });
    setTimeout(function () {
        $nextSlide.css({
            opacity: 1,
            transform: 'translateX(0)'
        });
    }, 100);  // delay the animation slightly to ensure the previous slide has finished sliding out
}

function slideInPrevSlide() {
    var $prevSlide = $('.carousel-item.active').prev();
    if ($prevSlide.length === 0) {  // if there's no previous slide, go to the last one
        $prevSlide = $('.carousel-item').last();
    }
    $prevSlide.css({
        opacity: 0,
        transform: 'translateX(-100%)'
    });
    setTimeout(function () {
        $prevSlide.css({
            opacity: 1,
            transform: 'translateX(0)'
        });
    }, 100);  // delay the animation slightly to ensure the previous slide has finished sliding out
}