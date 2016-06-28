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
    $('#listenSlideContHolder').html("<div style='float:right;'><span style='font-size:24px; color:#959595;'>Searching...</span></div>");

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
    window.open(params, '_blank');
}

function SearchMusicYTID()
{
    var ss = $("#txtSearchString_youtube").val();
    ss = ss.replace(" ", "+");
    $('#listenSlideContHolder').html("<div style='float:right; width:680px;'><span style='font-size:24px; color:#454545;'>Searching...</span></div>");

    $.ajax({
        type: "POST",
        url: "/search/MusicYTID?ss=" + ss,
        async: true,
        success: function (data) {
            $('#listenSlideContHolder').html(data);
        }
    });
    $(document).scrollTop(640);
}