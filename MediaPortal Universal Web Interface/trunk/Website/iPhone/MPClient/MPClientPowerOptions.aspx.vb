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

Partial Public Class MPClientPowerOptions
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim wa As String = "waMPClientPowerOptions"

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
        xw.WriteCData(DisplayPowerOptions(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayPowerOptions(ByVal wa As String, ByVal friendly As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "power_options"))
        markup += "<ul class=""iArrow"">"

        markup += String.Format("<li id=""jsPowerOption"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "power_option_select"))
        markup += String.Format("<label><input type=""radio"" name=""jsPowerOption"" value=""{0}"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "power_logoff"))
        markup += String.Format("<label><input type=""radio"" name=""jsPowerOption"" value=""{0}"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "power_suspend"))
        markup += String.Format("<label><input type=""radio"" name=""jsPowerOption"" value=""{0}"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "power_hibernate"))
        markup += String.Format("<label><input type=""radio"" name=""jsPowerOption"" value=""{0}"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "power_reboot"))
        markup += String.Format("<label><input type=""radio"" name=""jsPowerOption"" value=""{0}"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "power_shutdown"))
        markup += String.Format("<label><input type=""radio"" name=""jsPowerOption"" value=""{0}"" /> {0}</label>", GetGlobalResourceObject("uWiMPStrings", "power_close"))

        markup += "</li>"
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBAction"" onclick=""return poweroption('{0}')"">{1}</a>", friendly, GetGlobalResourceObject("uWiMPStrings", "submit"))
        markup += "</div>"

        Return markup

    End Function

End Class