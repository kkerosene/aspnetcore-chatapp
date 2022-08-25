var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();
var _connectionId = '';

connection.start()
    .then(function () {
        connection.invoke('getConnectionId');
    })
    .then(function (connectionId) {
        _connectionId = connectionId
        joinChat();
    })
    .catch(function (err) {
        console.log(err)
    })

connection.on("RecieveMessage", function (data) {
    var message = document.createElement("div")
    message.classList.add("message")

    var header = document.createElement("header")
    header.appendChild(document.createTextNode(data.name))

    var p = document.createElement("p")
    p.appendChild(document.createTextNode(data.text))

    var footer = document.createElement("footer")
    footer.appendChild(document.createTextNode(data.timestamp))

    message.appendChild(header);
    message.appendChild(p);
    message.appendChild(footer);

    document.querySelector('.chat-body').append(message);
})

var joinChat = function () {

    axios.post('/Chat/JoinChat/' + _connectionId + '/@Model.Name', null)
        .then(res => {
            console.log('Joined a Chat!', res);
        })
        .catch(err => {
            console.log('Failed to Join a Chat!', res);
        })
}

window.addEventListener('onunload', function () {
    connection.invoke('leaveChat', '@Model.ChatId');
})

var sendMessage = function (event) {
    event.preventDefault();

    var data = new FormData(event.target);

    document.getElementById('message-input').value = '';

    axios.post('/Chat/SendMessage', data)
        .then(res => {
            console.log("Message Sent!")
        })
        .catch(err => {
            console.log("Failed to Send a Message!")
        })
}

var editMessage = function (event) {
    event.preventDefault();

    var content = data.text

    document.getElementById('message-input').value = content;

    axios.put('/Chat/EditMessage', data)
        .then(res => {
            console.log("Message Edit Successfully!")
        })
        .catch(err => {
            console.log("Failed to Edit a Message!")
        })
}

var deleteMessage = function (event) {
    axios.delete('/Chat/DeleteMessage', data)
        .then(res => {
            console.log("Message Deleted!")
        })
        .catch(err => {
            console.log("Failed to Delete a Message!")
        })
}