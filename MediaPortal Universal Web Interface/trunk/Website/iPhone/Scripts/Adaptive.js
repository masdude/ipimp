WA.AddEventListener("beginslide", function(evt) {
    if (evt.context[1][0] == "waSettings" || evt.context[1][0] == "waSettingsResult") {
        document.getElementById('iHeader').style.background = "#FF8000";
        document.getElementById('WebApp').style.background = "#F5D0A9 url(WebApp/Design/img/bg.png)";
    } else if (evt.context[1][0] == "waClientMenu" || evt.context[1][0] == "waClientAddMenu" || evt.context[1][0] == "waClientAddResult" || evt.context[1][0] == "waClientDeleteMenu" || evt.context[1][0] == "waClientDeleteConfirm" || evt.context[1][0] == "waClientDelResult" || evt.context[1][0] == "waClientUpdateMenu" || evt.context[1][0] == "waClientUpdate" || evt.context[1][0] == "waClientUpdateResult") {
        document.getElementById('iHeader').style.background = "#8000FF";
        document.getElementById('WebApp').style.background = "#D0A9F5 url(WebApp/Design/img/bg.png)";
    } else if (evt.context[1][0] == "waChangePasswordMenu" || evt.context[1][0] == "waChangePass" || evt.context[1][0] == "waCreateUserMenu" || evt.context[1][0] == "waCreateUser" || evt.context[1][0] == "waDeleteUserMenu" || evt.context[1][0] == "waDeleteUserConfirm" || evt.context[1][0] == "waDeleteUserResult") {
        document.getElementById('iHeader').style.background = "#FF0000";
        document.getElementById('WebApp').style.background = "#F5A9A9 url(WebApp/Design/img/bg.png)";
    } else if (evt.context[1][0] == "waAboutiPiMP") {
        document.getElementById('iHeader').style.background = "#088A08";
        document.getElementById('WebApp').style.background = "#81F781 url(WebApp/Design/img/bg.png)";
    } else {
        document.getElementById('iHeader').style.background = "";
        document.getElementById('WebApp').style.background = "";
    }
});
