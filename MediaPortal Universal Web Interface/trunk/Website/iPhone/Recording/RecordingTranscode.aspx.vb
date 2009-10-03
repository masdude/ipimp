Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingTranscode
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim recordingID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waTranscode{0}", recordingID)
        Dim action As String = Request.QueryString("action")

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
        xw.WriteCData(TranscodeRecording(wa, recordingID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function TranscodeRecording(ByVal wa As String, ByVal recordingID As String) As String

        Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(recordingID))
        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += "<div class=""iBlock"">"
        markup += String.Format("<h3>{0}</h3>", recording.Title)
        markup += String.Format("<h3>{0}</h3>", recording.StartTime)

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class