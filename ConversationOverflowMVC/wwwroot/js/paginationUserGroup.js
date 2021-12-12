$(document).ready(function () {
    var i = 0;
    var j = 0;
    var max = GetCountPagination() - 1;
    var maxGroup = GetCountGroupPagination() - 1;
    $("#Search").on("input", function () {
        if ($("#Search").val() != "") {
            RefreshListUserByName($("#Search").val(), 0);
            RefreshPaginationUserByName($("#Search").val(), 0);
        }
        else {
            RefreshListUser(0);
            RefreshPaginationUser(0);
        }
        i = 0;
    });
    $("#SearchGroup").on("input", function () {
        if ($("#SearchGroup").val() != "") {
            RefreshListGroupByName($("#SearchGroup").val(), 0);
            RefreshPaginationGroupByName($("#SearchGroup").val(), 0);
        }
        else {
            RefreshListGroup(0);
            RefreshPaginationGroup(0);
        }
        j = 0;
    });
    $("#messages").on("scroll", function (e) {
        let messages = e.target;
        if (messages.scrollTop == 0) {
            var groupId = messages.getAttribute("groupId");
            var startId = messages.querySelector("div").getAttribute("Id").replace("message_", "");
            $.ajax({
                type: "GET",
                url: listMessage,
                data: {
                    groupId: groupId,
                    startId: startId,
                    count: 10
                },
                dataType: "text",
                success: function (response) {
                    var oldhtml = $("#messages").html();
                    $("#messages").html(response);
                    $("#messages").append(oldhtml);
                    $("#messages").scrollTop(52);
                    $("#messages button.download").click(function (e) {
                        let button = e.target;
                        while (button.nodeName.toLowerCase() != "button") {
                            button = button.parentNode;
                        }
                        var messageId = button.id.replace("attachment_", "");
                        var groupId = document.getElementById("messages").getAttribute("groupId");

                        location.href = "/Message/DownloadFile?groupId=" + groupId + "&messageId=" + messageId;
                    });
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    });
    function RefreshListUser(index) {
        $.ajax({
            type: "GET",
            url: listUser,
            data: {
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#listuser").html(response);
                $("#listuser .list-group-item").on("click", function (e) {
                    let a = e.target;
                    while (!/list-group-item/.test(a.className)) {
                        a = a.parentNode;
                    }

                    var obj = IsExistPrivateGroup(a.getAttribute("data-userId"));
                    if (obj.result) {
                        RefreshListMessage(obj.groupId)
                    } else {
                        RefreshUser(a.getAttribute("data-userId"));
                    }
                });

                let page = $("#pagination li[name='page'][number='" + index + "']");
                let prevPage = $("#pagination li[name='page'][number='" + (index - 1) + "']");
                let nextPage = $("#pagination li[name='page'][number='" + (index + 1) + "']");
                if (prevPage.hasClass("active")) {
                    prevPage.removeClass("active");
                }
                if (nextPage.hasClass("active")) {
                    nextPage.removeClass("active");
                }
                if (!page.hasClass("active")) {
                    page.addClass("active");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function RefreshListGroup(index) {
        $.ajax({
            type: "GET",
            url: listGroup,
            data: {
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#listgroup").html(response);
                $("#listgroup .list-group-item").on("click", function (e) {
                    let a = e.target;
                    while (!/list-group-item/.test(a.className)) {
                        a = a.parentNode;
                    }
                    RefreshListMessage(a.getAttribute("data-groupId"));
                });

                let page = $("#paginationGroup li[name='page'][number='" + index + "']");
                let prevPage = $("#paginationGroup li[name='page'][number='" + (index - 1) + "']");
                let nextPage = $("#paginationGroup li[name='page'][number='" + (index + 1) + "']");
                if (prevPage.hasClass("active")) {
                    prevPage.removeClass("active");
                }
                if (nextPage.hasClass("active")) {
                    nextPage.removeClass("active");
                }
                if (!page.hasClass("active")) {
                    page.addClass("active");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function RefreshListGroupByName(name, index) {
        $.ajax({
            type: "GET",
            url: listGroup,
            data: {
                name: name,
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#listgroup").html(response);
                $("#listgroup .list-group-item").on("click", function (e) {
                    let a = e.target;
                    while (!/list-group-item/.test(a.className)) {
                        a = a.parentNode;
                    }
                    RefreshListMessage(a.getAttribute("data-groupId"));
                });

                let page = $("#paginationGroup li[name='page'][number='" + index + "']");
                let prevPage = $("#paginationGroup li[name='page'][number='" + (index - 1) + "']");
                let nextPage = $("#paginationGroup li[name='page'][number='" + (index + 1) + "']");
                if (prevPage.hasClass("active")) {
                    prevPage.removeClass("active");
                }
                if (nextPage.hasClass("active")) {
                    nextPage.removeClass("active");
                }
                if (!page.hasClass("active")) {
                    page.addClass("active");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function RefreshListUserByName(name, index) {
        $.ajax({
            type: "GET",
            url: listUser,
            data: {
                name: name,
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#listuser").html(response);

                let page = $("#pagination li[name='page'][number='" + index + "']");
                let prevPage = $("#pagination li[name='page'][number='" + (index - 1) + "']");
                let nextPage = $("#pagination li[name='page'][number='" + (index + 1) + "']");
                if (prevPage.hasClass("active")) {
                    prevPage.removeClass("active");
                }
                if (nextPage.hasClass("active")) {
                    nextPage.removeClass("active");
                }
                if (!page.hasClass("active")) {
                    page.addClass("active");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function GetCountPagination(interval = 7) {
        var result = 0;
        $.ajax({
            async: false,
            type: "GET",
            url: getCountPagination,
            data: {
                interval: interval
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
    function GetCountGroupPagination(interval = 7) {
        var result = 0;
        $.ajax({
            async: false,
            type: "GET",
            url: getCountPaginationGroup,
            data: {
                interval: interval
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
    function GetCountPaginationByName(name, interval = 7) {
        var result = 0;
        $.ajax({
            async: false,
            type: "GET",
            url: getCountPagination,
            data: {
                name: name,
                interval: interval
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
    function GetCountGroupPaginationByName(name, interval = 7) {
        var result = 0;
        $.ajax({
            async: false,
            type: "GET",
            url: getCountPaginationGroup,
            data: {
                name: name,
                interval: interval
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
    function RefreshListMessage(groupId) {
        $.ajax({
            type: "GET",
            url: listMessageLast,
            data: {
                groupId: groupId,
                interval: 10
            },
            dataType: "text",
            success: function (response) {
                $("#messages").html(response);
                document.getElementById("messages").scrollTop = document.getElementById("messages").scrollHeight + 300;
                $("#messages button.download").click(function (e) {
                    let button = e.target;
                    while (button.nodeName.toLowerCase() != "button") {
                        button = button.parentNode;
                    }
                    var messageId = button.id.replace("attachment_", "");
                    var groupId = document.getElementById("messages").getAttribute("groupId");

                    location.href = "/Message/DownloadFile?groupId=" + groupId + "&messageId=" + messageId;
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
        $.ajax({
            type: "GET",
            url: user,
            data: {
                groupId: groupId
            },
            dataType: "text",
            success: function (response) {
                $("#usericon").html(response);
                let dot3 = document.getElementById("dot3");
                let em = dot3.getElementsByTagName("em")[0];

                em.innerHTML = "&nbsp;";
            },
            failure: function (response) {
                $("#usericon").html("");
            },
            error: function (response) {
                $("#usericon").html("");
            }
        });
        document.getElementById("Send").setAttribute("userId", 0);
        document.getElementById("Send").setAttribute("groupId", groupId);
        document.getElementById("Send").setAttribute("IsGroup", true);

        document.getElementById("messages").setAttribute("userId", 0);
        document.getElementById("messages").setAttribute("groupId", groupId);
        document.getElementById("messages").setAttribute("IsGroup", true);

        document.getElementById("usericon").setAttribute("userId", 0);
        document.getElementById("usericon").setAttribute("groupId", groupId);
        document.getElementById("usericon").setAttribute("IsGroup", true);

        document.getElementById("Send").removeAttribute("disabled");
        document.getElementById("btnFile").removeAttribute("disabled");
    }
    function RefreshUser(userId) {
        $.ajax({
            type: "GET",
            url: user2,
            data: {
                userId: userId
            },
            dataType: "text",
            success: function (response) {
                $("#usericon").html(response);

                let dot3 = document.getElementById("dot3");
                let em = dot3.getElementsByTagName("em")[0];

                em.innerText = "";
            },
            failure: function (response) {
                $("#usericon").html("");
            },
            error: function (response) {
                $("#usericon").html("");
            }
        });
        $("#messages").html("");
        document.getElementById("Send").setAttribute("userId", userId);
        document.getElementById("Send").setAttribute("groupId", 0);
        document.getElementById("Send").setAttribute("IsGroup", false);

        document.getElementById("messages").setAttribute("userId", userId);
        document.getElementById("messages").setAttribute("groupId", 0);
        document.getElementById("messages").setAttribute("IsGroup", false);
        document.getElementById("messages").setAttribute("userIds", parseInt(GetUserId()) + parseInt(userId));

        document.getElementById("usericon").setAttribute("userId", userId);
        document.getElementById("usericon").setAttribute("groupId", 0);
        document.getElementById("usericon").setAttribute("IsGroup", false);

        document.getElementById("Send").removeAttribute("disabled");
        document.getElementById("btnFile").setAttribute("disabled", "");
    }
    function IsExistPrivateGroup(userId) {
        var result;
        $.ajax({
            async: false,
            type: "GET",
            url: isExistPrivateGroup,
            data: {
                userId: userId
            },
            dataType: "text",
            success: function (response) {
                result = {
                    groupId: jQuery.parseJSON(response).groupId,
                    result: jQuery.parseJSON(response).result
                };
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
    function RefreshPaginationUser(index) {
        $.ajax({
            type: "GET",
            url: userPagination,
            data: {
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#pagination").html(response);

                $("#pagination #previous").click(function () {
                    i--;
                    if (i < 0) i = 0;
                    RefreshListUser(i);
                    if (!((i + 1) % 3)) {
                        RefreshPaginationUser(i - 2);
                    }
                });
                $("#pagination #next").click(function () {
                    i++;
                    max = GetCountPagination() - 1;
                    if (i > max) i = max;
                    RefreshListUser(i);
                    if (!(i % 3)) {
                        RefreshPaginationUser(i);
                    }
                });
                if (index == i) {
                    document.querySelector("#pagination li[name='page'][number='" + index + "']").classList.add("active")
                } else {
                    document.querySelector("#pagination li[name='page'][number='" + (index + 2) + "']").classList.add("active")
                }
                $("#pagination li[name='page']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    let li_old = document.querySelector("#pagination li[name='page'].active");
                    var isActive = /active/.test(li.className);
                    if (!isActive) {
                        i = li.getAttribute("number");
                        li_old.classList.remove("active");
                        RefreshListUser(i);
                    }
                });
                $("#pagination li[name='moveto']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    i = li.getAttribute("number");
                    RefreshListUser(i);
                    RefreshPaginationUser(i);
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function RefreshPaginationGroup(index) {
        $.ajax({
            type: "GET",
            url: groupPagination,
            data: {
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#paginationGroup").html(response)

                $("#paginationGroup #previousGroup").click(function () {
                    j--;
                    if (j < 0) j = 0;
                    if ($("#SearchGroup").val() != "") {
                        RefreshListGroupByName($("#SearchGroup").val(), j);
                        if (!((j + 1) % 3)) {
                            RefreshPaginationGroupByName($("#SearchGroup").val(), j - 2);
                        }
                    } else {
                        RefreshListGroup(j);
                        if (!((j + 1) % 3)) {
                            RefreshPaginationGroup(j - 2);
                        }
                    }
                });
                $("#paginationGroup #nextGroup").click(function () {
                    j++;
                    if ($("#SearchGroup").val() != "") {
                        maxGroup = GetCountGroupPaginationByName($("#SearchGroup").val()) - 1;
                        if (j > maxGroup) j = maxGroup;
                        RefreshListGroupByName($("#SearchGroup").val(), j);
                        if (!(j % 3)) {
                            RefreshPaginationGroupByName($("#SearchGroup").val(), j);
                        }
                    } else {
                        maxGroup = GetCountGroupPagination() - 1;
                        if (j > maxGroup) j = maxGroup;
                        RefreshListGroup(j);
                        if (!(j % 3)) {
                            RefreshPaginationGroup(j);
                        }
                    }
                });
                if (index == j) {
                    document.querySelector("#paginationGroup li[name='page'][number='" + index + "']").classList.add("active")
                } else {
                    document.querySelector("#paginationGroup li[name='page'][number='" + (index + 2) + "']").classList.add("active")
                }
                $("#paginationGroup li[name='page']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    let li_old = document.querySelector("#paginationGroup li[name='page'].active");
                    var isActive = /active/.test(li.className);
                    if (!isActive) {
                        j = li.getAttribute("number");
                        li_old.classList.remove("active");
                        RefreshListGroup(j);
                    }
                });
                $("#paginationGroup li[name='moveto']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    j = li.getAttribute("number");
                    RefreshListGroup(j);
                    RefreshPaginationGroup(j);
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function RefreshPaginationUserByName(name, index) {
        $.ajax({
            type: "GET",
            url: userPagination,
            data: {
                name: name,
                index: index
            },
            dataType: "text",
            success: function (response) {
                console.log(response);
                $("#pagination").html(response);

                $("#pagination #previous").click(function () {
                    i--;
                    if (i < 0) i = 0;
                    RefreshListUserByName($("#Search").val(), i);
                    if (!((i + 1) % 3)) {
                        RefreshPaginationUserByName($("#Search").val(), i - 2);
                    }
                });
                $("#pagination #next").click(function () {
                    i++;
                    max = GetCountPaginationByName($("#Search").val()) - 1;
                    console.log(max);
                    if (i > max) i = max;
                    RefreshListUserByName($("#Search").val(), i);
                    if (!(i % 3)) {
                        RefreshPaginationUserByName($("#Search").val(), i);
                    }
                });
                if (index == i) {
                    document.querySelector("#pagination li[name='page'][number='" + index + "']").classList.add("active")
                } else {
                    document.querySelector("#pagination li[name='page'][number='" + (index + 2) + "']").classList.add("active")
                }
                $("#pagination li[name='page']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    let li_old = document.querySelector("#pagination li[name='page'].active");
                    var isActive = /active/.test(li.className);
                    if (!isActive) {
                        i = li.getAttribute("number");
                        li_old.classList.remove("active");
                        RefreshListUserByName($("#Search").val(), i);
                    }
                });
                $("#pagination li[name='moveto']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    i = li.getAttribute("number");
                    RefreshListUserByName($("#Search").val(), i);
                    RefreshPaginationUserByName($("#Search").val(), i);
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    function RefreshPaginationGroupByName(name, index) {
        $.ajax({
            type: "GET",
            url: groupPagination,
            data: {
                name: name,
                index: index
            },
            dataType: "text",
            success: function (response) {
                $("#paginationGroup").html(response);

                $("#paginationGroup #previousGroup").click(function () {
                    j--;
                    if (j < 0) j = 0;
                    if ($("#SearchGroup").val() != "") {
                        RefreshListGroupByName($("#SearchGroup").val(), j);
                        if (!((j + 1) % 3)) {
                            RefreshPaginationGroupByName($("#SearchGroup").val(), j - 2);
                            console.log(j);
                        }
                    } else {
                        RefreshListGroup(j);
                        if (!((j + 1) % 3)) {
                            RefreshPaginationGroup(j - 2);
                        }
                    }
                });
                $("#paginationGroup #nextGroup").click(function () {
                    j++;
                    if ($("#SearchGroup").val() != "") {
                        maxGroup = GetCountGroupPaginationByName($("#SearchGroup").val()) - 1;
                        if (j > maxGroup) j = maxGroup;
                        RefreshListGroupByName($("#SearchGroup").val(), j);
                        if (!(j % 3)) {
                            RefreshPaginationGroupByName($("#SearchGroup").val(), j);
                            console.log(j);
                        }
                    } else {
                        maxGroup = GetCountGroupPagination() - 1;
                        if (j > maxGroup) j = maxGroup;
                        RefreshListGroup(j);
                        if (!(j % 3)) {
                            RefreshPaginationGroup(j);
                        }
                    }
                });
                if (index == j) {
                    document.querySelector("#paginationGroup li[name='page'][number='" + index + "']").classList.add("active")
                } else {
                    document.querySelector("#paginationGroup li[name='page'][number='" + (index + 2) + "']").classList.add("active")
                }
                $("#paginationGroup li[name='page']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    let li_old = document.querySelector("#paginationGroup li[name='page'].active");
                    var isActive = /active/.test(li.className);
                    if (!isActive) {
                        j = li.getAttribute("number");
                        li_old.classList.remove("active");
                        RefreshListGroupByName($("#SearchGroup").val(), j);
                    }
                });
                $("#paginationGroup li[name='moveto']").click(function (e) {
                    let li = e.target;
                    while (!/page-item/.test(li.className)) {
                        li = li.parentNode;
                    }
                    j = li.getAttribute("number");
                    RefreshListGroupByName($("#SearchGroup").val(), j);
                    RefreshPaginationGroupByName($("#SearchGroup").val(), j);
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

    RefreshPaginationUser(0);
    RefreshPaginationGroup(0);
    RefreshListUser(0);
    RefreshListGroup(0);
});