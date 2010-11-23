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
            <asp:Table ID="table1" runat="server" Width="100%">
                <asp:TableRow ID="row1" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server" Width="10%" VerticalAlign="middle">
                        <asp:Hyperlink ID="link" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server" Width="20%" VerticalAlign="middle">
                        <asp:Literal ID="litChannelGroups" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server" Width="40%" HorizontalAlign="center">
                        <asp:Button ID="Button1" runat="server" Text="<<" CssClass="button" />&nbsp;
                        <asp:Button ID="Button2" runat="server" Text="<" CssClass="button" />&nbsp;
                        <asp:Label ID="Label3" runat="server" />&nbsp;
                        <asp:Button ID="Button3" runat="server" Text=">" CssClass="button" />&nbsp;
                        <asp:Button ID="Button4" runat="server" Text=">>" CssClass="button" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server" Width="30%"></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
        <div>
            <asp:PlaceHolder ID="ph1" runat="server"></asp:PlaceHolder>
            <asp:PlaceHolder ID="ph2" runat="server"></asp:PlaceHolder>
        </div>
    </form>
</body>
</html>