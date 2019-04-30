"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();

const pictureElement = document.querySelector("#picture");
const countdownElement = document.querySelector("#countdown");

connection.on("countdown", function () {
    startTimer();
});

connection.on("reloadPicture", function () {
    pictureElement.src = "http://localhost:5513/preview.jpg";
    pictureElement.alt = "Fotobox Foto";
});

connection.on("reset", function (text) {
    countdownElement.innerHTML = text;
    fadeOutText(countdownElement);
});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.log(err.toString());
});


function startTimer() {
    var timeLeft = 3 + 1;
    var downloadTimer = setInterval(function () {
        countdownElement.innerHTML = timeLeft - 1;
        timeLeft -= 1;
        if (timeLeft == 0) {
            countdownElement.innerHTML = "Foto!!";
        }
        if (timeLeft < 0) {
            clearInterval(downloadTimer);
            countdownElement.innerHTML = "";
        }
    }, 1000);
}

function fadeOutText(target) {
    var fadeEffect = setInterval(function () {
        if (!target.style.opacity) {
            target.style.opacity = 1;
        }
        if (target.style.opacity > 0) {
            target.style.opacity -= 0.1;
        } else {
            clearInterval(fadeEffect);
            pictureElement.src = "";
        }
    }, 200);
}