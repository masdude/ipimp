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

Partial Public Class AboutiPiMP
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waAboutiPiMP"

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
        xw.WriteCData(AboutiPiMP())
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function AboutiPiMP() As String

        Dim markup As String = String.Empty

        markup += "<div>"
        markup += "<center>"
        markup += "<img alt=""iPiMP"" src=""images/iPiMP logo.png"" />"
        markup += "<br>"
        markup += "iPiMP is a web interface for <a target=""_blank"" href=""http://www.team-mediaportal.com"">MediaPortal</a> designed for the iPhone, use it to view your TV guide, manage recordings and schedules and remotely control MediaPortal clients. Future features are listed on the <a target=""_blank"" href=""http://www.team-mediaportal.com/manual/Extensions-TV-Server-Plugins/iPiMP"">iPiMP wiki</a>."
        markup += "<br><br>"
        markup += "<a target=""_blank"" href=""http://forum.team-mediaportal.com/ipimp-518/"">Visit the iPiMP sub forum on the MP forums.</a>"
        markup += "<br><br>"
        markup += "<small>"
        markup += "Developed by Cheezey."
        markup += "<br>"
        markup += "German translation by rhartinger."
        markup += "<br>"
        markup += "Norwegian translation by Reon."
        markup += "<br>"
        markup += "Danish translation by sbp."
        markup += "<br>"
        markup += "Dutch translation by minvis."
        markup += "<br>"
        markup += "Swedish translation by umlaut (and his girlfriend)."
        markup += "<br>"
        markup += "French translation by Cyberbabacool & CHli."
        markup += "<br>"
        markup += "Spanish translation by bodiroga."
        'markup += "<br>"
        'markup += "Italian translation by Nephilim."
        'markup += "<br>"
        'markup += "Faroese translation by uggabugga (who's probably the only Faroese user ;-)."
        markup += "<br><br>"
        markup += String.Format("You are running version <b>{0}</b>", uWiMP.TVServer.Utilities.GetAppConfig("VERSION"))
        markup += "<br>"
        markup += "<a target=""_blank"" href=""http://www.team-mediaportal.com""><img src=""../images/MP-logo.png"" alt=""Team MediaPortal"" /></a>"
        markup += "<br>"
        markup += "iPiMP uses <a target=""_blank"" href=""http://webapp.net.free.fr/"">WebApp.Net</a> - an iPhone/iPod touch Web<br>Application Micro-Framework."
        markup += "<br>"
        markup += "iPiMP uses uses video streaming technology by <a target=""_blank"" href=""http://smoothstreaming.code-shop.com/"">CodeShop</a>"
        markup += "<br>"
        markup += "iPiMP uses <a target=""_blank"" href=""http://jayrock.berlios.de/"">Jayrock</a> - an open source implementation of JSON."
        markup += "<br>"
        markup += "<a target=""_blank"" href=""http://www.visualsvn.com/""><img src=""http://www.visualsvn.com/images/VisualSVN_125x37.gif"" alt=""Powered by VisualSVN!"" /></a>"
        markup += "</small>"
        markup += "</center>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6722080"" rel=""Action"" class=""iButton iBOrange"" target=""_blank"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "donate"))
        markup += "</div>"

        Return markup

    End Function

End Class