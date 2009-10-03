﻿Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class SchedulesByDateList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim period As String = Request.QueryString("period")
        Dim wa As String = String.Format("waSchedDate{0}", period)

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

    Private Function DisplaySchedulesByDate(ByVal wa As String, ByVal period As String) As String

        Dim schedules As List(Of Schedule) = uWiMP.TVServer.Schedules.GetSchedules
        If schedules.Count > 1 Then schedules.Sort(New uWiMP.TVServer.ScheduleStartTimeComparerDesc)

        Dim schedule As Schedule
        Dim channel As Channel
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        Select Case period.ToLower
            Case "thisweek"
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_this_week"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In schedules
                    If (schedule.StartTime > Now) And (schedule.StartTime < Now.AddDays(7)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                    End If
                Next
                markup += "</ul>"

            Case "thismonth"
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_this_month"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In schedules
                    If (schedule.StartTime > Now.AddDays(7)) And (schedule.StartTime < Now.AddDays(31)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                    End If
                Next
                markup += "</ul>"

            Case "lastmonth"
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_last_month"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In schedules
                    If (schedule.StartTime < Now) And (schedule.StartTime > Now.AddDays(-31)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                    End If
                Next
                markup += "</ul>"

            Case "other"
                markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "scheduled_other"))
                markup += "<ul class=""iArrow"">"
                For Each schedule In schedules
                    If (schedule.StartTime < Now.AddDays(-31)) Or (schedule.StartTime > Now.AddDays(31)) Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
                        markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
                    End If
                Next
                markup += "</ul>"

        End Select

        markup += "</div>"

        Return markup

    End Function

End Class