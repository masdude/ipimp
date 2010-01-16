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

Partial Public Class RecordManualConfirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waRecordManualConfirm"
        Dim channelID As Integer = CInt(Request.QueryString("channel"))
        Dim schedName As String = Request.QueryString("schedName")
        Dim startDate As String = Request.QueryString("startdate")
        Dim startTime As String = Request.QueryString("starttime")
        Dim duration As String = Request.QueryString("duration")

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
        xw.WriteCData(RecordProgram(channelID, schedName, startDate, startTime, duration))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function RecordProgram(ByVal channelID As Integer, ByVal schedName As String, ByVal startDate As String, ByVal startTime As String, ByVal duration As String) As String

        Dim success As Boolean = True
        Dim errorText As String = String.Empty

        If startTime.Length <> "5" Then errorText += "Start time must be in the format hh:mm.</br>"
        If schedName.Length = 0 Then schedName = GetGlobalResourceObject("uWiMPStrings", "manual")
        If schedName.Length > 30 Then schedName = Left(schedName, 30)
        If CInt(duration) > 600 Then errorText += "Duration must be less than 600 mins.</br>"

        Dim addDays As String = String.Empty

        Select Case startDate.ToLower
            Case GetGlobalResourceObject("uWiMPStrings", "today").ToString.ToLower
                addDays = 0
            Case GetGlobalResourceObject("uWiMPStrings", "tomorrow").ToString.ToLower
                addDays = 1
            Case Now.AddDays(2).DayOfWeek
                addDays = 2
            Case Now.AddDays(3).DayOfWeek
                addDays = 3
            Case Now.AddDays(4).DayOfWeek
                addDays = 4
            Case Now.AddDays(5).DayOfWeek
                addDays = 5
            Case Now.AddDays(6).DayOfWeek
                addDays = 6
            Case Else
                addDays = 0
                success = False
        End Select

        Dim iYear As Integer = Year(Now.AddDays(addDays))
        Dim iMonth As Integer = Month(Now.AddDays(addDays))
        Dim iDay As Integer = Day(Now.AddDays(addDays))

        Dim iHour As Integer
        Dim iMinute As Integer
        Dim iSecond As Integer
        Try
            iHour = CInt(Left(startTime, 2))
            iMinute = CInt(Right(startTime, 2))
            iSecond = 0
        Catch ex As Exception
            errorText = String.Format("Could not convert start time {0}.</br>", startTime)
            success = False
        End Try

        Dim sDateTime, eDateTime As DateTime
        If success Then
            sDateTime = New DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond)
            eDateTime = sDateTime.AddMinutes(duration)
        End If

        Dim markup As String = String.Empty
        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "manual_record"))

        If success Then
            markup += "<ul class=""iArrow"">"
            If uWiMP.TVServer.Recordings.RecordProgram(channelID, schedName, sDateTime, eDateTime) = True Then
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "recording_add_success"))
            Else
                markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "recording_add_fail"))
            End If
            markup += "</ul>"
        Else
            markup += "<div class=""iBlock"">"
            markup += String.Format("<p>{0}</p>", errorText)
            markup += "</div>"
        End If

        markup += "</div>"

        Return markup

    End Function

End Class