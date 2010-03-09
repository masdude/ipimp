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


Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Imports System.IO
Imports System.Xml

Partial Public Class MyMusicListTracksForSearch
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim search As String = Server.UrlDecode(Request.QueryString("search"))
        Dim start As Integer = CInt(Request.QueryString("start"))
        Dim pagesize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE")
        Dim wa As String = "waMyMusicSearchResults"

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

            'start title
            xw.WriteStartElement("title")
            xw.WriteAttributeString("set", wa)
            xw.WriteEndElement()
            'end title
        End If

        'start dest
        xw.WriteStartElement("destination")
        If start >= pagesize Then
            xw.WriteAttributeString("mode", "self")
            xw.WriteAttributeString("zone", "moresearch")
        Else
            xw.WriteAttributeString("mode", "replace")
            xw.WriteAttributeString("zone", wa)
            xw.WriteAttributeString("create", "true")
        End If
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplaySongs(wa, friendly, search, start, pagesize))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySongs(ByVal wa As String, _
                                   ByVal friendly As String, _
                                   ByVal search As String, _
                                   ByVal start As Integer, _
                                   ByVal pagesize As Integer) As String

        Dim markup As String = String.Empty

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "gettracks"
        mpRequest.Filter = "search"
        mpRequest.Value = search
        mpRequest.Start = start
        mpRequest.PageSize = pagesize

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim tracks As uWiMP.TVServer.MPClient.AlbumTrack() = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.AlbumTrack()), response), uWiMP.TVServer.MPClient.AlbumTrack())

        If start = 0 Then
            markup += "<div class=""iMenu"">"
            markup += String.Format("<h3>{0}</h3>", friendly)

            markup += "<ul class=""iArrow"">"
            markup += String.Format("<li><a href=""#"" onclick=""return playtracks('{3}','{0}','{1}');"">{2}</a></li>", friendly, "", GetGlobalResourceObject("uWiMPStrings", "play"), wa)
            markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsShuffleSearch"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "shuffle"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
            markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsEnqueueSearch"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "enqueue"), GetGlobalResourceObject("uWiMPStrings", "yesno"))
            markup += "</ul>"

            markup += "<ul>"
        End If

        Dim i As Integer = 0
        For Each track As uWiMP.TVServer.MPClient.AlbumTrack In tracks
            i += 1
            markup += String.Format("<li><div style=""display:inline-block; width:65%; overflow:hidden""><label>{0}<br/><small style=""display:block;font-size:14px;color:#7f7f7f;line-height:18px;text-shadow:none;font-weight:normal;"">{4}</small></div></label><input type=""checkbox"" id=""MusicTrack{3}"" value=""{3}"" class=""iToggle"" title=""{1}"" /></li>", track.Title, GetGlobalResourceObject("uWiMPStrings", "yesno"), (start + i).ToString, track.ID, track.Artist)
        Next

        mpRequest.Start = start + pagesize
        response = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        tracks = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.AlbumTrack()), response), uWiMP.TVServer.MPClient.AlbumTrack())
        If UBound(tracks) > 0 Then markup += String.Format("<li id=""moresearch"" class=""iMore""><a href=""MPClient/MyMusicListTracksForSearch.aspx?friendly={0}&search={1}&start={2}#_MyMusicSearchResults"" rev=""async"" title=""{3}"">{4}</a></li>", friendly, search, (start + pagesize).ToString, GetGlobalResourceObject("uWiMPStrings", "loading"), GetGlobalResourceObject("uWiMPStrings", "more"))

        If start = 0 Then
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class