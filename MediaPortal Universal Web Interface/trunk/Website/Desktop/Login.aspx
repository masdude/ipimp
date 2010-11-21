<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="Website.DesktopLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Table ID="Table1" runat="server" HorizontalAlign="Center">
            <asp:TableRow ID="TableRow1" runat="server">
                <asp:TableCell ID="TableCell1" runat="server">
                    <asp:Image ID="Image1" runat="server" ImageUrl="../Images/iPiMP logo.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell2" runat="server">
                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:uWiMPStrings, username %>" />&nbsp;
                    <asp:TextBox ID="Username" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow3" runat="server" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell3" runat="server">
                    <asp:Label ID="Label2" runat="server" Text="<%$ Resources:uWiMPStrings, password %>" />&nbsp;
                    <asp:TextBox ID="Password" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell4" runat="server">
                    <asp:Label ID="Label4" runat="server" Visible="false" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow5" runat="server" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell5" runat="server">
                    <asp:Label ID="Label3" runat="server" Text="<%$ Resources:uWiMPStrings, remember_me %>" />&nbsp;
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow6" runat="server" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell6" runat="server">
                    <asp:Button ID="Button1" runat="server" Text="<%$ Resources:uWiMPStrings, login %>" CommandName="login" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    
        
    
    </div>
    </form>
</body>
</html>
