Imports System.IO
Imports System.Xml
Imports TvDatabase

Partial Public Class ScheduleCleanResult
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waSchedCleanResult"

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
        xw.WriteCData(CleanSchedules(wa))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function CleanSchedules(ByVal wa As String) As String

        Dim markup As String = String.Empty


        Dim schedules As List(Of Schedule) = uWiMP.TVServer.Schedules.GetSchedules
        Dim schedule As Schedule
        Dim i As Integer = 0
        Dim j As Integer = 0

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", GetGlobalResourceObject("uWiMPStrings", "clean_up_schedules"))
        markup += "<ul class=""iArrow"">"

        For Each schedule In schedules
            If schedule.IsDone() Or schedule.Canceled <> schedule.MinSchedule Then
                If uWiMP.TVServer.Schedules.DeleteScheduleById(schedule.IdSchedule) = True Then
                    i = i + 1
                Else
                    j = j + 1
                End If
            End If
        Next

        If i + j = 0 Then markup += String.Format("<li>{0} {1}</li>", i + j.ToString, GetGlobalResourceObject("uWiMPStrings", "schedules_deleted_success"))
        If i > 0 Then markup += String.Format("<li>{0} {1}</li>", i.ToString, GetGlobalResourceObject("uWiMPStrings", "schedules_deleted_success"))
        If j > 0 Then markup += String.Format("<li>{0} {1}</li>", j.ToString, GetGlobalResourceObject("uWiMPStrings", "schedules_deleted_fail"))

        markup += "</ul>"
        markup += "</div>"

        markup += "<div>"
        markup += String.Format("<a href=""#"" rel=""Action"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "home"))
        markup += String.Format("<a href=""Scheules/MainMenu.aspx#_Schedules"" rev=""async"" rel=""Back"" class=""iButton iBClassic"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "back"))
        markup += "</div>"

        Return markup

    End Function

End Class