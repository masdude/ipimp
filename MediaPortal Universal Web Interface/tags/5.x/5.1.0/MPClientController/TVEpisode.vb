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
Imports WindowPlugins.GUITVSeries
Imports MediaPortal.Player

Namespace MPClientController

    Public Class TVEpisode

        Private _compositeID As String = String.Empty
        Private _playListPlayer As MediaPortal.Playlists.PlayListPlayer


        Public Property compositeID() As String
            Get
                Return _compositeID
            End Get
            Set(ByVal value As String)
                _compositeID = value
            End Set
        End Property

        Public Sub PlayEpisode()
            SyncLock Me
                Dim t As New Thread(New ThreadStart(AddressOf StartPlayingEpisode))
                t.Start()
            End SyncLock
        End Sub

        Private Sub StartPlayingEpisode()

            Dim sqlCondition As New SQLCondition
            sqlCondition.Add(New DBEpisode(), DBEpisode.cCompositeID, _compositeID, SQLConditionType.Equal)

            Dim episodeList As List(Of DBEpisode) = DBEpisode.Get(sqlCondition)
            If (episodeList.Count > 0) Then

                If g_Player.Playing Then g_Player.Stop()

                _playListPlayer = New MediaPortal.Playlists.PlayListPlayer
                _playListPlayer = MediaPortal.Playlists.PlayListPlayer.SingletonPlayer
                _playListPlayer.RepeatPlaylist = False

                _playListPlayer.GetPlaylist(MediaPortal.Playlists.PlayListType.PLAYLIST_VIDEO).Clear()
                Dim item As New MediaPortal.Playlists.PlayListItem
                item.FileName = episodeList(0).Item(DBEpisode.cFilename)
                item.Type = MediaPortal.Playlists.PlayListItem.PlayListItemType.Video
                _playListPlayer.GetPlaylist(MediaPortal.Playlists.PlayListType.PLAYLIST_VIDEO).Add(item)

                If _playListPlayer.GetPlaylist(MediaPortal.Playlists.PlayListType.PLAYLIST_VIDEO).Count > 0 Then
                    _playListPlayer.Reset()
                    _playListPlayer.CurrentPlaylistType = MediaPortal.Playlists.PlayListType.PLAYLIST_VIDEO
                    _playListPlayer.Play(0)
                End If

            End If

        End Sub

    End Class

End Namespace
