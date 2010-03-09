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
Imports TvDatabase

Partial Public Class RecordingsByGenre
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waRecGenre"

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
        xw.WriteCData(DisplayRecordingsByGenre(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsByGenre(ByVal wa As String) As String

        Dim recordings As List(Of Recording) = uWiMP.TVServer.Recordings.GetAllRecordings
        If recordings.Count > 1 Then recordings.Sort(New uWiMP.TVServer.RecordingGenreComparer)

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "recordings"), GetGlobalResourceObject("uWiMPStrings", "genre"))
        markup += "<ul class=""iArrow"">"

        Dim recording As Recording
        Dim channel As Channel
        Dim lastGenre As String = String.Empty

        For Each recording In recordings
            channel = uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel)
            If recording.Genre.ToLower <> lastGenre.ToLower Then
                markup += String.Format("<li><a href=""Recording/RecordingsByGenreList.aspx?genre={0}#_RecGenre{0}"" rev=""async"">{0}</a></li>", recording.Genre)
                lastGenre = recording.Genre
            End If
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class