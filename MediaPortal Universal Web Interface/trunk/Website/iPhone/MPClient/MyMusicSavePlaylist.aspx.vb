Imports System.IO
Imports System.Xml

Partial Public Class MyMusicSavePlaylist
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMyMusicSavePlaylist"
        Dim friendly As String = Request.QueryString("friendly")

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
        xw.WriteCData(DisplayMusicSearchMenu(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMusicSearchMenu(ByVal wa As String, ByVal friendly As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iPanel"" id=""{0}"">", wa)
        markup += "<fieldset>"
        markup += String.Format("<legend>{0} - {1}</legend>", friendly, GetGlobalResourceObject("uWiMPStrings", "save_playlist"))

        markup += "<ul>"
        markup += String.Format("<li><input type=""text"" id=""jsSaveFilename"" placeholder=""{0}""/></li>", GetGlobalResourceObject("uWiMPStrings", "enter_filename"))
        markup += "</ul>"
        markup += "</fieldset>"

        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""return saveplaylist('{0}');"" rel=""Action"" class=""iButton iBAction"">{1}</a>", friendly, GetGlobalResourceObject("uWiMPStrings", "submit"))
        markup += "</div>"

        Return markup

    End Function

End Class