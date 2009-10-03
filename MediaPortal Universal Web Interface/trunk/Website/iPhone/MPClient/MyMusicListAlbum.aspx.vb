Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Imports System.IO
Imports System.Xml

Partial Public Class MyMusicListAlbum
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMyMusicListAlbum"
        Dim friendly As String = Request.QueryString("friendly")
        Dim album As String = Server.UrlDecode(Request.QueryString("album"))
        Dim artist As String = Server.UrlDecode(Request.QueryString("artist"))

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        'go
        'xw.WriteStartElement("go")
        'xw.WriteAttributeString("to", wa)
        'xw.WriteEndElement()
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
        xw.WriteCData(DisplayAlbum(wa, friendly, album, artist))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayAlbum(ByVal wa As String, ByVal friendly As String, ByVal album As String, ByVal artist As String) As String

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, artist, album)

        markup += "<ul class=""iArrow"">"
        'markup += String.Format("<li><a href=""#"" onclick=""return playmusic('{0}','{1}','{2}');"">{3}</a></li>", friendly, Replace(album, "'", "\'"), Replace(artist, "'", "\'"), GetGlobalResourceObject("uWiMPStrings", "play"))
        'markup += String.Format("<li><a href=""#"" onclick=""return playtracks('{0}','{1}');"">{2}</a></li>", friendly, Replace(artist, "'", "\'"), GetGlobalResourceObject("uWiMPStrings", "play"))
        markup += String.Format("<li><a href=""#"" onclick=""return playtracks('{3}','{0}','{1}');"">{2}</a></li>", friendly, Replace(artist, "'", "\'"), GetGlobalResourceObject("uWiMPStrings", "play"), wa)
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsShuffleAlbum"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "shuffle"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsEnqueueAlbum"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "enqueue"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
        markup += "</ul>"

        markup += "<ul>"

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getalbum"
        mpRequest.Filter = album
        mpRequest.Value = artist
        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim tracks As uWiMP.TVServer.MPClient.AlbumTrack() = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.AlbumTrack()), response), uWiMP.TVServer.MPClient.AlbumTrack())

        Dim i As Integer = 0
        For Each track As uWiMP.TVServer.MPClient.AlbumTrack In tracks
            i += 1
            markup += String.Format("<li><label><div style=""display:inline-block; width:65%; overflow:hidden"">{0}</div></label><input type=""checkbox"" id=""MusicTrack{3}"" value=""{3}"" class=""iToggle"" title=""{1}"" /></li>", track.Title, GetGlobalResourceObject("uWiMPStrings", "yesno"), i.ToString, track.ID)
        Next

        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/MyMusicCoverArt.aspx?friendly={0}&artist={1}&album={2}#_MyMusicCoverArt"" rev=""async"">{3}</a></li>", friendly, artist, album, GetGlobalResourceObject("uWiMPStrings", "cover_art"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

End Class