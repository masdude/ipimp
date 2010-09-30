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


Imports MediaPortal.Player
Imports MediaPortal.Music.Database
Imports MediaPortal.Video.Database
Imports Jayrock.Json

Namespace MPClientController

    Public Class NowPlaying

        Public Shared Function GetNowPlaying(ByVal useMovingPictures As Boolean) As String

            Dim result As String = String.Empty

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("media")

            If g_Player.Playing Then

                If g_Player.IsMusic Then
                    Dim song As New Song
                    Dim musicdb As MusicDatabase = MusicDatabase.Instance
                    musicdb.GetSongByFileName(g_Player.Player.CurrentFile, song)
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
                    jw.WriteMember("filename")
                    jw.WriteString(g_Player.Player.CurrentFile)
                    jw.WriteMember("year")
                    jw.WriteString(song.Year.ToString)
                ElseIf g_Player.IsVideo Then
                    If MediaPortal.Util.Utils.IsDVD(g_Player.Player.CurrentFile) Then
                        jw.WriteString("dvd")
                    Else
                        Dim movie As New IMDBMovie
                        Dim movieID As Integer = 0
                        movieID = VideoDatabase.GetMovieId(g_Player.Player.CurrentFile)
                        If movieID > 0 Then
                            VideoDatabase.GetMovieInfoById(movieID, movie)
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
                            jw.WriteMember("plot")
                            jw.WriteString(movie.Plot)
                            jw.WriteMember("director")
                            jw.WriteString(movie.Director)
                            jw.WriteMember("year")
                            jw.WriteString(movie.Year)
                            jw.WriteMember("rating")
                            jw.WriteString(movie.Rating)
                        Else
                            If useMovingPictures Then
                                Dim s As String = MovingPictures.GetPlayingMovie
                                If InStr(s, "movingpicture") > 0 Then
                                    Return s
                                End If
                            End If
                        End If
                    End If
                ElseIf g_Player.IsTVRecording Then
                    jw.WriteString("recording")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                    jw.WriteMember("filename")
                    jw.WriteString(MediaPortal.Util.Utils.SplitFilename(g_Player.Player.CurrentFile.ToString))
                ElseIf g_Player.IsRadio Then
                    jw.WriteString("radio")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                ElseIf g_Player.IsTV Or (g_Player.IsTimeShifting) Then
                    jw.WriteString("tv")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                Else
                    jw.WriteString("unknown")
                End If
                jw.WriteMember("duration")
                jw.WriteString(g_Player.Player.Duration.ToString)
                jw.WriteMember("position")
                jw.WriteString(g_Player.Player.CurrentPosition.ToString)
                jw.WriteMember("volume")
                If VolumeHandler.Instance.IsMuted Then
                    jw.WriteNumber(0)
                Else
                    jw.WriteString(Math.Floor(100.0 * VolumeHandler.Instance.Volume / VolumeHandler.Instance.Maximum))
                End If
                jw.WriteMember("playstatus")
                If (g_Player.Player.Paused) Then
                    jw.WriteString("paused")
                Else
                    jw.WriteString("playing")
                End If
            Else
                jw.WriteString("nothing")
            End If

            jw.WriteEndObject()

            Return jw.ToString

        End Function

    End Class

End Namespace