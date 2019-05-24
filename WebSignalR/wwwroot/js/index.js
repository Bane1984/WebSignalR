﻿var chatterName = 'Posjetioc';

// inicijalizacija SignalR klijenta
var connection = new signalR.HubConnectionBuilder()
    .withUrl('/chatHub')
    .build();

connection.on('ReceiveMessage', renderMessage);

connection.start();

function showChatDialog(parameters) {
    var dialogEl = document.getElementById('chatDialog');
    dialogEl.style.display = 'block';
}

function sendMessage(text) {
    if (text&&text.length) {
        connection.invoke('SendMessage', chatterName, text)
    }
}

function ready() {
    setTimeout(showChatDialog, 500);

    var chatFormEl = document.getElementById('chatForm');
    chatFormEl.addEventListener('submit',
        function(e) {
            var text = e.target[0].value;
            e.target[0].value = '';
            sendMessage(text);
        });
}

function renderMessage(name, time, message) {
    var nameSpan = document.createElement('span');
    nameSpan.className = 'name';
    nameSpan.textContent = name;

    var timeSpan = document.createElement('span');
    timeSpan.className = 'time';
    var friendlyTime = moment(time).format('H:mm');
    timeSpan.textContent = friendlyTime;

    var headerDiv = document.createElement('div');
    headerDiv.appendChild(nameSpan);
    headerDiv.appendChild(timeSpan);

    var messageDiv = document.createElement('div');
    messageDiv.className = 'message';
    messageDiv.textContent = message;

    var newItem = document.createElement('li');
    newItem.appendChild(headerDiv);
    newItem.appendChild(messageDiv);

    var chatHistoryEl = document.getElementById('chatHistory');
    chatHistoryEl.appendChild(newItem);
    chatHistoryEl.scrollTop = chatHistoryEl.scrollHeight - chatHistoryEl.clientHeight;
}

document.addEventListener('DOMContentLoaded', ready);