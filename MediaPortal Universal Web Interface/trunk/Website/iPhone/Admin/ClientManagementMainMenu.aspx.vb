Imports System.IO
Imports System.Xml
Imports Website.uWiMP.TVServer.MPClient

Partial Public Class ClientManagementMainMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waClientMenu"

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
        xw.WriteCData(AddClientMainMenuOptions(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function AddClientMainMenuOptions(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "mediaportal_clients"))
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""Admin/ClientManagementAddMenu.aspx#_ClientAddMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "client_add"))

        Dim clients As List(Of Client)
        clients = uWiMP.TVServer.MPClientDatabase.GetClients
        If clients.Count > 0 Then
            markup += String.Format("<li><a href=""Admin/ClientManagementDeleteMenu.aspx#_ClientDeleteMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "client_del"))
            markup += String.Format("<li><a href=""Admin/ClientManagementUpdateMenu.aspx#_ClientUpdateMenu"" rev=""async"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "client_update"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function
End Class