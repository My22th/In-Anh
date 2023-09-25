// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$.LoadingOverlaySetup({
    background: "rgba(0, 0, 0, 0.5)",
    image: "/img/bars.svg",
    imageAnimation: "1.5s fadein",
    imageColor: "#ffcc00"
});

$(function () {
	$('.mhn-slide').owlCarousel({
        nav: false,
        loop: false,
        dots: false,
        pagination: false,
        margin: 25,
        autoHeight: false,
        stagePadding: 50,
        responsive: {
            0: {
                items: 1,
                stagePadding: 0,
                margin: 30,
            },
            767: {
                items: 3,
                stagePadding: 25,
            },
            1000: {
                items: 3,
            }
        }
		
	});
});