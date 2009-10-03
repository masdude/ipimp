Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class TVChannel
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim channelID As String = Request.QueryString("channel")
        Dim wa As String = "waChannel" & channelID

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
        xw.WriteCData(DisplayChannelMenu(wa, channelID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayChannelMenu(ByVal wa As String, ByVal channelID As String) As String

        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(CInt(channelID))
        Dim markup As String = String.Empty
        Dim scheduled As String = String.Empty

        markup += "<div class=""iMenu"" id=""" & wa & """>"
        markup += String.Format("<h3>{0}</h3>", channel.Name)

        markup += "<ul class=""iArrow"">"
        If Not channel.CurrentProgram Is Nothing Then
            If uWiMP.TVServer.Schedules.IsProgramScheduled(channel.CurrentProgram) Then
                scheduled = "style=""color: red;"""
            Else
                scheduled = ""
            End If
            markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async"">{1}<small><br/>{2}</small></a></li>", channel.CurrentProgram.IdProgram.ToString, channel.CurrentProgram.Title, channel.CurrentProgram.StartTime, scheduled)
        End If
        If Not channel.NextProgram Is Nothing Then
            If uWiMP.TVServer.Schedules.IsProgramScheduled(channel.NextProgram) Then
                scheduled = "style=""color: red;"""
            Else
                scheduled = ""
            End If
            markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async"">{1}<small><br/>{2}</small></a></li>", channel.NextProgram.IdProgram.ToString, channel.NextProgram.Title, channel.NextProgram.StartTime, scheduled)
        End If
        markup += "</ul>"
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day=0#_Channel{0}Day0"" rev=""async"">{1}</a></li>", channelID, GetGlobalResourceObject("uWiMPStrings", "today"))
        markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day=1#_Channel{0}Day1"" rev=""async"">{1}</a></li>", channelID, GetGlobalResourceObject("uWiMPStrings", "tomorrow"))
        For i As Integer = 2 To 6
            markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day={1}#_Channel{0}Day{1}"" rev=""async"">{2}</a></li>", channelID, i.ToString, Now.AddDays(i).ToString("dddd"))
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class