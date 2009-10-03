Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class SchedulesByChannelList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim id As String = Request.QueryString("id")
        Dim wa As String = String.Format("waSchedChannel{0}", id)

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
        xw.WriteCData(DisplaySchedulesByChannel(wa, id))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySchedulesByChannel(ByVal wa As String, ByVal id As String) As String

        Dim schedules As List(Of Schedule) = uWiMP.TVServer.Schedules.GetSchedulesForChannel(CInt(id))
        If schedules.Count > 1 Then schedules.Sort(New uWiMP.TVServer.ScheduleStartTimeComparerDesc)

        Dim schedule As Schedule
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(id)
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "schedules"), channel.DisplayName)
        markup += "<ul class=""iArrow"">"

        For Each schedule In schedules
            markup += String.Format("<li><a href=""Schedule/ScheduledProgram.aspx?id={0}#_SchedProgram{0}"" rev=""async""><img src=""http://{1}/TVLogos/{2}.png"" height=""40"" style=""vertical-align:middle""/><em>{3}<small><br/>{4}</small></em></a></li>", schedule.IdSchedule.ToString, Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString, schedule.ProgramName, schedule.StartTime)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class