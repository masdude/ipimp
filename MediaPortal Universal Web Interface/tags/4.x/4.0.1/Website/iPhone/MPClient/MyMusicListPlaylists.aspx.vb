' 
'   Copyright (C) 2008-2009 Martin van der Boon
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

Partial Public Class MyMusicListPlaylists
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMyMusicListPlaylists"
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
        xw.WriteCData(DisplayMyMusicGenres(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyMusicGenres(ByVal wa As String, ByVal friendly As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request

        mpRequest.Action = "getrandomplaylist"
        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim ja As JsonArray = CType(jo("Playlists"), JsonArray)
        Dim playlists As String() = CType(ja.ToArray(GetType(String)), String())
        Array.Sort(playlists)

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_playlists"))

        For Each playlist As String In playlists
            If Not playlist.ToLower = "noplaylists" Then
                markup += "<ul class=""iArrow"">"
                markup += String.Format("<li><a href=""MPClient/MyMusicListTracksForPlaylist.aspx?friendly={0}&playlist={1}&start=0#_MyMusicListTracksForPlaylist"" rev=""async"">{2}</a></li>", friendly, playlist, GetGlobalResourceObject("uWiMPStrings", "play_random_playlist"))
                markup += "</ul>"
            End If
        Next

        mpRequest.Action = "getplaylists"
        response = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        jo = CType(JsonConvert.Import(response), JsonObject)
        ja = CType(jo("Playlists"), JsonArray)
        playlists = CType(ja.ToArray(GetType(String)), String())
        Array.Sort(playlists)

        markup += "<ul class=""iArrow"">"
        For Each playlist As String In playlists
            If playlist.ToLower = "noplaylists" Then
                markup += String.Format("<li style=""color:red"">{1}</li>", friendly, GetGlobalResourceObject("uWiMPStrings", "no_playlists_found"))
            Else
                markup += String.Format("<li><a href=""MPClient/MyMusicListTracksForPlaylist.aspx?friendly={0}&playlist={1}&start=0#_MyMusicListTracksForPlaylist"" rev=""async"">{1}</a></li>", friendly, playlist)
            End If
        Next
        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class