Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private Shared resource As System.Resources.ResourceManager
    Private Shared textInfo As System.Globalization.TextInfo

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then

            Page.Title = "iPiMP by Cheezey"

            Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            AddTopMenuItems()
            AddAdminMenuItems()

            litTitle.Text = GetGlobalResourceObject("uWiMPStrings", "ipimp")
            litBack.Text = GetGlobalResourceObject("uWiMPStrings", "back")
            litHome.Text = GetGlobalResourceObject("uWiMPStrings", "home")
            litLogout.Text = GetGlobalResourceObject("uWiMPStrings", "logout")
            litDonate.Text = GetGlobalResourceObject("uWiMPStrings", "donate")
            
        End If

    End Sub

    Private Sub AddTopMenuItems()

        Dim markup As String = ""

        If uWiMP.TVServer.Utilities.GetAppConfig("USETVSERVER").ToLower = "true" Then
            markup += "<li><a href=""TVGuide/MainMenu.aspx#_ChannelGroups"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "tv_guide") & "</a></li>"
            markup += "<li><a href=""Recording/MainMenu.aspx#_Recordings"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "recorded_programs") & "</a></li>"
            markup += "<li><a href=""Schedule/MainMenu.aspx#_Schedules"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "scheduled_programs") & "</a></li>"
            markup += "<li><a href=""TVServer/MainMenu.aspx#_TVServer"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "tv_server_status") & "</a></li>"
        End If

        If uWiMP.TVServer.Utilities.GetAppConfig("USEMPCLIENT").ToLower = "true" Then
            markup += "<li><a href=""MPClient/MainMenu.aspx#_MPClientMenu"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "mediaportal_clients") & "</a></li>"
        End If

        Dim li As New LiteralControl
        li.Text = markup

        phMainMenu.Controls.Add(li)

    End Sub

    Private Sub AddAdminMenuItems()

        Dim markup As String = ""

        markup += "<li><a href=""Admin/MainMenu.aspx#_Admin"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "administration") & "</a></li>"
        markup += "<li><a href=""Admin/AboutiPiMP.aspx#_About"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "about_ipimp") & "</a></li>"

        Dim li As New LiteralControl
        li.Text = markup

        phAdminMenu.Controls.Add(li)

    End Sub

    Protected Sub Logout(ByVal sender As Object, ByVal e As System.EventArgs) Handles aspBtnLogout.Click
        FormsAuthentication.SignOut()
        FormsAuthentication.RedirectToLoginPage()
    End Sub

End Class