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
Imports System.Xml
Imports TvDatabase

Partial Public Class TranscodeStatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waTranscodeStatus"

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
        xw.WriteCData(TranscodeStatus())

        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function TranscodeStatus() As String

        Dim markup As String = String.Empty
        Dim markup2 As String = String.Empty
        Dim task As uWiMP.TVServer.Transcode = Nothing

        task = TryCast(Cache("transcoding"), uWiMP.TVServer.Transcode)

        If task Is Nothing Then
            task = New uWiMP.TVServer.Transcode()
            Cache("transcoding") = task
        End If

        If task.Running Then
            Dim startTime As DateTime = task.LastStartTime
            Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(task.RecordingID)
            markup2 += "Transcoding is running."
            markup2 += String.Format("<br>Currently transcoding {0}", recording.Title)
            markup2 += String.Format("<br>Shown on {0} at {1}", uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel).DisplayName, recording.StartTime)
            markup2 += String.Format("<br>Transcoding started at {0}", startTime.ToString)
            markup2 += String.Format("<br>Current time is ", Now.ToString)
        ElseIf uWiMP.TVServer.Transcode.IsTaskRunning Then
            markup2 += "Transcoding is running."
            markup2 += "<br>Unknown program (webserver reset?)"
            markup2 += String.Format("<br>Transcoding started at ", uWiMP.TVServer.Transcode.GetProgress)
            markup2 += String.Format("<br>Current time is ", Now.ToString)
        Else
            markup2 += "Transcoding is not running."
            If task.firstRunComplete Then
                markup2 += String.Format("<br>Last time it started at {0}<br>and finished at <br>", task.LastStartTime.ToString(), task.LastFinishTime.ToString())
                If task.LastTaskSuccess Then
                    markup2 += "This task succeeded."
                Else
                    markup2 += "This task failed."
                    If task.ExceptionOccured IsNot Nothing Then
                        markup2 += String.Format("<br>The exception was: ", task.ExceptionOccured.ToString())
                    End If
                End If
            End If
        End If

        markup += "<div class=""iMenu"">"
        markup += "<h3>Transcoding status</h3>"

        markup += "<div class=""iBlock"">"
        markup += String.Format("<p>{0}</p>", markup2)
        markup += "</div>"

        markup += "</div>"

        Return markup

    End Function

End Class