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

Partial Public Class TVChannelDay
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim channelID As String = Request.QueryString("channel")
        Dim day As String = Request.QueryString("day")
        Dim wa As String = String.Format("waChannel{0}_{1}", channelID, day)

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
        xw.WriteCData(DisplayChannelPrograms(channelID, day))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayChannelPrograms(ByVal channelID As String, ByVal day As String) As String

        Dim channel As TvDatabase.Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(CInt(channelID))

        Dim guideStart, guideEnd As DateTime
        Dim dayName As String = String.Empty

        Select Case day
            Case 0
                dayName = GetGlobalResourceObject("uWiMPStrings", "today")
            Case 1
                dayName = GetGlobalResourceObject("uWiMPStrings", "tomorrow")
            Case Else
                dayName = StrConv(Now.AddDays(CInt(day)).ToString("dddd"), vbProperCase)
        End Select

        Dim markup As String = ""
        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{2}: {0} {1}</h3>", GetGlobalResourceObject("uWiMPStrings", "programs_on"), dayName, channel.DisplayName)
        markup += "<ul class=""iArrow"">"

        Dim idLastProgram, idCurrentProgram As Integer
        Dim currentProgram As Program
        Dim startTime As String = String.Empty

        If day = 0 Then
            guideStart = Now.AddDays(day)
        Else
            guideStart = Now.Date.AddDays(day)
        End If

        If Not channel.CurrentProgram Is Nothing Then
            If day = 0 Then
                guideStart = Now.AddDays(day)
            Else
                guideStart = Now.Date.AddDays(day)
            End If
        ElseIf Not channel.NextProgram Is Nothing Then
            guideStart = channel.NextProgram.StartTime
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_programs_found"))
            markup += "</ul>"
            markup += "</div>"
            Return markup
            Exit Function
        End If

        '
        ' Finish at 23:59
        '
        guideEnd = guideStart.AddHours(23 - guideStart.Hour).AddMinutes(59 - guideStart.Minute)
        If channel.IsTv And channel.VisibleInGuide Then
            Do While guideStart <= guideEnd
                currentProgram = channel.GetProgramAt(guideStart)
                If currentProgram Is Nothing Then
                    idCurrentProgram = -1
                Else
                    idCurrentProgram = currentProgram.IdProgram
                End If

                If Not idLastProgram = idCurrentProgram And idCurrentProgram <> -1 Then
                    Dim scheduled As String = String.Empty
                    If uWiMP.TVServer.Schedules.IsProgramScheduled(currentProgram) Then
                        scheduled = "style=""color: red;"""
                    Else
                        scheduled = ""
                    End If
                    markup += String.Format("<li><a {3} href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async"">{1}<small><br/>{2}</small></a></li>", idCurrentProgram.ToString, currentProgram.Title.ToString, currentProgram.StartTime.ToShortTimeString, scheduled)
                    guideStart = currentProgram.EndTime
                End If

                '
                ' If there's an epg gap then add 1 minute and retest until the next program is found
                '
                If idLastProgram = -1 And idCurrentProgram = -1 Then
                    guideStart = guideStart.AddMinutes(1)
                End If
                idLastProgram = idCurrentProgram
            Loop

            markup += "</ul>"

            If CInt(day) < CInt(uWiMP.TVServer.Utilities.GetAppConfig("GUIDEDAYS")) - 1 Then
                markup += "<ul class=""iArrow"">"
                markup += String.Format("<li><a href=""TVGuide/TVChannelDay.aspx?channel={0}&day={2}#_Channel{0}Day{2}"" rev=""async"">{1}</a></li>", channelID, GetGlobalResourceObject("uWiMPStrings", "next_day"), (CInt(day) + 1).ToString)
                markup += "</ul>"
            End If

            markup += "</div>"

            Return markup
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_programs_found"))
            markup += "</ul>"
            markup += "</div>"
            Return markup
        End If

    End Function

End Class