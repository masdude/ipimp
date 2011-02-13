<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SearchTVGuide.aspx.vb" Inherits="Website.SearchTVGuide1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" href="css/desktop.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:PlaceHolder ID="SearchResults" runat="server" />
            <script type="text/javascript">
                $("td[title]").tooltip({ effect: "fade", predelay: 500, position: 'bottom center', offset: [-10, 0] });

                var overlayObject = $('#record2').overlay({
                    api: true,
                    mask: { color: '#404040', loadSpeed: 100, opacity: 0.9 },
                    closeOnClick: true,
                    fixed: false,
                    onBeforeLoad: function () {
                        $.get(overlayUrl, function (data) {
                        document.getElementById('modalimage2').innerHTML = '<img src="../TVLogos/' + data.channel + '.png" height="40px" style="float:left" />';
                        document.getElementById('modaltitle2').innerHTML = '<h2>' + data.title + '</h2>';
                        document.getElementById('modaldesc2').innerHTML = '<p>' + data.description + '</p>';
                        }
                    )},
                    onClose: function () {
                        document.getElementById('modalimage2').innerHTML = '';
                        document.getElementById('modaltitle2').innerHTML = '';
                        document.getElementById('modaldesc2').innerHTML = '';
                    }
                });


                // set-up a live() event to bind click to all future triggers 
                $('.record2').live('click', function (e) {
                    overlayUrl = "ProgramDetails.aspx?programID=" + $(this).attr("id");
                    e.preventDefault();
                    // load the overlay 
                    overlayObject.load();
                    return false;
                });
            </script>
        </div>
    </form>
</body>
</html>
