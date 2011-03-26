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
Imports System.IO
Imports TvControl

Namespace uWiMP.TVServer

    Public Class Recordings

        ''' <summary>
        ''' Recording periods.
        ''' </summary>
        ''' <remarks></remarks>
        Enum RecordingType
            Once = 0
            AtThisTimeEveryDay = 1
            AtThisTimeEveryWeek = 2
            EachTimeOnThisChannel = 3
            EachTimeOnAnyChannel = 4
            AtThisTimeAtWeekends = 5
            AtThisTimeOnWeekdays = 6
        End Enum

        ''' <summary>
        ''' Recording retention periods.
        ''' </summary>
        ''' <remarks></remarks>
        Enum KeepType
            UntilWatched = 0
            UntilSpaceNeeded = 1
            Always = 3
            OneWeek = 10
            OneMonth = 11
            OneYear = 12
        End Enum

        ''' <summary>
        ''' Retrieves all recordings.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllRecordings() As List(Of Recording)

            Return Recording.ListAll

        End Function

        ''' <summary>
        ''' Retrieves a count of the number of recordings.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingCount() As Integer

            Return Recording.ListAll.Count

        End Function

        ''' <summary>
        ''' Retrieves a collection of recordings.
        ''' </summary>
        ''' <param name="start">The index to start the recording collection from.</param>
        ''' <param name="num">The size of the required collection.</param>
        ''' <param name="sort">The sort option to order the collection. Valid sort options are title, channel, id, genre and date.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordings(ByVal start As Integer, ByVal num As Integer, ByVal sort As String) As List(Of Recording)

            Dim recs As List(Of Recording) = Recording.ListAll

            Select Case sort.ToLower
                Case "title"
                    recs.Sort(New uWiMP.TVServer.RecordingTitleComparer)
                Case "channel"
                    recs.Sort(New uWiMP.TVServer.RecordingidChannelComparer)
                Case "id"
                    recs.Sort(New uWiMP.TVServer.RecordingidRecordingComparer)
                Case "genre"
                    recs.Sort(New uWiMP.TVServer.RecordingGenreComparer)
                Case "date"
                    recs.Sort(New uWiMP.TVServer.RecordingStartTimeComparerDesc)
            End Select

            Return recs.GetRange(start, num)

        End Function

        ''' <summary>
        ''' Retrieves a specific recording.
        ''' </summary>
        ''' <param name="idRecording">The id number of the recording.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingById(ByVal idRecording As Integer) As Recording

            Return Recording.Retrieve(idRecording)

        End Function

        ''' <summary>
        ''' Retrieves a specific recording.
        ''' </summary>
        ''' <param name="fileName">The filename number of the require recording.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingByFilename(ByVal fileName As String) As Recording

            Dim recs As List(Of Recording) = Recording.ListAll

            For Each r As Recording In recs
                If r.FileName.ToLower = fileName.ToLower Then Return r
            Next

            Return Nothing

        End Function

        ''' <summary>
        ''' Retrieves a collection of recordings.
        ''' </summary>
        ''' <param name="idChannel">The id of the channel for which recordings should be returned.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingsForChannel(ByVal idChannel As Integer) As List(Of Recording)

            Dim recs As List(Of Recording) = Recording.ListAll
            Dim channelRecs As New List(Of Recording)

            For Each r As Recording In recs
                If r.IdChannel = idChannel Then channelRecs.Add(r)
            Next

            Return channelRecs

        End Function

        
        ''' <summary>
        ''' Retrieves a count of the number of recordings.
        ''' </summary>
        ''' <param name="rootPath">The root directory path for which the count should be returned.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingCountForDrive(ByVal rootPath As String) As Integer

            Dim recs As List(Of Recording) = Recording.ListAll
            Dim driveRecs As New List(Of Recording)

            For Each r As Recording In recs
                If Path.GetPathRoot(r.FileName).ToLower = rootPath.ToLower Then driveRecs.Add(r)
            Next

            Return driveRecs.Count

        End Function

        ''' <summary>
        ''' Retrieves a collection of recordings.
        ''' </summary>
        ''' <param name="start">The index to start the recording collection from.</param>
        ''' <param name="num">The size of the required collection.</param>
        ''' <param name="sort">The sort option to order the collection. Valid sort options are title, channel, id, genre and date.</param>
        ''' <param name="rootPath">The root directory path for which recordings should be returned.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingsForDrive(ByVal start As Integer, ByVal num As Integer, ByVal sort As String, ByVal rootPath As String) As List(Of Recording)

            Dim recs As List(Of Recording) = Recording.ListAll
            Dim driveRecs As New List(Of Recording)

            For Each r As Recording In recs
                If Path.GetPathRoot(r.FileName).ToLower = rootPath.ToLower Then driveRecs.Add(r)
            Next

            Select Case sort.ToLower
                Case "title"
                    driveRecs.Sort(New uWiMP.TVServer.RecordingTitleComparer)
                Case "channel"
                    driveRecs.Sort(New uWiMP.TVServer.RecordingidChannelComparer)
                Case "id"
                    driveRecs.Sort(New uWiMP.TVServer.RecordingidRecordingComparer)
                Case "genre"
                    driveRecs.Sort(New uWiMP.TVServer.RecordingGenreComparer)
                Case "date"
                    driveRecs.Sort(New uWiMP.TVServer.RecordingStartTimeComparerDesc)
            End Select

            Return driveRecs.GetRange(start, num)

        End Function

        ''' <summary>
        ''' Retrieves a collection of recordings.
        ''' </summary>
        ''' <param name="genre">The genre of recordings which should be returned.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRecordingsForGenre(ByVal genre As String) As List(Of Recording)

            Dim recs As List(Of Recording) = Recording.ListAll
            Dim genreRecs As New List(Of Recording)

            For Each r As Recording In recs
                If r.Genre.ToLower = genre.ToLower Then genreRecs.Add(r)
            Next

            Return genreRecs

        End Function

        ''' <summary>
        ''' Records a TV program.
        ''' </summary>
        ''' <param name="idProgram">The id of the program to record.</param>
        ''' <param name="recType">The recording period. (uWiMP.TVServer.Recordings.RecordingType)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RecordProgramById(ByVal idProgram As Integer, ByVal recType As RecordingType) As Boolean

            Try

                Dim layer As New TvBusinessLayer
                Dim p As Program = Program.Retrieve(idProgram)
                Dim s As Schedule = New Schedule(p.IdChannel, p.Title, p.StartTime, p.EndTime)

                s.ScheduleType = recType
                s.PreRecordInterval = Int32.Parse(layer.GetSetting("preRecordInterval", "5").Value)
                s.PostRecordInterval = Int32.Parse(layer.GetSetting("postRecordInterval", "5").Value)
                s.Quality = 73
                s.MaxAirings = 2147483647
                s.Persist()
                RemoteControl.Instance.OnNewSchedule()

                Return True

            Catch ex As Exception

                Return False

            End Try

        End Function

        ''' <summary>
        ''' Records a manual schedule.
        ''' </summary>
        ''' <param name="channelID">The id of the channel to record.</param>
        ''' <param name="schedName">A text label for the manual recording.</param>
        ''' <param name="startTime">The start time of the recording.</param>
        ''' <param name="endTime">The end time of the recording.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RecordProgram(ByVal channelID As Integer, ByVal schedName As String, ByVal startTime As DateTime, ByVal endTime As DateTime) As Boolean

            Try

                Dim layer As New TvBusinessLayer
                Dim s As Schedule = New Schedule(channelID, schedName, startTime, endTime)

                s.ScheduleType = RecordingType.Once
                s.PreRecordInterval = Int32.Parse(layer.GetSetting("preRecordInterval", "5").Value)
                s.PostRecordInterval = Int32.Parse(layer.GetSetting("postRecordInterval", "5").Value)
                s.Quality = 73
                s.MaxAirings = 2147483647
                s.Persist()
                RemoteControl.Instance.OnNewSchedule()

                Return True

            Catch ex As Exception

                Return False

            End Try

        End Function

        ''' <summary>
        ''' Deletes a recording and all its associated files.
        ''' </summary>
        ''' <param name="idRecording">The id number of the recording.</param>
        ''' <returns></returns>
        ''' <remarks>Deletes the following files:
        ''' recording description file (xml)
        ''' recording native file (ts)
        ''' recording transcoded file (mp4)
        ''' recording trascoded thumbnail file (jpg)
        ''' </remarks>
        Public Shared Function DeleteRecordingById(ByVal idRecording As Integer) As Boolean

            Try
                Dim layer As New TvBusinessLayer
                Dim r As Recording = Recording.Retrieve(idRecording)
                Dim recFile As String = r.FileName
                Dim xmlFile As String = Path.GetDirectoryName(recFile) & "\" & Path.GetFileNameWithoutExtension(recFile) & ".xml"
                Dim mp4File As String = Utilities.GetAppConfig("MP4PATH") & "\" & Path.GetFileNameWithoutExtension(recFile) & ".mp4"
                Dim jpgFile As String = Utilities.GetAppConfig("MP4PATH") & "\" & Path.GetFileNameWithoutExtension(recFile) & ".jpg"

                If File.Exists(xmlFile) Then
                    File.Delete(xmlFile)
                End If

                If File.Exists(recFile) Then
                    File.Delete(recFile)
                End If

                If File.Exists(mp4File) Then
                    File.Delete(mp4File)
                End If

                If File.Exists(jpgFile) Then
                    File.Delete(jpgFile)
                End If

                r.Delete()

                Return True

            Catch ex As Exception

                Return False

            End Try

        End Function


        ''' <summary>
        ''' Sets the retention policy of the recording.
        ''' </summary>
        ''' <param name="idRecording">The id number of the recording.</param>
        ''' <param name="keepType">The retention period. (uWiMP.TVServer.Recordings.KeepType)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function KeepRecordingById(ByVal idRecording As Integer, ByVal keepType As KeepType) As Boolean

            Dim layer As New TvBusinessLayer
            Dim r As Recording = Recording.Retrieve(idRecording)

            If (keepType = Recordings.KeepType.UntilWatched) Or _
               (keepType = Recordings.KeepType.UntilSpaceNeeded) Or _
               (keepType = Recordings.KeepType.Always) Then
                Try
                    r.KeepUntil = keepType
                    Return True
                Catch ex As Exception
                    Return False
                End Try
            Else
                Try
                    Select Case keepType
                        Case Recordings.KeepType.OneWeek
                            r.KeepUntilDate = Now.AddDays(7)
                        Case Recordings.KeepType.OneMonth
                            r.KeepUntilDate = Now.AddDays(31)
                        Case Recordings.KeepType.OneYear
                            r.KeepUntilDate = Now.AddDays(365)
                        Case Else
                            Return False
                    End Select
                    Return True
                Catch ex As Exception
                    Return False
                End Try

            End If

        End Function

    End Class

    ''' <summary>
    ''' Sorts a list of recordings by the recording ids. (Ascending order)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RecordingidRecordingComparer
        Implements IComparer(Of Recording)
        Public Overridable Overloads Function Compare(ByVal x As Recording, ByVal y As Recording) As Integer Implements IComparer(Of Recording).Compare
            Return x.IdRecording.CompareTo(y.IdRecording)
        End Function
    End Class

    ''' <summary>
    ''' Sorts a list of recordings by the recording titles. (Ascending order)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RecordingTitleComparer
        Implements IComparer(Of Recording)
        Public Overridable Overloads Function Compare(ByVal x As Recording, ByVal y As Recording) As Integer Implements IComparer(Of Recording).Compare
            Return x.Title.CompareTo(y.Title)
        End Function
    End Class

    ''' <summary>
    ''' Sorts a list of recordings by the recording genres. (Ascending order)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RecordingGenreComparer
        Implements IComparer(Of Recording)
        Public Overridable Overloads Function Compare(ByVal x As Recording, ByVal y As Recording) As Integer Implements IComparer(Of Recording).Compare
            Return x.Genre.CompareTo(y.Genre)
        End Function
    End Class

    ''' <summary>
    ''' Sorts a list of recordings by the recording channel ids. (Ascending order)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RecordingidChannelComparer
        Implements IComparer(Of Recording)
        Public Overridable Overloads Function Compare(ByVal x As Recording, ByVal y As Recording) As Integer Implements IComparer(Of Recording).Compare
            Return x.IdChannel.CompareTo(y.IdChannel)
        End Function
    End Class

    ''' <summary>
    ''' Sorts a list of recordings by the recording start times. (Ascending order)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RecordingStartTimeComparer
        Implements IComparer(Of Recording)
        Public Overridable Overloads Function Compare(ByVal x As Recording, ByVal y As Recording) As Integer Implements IComparer(Of Recording).Compare
            Return x.StartTime.CompareTo(y.StartTime)
        End Function
    End Class

    ''' <summary>
    ''' Sorts a list of recordings by the recording start times. (Descending order)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RecordingStartTimeComparerDesc
        Implements IComparer(Of Recording)
        Public Overridable Overloads Function Compare(ByVal x As Recording, ByVal y As Recording) As Integer Implements IComparer(Of Recording).Compare
            Return y.StartTime.CompareTo(x.StartTime)
        End Function
    End Class

End Namespace

