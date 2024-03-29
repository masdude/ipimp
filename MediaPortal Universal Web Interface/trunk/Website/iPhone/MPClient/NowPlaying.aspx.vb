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


Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports TvControl
Imports TvDatabase

Imports System.IO
Imports System.Xml
Imports System.Net

Partial Public Class NowPlaying
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim wa As String = "waMPClientNowPlaying"

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
        xw.WriteCData(GetNowPlaying(friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function GetNowPlaying(ByVal friendly As String) As String

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "nowplaying"

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))

        Dim media As String = CType(jo("media"), String)

        Select Case media.ToLower
            Case "music"
                Return PlayingMusic(friendly, jo)
            Case "video"
                Return PlayingVideo(friendly, jo, "video")
            Case "movingpicture"
                Return PlayingVideo(friendly, jo, "movingpicture")
            Case "dvd"
                Return PlayingDVD(friendly, jo)
            Case "radio"
                Return PlayingRadio(friendly, jo)
            Case "tv"
                Return PlayingTV(friendly, jo)
            Case "recording"
                Return PlayingRecording(friendly, jo)
            Case "tvepisode"
                Return PlayingTVEpisode(friendly, jo)
            Case "unknown"
                Return PlayingUnknown(friendly)
            Case "nothing"
                Return PlayingNothing(friendly)
            Case Else
                Return PlayingUnknown(friendly)
        End Select

    End Function

    Private Function PlayingMusic(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim album As String = CType(jo("album"), String)
        Dim artist As String = CType(jo("artist"), String)
        Dim title As String = CType(jo("title"), String)
        Dim genre As String = CType(jo("genre"), String)
        Dim track As String = CType(jo("track"), String)
        Dim duration As String = CType(jo("duration"), String)
        Dim position As String = CType(jo("position"), String)
        Dim thumb As String = CType(jo("thumb"), String)
        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "music"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        If thumb.Length > 0 Then markup += String.Format("<td align=""center""><img src=""{0}"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/></td>", GetThumb(friendly, thumb))
        markup += "</tr>"
        markup += "<tr>"
        markup += String.Format("<td align=""center""><b>{0}</b><br>", artist)

        If title <> String.Empty Then
            If track <> String.Empty Then
                markup += String.Format("<b>{0}. {1}</b><br>", track, title)
            Else
                markup += String.Format("<b>{0}</b><br>", title)
            End If
        Else
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_title"))
        End If

        If album <> String.Empty Then
            markup += String.Format("<b>{0}</b><br>", album)
        Else
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_album"))
        End If

        If (duration <> String.Empty) And (position <> String.Empty) Then
            Dim d, p As String
            d = uWiMP.TVServer.Utilities.ConvertToHHMMSS(duration)
            p = uWiMP.TVServer.Utilities.ConvertToHHMMSS(position)
            markup += String.Format("<b>{0} / {1}</b><br>", p, d)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/MyMusicListAlbumsForArtist.aspx?friendly={0}&value={1}#_MyMusicListAlbumForArtist"" rev=""async"">{2}</a></li>", friendly, artist, GetGlobalResourceObject("uWiMPStrings", "show_artist"))
        markup += String.Format("<li><a href=""MPClient/MyMusicListAlbum.aspx?friendly={0}&album={1}&artist={2}#_MyMusicListAlbum"" rev=""async"">{3}</a></li>", friendly, Server.UrlEncode(album), Server.UrlEncode(artist), GetGlobalResourceObject("uWiMPStrings", "show_album"))
        markup += String.Format("<li><a href=""MPClient/MyMusicListAlbumsForGenre.aspx?friendly={0}&value={1}&start=0#_MyMusicListAlbumForGenre"" rev=""async"">{2}</a></li>", friendly, genre, GetGlobalResourceObject("uWiMPStrings", "show_genre"))
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingVideo(ByVal friendly As String, ByVal jo As JsonObject, ByVal type As String) As String

        Dim title As String = CType(jo("title"), String)
        Dim tagline As String = CType(jo("tagline"), String)
        Dim id As String = CType(jo("id"), String)
        Dim genre As String = CType(jo("genre"), String)
        Dim filename As String = CType(jo("filename"), String)
        Dim duration As String = CType(jo("duration"), String)
        Dim position As String = CType(jo("position"), String)
        Dim thumb As String = CType(jo("thumb"), String)

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "movie"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += String.Format("<td align=""center""><img src=""{0}"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/></td>", GetThumb(friendly, thumb))
        markup += "</tr>"
        markup += "<tr>"
        markup += "<td align=""center"">"

        If title <> String.Empty Then
            markup += String.Format("<b>{0}</b><br>", title)
        Else
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_title"))
        End If

        If tagline <> String.Empty Then
            markup += String.Format("<b>{0}</b><br>", tagline)
        Else
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_tagline"))
        End If

        If (duration <> String.Empty) And (position <> String.Empty) Then
            Dim d, p As String
            d = uWiMP.TVServer.Utilities.ConvertToHHMMSS(duration)
            p = uWiMP.TVServer.Utilities.ConvertToHHMMSS(position)
            markup += String.Format("<b>{0} / {1}</b><br>", p, d)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        If type.ToLower = "video" Then
            markup += String.Format("<li><a href=""MPClient/MyVideosDisplay.aspx?friendly={0}&ID={1}#_MPClientVideo"" rev=""async"">{2}</a></li>", friendly, id, GetGlobalResourceObject("uWiMPStrings", "show_video"))
            markup += String.Format("<li><a href=""MPClient/MyVideosList.aspx?friendly={0}&filter=genre&value={1}&start=0#_MPClientVideos"" rev=""async"">{2}</a></li>", friendly, genre, GetGlobalResourceObject("uWiMPStrings", "show_genre"))
        End If
        If type.ToLower = "movingpicture" Then
            markup += String.Format("<li><a href=""MPClient/MovingPicturesDisplay.aspx?friendly={0}&ID={1}#_MovingPicturesVideo{1}"" rev=""async"">{2}</a></li>", friendly, id, GetGlobalResourceObject("uWiMPStrings", "show_video"))
            markup += String.Format("<li><a href=""MPClient/MovingPicturesList.aspx?friendly={0}&filter=genre&value={1}&start=0#_MovingPicturesMovies{1}"" rev=""async"">{2}</a></li>", friendly, genre, GetGlobalResourceObject("uWiMPStrings", "show_genre"))
        End If
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingDVD(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim duration As String = CType(jo("duration"), String)
        Dim position As String = CType(jo("position"), String)

        Dim markup As String = String.Empty
        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing_dvd"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += "<td align=""center""><img src=""../../images/dvd.png"" style=""height:200px;width:200px""/></td>"
        markup += "</tr>"
        markup += "<tr>"
        markup += "<td align=""center"">"

        If (duration <> String.Empty) And (position <> String.Empty) Then
            Dim d, p As String
            d = uWiMP.TVServer.Utilities.ConvertToHHMMSS(duration)
            p = uWiMP.TVServer.Utilities.ConvertToHHMMSS(position)
            markup += String.Format("<b>{0} / {1}</b><br>", p, d)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup


    End Function

    Private Function PlayingRadio(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim hostname As String = CType(jo("hostname"), String)
        Dim channelName As String = CType(jo("channel"), String)
        Dim programID As Integer = uWiMP.TVServer.Cards.GetProgramIDForClient(hostname)
        Dim program As Program = Nothing
        If programID <> -1 Then program = uWiMP.TVServer.Programs.GetProgramByProgramId(programID)
        
        Dim imagePath As String = String.Empty
        If channelName = "" Then
            imagePath = "<img src=""../../images/radio.png"" alt=""tv"" style=""height:200px;width:200px""/>"
        Else
            imagePath = String.Format("<img src=""../../RadioLogos/{0}.png"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/>", uWiMP.TVServer.Utilities.GetMPSafeFilename(channelName))
        End If

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "radio"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += String.Format("<td align=""center"">{0}</td>", imagePath)
        markup += "</tr>"
        markup += "<tr>"
        markup += "<td align=""center"">"
        markup += String.Format("<b>{0}</b><br>", channelName)

        If programID = -1 Then
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_program"))
        Else
            markup += String.Format("<b>{0}</b><br>", program.Title)
            markup += String.Format("<b>{0} - {1}</b><br>", program.StartTime.ToShortTimeString, program.EndTime.ToShortTimeString)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        Dim scheduled As String = String.Empty
        If uWiMP.TVServer.Schedules.IsProgramScheduled(program) Then
            scheduled = "style=""color: red;"""
        Else
            scheduled = ""
        End If
        If programID <> -1 Then markup += String.Format("<li><a {0} href=""RadioGuide/RadioProgram.aspx?program={1}#_Program{1}"" rev=""async"">{2}</a></li>", scheduled, programID.ToString, GetGlobalResourceObject("uWiMPStrings", "show_program"))
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingTV(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim hostname As String = CType(jo("hostname"), String)
        Dim programID As Integer = uWiMP.TVServer.Cards.GetProgramIDForClient(hostname)
        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(programID)
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(program.IdChannel)

        Dim imagePath As String = String.Empty
        If programID = -1 Then
            imagePath = "<img src=""../../images/tv.png"" alt=""tv"" style=""height:200px;width:200px""/>"
        Else
            imagePath = String.Format("<img src=""../../TVLogos/{0}.png"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/>", uWiMP.TVServer.Utilities.GetMPSafeFilename(channel.DisplayName))
        End If

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "tv"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += String.Format("<td align=""center"">{0}</td>", imagePath)
        markup += "</tr>"
        markup += "<tr>"
        markup += "<td align=""center"">"

        If programID = -1 Then
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_program"))
        Else
            markup += String.Format("<b>{0}</b><br>", program.Title)
            markup += String.Format("<b>{0}</b><br>", channel.Name)
            markup += String.Format("<b>{0} - {1}</b><br>", program.StartTime.ToShortTimeString, program.EndTime.ToShortTimeString)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        Dim scheduled As String = String.Empty
        If uWiMP.TVServer.Schedules.IsProgramScheduled(program) Then
            scheduled = "style=""color: red;"""
        Else
            scheduled = ""
        End If
        markup += String.Format("<li><a {0} href=""TVGuide/TVProgram.aspx?program={1}#_Program{1}"" rev=""async"">{2}</a></li>", scheduled, programID.ToString, GetGlobalResourceObject("uWiMPStrings", "show_program"))
        markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingRecording(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim hostname As String = CType(jo("hostname"), String)
        Dim filename As String = CType(jo("filename"), String)
        Dim duration As String = CType(jo("duration"), String)
        Dim position As String = CType(jo("position"), String)

        Dim clients As List(Of TvLibrary.Streaming.RtspClient) = uWiMP.TVServer.Cards.GetClients
        Dim client As TvLibrary.Streaming.RtspClient = Nothing
        Dim foundClient As Boolean = False

        For Each client In clients
            If InStr(client.StreamName, filename) > 0 Then
                foundClient = True
            End If
        Next

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "tvrecording"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += "<td align=""center""><img src=""../../images/rec.png"" style=""height:200px;width:200px""/></td>"
        markup += "</tr>"
        markup += "<tr>"
        markup += "<td align=""center"">"

        If foundClient Then
            markup += String.Format("<b>{0}</b><br>", client.Description)
            markup += String.Format("<b>{0}: {1}</b><br>", GetGlobalResourceObject("uWiMPStrings", "started_at"), client.DateTimeStarted.ToShortTimeString)
        Else
            markup += String.Format("<b>{0}</b><br>", GetGlobalResourceObject("uWiMPStrings", "unknown_recording"))
        End If

        If (duration <> String.Empty) And (position <> String.Empty) Then
            Dim d, p As String
            d = uWiMP.TVServer.Utilities.ConvertToHHMMSS(duration)
            p = uWiMP.TVServer.Utilities.ConvertToHHMMSS(position)
            markup += String.Format("<b>{0} / {1}</b><br>", p, d)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        'markup += "<ul class=""iArrow"">"
        'markup += String.Format("<li><a href=""Recording/RecordedProgram.aspx?id={0}#_RecProgram{0}"" rev=""async"">{1}</a></li>", recording.IdRecording.ToString, GetGlobalResourceObject("uWiMPStrings", "show_recording"))
        'markup += "</ul>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingTVEpisode(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim episode As String = CType(jo("episode"), String)
        Dim season As String = CType(jo("season"), String)
        Dim plot As String = CType(jo("plot"), String)
        Dim title As String = CType(jo("title"), String)
        Dim thumb As String = CType(jo("thumb"), String)
        Dim duration As String = CType(jo("duration"), String)
        Dim position As String = CType(jo("position"), String)

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"" >"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "tvseries_episode"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += String.Format("<td align=""center""><img src=""{0}"" width=""260"" style=""display:block; margin-left:auto; margin-right:auto;""/></td>", GetThumb(friendly, thumb))
        markup += "</tr>"
        markup += "<tr>"
        markup += "<td align=""center"">"

        If title <> String.Empty Then
            markup += String.Format("{0}<br>", title)
        Else
            markup += String.Format("{0}<br>", GetGlobalResourceObject("uWiMPStrings", "unknown_title"))
        End If

        If plot <> String.Empty Then
            markup += String.Format("{0}<br>", plot)
        Else
            markup += String.Format("{0}<br>", GetGlobalResourceObject("uWiMPStrings", "unknown_plot"))
        End If

        If (duration <> String.Empty) And (position <> String.Empty) Then
            Dim d, p As String
            d = uWiMP.TVServer.Utilities.ConvertToHHMMSS(duration)
            p = uWiMP.TVServer.Utilities.ConvertToHHMMSS(position)
            markup += String.Format("<b>{0} / {1}</b><br>", p, d)
        End If

        markup += "</td>"
        markup += "</tr>"
        markup += "</table>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "refresh"))
        markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingNothing(ByVal friendly As String) As String

        Dim markup As String = String.Empty
        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"))
        markup += "<ul>"
        markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "now_playing_nothing"))
        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

    Private Function PlayingUnknown(ByVal friendly As String) As String

        Dim markup As String = String.Empty
        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"))
        markup += "<ul>"
        markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "now_playing_unknown"))
        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

    Function GetThumb(ByVal friendly As String, ByVal thumb As String) As String

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getfile"
        mpRequest.Value = thumb
        mpRequest.Filter = "large"

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))
        Dim filetype As String = CType(jo("filetype"), String)
        Dim filename As String = CType(jo("filename"), String)
        Dim data As String = CType(jo("data"), String)

        Dim relativePath As String = String.Format("../../images/{0}", Split(thumb, ":")(0))
        Dim imagePath As String = String.Format("{0}/{1}", relativePath, filename)
        Dim format As System.Drawing.Imaging.ImageFormat
        Select Case filetype.ToLower
            Case "jpeg"
                format = System.Drawing.Imaging.ImageFormat.Jpeg
            Case "gif"
                format = System.Drawing.Imaging.ImageFormat.Gif
            Case "png"
                format = System.Drawing.Imaging.ImageFormat.Png
            Case "bmp"
                format = System.Drawing.Imaging.ImageFormat.Bmp
            Case Else
                Return String.Format("{0}/blank.png", relativePath)
        End Select

        If Not File.Exists(Server.MapPath(imagePath)) Then

            If Not Directory.Exists(relativePath) Then Directory.CreateDirectory(Server.MapPath(relativePath))

            Dim newImage As System.Drawing.Image

            Dim imageAsBytes() As Byte = System.Convert.FromBase64String(data)
            Dim myStream As MemoryStream = New MemoryStream(imageAsBytes, 0, imageAsBytes.Length)
            myStream.Write(imageAsBytes, 0, imageAsBytes.Length)
            newImage = System.Drawing.Image.FromStream(myStream, True)

            Try
                newImage.Save(Server.MapPath(imagePath), format)
            Catch ex As Exception
                If File.Exists(Server.MapPath(imagePath)) Then File.Delete(Server.MapPath(imagePath))
            End Try

        End If

        Return imagePath

    End Function

End Class