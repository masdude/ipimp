' 
'   Copyright (C) 2008-2010 Martin van der Boon
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

Imports MediaPortal.Util
Imports MediaPortal.GUI.Library

Namespace MPClientController

    Public Class PowerOptions

        Public Shared Function DoPowerOption(ByVal poweroption As String)

            Select Case poweroption.ToLower
                Case "0"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.LogOff, True)
                Case "1"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.Suspend, True)
                Case "2"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.Hibernate, True)
                Case "3"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.Reboot, True)
                Case "4"
                    MediaPortal.Util.WindowsController.ExitWindows(RestartOptions.ShutDown, True)
                Case "5"
                    Dim action As Action = New Action(action.ActionType.ACTION_EXIT, 0, 0)
                    GUIGraphicsContext.OnAction(action)
            End Select

            Return iPiMPUtils.SendBool(True)

        End Function

    End Class

End Namespace
