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

Partial Public Class MyMusicPlayTracks
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim filter As String = Request.QueryString("filter")
        Dim shuffle As String = CBool(Request.QueryString("shuffle"))
        Dim enqueue As String = CBool(Request.QueryString("enqueue"))
        Dim tracks As String = Replace(Request.QueryString("tracks"), " ", ",")

        Dim wa As String = "waMPClientPlayTracks"

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
        xw.WriteCData(PlayMusic(wa, friendly, filter, shuffle, enqueue, tracks))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function PlayMusic(ByVal wa As String, _
                               ByVal friendly As String, _
                               ByVal filter As String, _
                               ByVal shuffle As Boolean, _
                               ByVal enqueue As Boolean, _
                               ByVal tracks As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "playtracks"
        mpRequest.Tracks = tracks
        mpRequest.Shuffle = shuffle
        mpRequest.Enqueue = enqueue

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", GetGlobalResourceObject("uWiMPStrings", "now_playing"), filter)
        markup += "<ul class=""iArrow"">"

        If success Then
            markup += String.Format("<li><a href=""MPClient/MPClientRemoteControl.aspx?friendly={0}#_MPClientRemote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
            markup += String.Format("<li><a href=""MPClient/MyMusicSavePlaylist.aspx?friendly={0}#_MyMusicSavePlaylist"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "save_playlist"))
        Else
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "could_not_start_music"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class