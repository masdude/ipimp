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