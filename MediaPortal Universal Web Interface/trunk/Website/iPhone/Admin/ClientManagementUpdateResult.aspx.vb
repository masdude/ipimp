﻿Imports System.IO
Imports System.Xml

Partial Public Class clientManagementUpdateResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waClientUpdateResult"

        Dim client As New uWiMP.TVServer.MPClient.Client
        client.Friendly = Request.QueryString("friendly")
        client.Hostname = Request.QueryString("hostname")
        client.Port = Request.QueryString("port")
        client.MACAddress = Request.QueryString("macaddress")

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
        xw.WriteCData(UpdateClient(wa, client))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function UpdateClient(ByVal wa As String, ByVal client As uWiMP.TVServer.MPClient.Client) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "client_update"))

        markup += "<ul>"

        If client.Friendly = "" Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "client_empty"))
        ElseIf uWiMP.TVServer.MPClientDatabase.ManageClient(client, "update") Then
            markup += String.Format("<li>{0} {1}", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "has_been_updated"))
        Else
            markup += String.Format("<li style=""color:red"">{0} {1}", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "could_not_be_updated"))
        End If

        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += String.Format("<a href=""Admin/ClientManagementMainMenu.aspx#_ClientMenu"" rel=""Back"" rev=""async"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += "</div>"

        Return markup

    End Function

End Class