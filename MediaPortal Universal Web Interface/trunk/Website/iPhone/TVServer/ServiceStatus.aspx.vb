Imports System.IO
Imports System.Xml

Partial Public Class ServiceStatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waServiceStatus"

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
        xw.WriteCData(DisplayServiceStatus(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayServiceStatus(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "service_status"))
        markup += "<ul class=""iArrow"">"

        Dim status As String = uWiMP.TVServer.Utilities.GetServiceStatus("TVService")
        If status = "" Then status = GetGlobalResourceObject("uWiMPStrings", "unknown")
        markup += String.Format("<li>{0}</li>", status)

        markup += "</ul>"
        markup += "</div>"

        If User.IsInRole("admin") Then
            markup += "<div>"
            Select Case status.ToLower
                Case "running"
                    markup += String.Format("<a href=""TVServer/ServiceActionConfirm.aspx?action=stop#_ServiceAction"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "stop"))
                Case "stopped"
                    markup += String.Format("<a href=""TVServer/ServiceActionConfirm.aspx?action=start#_ServiceAction"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "start"))
            End Select
            markup += "</div>"
        End If

        Return markup

    End Function

End Class