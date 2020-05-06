// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var appointmentsToPost;
$(document).ready( function () {
    $.ajax({
        url: 'https://localhost:44366/Contractors/GetAppointments',
        type: 'GET',
        success: function (response, data) {
            var addresses = new Array();
            var pins = new Array();
            for (var i = 0; i < response.length; i++) {
                addresses.push(response[i].client.address);
            }
            for (var i = 0; i < addresses.length; i++) {
                var pin = new Array();
                pin[0] = addresses[i].lat;
                pin[1] = addresses[i].lon;
                pins.push(pin);
            } 
            appointmentsToPost = pins;
            for (var i = 0; i < appointmentsToPost.length; i++) {
            }
        }
    })
});