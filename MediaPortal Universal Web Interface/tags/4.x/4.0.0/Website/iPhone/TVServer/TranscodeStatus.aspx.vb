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
        xw.WriteCData(DisplayServiceStatus(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayServiceStatus(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "transcode_status"))
        markup += "<ul class=""iArrow"">"

        Dim status As String = uWiMP.TVServer.Utilities.GetServiceStatus("TVService")
        If status = "" Then status = GetGlobalResourceObject("uWiMPStrings", "unknown")
        markup += String.Format("<li>{0}</li>", status)

        markup += "</ul>"
        markup += "</div>"

        If User.IsInRole("admin") Then
            markup += "<div>"
            Select Case status.ToLower
                Case "running"
                    markup += String.Format("<a href=""TVServer/ServiceActionConfirm.aspx?action=stop#_ServiceAction"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "stop"))
                Case "stopped"
                    markup += String.Format("<a href=""TVServer/ServiceActionConfirm.aspx?action=start#_ServiceAction"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "start"))
            End Select
            markup += "</div>"
        End If

        Return markup

    End Function

    Private Function TranscodeStatus() As String

        Dim markup As String = ""
        Dim embed As String = ""
        Dim task As uWiMP.TVServer.Transcode = Nothing

        task = TryCast(Cache("transcoding"), uWiMP.TVServer.Transcode)

        If task Is Nothing Then
            task = New uWiMP.TVServer.Transcode()
            Cache("transcoding") = task
        End If

        If task.Running Then
            Dim startTime As DateTime = task.LastStartTime
            Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(task.RecordingID)
            embed += "Transcoding is running."
            embed += "<br>Currently transcoding " & recording.Title
            embed += "<br>Shown on " & uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel).Name & " at " & recording.StartTime
            embed += "<br>Transcoding started at " & startTime.ToString
            embed += "<br>Current time is " & Now.ToString
        ElseIf uWiMP.TVServer.Transcode.IsFFMpegRunning Then
            embed += "Transcoding is running."
            embed += "<br>Unknown program (webserver reset?)"
            embed += "<br>Transcoding started at " & uWiMP.TVServer.Transcode.GetProgress
            embed += "<br>Current time is " & Now.ToString
        Else
            embed += "Transcoding is not running."
            If task.firstRunComplete Then
                embed += "<br>Last time it started at " & task.LastStartTime.ToString() & "<br>" & "and finished at " & task.LastFinishTime.ToString() & "<br>"
                If task.LastTaskSuccess Then
                    embed += "This task succeeded."
                Else
                    embed += "This task failed."
                    If task.ExceptionOccured IsNot Nothing Then
                        embed += "<br>The exception was: " & task.ExceptionOccured.ToString()
                    End If
                End If
            End If
        End If

        markup += "<div class=""iMenu"">"
        markup += "<h3>Transcoding status</h3>"

        markup += "<div class=""iBlock"">"
        markup += "<p>" & embed & "</p>"
        markup += "</div>"

        markup += "</div>"

        Return markup

    End Function

End Class