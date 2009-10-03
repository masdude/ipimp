Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class ScheduleDeleteResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim scheduleID As String = Request.QueryString("id")
        Dim wa As String = String.Format("waDeleteRecResult{0}", scheduleID)

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
        xw.WriteCData(DisplayDeleteScheduleResult(wa, scheduleID))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayDeleteScheduleResult(ByVal wa As String, ByVal scheduleID As String) As String

        Dim markup As String = String.Empty
        Dim schedule As Schedule = uWiMP.TVServer.Schedules.GetScheduleById(CInt(scheduleID))

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", schedule.ProgramName)
        markup += String.Format("<h3>{0}</h3>", schedule.StartTime)
        markup += "<ul class=""iArrow"">"

        If uWiMP.TVServer.Schedules.DeleteScheduleById(CInt(scheduleID)) = True Then
            markup += String.Format("<li>{0}</li>", GetGlobalResourceObject("uWiMPStrings", "schedule_delete_success"))
        Else
            markup += String.Format("<li style=""color:red"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "schedule_delete_fail"))
        End If

        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += String.Format("<a href=""Schedule/MainMenu.aspx#_Scheduless"" rel=""Back"" rev=""async"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += "</div>"

        Return markup

    End Function

End Class