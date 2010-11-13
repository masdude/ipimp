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
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingsByDateList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim period As String = Request.QueryString("period")
        Dim wa As String = String.Format("waRecDate{0}", period)

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
        xw.WriteCData(DisplayRecordingsByDate(wa, period))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsByDate(ByVal wa As String, ByVal period As String) As String

        Dim recordings As List(Of Recording) = uWiMP.TVServer.Recordings.GetAllRecordings
        If recordings.Count > 1 Then recordings.Sort(New uWiMP.TVServer.RecordingStartTimeComparerDesc)
        Dim recording As Recording

        Dim markup As String = String.Empty
        markup += String.Format("<div class=""iList"" id=""{0}"">", wa)

        Select Case period.ToLower
            Case "thisweek"
                markup += String.Format("<h2>{0}</h2>", GetGlobalResourceObject("uWiMPStrings", "recorded_this_week"))
                markup += "<ul class=""iArrow iShop"">"
                For Each recording In recordings
                    If recording.StartTime > Now.AddDays(-7) Then
                        markup += RecordingMarkup(recording)
                    End If
                Next
                markup += "</ul>"

            Case "lastweek"
                markup += String.Format("<h2>{0}</h2>", GetGlobalResourceObject("uWiMPStrings", "recorded_last_week"))
                markup += "<ul class=""iArrow iShop"">"
                For Each recording In recordings
                    If (recording.StartTime > Now.AddDays(-14)) And (recording.StartTime < Now.AddDays(-7)) Then
                        markup += RecordingMarkup(recording)
                    End If
                Next
                markup += "</ul>"
            Case "lastmonth"
                markup += String.Format("<h2>{0}</h2>", GetGlobalResourceObject("uWiMPStrings", "recorded_last_month"))
                markup += "<ul class=""iArrow iShop"">"
                For Each recording In recordings
                    If (recording.StartTime > Now.AddDays(-31)) And (recording.StartTime < Now.AddDays(-14)) Then
                        markup += RecordingMarkup(recording)
                    End If
                Next
                markup += "</ul>"
            Case "lastyear"
                markup += String.Format("<h2>{0}</h2>", GetGlobalResourceObject("uWiMPStrings", "recorded_last_year"))
                markup += "<ul class=""iArrow iShop"">"
                For Each recording In recordings
                    If (recording.StartTime > Now.AddDays(-365)) And (recording.StartTime < Now.AddDays(-31)) Then
                        markup += RecordingMarkup(recording)
                    End If
                Next
                markup += "</ul>"
            Case "older"
                markup += String.Format("<h2>{0}</h2>", GetGlobalResourceObject("uWiMPStrings", "recorded_older"))
                markup += "<ul class=""iArrow iShop"">"
                For Each recording In recordings
                    If recording.StartTime < Now.AddDays(-365) Then
                        markup += RecordingMarkup(recording)
                    End If
                Next
                markup += "</ul>"
        End Select

        markup += "</div>"

        Return markup

    End Function

    Private Function RecordingMarkup(ByVal recording As Recording) As String

        Dim image, imageName As String
        Dim MP4path As String = uWiMP.TVServer.Utilities.GetAppConfig("STREAMPATH")
        Dim markup As String = String.Empty
        Dim channel As Channel
        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel)

        imageName = MP4path & "\" & Path.GetFileNameWithoutExtension(recording.FileName) & ".jpg"
        If uWiMP.TVServer.Utilities.DoesFileExist(imageName) Then
            image = String.Format("../../MP4/{0}.jpg", Path.GetFileNameWithoutExtension(recording.FileName))
        Else
            image = String.Format("../../TVLogos/{0}.png", channel.DisplayName)
        End If
        markup += String.Format("<li><a href=""Recording/RecordedProgram.aspx?id={0}#_RecProgram{0}"" rev=""async""><img src=""{1}"" class=""iFull"" /><em>{2}</em><big>{3}<small>{4}</small></big></a></li>", recording.IdRecording.ToString, Image, Channel.DisplayName, recording.Title, recording.StartTime)

        Return markup

    End Function
End Class