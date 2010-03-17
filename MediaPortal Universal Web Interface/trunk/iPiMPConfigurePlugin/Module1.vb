Imports TvDatabase
Imports System.Collections.Specialized
Imports System.Configuration

Module Module1

    Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
    Dim layer As New TvBusinessLayer
    Dim setting As Setting
    Dim args() As String = Command.Split("=")
    'Dim args() As String = Split("add=iPiMPTranscodeToMP4_Test=true", "=")

    Sub Main()

        appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

        If UBound(args) <> 2 Then
            Console.Write("Invalid parameter passed.")
            System.Environment.ExitCode = 1
            Exit Sub
        End If

        Select Case args(0).ToLower
            Case "add"
                Try
                    setting = layer.GetSetting(args(1))
                    setting.Value = args(2).ToString
                    setting.Persist()
                    Console.WriteLine(String.Format("Setting {0} to {1}. Result is !!SUCCESS!!", args(1), args(2)))
                Catch ex As Exception
                    Console.WriteLine(String.Format("Setting {0} to {1}. Result is !!FAILURE!!", args(1), args(2)))
                    System.Environment.ExitCode = 1
                Finally
                    Dispose()
                End Try
            Case "del"
                Try
                    setting = layer.GetSetting(args(1))
                    setting.Remove()
                    Console.WriteLine(String.Format("Removing {0}. Result is !!SUCCESS!!", args(1), args(2)))
                Catch ex As Exception
                    Console.WriteLine(String.Format("Removing {0}. Result is !!FAILURE!!", args(1), args(2)))
                    System.Environment.ExitCode = 1
                Finally
                    Dispose()
                End Try
            Case "get"
                Try
                    setting = layer.GetSetting(args(1))
                    Dim result As String = setting.Value.ToString
                    If result = "" Then
                        Console.WriteLine(String.Format("Setting {0} is not set.", args(1), args(2)))
                    Else
                        Console.WriteLine(setting.Value.ToString)
                    End If
                Catch ex As Exception
                    Console.WriteLine(String.Format("There was an error."))
                    System.Environment.ExitCode = 1
                Finally
                    Dispose()
                End Try
            Case Else
                Console.WriteLine("Invalid action passed.")
                System.Environment.ExitCode = 1
                Dispose()
        End Select


    End Sub

    Private Sub Dispose()
        Setting = Nothing
        layer = Nothing
        args = Nothing
        appSettings = Nothing
    End Sub

End Module
