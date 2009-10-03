Imports System.IO
Imports System.Xml

Partial Public Class UserManagementDeleteUser
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waDeleteUserMenu"

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
        xw.WriteCData(DeleteUserMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DeleteUserMenu(ByVal wa As String) As String

        Dim markup As String = String.Empty
        Dim iUsers As Integer = 0
        Dim user As MembershipUser
        Dim users As MembershipUserCollection = Membership.GetAllUsers

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "delete_user"))
        markup += "<ul class=""iArrow"">"

        For Each user In users
            If user.UserName.ToLower <> "admin" Then
                iUsers += 1
            End If
        Next

        If iUsers = 0 Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_users"))
        Else
            markup += String.Format("<li id=""jsMPUser"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "select_user"))
            For Each user In users
                If user.UserName.ToLower <> "admin" Then
                    markup += String.Format("<label><input type=""radio"" name=""jsMPUser"" value=""{0}"" /> {0}</label>", user.UserName)
                End If
            Next
        End If

        markup += "</li>"
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBWarn"" onclick=""return deleteuser()"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "delete"))
        markup += "</div>"

        Return markup

    End Function

End Class