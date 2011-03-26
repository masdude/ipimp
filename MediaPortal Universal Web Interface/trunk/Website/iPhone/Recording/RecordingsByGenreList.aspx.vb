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

Partial Public Class RecordingsByGenreList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim genre As String = Request.QueryString("genre")
        Dim wa As String = String.Format("waRecGenre{0}", genre)

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
        xw.WriteCData(DisplayRecordingsByGenre(wa, genre))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsByGenre(ByVal wa As String, ByVal genre As String) As String

        Dim markup As String = String.Empty
        markup += String.Format("<div class=""iList"" id=""{0}"">", wa)
        markup += String.Format("<h2>{0} {1}</h2>", GetGlobalResourceObject("uWiMPStrings", "recordings"), genre)
        markup += "<ul class=""iArrow iShop"">"

        Dim recordings As List(Of Recording) = uWiMP.TVServer.Recordings.GetRecordingsForGenre(genre)
        If recordings.Count > 1 Then recordings.Sort(New uWiMP.TVServer.RecordingGenreComparer)
        Dim recording As Recording
        Dim image, imageName As String
        Dim MP4path As String = uWiMP.TVServer.Utilities.GetAppConfig("STREAMPATH")
        Dim channel As Channel

        For Each recording In recordings
            channel = uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel)
            imageName = MP4path & "\" & Path.GetFileNameWithoutExtension(recording.FileName) & ".jpg"
            If uWiMP.TVServer.Utilities.DoesFileExist(imageName) Then
                image = String.Format("../../MP4/{0}.jpg", Path.GetFileNameWithoutExtension(recording.FileName))
            Else
                image = String.Format("../../TVLogos/{0}.png", uWiMP.TVServer.Utilities.GetMPSafeFilename(channel.DisplayName))
            End If
            markup += String.Format("<li><a href=""Recording/RecordedProgram.aspx?id={0}#_RecProgram{0}"" rev=""async""><img src=""{1}"" class=""iFull"" /><em>{2}</em><big>{3}<small>{4}</small></big></a></li>", recording.IdRecording.ToString, image, channel.DisplayName, recording.Title, recording.StartTime)
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class