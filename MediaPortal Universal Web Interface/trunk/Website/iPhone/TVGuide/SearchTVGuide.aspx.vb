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


Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class SearchTVGuide
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim groupID As String = Request.QueryString("group")
        Dim wa As String = "waSearchGroup" & groupID

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
        xw.WriteCData(DisplaySearchGroupMenu(wa, groupID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplaySearchGroupMenu(ByVal wa As String, ByVal groupID As String) As String

        Dim group As ChannelGroup = uWiMP.TVServer.ChannelGroups.GetChannelGroupByGroupId(CInt(groupID))

        Dim markup As String = ""
        Dim regexPattern = "[\\\/:\*\?""'<>|] "
        Dim oRegEx As New Regex(regexPattern)
        Dim safeGenre As String = ""

        Dim genres As List(Of String) = uWiMP.TVServer.Genres.GetGenres
        Dim genre As String
        If genres.Count > 1 Then genres.Sort()

        markup += "<div class=""iPanel"" id=""" & wa & """>"
        markup += "<fieldset>"
        markup += String.Format("<legend>{0} {1}</legend>", GetGlobalResourceObject("uWiMPStrings", "search"), group.GroupName)

        markup += "<ul>"
        markup += String.Format("<li><input type=""text"" id=""jsTVSearchText"" placeholder=""{0}""/></li>", GetGlobalResourceObject("uWiMPStrings", "search_term"))
        markup += String.Format("<li id=""jsTVSearchGenre"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "genre"))
        markup += String.Format("<label><input type=""radio"" name=""jsTVSearchGenre"" value=""all"" checked=""checked"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "all"))

        For Each genre In genres
            safeGenre = oRegEx.Replace(genre, "").ToLower
            safeGenre = Replace(safeGenre, " ", "")
            If genre <> "" Then
                markup += String.Format("<label><input type=""radio"" name=""jsTVSearchGenre"" value=""{0}"" /> {1}</label>", safeGenre, genre)
            End If
        Next
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsSearchDesc"" class=""iToggle"" title=""{1}"" /></li>", GetGlobalResourceObject("uWiMPStrings", "search_description"), GetGlobalResourceObject("uWiMPStrings", "yesno"))

        markup += "</li>"
        markup += "</ul>"

        markup += "</fieldset>"
        markup += "</div>"

        markup += "<div class=""iMenu"" style=""visibility:hidden"" id=""jsLoading"">"
        markup += String.Format("<ul class=""iArrow""><li><img style=""float:right;margin:3px 0 0"" src=images\loader.gif />{0}</li></ul></div>", GetGlobalResourceObject("uWiMPStrings", "searching"))

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""return tvsearch({0});"" rel=""Action"" class=""iButton iBAction"">{1}</a>", groupID, GetGlobalResourceObject("uWiMPStrings", "search"))
        markup += "</div>"

        Return markup

    End Function

End Class