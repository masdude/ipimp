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
Imports TvDatabase

Partial Public Class SearchTVGuideResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim groupID As String = Request.QueryString("group")
        Dim wa As String = "waSearchResults" & groupID
        Dim search As String = Request.QueryString("search")
        Dim genre As String = Request.QueryString("genre")
        Dim desc As Boolean = CBool(Request.QueryString("desc"))
        If genre = "" Then genre = "all"

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
        xw.WriteCData(DisplaySearchResults(groupID, genre, search, desc))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySearchResults(ByVal groupID As String, ByVal genre As String, ByVal search As String, ByVal SearchDesc As Boolean) As String


        Dim markup As String = String.Empty
        Dim matchedPrograms As List(Of Program) = uWiMP.TVServer.Programs.SearchPrograms(groupID, genre, search, SearchDesc, "tv")

        markup += "<div class=""iMenu"">"
        If genre.ToLower = "all" Then
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "programs_found"))
        Else
            markup += String.Format("<h3>{0} ({1})</h3>", GetGlobalResourceObject("uWiMPStrings", "programs_found"), genre)
        End If
        markup += "<ul class=""iArrow"">"
        
        If matchedPrograms.Count = 0 Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_programs_found"))
        Else
            For Each program In matchedPrograms
                Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(program.IdChannel)
                If uWiMP.TVServer.Schedules.IsProgramScheduled(program) Then
                    markup += String.Format("<li><a style=""color: red;"" href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async""><img src=""../../TVLogos/{1}.png"" height=""40""/><em>{2}<small><br/>{3}</small></em></a></li>", program.IdProgram.ToString, uWiMP.TVServer.Utilities.GetMPSafeFilename(channel.DisplayName), program.Title.ToString, program.StartTime)
                Else
                    markup += String.Format("<li><a href=""TVGuide/TVProgram.aspx?program={0}#_Program{0}"" rev=""async""><img src=""../../TVLogos/{1}.png"" height=""40""/><em>{2}<small><br/>{3}</small></em></a></li>", program.IdProgram.ToString, uWiMP.TVServer.Utilities.GetMPSafeFilename(channel.DisplayName), program.Title.ToString, program.StartTime)
                End If
            Next
        End If

        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""TVGuide/SearchTVGuide.aspx?group={0}#_TVGuideSearch"" rev=""async"" rel=""Action"" class=""iButton iBAction"">{1}</a></li>", groupID, GetGlobalResourceObject("uWiMPStrings", "search"))
        markup += "</div>"

        Return markup

    End Function

End Class