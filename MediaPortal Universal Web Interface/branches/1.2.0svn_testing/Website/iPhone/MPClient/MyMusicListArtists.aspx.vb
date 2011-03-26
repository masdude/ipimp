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

Partial Public Class MyMusicListArtists
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim letter As String = Request.QueryString("value")
        Dim start As Integer = CInt(Request.QueryString("start"))
        Dim pagesize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE")

        Dim wa As String = String.Format("waMyMusicListArtists{0}", letter)

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
            xw.WriteAttributeString("zone", "more")
        Else
            xw.WriteAttributeString("mode", "replace")
            xw.WriteAttributeString("zone", wa)
            xw.WriteAttributeString("create", "true")
        End If
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplayMyMusicArtists(friendly, letter, start, pagesize))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyMusicArtists(ByVal friendly As String, _
                                           ByVal letter As String, _
                                           ByVal start As Integer, _
                                           ByVal pagesize As Integer) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmusicfilter"
        mpRequest.Filter = "artist"
        mpRequest.Value = letter

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))

        Dim ja As JsonArray = CType(jo("artist"), JsonArray)
        Dim filters As String() = CType(ja.ToArray(GetType(String)), String())
        Array.Sort(filters)

        If start = 0 Then
            markup += "<div class=""iMenu"" >"
            markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "music_by_artist"))
            markup += "<ul class=""iArrow"">"
        End If

        For i As Integer = start To (start + (pagesize - 1))
            If i <= UBound(filters) Then
                markup += String.Format("<li><a href=""MPClient/MyMusicListAlbumsForArtist.aspx?friendly={0}&value={1}#_MyMusicListAlbumForArtist"" rev=""async"">{1}</a></li>", friendly, filters(i))
            End If
        Next

        If (start + (pagesize - 1)) < UBound(filters) Then
            markup += String.Format("<li id=""more"" class=""iMore""><a href=""MPClient/MyMusicListArtists.aspx?friendly={0}&value={1}&start={2}#_MyMusicAlbumsForArtist"" rev=""async"" title=""{3}"">{4}</a></li>", friendly, letter, (start + pagesize).ToString, GetGlobalResourceObject("uWiMPStrings", "loading"), GetGlobalResourceObject("uWiMPStrings", "more"))
        End If

        If start = 0 Then
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class