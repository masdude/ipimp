Imports System.IO
Imports System.Xml
Imports Website.uWiMP.TVServer.MPClient

Partial Public Class ClientManagementUpdateMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waClientUpdateMenu"

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
        xw.WriteCData(UpdateClientMenuOptions(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function UpdateClientMenuOptions(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "client_update"))
        markup += "<ul class=""iArrow"">"

        Dim clients As List(Of Client) = uWiMP.TVServer.MPClientDatabase.GetClients
        If clients Is Nothing Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "no_clients_defined"))
        Else
            markup += String.Format("<li id=""jsMPClient"" class=""iRadio"" value=""autoback"">{0}", GetGlobalResourceObject("uWiMPStrings", "select_client"))
            For Each client As uWiMP.TVServer.MPClient.Client In clients
                markup += String.Format("<label><input type=""radio"" name=""jsMPClient"" value=""{0}"" /> {0}</label>", client.Friendly)
            Next
        End If

        markup += "</li>"
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBAction"" onclick=""return updmpclient()"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "update"))
        markup += "</div>"

        Return markup

    End Function

End Class