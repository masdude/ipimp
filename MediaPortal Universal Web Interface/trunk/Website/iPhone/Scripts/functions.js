//version 5.1.0

function logout() {
    document.location = '#_Logout';
    var btn = document.getElementById('aspBtnLogout');
    if (btn == null)
        alert('Button not found!');
    else
        btn.click();
} 

function createuser() {
    var username = document.getElementById('jsUsername').value;
    var password1 = document.getElementById('jsPassword1').value;
    var password2 = document.getElementById('jsPassword2').value;
    var recorder = document.getElementById('jsRecorder').checked;
    var watcher = document.getElementById('jsWatcher').checked;
    var deleter = document.getElementById('jsDeleter').checked;
    var remoter = document.getElementById('jsRemoter').checked;
    var admin = document.getElementById('jsAdmin').checked;

    WA.Request('Admin/UserManagementAddUserResult.aspx?user=' + username + '&newpass=' + password1 + '&confpass=' + password2 + '&recorder=' + recorder + '&watcher=' + watcher + '&deleter=' + deleter + '&remoter=' + remoter + '&admin=' + admin + '#_CreateUser', null, -1, true, null);
    return false;
}

function deleteuser() {
    var doc = document.getElementById('jsMPUser');
    var o = doc.childNodes;
    var username = o[0].firstChild.innerHTML;

    WA.Request('Admin/UserManagementDeleteConfirm.aspx?username=' + username + '#_DeleteUserConfirm', null, -1, true, null);
    return false;
}

function changepassword() {
    var oldpass = document.getElementById('jsOldPassword').value;
    var newpass = document.getElementById('jsNewPassword1').value;
    var confpass = document.getElementById('jsNewPassword2').value;

    WA.Request('Admin/UserManagementChangePasswordResult.aspx?oldpass=' + oldpass + '&newpass=' + newpass + '&confpass=' + confpass + '#_ChangePass', null, -1, true, null);
    return false;
}

function addmpclient() {
    var friendly = document.getElementById('jsFriendly').value;
    var hostname = document.getElementById('jsHostname').value;
    var port = document.getElementById('jsPort').value;
    var macaddress = document.getElementById('jsMAC').value;
    var usemovpics = document.getElementById('jsUseMovingPictures').checked;
    var usetvseries = document.getElementById('jsUseTVSeries').checked;

    WA.Request('Admin/ClientManagementAddResult.aspx?friendly=' + friendly + '&hostname=' + hostname + '&port=' + port + '&macaddress=' + macaddress + '&usemovpics=' + usemovpics + '&usetvseries=' + usetvseries + '#_ClientAddResult', null, -1, true, null);
    return false;
}

function delmpclient() {
    var doc = document.getElementById('jsMPClient');
    var o = doc.childNodes;
    var friendly = o[0].firstChild.innerHTML;

    WA.Request('Admin/ClientManagementDeleteConfirm.aspx?friendly=' + friendly + '#_ClientDeleteConfirm', null, -1, true, null);
    return false;
}

function updmpclient() {
    var doc = document.getElementById('jsMPClient');
    var o = doc.childNodes;
    var friendly = o[0].firstChild.innerHTML;

    WA.Request('Admin/ClientManagementUpdate.aspx?friendly=' + friendly + '#_ClientUpdate', null, -1, true, null);
    return false;
}

function updmpclient2() {
    var friendly = document.getElementById('jsFriendly').value;
    var hostname = document.getElementById('jsHostname').value;
    var port = document.getElementById('jsPort').value;
    var macaddress = document.getElementById('jsMAC').value;
    var usemovpics = document.getElementById('jsUseMovingPictures').checked;
    var usetvseries = document.getElementById('jsUseTVSeries').checked;

    WA.Request('Admin/ClientManagementUpdateResult.aspx?friendly=' + friendly + '&hostname=' + hostname + '&port=' + port + '&macaddress=' + macaddress + '&usemovpics=' + usemovpics + '&usetvseries=' + usetvseries + '#_ClientUpdateResult', null, -1, true, null);
    return false;
}

function tvsearch(group) {
    document.getElementById('jsLoading').style.visibility = 'visible';
    
    var search = document.getElementById('jsTVSearchText').value;
    var doc = document.getElementById('jsTVSearchGenre');
    var o = doc.childNodes;
    var genre = o[0].firstChild.innerHTML;
    var desc = document.getElementById('jsSearchDesc').checked;
    
    WA.Request('TVGuide/SearchTVGuideResults.aspx?group=' + group + '&search=' + search + '&genre=' + genre + '&desc=' + desc + '#_SearchResults' + group, null, -1, true, null);
    return false;
}

function radiosearch(group) {

    document.getElementById('jsLoading').style.visibility = 'visible';

    var search = document.getElementById('jsRadioSearchText').value;
    var doc = document.getElementById('jsRadioSearchGenre');
    var o = doc.childNodes;
    var genre = o[0].firstChild.innerHTML;
    var desc = document.getElementById('jsRadioSearchDesc').checked;

    WA.Request('RadioGuide/SearchRadioGuideResults.aspx?group=' + group + '&search=' + search + '&genre=' + genre + '&desc=' + desc + '#_RadioSearchResults' + group, null, -1, true, null);
    return false;
}

function deletemulti() {
    var cbResults = '';
    var doc = document.getElementsByTagName('input');
    for (var i = 0; i < doc.length; i++) {
        if (doc[i].checked == true) {
            cbResults += doc[i].value + '-';
        }
    }
    WA.Request('Recording/RecordingsMultiDeleteConfirm.aspx?ids=' + cbResults + '#_RecDeleteConfirm', null, -1, true, null);
    return false;
}

function playtracks(wadiv, friendly, filter) {

    div = document.getElementById(wadiv);
    inputs = div.getElementsByTagName('input');
    tracks = '';

    for (i = 0; i < inputs.length; i++) {
        if (inputs[i].id.substring(0, 9) == 'jsShuffle') { shuffle = inputs[i].checked; }
        if (inputs[i].id.substring(0, 9) == 'jsEnqueue') { enqueue = inputs[i].checked; }
        if (inputs[i].id.substring(0, 10) == 'MusicTrack') {
            if (inputs[i].checked == true) { tracks += inputs[i].value + '+'; }
        }
    }
    
    if (tracks == '') {
        for (j = 0; j < inputs.length; j++) {
            if (inputs[j].id.substring(0, 10) == 'MusicTrack') { tracks += inputs[j].value + '+'; }
        }
    }
    
    WA.Request('MPClient/MyMusicPlayTracks.aspx?friendly=' + friendly + '&filter=' + filter + '&shuffle=' + shuffle + '&enqueue=' + enqueue + '&tracks=' + tracks + '#_MPClientPlayTracks', null, -1, true, null);
    return false;
}

function musicsearch(friendly) {
    var artist = document.getElementById('jsMusicSearchArtist').value;
    var album = document.getElementById('jsMusicSearchAlbum').value;
    var track = document.getElementById('jsMusicSearchTrack').value;
    var genre = document.getElementById('jsMusicSearchGenre').value;
    WA.Request('MPClient/MyMusicListTracksForSearch.aspx?friendly=' + friendly + '&search=' + escape(artist + ',' + album + ',' + track + ',' + genre) + '&start=0#_MyMusicSearchResults', null, -1, true, null);
    return false;
}

function saveplaylist(friendly) {
    var filename = document.getElementById('jsSaveFilename').value;
    WA.Request('MPClient/MyMusicSavePlaylistResult.aspx?friendly=' + friendly + '&filename=' + filename + '#_MyMusicSavePlaylistResult', null, -1, true, null);
    return false;
}

function sendmessage(friendly) {
    var message = document.getElementById('jsMessage').value;
    WA.Request('MPClient/MPClientSendMessageResult.aspx?friendly=' + friendly + '&message=' + message + '#_MPClientSendMessageResult', null, -1, true, null);
    return false;
}

function sendkeystring(friendly) {
    keystring = document.getElementById('jsKeyString').value;
    WA.Request('MPClient/MCERemoteControlButton.aspx?friendly=' + friendly + '&button=' + keystring + '#_MCEButton', null, -1, false, null);
    return false;
}

function updatesettings() {
    var doc = document.getElementById('jsPageSize');
    var o = doc.childNodes;
    var pagesize = o[0].firstChild.innerHTML;

    doc = document.getElementById('jsRecOrder');
    o = doc.childNodes;
    var order = o[0].firstChild.innerHTML;

    doc = document.getElementById('jsRecentSize');
    o = doc.childNodes;
    var recent = o[0].firstChild.innerHTML;

    doc = document.getElementById('jsGuideDays');
    o = doc.childNodes;
    var guidedays = o[0].firstChild.innerHTML;

    var client = document.getElementById('jsMPClientEnable').checked;
    var server = document.getElementById('jsTVServerEnable').checked;
    var submenu = document.getElementById('jsMPClientSubmenu').checked;
    var recsubmenu = document.getElementById('jsRecsSubmenu').checked;
    var myvideos = document.getElementById('jsMyVideos').checked;
    var sortlists = document.getElementById('jsSortLists').checked;
    
    WA.Request('Admin/ManageSettingsResult.aspx?pagesize=' + pagesize + '&order=' + order + '&client=' + client + '&server=' + server + '&submenu=' + submenu + '&recsubmenu=' + recsubmenu + '&recent=' + recent + '&myvideos=' + myvideos + '&sortlists=' + sortlists + '&guidedays=' + guidedays + '#_SettingsResult', null, -1, true, null);
    
    return false;
}

function manualrecord(channelID) {
    var doc = document.getElementById('jsStartDate');
    var o = doc.childNodes;
    var startDate = o[0].firstChild.innerHTML;
    var startTime = document.getElementById('jsStartTime').value;
    var schedName = document.getElementById('jsSchedName').value;
    var duration = document.getElementById('jsDuration').value;

    WA.Request('TVGuide/RecordManualConfirm.aspx?channel=' + channelID + '&schedName=' + schedName + '&startdate=' + startDate + '&starttime=' + startTime + '&duration=' + duration + '#_RecordManualConfirm', null, -1, true, null);
    return false;
}

function manualradiorecord(channelID) {
    var doc = document.getElementById('jsRadioStartDate');
    var o = doc.childNodes;
    var startDate = o[0].firstChild.innerHTML;
    var startTime = document.getElementById('jsRadioStartTime').value;
    var schedName = document.getElementById('jsRadioSchedName').value;
    var duration = document.getElementById('jsRadioDuration').value;

    WA.Request('RadioGuide/RadioRecordManualConfirm.aspx?channel=' + channelID + '&schedName=' + schedName + '&startdate=' + startDate + '&starttime=' + startTime + '&duration=' + duration + '#_RadioRecordManualConfirm', null, -1, true, null);
    return false;
}

function changecolour() {
    var doc = document.getElementById('jsAppearance');
    var o = doc.childNodes;
    var colour = o[0].firstChild.innerHTML;

    WA.Request('Admin/AppearanceResult.aspx?colour=' + colour + '#_AppearanceResult', null, -1, true, null);
    return false;
}

function startCountdown(iCount) {
    if (iCount > 1) {
        iCount = iCount - 1;
        document.getElementById('tvtimer').innerText = iCount;
        setTimeout('startCountdown(' + iCount + ')', 1000);
    }
    else 
    {
        WA.Request('Streaming/WatchStream.aspx?_WatchStream', null, -1, true, null);
        return false;
    }
}
