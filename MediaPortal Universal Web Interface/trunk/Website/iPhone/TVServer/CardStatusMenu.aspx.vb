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

Partial Public Class CardStatusMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim id As String = Request.QueryString("id")
        Dim wa As String = String.Format("waCardStatus{0}", id)

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
        xw.WriteCData(DisplayCardStatus(wa, id))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayCardStatus(ByVal wa As String, ByVal id As String) As String

        Dim card As Card = uWiMP.TVServer.Cards.GetCard(CInt(id))
        Dim status As uWiMP.TVServer.Cards.Status = uWiMP.TVServer.Cards.GetCardStatus(card)
        Dim usage As String = uWiMP.TVServer.Cards.GetCardUsageStatus(card, status)
        Dim username As String = Split(usage, ",")(0)
        Dim usageid As String = Split(usage, ",")(1)
        Dim result As String = String.Empty
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", card.Name)
        markup += "<div class=""iBlock"">"

        If status = uWiMP.TVServer.Cards.Status.grabbing Then
            result = GetGlobalResourceObject("uWiMPStrings", "grabbing")
        End If

        If status = uWiMP.TVServer.Cards.Status.scanning Then
            result = GetGlobalResourceObject("uWiMPStrings", "scanning")
        End If

        If status = uWiMP.TVServer.Cards.Status.timeshifting Then
            Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(usageid)
            result = GetGlobalResourceObject("uWiMPStrings", "timeshifting")
            result += "<br>"
            result += String.Format("{0}<br>", username)
            result += String.Format("{0}<br>", channel.Name)
            result += String.Format("{0}<br>", channel.CurrentProgram.Title)
            result += String.Format("{0} - {1}<br>", channel.CurrentProgram.StartTime.ToShortTimeString, channel.CurrentProgram.EndTime.ToShortTimeString)
        End If

        If status = uWiMP.TVServer.Cards.Status.recording Then
            result = GetGlobalResourceObject("uWiMPStrings", "recording")
            Dim schedule As Schedule = uWiMP.TVServer.Schedules.GetScheduleById(usageid)
            Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(schedule.IdChannel)
            result += "<br>"
            result += String.Format("{0}<br>", channel.Name)
            If schedule.ProgramName.ToLower = channel.CurrentProgram.Title.ToLower Then
                result += String.Format("{0}<br>", channel.CurrentProgram.Title)
                result += String.Format("{0} - {1}<br>", channel.CurrentProgram.StartTime.ToShortTimeString, channel.CurrentProgram.EndTime.ToShortTimeString)
            Else
                result += String.Format("{0}<br>", schedule.ProgramName)
                result += String.Format("{0} - {1}<br>", schedule.StartTime.ToShortTimeString, schedule.EndTime.ToShortTimeString)
            End If
        End If

        markup += String.Format("<p style=""font-weight:bold"">{0}</p>", result)

        markup += "</div>"
        markup += "</div>"

        If status = uWiMP.TVServer.Cards.Status.timeshifting Then
            markup += "<div>"
            markup += String.Format("<a href=""TVServer/CardActionConfirm.aspx?id={0}#_CardStop{0}"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{1}</a>", card.IdCard, GetGlobalResourceObject("uWiMPStrings", "stop"))
            markup += "</div>"
        End If

        Return markup

    End Function

End Class