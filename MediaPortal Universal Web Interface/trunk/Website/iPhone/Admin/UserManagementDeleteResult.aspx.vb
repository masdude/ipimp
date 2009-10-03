﻿Imports System.IO
Imports System.Xml

Partial Public Class UserManagementDeleteResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim username As String = Request.QueryString("username")
        Dim wa As String = "waDeleteUserResult"

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
        xw.WriteCData(DeleteUser(username, wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DeleteUser(ByVal user As String, ByVal wa As String) As String

        Dim delRoles As String() = Roles.GetRolesForUser(user)
        Roles.RemoveUserFromRoles(user, delRoles)

        Dim deleted As Boolean = Membership.DeleteUser(user)

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "delete_user"))
        markup += "<ul class=""iArrow"">"

        If deleted Then
            markup += String.Format("<li>{0} {1}</li>", user, GetGlobalResourceObject("uWiMPStrings", "has_been_deleted"))
        Else
            markup += String.Format("<li style=""color:red"">{0} {1}</li>", user, GetGlobalResourceObject("uWiMPStrings", "has_been_deleted_fail"))
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