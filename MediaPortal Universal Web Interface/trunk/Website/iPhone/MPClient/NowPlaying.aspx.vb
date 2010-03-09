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
        xw.WriteCData(GetNowPlaying(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function GetNowPlaying(ByVal wa As String, ByVal friendly As String) As String

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "nowplaying"

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
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

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "music"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += String.Format("<td align=""center""><img src=""{0}"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/></td>", GetMusicCoverArt(friendly, artist, album))
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

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1} - {2}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"), GetGlobalResourceObject("uWiMPStrings", "movie"))

        markup += "<div class=""iBlock"">"
        markup += "<table class=""imdbtable"">"
        markup += "<tr>"
        markup += String.Format("<td align=""center""><img src=""{0}"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/></td>", GetVideoCoverArt(friendly, id, type))
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
        Dim programID As Integer = uWiMP.TVServer.Cards.GetProgramIDForClient(hostname)
        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(programID)
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(program.IdChannel)

        Dim imagePath As String = String.Empty
        If programID = -1 Then
            imagePath = "<img src=""../../images/radio.png"" alt=""tv"" style=""height:200px;width:200px""/>"
        Else
            imagePath = String.Format("<img src=""http://{0}/RadioLogos/{1}.png"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/>", Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString)
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

    Private Function PlayingTV(ByVal friendly As String, ByVal jo As JsonObject) As String

        Dim hostname As String = CType(jo("hostname"), String)
        Dim programID As Integer = uWiMP.TVServer.Cards.GetProgramIDForClient(hostname)
        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(programID)
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(program.IdChannel)

        Dim imagePath As String = String.Empty
        If programID = -1 Then
            imagePath = "<img src=""../../images/tv.png"" alt=""tv"" style=""height:200px;width:200px""/>"
        Else
            imagePath = String.Format("<img src=""http://{0}/TVLogos/{1}.png"" height=""200"" style=""display:block; margin-left:auto; margin-right:auto;""/>", Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString)
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

    Private Function GetMusicCoverArt(ByVal friendly As String, ByVal artist As String, ByVal album As String) As String

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmusiccoverart"
        mpRequest.Filter = artist
        mpRequest.Value = album

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim imageString As String = CType(jo("result"), String)

        Dim imagePath As String = String.Empty
        If imageString.ToLower = "noimage" Then
            imagePath = "../../images/music/blankmusic.png"
        Else
            imagePath = SaveMusicImageToDisk(artist, album, imageString)
        End If

        Return imagePath

    End Function

    Private Function GetVideoCoverArt(ByVal friendly As String, ByVal movieID As String, ByVal type As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = String.Format("get{0}", type.ToLower)
        mpRequest.Filter = movieID

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim movie As uWiMP.TVServer.MPClient.BigMovieInfo = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.BigMovieInfo), response), uWiMP.TVServer.MPClient.BigMovieInfo)

        If Not Directory.Exists(Server.MapPath("~/images/imdb")) Then Directory.CreateDirectory(Server.MapPath("~/images/imdb"))

        If File.Exists(Server.MapPath("~/images/imdb/" & movie.IMDBNumber & ".jpg")) Then File.Delete(Server.MapPath("~/images/imdb/" & movie.IMDBNumber & ".jpg"))

        If Not movie.ThumbURL = "" Then
            SaveVideoImageByUrlToDisk(movie.ThumbURL, Server.MapPath("~/images/imdb/" & movie.IMDBNumber & ".jpg"))
            Return String.Format("../../images/imdb/{0}.jpg", movie.IMDBNumber)
        Else
            Return "../../images/imdb/blankmovie.png"
        End If

    End Function

    Private Function SaveMusicImageToDisk(ByVal artist As String, ByVal album As String, ByVal imageString As String) As String

        Dim imagePath As String = String.Empty

        Dim filename As String = String.Format("{0}_{1}.png", album, artist)
        Dim regexPattern = "[\\\/:\*\?""'<>|] "
        Dim objRegEx As New Regex(regexPattern)
        Dim safeFilename As String = ""
        safeFilename = objRegEx.Replace(filename, "").ToLower
        safeFilename = String.Format("../../images/music/{0}", Replace(safeFilename, " ", ""))

        If Not File.Exists(Server.MapPath(safeFilename)) Then

            If Not Directory.Exists(Server.MapPath("../../images/music")) Then Directory.CreateDirectory(Server.MapPath("../../images/music"))

            Dim newImage As System.Drawing.Image

            Dim imageAsBytes() As Byte = System.Convert.FromBase64String(imageString)
            Dim myStream As MemoryStream = New MemoryStream(imageAsBytes, 0, imageAsBytes.Length)
            myStream.Write(imageAsBytes, 0, imageAsBytes.Length)
            newImage = System.Drawing.Image.FromStream(myStream, True)

            Try
                newImage.Save(Server.MapPath(safeFilename), System.Drawing.Imaging.ImageFormat.Png)
            Catch ex As Exception
                If File.Exists(Server.MapPath(safeFilename)) Then File.Delete(Server.MapPath(safeFilename))
            End Try

        End If

        Return safeFilename

    End Function

    Public Shared Function SaveVideoImageByUrlToDisk(ByVal url As String, ByVal filename As String) As Boolean

        Dim response As WebResponse = Nothing
        Dim remoteStream As Stream = Nothing
        Dim readStream As StreamReader = Nothing
        Try
            Dim request As WebRequest = WebRequest.Create(url)
            If Not request Is Nothing Then
                response = request.GetResponse()
                If Not response Is Nothing Then
                    remoteStream = response.GetResponseStream()

                    readStream = New StreamReader(remoteStream)

                    Dim fw As Stream = File.Open(filename, FileMode.Create)

                    Dim buf() As Byte = New Byte(256) {}
                    Dim count As Integer = remoteStream.Read(buf, 0, 256)
                    While count > 0
                        fw.Write(buf, 0, count)

                        count = remoteStream.Read(buf, 0, 256)
                    End While

                    fw.Close()
                End If
            End If
        Finally
            If Not response Is Nothing Then
                response.Close()
            End If
            If Not remoteStream Is Nothing Then
                remoteStream.Close()
            End If
        End Try

        Return True

    End Function

End Class