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
Imports System.Diagnostics

Partial Public Class uWiMPError
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waError"
        Dim ex As Exception = DirectCast(Application("fubar"), Exception)

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
        xw.WriteCData(GenerateError(wa, ex))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function GenerateError(ByVal wa As String, ByVal ex As Exception) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += "<div class=""iBlock"" >"
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "error"))
        If Not ex Is Nothing Then
            markup += String.Format("<p style=""color:maroon;font-weight:bold"">{0}</p>", ex.Message)
            markup += String.Format("<p style=""color:red;word-wrap:break-word;background-color:#ffffcc"">{0}</p>", ex.StackTrace)
        End If
        markup += "</div>"

        Dim mailSubject As String = String.Format("{0} {1}", GetGlobalResourceObject("uWiMPStrings", "ipimp"), GetGlobalResourceObject("uWiMPStrings", "error"))
        Dim mailBody As String = String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace)
        mailBody = Replace(mailBody, Environment.NewLine, "%0D%0A")

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""mailto:?subject={0}&body={1}"">{2}</a></li>", mailSubject, mailBody, GetGlobalResourceObject("uWiMPStrings", "email"))
        markup += "</ul>"

        markup += "</div>"

        Return markup

    End Function

End Class