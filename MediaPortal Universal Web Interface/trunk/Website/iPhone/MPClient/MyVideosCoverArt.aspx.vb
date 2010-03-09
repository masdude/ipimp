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
Imports System.Net

Partial Public Class MPClientMyVideoCoverArt
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim movieID As String = Request.QueryString("ID")
        Dim wa As String = "waMPClientCoverArt"

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
        xw.WriteCData(DisplayCoverArt(wa, friendly, movieID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayCoverArt(ByVal wa As String, ByVal friendly As String, ByVal movieID As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmovie"
        mpRequest.Filter = movieID

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim movie As uWiMP.TVServer.MPClient.BigMovieInfo = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.BigMovieInfo), response), uWiMP.TVServer.MPClient.BigMovieInfo)

        If Not Directory.Exists(Server.MapPath("~/images/imdb")) Then Directory.CreateDirectory(Server.MapPath("~/images/imdb"))

        If File.Exists(Server.MapPath("~/images/imdb/" & movie.IMDBNumber & ".jpg")) Then File.Delete(Server.MapPath("~/images/imdb/" & movie.IMDBNumber & ".jpg"))

        saveImageByUrlToDisk(movie.ThumbURL, Server.MapPath("~/images/imdb/" & movie.IMDBNumber & ".jpg"))

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", movie.Title)
        markup += "<div class=""iBlock"" >"
        markup += "<p>"
        markup += String.Format("<img src=""../../images/imdb/{0}.jpg"" height=""300"" style=""display:block; margin-left:auto; margin-right:auto;""/>", movie.IMDBNumber)
        markup += "</p>"
        markup += "</ul>"
        markup += "</div>"
        markup += "</div>"

        Return markup

    End Function

    Public Shared Function saveImageByUrlToDisk(ByVal url As String, ByVal filename As String) As Boolean

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