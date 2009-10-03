' 
'   Copyright (C) 2008-2009 Martin van der Boon
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

Partial Public Class UserManagementChangePasswordResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim oldpass As String = Request.QueryString("oldpass")
        Dim newpass As String = Request.QueryString("newpass")
        Dim confpass As String = Request.QueryString("confpass")

        Dim wa As String = "waChangePass"

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
        xw.WriteCData(ChangePassword(oldpass, newpass, confpass, wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function ChangePassword(ByVal oldpass As String, ByVal newpass As String, ByVal confpass As String, ByVal wa As String) As String

        Dim membershipuser As MembershipUser = Membership.GetUser(User.Identity.Name.ToString)

        Dim markup As String = ""

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "change_password"))
        markup += "<ul class=""iArrow"">"

        If newpass <> confpass Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "pass_dont_match"))
        Else
            Try
                membershipuser.ChangePassword(oldpass, newpass)
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "pass_change_success"))
            Catch ex As Exception
                markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "pass_change_fail"))
            End Try
        End If

        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += String.Format("<a href=""Admin/MainMenu.aspx#_Admin"" rel=""Back"" rev=""async"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += "</div>"

        Return markup

    End Function

End Class