"use strict";


// Map 

var map;

function updateMarkers() {
    for (var i = 0; i < markers.length; i++) {
        if (markers[i] != null) {
            markers[i].map = null;
        }
    }
    markers = [];

    for (var i = 0; i < locations.length; i++) {
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(locations[i].latitude, locations[i].longitude),
            map: map,
        });
        (function (marker, i) {
            // add click event
            google.maps.event.addListener(marker, 'click', function () {
                infowindow = new google.maps.InfoWindow({
                    content: "Bus #" + locations[i].vehicleId + " @@ " + locations[i].lastUpdated + " UTC"
                });
                infowindow.open(map, marker);
            });
        })(marker, i);
    }
}

function initMap() {
    // execute
    (function () {
        // map options
        var options = {
            zoom: 5,
            center: new google.maps.LatLng(37.811321571951616, -122.24355892440079),
            mapTypeId: google.maps.MapTypeId.TERRAIN,
            mapTypeControl: false
        };

        // init map
        map = new google.maps.Map(document.getElementById('map-canvas'), options);

        updateMarkers();
    })();
}

// SignalR

var connection = new signalR.HubConnectionBuilder().withUrl("/map").build()

connection.start().then(function () {
    console.log("SignalR connection established.");
}).catch(function (err) {
    return console.error(err.toString())
})

connecton.on("UpdateLocations", function (newLocations) {
    locations = newLocations;
    updateMarkers()
});