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

Partial Public Class RadioRecordManual
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waRecordManual"
        Dim channelID As Integer = CInt(Request.QueryString("channel"))

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
        xw.WriteCData(DisplayRecordManualMenu(channelID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordManualMenu(ByVal channelID As Integer) As String

        Dim markup As String = String.Empty

        markup += "<div class=""iPanel"">"
        markup += "<fieldset>"
        markup += String.Format("<legend>{0}</legend>", GetGlobalResourceObject("uWiMPStrings", "manual_record"))

        markup += "<ul>"
        markup += String.Format("<li id=""jsRadioStartDate"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "date"))
        markup += String.Format("<label><input type=""radio"" name=""jsRadioStartDate"" value=""0"" checked=""checked""/> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "today"))
        markup += String.Format("<label><input type=""radio"" name=""jsRadioStartDate"" value=""1"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "tomorrow"))
        For i As Integer = 2 To 6
            markup += String.Format("<label><input type=""radio"" name=""jsRadioStartDate"" value=""{0}"" /> {1}</label>", i.ToString, Now.AddDays(i).ToString("dddd"))
        Next
        markup += "</li>"
        markup += String.Format("<li><input type=""number"" id=""jsRadioStartTime"" placeholder=""{0}""/></li>", GetGlobalResourceObject("uWiMPStrings", "start_time"))
        markup += String.Format("<li><input type=""number"" id=""jsRadioDuration"" placeholder=""{0}""/></li>", GetGlobalResourceObject("uWiMPStrings", "manual_record_duration"))
        markup += String.Format("<li><input type=""number"" id=""jsRadioSchedName"" placeholder=""{0}""/></li>", GetGlobalResourceObject("uWiMPStrings", "manual_record_name"))
        markup += "</ul>"
        markup += "</fieldset>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""return manualradiorecord('{0}');"" rel=""Action"" class=""iButton iBAction"">{1}</a>", channelID.ToString, GetGlobalResourceObject("uWiMPStrings", "submit"))
        markup += "</div>"

        Return markup

    End Function

End Class