﻿' 
'   Copyright (C) 2008-2011 Martin van der Boon
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


Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Imports System.IO
Imports System.Xml

Partial Public Class MyMusicListAlbumsForArtist
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim artist As String = Request.QueryString("value")
        Dim start As Integer = CInt(Request.QueryString("start"))
        Dim pagesize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE")
        Dim wa As String = "waMyMusicListAlbumForArtist"

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        If start >= pagesize Then
        Else
            'go
            xw.WriteStartElement("go")
            xw.WriteAttributeString("to", wa)
            xw.WriteEndElement()
            'end go

            'title
            xw.WriteStartElement("title")
            xw.WriteAttributeString("set", wa)
            xw.WriteEndElement() 'title
            'end title
        End If

        xw.WriteStartElement("destination")

        'start destination
        If start >= pagesize Then
            xw.WriteAttributeString("mode", "self")
            xw.WriteAttributeString("zone", "morealbumsartist")
        Else
            xw.WriteAttributeString("mode", "replace")
            xw.WriteAttributeString("zone", wa)
            xw.WriteAttributeString("create", "true")
        End If
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplayAlbumsForArtists(friendly, artist, start, pagesize))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayAlbumsForArtists(ByVal friendly As String, _
                                       ByVal artist As String, _
                                       ByVal start As Integer, _
                                       ByVal pagesize As Integer) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getalbums"
        mpRequest.Filter = "artist"
        mpRequest.Value = artist

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))

        Dim ja As JsonArray = CType(jo("albums"), JsonArray)
        Dim albums As uWiMP.TVServer.MPClient.SmallAlbumInfo() = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.SmallAlbumInfo()), ja.ToString), uWiMP.TVServer.MPClient.SmallAlbumInfo())

        If start = 0 Then
            markup += "<div class=""iMenu"" >"
            markup += String.Format("<h3>{0} - {1}</h3>", friendly, artist)
            markup += "<ul class=""iArrow"">"
            markup += String.Format("<li><a href=""MPClient/MyMusicPlayRandom.aspx?friendly={0}&filter=artist&value={1}#_MPClientPlayRandom"" rev=""async"">{2}</a></li>", friendly, artist, GetGlobalResourceObject("uWiMPStrings", "play_100_random"))
            markup += String.Format("<li><a href=""MPClient/MyMusicListTracksForArtist.aspx?friendly={0}&artist={1}&start=0#_MyMusicListTracksForArtist"" rev=""async"">{2}</a></li>", friendly, artist, GetGlobalResourceObject("uWiMPStrings", "display_all_tracks"))
            markup += "</ul>"
            markup += "<ul class=""iArrow"">"
        End If

        For i As Integer = start To (start + (pagesize - 1))
            If i <= UBound(albums) Then
                markup += String.Format("<li><a href=""MPClient/MyMusicListAlbum.aspx?friendly={0}&album={3}&artist={4}#_MyMusicListAlbum"" rev=""async"">{1}</a></li>", friendly, albums(i).Album, albums(i).Artist, Server.UrlEncode(albums(i).Album), Server.UrlEncode(albums(i).Artist))
            End If
        Next

        If (start + (pagesize - 1)) < UBound(albums) Then
            markup += String.Format("<li id=""morealbumsartist"" class=""iMore""><a href=""MPClient/MyMusicListAlbumsForArtist.aspx?friendly={0}&value={1}&start={2}#_MyMusicListAlbumForArtist"" rev=""async"" title=""{3}"">{4}</a></li>", friendly, artist, (start + pagesize).ToString, GetGlobalResourceObject("uWiMPStrings", "loading"), GetGlobalResourceObject("uWiMPStrings", "more"))
        End If

        If start = 0 Then
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class