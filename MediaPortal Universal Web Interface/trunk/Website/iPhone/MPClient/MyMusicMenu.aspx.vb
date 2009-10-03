Imports System.IO
Imports System.Xml

Partial Public Class MyMusicMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMyMusic"
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
        xw.WriteCData(DisplayMyMusicMenu(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyMusicMenu(ByVal wa As String, ByVal friendly As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "my_music"))
        markup += "<ul class=""iArrow"">"

        markup += String.Format("<li><a href=""MPClient/MyMusicListGenres.aspx?friendly={0}#_MyMusicListGenres"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_by_genre"))
        markup += String.Format("<li><a href=""MPClient/MyMusicListArtistLetters.aspx?friendly={0}#_MyMusicListArtists"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_by_artist"))
        markup += String.Format("<li><a href=""MPClient/MyMusicListDecades.aspx?friendly={0}#_MyMusicListYears"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_by_year"))
        markup += String.Format("<li><a href=""MPClient/MyMusicListAlbumLetters.aspx?friendly={0}#_MyMusicListAlbums"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_by_album"))
        markup += String.Format("<li><a href=""MPClient/MyMusicListPlaylists.aspx?friendly={0}#_MyMusicListPlaylists"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_playlists"))
        markup += String.Format("<li><a href=""MPClient/MyMusicSearch.aspx?friendly={0}#_MyMusicSearch"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_search"))
        markup += String.Format("<li><a href=""MPClient/MyMusicUpdate.aspx?friendly={0}#_MyMusicUpdate"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_update"))

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class