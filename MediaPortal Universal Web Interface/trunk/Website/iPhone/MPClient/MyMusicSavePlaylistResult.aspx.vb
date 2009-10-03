﻿Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports System.IO
Imports System.Xml

Partial Public Class MyMusicSavePlaylistResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMyMusicSavePlaylistResult"
        Dim friendly As String = Request.QueryString("friendly")
        Dim filename As String = Request.QueryString("filename")

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
        xw.WriteCData(DisplayMusicSearchMenu(wa, friendly, filename))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMusicSearchMenu(ByVal wa As String, ByVal friendly As String, ByVal filename As String) As String

        Dim markup As String = String.Empty
        Dim regexPattern = "[\\\/.:\*\?""'<>|] "
        Dim objRegEx As New Regex(regexPattern)
        filename = objRegEx.Replace(filename, "")

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "saveplaylist"
        mpRequest.Filter = filename

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "save_playlist"))

        markup += "<ul>"
        If success Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "save_playlist_success"))
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "save_playlist_fail"))
        End If
        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class