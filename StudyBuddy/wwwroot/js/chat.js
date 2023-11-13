"use strict";

let senderId = document.getElementById("userId").value;
let receiverId = document.getElementById("receiverId").value;

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat?receiver=" + receiverId + "&sender=" + senderId) // Pass the receiver and sender parameter in the query string
    .build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

//Message style right here
connection.on("ReceiveMessage", function (user, message) {
    let messageList = document.getElementById("messagesList");

    let messageContainer = document.createElement("div");
    messageContainer.classList.add(user === userId.value ? "chat-message-left" : "chat-message-right", "pb-4");

    let messageContent = document.createElement("div");
    messageContent.classList.add("d-flex");

    let avatarDiv = document.createElement("div");
    avatarDiv.innerHTML = `
        <img src="${user === userId.value ? senderAvatar.value : receiverAvatar.value}"
            class="rounded-circle me-1" alt="${user === userId.value ? userName.value : receiverName.value}" width="40" height="40">
        <div class="text-muted small text-nowrap mt-2">${getCurrentTime()}</div>
    `;

    let textDiv = document.createElement("div");
    textDiv.classList.add("flex-shrink-1", "bg-light", "rounded", "py-2", "px-3", user === userId.value ? "me-3" : "ms-3");

    let userLabel = document.createElement("div");
    userLabel.classList.add("fw-bold", "mb-1");
    userLabel.textContent = user === userId.value ? "You" : receiverName.value;

    // Wrap the message at 80 characters
    message = wrapText(message, 80);

    let messageText = document.createElement("div");
    messageText.textContent = message;

    textDiv.appendChild(userLabel);
    textDiv.appendChild(messageText);
    messageContent.appendChild(avatarDiv);
    messageContent.appendChild(textDiv);
    messageContainer.appendChild(messageContent);
    messageList.appendChild(messageContainer);
    scrollToBottom();
});

// Function to wrap text at a specified character limit
function wrapText(text, limit) {
    let result = '';
    while (text.length > limit) {
        let chunk = text.slice(0, limit);
        let lastSpace = chunk.lastIndexOf(' ');
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

function scrollToBottom() {
    let chatMessages = document.getElementById("messagesList");
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

function getCurrentTime() {
    let currentUnixTimestamp = ~~(+new Date() / 1000);
    let date = new Date(currentUnixTimestamp * 1000)
    let hours = date.getHours();
    let minutes = "0" + date.getMinutes();
    let formattedTime = hours + ':' + minutes.substr(-2);

    return formattedTime;
}


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    let messageInput = document.getElementById("messageInput");
    let message = messageInput.value.trim(); // Check and trim the message
    let groupName = document.getElementById("groupName").value;

    if (message !== '') {
        // If message is not empty, proceed to send
        connection.invoke("SendMessageToGroup", groupName, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    messageInput.value = ""; // clear the input field
    event.preventDefault();
});


