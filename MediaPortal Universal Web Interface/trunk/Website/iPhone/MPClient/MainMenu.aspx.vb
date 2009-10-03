Imports System.IO
Imports System.Xml
Imports Website.uWiMP.TVServer.MPClient

Partial Public Class MPClientMainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMPClientMenu"

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
        xw.WriteCData(DisplayMPClientsMenu(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMPClientsMenu(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "mediaportal_clients"))

        Dim clients As List(Of Client) = uWiMP.TVServer.MPClientDatabase.GetClients

        If (User.IsInRole("remoter")) And (clients.Count > 0) Then
            markup += "<ul class=""iArrow"">"
            For Each client As uWiMP.TVServer.MPClient.Client In clients
                markup += String.Format("<li><a href=""MPClient/MPClientMenu.aspx?friendly={0}#_MPClient"" rev=""async"">{0}</a></li>", client.Friendly)
            Next
            markup += "</ul>"
            markup += "<ul class=""iArrow"">"
            markup += String.Format("<li><a href=""MPClient/MPClientSendMessage.aspx?friendly=all#_MPClientSendMessage"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "send_message_global"))
            markup += "</ul>"
        End If

        markup += "</div>"

        Return markup

    End Function

End Class