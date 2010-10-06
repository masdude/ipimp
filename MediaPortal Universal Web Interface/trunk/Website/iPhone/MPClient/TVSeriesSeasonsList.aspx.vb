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

Partial Public Class TVSeriesSeasonsList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim ID As String = Request.QueryString("ID")

        Dim wa As String = String.Format("waTVSeriesSeasons{0}", ID)

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
        xw.WriteCData(DisplayTVSeasons(friendly, ID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayTVSeasons(ByVal friendly As String, ByVal seriesID As String)

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getseries"
        mpRequest.Value = seriesID

        Dim josnResponse As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(josnResponse), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))
        Dim name As String = CType(jo("name"), String)
        Dim summary As String = CType(jo("summary"), String)
        Dim banner As String = CType(jo("banner"), String)
        Dim imagePath As String = GetSeriesBanner(friendly, banner)

        mpRequest.Action = "getseasons"
        josnResponse = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        jo = CType(JsonConvert.Import(josnResponse), JsonObject)
        success = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))
        Dim ja As JsonArray = CType(jo("seasons"), JsonArray)
        Dim seasonList As uWiMP.TVServer.MPClient.SmallSeasonInfo() = DirectCast(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.SmallSeasonInfo()), ja.ToString), uWiMP.TVServer.MPClient.SmallSeasonInfo())

        If seasonList.Length = 1 Then
            Response.Redirect(String.Format("TVSeriesSeasonList.aspx?friendly={0}&compositeID={1}_{2}#_TVSeriesSeason{1}_{2}", friendly, seriesID, Replace(Split(seasonList(0).ID, "_")(1), "s", "")))
        End If

        markup += "<div class=""iMenu"" >"
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, name)

        markup += "<div class=""iBlock"">"
        markup += String.Format("<p><img src=""{0}"" width=""260"" style=""display:block; margin-left:auto; margin-right:auto;""/><br>{1}</p>", imagePath, summary)
        markup += "</div>"

        markup += "<ul class=""iArrow"">"

        For Each season As uWiMP.TVServer.MPClient.SmallSeasonInfo In seasonList
            Dim seasonIndex As String = Replace(Split(season.ID, "_")(1), "s", "")
            markup += String.Format("<li><a href=""MPClient/TVSeriesSeasonList.aspx?friendly={0}&compositeID={1}_{2}#_TVSeriesSeason{1}_{2}"" rev=""async"">{3} {2}</a></li>", friendly, seriesID, seasonIndex, GetGlobalResourceObject("uWiMPStrings", "tv_series_season"))
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

    Function GetSeriesBanner(ByVal friendly As String, ByVal banner As String) As String

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getimage"
        mpRequest.Value = banner

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
        Dim success As Boolean = CType(jo("result"), Boolean)
        If Not success Then Throw New Exception(String.Format("Error with iPiMP remoting...<br>Client: {0}<br>Action: {1}", friendly, mpRequest.Action))
        Dim filetype As String = CType(jo("filetype"), String)
        Dim filename As String = CType(jo("filename"), String)
        Dim data As String = CType(jo("data"), String)

        Dim relativePath As String = "../../images/tvseries"
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
                Return String.Format("{0}/tvepisodeblankbanner.png", relativePath)
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