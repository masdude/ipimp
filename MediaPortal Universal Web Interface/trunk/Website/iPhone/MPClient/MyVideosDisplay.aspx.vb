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

Partial Public Class MyVideosDisplay
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim movieID As String = Request.QueryString("ID")
        Dim wa As String = String.Format("waMPClientVideo{0}", movieID)

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
        xw.WriteCData(DisplayMyVideoMenu(wa, friendly, movieID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyVideoMenu(ByVal wa As String, ByVal friendly As String, ByVal movieID As String) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmovie"
        mpRequest.Filter = movieID

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim movie As uWiMP.TVServer.MPClient.BigMovieInfo = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.BigMovieInfo), response), uWiMP.TVServer.MPClient.BigMovieInfo)

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, movie.Title)

        markup += "<div class=""iBlock"">"
        markup += String.Format("<p><em>{0}</em><br><br>{1}</p>", movie.Tagline, movie.Plot)
        markup += "</div>"

        'starbar
        markup += "<div class=""starbar static"">"
        markup += "<div class=""outer"">"
        markup += String.Format("<div class=""inner"" style=""width:{0}px;""/>", (movie.Rating * 20).ToString)
        markup += String.Format("<div class=""meta""><b>{0}/10</b></div>", movie.Rating)
        markup += "</div>"
        markup += "</div>"
        markup += "</div>"

        markup += "<ul class=""iArrow"">"

        markup += String.Format("<li><a href=""MPClient/MyVideosPlay.aspx?friendly={0}&ID={1}#_MPClientPlayVideo"" rev=""async"">{2}</a></li>", friendly, movieID, GetGlobalResourceObject("uWiMPStrings", "play"))
        markup += String.Format("<li><a href=""MPClient/MyVideosCoverArt.aspx?friendly={0}&ID={1}#_MPClientCoverArt"" rev=""async"">{2}</a></li>", friendly, movieID, GetGlobalResourceObject("uWiMPStrings", "cover_art"))
        markup += String.Format("<li><a href=""http://www.imdb.com/title/{0}/"" target=""_blank"" >{1}</a></li>", movie.IMDBNumber, GetGlobalResourceObject("uWiMPStrings", "imdb"))

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class