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

Partial Public Class MPClientWOL
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMPClientWOL"
        Dim friendly As String = Request.QueryString("friendly")

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
        xw.WriteCData(MPClientWOL(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function MPClientWOL(ByVal wa As String, ByVal friendly As String) As String

        Dim client As uWiMP.TVServer.MPClient.Client
        client = uWiMP.TVServer.MPClientDatabase.GetClient(friendly)

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", client.Friendly)
        markup += "<ul class=""iArrow"">"

        uWiMP.TVServer.External.WOL.Init(7)
        uWiMP.TVServer.External.WOL.macAddress = client.MACAddress
        uWiMP.TVServer.External.WOL.wakeIt()

        markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "wol_sent"))
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += String.Format("<a href=""MPClient/MainMenu.aspx#_MPClientMenu"" rel=""Back"" rev=""async"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += "</div>"

        Return markup

    End Function

End Class