﻿Imports System.IO
Imports System.Xml

Partial Public Class ClientManagementDeleteConfirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waClientDeleteConfirm"

        Dim client As New uWiMP.TVServer.MPClient.Client
        client.Friendly = Request.QueryString("friendly")

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
        xw.WriteCData(DeleteClientMenuConfirm(wa, client))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DeleteClientMenuConfirm(ByVal wa As String, ByVal client As uWiMP.TVServer.MPClient.Client) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0} {1}</h3>", GetGlobalResourceObject("uWiMPStrings", "client_del"), client.Friendly)
        markup += "<ul class=""iArrow"">"
        If client.Friendly = "" Then
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "client_not_selected"))
        Else
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "are_you_sure"))
            markup += "</ul>"
            markup += "</div>"

            markup += "<div>"
            markup += String.Format("<a href=""Admin/ClientManagementDeleteResult.aspx?friendly={0}#_ClientDelResult"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{1}</a>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "yes"))
            markup += String.Format("<a href=""#"" onclick=""return WA.Back()"" rel=""Back"" class=""iButton iBClassic"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "no"))
            markup += "</div>"
        End If

        Return markup

    End Function

End Class