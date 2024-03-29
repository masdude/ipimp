﻿' 
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

Namespace uWiMP.TVServer

    Public Class Programs

        Public Shared Function GetProgramByProgramId(ByVal idProgram As Integer) As Program

            Return Program.Retrieve(idProgram)

        End Function

        Public Shared Function GetProgramByProgramName(ByVal title As String) As List(Of Program)

            Dim programs As List(Of Program) = Program.RetrieveByTitle(title)
            If programs.Count > 1 Then programs.Sort(New uWiMP.TVServer.ProgramStartTimeComparer)

            Return programs

        End Function

        Public Shared Function GetProgramByProgramNameDate(ByVal title As String, ByVal startTime As Date, ByVal endTime As Date) As Program

            Dim p As Program
            If startTime >= Now Then
                p = Program.RetrieveByTitleAndTimes(title, startTime, endTime)
                Return p
            Else
                Return Nothing
            End If

        End Function

        Public Shared Function GetProgramsByChannel(ByVal idChannel As Integer) As List(Of Program)

            Dim layer As New TvBusinessLayer
            Dim c As Channel = Channels.GetChannelByChannelId(idChannel)
            Dim programs As List(Of Program) = layer.GetPrograms(c, Now, Now.AddDays(7))

            Return programs

        End Function

        Public Shared Function GetProgramsByChannel(ByVal idChannel As Integer, ByVal hours As Integer) As List(Of Program)

            Dim layer As New TvBusinessLayer
            Dim c As Channel = Channels.GetChannelByChannelId(idChannel)
            Dim programs As List(Of Program) = layer.GetPrograms(c, Now, Now.AddHours(hours))

            Return programs

        End Function

        Public Shared Function GetProgramsByChannel(ByVal idChannel As Integer, ByVal start As DateTime, ByVal hours As Integer) As List(Of Program)

            Dim layer As New TvBusinessLayer
            Dim c As Channel = Channels.GetChannelByChannelId(idChannel)
            Dim programs As List(Of Program) = layer.GetPrograms(c, start, start.AddHours(hours))

            Return programs

        End Function

        Public Shared Function GetProgramsByGroup(ByVal idGroup As Integer, Optional ByVal isTV As Boolean = True) As List(Of Program)

            Dim programs As New List(Of Program)
            Dim allChannels As List(Of Channel)
            If isTV Then
                allChannels = Channels.GetChannelsByGroupId(idGroup)
            Else
                allChannels = RadioChannels.GetRadioChannelsByGroupId(idGroup)
            End If

            For Each c As Channel In allChannels
                For Each p As Program In GetProgramsByChannel(c.IdChannel)
                    programs.Add(p)
                Next
            Next

            Return programs

        End Function

        Public Shared Function SearchPrograms(ByVal groupID As String, ByVal genre As String, ByVal search As String, ByVal searchDesc As Boolean, ByVal TVOrRadio As String) As List(Of Program)

            Dim programs As List(Of Program) = Nothing
            If groupID = -1 Then
                programs = Program.ListAll
            Else
                programs = GetProgramsByGroup(CInt(groupID))
            End If

            Dim channel As Channel = Nothing
            Dim p As Program = Nothing
            Dim matchedPrograms As New List(Of Program)

            Dim regexPattern = "[\\\/:\*\?""'<>|] "
            Dim oRegEx As New Regex(regexPattern)
            Dim title As String = String.Empty
            Dim desc As String = String.Empty

            For Each p In programs
                If (genre.ToLower = p.Genre.ToLower) Or (genre.ToLower = "all") Then

                    title = oRegEx.Replace(p.Title, "")
                    If InStr(title.ToLower, search.ToLower) > 0 Then
                        channel = uWiMP.TVServer.Channels.GetChannelByChannelId(p.IdChannel)
                        If TVOrRadio.ToLower = "tv" And channel.IsTv Then
                            matchedPrograms.Add(p)
                        ElseIf TVOrRadio.ToLower = "radio" And channel.IsRadio Then
                            matchedPrograms.Add(p)
                        End If
                    End If

                    If searchDesc Then
                        desc = oRegEx.Replace(p.Description, "")
                        If InStr(desc.ToLower, search.ToLower) > 0 Then
                            channel = uWiMP.TVServer.Channels.GetChannelByChannelId(p.IdChannel)
                            If TVOrRadio.ToLower = "tv" And channel.IsTv Then
                                If Not matchedPrograms.Contains(p) Then matchedPrograms.Add(p)
                            ElseIf TVOrRadio.ToLower = "radio" And channel.IsRadio Then
                                If Not matchedPrograms.Contains(p) Then matchedPrograms.Add(p)
                            End If
                        End If
                    End If

                End If
            Next

            If matchedPrograms.Count > 1 Then matchedPrograms.Sort(New ProgramStartTimeComparer)

            Return matchedPrograms

        End Function

    End Class

    Public Class ProgramStartTimeComparer
        Implements IComparer(Of Program)
        Public Overridable Overloads Function Compare(ByVal x As Program, ByVal y As Program) As Integer Implements IComparer(Of Program).Compare
            Return x.StartTime.CompareTo(y.StartTime)
        End Function
    End Class

End Namespace
