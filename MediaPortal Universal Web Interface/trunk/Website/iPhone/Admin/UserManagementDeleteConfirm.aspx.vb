Imports System.IO
Imports System.Xml

Partial Public Class UserManagementDeleteConfirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waDeleteUserConfirm"

        Dim username As String = Request.QueryString("username")

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
        xw.WriteCData(DeleteUserMenuConfirm(wa, username))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DeleteUserMenuConfirm(ByVal wa As String, ByVal username As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} {1}</h3>", GetGlobalResourceObject("uWiMPStrings", "delete_user"), username)
        markup += "<ul class=""iArrow"">"
        If username = "" Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "user_not_selected"))
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "are_you_sure"))
            markup += "</ul>"
            markup += "</div>"

            markup += "<div>"
            markup += String.Format("<a href=""Admin/UserManagementDeleteResult.aspx?username={0}#_DeleteUserResult"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{1}</a>", username, GetGlobalResourceObject("uWiMPStrings", "yes"))
            markup += String.Format("<a href=""#"" onclick=""return WA.Back()"" rel=""Back"" class=""iButton iBClassic"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "no"))
            markup += "</div>"
        End If

        Return markup

    End Function

End Class