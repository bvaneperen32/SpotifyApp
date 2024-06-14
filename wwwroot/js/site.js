$(document).ready(function () {

    changeBackground();
    // Event listeners for next and previous icons
    document.getElementById('next-icon').onclick = function () {
        slideOutCurrentSlide('left');
        slideInNextSlide();
    }

    document.getElementById('prev-icon').onclick = function () { 
        slideOutCurrentSlide('right');
        slideInPrevSlide();
    }
});

function changeBackground(direction, $slide) {
    if ($slide === undefined) {
        var $currentSlide = $('.carousel-item.active');
    } else {
        var $currentSlide = $slide; 
    }

    var colors = $currentSlide.data('colors').split(',');


    // Construct a CSS gradient string from the colors
    var gradient = 'repeating-linear-gradient(to right, ' + colors.join(', ') + ')';


    // Apply the gradient to the next background
    var $nextBackground = $('.next-background');
    $nextBackground.css('background-image', gradient);

    // Use CSS animation to fade in the next background
    $nextBackground.css('animation', 'fade-in 2s forwards');
    $nextBackground.on('animationend', function () {
        // After the fade-in animation ends, set the main background to the new gradient
        // and reset the next background for the next transition
        $('.background').css('background-image', gradient);
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

    changeBackground('next', $nextSlide);

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

    changeBackground('prev', $prevSlide);

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
