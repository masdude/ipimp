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

Partial Public Class UserManagementChangePassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waChangePasswordMenu"

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

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
        xw.WriteCData(ChangePasswordMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function ChangePasswordMenu(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iPanel"" id=""{0}"">", wa)
        markup += "<fieldset>"
        markup += String.Format("<legend>{0}</legend>", GetGlobalResourceObject("uWiMPStrings", "change_password"))

        markup += "<ul>"
        markup += String.Format("<li><input type=""password"" id=""jsOldPassword"" placeholder=""{0}""/></li>", GetGlobalResourceObject("uWiMPStrings", "old_password"))
        markup += String.Format("<li><input type=""password"" id=""jsNewPassword1"" placeholder=""{0}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "new_password"))
        markup += String.Format("<li><input type=""password"" id=""jsNewPassword2"" placeholder=""{0}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "confirm_password"))
        markup += "</ul>"

        markup += "</fieldset>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBAction"" onclick=""return changepassword()"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "change"))
        markup += "</div>"

        Return markup

    End Function

End Class