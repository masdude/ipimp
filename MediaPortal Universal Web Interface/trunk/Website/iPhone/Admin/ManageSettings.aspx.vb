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
        Dim pagesize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("PAGESIZE")
        Dim recsubmenu As Boolean = uWiMP.TVServer.Utilities.GetAppConfig("RECSUBMENU")
        Dim recorder As String = uWiMP.TVServer.Utilities.GetAppConfig("RECORDER").ToLower
        Dim tvserver As Boolean = uWiMP.TVServer.Utilities.GetAppConfig("USETVSERVER")
        Dim client As Boolean = uWiMP.TVServer.Utilities.GetAppConfig("USEMPCLIENT")
        Dim submenu As Boolean = uWiMP.TVServer.Utilities.GetAppConfig("SUBMENU")
        Dim recentsize As Integer = uWiMP.TVServer.Utilities.GetAppConfig("RECENTSIZE")
        Dim myvideos As Boolean = uWiMP.TVServer.Utilities.GetAppConfig("MYVIDEOS")
        Dim movingpictures As Boolean = uWiMP.TVServer.Utilities.GetAppConfig("MOVINGPICTURES")

        markup += "<div class=""iPanel"" >"
        markup += "<fieldset>"
        markup += "<legend>" & GetGlobalResourceObject("uWiMPStrings", "ipimp_settings") & "</legend>"
        markup += "<ul>"
        markup += String.Format("<li id=""jsPageSize"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "select_list_size"))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" value=""5"" checked=""{0}""/> 5</label>", IIf(pagesize = 5, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" value=""10"" checked=""{0}""/> 10</label>", IIf(pagesize = 10, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" value=""20"" checked=""{0}""/> 20</label>", IIf(pagesize = 20, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsPageSize"" value=""50"" checked=""{0}""/> 50</label>", IIf(pagesize = 50, "checked", ""))
        markup += "</li>"
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsRecsSubmenu"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "show_recording_submenu"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(recsubmenu = True, "checked=""""", ""))
        markup += String.Format("<li id=""jsRecOrder"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "select_default_rec_order"))
        markup += String.Format("<label><input type=""radio"" name=""jsRecOrder"" value=""date"" checked=""{0}""/> {1}</label>", IIf(recorder = "date", "checked", ""), GetGlobalResourceObject("uWiMPStrings", "date"))
        markup += String.Format("<label><input type=""radio"" name=""jsRecOrder"" value=""genre"" checked=""{0}""/> {1}</label>", IIf(recorder = "genre", "checked", ""), GetGlobalResourceObject("uWiMPStrings", "genre"))
        markup += String.Format("<label><input type=""radio"" name=""jsRecOrder"" value=""channel"" checked=""{0}""/> {1}</label>", IIf(recorder = "channel", "checked", ""), GetGlobalResourceObject("uWiMPStrings", "channel"))
        markup += "</li>"
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsTVServerEnable"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "enable_server"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(tvserver = True, "checked=""""", ""))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsMPClientEnable"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "enable_client"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(client = True, "checked=""""", ""))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsMPClientSubmenu"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "show_client_submenu"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(submenu = True, "checked=""""", ""))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsMyVideos"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "my_videos"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(myvideos = True, "checked=""""", ""))
        markup += String.Format("<li><label>{0}</label><input type=""checkbox"" id=""jsMovingPictures"" class=""iToggle"" title=""{1}"" {2}/></li>", GetGlobalResourceObject("uWiMPStrings", "moving_pictures"), GetGlobalResourceObject("uWiMPStrings", "yesno"), IIf(movingpictures = True, "checked=""""", ""))
        markup += String.Format("<li id=""jsRecentSize"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "movingpictures_period"))
        markup += String.Format("<label><input type=""radio"" name=""jsRecentSize"" value=""7"" checked=""{0}""/> 7</label>", IIf(recentsize = 7, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsRecentSize"" value=""14"" checked=""{0}""/> 14</label>", IIf(recentsize = 14, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsRecentSize"" value=""31"" checked=""{0}""/> 31</label>", IIf(recentsize = 31, "checked", ""))
        markup += String.Format("<label><input type=""radio"" name=""jsRecentSize"" value=""180"" checked=""{0}""/> 180</label>", IIf(recentsize = 180, "checked", ""))
        markup += "</li>"
        markup += "</ul>"
        markup += "</fieldset>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" onclick=""return updatesettings();"" rel=""Action"" class=""iButton iBAction"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "update"))
        markup += "</div>"

        Return markup

    End Function

End Class