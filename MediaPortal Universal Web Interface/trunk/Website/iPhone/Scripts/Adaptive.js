WA.AddEventListener("load", function() {
    var div = document.getElementById('colour');
    var colour = div.innerHTML;

    switch (colour) {
        case "black":
            header = "#1C1C1C";
            webapp = "#D8D8D8";
            break;
        case "green":
            header = "#088A08";
            webapp = "#81F781";
            break;
        case "orange":
            header = "#FF8000";
            webapp = "#F5D0A9";
            break;
        case "purple":
            header = "#7401DF";
            webapp = "#D0A9F5";
            break;
        case "pink":
            header = "#FF00FF";
            webapp = "#F5A9F2";
            break;
        case "red":
            header = "#B40404";
            webapp = "#F6CECE";
            break;
        case "blue":
            header = "#0B0B61";
            webapp = "#8181F7";
            break;
        default:
            header = "";
            webapp = "";
    }
    document.getElementById('iHeader').style.backgroundColor = header;
    document.getElementById('WebApp').style.backgroundColor = webapp;
}); 
