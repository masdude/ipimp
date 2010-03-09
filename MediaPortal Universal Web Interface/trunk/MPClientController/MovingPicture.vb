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


Imports System.Threading
Imports MediaPortal.Plugins.MovingPictures.Database

Imports MediaPortal.GUI.Video
Imports MediaPortal.Playlists

Namespace MPClientController

    Public Class MovingPicture

        Private _movieInfo As DBMovieInfo
        Private _playlistPlayer As PlayListPlayer

        Public Property movieInfo() As DBMovieInfo
            Get
                Return _movieInfo
            End Get
            Set(ByVal value As DBMovieInfo)
                _movieInfo = value
            End Set
        End Property

        Public Sub PlayVideo()
            SyncLock Me
                Dim t As New Thread(New ThreadStart(AddressOf Me.StartPlaying))
                t.Start()
            End SyncLock
        End Sub

        Private Sub StartPlaying()

            _playlistPlayer = New PlayListPlayer
            _playlistPlayer = playlistPlayer.SingletonPlayer
            _playlistPlayer.RepeatPlaylist = False

            _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO).Clear()
            For Each mediaFile As DBLocalMedia In _movieInfo.LocalMedia
                Dim item As New PlayListItem
                item.FileName = mediaFile.FullPath
                item.Description = _movieInfo.Title
                item.Duration = mediaFile.Duration
                item.Type = PlayListItem.PlayListItemType.Video
                _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO).Add(item)
            Next

            If _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO).Count > 0 Then
                _playlistPlayer.Reset()
                _playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO
                _playlistPlayer.Play(0)
            End If

        End Sub

    End Class

End Namespace
