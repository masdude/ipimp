Imports System
Imports System.Windows.Forms

Imports MediaPortal.Configuration

Partial Public Class SetupForm
    Inherits MediaPortal.UserInterface.Controls.MPConfigForm

    Private Sub SetupForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub

    Private Sub LoadSettings()
        Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
        Port.Value = xmlReader.GetValueAsInt("MPClientController", "TCPPort", 55667)
    End Sub

    Private Sub SaveSettings()
        Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
        xmlReader.SetValue("MPClientController", "TCPPort", Port.Value.ToString)
    End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click
        SaveSettings()
        Me.Close()
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

End Class

