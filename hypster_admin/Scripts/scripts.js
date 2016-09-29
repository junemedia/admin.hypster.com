var myWidth = 0;
var myHeight = 0;
var isOrderOn = "";

function SearchMusic()
{
    var ss = $("#txtSearchString").val();

    if ($("input[name=group_orderBy]:radio").val() != undefined)
    {
        isOrderOn = $("input[name=group_orderBy]:radio").val();
    }
    SearchMusicStr(ss);
}

function SearchMusicStr(search_string)
{
    $("#listenSlideContHolder").html("<div style='float:right;'><span style='font-size:24px; color:#959595;'>Searching...</span></div>");

    var ss = search_string;
    ss = ss.replace(" ", "+");
    ss = ss.replace("/", "+");
    ss = ss.replace("\\", "+");
    ss = ss.replace("&", "+");
    ss = ss.replace("$", "+");
    ss = ss.replace("?", "+");
    ss = ss.replace("!", "+");
    ss = ss.replace(".", "+");

    var search_url = "/search/Music?ss=" + ss;
    if (isOrderOn != "") {
        search_url += "&orderBy=" + isOrderOn;
    }
    $.ajax({
        type: "GET",
        url: search_url,
        async: true,
        success: function (data) {
            $('#listenSlideContHolder').html(data);
        }
    });
    $(document).scrollTop(750);
}

function SearchMusicStrPage(search_string, page) {

    $("#listenSlideContHolder").html("<div style='float:right; width:680px;'><span style='font-size:24px; color:#959595;'>Searching...</span></div>");

    var ss = search_string;
    ss = ss.replace(" ", "+");
    ss = ss.replace("/", "+");
    ss = ss.replace("\\", "+");
    ss = ss.replace("&", "+");
    ss = ss.replace("$", "+");
    ss = ss.replace("?", "+");
    ss = ss.replace("!", "+");
    ss = ss.replace(".", "+");

    var search_url = "/search/Music?ss=" + ss + "&page=" + page;
    if (isOrderOn != "") {
        search_url += "&orderBy=" + isOrderOn;
    }

    $.ajax({
        type: "POST",
        url: search_url,
        async: true,
        success: function (data) {
            $("#listenSlideContHolder").html(data);
        }
    });

    $(document).scrollTop(640);
}

function ListenSearchString_KeyUp(e) {
    if (window.event) {
        if (window.event.keyCode == 13) {
            SearchMusic();
        }
    }
    else {
        if (e.which == 13) {
            SearchMusic();
        }
    }
}

function showHide_AddSearch(caller)
{
    if ($("#AddSearch").css("display") == "none")
    {
        $("#AddSearch").css("display", "block");
        $("#MainSearchLn").css("height", "130px;");
        $(".contMiddle").css("display", "none");
        $("#txtSearchString").css("background-color", "#DDDDDD");
        $("#txtSearchString").css("border", "3px solid #DDDDDD");
    }
    else
    {
        $("#AddSearch").css("display", "none");
        $("#MainSearchLn").css("height", "70px;");
        $(".contMiddle").css("display", "block");
        $("#txtSearchString").css("background-color", "#FFFFFF");
        $("#txtSearchString").css("border", "3px solid #FFFFFF");
    }
}

function ShowAddToMyPlaylist(song_guid, song_title)
{
    $(document).scrollTop(0);
    var ss = "";
    try {
        if ($("#txtSearchString") != null) {
            ss = $("#txtSearchString").val();
        }
    }
    catch (ex) {
        console.log("Name: " + ex.name + "\nNumber: " + ex.number + "\nStack Trace: " + ex.stack + "\nContent: " + ex.message);
    }
    $.ajax({
        type: "POST",
        url: "/Editors/managePlaylist/SubmitAddNewSong",
        data: {
            Song_Title: decodeURIComponent(song_title),
            Song_Guid: song_guid,
            Sel_Playlist_ID: $("#playlist_id").val()
        },
        async: true,
        success: function (data) {
            window.location = "/Editors/managePlaylist/?playlist_id=" + $("#playlist_id").val();
        }
    });
}

function OpenPlayerM(params)
{
    var player_W = 1210;
    var player_H = 710;

    if (myWidth < 1210)
        player_W = myWidth;
    if (myWidth > 1310)
        player_W = 1310;

    if (myHeight < 710) {
        player_H = myHeight;
        player_W = 1210;
    }
    window.open(params, "_blank");
}

function add_new_tag()
{
    if ($("#newTagName").val() == "")
    {
        alert("Tag is empty");
        return;
    }
    var res_str = $("#newTagName").val();
    res_str = res_str.replace("http://", "");
    res_str = res_str.replace("https://", "");
    res_str = res_str.replace("http", "");
    $.ajax({
        type: "GET",
        url: "/Editors/managePlaylist/addnewtag?tag_name=" + res_str + "&playlist_id=" + $("#newTagPlst").val(),
        async: true,
        success: function (data) {
            $("#tagsMCH1").append("<a id='tg_" + data + "' class='TagI'>" + res_str + "&nbsp;<span class='delTagSpn  AddNew_Container' onclick='delete_plst_tag(\"" + data + "\")'>&nbsp;&nbsp;X&nbsp;&nbsp;</span></a>");
            $("#newTagName").val("");
        }
    });
}

function delete_plst_tag(tag_id)
{
    $.ajax({
        type: "GET",
        url: "/Editors/managePlaylist/deletePlaylistTag?tag_plst_id=" + tag_id + "&playlist_id=" + $("#newTagPlst").val(),
        async: true,
        success: function (data) {
            $("#tg_" + tag_id).css("display", "none");
        }
    });
}

function showHide_PostVideo() {
    if ($("#PostVideo_cont").css("display") == "none")
        $("#PostVideo_cont").css("display", "block");
    else
    {
        $("#YouTube_ID").val("");
        $("#PostVideo_EditTextArea1").val("");
        $("#PostVideo_cont").css("display", "none");
    }
}

function showHide_PostMusicPlayer()
{
    if ($("#PostMusicPlayer_cont").css("display") == "none")
        $("#PostMusicPlayer_cont").css("display", "block");
    else
    {
        $("#Music_User_ID").val("");
        $("#Music_Playlist_ID").val("");
        $("#PostMusicPlayer_EditTextArea1").val("");
        $("#PostMusicPlayer_cont").css("display", "none");
    }
}

function addNewTag_click() {
    if ($('#newTagName').val() == "") {
        alert("Tag is empty");
        return;
    }

    var res_str = $('#newTagName').val();
    res_str = res_str.replace("http://", "");
    res_str = res_str.replace("https://", "");
    res_str = res_str.replace("http", "");

    $.ajax({
        type: "POST",
        url: "/Editors/managePost/addnewtag?tag_name=" + res_str + "&article_id=" + $('#newTagArticle').val(),
        async: true,
        success: function (data) {
            $("#tagsHolder1").prepend("<a class='TagI' href='/tags/" + res_str + "'>" + res_str + "</a>");
            $("#newTagName").val("");
        }
    });
}

function del_news_tag(tag_plst_id) {
    if (confirm('Are you sure?') == true) {
        $.ajax({
            type: "POST",
            url: "/Editors/managePost/deletetag?tag_plst_id=" + tag_plst_id + "&article_id=" + $('#newTagArticle').val(),
            async: true,
            success: function (data) {
                $("#tgn" + tag_plst_id).css("display", "none");
            }
        });
    }
}

function validateCloneList(playlistId, playlistName, cloneTo)
{
    var choice = $("input:radio[name=choice]:checked").val();
    var errMsg = "";
    $(".input_field").removeClass("error");
    if (choice == "Input Playlist ID")
    {
        if (playlistId == "") {
            errMsg += "Playlist ID\n";
            $("#Playlist_ID").addClass("error");
        }
    }
    if (choice == "Search Playlist") {
        if ($("#Music_User_ID").val() == "") {
            errMsg += "Username\n";
            $("#Music_User_ID").addClass("error");
        }
        if (playlistId == "") {
            errMsg += "Choose a Playlist\n";
            $("#Music_Playlist_ID").addClass("error");
        }
    }
    if (playlistName == "") {
        errMsg += "Playlist Name\n";
        $("#Playlist_Name").addClass("error");
    }

    if (errMsg != "")
        alert("Please Fill Up the following fields:\n" + errMsg);
    else if (isNaN(playlistId)) {
        alert("Please make sure the Playlist ID is a valid number.");
    }
    else {
        if (confirm('Are you sure?')) {
            $.ajax({
                type: "GET",
                url: "/Editors/managePlaylist/clonePlaylist?playlistId=" + playlistId + "&playlistName=" + playlistName + "&cloneTo=" + cloneTo,
                async: true,
                success: function (data) {
                    if (data.indexOf("Error: ") >= 0)
                        alert(data);
                    else
                        window.location = "/Editors/managePlaylist/";
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status + " " + ajaxOptions + "\n" + thrownError + "\n\n" + xhr.responseText);
                }
            });
        }
    }
}

function reset() {
    $(".input_field").removeClass("error");
    $(".input_field").each(function () {
        $(this).val("");
    });
    $("option", "#Music_Playlist_ID").remove();
}

function getPlaylists() {
    var username = $("#Music_User_ID").val();
    $.ajax({
        type: "GET",
        url: "/Editors/managePost/GetPlaylistsByUsername?username=" + username,
        async: true,
        dataType: "json",
        success: updatePlaylistOptions
    });
}

function updatePlaylistOptions(lists) {
    var current;
    var option;
    var selectEl = $("#Music_Playlist_ID");

    // clear out current options
    selectEl.empty();

    if (lists.length > 0) {
        // add empty option
        option = $("<option></option>").attr("value", "").text("- Select one -");
        selectEl.append(option);

        for (var i = 0; i < lists.length; i++) {
            current = lists[i];
            option = $("<option></option>").attr("value", current.id).text(current.name);
            selectEl.append(option);
        }
    }
    else
        option = $("<option></option>").attr("value", "").text("");
}

function SearchUser()
{
    $("#serUserPar").removeClass("error");

    if ($("#serUserPar").val() == "") {
        $("#serUserPar").addClass("error");
        alert("Please Fill Up the textbox.");
        return false;
    }
    else
    {
        //console.log($("#SearchFor").val() + "; " + validateEmail($("#serUserPar").val()));
        if ($("#SearchFor").val() == "serUserEmail" && !validateEmail($("#serUserPar").val()))
        {
            $("#serUserPar").addClass("error");
            alert("Please Verify if the email format is correct.");
            return false;
        }
        else if ($("#SearchFor").val() == "serUserID" && isNaN($("#serUserPar").val()))
        {
            $("#serUserPar").addClass("error");
            alert("Please Verify this is a Number.");
            return false;
        }
        else
            return true;
    }
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function validatePassword(password) {
    var re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*?[0-9_\W])[a-zA-Z\d]{8,}$/;
    return re.test(password);
}

function validateUserInfo() {
    var errMsg = "", errMsg2 = "";
    $(".newUser_fields").removeClass("error");
    if ($("#user").val() == "") {
        $("#user").addClass("error");
        errMsg += "Username\n";
    }
    if ($("#pwd").val() == "") {
        $("#pwd").addClass("error");
        errMsg += "Password\n";
    }
    else {
        if ($("#pwd").val().length < 8 || !validatePassword($("#pwd").val())) {
            $("#pwd").addClass("error");
            errMsg2 += "Password must be at least 8 characters long AND must have at least 1 Upper Case, 1 Lower Case characters, and 1 Number.\n";
        }
    }
    if ($("#name").val() == "") {
        $("#name").addClass("error");
        errMsg += "Name\n";
    }
    if ($("#email").val() == "") {
        $("#email").addClass("error");
        errMsg += "Email Address\n";
    }
    else {
        if (!validateEmail($("#email").val())) {
            $("#email").addClass("error");
            errMsg2 += "Please make sure the email address is in correct format.\n";
        }
    }

    if (errMsg != "") {
        alert("Please Fill Up the following fields:\n" + errMsg);
        return false;
    }
    else if (errMsg2 != "") {
        alert("Please Correct the following Errors:\n" + errMsg2);
        return false;
    }
    else
        return true;
}

function validateUserInfo(user_id) {
    var errMsg = "", errMsg2 = "";
    $(".newUser_fields").removeClass("error");
    if ($("#username").val() == "") {
        $("#username").addClass("error");
        errMsg += "Username\n";
    }
    if ($("#password").val() == "") {
        $("#password").addClass("error");
        errMsg += "Password\n";
    }
    else {
        if ($("#password").val().length < 8 || !validatePassword($("#password").val())) {
            $("#password").addClass("error");
            errMsg2 += "Password must be at least 8 characters long AND must have at least 1 Upper Case, 1 Lower Case characters, and 1 Number.\n";
        }
    }
    if ($("#name").val() == "") {
        $("#name").addClass("error");
        errMsg += "Name\n";
    }
    if ($("#email").val() == "") {
        $("#email").addClass("error");
        errMsg += "Email Address\n";
    }
    else {
        if (!validateEmail($("#email").val())) {
            $("#email").addClass("error");
            errMsg2 += "Please make sure the email address is in correct format.\n";
        }
    }

    if (errMsg != "")
    {
        alert("Please Fill Up the following fields:\n" + errMsg);
    }
    else if (errMsg2 != "") {
        alert("Please Correct the following Errors:\n" + errMsg2);
    }
    else
    {
        var user_info = $("#username").val() + "," + $("#password").val() + "," + $("#name").val() + "," + $("#email").val() + "," + $("#adminLevel").val();
        $.ajax({
            type: "POST",
            url: "/Administrators/manageUsers/updateUser?user_id=" + user_id + "&user_info=" + user_info,
            async: true,
            success: function (data) {
                $("#modifyRslt").html(data);
                $("#modifyRslt").fadeIn(1000);
                $("#modifyRslt").fadeOut(3000);
            }
        });
    }
}

function pageClick(num, num2)
{
    var current = parseInt($("#showPage").val());
    $(".pageButton").css("display", "");
    $("#page_" + current).css("display", "none");
    $("#page_" + eval(current + num)).css("display", "");
    $("#showPage").val(eval(current + num));
    if (eval(current + num) === 0)
        $("#PrevBtn").css("display", "none");
    if (eval(current + num) === (num2-1))
        $("#NextBtn").css("display", "none");
}