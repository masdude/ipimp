Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingMainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waRecordings"

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
        xw.WriteCData(DisplayRecordingsMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsMenu(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "recordings"))
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""Recording/RecordingsByDate.aspx?start=0#_RecDate"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "date"))
        markup += String.Format("<li><a href=""Recording/RecordingsByChannel.aspx?start=0#_RecChannel"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "channel"))
        markup += String.Format("<li><a href=""Recording/RecordingsByGenre.aspx?start=0#_RecGenre"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "genre"))
        markup += "</ul>"

        If User.IsInRole("deleter") Then
            markup += "<ul class=""iArrow"">"
            markup += String.Format("<li><a href=""Recording/RecordingsMultiDelete.aspx?start=0#_RecDelete"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "delete"))
            markup += "</ul>"
        End If

        markup += "</div>"

        Return markup

    End Function

End Class