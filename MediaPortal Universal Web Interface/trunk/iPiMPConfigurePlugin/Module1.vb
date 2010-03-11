Imports TvDatabase
Imports System.Collections.Specialized
Imports System.Configuration

Module Module1

    Sub Main()

        Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
        appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

        Dim args() As String = Command.Split("=")
        'Dim args() As String = Split("add=iPiMPTranscodeToMP4_Delete=true", "=")

        If UBound(args) <> 2 Then
            Console.Write("Invalid parameter passed.")
            System.Environment.ExitCode = 1
            Exit Sub
        End If

        Dim layer As New TvBusinessLayer
        Dim setting As Setting
        Select Case args(0).ToLower
            Case "add"
                Try
                    setting = layer.GetSetting(args(1))
                    setting.Value = args(2).ToString
                    setting.Persist()
                    Console.Write("Setting " & args(1) & " to " & args(2) & ". !!!SUCCESS!!!" & vbNewLine)
                Catch ex As Exception
                    Console.Write("Setting " & args(1) & " to " & args(2) & ". !!!FAILED!!!" & vbNewLine)
                    System.Environment.ExitCode = 1
                End Try
            Case "del"
                Try
                    setting = layer.GetSetting(args(1))
                    setting.Remove()
                    Console.Write("Removing " & args(1) & " !!!SUCCESS!!!" & vbNewLine)
                Catch ex As Exception
                    Console.Write("Removing " & args(1) & " !!!FAILED!!!" & vbNewLine)
                    System.Environment.ExitCode = 1
                End Try
            Case "get"
                Try
                    setting = layer.GetSetting(args(1))
                    Console.Write(setting.Value.ToString)
                Catch ex As Exception
                    Console.Write("!!!ERROR!!!" & vbNewLine)
                    System.Environment.ExitCode = 1
                End Try
            Case Else
                Console.Write("Invalid action passed.")
                System.Environment.ExitCode = 1
        End Select
    End Sub

End Module
