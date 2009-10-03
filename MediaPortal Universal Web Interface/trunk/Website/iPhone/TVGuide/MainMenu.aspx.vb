Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class TVGuideMainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waChannelGroups"

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
        xw.WriteCData(DisplayChannelGroupsMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayChannelGroupsMenu(ByVal wa As String) As String

        Dim groups As List(Of ChannelGroup) = uWiMP.TVServer.ChannelGroups.GetChannelGroups
        Dim group As ChannelGroup

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "channel_groups"))
        markup += "<ul class=""iArrow"">"

        For Each group In groups
            markup += String.Format("<li><a href=""TVGuide/ChannelGroups.aspx?group={0}#_ChannelGroup{0}"" rev=""async"">{1}</a></li>", group.IdGroup.ToString, group.GroupName)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class