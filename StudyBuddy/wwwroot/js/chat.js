"use strict";

var senderId = document.getElementById("senderInput").value;

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat?userid=" + document.getElementById("receiverInput").value + "&sender=" + senderId) // Pass the sender parameter in the query string
    .build();
console.log("Console result");
console.log(document.getElementById("receiverInput").value);
console.log(document.getElementById("senderInput").value);
//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.textContent = `${user} says ${message}`;
    document.getElementById("messagesList").appendChild(li);
});


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var sender = document.getElementById("senderInput").value;
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;

    // Sort user IDs to ensure consistent group names regardless of user roles
    var userIds = [sender, receiver];
    userIds.sort();

    var groupName = `${userIds[0]}-${userIds[1]}`;

    connection.invoke("SendMessageToGroup", groupName, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

