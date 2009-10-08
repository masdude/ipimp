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


Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports MediaPortal.Util
Imports MediaPortal.GUI.Library

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
                Case "close"
                    Dim action As Action = New Action(action.ActionType.ACTION_EXIT, 0, 0)
                    GUIGraphicsContext.OnAction(action)
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
