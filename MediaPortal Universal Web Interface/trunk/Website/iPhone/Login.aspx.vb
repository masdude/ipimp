Imports System.Web

Partial Public Class iPhoneLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then

            Page.Title = "iPiMP by Cheezey"

            Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            litBack.Text = GetGlobalResourceObject("uWiMPStrings", "back")
            litHome.Text = GetGlobalResourceObject("uWiMPStrings", "home")
            litLogin.Text = GetGlobalResourceObject("uWiMPStrings", "login")
            litDonate.Text = GetGlobalResourceObject("uWiMPStrings", "donate")
            litRemember.Text = GetGlobalResourceObject("uWiMPStrings", "remember_me")
            jsUsername.Attributes("placeholder") = GetGlobalResourceObject("uWiMPStrings", "username")
            jsPassword.Attributes("placeholder") = GetGlobalResourceObject("uWiMPStrings", "password")
            jsRemember.Attributes("title") = GetGlobalResourceObject("uWiMPStrings", "yesno")

        End If

    End Sub

    Protected Sub iPhoneLogin(ByVal sender As Object, ByVal e As System.EventArgs) Handles aspBtnLogin.Click

        Dim MembershipProvider As uWiMP.TVServer.SQLiteMembershipProvider = DirectCast(Membership.Providers("SQLiteMembershipProvider"), uWiMP.TVServer.SQLiteMembershipProvider)
        Dim validuser As Boolean = Membership.ValidateUser(jsUsername.Value, jsPassword.Value)

        If validuser Then
            FormsAuthentication.SetAuthCookie(jsUsername.Value, jsRemember.Checked)
            Response.Redirect("Default.aspx")
        Else
            aspLblLogin.Text = "Login failed! Retry"
            aspLblLogin.ForeColor = Drawing.Color.Red
        End If

    End Sub

End Class