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
Imports Jayrock.Json
Imports MediaPortal.Configuration
Imports MediaPortal.Player
Imports MediaPortal.GUI.Library

Namespace MPClientController

    Public Class iPiMPUtils

        Public Shared Function SendString(ByVal str As String, ByVal val As String) As String

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember(str)
            jw.WriteString(val)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function SendBool(ByVal val As Boolean) As String

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(val)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function SendError(ByVal errorCode As Integer, ByVal errorMessage As String) As String

            Dim jw As New JsonTextWriter
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

        End Function

        Public Shared Sub TextLog(ByVal text As String)

            Dim filename As String = "C:\LogMPController.txt"

            If System.IO.File.Exists(filename) = True Then
                Dim objWriter As New System.IO.StreamWriter(filename, True)
                objWriter.WriteLine(text)
                objWriter.Close()
            Else
                Dim objWriter As New System.IO.StreamWriter(filename, False)
                objWriter.WriteLine(text)
                objWriter.Close()
            End If

        End Sub

        Public Shared Function IsPluginLoaded(ByVal dll As String, Optional ByVal type As String = "windows") As Boolean

            If File.Exists(String.Format("{0}\{1}\{2}", Config.Dir.Plugins, type, dll)) Then
                If PluginManager.IsPlugInEnabled(dll) Then Return True
            End If

            Return False

        End Function

        Public Shared Function GetImage(ByVal filename As String) As String

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")

            If (File.Exists(filename)) Then

                Dim stream As MemoryStream = New MemoryStream()
                Dim image As Image = Drawing.Image.FromFile(filename)
                Dim format As Imaging.ImageFormat
                Dim ext As String = Path.GetExtension(filename)

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

                jw.WriteBoolean(True)
                jw.WriteMember("filetype")
                jw.WriteString(format.ToString)
                jw.WriteMember("filename")
                jw.WriteString(Path.GetFileName(filename))
                jw.WriteMember("data")
                jw.WriteString(Convert.ToBase64String(stream.ToArray()))
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

        End Function

        Public Shared Function GetFile(ByVal url As String, ByVal size As String) As String

            Dim req As String() = Split(url, ":")

            If (Not size.ToLower = "small") Then
                size = "large"
            End If

            If (req.Length < 2) Then
                Return String.Empty
            Else
                Select Case req(0)
                    Case "videothumb"
                        Return MyVideos.GetVideoThumb(req(1), size)
                    Case "videotitle"
                        Return MyVideos.GetVideoTitle(req(1), size)
                    Case "musicthumb"
                        Return MyMusic.GetMusicThumb(req(1), req(2), size)
                    Case Else
                        Return SendError(5, "Unknown file url.")
                End Select
            End If

            Return String.Empty

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
