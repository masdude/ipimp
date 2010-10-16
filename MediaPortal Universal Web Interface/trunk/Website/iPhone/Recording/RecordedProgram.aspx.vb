﻿' 
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
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordedProgram
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim recordingID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waRecProgram{0}", recordingID)

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        'go
        xw.WriteStartElement("go")
        xw.WriteAttributeString("to", wa)
        xw.WriteEndElement()
        'end go

        'start title
        xw.WriteStartElement("title")
        xw.WriteAttributeString("set", wa)
        xw.WriteEndElement()
        'end title

        'start dest
        xw.WriteStartElement("destination")
        xw.WriteAttributeString("mode", "replace")
        xw.WriteAttributeString("zone", wa)
        xw.WriteAttributeString("create", "true")
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplayRecording(recordingID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecording(ByVal recordingID As String) As String

        Dim markup As String = String.Empty

        Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(recordingID))
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel)

        Dim duration As System.TimeSpan = (recording.EndTime - recording.StartTime)
        Dim strDuration As String = ""

        If duration.Hours = 1 Then
            strDuration += String.Format("{0} {1} ", duration.Hours.ToString, GetGlobalResourceObject("uWiMPStrings", "hour"))
        Else
            strDuration += String.Format("{0} {1} ", duration.Hours.ToString, GetGlobalResourceObject("uWiMPStrings", "hours"))
        End If

        If duration.Minutes = 1 Then
            strDuration += String.Format("{0} {1}", duration.Minutes.ToString, GetGlobalResourceObject("uWiMPStrings", "min"))
        Else
            strDuration += String.Format("{0} {1}", duration.Minutes.ToString, GetGlobalResourceObject("uWiMPStrings", "mins"))
        End If

        Dim MP4path As String = uWiMP.TVServer.Utilities.GetAppConfig("STREAMPATH")
        Dim recFile As String = String.Format("{0}\{1}.mp4", MP4path, Path.GetFileNameWithoutExtension(recording.FileName))

        markup += "<div class=""iMenu"">"
        markup += "<div class=""iBlock"">"
        markup += String.Format("<h3>{0}</h3>", recording.Title)
        markup += String.Format("<h3>{0}</h3>", recording.StartTime)
        markup += String.Format("<h3>{0}</h3>", strDuration)
        markup += String.Format("<p>{0}</p>", recording.Description)
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        If User.IsInRole("watcher") Then
            If uWiMP.TVServer.Utilities.DoesFileExist(recFile) Then
                markup += String.Format("<li><a href=""../../MP4/{0}.mp4"">{1}</a></li>", Replace(Path.GetFileNameWithoutExtension(recording.FileName), " ", "%20"), GetGlobalResourceObject("uWiMPStrings", "watch"))
            Else
                markup += String.Format("<li><a href=""Recording/RecordingTranscode.aspx?recid={0}#_Transcode{0}"" rev=""async"">{1}</a></li>", recordingID, GetGlobalResourceObject("uWiMPStrings", "not_transcoded"))
                markup += String.Format("<li><a href=""Streaming/StartStream.aspx?type=rec&id={0}#_StartStream"" rev=""async"">{1}</a></li>", recording.IdRecording.ToString, GetGlobalResourceObject("uWiMPStrings", "stream"))
            End If
        End If
        If User.IsInRole("deleter") Then
            markup += String.Format("<li><a href=""Recording/RecordingDelete.aspx?id={0}#_DeleteRec{0}"" rev=""async"">{1}</a></li>", recording.IdRecording.ToString, GetGlobalResourceObject("uWiMPStrings", "delete"))
        End If
        If User.IsInRole("recorder") Then
            markup += String.Format("<li><a href=""Recording/RecordingKeep.aspx?id={0}#_KeepRec{0}"" rev=""async"">{1}</a></li>", recording.IdRecording.ToString, GetGlobalResourceObject("uWiMPStrings", "keep"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class