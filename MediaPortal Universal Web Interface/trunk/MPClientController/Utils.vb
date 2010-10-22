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

Imports System.IO
Imports System.Drawing
Imports System.Diagnostics
Imports Jayrock.Json
Imports MediaPortal.Configuration
Imports MediaPortal.Player
Imports MediaPortal.GUI.Library

Namespace MPClientController

    Public NotInheritable Class iPiMPUtils

        Private Sub New()
        End Sub

        Public Shared Function SendString(ByVal str As String, ByVal val As String) As String

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(True)
                jw.WriteMember(str)
                jw.WriteString(val)
                jw.WriteEndObject()

                Return jw.ToString
            End Using
        End Function

        Public Shared Function SendBool(ByVal val As Boolean) As String

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(val)
                jw.WriteEndObject()

                Return jw.ToString
            End Using
        End Function

        Public Shared Function SendError(ByVal errorCode As Integer, ByVal errorMessage As String) As String

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(False)
                jw.WriteMember("error")
                jw.WriteString(errorMessage)
                jw.WriteMember("errorcode")
                jw.WriteNumber(errorCode)
                jw.WriteEndObject()

                Return jw.ToString
            End Using
        End Function

        Public Shared Function IsPluginLoaded(ByVal dll As String, ByVal minVersion As String, Optional ByVal type As String = "windows") As Boolean

            Dim filename As String = String.Format("{0}\{1}\{2}", Config.Dir.Plugins, type, dll)

            If File.Exists(filename) Then
                Dim fileVersionInfo As FileVersionInfo = fileVersionInfo.GetVersionInfo(filename)
                Dim maj As Integer = Split(minVersion, ".")(0)
                Dim min As Integer = Split(minVersion, ".")(1)
                Dim bld As Integer = Split(minVersion, ".")(2)
                Dim rev As Integer = Split(minVersion, ".")(3)
                Log.Info(String.Format("plugin: MPClientController - Plugin found : {0} ({1}.{2}.{3}.{4})", dll, fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart))
                If PluginManager.IsPlugInEnabled(dll) Then
                    If (fileVersionInfo.FileMajorPart > maj) Then Return True
                    If (fileVersionInfo.FileMajorPart < maj) Then Return False
                    If (fileVersionInfo.FileMinorPart > min) Then Return True
                    If (fileVersionInfo.FileMinorPart < min) Then Return False
                    If (fileVersionInfo.FileBuildPart > bld) Then Return True
                    If (fileVersionInfo.FileBuildPart < bld) Then Return False
                    If (fileVersionInfo.FilePrivatePart >= rev) Then Return True
                End If
            End If

            Return False

        End Function

        Public Shared Function GetImage(ByVal fileName As String) As String

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")

                If (File.Exists(fileName)) Then

                    Try
                        Using stream As MemoryStream = New MemoryStream()
                            Dim image As Image = Drawing.Image.FromFile(fileName)
                            Dim format As Imaging.ImageFormat
                            Dim ext As String = Path.GetExtension(fileName)
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
                            image.Dispose()

                            jw.WriteBoolean(True)
                            jw.WriteMember("filetype")
                            jw.WriteString(format.ToString)
                            jw.WriteMember("filename")
                            jw.WriteString(Path.GetFileName(fileName))
                            jw.WriteMember("data")
                            jw.WriteString(Convert.ToBase64String(stream.ToArray()))
                        End Using

                    Catch ex As Exception
                        Using jw2 = New JsonTextWriter
                            jw2.PrettyPrint = True
                            jw2.WriteStartObject()
                            jw2.WriteMember("result")
                            jw2.WriteBoolean(False)
                            jw2.WriteMember("filetype")
                            jw2.WriteString("nofile")
                            jw2.WriteMember("filename")
                            jw2.WriteString("")
                            jw2.WriteMember("data")
                            jw2.WriteString("")
                            Log.Info("plugin: iPiMPClient - Error converting image {0} - {1}", fileName, ex.Message)
                            jw2.WriteEndObject()
                            Return jw2.ToString()
                        End Using
                    End Try

                Else
                    jw.WriteBoolean(False)
                    jw.WriteMember("filetype")
                    jw.WriteString("nofile")
                    jw.WriteMember("filename")
                    jw.WriteString("")
                    jw.WriteMember("data")
                    jw.WriteString("")
                End If

                jw.WriteEndObject()
                Return jw.ToString
            End Using
        End Function

        Public Shared Function GetFile(ByVal path As String, ByVal size As String) As String

            Dim req As String() = Split(path, ":")

            If (Not size.ToLower = "small") Then
                size = "large"
            End If

            If (req.Length < 2) Then
                Return String.Empty
            Else
                Select Case req(0)
                    Case "tvepisodethumb"
                        Return TVSeries.GetEpisodeThumb(req(1))
                    Case "tvseriesposter"
                        Return TVSeries.GetSeriesPoster(req(1))
                    Case "tvseriesfanart"
                        Return TVSeries.GetSeriesFanart(req(1))
                    Case "tvseasonposter"
                        Return TVSeries.GetSeasonPoster(req(1), req(2))
                    Case "tvseasonfanart"
                        Return TVSeries.GetSeasonFanart(req(1), req(2))
                    Case "videothumb"
                        Return MyVideos.GetVideoThumb(req(1), size)
                    Case "videotitle"
                        Return MyVideos.GetVideoTitle(req(1), size)
                    Case "musicalbum"
                        Return MyMusic.GetAlbumThumb(req(1), String.Join(":", req, 2, req.Length - 2), size)
                    Case "musicartist"
                        Return MyMusic.GetArtistThumb(req(1), size)
                    Case "movingpicturethumb"
                        Return MovingPictures.GetThumb(req(1))
                    Case "movingpicturefanart"
                        Return MovingPictures.GetFanart(req(1))
                    Case Else
                        Return SendError(5, "Unknown file url.")
                End Select
            End If
        End Function

        Public Shared Function SeekPercentage(ByVal percent As Integer) As String

            If g_Player.Playing Then
                g_Player.Player.SeekAsolutePercentage(percent)
                Return iPiMPUtils.SendBool(True)
            Else
                Return iPiMPUtils.SendBool(False)
            End If

        End Function

        Public Shared Function SetVolume(ByVal volume As Integer) As String

            VolumeHandler.Instance.Volume = Math.Floor(volume * VolumeHandler.Instance.Maximum / 100.0)
            Return iPiMPUtils.SendBool(True)

        End Function

    End Class

End Namespace
