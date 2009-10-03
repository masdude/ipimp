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

<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="Website.iPhoneLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <meta name="viewport" content="width=320; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"/>
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" href="WebApp/Design/Render.css" />
    <script type="text/javascript" src="WebApp/Action/Logic.js"></script>
    <script type="text/javascript">
        function login() {
            var btn = document.getElementById('aspBtnLogin');
            btn.click();
        }
    </script>
</head>
<body>
    <div id="WebApp">
        <form id="login" runat="server" >
            <div id="iHeader">
                <span id="waHeadTitle"><asp:literal runat="server" ID="litTitle" /></span>
                <a href="#" id="waBackButton"><asp:literal runat="server" ID="litBack" /></a>
                <a href="#" id="waHomeButton"><asp:literal runat="server" ID="litHome" /></a>
            </div>
	        <div id="iGroup">
                <div class="iLayer" id="waLogin">
                    <div class="iPanel">
                    <center><img alt="iPiMP" style="margin-bottom:-10px" src="images/iPiMP logo.png" /></center>
                        <fieldset>
                            <ul>
                                <li><input type="text" id="jsUsername" runat="server" /></li>
                                <li><input type="password" id="jsPassword" runat="server" /></li>
                                <li><input type="checkbox" id="jsRemember" runat="server" class="iToggle" /><asp:Literal ID="litRemember" runat="server" /></li>
                            </ul>
                        </fieldset>
                        <div>
                            <asp:Label runat="server" ID="aspLblLogin" />
                        </div>
                    </div>
                    <div>
                        <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6722080" rel="Action" class="iButton iBOrange" target="_blank"><asp:literal runat="server" ID="litDonate" /></a>
                        <a href="javascript:login()" rel="Back" class="iButton iBAction"><asp:literal runat="server" ID="litLogin" /></a>
                    </div>
                </div>
            </div>
            <asp:Button ID="aspBtnLogin" runat="server" style="display:none;" />
        </form>
    </div>
</body>
</html>

