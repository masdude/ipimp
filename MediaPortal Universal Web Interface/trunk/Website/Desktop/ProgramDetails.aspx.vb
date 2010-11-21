Imports TvDatabase

Public Class ProgramDetails
    Inherits System.Web.UI.Page

    Dim p As Program

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim programID As String
        programID = Request.QueryString("programID")
        p = uWiMP.TVServer.Programs.GetProgramByProgramId(CInt(programID))

        If Not Page.IsPostBack Then
            
            Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            litTitle.Text = p.Title

            BuildTable()
        Else
            ClientScript.RegisterStartupScript(GetType(Page), "parentWindow", "<script type='text/JavaScript'>opener.location.href='TVGuide.aspx';</script>")
        End If
        
    End Sub

    Sub BuildTable()

        AddProgramTitle()
        AddProgramDescription()

    End Sub


    Sub AddProgramTitle()
        Dim t As New Table With {.Width = Unit.Percentage(100)}
        Dim r1 As New TableRow
        Dim c1 As New TableCell With {.CssClass = "rounded-corners darkgrey largerwhite", .Width = Unit.Percentage(20), .BorderWidth = 1, .BorderStyle = BorderStyle.Solid}
        c1.Text = uWiMP.TVServer.Channels.GetChannelNameByChannelId(p.IdChannel)
        r1.Cells.Add(c1)

        Dim r2 As New TableRow
        Dim c2 As New TableCell With {.CssClass = "rounded-corners darkgrey largerwhite", .Width = Unit.Percentage(80), .BorderWidth = 1, .BorderStyle = BorderStyle.Solid}
        Dim title As String = p.Title
        If title = String.Empty Then title = GetGlobalResourceObject("uWiMPStrings", "unknown_title")
        c2.Text = String.Format("<div>{0}<br>{1} - {2}</div>", title, p.StartTime.ToShortTimeString, p.EndTime.ToShortTimeString)
        
        r1.Cells.Add(c2)
        t.Rows.Add(r1)
        phProgramDetails.Controls.Add(t)

    End Sub

    Sub AddProgramDescription()
        Dim t As New Table With {.Width = Unit.Percentage(100)}
        Dim r As New TableRow
        Dim c As New TableCell With {.CssClass = "rounded-corners midgrey", .Width = Unit.Percentage(100), .BorderWidth = 1, .BorderStyle = BorderStyle.Solid}

        Dim description As String = p.Description
        If p.Description = String.Empty Then description = GetGlobalResourceObject("uWiMPStrings", "description_not_found")
        c.Text = description
        
        r.Cells.Add(c)
        t.Rows.Add(r)
        phProgramDetails.Controls.Add(t)
    End Sub

    Private Sub Btn1Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btn1.Click

        If uWiMP.TVServer.Recordings.RecordProgramById(p.IdProgram, rbl.SelectedValue) Then
            ClientScript.RegisterStartupScript(GetType(Page), "closePage", "<script type='text/JavaScript'>window.close();</script>")
        End If

    End Sub

End Class