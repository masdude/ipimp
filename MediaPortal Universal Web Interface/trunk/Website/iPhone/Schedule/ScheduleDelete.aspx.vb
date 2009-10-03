﻿Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class ScheduleDelete
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim scheduleID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waDeleteSched{0}", scheduleID)

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
        xw.WriteCData(DeleteSchedule(wa, scheduleID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DeleteSchedule(ByVal wa As String, ByVal scheduleID As String) As String

        Dim markup As String = String.Empty
        Dim schedule As Schedule = uWiMP.TVServer.Schedules.GetScheduleById(CInt(scheduleID))

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", schedule.ProgramName)
        markup += String.Format("<h3>{0}</h3>", schedule.StartTime)
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "are_you_sure"))
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""Schedule/ScheduleDeleteResult.aspx?id={0}#_DeleteSchedResult{0}"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{1}</a>", scheduleID, GetGlobalResourceObject("uWiMPStrings", "yes"))
        markup += String.Format("<a href=""#"" onclick=""return WA.Back()"" rel=""Back"" class=""iButton iBClassic"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "no"))
        markup += "</div>"

        Return markup

    End Function

End Class