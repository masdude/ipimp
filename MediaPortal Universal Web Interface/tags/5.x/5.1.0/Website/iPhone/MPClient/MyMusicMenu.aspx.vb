' 
'   Copyright (C) 2008-2010 Martin van der Boon
' 
'  This program is free software: you can redistribute it and/or modify 
'  it under the terms of the GNU General Public License as published by 
'  the Free Software Foundation, either version 3 of the License, or 
'  (at your option) any later version. 
' 
'   This program is distributed in the hope that it will be useful, 
'   but WITHOUT ANY WARRANTY; without even the implied warranty of 
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
'   GNU General Public License for more details. 
' 
'   You should have received a copy of the GNU General Public License 
'   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
' 


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
        xw.WriteCData(DisplayMyMusicMenu(friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyMusicMenu(ByVal friendly As String) As String

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"" >"
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