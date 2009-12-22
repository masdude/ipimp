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


Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Imports System.IO
Imports System.Xml
Imports System.Net

Partial Public Class MyMusicCoverArt
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim artist As String = Request.QueryString("artist")
        Dim album As String = Request.QueryString("album")
        Dim wa As String = "waMyMusicCoverArt"

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
        xw.WriteCData(DisplayCoverArt(wa, friendly, artist, album))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayCoverArt(ByVal wa As String, ByVal friendly As String, ByVal artist As String, ByVal album As String) As String

        Dim markup As String = String.Empty
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
            imagePath = saveImageToDisk(artist, album, imageString)
        End If

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", artist, album)
        markup += "<div class=""iBlock"" >"
        markup += "<p>"
        markup += String.Format("<img src=""{0}"" width=""260"" style=""display:block; margin-left:auto; margin-right:auto;""/>", imagePath)
        markup += "</p>"
        markup += "</ul>"
        markup += "</div>"
        markup += "</div>"

        Return markup

    End Function

    Function saveImageToDisk(ByVal artist As String, ByVal album As String, ByVal imageString As String) As String

        Dim imagePath As String = String.Empty

        Dim filename As String = album & "_" & artist & ".png"
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


End Class