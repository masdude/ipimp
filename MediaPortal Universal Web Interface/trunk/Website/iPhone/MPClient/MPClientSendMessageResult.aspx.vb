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


Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports System.IO
Imports System.Xml
Imports Website.uWiMP.TVServer.MPClient

Partial Public Class MPClientSendMessageResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim message As String = Request.QueryString("message")
        Dim wa As String = String.Format("waMPClientSendMessageResult{0}", friendly)

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
        xw.WriteCData(SendMessage(friendly, message))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function SendMessage(ByVal friendly As String, ByVal message As String) As String

        Dim markup As String = String.Empty
        Dim response As String = String.Empty
        Dim successfulClients As New List(Of Client)
        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "sendmessage"
        mpRequest.Filter = String.Format("{0} {1}", GetGlobalResourceObject("uWiMPStrings", "message_from"), User.Identity.Name.ToString)
        mpRequest.Value = message


        If friendly.ToLower = "all" Then

            Dim clients As List(Of Client) = uWiMP.TVServer.MPClientDatabase.GetClients

            If (User.IsInRole("remoter")) And (clients.Count > 0) Then
                For Each client As uWiMP.TVServer.MPClient.Client In clients
                    If uWiMP.TVServer.MPClientRemoting.CanConnect(client.Friendly) Then
                        mpRequest.Filter = client.Friendly
                        response = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(client.Friendly, mpRequest)
                        Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
                        Dim success As Boolean = CType(jo("result"), Boolean)
                        If success Then successfulClients.Add(client)
                    End If
                Next
            End If

            markup += "<div class=""iMenu"">"
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "send_message_to"))
            markup += "<ul class=""iArrow"">"
            If successfulClients.Count > 0 Then
                For Each client As uWiMP.TVServer.MPClient.Client In successfulClients
                    markup += String.Format("<li><a href=""MPClient/MPClientMenu.aspx?friendly={0}#_MPClient"" rev=""async"">{0}</a></li>", client.Friendly)
                Next
            Else
                markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_messages_sent"))
            End If
            markup += "</ul>"
            markup += "</div>"

        Else
            response = uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, mpRequest)
            Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
            Dim success As Boolean = CType(jo("result"), Boolean)

            markup += "<div class=""iMenu"">"
            markup += String.Format("<h3>{0} - {1}</h3>", friendly, GetGlobalResourceObject("uWiMPStrings", "send_message"))
            markup += "<ul>"
            If success Then
                markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "send_message_success"))
            Else
                markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "send_message_fail"))
            End If
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class