﻿' 
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

Partial Public Class RecordRadioProgramConfirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim programID As String = Request.QueryString("program")
        Dim wa As String = "waRecordRadioConfirm" & programID
        Dim recType As Integer = CInt(Request.QueryString("rectype"))

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
        xw.WriteCData(RecordProgram(programID, recType))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function RecordProgram(ByVal programID As String, ByVal recType As Integer) As String

        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(CInt(programID))
        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "recording"))
        markup += String.Format("<h3>{0}</h3>", program.Title)
        markup += String.Format("<h3>{0}</h3>", program.StartTime)

        markup += "<ul class=""iArrow"">"

        If uWiMP.TVServer.Recordings.RecordProgramById(CInt(programID), recType) = True Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "recording_add_success"))
        Else
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "recording_add_fail"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class