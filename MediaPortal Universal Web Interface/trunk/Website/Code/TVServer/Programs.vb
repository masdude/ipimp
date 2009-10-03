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

        Public Shared Function GetProgramsByGroup(ByVal idGroup As Integer) As List(Of Program)

            Dim programs As New List(Of Program)
            Dim allChannels As List(Of Channel) = Channels.GetChannelsByGroupId(idGroup)
            Dim layer As New TvBusinessLayer
            For Each c As Channel In allChannels
                For Each p As Program In GetProgramsByChannel(c.IdChannel)
                    programs.Add(p)
                Next
            Next

            Return programs

        End Function

    End Class

    Public Class ProgramStartTimeComparer
        Implements IComparer(Of Program)
        Public Overridable Overloads Function Compare(ByVal x As Program, ByVal y As Program) As Integer Implements IComparer(Of Program).Compare
            Return x.StartTime.CompareTo(y.StartTime)
        End Function
    End Class

End Namespace
