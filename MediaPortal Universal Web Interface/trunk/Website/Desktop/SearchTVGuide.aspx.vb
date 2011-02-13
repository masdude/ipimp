' 
'   Copyright (C) 2008-2010 Martin van der Boon
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

Public Class SearchTVGuide1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim searchText As String = Request.QueryString("search")
        Dim searchDescription As Boolean = CBool(Request.QueryString("desc"))

        Dim matchedPrograms As List(Of Program) = uWiMP.TVServer.Programs.SearchPrograms("-1", "all", searchText, searchDescription, "tv")

        Dim alternate As Boolean = True
        Dim cssClass As String = String.Empty

        For Each p As Program In matchedPrograms

            Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(p.IdChannel)
            Dim table As New Table With {.Width = Unit.Percentage(100)}
            Dim row As New TableRow
            Dim cell As New TableCell With {.Width = 40}

            If alternate Then
                cssClass = "rounded-corners midgrey largeblack record2"
            Else
                cssClass = "rounded-corners lightgrey largeblack record2"
            End If

            Dim channelImage As New Image With {.AlternateText = channel.Name, .ImageUrl = String.Format("../../TVLogos/{0}.png", channel.Name), .Width = 40}
            cell.Controls.Add(channelImage)
            row.Cells.Add(cell)

            Dim pc As TableCell = CreateProgramCell(p, channel)

            If uWiMP.TVServer.Schedules.IsProgramScheduled(p) Then
                pc.CssClass = "rounded-corners red largewhite record2"
            Else
                pc.CssClass = cssClass
            End If

            pc.Width = Unit.Percentage(40)
            pc.Height = Unit.Pixel(40)

            row.Cells.Add(pc)

            table.Rows.Add(row)
            SearchResults.Controls.Add(table)

            If alternate Then
                alternate = False
            Else
                alternate = True
            End If

        Next

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
        c.Attributes("rel") = "#record2"
        c.ID = p.IdProgram
        Return (c)

    End Function

End Class