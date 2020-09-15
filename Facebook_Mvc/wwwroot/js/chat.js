"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
    //MEDIUM
    if (user == $("#userInput").val()) {
        var mesaj = `<div class="d-flex justify-content-end mb-4">
                <div class="msg_cotainer_send">
                    `+ msg + `</b>
                      </div>
                 </div>`;
        $("#messageList").append(mesaj);
    } else {
        var mesaj = `<div class="d-flex justify-content-start mb-4">
                <div class="msg_cotainer" role="alert">
                    `+ msg + `</b>
                      </div>
                 </div>`;
        $("#messageList").append(mesaj);
    }
    $(".card-body").scrollTop($('.card-body')[0].scrollHeight - $('.card-body')[0].clientHeight);
    //MEDıum
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messagInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
    var data =
    {
        'usereposta': user,
        'Message': message,
    }
    $.ajax({
        type: 'Post',
        url: '/Home/Message',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (data) {
            console.log('sample', data);
            console.log('Basarılı');
        },
        error: function () {

        }

    });
    
});