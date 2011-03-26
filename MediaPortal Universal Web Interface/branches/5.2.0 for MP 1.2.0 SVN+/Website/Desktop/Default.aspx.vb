' 
'   Copyright (C) 2008-2011 Martin van der Boon
' 
'  This program is free software: you can redistribute it and/or modify 
'  it under the terms of the GNU General Public License as published by 
'  the Free Software Foundation, either version 3 of the License, or 
'  (at your option) any later version. 
' 
'   This program is distributed in the hope that it will be useful, 
'   but WITHOUT ANY WARRANTY; without even the implied warranty of 
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
'   GNU General Public License for more details. 
' 
'   You should have received a copy of the GNU General Public License 
'   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
' 

Imports TvDatabase

Public Class _DesktopDefault
    Inherits System.Web.UI.Page

    Const MAXMINUTES As Integer = 900
    Private _startTime As DateTime
    Private _channelGroup As String = String.Empty
    Private _period As Integer = 3
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
        appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

        '
        ' Start TV Guide code
        '
        If _startTime = Nothing Then _startTime = Now
        If Session("st") IsNot Nothing Then _startTime = Session("st")
        If Session("cg") IsNot Nothing Then _channelGroup = Session("cg")
        If Session("hr") IsNot Nothing Then _period = Session("hr")

        For Each item In Request.QueryString.AllKeys
            If item.ToLower = "cg" Then _channelGroup = Request.QueryString("cg")
            If item.ToLower = "hr" Then _period = Request.QueryString("hr")
            If item.ToLower = "nav" Then
                Select Case Request.QueryString("nav")
                    Case "pd"
                        _startTime = _startTime.AddDays(-1)
                    Case "ph"
                        _startTime = _startTime.AddHours(-_period)
                    Case "nh"
                        _startTime = _startTime.AddHours(_period)
                    Case "nd"
                        _startTime = _startTime.AddDays(1)
                    Case "now"
                        _startTime = Now
                End Select
            End If
        Next

        If _channelGroup = String.Empty Then _channelGroup = uWiMP.TVServer.ChannelGroups.GetFirstChannelGroupName


        Page.Title = String.Format("iPiMP - {0}", GetGlobalResourceObject("uWiMPStrings", "tv_guide"))
        If litChannelGroups.Text.Length = 0 Then LoadDropDownLists()
        AddTabs()
        LoadTVGuide()


        Session("st") = _startTime
        Session("cg") = _channelGroup
        Session("hr") = _period

        '
        ' End TV Guide code
        '

    End Sub

    Private Sub AddTabs()

        Dim markup As String = String.Empty
        markup += "<ul class=""css-tabs"">"

        markup += String.Format("<li><a href=""#"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "tv_guide"))
        markup += String.Format("<li><a href=""#"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "search"))
        'markup += String.Format("<li><a href=""#"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "scheduled_programs"))
        'markup += String.Format("<li><a href=""#"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "recorded_programs"))
        markup += "</ul>"

        Dim lit As New Literal With {.Text = markup}
        PlaceHolder1.Controls.Add(lit)

    End Sub

#Region "TV Guide"

    Private Sub LoadDropDownLists()

        Dim channelGroups As List(Of ChannelGroup) = uWiMP.TVServer.ChannelGroups.GetChannelGroups
        Dim markup As String = String.Empty
        markup += "<ul class=""nav"">"

        markup += String.Format("<li><a href=""#"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "channel_groups"))
        markup += "<ul>"
        If Not channelGroups Is Nothing Then
            For Each cg As ChannelGroup In channelGroups
                markup += String.Format("<li><a href=""Default.aspx?cg={0}"">{1}</a></li> ", cg.IdGroup.ToString, cg.GroupName)
            Next
        Else
            markup += String.Format("<li><a href=""#"">{0}</a></li>", GetGlobalResourceObject("uWiMPStrings", "no_channel_groups"))
        End If
        markup += "</ul>"
        markup += "</li>"

        markup += String.Format("<li><a href=""#"">{0}</a>", GetGlobalResourceObject("uWiMPStrings", "hours"))
        markup += "<ul>"
        For i As Integer = 3 To 6
            markup += String.Format("<li><a href=""Default.aspx?hr={0}"">{1} {2}</a></li>", i.ToString, i.ToString, GetGlobalResourceObject("uWiMPStrings", "hours"))
        Next
        markup += "</ul>"
        markup += "</li>"

        markup += "<li><a href=""Default.aspx?nav=pd"">&lt;&lt;</a></li>"
        markup += "<li><a href=""Default.aspx?nav=ph"">&lt;</a></li>"
        markup += String.Format("<li class=""datetime"">{0} {1} {2} : {3}-{4}</li>", _startTime.ToString("dddd"), _startTime.ToString("dd"), _startTime.ToString("MMM"), _startTime.ToShortTimeString, (_startTime.AddHours(_period)).ToShortTimeString)
        markup += "<li><a href=""Default.aspx?nav=nh"">&gt;</a></li>"
        markup += "<li><a href=""Default.aspx?nav=nd"">&gt;&gt;</a></li>"
        markup += String.Format("<li><a href=""Default.aspx?nav=now"">{0}</li>", GetGlobalResourceObject("uWiMPStrings", "now"))
        markup += "</ul>"

        litChannelGroups.Text = markup

    End Sub

    Private Sub LoadTVGuide()

        Dim idGroup As Integer

        For Each Group As ChannelGroup In uWiMP.TVServer.ChannelGroups.GetChannelGroups
            If Group.IdGroup = _channelGroup Then
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
                cssClass = "rounded-corners midgrey largeblack record"
            Else
                cssClass = "rounded-corners lightgrey largeblack record"
            End If

            Dim channelImage As New Image With {.AlternateText = channel.DisplayName, .ImageUrl = String.Format("../../TVLogos/{0}.png", channel.DisplayName), .Width = 40}
            cell.Controls.Add(channelImage)
            row.Cells.Add(cell)

            Dim programs As List(Of Program) = uWiMP.TVServer.Programs.GetProgramsByChannel(channel.IdChannel, _startTime, _period)
            Dim mins As Integer
            Dim minsleft As Integer = MAXMINUTES

            If programs.Count >= 1 Then
                rows = rows + 1
                For Each p As Program In programs

                    Dim pc As TableCell = CreateProgramCell(p, channel)

                    If uWiMP.TVServer.Schedules.IsProgramScheduled(p) Then
                        pc.CssClass = "rounded-corners red largewhite record"
                    Else
                        pc.CssClass = cssClass
                    End If

                    If (p.StartTime > _startTime.AddMinutes(_period * 60)) Then
                        Exit For
                    ElseIf (p.StartTime < _startTime) And (p.EndTime > _startTime.AddMinutes(_period * 60)) Then
                        mins = minsleft
                    ElseIf (p.StartTime < _startTime) And (p.EndTime < _startTime.AddMinutes(_period * 60)) Then
                        mins = (p.EndTime - _startTime).TotalMinutes
                    ElseIf (p.StartTime > _startTime) And (p.EndTime < _startTime.AddMinutes(_period * 60)) Then
                        mins = (p.EndTime - p.StartTime).TotalMinutes
                    ElseIf (p.StartTime > _startTime) And (p.EndTime > _startTime.AddMinutes(_period * 60)) Then
                        mins = (_startTime.AddMinutes(_period * 60) - p.StartTime).TotalMinutes
                    End If

                    minsleft -= mins

                    pc.Width = Unit.Percentage((mins / (_period * 60)) * 100)
                    pc.Height = Unit.Pixel(40)

                    row.Cells.Add(pc)
                Next

                table.Rows.Add(row)
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

            hh = _startTime.Hour
            mm = _startTime.Minute
            Dim text As String = String.Empty
            If (hh + i) = 0 Or (hh + i) = 24 Then
                text = "12am"
            ElseIf (hh + i) = 12 Then
                text = "12pm"
            ElseIf (hh + i) < 12 Then
                text = String.Format("{0}am", (hh + i).ToString)
            ElseIf (hh + i) > 12 AndAlso (hh + i) < 24 Then
                text = String.Format("{0}pm", (hh + i - 12).ToString)
            ElseIf (hh + i) > 24 Then
                text = String.Format("{0}am", (hh + i - 24).ToString)
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

        Dim title As String = p.Title
        Dim description As String = p.Description
        Dim timetext As String = p.StartTime.ToShortTimeString & " - " & p.EndTime.ToShortTimeString

        If title Is Nothing Then title = GetGlobalResourceObject("uWiMPStrings", "unknown_title")
        If description Is Nothing Then description = GetGlobalResourceObject("uWiMPStrings", "description_not_found")

        c.Attributes("title") = String.Format("{0}<br>{1}", timetext, title)
        c.Attributes("pid") = p.IdProgram
        c.Attributes("rel") = "#record"
        c.ID = p.IdProgram
        Return (c)

    End Function

#End Region

#Region "TV Guide Search"

    Private Sub SearchTVGuide(ByVal groupID As String, ByVal genre As String, ByVal search As String, ByVal SearchDesc As Boolean)

        Dim matchedPrograms As List(Of Program) = uWiMP.TVServer.Programs.SearchPrograms(groupID, genre, search, SearchDesc, "tv")

        Dim alternate As Boolean = True
        Dim cssClass As String = String.Empty

        For Each p As Program In matchedPrograms

            Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(p.IdChannel)
            Dim table As New Table With {.Width = Unit.Percentage(100)}
            Dim row As New TableRow
            Dim cell As New TableCell With {.Width = 40}

            If alternate Then
                cssClass = "rounded-corners midgrey largeblack record"
            Else
                cssClass = "rounded-corners lightgrey largeblack record"
            End If

            Dim channelImage As New Image With {.AlternateText = channel.DisplayName, .ImageUrl = String.Format("../../TVLogos/{0}.png", channel.DisplayName), .Width = 40}
            cell.Controls.Add(channelImage)
            row.Cells.Add(cell)

            Dim pc As TableCell = CreateProgramCell(p, channel)

            If uWiMP.TVServer.Schedules.IsProgramScheduled(p) Then
                pc.CssClass = "rounded-corners red largewhite record"
            Else
                pc.CssClass = cssClass
            End If

            pc.Width = Unit.Percentage(40)
            pc.Height = Unit.Pixel(40)

            row.Cells.Add(pc)

            table.Rows.Add(row)
            'ph2.Controls.Add(table)

            If alternate Then
                alternate = False
            Else
                alternate = True
            End If

        Next

    End Sub

#End Region

#Region "TV Schedules"

    Private Sub BuildTVScheduleTable()

    End Sub

#End Region


End Class
