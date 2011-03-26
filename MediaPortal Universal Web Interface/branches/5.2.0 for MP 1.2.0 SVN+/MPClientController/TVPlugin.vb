﻿' 
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


Imports TvDatabase
Imports TvPlugin.TVHome
Imports MediaPortal.GUI.Library

Namespace MPClientController

    Public NotInheritable Class MyTV

        Private Sub New()
        End Sub

        Public Shared Function StartChannel(ByVal idChannel As Integer) As Boolean

            Dim success As Boolean = False
            Try
                GUIWindowManager.ReplaceWindow(500)
                Dim channel As Channel = channel.Retrieve(idChannel)
                success = ViewChannelAndCheck(channel)
            Catch ex As Exception

            End Try

            Return iPiMPUtils.SendBool(success)

        End Function

    End Class

End Namespace
