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

Partial Public Class CardActionResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim id As String = Request.QueryString("id")
        Dim wa As String = String.Format("waCardStopResult{0}", id)

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
        xw.WriteCData(StopCard(wa, id))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function StopCard(ByVal wa As String, ByVal id As String) As String

        Dim card As Card = uWiMP.TVServer.Cards.GetCard(CInt(id))
        Dim status As uWiMP.TVServer.Cards.Status = uWiMP.TVServer.Cards.GetCardStatus(card)
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        If status = uWiMP.TVServer.Cards.Status.timeshifting Then
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "stop_timeshifting"))
            markup += "<ul class=""iArrow"">"
            If uWiMP.TVServer.Cards.StopTimeshifting(card) Then
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "stop_timeshifting_success"))
            Else
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "stop_timeshifting_fail"))
            End If
            markup += "</ul>"
        ElseIf status = uWiMP.TVServer.Cards.Status.recording Then
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "stop_recording"))
            markup += "<ul class=""iArrow"">"
            If uWiMP.TVServer.Cards.StopRecording(card) Then
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "stop_recording_success"))
            Else
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "stop_recording_fail"))
            End If
            markup += "</ul>"
        End If

        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""TVServer/CardStatus.aspx#_CardStatus"" rev=""async"" rel=""Back"" class=""iButton iBClassic"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += String.Format("<a href=""#_Home""  rel=""Action"" class=""iButton iBClassic"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += "</div>"

        Return markup

    End Function

End Class