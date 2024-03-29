﻿' 
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

Partial Public Class RecordingKeep
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim recordingID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waDeleteRec{0}", recordingID)

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
        xw.WriteCData(DisplayKeepRecordingMenu(wa, recordingID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayKeepRecordingMenu(ByVal wa As String, ByVal recordingID As String) As String

        Dim markup As String = String.Empty
        Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(recordingID))

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", recording.Title)
        markup += String.Format("<h3>{0}</h3>", recording.StartTime)
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""Recording/RecordingKeepResult.aspx?id={0}&keep={1}#_Keep{1}Rec{0}"" rev=""async"">{2}</a></li>", recordingID, CInt(uWiMP.TVServer.Recordings.KeepType.UntilWatched), GetGlobalResourceObject("uWiMPStrings", "keep_until_watched"))
        markup += String.Format("<li><a href=""Recording/RecordingKeepResult.aspx?id={0}&keep={1}#_Keep{1}Rec{0}"" rev=""async"">{2}</a></li>", recordingID, CInt(uWiMP.TVServer.Recordings.KeepType.UntilSpaceNeeded), GetGlobalResourceObject("uWiMPStrings", "keep_until_space_needed"))
        markup += String.Format("<li><a href=""Recording/RecordingKeepResult.aspx?id={0}&keep={1}#_Keep{1}Rec{0}"" rev=""async"">{2}</a></li>", recordingID, CInt(uWiMP.TVServer.Recordings.KeepType.Always), GetGlobalResourceObject("uWiMPStrings", "keep_always"))
        markup += String.Format("<li><a href=""Recording/RecordingKeepResult.aspx?id={0}&keep={1}#_Keep{1}Rec{0}"" rev=""async"">{2}</a></li>", recordingID, CInt(uWiMP.TVServer.Recordings.KeepType.OneWeek), GetGlobalResourceObject("uWiMPStrings", "keep_for_one_week"))
        markup += String.Format("<li><a href=""Recording/RecordingKeepResult.aspx?id={0}&keep={1}#_Keep{1}Rec{0}"" rev=""async"">{2}</a></li>", recordingID, CInt(uWiMP.TVServer.Recordings.KeepType.OneMonth), GetGlobalResourceObject("uWiMPStrings", "keep_for_one_month"))
        markup += String.Format("<li><a href=""Recording/RecordingKeepResult.aspx?id={0}&keep={1}#_Keep{1}Rec{0}"" rev=""async"">{2}</a></li>", recordingID, CInt(uWiMP.TVServer.Recordings.KeepType.OneYear), GetGlobalResourceObject("uWiMPStrings", "keep_for_one_year"))
        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class