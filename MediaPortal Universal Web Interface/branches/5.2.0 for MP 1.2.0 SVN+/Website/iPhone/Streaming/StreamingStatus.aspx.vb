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


Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class StreamingStatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waStreamingStatus"

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
        xw.WriteCData(StreamingStatus())

        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function StreamingStatus() As String

        Dim markup As String = String.Empty
        Dim imageURI As String = "../../images/remote/"

        markup += String.Format("<div class=""iMenu"">")
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "streaming_status"))
        markup += "<ul>"

        Dim mediaType As uWiMP.TVServer.Streamer.MediaType
        Dim mediaID As String = String.Empty
        Dim cardID As String = String.Empty
        Dim userName As String = String.Empty

        Dim basedir As String = String.Format("{0}\SmoothStream.isml", AppDomain.CurrentDomain.BaseDirectory)
        Dim path As String = String.Format("{0}\Stream.txt", basedir)
        If File.Exists(path) And Directory.GetFiles(basedir).GetLength(0) > 0 Then
            Using sr As StreamReader = File.OpenText(path)
                mediaType = sr.ReadLine
                mediaID = sr.ReadLine
                cardID = sr.ReadLine
                userName = sr.ReadLine
            End Using
            Dim text As String = String.Empty
            Select Case mediaType
                Case uWiMP.TVServer.Streamer.MediaType.Tv, uWiMP.TVServer.Streamer.MediaType.Radio
                    text = uWiMP.TVServer.Channels.GetChannelNameByChannelId(CInt(mediaID))
                Case uWiMP.TVServer.Streamer.MediaType.Recording
                    text = uWiMP.TVServer.Recordings.GetRecordingById(CInt(mediaID)).Title
                Case uWiMP.TVServer.Streamer.MediaType.TvSeries
                    text = GetGlobalResourceObject("uWiMPStrings", "tv_series")
                Case Else
                    text = GetGlobalResourceObject("uWiMPStrings", "unknown")
            End Select
            markup += String.Format("<li>{0} {1}</li>", GetGlobalResourceObject("uWiMPStrings", "streaming"), text)
            markup += "</ul>"

            markup += "<div class=""iBlock"">"
            markup += String.Format("<div><p>{0}</p>", GetGlobalResourceObject("uWiMPStrings", "stream_started"))
            markup += "<table class=""center""><tr>"
            markup += String.Format("<td class=""grid""><a href=""../../SmoothStream.isml/SmoothStream.m3u8"" target=""_blank""><img src=""{0}{1}.png"" /></a></td>", imageURI, "play")
            markup += String.Format("<td class=""grid""><a href=""Streaming/StopStream.aspx#_StopStream"" rev=""async""><img src=""{0}{1}.png"" /></a></td>", imageURI, "stop")
            'markup += String.Format("<td class=""grid""><a href=""#_StopStream"" onclick=""WA.Request('Streaming/StopStream.aspx#_StopStream', null, -1);"" ><img src=""{0}{1}.png"" /></a></td>", imageURI, "stop")
            markup += "</tr>"
            markup += "<tr>"
            markup += String.Format("<td class=""grid""><a href=""../../Desktop/Players/Silverlight/Silverlight.htm"" target=""_blank""><img src=""{0}{1}.png"" /></a></td>", imageURI, "slight")
            markup += String.Format("<td class=""grid""><a href=""../../Desktop/Players/Flash/player.htm"" target=""_blank""><img src=""{0}{1}.png"" /></a></td>", imageURI, "flash")
            markup += "</tr></table>"
            markup += "</div>"
            markup += "</div>"
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "streaming_nothing"))
            markup += "</ul>"
        End If

        markup += "</div>"

        Return markup

    End Function

End Class