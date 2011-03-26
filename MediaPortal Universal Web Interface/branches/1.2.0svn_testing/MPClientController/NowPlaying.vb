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


Imports System.IO
Imports MediaPortal.Player
Imports MediaPortal.Music.Database
Imports MediaPortal.Video.Database
Imports Jayrock.Json
Imports MediaPortal.Util
Imports MediaPortal.GUI.Library

Namespace MPClientController

    Public NotInheritable Class NowPlaying

        Private Sub New()
        End Sub

        Public Shared Function GetNowPlaying(ByVal isMovingPicturesPresent As Boolean, ByVal isTVSeriesPresent As Boolean, ByVal isFanartHandlerPresent As Boolean) As String

            Using jw As New JsonTextWriter
                Try
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
                            jw.WriteMember("thumb")
                            Dim thumbNailFileName As String
                            thumbNailFileName = MediaPortal.Util.Utils.GetAlbumThumbName(song.Artist, song.Album)
                            thumbNailFileName = MediaPortal.Util.Utils.ConvertToLargeCoverArt(thumbNailFileName)
                            If (File.Exists(thumbNailFileName)) Then
                                jw.WriteString(String.Format("{0}:{1}:{2}", "musicalbum", song.Artist, song.Album))
                            Else
                                jw.WriteString("")
                            End If
                            jw.WriteMember("fanart")
                            If isFanartHandlerPresent Then
                                jw.WriteString(MPCCFanArt.GetFanArtForSong(song))
                            Else
                                jw.WriteString("")
                            End If


                        ElseIf g_Player.IsVideo Then
                            If MediaPortal.Util.Utils.IsDVD(g_Player.Player.CurrentFile) Then
                                jw.WriteString("dvd")
                            Else
                                Dim movie As New IMDBMovie
                                Dim movieID As Integer = 0
                                movieID = VideoDatabase.GetMovieId(g_Player.Player.CurrentFile)
                                VideoDatabase.GetMovieInfoById(movieID, movie)
                                If movie.ID > 0 Then
                                    Dim fileList As New ArrayList
                                    VideoDatabase.GetFiles(movie.ID, fileList)

                                    Dim files As String = Join(fileList.ToArray, ";")
                                    Dim crc As New CRCTool
                                    crc.Init(CRCTool.CRCCode.CRC32)

                                    jw.WriteString("video")
                                    jw.WriteMember("title")
                                    jw.WriteString(movie.Title)
                                    jw.WriteMember("thumb")
                                    If (File.Exists(String.Format("{0}\{1}L{2}", Thumbs.MovieTitle, MediaPortal.Util.Utils.MakeFileName(movie.Title), MediaPortal.Util.Utils.GetThumbExtension()))) Then
                                        jw.WriteString(String.Format("videotitle:{0}", MediaPortal.Util.Utils.MakeFileName(movie.Title)))
                                    ElseIf (File.Exists(String.Format("{0}\{1}L{2}", Thumbs.Videos, crc.calc(files).ToString, MediaPortal.Util.Utils.GetThumbExtension()))) Then
                                        jw.WriteString(String.Format("videothumb:{0}", crc.calc(files).ToString))
                                    Else
                                        jw.WriteString("")
                                    End If
                                    jw.WriteMember("fanart")
                                    jw.WriteString("")
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
                                    Dim Found As Boolean = False
                                    If isMovingPicturesPresent Then
                                        Found = MovingPictures.FillNowPlaying(jw)
                                    End If
                                    If (Not Found And isTVSeriesPresent) Then
                                        Found = TVSeries.FillNowPlaying(jw)
                                    End If
                                    If (Not Found) Then
                                        jw.WriteString("unknown")
                                        jw.WriteMember("filename")
                                        jw.WriteString(g_Player.Player.CurrentFile)
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
                            jw.WriteMember("channel")
                            jw.WriteString(GUIPropertyManager.GetProperty("#Play.Current.Title"))
                        ElseIf g_Player.IsTV Or (g_Player.IsTimeShifting) Then
                            jw.WriteString("tv")
                            jw.WriteMember("hostname")
                            jw.WriteString(System.Net.Dns.GetHostName.ToString)
                        Else
                            jw.WriteString("unknown")
                            jw.WriteMember("filename")
                            jw.WriteString(g_Player.Player.CurrentFile)
                        End If
                        jw.WriteMember("duration")
                        jw.WriteString(CInt(g_Player.Player.Duration).ToString)
                        jw.WriteMember("position")
                        jw.WriteString(CInt(g_Player.Player.CurrentPosition).ToString)
                        jw.WriteMember("volume")
                        If VolumeHandler.Instance.IsMuted Then
                            jw.WriteNumber(0)
                        Else
                            jw.WriteString(CInt(Math.Floor(100.0 * VolumeHandler.Instance.Volume / VolumeHandler.Instance.Maximum)))
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
                Catch ex As Exception
                    Return iPiMPUtils.SendError(7, "Error processing request." & ex.Message)
                End Try
            End Using

        End Function

    End Class

End Namespace