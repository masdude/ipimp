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


Imports Jayrock.Json
Imports MediaPortal.Music.Database
Imports MediaPortal.Player
Imports MediaPortal.Playlists
Imports MediaPortal.GUI.Library
Imports MediaPortal.Configuration

Imports System.IO
Imports System.Drawing

Namespace MPClientController

    Public Class MyMusic

        Private Shared playlistPlayer As PlayListPlayer

        Private Class MPClientAlbumArtistInfo
            Public Album As String
            Public Artist As String
        End Class

        Private Class MPClientTrackInfo
            Public Title As String
            Public ID As Integer
            Public Artist As String
        End Class

        Private Class MPClientAlbumArtistInfoComparer
            Implements IComparer(Of MPClientAlbumArtistInfo)
            Public Overridable Overloads Function Compare(ByVal x As MPClientAlbumArtistInfo, ByVal y As MPClientAlbumArtistInfo) As Integer Implements IComparer(Of MPClientAlbumArtistInfo).Compare
                Return x.Album.CompareTo(y.Album)
            End Function
        End Class

        Private Class MPClientTrackFilenameInfoComparer
            Implements IComparer(Of MPClientTrackInfo)
            Public Overridable Overloads Function Compare(ByVal x As MPClientTrackInfo, ByVal y As MPClientTrackInfo) As Integer Implements IComparer(Of MPClientTrackInfo).Compare
                Return x.Title.CompareTo(y.Title)
            End Function
        End Class

        Public Shared Function GetMusicFilters(ByVal filter As String, Optional ByVal value As String = "") As String

            Dim filters As New ArrayList
            Dim musicDB As MusicDatabase = MusicDatabase.Instance

            Select Case filter.ToLower
                Case "genre"
                    musicDB.GetGenres(filters)

                Case "decade"
                    Dim decade As String = String.Empty
                    Dim subset As New ArrayList
                    Dim songs As New List(Of Song)
                    musicDB.GetSongsByFilter("select distinct iYear from tracks", songs, "album")
                    For Each song As Song In songs
                        If song.Year = 0 Then
                            decade = "0000"
                        Else
                            decade = String.Format("{0}0", Left(song.Year.ToString, 3))
                        End If
                        If Not subset.Contains(decade) Then subset.Add(decade)
                    Next
                    subset.Sort()
                    subset.Reverse()
                    filters = subset

                Case "year"
                    'value is the decade to return years for
                    Dim year As String = String.Empty
                    Dim subset As New ArrayList
                    Dim songs As New List(Of Song)
                    musicDB.GetSongsByFilter("select distinct iYear from tracks", songs, "album")
                    For Each song As Song In songs
                        If song.Year = 0 Then
                            year = "0000"
                        Else
                            year = song.Year.ToString
                        End If
                        If Left(year.ToLower, 3) = Left(value.ToLower, 3) Then
                            If Not subset.Contains(year) Then subset.Add(year)
                        End If
                    Next
                    subset.Sort()
                    subset.Reverse()
                    filters = subset

                Case "artistletter"
                    Dim subset As New ArrayList
                    musicDB.GetAllArtists(filters)
                    For Each artist As String In filters
                        Dim letter As New Char
                        letter = Left(artist, 1).ToUpper
                        If Not Char.IsLetter(letter) Then letter = "0"
                        If Not subset.Contains(letter) Then subset.Add(letter)
                    Next
                    subset.Sort()
                    filters = subset

                Case "artist"
                    'value is the artist letter return artists for - or * for all
                    Dim subset As New ArrayList
                    musicDB.GetAllArtists(filters)
                    If Not value = "*" Then
                        For Each artist As String In filters
                            Dim letter As Char = Left(artist, 1).ToUpper
                            If Not Char.IsLetter(letter) Then letter = "0"
                            If letter = value.ToUpper Then
                                If Not subset.Contains(artist) Then subset.Add(artist)
                            End If
                        Next
                        subset.Sort()
                        filters = subset
                    End If

                Case "albumletter"
                    Dim subset As New ArrayList
                    Dim songs As New List(Of Song)
                    musicDB.GetSongsByFilter("select * from tracks group by strAlbum order by strAlbum", songs, "album")
                    For Each song As Song In songs
                        Dim letter As New Char
                        letter = Left(song.Album, 1).ToUpper
                        If Not Char.IsLetter(letter) Then letter = "0"
                        If Not subset.Contains(letter) Then subset.Add(letter)
                    Next
                    subset.Sort()
                    filters = subset

            End Select

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember(filter.ToLower)
            jw.WriteStringArray(filters.ToArray)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetMusicAlbums(Optional ByVal filter As String = "", _
                                              Optional ByVal value As String = "", _
                                              Optional ByVal start As Integer = 0, _
                                              Optional ByVal pagesize As Integer = 0) As String

            Dim filters As New ArrayList
            Dim songs As New List(Of Song)
            Dim subset As New List(Of Song)
            Dim musicDB As MusicDatabase = MusicDatabase.Instance

            Select Case filter.ToLower
                Case "genre"
                    musicDB.GetSongsByGenre(value.ToLower, songs)
                Case "artist"
                    musicDB.GetSongsByArtist(value.ToLower, songs)
                Case "year"
                    musicDB.GetSongsByYear(CInt(value), songs)
                Case "letter"
                    musicDB.GetSongsByFilter("select * from tracks group by strAlbum order by strAlbum", songs, "album")
                    For Each song As Song In songs
                        If (value.ToLower = Left(song.Album, 1).ToLower) And Not subset.Contains(song) Then subset.Add(song)
                    Next
                    songs = subset
                Case Else
                    musicDB.GetSongsByFilter("select * from tracks group by strAlbum order by strAlbum", songs, "album")
            End Select

            Dim MPClientAlbumArtistInfos As New List(Of MPClientAlbumArtistInfo)
            Dim added As Boolean = False

            For Each song In songs
                Dim MPClientAlbumArtistInfo As New MPClientAlbumArtistInfo
                MPClientAlbumArtistInfo.Album = song.Album

                If filter.ToLower = "artist" Then
                    MPClientAlbumArtistInfo.Artist = song.Artist
                Else
                    MPClientAlbumArtistInfo.Artist = song.AlbumArtist
                End If

                For Each AlbumArtistInfo As MPClientAlbumArtistInfo In MPClientAlbumArtistInfos
                    If ((AlbumArtistInfo.Album = MPClientAlbumArtistInfo.Album) And (AlbumArtistInfo.Artist = MPClientAlbumArtistInfo.Artist)) Then
                        added = True
                    End If
                Next

                If Not added Then
                    MPClientAlbumArtistInfos.Add(MPClientAlbumArtistInfo)
                Else
                    added = False
                End If

            Next

            MPClientAlbumArtistInfos.Sort(New MPClientAlbumArtistInfoComparer)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("albums")
            jw.WriteStartArray()
            If pagesize = 0 Then
                For Each MPClientAlbumArtistInfo As MPClientAlbumArtistInfo In MPClientAlbumArtistInfos
                    jw.WriteStartObject()
                    jw.WriteMember("Album")
                    jw.WriteString(MPClientAlbumArtistInfo.Album)
                    jw.WriteMember("Artist")
                    jw.WriteString(MPClientAlbumArtistInfo.Artist)
                    jw.WriteEndObject()
                Next
            Else
                For i As Integer = start To (start + (pagesize - 1))
                    jw.WriteStartObject()
                    jw.WriteMember("Album")
                    jw.WriteString(MPClientAlbumArtistInfos(i).Album)
                    jw.WriteMember("Artist")
                    jw.WriteString(MPClientAlbumArtistInfos(i).Artist)
                    jw.WriteEndObject()
                Next
            End If
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetMusicTracks(Optional ByVal filter As String = "", _
                                              Optional ByVal value As String = "", _
                                              Optional ByVal start As Integer = 0, _
                                              Optional ByVal pagesize As Integer = 0) As String

            Dim filters As New ArrayList
            Dim songs As New List(Of Song)
            Dim subset As New List(Of Song)
            Dim musicDB As MusicDatabase = MusicDatabase.Instance

            Select Case filter.ToLower
                Case "genre"
                    musicDB.GetSongsByGenre(value.ToLower, songs)
                Case "artist"
                    musicDB.GetSongsByArtist(value.ToLower, songs)
                Case "year"
                    musicDB.GetSongsByYear(CInt(value), songs)
                Case "letter"
                    musicDB.GetSongsByFilter("select * from tracks group by strAlbum order by strAlbum", songs, "album")
                    For Each song As Song In songs
                        If (value.ToLower = Left(song.Album, 1).ToLower) And Not subset.Contains(song) Then songs.Add(song)
                    Next
                Case "playlist"
                    Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
                    Dim playlistDir As String = xmlReader.GetValueAsString("music", "playlists", "")
                    For Each item As String In Directory.GetFiles(playlistDir, "*.m3u")
                        If value.ToLower = Path.GetFileNameWithoutExtension(item).ToLower Then
                            Dim track As String = String.Empty
                            Dim sr As StreamReader = New StreamReader(item)
                            Do While sr.Peek() >= 0
                                track = sr.ReadLine()
                                If Not Left(track, 1) = "#" Then
                                    Dim song As New Song
                                    musicDB.GetSongByFileName(track, song)
                                    songs.Add(song)
                                End If
                            Loop
                            sr.Close()
                        End If
                    Next
                Case "search"
                    songs = SearchMusic(value, start, pagesize)
                Case Else
                    musicDB.GetSongsByFilter("select * from tracks group by strAlbum order by strAlbum", songs, "album")
            End Select

            Dim MPClientTrackInfos As New List(Of MPClientTrackInfo)
            Dim added As Boolean = False

            For Each song In songs
                Dim MPClientTrackInfo As New MPClientTrackInfo
                MPClientTrackInfo.Title = song.Title
                MPClientTrackInfo.ID = song.Id

                If filter.ToLower = "artist" Then
                    MPClientTrackInfo.Artist = song.Artist
                Else
                    MPClientTrackInfo.Artist = song.AlbumArtist
                End If

                For Each TrackInfo As MPClientTrackInfo In MPClientTrackInfos
                    If ((TrackInfo.Title = MPClientTrackInfo.Title) And (TrackInfo.Artist = MPClientTrackInfo.Artist)) Then
                        added = True
                    End If
                Next

                If Not added Then
                    MPClientTrackInfos.Add(MPClientTrackInfo)
                Else
                    added = False
                End If
            Next

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("tracks")
            jw.WriteStartArray()
            If pagesize = 0 Then
                For Each MPClientTrackInfo As MPClientTrackInfo In MPClientTrackInfos
                    jw.WriteStartObject()
                    jw.WriteMember("Title")
                    jw.WriteString(MPClientTrackInfo.Title)
                    jw.WriteMember("Artist")
                    jw.WriteString(MPClientTrackInfo.Artist)
                    jw.WriteMember("ID")
                    jw.WriteString(MPClientTrackInfo.ID)
                    jw.WriteEndObject()
                Next
            Else
                Dim finish As Integer = 0
                If MPClientTrackInfos.Count > start + (pagesize - 1) Then
                    finish = start + (pagesize - 1)
                Else
                    finish = MPClientTrackInfos.Count - 1
                End If
                For i As Integer = start To finish
                    If MPClientTrackInfos.Count >= i - 1 Then
                        jw.WriteStartObject()
                        jw.WriteMember("Title")
                        jw.WriteString(MPClientTrackInfos(i).Title)
                        jw.WriteMember("Artist")
                        jw.WriteString(MPClientTrackInfos(i).Artist)
                        jw.WriteMember("ID")
                        jw.WriteString(MPClientTrackInfos(i).ID)
                        jw.WriteEndObject()
                    End If
                Next
            End If
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetAlbum(ByVal album As String, ByVal artist As String) As String

            Dim songs As New ArrayList
            Dim musicDB As MusicDatabase = MusicDatabase.Instance
            musicDB.GetSongsByAlbum(album, songs)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("album")
            jw.WriteStartArray()
            For Each song As Song In songs
                If (song.Album.ToLower = album.ToLower) And (song.Artist.ToLower = artist.ToLower) Then
                    jw.WriteStartObject()
                    jw.WriteMember("Title")
                    jw.WriteString(song.Title)
                    jw.WriteMember("ID")
                    jw.WriteString(song.Id)
                    jw.WriteMember("Filename")
                    jw.WriteString(song.FileName)
                    jw.WriteEndObject()
                End If
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetMusicThumb(ByVal artist As String, ByVal album As String, ByVal size As String) As String

            Dim thumbNailFileName As String
            thumbNailFileName = MediaPortal.Util.Utils.GetAlbumThumbName(artist, album)
            thumbNailFileName = MediaPortal.Util.Utils.ConvertToLargeCoverArt(thumbNailFileName)

            Return iPiMPUtils.GetImage(thumbNailFileName)

        End Function


        Public Shared Function GetCoverArt(ByVal filter As String, ByVal value As String) As String

            Dim thumbNailFileName As String
            thumbNailFileName = MediaPortal.Util.Utils.GetAlbumThumbName(filter, value)
            thumbNailFileName = MediaPortal.Util.Utils.ConvertToLargeCoverArt(thumbNailFileName)

            Dim image As Image = Nothing
            Dim stream As MemoryStream = New MemoryStream()

            If File.Exists(thumbNailFileName) Then
                image = image.FromFile(thumbNailFileName)
                Dim format As Imaging.ImageFormat
                Dim ext As String = Path.GetExtension(thumbNailFileName)

                Select Case ext.ToLower
                    Case ".jpg"
                        format = Imaging.ImageFormat.Jpeg
                    Case ".png"
                        format = Imaging.ImageFormat.Png
                    Case ".gif"
                        format = Imaging.ImageFormat.Gif
                    Case ".bmp"
                        format = Imaging.ImageFormat.Bmp
                    Case Else
                        format = Nothing
                End Select
                image.Save(stream, format)
            Else

            End If

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("image")
            If Not image Is Nothing Then
                jw.WriteString(Convert.ToBase64String(Stream.ToArray()))
            Else
                jw.WriteString("empty")
            End If
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetPlaylists(Optional ByVal random As Boolean = False) As String

            System.Threading.Thread.Sleep(100)

            Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
            Dim playlistDir As String = xmlReader.GetValueAsString("music", "playlists", "")
            Dim playlistFiles As String() = Directory.GetFiles(playlistDir, "*.m3u")

            Dim playlists As New List(Of String)
            Dim playlist As String

            If playlistFiles.Count > 0 Then
                If random Then
                    Dim index As Integer = New MediaPortal.Util.PseudoRandomNumberGenerator().Next(0, (playlistFiles.Length - 1))
                    playlists.Add(Path.GetFileNameWithoutExtension(playlistFiles(index)))
                Else
                    For Each playlist In Directory.GetFiles(playlistDir, "*.m3u")
                        playlists.Add(Path.GetFileNameWithoutExtension(playlist))
                    Next
                End If
            Else
                playlists.Add("noplaylists")
            End If

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("playlists")
            jw.WriteStringArray(playlists.ToArray)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function PlayMusic(ByVal album As String, _
                                         ByVal artist As String, _
                                         ByVal tracks As String, _
                                         Optional ByVal shuffle As Boolean = False, _
                                         Optional ByVal enqueue As Boolean = False) As String

            Dim musicDB As MusicDatabase = MusicDatabase.Instance
            playlistPlayer = playlistPlayer.SingletonPlayer

            Dim songs As New ArrayList
            musicDB.GetSongsByAlbum(album, songs)

            If Not enqueue Then playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Clear()

            Dim playTracks() As String = Split(tracks, ",")
            Dim i As Integer = 0
            For Each Song As Song In songs
                If (Song.Album.ToLower = album.ToLower) And (Song.Artist.ToLower = artist.ToLower) Then
                    i += 1
                    Dim item As New PlayListItem(Song.Title, Song.FileName)
                    If playTracks.Contains(i.ToString) Then playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Add(item)
                End If
            Next

            If (playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Count > 0) AndAlso shuffle Then
                playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Shuffle()
            End If

            If Not enqueue Then
                StartPlayback()
            Else
                RefreshPlaylistWindow()
            End If

            If g_Player.Playing And g_Player.IsMusic Then
                Return iPiMPUtils.SendBool(True)
            Else
                Return iPiMPUtils.SendBool(False)
            End If

        End Function

        Public Shared Function PlayRandom(ByVal filter As String, ByVal value As String) As String

            Dim musicDB As MusicDatabase = MusicDatabase.Instance
            playlistPlayer = playlistPlayer.SingletonPlayer
            playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Clear()

            Dim itemcount As Integer = 100
            Dim i As Integer = 0
            Dim song As New Song
            Dim songs As New List(Of Song)
            Dim subset As New List(Of Song)

            Select Case filter.ToLower
                Case "all"
                    Do Until i = itemcount
                        musicDB.GetRandomSong(song)
                        songs.Add(song)
                        i += 1
                    Loop
                Case "genre"
                    musicDB.GetSongsByGenre(value, songs)
                Case "artist"
                    musicDB.GetSongsByArtist(value, songs)
                Case "year"
                    musicDB.GetSongsByYear(value, songs)
                Case "decade"
                    If value.ToLower = "0000" Then
                        musicDB.GetSongsByFilter("select * from tracks where iYear = 0", songs, "album")
                    Else
                        Dim lYear = Left(value.ToLower, 3) & "0"
                        Dim uYear = Left(value.ToLower, 3) & "9"
                        musicDB.GetSongsByFilter("select * from tracks where (iYear >= " & lYear & ") and (iYear <= " & uYear & ")", songs, "album")
                    End If
                Case "letter"
                    musicDB.GetSongsByFilter("select * from tracks", songs, "album")
                    For Each song In songs
                        If (value.ToLower = Left(song.Album, 1).ToLower) And Not subset.Contains(song) Then subset.Add(song)
                    Next
                    songs = subset

            End Select

            If songs.Count <= itemcount Then
                For Each song In songs
                    Dim item As New PlayListItem(song.Title, song.FileName)
                    playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Add(item)
                Next
                playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Shuffle()
            Else
                Do Until i = itemcount
                    Dim index As Integer = New MediaPortal.Util.PseudoRandomNumberGenerator().Next(0, (songs.Count - 1))
                    Dim item As New PlayListItem(songs(index).Title, songs(index).FileName)
                    playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Add(item)
                    i += 1
                    songs.RemoveAt(index)
                Loop
            End If

            StartPlayback()
            RefreshPlaylistWindow()

            If g_Player.Playing And g_Player.IsMusic Then
                Return iPiMPUtils.SendBool(True)
            Else
                Return iPiMPUtils.SendBool(False)
            End If

        End Function

        Public Shared Function PlayTracks(ByVal tracks As String, _
                                 Optional ByVal shuffle As Boolean = False, _
                                 Optional ByVal enqueue As Boolean = False) As String

            Dim musicDB As MusicDatabase = MusicDatabase.Instance
            playlistPlayer = playlistPlayer.SingletonPlayer

            Dim trackIDs() As String
            trackIDs = Split(tracks, ",")

            If Not enqueue Then playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Clear()

            For Each trackID As String In trackIDs
                Dim songs As New List(Of Song)
                If trackID <> "" Then
                    musicDB.GetSongsByFilter("select * from tracks where idTrack = " & trackID.ToString, songs, "album")
                    For Each Song As Song In songs
                        Dim item As New PlayListItem(Song.Title, Song.FileName)
                        playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Add(item)
                    Next
                End If
            Next

            If (playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Count > 0) AndAlso shuffle Then
                playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Shuffle()
            End If

            If Not enqueue Then
                StartPlayback()
            Else
                RefreshPlaylistWindow()
            End If

            If g_Player.Playing And g_Player.IsMusic Then
                Return iPiMPUtils.SendBool(True)
            Else
                Return iPiMPUtils.SendBool(False)
            End If

        End Function

        Private Shared Sub StartPlayback()

            If playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Count > 0 Then
                playlistPlayer.Reset()
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_MUSIC
                playlistPlayer.Play(0)
            End If

            RefreshPlaylistWindow()

        End Sub

        Public Shared Sub RefreshPlaylistWindow()

            If playlistPlayer.g_Player.Playing AndAlso _
               GUIWindowManager.Initalized AndAlso _
               (GUIWindowManager.ActiveWindow = 500) Then
                'Music playlist = 500 Now Playing = 510
                'GUIWindowManager.ActivateWindow(500) 
                GUIWindowManager.ReplaceWindow(500)
            End If

        End Sub

        Private Shared Function SearchMusic(ByVal value As String, _
                                           Optional ByVal start As Integer = 0, _
                                           Optional ByVal pagesize As Integer = 0) As List(Of Song)

            Dim musicDB As MusicDatabase = MusicDatabase.Instance
            Dim songs As New List(Of Song)

            Dim artist, album, track, genre As String
            artist = Split(value, ",")(0)
            album = Split(value, ",")(1)
            track = Split(value, ",")(2)
            genre = Split(value, ",")(3)

            Dim whereArtist As String = "(strArtist like '%" & artist & "%')"
            Dim whereAlbum As String = "(strAlbum like '%" & album & "%')"
            Dim whereTrack As String = "(strTitle like '%" & track & "%')"
            Dim whereGenre As String = "(strGenre like '%" & genre & "%')"

            Dim sqlQuery As String = "select * from tracks "
            If artist = "" Then
                If album = "" Then
                    If track = "" Then
                        If genre = "" Then
                            'Duh, no search entries supplied - should never get here
                            Return Nothing
                        Else
                            'No artist, album or track, genre is supplied
                            sqlQuery += "where " & whereGenre
                        End If
                    Else
                        'No artist or album, track is supplied
                        sqlQuery += "where " & whereTrack
                        If genre <> "" Then sqlQuery += " and " & whereGenre
                    End If
                Else
                    'No artist, album is supplied
                    sqlQuery += "where " & whereAlbum
                    If track <> "" Then sqlQuery += " and " & whereTrack
                    If genre <> "" Then sqlQuery += " and " & whereGenre
                End If
            Else
                'Artist is supplied
                sqlQuery += "where " & whereArtist
                If album <> "" Then sqlQuery += " and " & whereAlbum
                If track <> "" Then sqlQuery += " and " & whereTrack
                If genre <> "" Then sqlQuery += " and " & whereGenre
            End If

            musicDB.GetSongsByFilter(sqlQuery, songs, "album")

            Return songs

        End Function

        Public Shared Sub UpdateMusicDB()

            Dim musicShares As ArrayList = LoadShares()

            Dim musicDB As MusicDatabase = MusicDatabase.Instance
            musicDB.MusicDatabaseReorg(musicShares)

        End Sub

        Private Shared Function LoadShares() As ArrayList

            Dim _shares As ArrayList = New ArrayList()
            Dim currentFolder As String = String.Empty
            Dim fileMenuEnabled As Boolean = False
            Dim fileMenuPinCode As String = String.Empty
            Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))

            fileMenuEnabled = xmlReader.GetValueAsBool("filemenu", "enabled", True)

            Dim strDefault As String = xmlReader.GetValueAsString("music", "default", String.Empty)
            Dim i As Integer
            For i = 0 To 20 - 1 Step i + 1
                Dim strShareName As String = String.Format("sharename{0}", i)
                Dim strSharePath As String = String.Format("sharepath{0}", i)

                Dim shareType As String = String.Format("sharetype{0}", i)
                Dim shareServer As String = String.Format("shareserver{0}", i)
                Dim shareLogin As String = String.Format("sharelogin{0}", i)
                Dim sharePwd As String = String.Format("sharepassword{0}", i)
                Dim sharePort As String = String.Format("shareport{0}", i)
                Dim remoteFolder As String = String.Format("shareremotepath{0}", i)

                Dim SharePath As String = xmlReader.GetValueAsString("music", strSharePath, String.Empty)

                If SharePath.Length > 0 Then
                    _shares.Add(SharePath)
                End If
            Next

            Return _shares

        End Function

        Public Shared Function SaveCurrentPlaylist(ByVal filename As String) As String

            Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
            Dim playlistDir As String = xmlReader.GetValueAsString("music", "playlists", "")
            Dim result As Boolean = False

            filename = playlistDir & "\" & filename & ".m3u"

            If (playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_MUSIC) And playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Count > 0 Then
                Dim tio As IPlayListIO = PlayListFactory.CreateIO(filename)
                Try
                    tio.Save(playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC), filename)
                    result = True
                Catch ex As Exception
                    result = False
                End Try
            End If

            Return iPiMPUtils.SendBool(result)

        End Function

    End Class

End Namespace
