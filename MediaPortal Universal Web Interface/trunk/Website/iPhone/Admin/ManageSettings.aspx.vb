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

Partial Public Class ManageSettings
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waSettings"

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
        xw.WriteCData(ManageSettings())
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function ManageSettings() As String

        Dim markup As String = String.Empty
        Dim listSize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE")
        markup += "<div class=""iMenu"" >"

        markup += "<h3>" & GetGlobalResourceObject("uWiMPStrings", "ipimp_settings") & "</h3>"
        markup += "<ul>"

        markup += String.Format("<li id=""jsPageSize"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "select_list_size"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/>5</label>", IIf(listSize = 5, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/>10</label>", IIf(listSize = 10, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/>20</label>", IIf(listSize = 20, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/>50</label>", IIf(listSize = 50, "checked", ""))
        markup += "</li>"
        markup += "</ul>"

        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""return updatesettings();"" rel=""Action"" class=""iButton iBAction"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "update"))
        markup += "</div>"

        Return markup

    End Function

End Class