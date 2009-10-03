Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports MediaPortal.Util

Namespace MPClientController

    Public Class PowerOptions

        Public Shared Function DoPowerOption(ByVal poweroption As String)

            Select Case poweroption.ToLower
                Case "logoff"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.LogOff, True)
                Case "suspend"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.Suspend, True)
                Case "hibernate"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.Hibernate, True)
                Case "reboot"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.Reboot, True)
                Case "shutdown"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.ShutDown, True)
                Case "poweroff"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.PowerOff, True)
            End Select

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

    End Class

End Namespace
