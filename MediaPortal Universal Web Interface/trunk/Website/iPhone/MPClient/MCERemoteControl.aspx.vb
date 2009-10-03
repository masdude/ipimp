Imports System.IO
Imports System.Xml

Partial Public Class MCERemoteControl
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim friendly As String = Request.QueryString("friendly")

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        'part1 
        xw.WriteStartElement("part")

        'start title
        xw.WriteStartElement("title")
        xw.WriteAttributeString("set", "waRemote1")
        xw.WriteEndElement()
        'end title

        'start dest
        xw.WriteStartElement("destination")
        xw.WriteAttributeString("mode", "replace")
        xw.WriteAttributeString("zone", "waRemote1")
        xw.WriteAttributeString("create", "true")
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(RemoteMCE1(friendly))
        xw.WriteEndElement()
        'end data

        'end part
        xw.WriteEndElement()

        'part2 
        xw.WriteStartElement("part")

        'start title
        xw.WriteStartElement("title")
        xw.WriteAttributeString("set", "waRemote2")
        xw.WriteEndElement()
        'end title

        'start dest
        xw.WriteStartElement("destination")
        xw.WriteAttributeString("mode", "replace")
        xw.WriteAttributeString("zone", "waRemote2")
        xw.WriteAttributeString("create", "true")
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(RemoteMCE2(friendly))
        xw.WriteEndElement()
        'end data

        'end part
        xw.WriteEndElement()

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function RemoteMCE1(ByVal friendly As String) As String

        Dim imageURI As String = "../../images/remote/"
        Dim markup As String = String.Empty

        markup += "<div class=""board-div"" id=""waRemote1"">"
        markup += "<table>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "back")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "up")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "info")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "volup")
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "left")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "ok")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "right")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "voldown")
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "replay")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "down")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "skip")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "volmute")
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "stop")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "play")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "pause")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" ><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "page2")
        markup += "</tr>"
        markup += "</table>"
        markup += "<table>"
        markup += "<tr>"
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "red")
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "green")
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "yellow")
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote1"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "blue")
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        Return markup

    End Function

    Private Function RemoteMCE2(ByVal friendly As String) As String

        Dim imageURI As String = "../../images/remote/"
        Dim markup As String = String.Empty

        markup += "<div class=""board-div"" id=""waRemote2"">"
        markup += "<table>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "home")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "basichome")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "plugins")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "weather")
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "music")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "radio")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "nowplaying")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "playlists")
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "tv")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "tvguide")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "tvrecs")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "tvseries")
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "videos")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "dvd")
        markup += String.Format("<td class=""grid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "pictures")
        markup += String.Format("<td class=""grid""><a href=""#_Remote1"" ><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "page1")
        markup += "</tr>"
        markup += "</table>"
        markup += "<table>"
        markup += "<tr>"
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "red")
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "green")
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "yellow")
        markup += String.Format("<td class=""smallgrid""><a href=""#_Remote2"" onclick=""WA.Request('MPClient/MCERemoteControlButton.aspx?friendly={0}&button={2}#_MCEButton', null, -1, false, null);""><img src=""{1}{2}.png"" /></a></td>", friendly, imageURI, "blue")
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        Return markup

    End Function

End Class