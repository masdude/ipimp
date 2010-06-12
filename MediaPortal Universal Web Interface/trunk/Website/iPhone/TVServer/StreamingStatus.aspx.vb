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
        markup += String.Format("<h3>{0}</h3>", "Streaming status") 'GetGlobalResourceObject("uWiMPStrings", "watch"))
        markup += "<ul>"

        Dim type As String = String.Empty
        Dim id As Integer = -1

        Dim path As String = String.Format("{0}\SmoothStream.isml\Channel.txt", AppDomain.CurrentDomain.BaseDirectory)
        If File.Exists(path) Then
            Using sr As StreamReader = File.OpenText(path)
                type = sr.ReadLine
                id = CInt(sr.ReadLine)
            End Using
            Dim text As String = String.Empty
            Select Case type.ToLower
                Case "tv", "radio"
                    text = uWiMP.TVServer.Channels.GetChannelNameByChannelId(id)

            End Select
            markup += String.Format("<li>{0} {1}</li>", "Streaming", text) 'GetGlobalResourceObject("uWiMPStrings", "stream_stopped"))
            'markup += String.Format("<a href=""Streaming/StopTVStream.aspx#_StopTVStream"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "stop"))
            markup += "</ul>"

            markup += "<div class=""iBlock"">"
            markup += String.Format("<div><p>{0}</p>", GetGlobalResourceObject("uWiMPStrings", "stream_started"))
            markup += "<table class=""center""><tr>"
            markup += String.Format("<td class=""grid""><a href=""../../SmoothStream.isml/SmoothStream.m3u8""><img src=""{0}{1}.png"" /></a></td>", imageURI, "play")
            markup += String.Format("<td class=""grid""><a href=""Streaming/StopTVStream.aspx#_StopTVStream"" rev=""async""><img src=""{0}{1}.png"" /></a></td>", imageURI, "stop")
            markup += String.Format("<td class=""grid""><a href=""../../Desktop/SmoothStream.htm"" target=""_blank""><img src=""{0}{1}.png"" /></a></td>", imageURI, "browser")
            markup += "</tr></table>"
            markup += "</div>"
            markup += "</div>"
        Else
            markup += String.Format("<li>{0}</li>", "Nothing streaming") 'GetGlobalResourceObject("uWiMPStrings", "stream_stopped_failed"))
            markup += "</ul>"
        End If


        markup += "</div>"

        Return markup

    End Function

End Class