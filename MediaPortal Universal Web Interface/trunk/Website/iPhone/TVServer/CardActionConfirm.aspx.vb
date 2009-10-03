Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class CardActionConfirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim id As String = Request.QueryString("id")
        Dim wa As String = String.Format("waCardStop{0}", id)

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
        xw.WriteCData(StopCard(wa, id))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function StopCard(ByVal wa As String, ByVal id As String) As String

        Dim card As Card = uWiMP.TVServer.Cards.GetCard(CInt(id))
        Dim status As uWiMP.TVServer.Cards.Status = uWiMP.TVServer.Cards.GetCardStatus(card)
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        If status = uWiMP.TVServer.Cards.Status.timeshifting Then
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "stop_timeshifting"))
        ElseIf status = uWiMP.TVServer.Cards.Status.recording Then
            markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "stop_recording"))
        Else
        End If

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "are_you_sure"))
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""TVServer/CardActionResult.aspx?id={0}#_CardStopResult{0}"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{1}</a>", id, GetGlobalResourceObject("uWiMPStrings", "yes"))
        markup += String.Format("<a href=""#"" onclick=""return WA.Back()"" rel=""Back"" class=""iButton iBClassic"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "no"))
        markup += "</div>"

        Return markup

    End Function

End Class