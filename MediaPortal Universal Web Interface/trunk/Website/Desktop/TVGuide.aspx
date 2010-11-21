<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TVGuide.aspx.vb" Inherits="Website.TVGuide" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" href="css/desktop.css" />
</head>
<body>
    <script type="text/javascript" src="scripts/wz_tooltip.js"></script>
    <form id="form1" runat="server">
        <div>
            <asp:Hyperlink ID="link" runat="server" />
            <asp:DropDownList ID="ddlChannels" runat="server" AutoPostBack="true" />
            <asp:DropDownList ID="ddlHours" runat="server" AutoPostBack="true" />
        </div>
        <div>
            <asp:PlaceHolder ID="ph1" runat="server"></asp:PlaceHolder>
            <asp:PlaceHolder ID="ph2" runat="server"></asp:PlaceHolder>
        </div>
    </form>
</body>
</html>