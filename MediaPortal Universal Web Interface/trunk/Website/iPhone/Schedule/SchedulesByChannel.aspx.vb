Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class SchedulesByChannel
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waSchedChannel"

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
        xw.WriteCData(DisplaySchedulesByChannel(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySchedulesByChannel(ByVal wa As String) As String

        Dim schedules As List(Of Schedule) = uWiMP.TVServer.Schedules.GetSchedules
        If schedules.Count > 1 Then schedules.Sort(New uWiMP.TVServer.ScheduleChannelComparer)

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "schedules"), GetGlobalResourceObject("uWiMPStrings", "channel"))
        markup += "<ul class=""iArrow"">"

        Dim schedule As Schedule
        Dim channel As Channel
        Dim lastChannel As Integer
        Dim channels As New List(Of Channel)
        For Each schedule In schedules
            channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
            If schedule.IdChannel <> lastChannel Then
                channels.Add(channel)
                lastChannel = schedule.IdChannel
            End If
        Next

        If channels.Count > 1 Then channels.Sort(New uWiMP.TVServer.ChannelSortOrderComparer)
        For Each channel In channels
            markup += String.Format("<li><a href=""Schedule/SchedulesByChannelList.aspx?id={0}#_SchedChannel{0}"" rev=""async""><img src=""http://" & Request.ServerVariables("HTTP_HOST") & "/TVLogos/{1}.png"" style=""height:26px""/>{1}</a></li>", channel.IdChannel.ToString, channel.DisplayName.ToString)
        Next

               markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class