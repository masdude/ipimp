Imports System.IO
Imports System.Xml

Partial Public Class TVServerMainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waTVServer"

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
        xw.WriteCData(DisplayTVServerMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayTVServerMenu(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "tv_server_status"))
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""TVServer/CardStatus.aspx#_CardStatus"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "card_status"))
        markup += String.Format("<li><a href=""TVServer/ServiceStatus.aspx#_ServiceStatus"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "service_status"))
        markup += String.Format("<li><a href=""TVServer/DiskStatus.aspx#_DiskStatus"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "disk_status"))
        markup += String.Format("<li><a href=""TVServer/TranscodeStatus.aspx#_TranscodeStatus"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "transcode_status"))
        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class