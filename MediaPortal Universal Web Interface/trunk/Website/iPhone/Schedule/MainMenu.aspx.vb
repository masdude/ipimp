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

Partial Public Class ScheduleMainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waSchedules"

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
        xw.WriteCData(DisplaySchedulesMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySchedulesMenu(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "schedules"))
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""Schedule/SchedulesByDate.aspx#_SchedDate"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "date"))
        markup += String.Format("<li><a href=""Schedule/SchedulesByChannel.aspx#_SchedChannel"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "channel"))
        markup += String.Format("<li><a href=""Schedule/SchedulesByTitleList.aspx?#_SchedTitle"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "title"))
        markup += "</ul>"

        If User.IsInRole("deleter") Then
            markup += "<ul class=""iArrow"">"
            markup += String.Format("<li><a href=""Schedule/ScheduleClean.aspx?#_SchedClean"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "clean_up_schedules"))
            markup += "</ul>"
        End If

        markup += "</div>"

        Return markup

    End Function

End Class