<!--
    Copyright (C) 2008-2009 Martin van der Boon
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details. 

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
-->

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
    <link rel="apple-touch-icon" href="images/apple-touch-icon.png" />
    <link rel="apple-touch-startup-image" href="images/apple-touch-startup.png" />
    <script type="text/javascript" src="WebApp/Action/Logic.js"></script>
    <script type="text/javascript" src="Scripts/functions.js"></script>
    <script type="text/javascript" src="Scripts/Adaptive.js"></script>
</head>
<body onload="changecolour">
    <div id="WebApp">
        <div id="iHeader">
            <span id="waHeadTitle"><asp:literal runat="server" ID="litTitle" /></span>
            <a href="#" id="waBackButton"><asp:literal runat="server" ID="litBack" /></a>
            <a href="#" id="waHomeButton"><asp:literal runat="server" ID="litHome" /></a>
        </div>
		<div id="iGroup">
            <div class="iLayer" id="waHome">
                <div class="iMenu">
                    <h3><asp:literal runat="server" ID="litMainMenu" Visible="false"/></h3>
                    <ul class="iArrow"> 
                        <asp:PlaceHolder runat="server" ID="phMainMenu" />
                    </ul>
                    <h3><asp:literal runat="server" ID="litCliMenu" Visible="false" /></h3>
                    <ul class="iArrow"> 
                        <asp:PlaceHolder runat="server" ID="phCliMenu" />
                    </ul>
                    <h3><asp:literal runat="server" ID="litAdminMenu" Visible="false" /></h3>
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
        <div id="colour"><asp:literal runat="server" id="litColour" /></div>
    </form>
</body>
</html>
