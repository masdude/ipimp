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

Partial Public Class ScheduledProgram
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim scheduleID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waSchedProgram{0}", scheduleID)

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
        xw.WriteCData(DisplaySchedule(wa, scheduleID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySchedule(ByVal wa As String, ByVal scheduleID As String) As String

        Dim schedule As Schedule = uWiMP.TVServer.Schedules.GetScheduleById(CInt(scheduleID))
        Dim programs As List(Of Program)
        Dim program As Program

        If schedule.ScheduleType = TvDatabase.ScheduleRecordingType.Once Then
            program = uWiMP.TVServer.Programs.GetProgramByProgramNameDate(schedule.ProgramName, schedule.StartTime, schedule.EndTime)
        Else
            program = Nothing
            programs = uWiMP.TVServer.Programs.GetProgramByProgramName(schedule.ProgramName)
            For Each program In programs
                If program.StartTime > Now Then Exit For
            Next
        End If

        Dim description As String = String.Empty
        Dim title As String = String.Empty
        Dim starttime As Date = Now

        If program Is Nothing Then
            title = schedule.ProgramName
            starttime = schedule.StartTime
            description = GetGlobalResourceObject("uWiMPStrings", "program_not_found")
        ElseIf program.Description = "" Then
            description = GetGlobalResourceObject("uWiMPStrings", "description_not_found")
        Else
            title = program.Title
            starttime = program.StartTime
            description = program.Description
        End If

        Dim markup As String = String.Empty
        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += "<div class=""iBlock"">"
        markup += String.Format("<h3>{0}</h3>", title)
        markup += String.Format("<h3>{0}</h3>", starttime)
        markup += String.Format("<p>{0}</p>", description)
        markup += "</div>"

        markup += "<ul class=""iArrow"">"

        If User.IsInRole("deleter") Then
            markup += String.Format("<li><a href=""Schedule/ScheduleDelete.aspx?id={0}#_DeleteSched{0}"" rev=""async"">{1}</a></li>", schedule.IdSchedule.ToString, GetGlobalResourceObject("uWiMPStrings", "delete"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class