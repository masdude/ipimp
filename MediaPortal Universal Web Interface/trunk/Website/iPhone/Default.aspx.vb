' 
'   Copyright (C) 2008-2011 Martin van der Boon
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

Imports Website.uWiMP.TVServer.MPClient

Partial Public Class _iPhoneDefault
    Inherits System.Web.UI.Page

    Private Shared resource As System.Resources.ResourceManager
    Private Shared textInfo As System.Globalization.TextInfo

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then

            Page.Title = "iPiMP by Cheezey"

            Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            AddTVServerItems()
            AddMPClientItems()
            AddAdminMenuItems()

            litMainMenu.Text = GetGlobalResourceObject("uWiMPStrings", "main_menu")
            litCliMenu.Text = GetGlobalResourceObject("uWiMPStrings", "mediaportal_clients")
            litAdminMenu.Text = GetGlobalResourceObject("uWiMPStrings", "administration")
            litTitle.Text = GetGlobalResourceObject("uWiMPStrings", "ipimp")
            litBack.Text = GetGlobalResourceObject("uWiMPStrings", "back")
            litHome.Text = GetGlobalResourceObject("uWiMPStrings", "home")
            litLogout.Text = GetGlobalResourceObject("uWiMPStrings", "logout")
            litDonate.Text = GetGlobalResourceObject("uWiMPStrings", "donate")
            litColour.Text = uWiMP.TVServer.Utilities.GetAppConfig("COLOUR").ToLower

        End If

    End Sub

    Private Sub AddTVServerItems()

        Dim markup As String = String.Empty
        Dim li As New LiteralControl

        If uWiMP.TVServer.Utilities.GetAppConfig("USETVSERVER").ToLower = "true" Then
            markup += "<li><a href=""TVGuide/MainMenu.aspx#_ChannelGroups"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "tv_guide") & "</a></li>"
            markup += "<li><a href=""RadioGuide/MainMenu.aspx#_RadioChannelGroups"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "radio_guide") & "</a></li>"
            If uWiMP.TVServer.Utilities.GetAppConfig("RECSUBMENU").ToLower = "true" Then
                markup += "<li><a href=""Recording/MainMenu.aspx#_Recordings"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "recorded_programs") & "</a></li>"
            Else
                Select Case uWiMP.TVServer.Utilities.GetAppConfig("RECORDER").ToLower
                    Case "date"
                        markup += String.Format("<li><a href=""Recording/RecordingsByDate.aspx?start=0#_RecDate"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "recorded_programs"))
                    Case "genre"
                        markup += String.Format("<li><a href=""Recording/RecordingsByGenre.aspx?start=0#_RecGenre"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "recorded_programs"))
                    Case "channel"
                        markup += String.Format("<li><a href=""Recording/RecordingsByChannel.aspx?start=0#_RecChannel"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "recorded_programs"))
                End Select
            End If
            markup += "<li><a href=""Schedule/MainMenu.aspx#_Schedules"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "scheduled_programs") & "</a></li>"
            markup += "<li><a href=""TVServer/MainMenu.aspx#_TVServer"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "tv_server_status") & "</a></li>"
            litMainMenu.Visible = True
            li.Text = markup
            phMainMenu.Controls.Add(li)
        Else
            litMainMenu.Visible = False
        End If
    End Sub

    Private Sub AddMPClientItems()

        Dim markup As String = String.Empty
        Dim li As New LiteralControl

        If uWiMP.TVServer.Utilities.GetAppConfig("USEMPCLIENT").ToLower = "true" Then
            If uWiMP.TVServer.Utilities.GetAppConfig("SUBMENU").ToLower = "true" Then
                litCliMenu.Visible = False
                markup += "<li><a href=""MPClient/MainMenu.aspx#_MPClientMenu"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "mediaportal_clients") & "</a></li>"
            Else
                litCliMenu.Visible = True
                markup += DisplayMPClientsMenu()
            End If
            li.Text = markup
            phCliMenu.Controls.Add(li)
        Else
            litCliMenu.Visible = False
        End If

    End Sub

    Private Sub AddAdminMenuItems()

        Dim markup As String = ""

        markup += "<li><a href=""Admin/MainMenu.aspx#_Admin"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "administration") & "</a></li>"
        markup += "<li><a href=""Admin/AboutiPiMP.aspx#_About"" rev=""async"">" & GetGlobalResourceObject("uWiMPStrings", "about_ipimp") & "</a></li>"
        markup += "<li><a href=""../Desktop/Default.aspx"" _target=""blank"" >" & GetGlobalResourceObject("uWiMPStrings", "desktop") & "</a></li>"

        litAdminMenu.Visible = True
        Dim li As New LiteralControl
        li.Text = markup

        phAdminMenu.Controls.Add(li)

    End Sub

    Private Function DisplayMPClientsMenu() As String

        Dim markup As String = String.Empty

        Dim clients As List(Of Client) = uWiMP.TVServer.MPClientDatabase.GetClients

        If (User.IsInRole("remoter")) Then
            Select Case clients.Count
                Case 0
                    markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_clients_defined"))
                Case Else
                    For Each client As uWiMP.TVServer.MPClient.Client In clients
                        markup += String.Format("<li><a href=""MPClient/MPClientMenu.aspx?friendly={0}#_MPClient"" rev=""async"">{0}</a></li>", client.Friendly)
                    Next
                    If clients.Count > 1 Then markup += String.Format("<li><a href=""MPClient/MPClientSendMessage.aspx?friendly=all#_MPClientSendMessage"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "send_message_global"))
            End Select
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_access"))
        End If

        Return markup

    End Function

    Protected Sub Logout(ByVal sender As Object, ByVal e As System.EventArgs) Handles aspBtnLogout.Click
        FormsAuthentication.SignOut()
        FormsAuthentication.RedirectToLoginPage()
    End Sub

End Class