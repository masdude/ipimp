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

Partial Public Class AppearanceSelect
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waAppearanceSelect"

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
        xw.WriteCData(AppearanceSelect())
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function AppearanceSelect() As String

        Dim markup As String = String.Empty
        Dim colour As String = uWiMP.TVServer.Utilities.GetAppConfig("COLOUR")

        markup += "<div class=""iPanel"" >"
        markup += "<fieldset>"
        markup += "<legend>" & GetGlobalResourceObject("uWiMPStrings", "appearance") & "</legend>"
        markup += "<ul>"
        markup += String.Format("<li id=""jsAppearance"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "select_colour"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "default"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "default"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "black"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "black"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "orange"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "orange"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "green"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "green"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "purple"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "purple"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "pink"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "pink"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "red"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "red"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" checked=""{0}""/> {1}</label>", IIf(colour = GetGlobalResourceObject("uWiMPStrings", "blue"), "checked", ""), GetGlobalResourceObject("uWiMPStrings", "blue"))
        markup += "</li>"
        markup += "</ul>"
        markup += "</fieldset>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""return changecolour();"" rel=""Action"" class=""iButton iBAction"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "update"))
        markup += "</div>"

        Return markup

    End Function

End Class