' 
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

Partial Public Class MyMusicPlayRandom
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim filter As String = Request.QueryString("filter")
        Dim value As String = Request.QueryString("value")
        Dim wa As String = "waMPClientPlayRandom"

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
        xw.WriteCData(PlayRandom(friendly, filter, value))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function PlayRandom(ByVal friendly As String, ByVal filter As String, ByVal value As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "playrandommusic"
        mpRequest.Filter = filter
        mpRequest.Value = value

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)

        markup += "<div class=""iMenu"" >"
        markup += String.Format("<h3>{0} - {1}</h3>", GetGlobalResourceObject("uWiMPStrings", "now_playing"), filter)
        markup += "<ul class=""iArrow"">"

        If success Then
            markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
            markup += String.Format("<li><a href=""MPClient/MyMusicSavePlaylist.aspx?friendly={0}#_MyMusicSavePlaylist"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "save_playlist"))
        Else
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "could_not_start_music"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class