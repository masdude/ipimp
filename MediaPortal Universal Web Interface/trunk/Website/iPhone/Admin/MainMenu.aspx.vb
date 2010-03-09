' 
'   Copyright (C) 2008-2010 Martin van der Boon
' 
'  This program is free software: you can redistribute it and/or modify 
'  it under the terms of the GNU General Public License as published by 
'  the Free Software Foundation, either version 3 of the License, or 
'  (at your option) any later version. 
' 
'   This program is distributed in the hope that it will be useful, 
'   but WITHOUT ANY WARRANTY; without even the implied warranty of 
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
'   GNU General Public License for more details. 
' 
'   You should have received a copy of the GNU General Public License 
'   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
' 


Imports System.IO
Imports System.Xml

Partial Public Class MainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waAdmin"

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        'go
        xw.WriteStartElement("go")
        xw.WriteAttributeString("to", wa)
        xw.WriteEndElement()
        'end go

        'start title
        xw.WriteStartElement("title")
        xw.WriteAttributeString("set", wa)
        xw.WriteEndElement()
        'end title

        'start dest
        xw.WriteStartElement("destination")
        xw.WriteAttributeString("mode", "replace")
        xw.WriteAttributeString("zone", wa)
        xw.WriteAttributeString("create", "true")
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(AddAdminMenuOptions(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function AddAdminMenuOptions(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "user_admin"))
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""Admin/UserManagementChangePassword.aspx#_ChangePasswordMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "change_password"))

        If User.IsInRole("admin") Then
            markup += String.Format("<li><a href=""Admin/UserManagementAddUser.aspx#_CreateUserMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "create_user"))
            markup += String.Format("<li><a href=""Admin/UserManagementDeleteUser.aspx#_DeleteUserMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "delete_user"))
        End If
        markup += "</ul>"

        If User.IsInRole("admin") Then
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "ipimp_admin"))
            markup += "<ul class=""iArrow"">"

            markup += String.Format("<li><a href=""Admin/ManageSettings.aspx#_Settings"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "ipimp_settings"))
            markup += String.Format("<li><a href=""Admin/AppearanceSelect.aspx#_AppearanceSelect"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "appearance"))

            If uWiMP.TVServer.Utilities.GetAppConfig("USEMPCLIENT").ToLower = "true" Then
                markup += String.Format("<li><a href=""Admin/ClientManagementMainMenu.aspx#_ClientMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "mediaportal_clients"))
            End If

            markup += "</ul>"

        End If

        markup += "</div>"

        Return markup

    End Function

End Class