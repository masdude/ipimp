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

Partial Public Class WatchTVChannel
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim channelID As String = Request.QueryString("channel")
        Dim wa As String = "waWatchTVChannel" & channelID

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
        xw.WriteCData(WatchChannel("lounge", channelID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function WatchChannel(ByVal friendly As String, ByVal channelID As String) As String

        Dim task As New uWiMP.TVServer.Streamer
        task.Media = uWiMP.TVServer.Streamer.MediaType.Tv
        task.ChannelID = channelID

        Dim asyncTask As New PageAsyncTask(AddressOf task.OnBegin, AddressOf task.OnEnd, AddressOf task.OnTimeout, "Stream", True)
        Page.RegisterAsyncTask(asyncTask)
        Page.ExecuteRegisteredAsyncTasks()

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1}</h3>", "lounge", GetGlobalResourceObject("uWiMPStrings", "watch"))

        markup += "<ul>"
        markup += String.Format("<li>{0}</li>", Replace(task.GetAsyncTaskProgress, vbCrLf, "<br>"))
        'If success Then
        '    markup += String.Format("<li>ZZ {0}</li>", GetGlobalResourceObject("uWiMPStrings", "loading"))
        'Else
        '    markup += String.Format("<li>ZZ {0}</li>", GetGlobalResourceObject("uWiMPStrings", "save_playlist_fail"))
        'End If
        'markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class
