"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44391/fotoboxHub").build();


connection.on("Countdown", function () {
    StartTimer();
});

connection.on("ReloadPicture", function () {
    //document.getElementById("countdown").innerHTML = "realoaded the mofogger";
    document.getElementById("capture").src = "http://localhost:5513/preview.jpg";
    document.getElementById("capture").src = "http://digicamcontrol.com/user/themes/woo/images/footer-logo.png";
});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.log(err.toString());
});


function StartTimer() {
    var timeLeft = 3 + 1;
    var downloadTimer = setInterval(function () {
        document.getElementById("countdown").innerHTML = timeLeft - 1;
        timeLeft -= 1;
        if (timeLeft == 0) {
            document.getElementById("countdown").innerHTML = "Foto!!";
        }
        if (timeLeft < 0) {
            clearInterval(downloadTimer);
            document.getElementById("countdown").innerHTML = "";
        }
    }, 1000);
}