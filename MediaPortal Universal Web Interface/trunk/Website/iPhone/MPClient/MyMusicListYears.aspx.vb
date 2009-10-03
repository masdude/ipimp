Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports System.IO
Imports System.Xml

Partial Public Class MyMusicListYears
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim decade As String = Request.QueryString("value")
        Dim wa As String = String.Format("waMyMusicListYears{0}", decade)

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
        xw.WriteCData(DisplayMyMusicDecades(wa, friendly, decade))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyMusicDecades(ByVal wa As String, ByVal friendly As String, ByVal decade As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmusicfilter"
        mpRequest.Filter = "year"
        mpRequest.Value = decade

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)

        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim ja As JsonArray = CType(jo("year"), JsonArray)
        Dim filters As String() = CType(ja.ToArray(GetType(String)), String())

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_by_year"))
        markup += "<ul class=""iArrow"">"

        markup += String.Format("<li><a href=""MPClient/MyMusicPlayRandom.aspx?friendly={0}&filter=decade&value={1}#_MPClientPlayRandom"" rev=""async"">{2}</a></li>", friendly, decade, GetGlobalResourceObject("uWiMPStrings", "play_100_random"))
        markup += "</ul>"
        markup += "<ul class=""iArrow"">"

        For Each filter As String In filters
            If filter <> "" Then markup += String.Format("<li><a href=""MPClient/MyMusicListAlbumsForYear.aspx?friendly={0}&value={1}&start=0#_MyMusicAlbumsForYear"" rev=""async"">{1}</a></li>", friendly, filter)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class