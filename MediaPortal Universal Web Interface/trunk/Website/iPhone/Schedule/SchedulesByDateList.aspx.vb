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

Partial Public Class SchedulesByDateList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim period As Integer = CInt(Request.QueryString("period"))
        Dim wa As String = String.Format("waSchedDate{0}", period.ToString)

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
        xw.WriteCData(DisplaySchedulesByDate(wa, period))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySchedulesByDate(ByVal wa As String, ByVal period As Integer) As String

        Dim schedules As List(Of Schedule) = uWiMP.TVServer.Schedules.GetSchedules
        Dim periodScheds As New List(Of Schedule)
        For Each s As Schedule In schedules
            periodScheds.AddRange(uWiMP.TVServer.Schedules.GetRecordingTimes(s, CInt(period)))
        Next
        If periodScheds.Count > 1 Then periodScheds.Sort(New uWiMP.TVServer.ScheduleStartTimeComparer)

        Dim schedule As Schedule
        Dim channel As Channel
        Dim markup As String = String.Empty
        Dim iSchedules As Integer = 0
        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        Select Case period
            Case 7
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_this_week"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In periodScheds
                    If (schedule.StartTime > Now) And (schedule.StartTime < Now.AddDays(7)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                        iSchedules += 1
                    End If
                Next
                If iSchedules = 0 Then markup += String.Format("<li>{0}", GetGlobalResourceObject("uWiMPStrings", "scheduled_none"))
                markup += "</ul>"

            Case 14
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_next_week"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In periodScheds
                    If (schedule.StartTime > Now.AddDays(7)) And (schedule.StartTime < Now.AddDays(14)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                        iSchedules += 1
                    End If
                Next
                If iSchedules = 0 Then markup += String.Format("<li>{0}", GetGlobalResourceObject("uWiMPStrings", "scheduled_none"))
                markup += "</ul>"

            Case 31
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_rest_month"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In periodScheds
                    If (schedule.StartTime > Now.AddDays(14)) And (schedule.StartTime < Now.AddDays(31)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                        iSchedules += 1
                    End If
                Next
                If iSchedules = 0 Then markup += String.Format("<li>{0}", GetGlobalResourceObject("uWiMPStrings", "scheduled_none"))
                markup += "</ul>"

        End Select

        markup += "</div>"

        Return markup

    End Function

End Class