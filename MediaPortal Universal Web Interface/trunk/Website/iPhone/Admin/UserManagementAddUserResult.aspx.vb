Imports System.IO
Imports System.Xml

Partial Public Class UserManagementAddUserResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waCreateUser"

        Dim newpass As String = Request.QueryString("newpass")
        Dim confpass As String = Request.QueryString("confpass")
        Dim username As String = Request.QueryString("user")
        Dim recorder As String = CBool(Request.QueryString("recorder"))
        Dim watcher As String = CBool(Request.QueryString("recorder"))
        Dim deleter As String = CBool(Request.QueryString("deleter"))
        Dim remoter As String = CBool(Request.QueryString("remoter"))
        Dim admin As String = CBool(Request.QueryString("admin"))

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
        xw.WriteCData(CreateUser(username, newpass, confpass, recorder, watcher, deleter, remoter, admin, wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function CreateUser(ByVal user As String, ByVal pass1 As String, ByVal pass2 As String, _
                                ByVal recorder As Boolean, ByVal watcher As Boolean, ByVal deleter As Boolean, _
                                ByVal remoter As Boolean, ByVal admin As Boolean, ByVal wa As String) As String

        Dim markup As String = ""

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "create_user"))
        markup += "<ul class=""iArrow"">"

        If user = "" Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "username_blank"))
            markup += "</ul>"
            markup += "</div>"
            Return markup
        End If


        If Membership.FindUsersByName(user).Count > 0 Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "username_exists"))
            markup += "</ul>"
            markup += "</div>"
            Return markup
        End If

        If pass1 <> pass2 Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "pass_dont_match"))
            markup += "</ul>"
            markup += "</div>"
            Return markup
        End If

        Try
            Dim newuser As MembershipUser = Membership.CreateUser(user, pass1, user & "@uWiMP.mp")
        Catch ex As Exception
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "create_user_fail"))
            markup += "</ul>"
            markup += "</div>"
            Return markup
        End Try

        markup += AddUserToRole(user, "reader")
        If recorder Then markup += AddUserToRole(user, "recorder")
        If watcher Then markup += AddUserToRole(user, "watcher")
        If deleter Then markup += AddUserToRole(user, "deleter")
        If remoter Then markup += AddUserToRole(user, "remoter")
        If admin Then markup += AddUserToRole(user, "admin")

        markup += String.Format("<li>{0} {1}</li>", user, GetGlobalResourceObject("uWiMPStrings", "create_user_success"))

        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += String.Format("<a href=""Admin/MainMenu.aspx#_Admin"" rel=""Back"" rev=""async"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += "</div>"

        Return markup

    End Function

    Private Function AddUserToRole(ByVal username As String, ByVal role As String)

        Dim markup As String = String.Empty

        Try
            Roles.AddUserToRole(username, role)
        Catch ex As Exception
            markup += String.Format("<li style=""color:red"">{0}</li>", String.Format(GetGlobalResourceObject("uWiMPStrings", "role_add_fail"), username))
            Roles.RemoveUserFromRoles(username, Roles.GetRolesForUser(username))
            Membership.DeleteUser(username)
        End Try

        Return markup

    End Function

End Class