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
            alert(data);
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

function getTimezoneName() {
    var d = new Date();
    var timeSummer = new Date(Date.UTC(d.getFullYear(), 6, 30, 0, 0, 0, 0));
    var summerOffset = -1 * timeSummer.getTimezoneOffset();
    var timeWinter = new Date(Date.UTC(d.getFullYear(), 12, 30, 0, 0, 0, 0));
    var winterOffset = -1 * timeWinter.getTimezoneOffset();
    var timeZoneHiddenField;

    if (-720 == summerOffset && -720 == winterOffset) { timeZoneHiddenField = 'Dateline Standard Time'; }
    else if (-660 == summerOffset && -660 == winterOffset) { timeZoneHiddenField = 'UTC-11'; }
    else if (-660 == summerOffset && -660 == winterOffset) { timeZoneHiddenField = 'Samoa Standard Time'; }
    else if (-660 == summerOffset && -600 == winterOffset) { timeZoneHiddenField = 'Hawaiian Standard Time'; }
    else if (-570 == summerOffset && -570 == winterOffset) { timeZoneHiddenField.value = 'Pacific/Marquesas'; }
    else if (-540 == summerOffset && -600 == winterOffset) { timeZoneHiddenField.value = 'America/Adak'; }
    else if (-540 == summerOffset && -540 == winterOffset) { timeZoneHiddenField.value = 'Pacific/Gambier'; }
    else if (-480 == summerOffset && -540 == winterOffset) { timeZoneHiddenField = 'Alaskan Standard Time'; }
    else if (-480 == summerOffset && -480 == winterOffset) { timeZoneHiddenField = 'Pacific/Pitcairn'; }
    else if (-420 == summerOffset && -480 == winterOffset) { timeZoneHiddenField = 'Pacific Standard Time'; }
    else if (-420 == summerOffset && -420 == winterOffset) { timeZoneHiddenField = 'US Mountain Standard Time'; }
    else if (-360 == summerOffset && -420 == winterOffset) { timeZoneHiddenField = 'Mountain Standard Time'; }
    else if (-360 == summerOffset && -360 == winterOffset) { timeZoneHiddenField = 'Central America Standard Time'; }
    else if (-360 == summerOffset && -300 == winterOffset) { timeZoneHiddenField = 'Pacific/Easter'; }
    else if (-300 == summerOffset && -360 == winterOffset) { timeZoneHiddenField = 'Central Standard Time'; }
    else if (-300 == summerOffset && -300 == winterOffset) { timeZoneHiddenField = 'SA Pacific Standard Time'; }
    else if (-240 == summerOffset && -300 == winterOffset) { timeZoneHiddenField = 'Eastern Standard Time'; }
    else if (-270 == summerOffset && -270 == winterOffset) { timeZoneHiddenField = 'Venezuela Standard Time'; }
    else if (-240 == summerOffset && -240 == winterOffset) { timeZoneHiddenField = 'SA Western Standard Time'; }
    else if (-240 == summerOffset && -180 == winterOffset) { timeZoneHiddenField = 'Central Brazilian Standard Time'; }
    else if (-180 == summerOffset && -240 == winterOffset) { timeZoneHiddenField = 'Atlantic Standard Time'; }
    else if (-180 == summerOffset && -180 == winterOffset) { timeZoneHiddenField = 'Montevideo Standard Time'; }
    else if (-180 == summerOffset && -120 == winterOffset) { timeZoneHiddenField = 'E. South America Standard Time'; }
    else if (-150 == summerOffset && -210 == winterOffset) { timeZoneHiddenField = 'Mid-Atlantic Standard Time'; }
    else if (-120 == summerOffset && -180 == winterOffset) { timeZoneHiddenField = 'America/Godthab'; }
    else if (-120 == summerOffset && -120 == winterOffset) { timeZoneHiddenField = 'SA Eastern Standard Time'; }
    else if (-60 == summerOffset && -60 == winterOffset) { timeZoneHiddenField = 'Cape Verde Standard Time'; }
    else if (0 == summerOffset && -60 == winterOffset) { timeZoneHiddenField = 'Azores Daylight Time'; }
    else if (0 == summerOffset && 0 == winterOffset) { timeZoneHiddenField = 'Morocco Standard Time'; }
    else if (60 == summerOffset && 0 == winterOffset) { timeZoneHiddenField = 'GMT Standard Time'; }
    else if (60 == summerOffset && 60 == winterOffset) { timeZoneHiddenField = 'Africa/Algiers'; }
    else if (60 == summerOffset && 120 == winterOffset) { timeZoneHiddenField = 'Namibia Standard Time'; }
    else if (120 == summerOffset && 60 == winterOffset) { timeZoneHiddenField = 'Central European Standard Time'; }
    else if (120 == summerOffset && 120 == winterOffset) { timeZoneHiddenField = 'South Africa Standard Time'; }
    else if (180 == summerOffset && 120 == winterOffset) { timeZoneHiddenField = 'GTB Standard Time'; }
    else if (180 == summerOffset && 180 == winterOffset) { timeZoneHiddenField = 'E. Africa Standard Time'; }
    else if (240 == summerOffset && 180 == winterOffset) { timeZoneHiddenField = 'Russian Standard Time'; }
    else if (240 == summerOffset && 240 == winterOffset) { timeZoneHiddenField = 'Arabian Standard Time'; }
    else if (270 == summerOffset && 210 == winterOffset) { timeZoneHiddenField = 'Iran Standard Time'; }
    else if (270 == summerOffset && 270 == winterOffset) { timeZoneHiddenField = 'Afghanistan Standard Time'; }
    else if (300 == summerOffset && 240 == winterOffset) { timeZoneHiddenField = 'Pakistan Standard Time'; }
    else if (300 == summerOffset && 300 == winterOffset) { timeZoneHiddenField = 'West Asia Standard Time'; }
    else if (330 == summerOffset && 330 == winterOffset) { timeZoneHiddenField = 'India Standard Time'; }
    else if (345 == summerOffset && 345 == winterOffset) { timeZoneHiddenField = 'Nepal Standard Time'; }
    else if (360 == summerOffset && 300 == winterOffset) { timeZoneHiddenField = 'N. Central Asia Standard Time'; }
    else if (360 == summerOffset && 360 == winterOffset) { timeZoneHiddenField = 'Central Asia Standard Time'; }
    else if (390 == summerOffset && 390 == winterOffset) { timeZoneHiddenField = 'Myanmar Standard Time'; }
    else if (420 == summerOffset && 360 == winterOffset) { timeZoneHiddenField = 'North Asia Standard Time'; }
    else if (420 == summerOffset && 420 == winterOffset) { timeZoneHiddenField = 'SE Asia Standard Time'; }
    else if (480 == summerOffset && 420 == winterOffset) { timeZoneHiddenField = 'North Asia East Standard Time'; }
    else if (480 == summerOffset && 480 == winterOffset) { timeZoneHiddenField = 'China Standard Time'; }
    else if (540 == summerOffset && 480 == winterOffset) { timeZoneHiddenField = 'Yakutsk Standard Time'; }
    else if (540 == summerOffset && 540 == winterOffset) { timeZoneHiddenField = 'Tokyo Standard Time'; }
    else if (570 == summerOffset && 570 == winterOffset) { timeZoneHiddenField = 'Cen. Australia Standard Time'; }
    else if (570 == summerOffset && 630 == winterOffset) { timeZoneHiddenField = 'Australia/Adelaide'; }
    else if (600 == summerOffset && 540 == winterOffset) { timeZoneHiddenField = 'Asia/Yakutsk'; }
    else if (600 == summerOffset && 600 == winterOffset) { timeZoneHiddenField = 'E. Australia Standard Time'; }
    else if (600 == summerOffset && 660 == winterOffset) { timeZoneHiddenField = 'AUS Eastern Standard Time'; }
    else if (630 == summerOffset && 660 == winterOffset) { timeZoneHiddenField = 'Australia/Lord_Howe'; }
    else if (660 == summerOffset && 600 == winterOffset) { timeZoneHiddenField = 'Tasmania Standard Time'; }
    else if (660 == summerOffset && 660 == winterOffset) { timeZoneHiddenField = 'West Pacific Standard Time'; }
    else if (690 == summerOffset && 690 == winterOffset) { timeZoneHiddenField = 'Central Pacific Standard Time'; }
    else if (720 == summerOffset && 660 == winterOffset) { timeZoneHiddenField = 'Magadan Standard Time'; }
    else if (720 == summerOffset && 720 == winterOffset) { timeZoneHiddenField = 'Fiji Standard Time'; }
    else if (720 == summerOffset && 780 == winterOffset) { timeZoneHiddenField = 'New Zealand Standard Time'; }
    else if (765 == summerOffset && 825 == winterOffset) { timeZoneHiddenField = 'Pacific/Chatham'; }
    else if (780 == summerOffset && 780 == winterOffset) { timeZoneHiddenField = 'Tonga Standard Time'; }
    else if (840 == summerOffset && 840 == winterOffset) { timeZoneHiddenField = 'Pacific/Kiritimati'; }
    else { timeZoneHiddenField = 'US/Pacific'; }
    return timeZoneHiddenField;
}