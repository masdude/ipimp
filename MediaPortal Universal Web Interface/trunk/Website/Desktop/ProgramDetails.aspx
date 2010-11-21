<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProgramDetails.aspx.vb" Inherits="Website.ProgramDetails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><asp:Literal ID="litTitle" runat="server"></asp:Literal></title>
    <link rel="Stylesheet" href="css/desktop.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <asp:PlaceHolder ID="phProgramDetails" runat="server"></asp:PlaceHolder>

         <asp:Table ID="Table1" runat="server" Width="100%">
            <asp:TableRow ID="Row1" runat="server">
                <asp:TableCell ID ="Cell1" runat="server" CssClass="rounded-corners red smallerwhite" Width="80%" BorderWidth="1" BorderStyle="Solid" >
                    <asp:RadioButtonList ID="rbl" runat="server">
                        <asp:ListItem Value="0" Text="<%$ Resources:uWiMPStrings, once %>" Selected="True" />
                        <asp:ListItem Value="1" Text="<%$ Resources:uWiMPStrings, at_this_time_every_day %>" />
                        <asp:ListItem Value="2" Text="<%$ Resources:uWiMPStrings, at_this_time_every_week %>" />
                        <asp:ListItem Value="3" Text="<%$ Resources:uWiMPStrings, each_time_on_this_channel %>" />
                        <asp:ListItem Value="4" Text="<%$ Resources:uWiMPStrings, each_time_on_any_channel %>" />
                        <asp:ListItem Value="5" Text="<%$ Resources:uWiMPStrings, at_this_time_at_weekends %>" />
                        <asp:ListItem Value="6" Text="<%$ Resources:uWiMPStrings, at_this_time_on_weekdays %>" />
                    </asp:RadioButtonList>
                </asp:TableCell>
                <asp:TableCell ID="Cell2" runat="server" CssClass="rounded-corners red smallerwhite" Width="20%" BorderWidth="1" BorderStyle="Solid" VerticalAlign="Top" >
                    <asp:Button ID="Btn1" runat="server" Text="<%$ Resources:uWiMPStrings, record %>" />
                </asp:TableCell>
            </asp:TableRow>
         </asp:Table>
       </div>
    </form>
</body>
</html>
