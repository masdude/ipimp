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


Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class DiskStatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = String.Format("waDiskStatus", ID)

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
        xw.WriteCData(DisplayDiskStatus(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayDiskStatus(ByVal wa As String) As String

        Dim markup As String = String.Empty
        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "disk_status"))
        markup += "<ul class=""iArrow"">"

        Dim cards As List(Of Card) = uWiMP.TVServer.Cards.GetCards
        Dim card As Card
        Dim space As String = String.Empty
        Dim folder As String = String.Empty
        Dim delete As String = String.Empty

        For Each card In cards
            If card.Enabled And (InStr(card.Name.ToLower, "builtin") = 0) Then
                If Not card.RecordingFolder = "" Then
                    folder = Path.GetPathRoot(card.RecordingFolder)
                    Try
                        space = String.Format("{0} {1}", Format((uWiMP.TVServer.External.DiskSpace.AvailableSpace(folder) / 1073741824), ".00"), GetGlobalResourceObject("uWiMPStrings", "gb_free"))
                    Catch ex As Exception
                        space = String.Format("{0} {1}", GetGlobalResourceObject("uWiMPStrings", "unknown"), GetGlobalResourceObject("uWiMPStrings", "gb_free"))
                    End Try
                End If
                If User.IsInRole("deleter") Then
                    delete = String.Format("<a href=""Recording/RecordingsMultiDeleteForDrive.aspx?start=0&path={0}#_RecDelete"" rev=""async"">", folder)
                Else
                    delete = ""
                End If
                markup += String.Format("<li>{0}{1}<br/><small>{2} {3}</small></a></li>", delete, card.Name, folder, space)
            End If
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class