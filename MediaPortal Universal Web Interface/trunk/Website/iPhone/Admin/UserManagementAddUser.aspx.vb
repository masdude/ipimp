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

Partial Public Class UserManagementAddUser
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waCreateUserMenu"

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
        xw.WriteCData(CreateUserMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function CreateUserMenu(ByVal wa As String) As String

        Dim markup As String = ""

        markup += String.Format("<div class=""iPanel"" id=""{0}"">", wa)
        markup += "<fieldset>"
        markup += String.Format("<legend>{0}</legend>", GetGlobalResourceObject("uWiMPStrings", "create_user"))

        markup += "<ul>"
        markup += String.Format("<li><input type=""text"" id=""jsUsername"" placeholder=""{0}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "username"))
        markup += String.Format("<li><input type=""password"" id=""jsPassword1"" placeholder=""{0}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "new_password"))
        markup += String.Format("<li><input type=""password"" id=""jsPassword2"" placeholder=""{0}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "confirm_password"))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsRecorder"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "recorder"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsWatcher"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "watcher"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsDeleter"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "deleter"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsRemoter"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "remoter"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsAdmin"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "administrator"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += "</ul>"

        markup += "</fieldset>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBAction"" onclick=""return createuser()"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "add"))
        markup += "</div>"

        Return markup

    End Function

End Class