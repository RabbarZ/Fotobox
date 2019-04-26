"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44391/fotoboxHub").build();


connection.on("Countdown", function () {
    StartTimer();
});

connection.on("ReloadPicture", function () {
    //document.getElementById("countdown").innerHTML = "realoaded the mofogger";
    var element = document.getElementById("picture");
    element.src = "http://localhost:5513/preview.jpg";
    element.alt = "Fotobox Foto";
});

connection.on("Reset", function (text) {
    var element = document.getElementById("countdown");
    element.innerHTML = text;
    FadeOutText(element);
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

function FadeOutText(fadeTarget) {
    var fadeEffect = setInterval(function () {
        if (!fadeTarget.style.opacity) {
            fadeTarget.style.opacity = 1;
        }
        if (fadeTarget.style.opacity > 0) {
            fadeTarget.style.opacity -= 0.1;
        } else {
            clearInterval(fadeEffect);
            document.getElementById("picture").src = "";
        }
    }, 200);
}