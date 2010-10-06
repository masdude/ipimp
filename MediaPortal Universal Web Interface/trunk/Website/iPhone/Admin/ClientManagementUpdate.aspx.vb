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

Partial Public Class ClientManagementUpdate
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim wa As String = "waClientUpdate"

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
        xw.WriteCData(CreateUpdateMenu(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function CreateUpdateMenu(ByVal wa As String, ByVal friendly As String) As String

        Dim client As uWiMP.TVServer.MPClient.Client
        client = uWiMP.TVServer.MPClientDatabase.GetClient(friendly)

        Dim markup As String = String.Empty

        If client.Friendly = "" Then
            markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "client_update"))
            markup += "<ul>"
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "client_not_selected"))
            markup += "</ul>"
            markup += "</div>"
        Else
            markup += String.Format("<div class=""iPanel"" id=""{0}"">", wa)
            markup += "<fieldset>"
            markup += String.Format("<legend>{0}</legend>", GetGlobalResourceObject("uWiMPStrings", "client_update"))

            markup += "<ul>"
            markup += String.Format("<li><input type=""text"" id=""jsFriendly"" placeholder=""{0}"" value=""{1}"" /></li>", If(client.Friendly = "", GetGlobalResourceObject("uWiMPStrings", "friendly"), ""), If(client.Friendly = "", "", client.Friendly))
            markup += String.Format("<li><input type=""text"" id=""jsHostname"" placeholder=""{0}"" value=""{1}"" /></li>", If(client.Hostname = "", GetGlobalResourceObject("uWiMPStrings", "hostname"), ""), If(client.Hostname = "", "", client.Hostname))
            markup += String.Format("<li><input type=""text"" id=""jsPort"" placeholder=""{0}"" value=""{1}"" /></li>", If(client.Port = "", GetGlobalResourceObject("uWiMPStrings", "port"), ""), If(client.Port = "", "", client.Port))
            markup += String.Format("<li><input type=""text"" id=""jsMAC"" placeholder=""{0}"" value=""{1}"" /></li>", If(client.MACAddress = "", GetGlobalResourceObject("uWiMPStrings", "mac"), ""), If(client.MACAddress = "", "", client.MACAddress))
            markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsUseMovingPictures"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "moving_pictures"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(client.usesMovingPictures, "checked=""""", ""))
            markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsUseTVSeries"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "tv_series"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(client.usesTVSeries, "checked=""""", ""))
            markup += "</ul>"

            markup += "<div class=""iBlock"">"
            markup += String.Format("<p>{0}<br><br>{1}<br><br>{2}", GetGlobalResourceObject("uWiMPStrings", "client_add_help1"), GetGlobalResourceObject("uWiMPStrings", "client_add_help2"), GetGlobalResourceObject("uWiMPStrings", "client_add_help3"))
            markup += "</div>"

            markup += "<div>"
            markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBAction"" onclick=""return updmpclient2()"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "update"))
            markup += "</div>"

            markup += "</fieldset>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class