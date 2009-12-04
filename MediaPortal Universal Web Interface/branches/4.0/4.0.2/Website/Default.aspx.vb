﻿' 
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


Public Partial Class _Default1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
        appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

        Dim UA As String = Request.UserAgent

        If UA.ToLower.Contains("webkit") Then
            Response.Redirect("/iPhone/Default.aspx")
        Else
            Response.Redirect("/iPhone/Default.aspx")
        End If

    End Sub

End Class