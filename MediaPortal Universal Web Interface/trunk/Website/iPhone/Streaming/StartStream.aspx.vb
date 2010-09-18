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

Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Imports System.IO
Imports System.Xml
Imports TvDatabase
Imports TvControl

Partial Public Class StartStream
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim mediaType As String = Request.QueryString("type")
        Dim mediaID As String = Request.QueryString("id")
        Dim wa As String = "waStartStream"

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
        xw.WriteCData(DoCountdown(mediaID, mediaType))
        xw.WriteEndElement()
        'end data

        'start script
        xw.WriteStartElement("script")
        xw.WriteCData("startCountdown(31)")
        xw.WriteEndElement()
        'end script

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DoCountdown(ByVal id As String, ByVal type As String) As String

        Dim task As New uWiMP.TVServer.Streamer

        Select Case type.ToLower
            Case "livetv"
                task.Media = uWiMP.TVServer.Streamer.MediaType.Tv
            Case "rec"
                task.Media = uWiMP.TVServer.Streamer.MediaType.Recording
        End Select
        task.MediaID = id

        Dim asyncTask As New PageAsyncTask(AddressOf task.OnBegin, AddressOf task.OnEnd, AddressOf task.OnTimeout, "Stream", True)
        Page.RegisterAsyncTask(asyncTask)
        Page.ExecuteRegisteredAsyncTasks()

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "stream"))

        markup += "<div class=""iBlock"">"

        markup += String.Format("<div><p>{0}</p>", GetGlobalResourceObject("uWiMPStrings", "stream_start_wait"))
        markup += "<table class=""center""><tr>"
        markup += "<td class=""grid"" id=""tvtimer"">-</td>"
        markup += "</tr></table>"
        markup += "</div>"

        markup += "</div>"

        markup += "</div>"

        Return markup

    End Function

End Class
