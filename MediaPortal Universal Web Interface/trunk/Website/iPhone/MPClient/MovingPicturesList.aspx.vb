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

Partial Public Class MovingPicturesList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim filter As String = Request.QueryString("filter")
        Dim value As String = Request.QueryString("value")
        Dim start As Integer = CInt(Request.QueryString("start"))
        Dim pagesize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE")

        Dim wa As String = String.Format("waMovingPicturesMovies{0}", value)

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        If start >= pagesize Then
        Else
            'go
            xw.WriteStartElement("go")
            xw.WriteAttributeString("to", wa)
            xw.WriteEndElement()
            'end go

            'title
            xw.WriteStartElement("title")
            xw.WriteAttributeString("set", wa)
            xw.WriteEndElement() 'title
            'end title
        End If

        xw.WriteStartElement("destination")

        'start destination
        If start >= pagesize Then
            xw.WriteAttributeString("mode", "self")
            xw.WriteAttributeString("zone", String.Format("moremovingpictures{0}", filter))
        Else
            xw.WriteAttributeString("mode", "replace")
            xw.WriteAttributeString("zone", wa)
            xw.WriteAttributeString("create", "true")
        End If
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplayMyVideosList(wa, friendly, filter, value, start, pagesize))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMyVideosList(ByVal wa As String, _
                                         ByVal friendly As String, _
                                         ByVal filter As String, _
                                         ByVal value As String, _
                                         ByVal start As Integer, _
                                         ByVal pagesize As Integer) As String

        Dim markup As String = String.Empty
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "getmovingpictures"
        mpRequest.Filter = filter
        mpRequest.Value = value

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
        Dim movies As uWiMP.TVServer.MPClient.SmallMovieInfo() = CType(JsonConvert.Import(GetType(uWiMP.TVServer.MPClient.SmallMovieInfo()), response), uWiMP.TVServer.MPClient.SmallMovieInfo())

        If start = 0 Then
            markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
            markup += String.Format("<h3>{0} - {1} ({2})</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "moving_pictures"), value)
            markup += "<ul class=""iArrow"">"
        End If

        For i As Integer = start To (start + (pagesize - 1))
            If i <= UBound(movies) Then
                markup += String.Format("<li><a href=""MPClient/MovingPicturesDisplay.aspx?friendly={0}&ID={1}#_MovingPicturesVideo{1}"" rev=""async"">{2}</a></li>", friendly, movies(i).ID, movies(i).Title)
            End If
        Next

        If (start + (pagesize - 1)) < UBound(movies) Then
            markup += String.Format("<li id=""moremovingpictures{1}"" class=""iMore""><a href=""MPClient/MovingPicturesList.aspx?friendly={0}&filter={1}&value={2}&start={3}#_MovingPicturesMovies{2}"" rev=""async"" title=""{4}"">{5}</a></li>", friendly, filter, value, (start + pagesize).ToString, GetGlobalResourceObject("uWiMPStrings", "loading"), GetGlobalResourceObject("uWiMPStrings", "more"))
        End If

        If start = 0 Then
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class



