Imports TvDatabase

Public Class TVGuide
    Inherits System.Web.UI.Page

    Const MAXMINUTES As Integer = 900
    Dim startTime As DateTime
    Private _channelGroup As String
    Private _period As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        For Each item In Request.QueryString.AllKeys
            If item.ToLower = "cg" Then _channelGroup = Request.QueryString("cg")
            If item.ToLower = "hr" Then _channelGroup = Request.QueryString("hr")
        Next

        If Session("starttime") IsNot Nothing Then
            startTime = CDate(Session("starttime"))
        End If

        If Session("cg") IsNot Nothing Then
            _channelGroup = CStr(Session("cg"))
        Else
            _channelGroup = uWiMP.TVServer.ChannelGroups.GetFirstChannelGroupName
        End If

        If Session("hr") IsNot Nothing Then
            _period = CInt(Session("hr"))
        Else
            _period = 3
        End If

        If Not Page.IsPostBack Then

            Page.Title = String.Format("iPiMP {0}", GetGlobalResourceObject("uWiMPStrings", "tv_guide"))

            Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            link.ImageUrl = "https://www.paypal.com/en_GB/i/btn/btn_donate_LG.gif"
            link.NavigateUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6722080"

            startTime = Now

            LoadDropDownLists()
            LoadTVGuide()

        End If

        Session("starttime") = startTime
        Session("cg") = _channelGroup
        Session("hr") = _period

    End Sub

    Private Sub LoadDropDownLists()

        Dim channelGroups As List(Of ChannelGroup) = uWiMP.TVServer.ChannelGroups.GetChannelGroups
        Dim menu1 As String = String.Empty
        menu1 += "<ul id=""nav"">"

        menu1 += String.Format("<li><a href=""#"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "channel_groups"))
        menu1 += "<ul>"
        If Not channelGroups Is Nothing Then
            For Each cg As ChannelGroup In channelGroups
                menu1 += String.Format("<li><a href=""TVGuide.aspx?cg={0}"">{1}</a></li> ", cg.IdGroup, cg.GroupName)
            Next
        Else
            menu1 += String.Format("<li><a href=""#"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "no_channel_groups"))
        End If
        menu1 += "</ul>"
        menu1 += "</li>"

        menu1 += String.Format("<li><a href=""#"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "hours"))
        menu1 += "<ul>"
        For i As Integer = 3 To 6
            menu1 += String.Format("<li><a href=""TVGuide.aspx?hr={0}"">{1} {2}</a></li>", i.ToString, i.ToString, GetGlobalResourceObject("uWiMPStrings", "hours"))
        Next
        menu1 += "</ul>"
        menu1 += "</li>"

        menu1 += "</ul>"

        litChannelGroups.Text = menu1
    End Sub

    'Private Sub LoadDropDownLists2()

    'Dim channelGroups As List(Of ChannelGroup) = uWiMP.TVServer.ChannelGroups.GetChannelGroups

    '    If Not channelGroups Is Nothing Then
    '        For Each cg As ChannelGroup In channelGroups
    'Dim item As New ListItem
    '            item.Text = cg.GroupName
    '            item.Value = cg.IdGroup
    '            ddlChannels.Items.Add(item)
    '        Next
    '        ddlChannels.SelectedIndex = 0
    '    Else
    '        ddlChannels.Items.Add("Error")
    '    End If

    '    For i As Integer = 3 To 6
    'Dim item As New ListItem
    '        item.Text = i.ToString & "hours"
    '        item.Value = i.ToString
    '        ddlHours.Items.Add(item)
    '    Next
    '    ddlHours.SelectedIndex = 0

    'End Sub

    Private Sub LoadTVGuide()

        Dim idGroup As Integer

            For Each Group As ChannelGroup In uWiMP.TVServer.ChannelGroups.GetChannelGroups
                If Group.GroupName.ToLower = _channelGroup.ToLower Then
                    idGroup = Group.IdGroup
                    Exit For
                End If
            Next

        Dim channels As List(Of Channel) = uWiMP.TVServer.Channels.GetChannelsByGroupId(idGroup)

        InsertTimeRow()

        '
        ' Build each channel table
        '
        Dim alternate As Boolean = True
        Dim cssClass As String = String.Empty
        Dim rows As Integer = 0

        For Each channel As Channel In channels

            Dim table As New Table With {.Width = Unit.Percentage(100)}
            Dim row As New TableRow
            Dim cell As New TableCell With {.Width = 40}

            If alternate Then
                cssClass = "rounded-corners midgrey largeblack"
            Else
                cssClass = "rounded-corners lightgrey largeblack"
            End If

            Dim channelImage As New Image With {.AlternateText = channel.Name, .ImageUrl = String.Format("../../TVLogos/{0}.png", channel.Name), .Width = 40}
            cell.Controls.Add(channelImage)
            row.Cells.Add(cell)

            Dim programs As List(Of Program) = uWiMP.TVServer.Programs.GetProgramsByChannel(channel.IdChannel, startTime, _period)
            Dim mins As Integer
            Dim minsleft As Integer = MAXMINUTES

            If programs.Count >= 1 Then
                rows = rows + 1
                For Each p As Program In programs

                    Dim pc As TableCell = CreateProgramCell(p, channel)

                    If uWiMP.TVServer.Schedules.IsProgramScheduled(p) Then
                        pc.CssClass = "rounded-corners red largewhite"
                    Else
                        pc.CssClass = cssClass
                    End If

                    If (p.StartTime > startTime.AddMinutes(_period * 60)) Then
                        Exit For
                    ElseIf (p.StartTime < startTime) And (p.EndTime > startTime.AddMinutes(_period * 60)) Then
                        mins = minsleft
                    ElseIf (p.StartTime < startTime) And (p.EndTime < startTime.AddMinutes(_period * 60)) Then
                        mins = (p.EndTime - startTime).TotalMinutes
                    ElseIf (p.StartTime > startTime) And (p.EndTime < startTime.AddMinutes(_period * 60)) Then
                        mins = (p.EndTime - p.StartTime).TotalMinutes
                    ElseIf (p.StartTime > startTime) And (p.EndTime > startTime.AddMinutes(_period * 60)) Then
                        mins = (startTime.AddMinutes(_period * 60) - p.StartTime).TotalMinutes
                    End If

                    minsleft -= mins

                    pc.Width = Unit.Percentage((mins / (_period * 60)) * 100)
                    pc.Height = Unit.Pixel(40)

                    row.Cells.Add(pc)
                Next

                table.Rows.Add(row)
                Dim c2 As New TableCell
                ph1.Controls.Add(table)

                If alternate Then
                    alternate = False
                Else
                    alternate = True
                End If

            End If

            If rows = 10 Then
                rows = 0
                InsertTimeRow()
            End If

        Next

        Label3.Text = String.Format("{0} - {1}-{2}", startTime.ToShortDateString, startTime.ToShortTimeString, (startTime.AddHours(_period)).ToShortTimeString)

    End Sub

    Private Sub InsertTimeRow()

        Dim table As New Table With {.Width = Unit.Percentage(100)}
        Dim row As New TableRow

        Dim cell1 As New TableCell With {.Width = 40}
        Dim channelImage As New Image With {.ImageUrl = "../../images/tv.png", .Width = 40}
        cell1.Controls.Add(channelImage)
        row.Cells.Add(cell1)

        Dim cellWidth As Integer
        Dim width As Integer = MAXMINUTES / _period

        For i As Integer = 0 To _period

            Dim cell As New TableCell
            Dim hh, mm As Integer

            hh = startTime.Hour
            mm = startTime.Minute
            Dim text As String
            If hh + i = 12 Then
                text = String.Format("{0}pm", hh + i)
            ElseIf (hh + i = 13) AndAlso (hh >= 18) Then
                text = String.Format("{0}pm", hh + i)
            ElseIf hh + i > 12 Then
                text = String.Format("{0}pm", hh + i - 12)
            Else
                text = String.Format("{0}am", hh + i)
            End If

            cell.Text = String.Format("<div style=""width:90%"" class=""programtitle"">{0}</div>", text)

            If i = 0 Then
                cellWidth = CInt(width - (width * (mm / 60)))
            ElseIf i = CInt(_period) Then
                cellWidth = CInt(width * (mm / 60))
            Else
                cellWidth = CInt(width)
            End If
            cell.Width = Unit.Percentage((cellWidth / width) * (100 / _period))
            cell.CssClass = "rounded-corners darkgrey largerwhite"
            row.Cells.Add(cell)
        Next

        table.Rows.Add(row)
        ph1.Controls.Add(table)

    End Sub

    Private Function CreateProgramCell(ByVal p As Program, ByVal ch As Channel) As TableCell

        Dim c As New TableCell

        c.Text = String.Format("<div class=""programtitle"">{0}</div>", p.Title)

        Dim title As String = Replace(p.Title, "'", "\'")
        Dim description As String = Replace(p.Description, "'", "\'")
        Dim timetext As String = p.StartTime.ToShortTimeString & " - " & p.EndTime.ToShortTimeString

        If title Is Nothing Then title = GetGlobalResourceObject("uWiMPStrings", "unknown_title")
        If description Is Nothing Then description = GetGlobalResourceObject("uWiMPStrings", "description_not_found")

        Dim height As Integer
        height = (description.Length / 3) + 200

        c.Attributes("onmouseover") = String.Format("Tip('{0}',TITLE,'{1}<br>{2}&nbsp;{3}')", description, title, ch.Name, timetext)
        c.Attributes("onmouseout") = "UnTip()"
        c.Attributes.Add("onclick", String.Format("window.open('ProgramDetails.aspx?programID={0}','program{0}', 'width=400, height={1}, resizeable=yes, menubar=no, status=no, titlebar=no');", p.IdProgram.ToString, height))

        Return (c)

    End Function

    'Private Sub cgListChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlChannels.SelectedIndexChanged
    '    Session("starttime") = startTime
    '    LoadTVGuide()
    'End Sub

    'Private Sub hListChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlHours.SelectedIndexChanged
    '    Session("starttime") = startTime
    '    LoadTVGuide()
    'End Sub

    Private Sub PreviousDay(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        startTime = startTime.AddDays(-1)
        Session("starttime") = startTime
        LoadTVGuide()
    End Sub

    Private Sub PreviousPeriod(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        startTime = startTime.AddHours(_period)
        Session("starttime") = startTime
        LoadTVGuide()
    End Sub

    Private Sub NextPeriod(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        startTime = startTime.AddHours(_period)
        Session("starttime") = startTime
        LoadTVGuide()
    End Sub

    Private Sub NextDay(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        startTime = startTime.AddDays(1)
        Session("starttime") = startTime
        LoadTVGuide()
    End Sub

End Class