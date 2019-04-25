"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44391/fotoboxHub").build();


connection.on("Countdown", function () {
    this.StartTimer();
});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.log(err.toString());
});


function StartTimer() {
    var timeLeft = 3;
    var downloadTimer = setInterval(function () {
        document.getElementById("countdown").innerHTML = timeLeft + " seconds remaining";
        timeLeft -= 1;
        if (timeLeft <= 0) {
            clearInterval(downloadTimer);
            document.getElementById("countdown").innerHTML = "Finished";
        }
    }, 1000);
}