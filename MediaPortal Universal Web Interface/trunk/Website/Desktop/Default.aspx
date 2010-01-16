<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Desktop/Desktop.Master" CodeBehind="Default.aspx.vb" Inherits="Website._Default2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Table ID="Table2" runat="server">
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="TableCell1" runat="server">
                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell3" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server"></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table ID="tblTVGuide" runat="server">
    </asp:Table>
    <asp:PlaceHolder ID="phTVGuide" runat="server" />
</asp:Content>
