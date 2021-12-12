var j = 0;
$("#SearchGroup").on("input", function () {
    if ($("#SearchGroup").val() != "") {
        RefreshListGroupByName($("#SearchGroup").val(), 0);
        RefreshPaginationGroupCardByName($("#SearchGroup").val(), 0);
    }
    else {
        RefreshListGroupCard(0);
        RefreshPaginationGroupCard(0);
    }
    j = 0;
});
function RefreshListGroupCard(index) {
    $.ajax({
        type: "GET",
        url: listGroupCard,
        data: {
            index: index
        },
        dataType: "text",
        success: function (response) {
            $("#ListGroupCard").html(response);

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
        url: listGroupCard,
        data: {
            name: name,
            index: index
        },
        dataType: "text",
        success: function (response) {
            $("#ListGroupCard").html(response);

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
function RefreshPaginationGroupCard(index) {
    $.ajax({
        type: "GET",
        url: paginationGroup,
        data: {
            index: index,
            interval: 3
        },
        dataType: "text",
        success: function (response) {
            $("#paginationGroup").html(response);
            $("#paginationGroup #previousGroup").click(function () {
                j--;
                if (j < 0) j = 0;
                RefreshListGroupCard(j);
                if (!((j + 1) % 3)) {
                    RefreshPaginationGroupCard(j - 2);
                }
            });
            $("#paginationGroup #nextGroup").click(function () {
                j++;
                maxGroup = GetCountGroupPagination() - 1;
                if (j > maxGroup) j = maxGroup;
                RefreshListGroupCard(j);
                if (!(j % 3)) {
                    RefreshPaginationGroupCard(j);
                }
            });
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
                    RefreshListGroupCard(j);
                }
            });
            $("#paginationGroup li[name='moveto']").click(function (e) {
                let li = e.target;
                while (!/page-item/.test(li.className)) {
                    li = li.parentNode;
                }
                j = li.getAttribute("number");
                RefreshListGroupCard(j);
                RefreshPaginationGroupCard(j);
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
function RefreshPaginationGroupCardByName(name, index) {
    $.ajax({
        type: "GET",
        url: paginationGroup,
        data: {
            name: name,
            index: index,
            interval: 3
        },
        dataType: "text",
        success: function (response) {
            $("#paginationGroup").html(response);
            $("#paginationGroup #previousGroup").click(function () {
                j--;
                if (j < 0) j = 0;
                RefreshListGroupByName($("#SearchGroup").val(), j);
                if (!((j + 1) % 3)) {
                    RefreshPaginationGroupCardByName($("#SearchGroup").val(), j - 2);
                }
            });
            $("#paginationGroup #nextGroup").click(function () {
                j++;
                maxGroup = GetCountGroupPaginationByName($("#SearchGroup").val()) - 1;
                if (j > maxGroup) j = maxGroup;
                RefreshListGroupByName($("#SearchGroup").val(), j);
                if (!(j % 3)) {
                    RefreshPaginationGroupCardByName($("#SearchGroup").val(), j);
                }
            });
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
                    RefreshListGroupByName($("#SearchGroup").val(), j)
                }
            });
            $("#paginationGroup li[name='moveto']").click(function (e) {
                let li = e.target;
                while (!/page-item/.test(li.className)) {
                    li = li.parentNode;
                }
                j = li.getAttribute("number");
                RefreshListGroupByName($("#SearchGroup").val(), j);
                RefreshPaginationGroupCardByName($("#SearchGroup").val(), j);
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
function GetCountGroupPagination(interval = 3) {
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
function GetCountGroupPaginationByName(name, interval = 3) {
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
RefreshListGroupCard(0);
RefreshPaginationGroupCard(0);