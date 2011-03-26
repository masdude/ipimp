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

Imports System.Threading
Imports MediaPortal.Video.Database
Imports MediaPortal.GUI.Video

Namespace MPClientController

    Public Class Video

        Private _movieid As Integer = 0

        Public Property movieID() As Integer
            Get
                Return _movieid
            End Get
            Set(ByVal value As Integer)
                _movieid = value
            End Set
        End Property

        Public Sub RemoveStopTime()

            Dim fileList As New ArrayList
            VideoDatabase.GetFiles(_movieid, fileList)

            For Each file As String In fileList
                Dim idFile As Integer = VideoDatabase.GetFileId(file)
                VideoDatabase.DeleteMovieStopTime(idFile)
            Next
        End Sub

        Public Sub PlayVideo()
            SyncLock Me
                Dim t As New Thread(New ThreadStart(AddressOf Me.StartPlaying))
                t.Start()
            End SyncLock
        End Sub

        Private Sub StartPlaying()
            GUIVideoFiles.PlayMovie(_movieid)
        End Sub

    End Class

End Namespace
