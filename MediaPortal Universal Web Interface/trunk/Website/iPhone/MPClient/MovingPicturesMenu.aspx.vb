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

Partial Public Class MovingPicturesMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMovingPictures"
        Dim friendly As String = Request.QueryString("friendly")

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
        xw.WriteCData(DisplayMyVideosMenu(friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyVideosMenu(ByVal friendly As String) As String

        Dim markup As String = String.Empty

        markup += "<div class=""iMenu"">"
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "moving_pictures"))
        markup += "<ul class=""iArrow"">"

        markup += String.Format("<li><a href=""MPClient/MovingPicturesList.aspx?friendly={0}&filter=&value=&start=0#_MovingPicturesMovies"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "movingpictures_all"))
        markup += String.Format("<li><a href=""MPClient/MovingPicturesList.aspx?friendly={0}&filter=recent&value={1}&start=0#_MovingPicturesMovies{1}"" rev=""async"">{2}</a></li>", friendly, uWiMP.TVServer.Utilities.GetAppConfig("RECENTSIZE"), GetGlobalResourceObject("uWiMPStrings", "movingpictures_recent"))
        markup += String.Format("<li><a href=""MPClient/MovingPicturesListGenres.aspx?friendly={0}#_MovingPicturesListGenres"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "movingpictures_genre"))
        markup += String.Format("<li><a href=""MPClient/MovingPicturesListCertifications.aspx?friendly={0}#_MovingPicturesListCertifications"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "movingpictures_certification"))
        markup += String.Format("<li><a href=""MPClient/MovingPicturesListDecades.aspx?friendly={0}#_MovingPicturesListDecades"" rev=""async"">{1}</a></li>", friendly, GetGlobalResourceObject("uWiMPStrings", "movingpictures_year"))

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class