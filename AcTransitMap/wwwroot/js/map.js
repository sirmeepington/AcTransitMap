"use strict";


// Locations = Initial values.

// Map

var map;

var markers = {};

// Creates a marker for the given vehicle position if a
// marker doesn't already exist.
function createMarker(location) {
    if (location == undefined || markers[location.vehicleId]) {
        return;
    }

    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(location.latitude, location.longitude),
        map: map,
    });

    (function (marker) {
        google.maps.event.addListener(marker, 'click', function () {
            infowindow = new google.maps.InfoWindow({
                content: "Bus #" + location.vehicleId + " @@ " + location.lastUpdated + " UTC"
            });
            infowindow.open(map, marker);
        });
    })(marker)

    markers[location.vehicleId] = marker;
}

// Updates a markers position or creates one if it doesn't
// exist for the given Vehicle Location
function updateMarker(location) {
    var marker = markers[location.vehicleId];
    if (marker) {
        marker.setPosition(new google.maps.LatLng(location.latitude, location.longitude));
    } else {
        createMarker(location);
    }
}

function initMap() {
    // map options
    var options = {
        zoom: 10,
        center: new google.maps.LatLng(37.811321571951616, -122.24355892440079),
        mapTypeId: google.maps.MapTypeId.TERRAIN,
        mapTypeControl: false
    };

    // init map
    map = new google.maps.Map(document.getElementById('map-canvas'), options);

    for (var loc in locations) {
        updateMarker(locations[loc]);
    }
}

// SignalR

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/mapHub")
    .build()

connection.start().then(function () {
    console.log("SignalR connection established.");
}).catch(function (err) {
    return console.error(err.toString())
})

connection.on("UpdateLocation", function (loc) {
    updateMarker(loc);
});
