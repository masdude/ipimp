Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class CardStatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waCardStatus"

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
        xw.WriteCData(DisplayCardStatus(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayCardStatus(ByVal wa As String) As String

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)

        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "card_status"))
        markup += "<ul class=""iArrow"">"

        Dim cards As List(Of Card) = uWiMP.TVServer.Cards.GetCards
        Dim card As Card
        Dim status As uWiMP.TVServer.Cards.Status
        For Each card In cards
            If card.Enabled And (InStr(card.Name.ToLower, "builtin") = 0) Then
                status = uWiMP.TVServer.Cards.GetCardStatus(card)
                If status = uWiMP.TVServer.Cards.Status.idle Then
                    markup += String.Format("<li>{0}<br/><small>{1}</small></li>", card.Name, GetGlobalResourceObject("uWiMPStrings", "idle"))
                Else
                    markup += String.Format("<li><a href=""TVServer/CardStatusMenu.aspx?id={0}#_CardStatus{0}"" rev=""async"">{1}<br/><small>{2}</small></a></li>", card.IdCard.ToString, card.Name, GetGlobalResourceObject("uWiMPStrings", "busy"))
                End If
            End If
        Next

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class