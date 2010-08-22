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

Partial Public Class TVChannel
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim channelID As String = Request.QueryString("channel")
        Dim wa As String = "waChannel" & channelID

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
        xw.WriteCData(DisplayChannelMenu(channelID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayChannelMenu(ByVal channelID As String) As String

        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(CInt(channelID))
        Dim markup As String = String.Empty
        Dim scheduled As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0}</h3>", channel.Name)

        markup += "<ul class=""iArrow"">"
        If Not channel.CurrentProgram Is Nothing Then
            If uWiMP.TVServer.Schedules.IsProgramScheduled(channel.CurrentProgram) Then
                scheduled = "style=""color: red;"""
            Else
                scheduled = ""
            End If
            markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async"">{1}<small><br/>{2}</small></a></li>", channel.CurrentProgram.IdProgram.ToString, channel.CurrentProgram.Title, channel.CurrentProgram.StartTime, scheduled)
        End If
        If Not channel.NextProgram Is Nothing Then
            If uWiMP.TVServer.Schedules.IsProgramScheduled(channel.NextProgram) Then
                scheduled = "style=""color: red;"""
            Else
                scheduled = ""
            End If
            markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async"">{1}<small><br/>{2}</small></a></li>", channel.NextProgram.IdProgram.ToString, channel.NextProgram.Title, channel.NextProgram.StartTime, scheduled)
        End If
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        If User.IsInRole("watcher") Then
            markup += String.Format("<li><a href=""Streaming/StreamTVChannel.aspx?channel={0}#_StreamTVChannel"" rev=""async"">{1}</a></li>", channel.IdChannel.ToString, GetGlobalResourceObject("uWiMPStrings", "stream"))
        End If
        If User.IsInRole("recorder") Then
            markup += String.Format("<li><a href=""TVGuide/RecordManual.aspx?channel={0}#_RecordManual"" rev=""async"">{1}</a></li>", channel.IdChannel.ToString, GetGlobalResourceObject("uWiMPStrings", "manual_record"))
        End If
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day=0#_Channel{0}Day0"" rev=""async"">{1}</a></li>", channelID, GetGlobalResourceObject("uWiMPStrings", "today"))
        markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day=1#_Channel{0}Day1"" rev=""async"">{1}</a></li>", channelID, GetGlobalResourceObject("uWiMPStrings", "tomorrow"))
        For i As Integer = 2 To CInt(uWiMP.TVServer.Utilities.GetAppConfig("GUIDEDAYS")) - 1
            markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day={1}#_Channel{0}Day{1}"" rev=""async"">{2}</a></li>", channelID, i.ToString, Now.AddDays(i).ToString("dddd"))
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class