Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingKeepResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim recordingID As String = Request.QueryString("id")
        Dim keep As String = Request.QueryString("keep")
        Dim wa As String = String.Format("waKeep{0}Rec{0}", keep, recordingID)

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
        xw.WriteCData(DisplayKeepRecordingResult(wa, recordingID, keep))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayKeepRecordingResult(ByVal wa As String, ByVal recordingID As String, ByVal keep As String) As String

        Dim markup As String = String.Empty
        Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(recordingID))

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", recording.Title)
        markup += String.Format("<h3>{0}</h3>", recording.StartTime)
        markup += "<ul class=""iArrow"">"

        If uWiMP.TVServer.Recordings.KeepRecordingById(CInt(recordingID), CInt(keep)) = True Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "recording_keep_success"))
        Else
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "recording_keep_fail"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class