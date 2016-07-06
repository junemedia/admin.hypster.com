﻿var myWidth = 0;
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
        //caller.innerHTML = "Hide";
        $("#txtSearchString").css("background-color", "#DDDDDD");
        $("#txtSearchString").css("border", "3px solid #DDDDDD");
    }
    else
    {
        $("#AddSearch").css("display", "none");
        $("#MainSearchLn").css("height", "70px;");
        $(".contMiddle").css("display", "block");
        //caller.innerHTML = "Advanced Search";
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

function showHide_PostImages() {
    if ($("#PostImage_cont").css("display") == "none")
        $("#PostImage_cont").css("display", "block");
    else
        $("#PostImage_cont").css("display", "none");
}

function showHide_PostVideo() {
    if ($("#PostVideo_cont").css("display") == "none")
        $("#PostVideo_cont").css("display", "block");
    else
        $("#PostVideo_cont").css("display", "none");
}

function showHide_PostMusicPlayer() {
    if ($("#PostMusicPlayer_cont").css("display") == "none")
        $("#PostMusicPlayer_cont").css("display", "block");
    else
        $("#PostMusicPlayer_cont").css("display", "none");
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