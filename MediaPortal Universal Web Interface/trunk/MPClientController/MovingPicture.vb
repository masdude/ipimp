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
Imports MediaPortal.Plugins.MovingPictures.MainUI
Imports MediaPortal.Plugins.MovingPictures.Database
Imports MediaPortal.Playlists
Imports System.Threading
Imports MediaPortal.Player

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

        Private Sub StartPlayingMovPic()
            Dim g As New MovingPicturesGUI
            Dim p As New MoviePlayer(g)
            p.Play(_movieInfo)
        End Sub

        Private Sub StartPlaying()

            If g_Player.Playing Then g_Player.Stop()

            If _movieInfo.LocalMedia(0).IsDVD Then
                g_Player.Player.Play(_movieInfo.LocalMedia(0).FullPath)
            Else
                _playlistPlayer = New PlayListPlayer
                _playlistPlayer = PlayListPlayer.SingletonPlayer
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

            End If

        End Sub
    End Class
End Namespace
