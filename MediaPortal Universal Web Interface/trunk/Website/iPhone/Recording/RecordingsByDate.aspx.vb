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

Partial Public Class RecordingsByDate
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waRecDate"

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
        xw.WriteCData(DisplayRecordingDatePeriods(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingDatePeriods(ByVal wa As String) As String

        Dim recordings As List(Of Recording) = uWiMP.TVServer.Recordings.GetAllRecordings
        If recordings.Count > 1 Then recordings.Sort(New uWiMP.TVServer.RecordingStartTimeComparerDesc)

        Dim recording As Recording
        Dim thisWeek As Boolean = False
        Dim lastWeek As Boolean = False
        Dim lastMonth As Boolean = False
        Dim lastYear As Boolean = False
        Dim older As Boolean = False
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "recordings"), GetGlobalResourceObject("uWiMPStrings", "date"))
        markup += "<ul class=""iArrow"">"

        For Each recording In recordings
            If (recording.StartTime > Now.AddDays(-7)) And Not thisWeek Then
                markup += String.Format("<li><a href=""Recording/RecordingsByDateList.aspx?period=ThisWeek#_RecDateThisWeek"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "this_week"))
                thisWeek = True
            ElseIf (recording.StartTime > Now.AddDays(-14)) And (recording.StartTime < Now.AddDays(-7)) And Not lastWeek Then
                markup += String.Format("<li><a href=""Recording/RecordingsByDateList.aspx?period=LastWeek#_RecDateLastWeek"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "last_week"))
                lastWeek = True
            ElseIf (recording.StartTime > Now.AddDays(-31)) And (recording.StartTime < Now.AddDays(-14)) And Not lastMonth Then
                markup += String.Format("<li><a href=""Recording/RecordingsByDateList.aspx?period=LastMonth#_RecDateLastMonth"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "last_month"))
                lastMonth = True
            ElseIf (recording.StartTime > Now.AddDays(-365)) And (recording.StartTime < Now.AddDays(-31)) And Not lastYear Then
                markup += String.Format("<li><a href=""Recording/RecordingsByDateList.aspx?period=LastYear#_RecDateLastYear"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "last_year"))
                lastYear = True
            ElseIf (recording.StartTime < Now.AddDays(-365)) And Not older Then
                markup += String.Format("<li><a href=""Recording/RecordingsByDateList.aspx?period=Older#_RecDateOlder"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "older"))
                older = True
            End If
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class