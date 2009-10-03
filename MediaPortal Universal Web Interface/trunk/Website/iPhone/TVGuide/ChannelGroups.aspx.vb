Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class ChannelGroups
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim groupID As String = Request.QueryString("group")
        Dim wa As String = "waChannelGroup" & groupID

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
        xw.WriteCData(DisplayChannelGroupMenu(wa, groupID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayChannelGroupMenu(ByVal wa As String, ByVal groupID As String) As String

        Dim group As ChannelGroup = uWiMP.TVServer.ChannelGroups.GetChannelGroupByGroupId(CInt(groupID))
        Dim channels As List(Of Channel) = uWiMP.TVServer.Channels.GetChannelsByGroupId(CInt(groupID))
        Dim channel As Channel

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"" id=""" & wa & """>"

        markup += "<h3>" & group.GroupName & "</h3>"
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""TVGuide/SearchTVGuide.aspx?group={0}#_TVGuideSearch"" rev=""async"">{1}</a></li>", groupID, GetGlobalResourceObject("uWiMPStrings", "search"))
        markup += String.Format("<li><a href=""TVGuide/NowAndNext.aspx?group={0}#_NowAndNext{0}"" rev=""async"">{1}</a></li>", groupID, GetGlobalResourceObject("uWiMPStrings", "now_and_next"))
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        For Each channel In channels
            markup += String.Format("<li><a href=""TVGuide/TVChannel.aspx?channel={0}#_Channel{0}"" rev=""async""><img src=""http://" & Request.ServerVariables("HTTP_HOST") & "/TVLogos/{1}.png"" height=""28""/><em>{1}</em></a></li>", channel.IdChannel.ToString, channel.DisplayName)
        Next
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

End Class