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

            litMainMenu.Text = GetGlobalResourceObject("uWiMPStrings", "main_menu")
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