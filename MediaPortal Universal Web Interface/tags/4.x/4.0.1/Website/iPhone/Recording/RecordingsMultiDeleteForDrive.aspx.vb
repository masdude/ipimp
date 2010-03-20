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

Partial Public Class RecordingsMultiDeleteForDrive
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim path As String = Request.QueryString("path")
        Dim start As String = Request.QueryString("start")
        Dim wa As String = "waRecDelete"

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
        If start = 0 Then
            xw.WriteAttributeString("mode", "replace")
            xw.WriteAttributeString("zone", wa)
            xw.WriteAttributeString("create", "true")
        Else
            xw.WriteAttributeString("mode", "self")
            xw.WriteAttributeString("zone", "moredrive")
        End If
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplayRecordingsDeleteMenu(wa, CInt(start), path))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsDeleteMenu(ByVal wa As String, ByVal start As Integer, ByVal path As String) As String

        Dim markup As String = String.Empty

        If start = 0 Then
            markup += "<div>"
            markup += String.Format("<a href=""#"" onclick=""return deletemulti();"" rel=""Action"" class=""iButton iBWarn"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "delete"))
            markup += "</div>"

            markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
            markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "recordings"), GetGlobalResourceObject("uWiMPStrings", "delete"))
            markup += "<ul class=""iArrow"">"
        End If

        Dim count As Integer = uWiMP.TVServer.Recordings.GetRecordingCountForDrive(path)
        Dim pagesize As Integer = CInt(uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE"))

        If (start + pagesize) > count Then
            pagesize = count - start
        End If

        Dim recordings As List(Of Recording) = uWiMP.TVServer.Recordings.GetRecordingsForDrive(start, pagesize, "date", path)
        Dim recording As Recording

        For Each recording In recordings
            markup += String.Format("<li><label>{0}<br/>{1}</label><input type=""checkbox"" value=""{2}"" class=""iToggle"" title=""{3}"" /></li>", recording.StartTime, Left(recording.Title.ToString, 20), recording.IdRecording.ToString, GetGlobalResourceObject("uWiMPStrings", "delete_keep"))
        Next

        If (start + pagesize) < count Then
            markup += String.Format("<li id=""moredrive"" class=""iMore""><a href=""Recording/RecordingsMultiDeleteForDrive.aspx?start={0}&path={1}#_RecDelete"" rev=""async"" title=""{2}"">{3}</a></li>", (start + pagesize).ToString, path, GetGlobalResourceObject("uWiMPStrings", "loading"), GetGlobalResourceObject("uWiMPStrings", "display_more"))
        End If

        If start = 0 Then
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class