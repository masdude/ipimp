Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingMultiDeleteConfirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim ids As String = Request.QueryString("ids")
        Dim wa As String = "waRecDeleteConfirm"

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
        xw.WriteCData(DisplayRecordingsDeleteMenu(wa, ids))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordingsDeleteMenu(ByVal wa As String, ByVal ids As String) As String

        Dim markup As String = String.Empty
        Dim count As Integer = ids.Length - ids.Replace("-", String.Empty).Length

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li>{0} {1}.<br><br>{2}</li>", count.ToString, GetGlobalResourceObject("uWiMPStrings", "recordings_selected_delete"), GetGlobalResourceObject("uWiMPStrings", "are_you_sure"))
        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""Recording/RecordingsMultiDeleteResult.aspx?ids={0}#_RecDeleteResults"" rev=""async"" rel=""Action"" class=""iButton iBWarn"">{1}</a></li>", ids, GetGlobalResourceObject("uWiMPStrings", "yes"))
        markup += String.Format("<a href=""#"" onclick=""return WA.Back()"" rel=""Back"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "no"))
        markup += "</div>"

        Return markup

    End Function

End Class