Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports System.IO
Imports System.Xml

Partial Public Class MyVideosListGenres
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMyVideosListGenres"
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
        xw.WriteCData(DisplayMyVideosGenres(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyVideosGenres(ByVal wa As String, ByVal friendly As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmoviefilter"
        mpRequest.Filter = "genre"

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)

        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim ja As JsonArray = CType(jo("genre"), JsonArray)
        Dim filters As String() = CType(ja.ToArray(GetType(String)), String())
        Array.Sort(filters)

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "videos_by_genre"))
        markup += "<ul class=""iArrow"">"

        For Each filter As String In filters
            If filter <> "" Then markup += String.Format("<li><a href=""MPClient/MyVideosList.aspx?friendly={0}&filter=genre&value={1}&start=0#_MPClientVideos"" rev=""async"">{1}</a></li>", friendly, filter)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class