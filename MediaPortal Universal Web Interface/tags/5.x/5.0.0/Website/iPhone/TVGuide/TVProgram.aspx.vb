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

Partial Public Class TVProgram
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim programID As String = Request.QueryString("program")
        Dim wa As String = "waProgram" & programID

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
        xw.WriteCData(DisplayProgram(wa, programID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayProgram(ByVal wa As String, ByVal programID As String) As String

        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(CInt(programID))

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += "<div class=""iBlock"">"
        markup += String.Format("<h3>{0}</h3>", program.Title)
        markup += String.Format("<h3>{0}</h3>", program.StartTime)
        markup += String.Format("<p>{0}</p>", program.Description)
        markup += "</div>"

        markup += "<ul class=""iArrow"">"

        If User.IsInRole("recorder") Then
            markup += String.Format("<li><a href=""TVGuide/RecordTVProgram.aspx?program={0}#_RecordProgram{0}"" rev=""async"">{1}</a></li>", programID, GetGlobalResourceObject("uWiMPStrings", "record"))
        End If

        If program.IsRunningAt(Now) Then
            If User.IsInRole("watcher") Then
                Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(program.IdChannel)
                Dim channelName As String = channel.Name.ToLower
                channelName = Replace(channelName, " ", "")
                markup += String.Format("<li><a href=""Streaming/StreamTVChannel.aspx?channel={0}#_StreamTVChannel"" rev=""async"">{1}</a></li>", channel.IdChannel.ToString, GetGlobalResourceObject("uWiMPStrings", "watch"))
            End If
        End If

        Dim mailSubject As String = String.Format("{3} {0} at {1} - {2}", uWiMP.TVServer.Channels.GetChannelNameByChannelId(program.IdChannel), program.StartTime, program.Title, GetGlobalResourceObject("uWiMPStrings", "email_subject"))
        Dim mailBody As String = program.Description

        markup += String.Format("<li><a href=""mailto:?subject={0}&body={1}"">{2}</a></li>", mailSubject, mailBody, GetGlobalResourceObject("uWiMPStrings", "email"))
        markup += "</ul>"

        markup += "</div>"
        markup += "</div>"

        Return markup

    End Function

End Class