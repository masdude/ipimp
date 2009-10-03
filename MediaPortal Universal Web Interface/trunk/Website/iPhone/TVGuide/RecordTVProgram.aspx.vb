Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class RecordTVProgram
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim programID As String = Request.QueryString("program")
        Dim wa As String = "waRecordProgram" & programID

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
        xw.WriteCData(DisplayRecordProgramMenu(wa, programID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayRecordProgramMenu(ByVal wa As String, ByVal programID As String) As String

        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(CInt(programID))

        Dim markup As String = ""

        markup += "<div class=""iMenu"" id=""" & wa & """>"
        markup += "<div class=""iBlock"">"
        markup += String.Format("<h3>{0}</h3>", program.Title)
        markup += String.Format("<h3>{0}</h3>", program.StartTime)
        If program.Description.Length > 110 Then
            markup += String.Format("<p>{0}....</p>", Left(program.Description, 100))
        Else
            markup += String.Format("<p>{0}</p>", program.Description)
        End If
        markup += "</div>"

        markup += "<ul class=""iArrow"">"
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.Once, GetGlobalResourceObject("uWiMPStrings", "once"))
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.AtThisTimeEveryDay, GetGlobalResourceObject("uWiMPStrings", "at_this_time_every_day"))
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.AtThisTimeEveryWeek, GetGlobalResourceObject("uWiMPStrings", "at_this_time_every_week"))
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.EachTimeOnThisChannel, GetGlobalResourceObject("uWiMPStrings", "each_time_on_this_channel"))
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.EachTimeOnAnyChannel, GetGlobalResourceObject("uWiMPStrings", "each_time_on_any_channel"))
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.AtThisTimeAtWeekends, GetGlobalResourceObject("uWiMPStrings", "at_this_time_at_weekends"))
        markup += String.Format("<li><a href=""TVGuide/RecordTVProgramConfirm.aspx?program={0}&rectype={1}#_RecordConfirm{0}"" rev=""async"">{2}</a></li>", programID, uWiMP.TVServer.Recordings.RecordingType.AtThisTimeOnWeekdays, GetGlobalResourceObject("uWiMPStrings", "at_this_time_on_weekdays"))
        markup += "</ul>"

        markup += "</div>"
        markup += "</div>"

        Return markup

    End Function
End Class