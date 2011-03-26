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

Partial Public Class RecordingsByChannelList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim channelID As String = Request.QueryString("channel")
        Dim wa As String = String.Format("waRecChannel{0}", channelID)

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
        xw.WriteCData(DisplayRecordingsByChannel(channelID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsByChannel(ByVal channelID As String) As String

        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(CInt(channelID))

        Dim markup As String = String.Empty
        markup += "<div class=""iMenu"" >"
        markup += String.Format("<h3>{0} {1}</h3>", GetGlobalResourceObject("uWiMPStrings", "recordings"), channel.DisplayName)
        markup += "<ul class=""iArrow"">"

        Dim recordings As List(Of Recording) = uWiMP.TVServer.Recordings.GetRecordingsForChannel(CInt(channelID))
        If recordings.Count > 1 Then recordings.Sort(New uWiMP.TVServer.RecordingTitleComparer)
        Dim recording As Recording

        For Each recording In recordings
            markup += String.Format("<li><a href=""Recording/RecordedProgram.aspx?id={0}#_RecProgram{0}"" rev=""async""><img src=""../../TVLogos/{1}.png"" height=""40"" style=""vertical-align:middle""/><em>{2}<small><br/>{3}</small></em></a></li>", recording.IdRecording.ToString, uWiMP.TVServer.Utilities.GetMPSafeFilename(channel.DisplayName), recording.Title, recording.StartTime)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class