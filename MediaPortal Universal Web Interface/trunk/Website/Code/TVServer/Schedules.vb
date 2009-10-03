Imports TvDatabase

Namespace uWiMP.TVServer

    Public Class Schedules

        Public Shared Function IsProgramScheduled(ByVal p As Program) As Boolean

            Dim schedules As List(Of Schedule) = Schedule.ListAll
            Dim onceScheds As List(Of Schedule) = Schedule.ListAll
            Dim seriesScheds As List(Of Schedule) = Schedule.ListAll

            onceScheds.Clear()
            seriesScheds.Clear()

            Dim s As Schedule

            For Each s In schedules
                If s.ScheduleType > 0 Then
                    seriesScheds.Add(s)
                Else
                    onceScheds.Add(s)
                End If
            Next

            If (seriesScheds.Count > 0) And (Not p Is Nothing) Then
                For Each s In seriesScheds
                    If (s.IdChannel = p.IdChannel) And (s.ProgramName.ToLower = p.Title.ToLower) Then
                        Return True
                        Exit Function
                    End If
                Next
            End If

            If (onceScheds.Count > 0) And (Not p Is Nothing) Then
                For Each s In onceScheds
                    If (s.IdChannel = p.IdChannel) And (s.ProgramName.ToLower = p.Title.ToLower) And (s.StartTime = p.StartTime) Then
                        Return True
                        Exit Function
                    End If
                Next
            End If

            Return False

        End Function

        Public Shared Function GetSchedules() As List(Of Schedule)

            Return Schedule.ListAll

        End Function

        Public Shared Function GetScheduleById(ByVal idSchedule As Integer) As Schedule

            Return Schedule.Retrieve(idSchedule)

        End Function

        Public Shared Function GetSchedulesForChannel(ByVal idChannel As Integer) As List(Of Schedule)

            Dim scheds As List(Of Schedule) = Schedule.ListAll
            Dim channelScheds As New List(Of Schedule)

            For Each s As Schedule In scheds
                If s.IdChannel = idChannel Then channelScheds.Add(s)
            Next

            Return channelScheds

        End Function

        Public Shared Function GetSchedulesForTitle(ByVal title As String) As List(Of Schedule)

            Dim scheds As List(Of Schedule) = Schedule.ListAll
            Dim titleSched As New List(Of Schedule)

            For Each s As Schedule In scheds
                If s.ProgramName.ToLower = title.ToLower Then titleSched.Add(s)
            Next
            If titleSched.Count > 1 Then titleSched.Sort(New uWiMP.TVServer.ScheduleStartTimeComparer)

            Return titleSched

        End Function

        Public Shared Function DeleteScheduleById(ByVal idSchedule As Integer) As Boolean

            Try
                Dim s As Schedule = Schedule.Retrieve(idSchedule)
                s.Delete()
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

    End Class

    Public Class ScheduleTitleComparer
        Implements IComparer(Of Schedule)
        Public Overridable Overloads Function Compare(ByVal x As Schedule, ByVal y As Schedule) As Integer Implements IComparer(Of Schedule).Compare
            Return x.ProgramName.CompareTo(y.ProgramName)
        End Function
    End Class

    Public Class ScheduleStartTimeComparer
        Implements IComparer(Of Schedule)
        Public Overridable Overloads Function Compare(ByVal x As Schedule, ByVal y As Schedule) As Integer Implements IComparer(Of Schedule).Compare
            Return x.StartTime.CompareTo(y.StartTime)
        End Function
    End Class

    Public Class ScheduleStartTimeComparerDesc
        Implements IComparer(Of Schedule)
        Public Overridable Overloads Function Compare(ByVal x As Schedule, ByVal y As Schedule) As Integer Implements IComparer(Of Schedule).Compare
            Return y.StartTime.CompareTo(x.StartTime)
        End Function
    End Class

    Public Class ScheduleChannelComparer
        Implements IComparer(Of Schedule)
        Public Overridable Overloads Function Compare(ByVal x As Schedule, ByVal y As Schedule) As Integer Implements IComparer(Of Schedule).Compare
            Return x.IdChannel.CompareTo(y.IdChannel)
        End Function
    End Class

    Public Class ScheduleScheduleTypeComparer
        Implements IComparer(Of Schedule)
        Public Overridable Overloads Function Compare(ByVal x As Schedule, ByVal y As Schedule) As Integer Implements IComparer(Of Schedule).Compare
            Return x.ScheduleType.CompareTo(y.ScheduleType)
        End Function
    End Class

End Namespace
