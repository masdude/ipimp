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


Imports TvDatabase
Imports MediaPortal.Player
Imports TvPlugin

Namespace MPClientController

    Public NotInheritable Class MyTV

        Private Sub New()
        End Sub

        Public Shared Function StartChannel(ByVal idChannel As Integer) As String

            Dim success As Boolean = False
            Try
                Dim channel As Channel = channel.Retrieve(idChannel)

                If g_Player.Playing Then
                    If Not g_Player.IsTimeShifting OrElse (g_Player.IsTimeShifting AndAlso channel.IsWebstream()) Then
                        g_Player.Stop()
                    End If
                End If

                If channel.IsWebstream() Then
                    g_Player.PlayAudioStream(GetPlayPath(channel))
                Else
                    TVHome.ViewChannel(channel)

                    If TVHome.Navigator.CurrentChannel = channel.Name AndAlso g_Player.IsRadio AndAlso g_Player.Playing Then
                        success = True
                    Else
                        success = False
                    End If

                End If

            Catch ex As Exception

            End Try

            Return iPiMPUtils.SendBool(success)

        End Function

        Private Shared Function GetPlayPath(ByVal channel As Channel) As String

            Dim details As IList(Of TuningDetail) = channel.ReferringTuningDetail()
            Dim detail As TuningDetail = details(0)

            If channel.IsWebstream() Then
                Return detail.Url
            Else
                Return String.Format("{0}.radio", detail.Frequency)
            End If

        End Function

    End Class

End Namespace
