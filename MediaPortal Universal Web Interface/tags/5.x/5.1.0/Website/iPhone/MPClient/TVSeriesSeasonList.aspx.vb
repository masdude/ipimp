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

Partial Public Class TVSeriesSeasonList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim compositeID As String = Request.QueryString("compositeID")

        Dim wa As String = String.Format("waTVSeriesSeason{0}", compositeID)

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
        xw.WriteCData(DisplayTVSeasons(friendly, compositeID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayTVSeasons(ByVal friendly As String, ByVal compositeID As String)

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getseries"
        mpRequest.Value = Split(compositeID, "_")(0)

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))
        Dim name As String = CType(jo("name"), String)

        mpRequest.Action = "getepisodes"
        mpRequest.Value = compositeID
        response = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        jo = CType(JsonConvert.Import(response), JsonObject)
        success = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))
        Dim ja As JsonArray = CType(jo("episodes"), JsonArray)
        Dim episodeList As uWiMP.TVServer.MPClient.SmallEpisodeInfo() = DirectCast(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.SmallEpisodeInfo()), ja.ToString), uWiMP.TVServer.MPClient.SmallEpisodeInfo())

        markup += "<div class=""iMenu"" >"
        markup += String.Format("<h3>{0} - {1} ({2})</h3>", friendly, name, Split(compositeID, "_")(1))
        markup += "<ul class=""iArrow"">"

        For Each episode As uWiMP.TVServer.MPClient.SmallEpisodeInfo In episodeList
            markup += String.Format("<li><a href=""MPClient/TVSeriesEpisode.aspx?friendly={0}&compositeID={1}#_TVSeriesEpisode{1}"" rev=""async"">{2}. {3}</a></li>", friendly, episode.ID, episode.Index.ToString, episode.Name)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class