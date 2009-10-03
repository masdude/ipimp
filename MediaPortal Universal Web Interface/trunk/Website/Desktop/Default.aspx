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
