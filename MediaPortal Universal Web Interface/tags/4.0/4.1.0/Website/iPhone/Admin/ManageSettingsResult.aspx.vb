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

Partial Public Class ManageSettingsResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waSettingsResult"
        Dim pagesize As String = Request.QueryString("pagesize")
        Dim order As String = Request.QueryString("order")
        Dim client As String = Request.QueryString("client")
        Dim server As String = Request.QueryString("server")
        Dim submenu As String = Request.QueryString("submenu")
        Dim recsubmenu As String = Request.QueryString("recsubmenu")
        Dim recent As String = Request.QueryString("recent")

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
        xw.WriteCData(UpdateSettings(pagesize, order, server, client, submenu, recsubmenu, recent))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function UpdateSettings(ByVal pagesize As String, ByVal order As String, _
                                    ByVal server As String, ByVal client As String, _
                                    ByVal submenu As String, ByVal recsubmenu As String, _
                                    ByVal recent As String) As String

        Dim markup As String = String.Empty
        Dim success As Boolean = False

        If uWiMP.TVServer.Utilities.SetAppConfig("PAGESIZE", pagesize) = True Then
            success = True
        Else
            success = False
        End If

        If uWiMP.TVServer.Utilities.SetAppConfig("RECSUBMENU", recsubmenu) = True Then
            success = True
        Else
            success = False
        End If

        If uWiMP.TVServer.Utilities.SetAppConfig("RECORDER", order) = True Then
            success = True
        Else
            success = False
        End If

        If uWiMP.TVServer.Utilities.SetAppConfig("USETVSERVER", server) = True Then
            success = True
        Else
            success = False
        End If

        If uWiMP.TVServer.Utilities.SetAppConfig("USEMPCLIENT", client) = True Then
            success = True
        Else
            success = False
        End If

        If uWiMP.TVServer.Utilities.SetAppConfig("SUBMENU", submenu) = True Then
            success = True
        Else
            success = False
        End If

        If uWiMP.TVServer.Utilities.SetAppConfig("RECENTSIZE", recent) = True Then
            success = True
        Else
            success = False
        End If

        markup += "<div class=""iMenu"" >"

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "ipimp_settings"))
        markup += "<ul>"

        If success Then
            markup += String.Format("<li>Settings updated.</li>", GetGlobalResourceObject("uWiMPStrings", "ipimp_settings_success"))
        Else
            markup += String.Format("<li style=""color:red"">Settings update failed.</li>", GetGlobalResourceObject("uWiMPStrings", "ipimp_settings_fail"))
        End If

        markup += "</li>"
        markup += "</ul>"

        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""window.location.reload();"" rel=""Action"" class=""iButton iBAction"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += "</div>"

        Return markup

    End Function

End Class