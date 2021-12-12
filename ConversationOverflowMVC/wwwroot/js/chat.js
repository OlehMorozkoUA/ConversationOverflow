var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessageFromUser", function (userIds, message) {
    var userToUserId = parseInt(userIds[0]) + parseInt(userIds[1]);
    let messages = document.getElementById("messages");
    var hasUserIds = messages.hasAttribute("userIds");
    if (hasUserIds) {
        var uIds = messages.getAttribute("userIds");
        var login = GetLogin(userIds[0]);
        if (userToUserId == uIds) {
            $.ajax({
                type: "GET",
                url: _message,
                data: {
                    login: login,
                    text: message
                },
                dataType: "text",
                success: function (response) {
                    $("#messages").append(response);
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    }
});

connection.on("ReceiveMessageFromGroup", function (userId, groupId, message) {
    let messages = document.getElementById("messages");
    var hasGroupId = messages.hasAttribute("groupId");
    if (hasGroupId) {
        var gId = messages.getAttribute("groupId");
        var login = GetLogin(userId);
        if (gId == groupId) {
            $.ajax({
                type: "GET",
                url: _message,
                data: {
                    login: login,
                    text: message
                },
                dataType: "text",
                success: function (response) {
                    $("#messages").append(response);
                    var len = messages.querySelectorAll("div[name='message']").length;
                    if (len <= 10) document.getElementById("messages").scrollTop = document.getElementById("messages").scrollHeight + 300;
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    }
});

connection.on("ReceiveAttachmentFromGroup", function (userId, groupId, attachment) {
    let messages = document.getElementById("messages");
    var hasGroupId = messages.hasAttribute("groupId");
    if (hasGroupId) {
        var gId = messages.getAttribute("groupId");
        var login = GetLogin(userId);
        if (gId == groupId) {
            $.ajax({
                type: "GET",
                url: _attachment,
                data: {
                    login: login,
                    attachment: attachment
                },
                dataType: "text",
                success: function (response) {
                    $("#messages").append(response);
                    var len = messages.querySelectorAll("div[name='message']").length;
                    if (len <= 10) document.getElementById("messages").scrollTop = document.getElementById("messages").scrollHeight + 300;
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    }
});

connection.on("ReceiveMessageFromUserForTyping", function (groupId) {
    let messages = document.getElementById("messages");
    var hasGroupId = messages.hasAttribute("groupId");
    if (hasGroupId) {
        var gId = messages.getAttribute("groupId");
        if (gId == groupId) {
            let dot3 = document.getElementById("dot3");
            let em = dot3.getElementsByTagName("em")[0];
            let start = Date.now();
            let timer = setInterval(function () {
                let timePassed = Date.now() - start;

                if (timePassed >= 2000) {
                    clearInterval(timer);
                    return;
                }

                if (timePassed < 500) em.innerText = "Typing...";
                //else if (timePassed > 500 && timePassed < 500) em.innerText = "Typing..";
                //else if (timePassed > 1000 && timePassed < 1500) em.innerText = "Typing...";
                else if (timePassed > 1500) em.innerHTML = "&nbsp;";

                //console.log(timePassed);
            }, 20);
        }
    }
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("Text").oninput = function (e) {
    let button = document.getElementById("Send");
    var hasIsGroup = button.hasAttribute("isGroup");
    if (hasIsGroup) {
        var isGroup = button.getAttribute("isGroup");
        if (isGroup == "true") {
            var groupId = button.getAttribute("groupId");
            var isPrivateGroup = IsPrivateGroup(groupId);
            if (isPrivateGroup) {
                var logins = JSON.parse(GetLogins(groupId));
                var login = GetCurrentLogin();
                var otherlogin = "";
                for (var i = 0; i < logins.length; i++) {
                    if (logins[i] != login) otherlogin = logins[i];
                }
                connection.invoke("SendToUserForTyping", parseInt(groupId), otherlogin).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        } else {
            var userId = button.getAttribute("userId");
        }
    }
}

document.getElementById("Send").onclick = function (e) {
    let button = e.target;
    var hasIsGroup = button.hasAttribute("isGroup");
    if (hasIsGroup) {
        var isGroup = button.getAttribute("isGroup");
        if (isGroup == "true") {
            var groupId = button.getAttribute("groupId");
            var text = document.getElementById("Text").value;
            if (groupId != 0 && text != "") {
                document.getElementById("Text").value = "";
                var logins = JSON.parse(GetLogins(groupId));
                var curUserId = GetUserId();

                connection.invoke("AddToGroupByLogins", logins, groupId.toString()).catch(function (err) {
                    return console.error(err.toString());
                }).then(function () {
                    $.ajax({
                        type: "POST",
                        url: _sendTextMessageToGroup,
                        data: {
                            groupId: groupId,
                            text: text
                        },
                        dataType: "text",
                        success: function (response) {
                            if (!response) alert("Server Error!");
                        },
                        failure: function (response) {
                            alert(response.responseText);
                        },
                        error: function (response) {
                            alert(response.responseText);
                        }
                    });
                }).then(function () {
                    connection.invoke("SendToGroup", parseInt(curUserId), groupId.toString(), text).catch(function (err) {
                        return console.error(err.toString());
                    });
                });
            }
        } else {
            var userId = button.getAttribute("userId");
            var text = document.getElementById("Text").value;
            if (userId != 0 && text != "") {
                $.ajax({
                    type: "POST",
                    url: _sendTextMessageToUser,
                    data: {
                        userId: userId,
                        text: text
                    },
                    dataType: "text",
                    success: function (response) {
                        if (response == 0) alert("Server Error!");
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    },
                    error: function (response) {
                        alert(response.responseText);
                    }
                });
                var curLogin = GetCurrentLogin();
                var login = GetLogin(userId);

                var logins = [curLogin, login];

                var curUserId = GetUserId();

                var userIds = [parseInt(curUserId), parseInt(userId)];

                connection.invoke("SendToUserLogins", userIds, logins, text).catch(function (err) {
                    return console.error(err.toString());
                }).then(function () {

                });

            }
        }
    }
}

$("#btnFile").click(function () {
    $("#File").click();
});
$("#File").change(function () {
    var messages = document.getElementById("messages");
    var hasIsGroup = messages.hasAttribute("isGroup");
    if (hasIsGroup) {
        var groupId = messages.getAttribute("groupId");
        var attachment = $("#File")[0].files[0];
        var logins = JSON.parse(GetLogins(groupId));
        var curUserId = GetUserId();

        connection.invoke("AddToGroupByLogins", logins, groupId.toString()).catch(function (err) {
            return console.error(err.toString());
        }).then(function () {
            connection.invoke("SendAttachmentToGroup", parseInt(curUserId), groupId.toString(), attachment.name).catch(function (err) {
                return console.error(err.toString());
            });
        }).then(function () {
            formData = new FormData();
            formData.append("file", attachment);
            formData.append("groupId", groupId);
            $.ajax({
                type: "POST",
                url: sendAttachment,
                data: formData,
                enctype: "multipart/form-data",
                contentType: false,
                processData: false,
                success: function (response) {
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }).then(function () {
            $("#File").val(null);
        });
    }
});

function GetLogin(userId) {
    var result = "";
    $.ajax({
        async: false,
        type: "GET",
        url: _getLogin,
        data: {
            userId: userId
        },
        dataType: "text",
        success: function (response) {
            result = response;
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    return result;
}

function GetCurrentLogin() {
    var result = "";
    $.ajax({
        async: false,
        type: "GET",
        url: _getCurrentLogin,
        data: {
        },
        dataType: "text",
        success: function (response) {
            result = response;
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    return result;
}

function GetLogins(groupId) {
    var result = "";
    $.ajax({
        async: false,
        type: "GET",
        url: _getLogins,
        data: {
            groupId: groupId
        },
        dataType: "text",
        success: function (response) {
            result = response;
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    return result;
}

function GetUserId() {
    var result = "";
    $.ajax({
        async: false,
        type: "GET",
        url: _getUserId,
        data: {
        },
        dataType: "text",
        success: function (response) {
            result = response;
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    return result;
}

function IsPrivateGroup(groupId) {
    var result = false;
    $.ajax({
        async: false,
        type: "GET",
        url: isPrivateGroup,
        data: {
            groupId: groupId
        },
        dataType: "text",
        success: function (response) {
            result = response;
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    return result;
}