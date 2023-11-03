"use strict";

var senderId = document.getElementById("senderInput").value;
var receiverId = document.getElementById("receiverInput").value;

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat?receiver=" + document.getElementById("receiverInput").value + "&sender=" + senderId) // Pass the receiver and sender parameter in the query string
    .build();
console.log("Console result");
console.log(document.getElementById("receiverInput").value);
console.log(document.getElementById("senderInput").value);
//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

//Message style right here
connection.on("ReceiveMessage", function (user, message) {
    var messageList = document.getElementById("messagesList");

    var messageContainer = document.createElement("div");
    messageContainer.classList.add(user === userId.value ? "chat-message-left" : "chat-message-right", "pb-4");

    var messageContent = document.createElement("div");
    messageContent.classList.add("d-flex");

    var avatarDiv = document.createElement("div");
    avatarDiv.innerHTML = `
        <img src="${user === userId.value ? senderAvatar.value : receiverAvatar.value}"
            class="rounded-circle me-1" alt="${user === userId.value ? userName.value : receiverName.value}" width="40" height="40">
        <div class="text-muted small text-nowrap mt-2">${getCurrentTime()}</div>
    `;

    var textDiv = document.createElement("div");
    textDiv.classList.add("flex-shrink-1", "bg-light", "rounded", "py-2", "px-3", user === userId.value ? "me-3" : "ms-3");

    var userLabel = document.createElement("div");
    userLabel.classList.add("fw-bold", "mb-1");
    userLabel.textContent = user === userId.value ? "You" : receiverName.value;

    // Wrap the message at 80 characters
    message = wrapText(message, 80);

    var messageText = document.createElement("div");
    messageText.textContent = message;

    textDiv.appendChild(userLabel);
    textDiv.appendChild(messageText);
    messageContent.appendChild(avatarDiv);
    messageContent.appendChild(textDiv);
    messageContainer.appendChild(messageContent);
    messageList.appendChild(messageContainer);
});

// Function to wrap text at a specified character limit
function wrapText(text, limit) {
    var result = '';
    while (text.length > limit) {
        var chunk = text.slice(0, limit);
        var lastSpace = chunk.lastIndexOf(' ');
        if (lastSpace !== -1) {
            result += chunk.slice(0, lastSpace + 1) + '\n';
            text = text.slice(lastSpace + 1);
        } else {
            result += chunk + '\n';
            text = text.slice(limit);
        }
    }
    return result + text;
}

function getCurrentTime() {
    const now = new Date();
    const hours = now.getHours().toString().padStart(2, '0'); // Ensure 2-digit format
    const minutes = now.getMinutes().toString().padStart(2, '0'); // Ensure 2-digit format
    return `${hours}:${minutes}`;
}


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var sender = document.getElementById("senderInput").value;
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;
    document.getElementById("messageInput").value = "";

    // Sort user IDs to ensure consistent group names regardless of user roles
    var userIds = [sender, receiver];
    userIds.sort();

    var groupName = `${userIds[0]}-${userIds[1]}`;

    connection.invoke("SendMessageToGroup", groupName, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

