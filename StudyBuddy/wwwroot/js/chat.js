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

    let messageContainer = createMessageContainer(user);

    let messageContent = createMessageContent(user);

    let avatarDiv = createAvatarDiv(user);

    let textDiv = createTextDiv(user, message);

    let userLabel = createUserLabel(user);

    let messageText = createMessageText(message);

    textDiv.appendChild(userLabel);
    textDiv.appendChild(messageText);
    // Check if the message is from the current user
    if (user === userId.value) {
        messageContent.appendChild(textDiv); // Append text first for the current user
        messageContent.appendChild(avatarDiv);
    } else {
        messageContent.appendChild(avatarDiv); // Append avatar first for other users
        messageContent.appendChild(textDiv);
    }
    messageContainer.appendChild(messageContent);
    messageList.appendChild(messageContainer);
    scrollToBottom();
});

function createMessageContainer(user) {
    let messageContainer = document.createElement("div");
    messageContainer.classList.add(user === userId.value ? "chat-message-right" : "chat-message-left", "pb-4");
    return messageContainer;
}

function createMessageContent(user) {
    let messageContent = document.createElement("div");
    messageContent.classList.add("d-flex");
    return messageContent;
}

function createAvatarDiv(user) {
    let avatarDiv = document.createElement("div");
    avatarDiv.innerHTML = `
        <img src="${user === userId.value ? senderAvatar.value : receiverAvatar.value}"
            class="rounded-circle ${user === userId.value ? "me-1" : "ms-1"}" alt="${user === userId.value ? userName.value : receiverName.value}" width="40" height="40">
        <div class="text-muted small text-nowrap mt-2">${getCurrentTime()}</div>
    `;
    return avatarDiv;
}

function createTextDiv(user, message) {
    let textDiv = document.createElement("div");
    textDiv.classList.add("text-break", "flex-shrink-1", "bg-light", "rounded", "py-2", "px-3", user === userId.value ? "me-3" : "ms-3");
    return textDiv;
}

function createUserLabel(user) {
    let userLabel = document.createElement("div");
    userLabel.classList.add("fw-bold", "mb-1");
    userLabel.textContent = user === userId.value ? "You" : receiverName.value;
    return userLabel;
}

function createMessageText(message) {
    let messageText = document.createElement("div");
    messageText.textContent = message;
    return messageText;
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
    return hours + ':' + minutes.slice(-2);
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
