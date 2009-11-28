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


Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingTranscodeResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim recordingID As String = Request.QueryString("recid")
        Dim wa As String = String.Format("waTranscodeResult{0}", recordingID)

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
        xw.WriteCData(TranscodeRecording(recordingID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function TranscodeRecording(ByVal recordingID As String) As String

        Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(recordingID))
        Dim markup As String = String.Empty
        Dim task As uWiMP.TVServer.Transcode = Nothing

        Dim MP4path As String = uWiMP.TVServer.Utilities.GetAppConfig("MP4PATH")
        Dim recfile As String = Path.GetFileNameWithoutExtension(recording.FileName) & ".mp4"

        task = TryCast(Cache("transcoding"), uWiMP.TVServer.Transcode)

        If task Is Nothing Then
            task = New uWiMP.TVServer.Transcode
            Cache("transcoding") = task
        End If

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0}</h3>", recording.Title)
        markup += String.Format("<h3>{0}</h3>", recording.StartTime)
        markup += "<ul class=""iArrow"">"

        If task.Running Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "transcoding_running"))
        Else
            task.RecordingID = recordingID
            task.RunTask()
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "transcoding_started"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class