Imports System.IO
Imports System.Xml

Partial Public Class UserManagementMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim action As String = Request.QueryString("action")
        Dim wa As String = String.Empty

        Select Case action.ToLower
            Case "change"
                wa = "waChangePasswordMenu"
            Case "create"
                wa = "waCreateUserMenu"
            Case "delete"
                wa = "waDeleteUserMenu"
        End Select

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
        Select Case action.ToLower
            Case "change"
                xw.WriteCData(ChangePasswordMenu(wa))
            Case "create"
                xw.WriteCData(CreateUserMenu(wa))
            Case "delete"
                xw.WriteCData(DeleteUserMenu(wa))
        End Select
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

        markup += "<div class=""iPanel"" id=""" & wa & """>"
        markup += "<fieldset>"
        markup += "<legend>" & GetGlobalResourceObject("uWiMPStrings", "change_password") & "</legend>"

        markup += "<ul>"
        markup += "<li><input type=""password"" id=""jsOldPassword"" placeholder=""" & GetGlobalResourceObject("uWiMPStrings", "old_password") & """/></li>"
        markup += "<li><input type=""password"" id=""jsNewPassword1"" placeholder=""" & GetGlobalResourceObject("uWiMPStrings", "new_password") & """ /></li>"
        markup += "<li><input type=""password"" id=""jsNewPassword2"" placeholder=""" & GetGlobalResourceObject("uWiMPStrings", "confirm_password") & """ /></li>"
        markup += "</ul>"

        markup += "<a style=""float:right;margin:0px 0px;color:#ffffff;"" class=""iButton iBAction"" onclick=""return changepassword();"" href=""#"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "submit") & "</a>"

        markup += "</fieldset>"
        markup += "</div>"

        Return markup

    End Function

    Private Function CreateUserMenu(ByVal wa As String) As String

        Dim markup As String = ""

        markup += "<div class=""iPanel"" id=""" & wa & """>"
        markup += "<fieldset>"
        markup += "<legend>" & GetGlobalResourceObject("uWiMPStrings", "create_user") & "</legend>"

        markup += "<ul>"
        markup += "<li><input type=""text"" id=""jsUsername"" placeholder=""" & GetGlobalResourceObject("uWiMPStrings", "username") & """ /></li>"
        markup += "<li><input type=""password"" id=""jsPassword1"" placeholder=""" & GetGlobalResourceObject("uWiMPStrings", "new_password") & """ /></li>"
        markup += "<li><input type=""password"" id=""jsPassword2"" placeholder=""" & GetGlobalResourceObject("uWiMPStrings", "confirm_password") & """ /></li>"
        markup += "<li><label>" & GetGlobalResourceObject("uWiMPStrings", "recorder") & "</label><input type=""checkbox"" id=""jsRecorder"" class=""iToggle"" title=""" & GetGlobalResourceObject("uWiMPStrings", "yesno") & """ /></li>"
        markup += "<li><label>" & GetGlobalResourceObject("uWiMPStrings", "watcher") & "</label><input type=""checkbox"" id=""jsWatcher"" class=""iToggle"" title=""" & GetGlobalResourceObject("uWiMPStrings", "yesno") & """ /></li>"
        markup += "<li><label>" & GetGlobalResourceObject("uWiMPStrings", "deleter") & "</label><input type=""checkbox"" id=""jsDeleter"" class=""iToggle"" title=""" & GetGlobalResourceObject("uWiMPStrings", "yesno") & """ /></li>"
        markup += "<li><label>" & GetGlobalResourceObject("uWiMPStrings", "remoter") & "</label><input type=""checkbox"" id=""jsRemoter"" class=""iToggle"" title=""" & GetGlobalResourceObject("uWiMPStrings", "yesno") & """ /></li>"
        markup += "<li><label>" & GetGlobalResourceObject("uWiMPStrings", "administrator") & "</label><input type=""checkbox"" id=""jsAdmin"" class=""iToggle"" title=""" & GetGlobalResourceObject("uWiMPStrings", "yesno") & """ /></li>"
        markup += "</ul>"
        markup += "<a style=""float:right;margin:0px 0px;color:#ffffff;"" class=""iButton iBAction"" onclick=""return createuser();"" href=""#"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "submit") & "</a>"

        markup += "</fieldset>"
        markup += "</div>"

        Return markup

    End Function

    Private Function DeleteUserMenu(ByVal wa As String) As String

        Dim markup As String = ""
        Dim iUsers As Integer = 0
        Dim user As MembershipUser
        Dim users As MembershipUserCollection = Membership.GetAllUsers

        markup += "<div class=""iMenu"" id=""" & wa & """>"
        markup += "<h3>" & GetGlobalResourceObject("uWiMPStrings", "delete_user") & "</h3>"
        markup += "<ul class=""iArrow"">"

        For Each user In users
            If user.UserName.ToLower <> "admin" Then
                markup += "<li><a href=""Admin/UserManagement.aspx?action=delete&user=" & user.UserName & "#_DeleteUser"" rev=""async""><span>" & GetGlobalResourceObject("uWiMPStrings", "delete") & "</span>" & user.UserName & "</a></li>"
                iUsers += 1
            End If
        Next

        If iUsers = 0 Then
            markup += "<li>" & GetGlobalResourceObject("uWiMPStrings", "no_users") & "</li>"
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class