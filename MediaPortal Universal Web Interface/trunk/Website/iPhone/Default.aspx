<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="Website._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=320; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"/>
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" href="WebApp/Design/Render.css" />
    <link rel="Stylesheet" href="Scripts/remote.css" />
    <link rel="Stylesheet" href="Scripts/starbar.css" />
    <script type="text/javascript" src="WebApp/Action/Logic.js"></script>
    <script type="text/javascript" src="Scripts/functions.js"></script>
</head>
<body>
    <div id="WebApp">
        <div id="iHeader">
            <span id="waHeadTitle"><asp:literal runat="server" ID="litTitle" /></span>
            <a href="#" id="waBackButton"><asp:literal runat="server" ID="litBack" /></a>
            <a href="#" id="waHomeButton"><asp:literal runat="server" ID="litHome" /></a>
        </div>
		<div id="iGroup">
            <div class="iLayer" id="waHome">
                <div class="iMenu">
                    <h3>Main menu</h3>
                    <ul class="iArrow"> 
                        <asp:PlaceHolder runat="server" ID="phMainMenu" />
                    </ul>
                    <ul class="iArrow"> 
                        <asp:PlaceHolder runat="server" ID="phAdminMenu" />
                    </ul>
                </div>
        		<div>
                    <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6722080" rel="Action" class="iButton iBOrange" target="_blank"><asp:literal runat="server" ID="litDonate" /></a>
                    <a href="javascript:logout()" rel="Back" class="iButton iBWarn"><asp:literal runat="server" ID="litLogout" /></a>
                </div>                
            </div>
        </div>
    </div>
    <form id="form1" runat="server">
    <div>
        <asp:PlaceHolder runat="server" ID="phHiddenControls" />
        <asp:Button ID="aspBtnLogout" runat="server" style="display:none;" />
    </div>
    </form>
</body>
</html>
