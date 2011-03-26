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

Imports MediaPortal.Configuration

Partial Public Class SetupForm
    Inherits MediaPortal.UserInterface.Controls.MPConfigForm

    Private Sub SetupForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub

    Private Sub SetupForm_UnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        SaveSettings()
    End Sub

    Private Sub LoadSettings()
        Using xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
            Port.Value = xmlReader.GetValueAsInt("MPClientController", "TCPPort", 55667)
        End Using
    End Sub

    Private Sub SaveSettings()
        Using xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
            xmlReader.SetValue("MPClientController", "TCPPort", Port.Value.ToString)
        End Using
    End Sub

End Class

