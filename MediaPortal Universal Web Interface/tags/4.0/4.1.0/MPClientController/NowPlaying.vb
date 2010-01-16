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


Imports MediaPortal.Player
Imports MediaPortal.Music.Database
Imports MediaPortal.Video.Database
Imports MediaPortal.Util.Utils
Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Namespace MPClientController


    Public Class NowPlaying

        Public Shared Function GetNowPlaying() As String

            Dim result As String = String.Empty

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()

            If g_Player.Playing Then

                If g_Player.IsMusic Then
                    Dim song As New Song
                    Dim musicdb As MusicDatabase = MusicDatabase.Instance
                    musicdb.GetSongByFileName(g_Player.Player.CurrentFile, song)
                    jw.WriteMember("media")
                    jw.WriteString("music")
                    jw.WriteMember("album")
                    jw.WriteString(song.Album)
                    jw.WriteMember("artist")
                    jw.WriteString(song.Artist)
                    jw.WriteMember("title")
                    jw.WriteString(song.Title)
                    jw.WriteMember("genre")
                    jw.WriteString(song.Genre)
                    jw.WriteMember("track")
                    jw.WriteString(song.Track.ToString)
                ElseIf g_Player.IsVideo Then
                    If MediaPortal.Util.Utils.IsDVD(g_Player.Player.CurrentFile) Then
                        jw.WriteMember("media")
                        jw.WriteString("dvd")
                    Else
                        Dim movie As New IMDBMovie
                        Dim movieID As Integer = 0
                        movieID = VideoDatabase.GetMovieId(g_Player.Player.CurrentFile)
                        If movieID > 0 Then
                            VideoDatabase.GetMovieInfoById(movieID, movie)
                            jw.WriteMember("media")
                            jw.WriteString("video")
                            jw.WriteMember("title")
                            jw.WriteString(movie.Title)
                            jw.WriteMember("tagline")
                            jw.WriteString(movie.TagLine)
                            jw.WriteMember("id")
                            jw.WriteString(movie.ID)
                            jw.WriteMember("genre")
                            jw.WriteString(movie.Genre)
                            jw.WriteMember("filename")
                            jw.WriteString(MediaPortal.Util.Utils.SplitFilename(g_Player.Player.CurrentFile.ToString))
                        Else
                            Dim s As String = MovingPictures.GetPlayingMovie
                            If InStr(s, "movingpicture") > 0 Then
                                Return s
                            End If
                        End If
                    End If
                ElseIf g_Player.IsTVRecording Then
                    jw.WriteMember("media")
                    jw.WriteString("recording")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                    jw.WriteMember("filename")
                    jw.WriteString(MediaPortal.Util.Utils.SplitFilename(g_Player.Player.CurrentFile.ToString))
                ElseIf g_Player.IsRadio Then
                    jw.WriteMember("media")
                    jw.WriteString("radio")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                ElseIf g_Player.IsTV Or (g_Player.IsTimeShifting) Then
                    jw.WriteMember("media")
                    jw.WriteString("tv")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                Else
                    jw.WriteMember("media")
                    jw.WriteString("unknown")
                End If
                jw.WriteMember("duration")
                jw.WriteString(g_Player.Player.Duration.ToString)
                jw.WriteMember("position")
                jw.WriteString(g_Player.Player.CurrentPosition.ToString)
            Else
                jw.WriteMember("media")
                jw.WriteString("nothing")
            End If

            jw.WriteEndObject()

            Return jw.ToString

        End Function

    End Class

End Namespace