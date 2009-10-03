Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordingWatch
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim recordingID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waWatchRec{0}", recordingID)

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
        xw.WriteCData(DisplayRecording(wa, recordingID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecording(ByVal wa As String, ByVal recordingID As String) As String

        Dim MP4path As String = uWiMP.TVServer.Utilities.GetAppConfig("STREAMPATH")
        Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(recordingID))
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(recording.IdChannel)
        Dim imageFile As String = MP4path & "\" & Path.GetFileNameWithoutExtension(recording.FileName) & ".png"
        Dim recFile As String = MP4path & "\" & Path.GetFileNameWithoutExtension(recording.FileName) & ".mp4"

        Dim markup As String = String.Empty
        Dim imageURI, recURI As String
        
        If uWiMP.TVServer.Utilities.DoesFileExist(imageFile) Then
            imageURI = String.Format("http://{0}/MP4/{1}.png", Request.ServerVariables("HTTP_HOST"), Path.GetFileNameWithoutExtension(recording.FileName))
        Else
            imageURI = String.Format("http://{0}/TVLogos/{1}.png", Request.ServerVariables("HTTP_HOST"), channel.DisplayName.ToString)
        End If

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += "<div class=""iBlock"">"
        markup += String.Format("<h3>{0}</h3>", recording.Title)
        markup += String.Format("<h3>{0}</h3>", recording.StartTime)

        If uWiMP.TVServer.Utilities.DoesFileExist(recFile) Then
            recURI = String.Format("http://{0}/MP4/{1}.mp4", Request.ServerVariables("HTTP_HOST"), Replace(Path.GetFileNameWithoutExtension(recording.FileName), " ", "%20"))
            markup += String.Format("<p><embed style=""margin:60px -9px -40px"" src=""{0}"" href=""{1}"" type=""video/quicktime"" target=""quicktimeplayer"" scale=""1"" autoplay=""true"" moviename=""{2}"" starttime=""05:00:00""/></p>", imageURI, recURI, recording.Title)
            markup += "</div>"
            markup += String.Format("<div class=""iBlock""><p>{0}</p></div>", GetGlobalResourceObject("uWiMPStrings", "quicktime"))
        Else
            recURI = String.Format("http://{0}/TVLogos/{1}.png", Request.ServerVariables("HTTP_HOST"), channel.DisplayName)
            markup += "<ul class=""iArrow"">"
            markup += String.Format("<li><a href=""Recording/RecordingTranscode.aspx?recid={0}&action=transcode#_Transcode{0}"" rev=""async"">{1}</a></li>", recordingID, GetGlobalResourceObject("uWiMPStrings", "not_transcoded"))
            markup += "</ul>"
            markup += "</div>"
        End If

        Return markup

    End Function

End Class