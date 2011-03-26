WA.AddEventListener("load", function() {
    var div = document.getElementById('colour');
    var colour = div.innerHTML;

    switch (colour) {
        case "black":
        case "svart":
        case "schwarz":
        case "noir":
        case "zwart":
        case "sort":
            header = "#1C1C1C";
            webapp = "#6E6E6E";
            break;
        case "green":
        case "grön":
        case "grün":
        case "vert":
        case "groen":
        case "grøn":
            header = "#088A08";
            webapp = "#81F781";
            break;
        case "orange":
        case "oranje":
            header = "#FF8000";
            webapp = "#F5D0A9";
            break;
        case "purple":
        case "purpur":
        case "violet":
        case "paars":
        case "lilla":
            header = "#7401DF";
            webapp = "#D0A9F5";
            break;
        case "pink":
        case "rosa":
        case "rose":
        case "roze":
            header = "#FF00FF";
            webapp = "#F5A9F2";
            break;
        case "red":
        case "röd":
        case "rot":
        case "rouge":
        case "rood":
        case "rød":
            header = "#B40404";
            webapp = "#F6CECE";
            break;
        case "blue":
        case "blå":
        case "blau":
        case "bleu":
        case "blauw":
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
