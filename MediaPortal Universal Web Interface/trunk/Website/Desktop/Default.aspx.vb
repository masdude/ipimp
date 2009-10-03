Imports TvDatabase

Partial Public Class _Default2
    Inherits System.Web.UI.Page

    Private _hours As Integer = 6

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            LoadChannelGroups()
        End If

        LoadTVGuide()

    End Sub

    Private Sub LoadChannelGroups()

        Dim channelGroups As List(Of ChannelGroup) = uWiMP.TVServer.ChannelGroups.GetChannelGroups

        If Not channelGroups Is Nothing Then
            For Each cg As ChannelGroup In channelGroups
                Dim i As New ListItem
                i.Text = cg.GroupName
                i.Value = cg.IdGroup
                DropDownList1.Items.Add(i)
            Next
            DropDownList1.SelectedIndex = 0
        Else
            DropDownList1.Items.Add("Error")
        End If

    End Sub

    Private Sub LoadTVGuide()

        Dim idGroup As Integer

        If (DropDownList1.SelectedValue = "") Or (DropDownList1.SelectedValue.ToLower = "error") Then
            Exit Sub
        Else
            idGroup = CInt(DropDownList1.SelectedValue)
        End If

        Dim channelGroup As ChannelGroup = uWiMP.TVServer.ChannelGroups.GetChannelGroupByGroupId(idGroup)
        Dim channels As List(Of Channel) = uWiMP.TVServer.Channels.GetChannelsByGroupId(idGroup)

        '
        ' Build header table
        '
        Dim t As New Table
        Dim r As New TableRow
        Dim c1 As New TableCell

        c1.Width = Unit.Pixel(60)
        c1.Text = "Channel"
        r.Cells.Add(c1)



        For i As Integer = 0 To 5
            Dim c As New TableCell

            Dim hh, mm As Integer

            hh = Now.Hour
            mm = Now.Minute

            'If mm < 5 Then
            'c.Text = ""
            'c.Width = Unit.Pixel((60 - mm) * 2)
            'Else

            If i = 0 Then
                c.Text = ""
                c.Width = Unit.Pixel((60 - mm) * 2)
            Else
                c.Text = (hh + i).ToString & ":00"
                c.Width = Unit.Pixel(120)
            End If
            'End If

            r.Cells.Add(c)
        Next

        t.Rows.Add(r)
        t.GridLines = GridLines.Both
        phTVGuide.Controls.Add(t)

        '
        ' Build each channel table
        '
        For Each channel As Channel In channels

            Dim ct As New Table
            Dim cr As New TableRow
            Dim cc As New TableCell
            Dim w As Integer = 0
            ct.GridLines = GridLines.Both

            cc.Text = channel.Name
            cc.Width = Unit.Pixel(60)
            cc.Height = Unit.Pixel(30)
            cc.Font.Size = 8
            cc.VerticalAlign = VerticalAlign.Top
            cr.Cells.Add(cc)

            For Each p As Program In uWiMP.TVServer.Programs.GetProgramsByChannel(channel.IdChannel, _hours)
                Dim pc As TableCell = CreateProgramCell(p)
                cr.Cells.Add(pc)
                w += pc.Width.Value
                If w >= 900 Then Exit For
            Next

            ct.Rows.Add(cr)

            phTVGuide.Controls.Add(ct)
        Next

    End Sub

    Private Sub cgListChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DropDownList1.SelectedIndexChanged
        LoadTVGuide()
    End Sub

    Private Function CreateProgramCell(ByVal p As Program) As TableCell

        Dim timespan = (_hours * 60)

        Dim c As New TableCell

        If p.StartTime < Now Then
            c.Width = ((p.EndTime - Now).TotalMinutes) * 2
        Else
            c.Width = ((p.EndTime - p.StartTime).TotalMinutes) * 2
        End If

        If c.Width.Value <= 30 Then
            c.Text = Left(p.Title, 20)
        Else
            c.Text = p.Title
        End If

        c.Font.Size = 8
        c.VerticalAlign = VerticalAlign.Top

        Dim title As String = Replace(p.Title, "'", "\'")
        Dim description As String = Replace(p.Description, "'", "\'")
        Dim timetext As String = p.StartTime.ToShortTimeString & " - " & p.EndTime.ToShortTimeString

        If title Is Nothing Then title = "Unknown"
        If description Is Nothing Then description = "Unknown"

        c.Attributes("onmouseover") = "Tip('" & timetext & "<br>" & description & "', TITLE, '" & title & "', WIDTH, -300, SHADOW, true, CLICKSTICKY, true, CLICKCLOSE, true, CLOSEBTN, true)"
        c.Attributes("onmouseout") = "UnTip()"

        Return c

    End Function

End Class