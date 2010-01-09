WA.AddEventListener("beginslide", function(evt) {
    if (evt.context[1][0] == "waSettings" || evt.context[1][0] == "waSettingsResult") {
        document.getElementById('iHeader').style.background = "#FF8000";
        document.getElementById('WebApp').style.background = "#F5D0A9";
    } else if (evt.context[1][0] == "waClientMenu" || evt.context[1][0] == "waClientAddMenu" || evt.context[1][0] == "waClientAddResult" || evt.context[1][0] == "waClientDeleteMenu" || evt.context[1][0] == "waClientDeleteConfirm" || evt.context[1][0] == "waClientDelResult" || evt.context[1][0] == "waClientUpdateMenu" || evt.context[1][0] == "waClientUpdate" || evt.context[1][0] == "waClientUpdateResult") {
        document.getElementById('iHeader').style.background = "#9A2EFE";
        document.getElementById('WebApp').style.background = "#D0A9F5";
    } else {
        document.getElementById('iHeader').style.background = "";
        document.getElementById('WebApp').style.background = "";
    }
});
