' 
'   Copyright (C) 2008-2009 Martin van der Boon
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
Imports System.Globalization

Partial Public Class NowAndNext
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim groupID As String = Request.QueryString("group")
        Dim wa As String = "waNowAndNext" & groupID

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
        xw.WriteCData(DisplayChannelGroupNowNext(wa, groupID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayChannelGroupNowNext(ByVal wa As String, ByVal groupID As String) As String

        Dim group As ChannelGroup = uWiMP.TVServer.ChannelGroups.GetChannelGroupByGroupId(CInt(groupID))
        Dim channels As List(Of Channel) = uWiMP.TVServer.Channels.GetChannelsByGroupId(CInt(groupID))
        Dim channel As Channel

        Dim markup As String = String.Empty
        Dim scheduled As String = String.Empty

        markup += "<div class=""iMenu"" id=""" & wa & """>"
        markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "now_and_next"), group.GroupName)
        markup += "<ul class=""iArrow"">"

        For Each channel In channels
            If Not channel.CurrentProgram Is Nothing Then
                If uWiMP.TVServer.Schedules.IsProgramScheduled(channel.CurrentProgram) Then
                    scheduled = "style=""color: red;"""
                Else
                    scheduled = ""
                End If
                markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async""><img src=""http://" & Request.ServerVariables("HTTP_HOST") & "/TVLogos/{4}.png"" height=""40""/><em>{1}<small><br/>{2}</small></em></a></li>", channel.CurrentProgram.IdProgram.ToString, channel.CurrentProgram.Title, channel.CurrentProgram.StartTime.ToShortTimeString, scheduled, channel.DisplayName)
            End If
            If Not channel.NextProgram Is Nothing Then
                If uWiMP.TVServer.Schedules.IsProgramScheduled(channel.NextProgram) Then
                    scheduled = "style=""color: red;"""
                Else
                    scheduled = ""
                End If
                markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async""><img src=""http://" & Request.ServerVariables("HTTP_HOST") & "/TVLogos/{4}.png"" height=""40""/><em>{1}<small><br/>{2}</small></em></a></li>", channel.NextProgram.IdProgram.ToString, channel.NextProgram.Title, channel.NextProgram.StartTime.ToShortTimeString, scheduled, channel.DisplayName)
            End If
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup


    End Function

End Class