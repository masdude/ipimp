Imports System
Imports System.IO
Imports System.Threading
Imports TvDatabase

Namespace uWiMP.TVServer

    Public Class Transcode

        ' property to indicate if task has run at least once.
        Private _firstRunComplete As Boolean = False
        Public ReadOnly Property firstRunComplete() As Boolean
            Get
                Return _firstRunComplete
            End Get
        End Property

        ' property to indicate iftask is running.
        Private _running As Boolean = False
        Public ReadOnly Property Running() As Boolean
            Get
                Return _running
            End Get
        End Property

        ' property containing the transcoded recording id 
        Private _recid As Integer = 0
        Public Property RecordingID() As String
            Get
                Return _recid
            End Get
            Set(ByVal value As String)
                _recid = value
            End Set
        End Property

        'property to indicate whether the last task succeeded.
        Public _lastTaskSuccess As Boolean = True
        Public ReadOnly Property LastTaskSuccess() As Boolean
            Get
                If _lastFinishTime = DateTime.MinValue Then
                    Throw New InvalidOperationException("The task has never completed.")
                End If
                Return _lastTaskSuccess
            End Get
        End Property

        'store any exception generated during the task.
        Private _exceptionOccured As Exception = Nothing
        Public ReadOnly Property ExceptionOccured() As Exception
            Get
                Return _exceptionOccured
            End Get
        End Property

        Private _lastStartTime As DateTime = DateTime.MinValue
        Public ReadOnly Property LastStartTime() As DateTime
            Get
                If _lastStartTime = DateTime.MinValue Then
                    Throw New InvalidOperationException("The task has never started.")
                End If
                Return _lastStartTime
            End Get
        End Property

        Private _lastFinishTime As DateTime = DateTime.MinValue
        Public ReadOnly Property LastFinishTime() As DateTime
            Get
                If _lastFinishTime = DateTime.MinValue Then
                    Throw New InvalidOperationException("The task has never completed.")
                End If
                Return _lastFinishTime
            End Get
        End Property

        ' Start the task
        Public Sub RunTask()

            ' Only one thread is allowed to enter here.
            SyncLock Me
                If Not _running Then
                    _running = True
                    _lastStartTime = DateTime.Now
                    Dim t As New Thread(New ThreadStart(AddressOf Me.DoWork))
                    t.Start()
                Else
                    Throw New InvalidOperationException("The task is already running!")
                End If
            End SyncLock
        End Sub

        Public Sub DoWork()
            Try
                Dim layer As TvBusinessLayer = New TvBusinessLayer()
                Dim MP4Path As String = layer.GetSetting("iPiMPTranscodeToMP4_SavePath").Value
                Dim numVideo As String = layer.GetSetting("iPiMPTranscodeToMP4_VideoBitrate").Value
                Dim numAudio As String = layer.GetSetting("iPiMPTranscodeToMP4_AudioBitrate").Value
                Dim FFPath As String = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegPath").Value
                Dim FFParam As String = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegParam").Value

                Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(_recid))
                Dim recFile As String = recording.FileName
                Dim mp4File As String = MP4Path & "\" & Path.GetFileNameWithoutExtension(recording.FileName) & ".mp4"
                Dim pngFile As String = MP4Path & "\" & Path.GetFileNameWithoutExtension(recording.FileName) & ".png"

                Dim process As New Process
                process.StartInfo.FileName = "" & FFPath & ""
                process.StartInfo.Arguments = "-i """ & recFile & """ -ss 400 -vcodec png -vframes 1 -an -f rawvideo -s 88x50 -y """ & pngFile & ""
                process.StartInfo.WorkingDirectory = MP4Path
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                process.StartInfo.UseShellExecute = False
                process.StartInfo.RedirectStandardOutput = True
                process.Start()
                process.WaitForExit()

                process.StartInfo.FileName = "" & FFPath & ""
                process.StartInfo.Arguments = "-i """ & recFile & """ " & String.Format(FFParam, numVideo, numAudio) & " """ & mp4File & ""
                process.StartInfo.WorkingDirectory = MP4Path
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                process.StartInfo.UseShellExecute = False
                process.StartInfo.RedirectStandardOutput = True
                process.Start()
                process.WaitForExit()

                If process.ExitCode = 0 Then
                    ' Set Success property.
                    _lastTaskSuccess = True
                Else
                    ' Task Failed.
                    _lastTaskSuccess = False
                    If File.Exists(mp4File) Then File.Delete(mp4File)
                End If

            Catch e As Exception
                ' Task Failed.
                _lastTaskSuccess = False
                _exceptionOccured = e
            Finally
                _running = False
                _lastFinishTime = DateTime.Now
                If Not _firstRunComplete Then
                    _firstRunComplete = True
                End If
            End Try
        End Sub

        Public Shared Function GetProgress() As String

            Dim processes As Process() = Process.GetProcessesByName("ffmpeg")
            Dim ffProcess As Process
            Dim output As String = ""

            For Each ffProcess In processes
                output = ffProcess.StartTime.ToString
            Next

            ffProcess = Nothing
            processes = Nothing

            Return output

        End Function

        Public Shared Function IsFFMpegRunning() As Boolean

            Dim processes As Process() = Process.GetProcessesByName("ffmpeg")
            If processes.Length > 0 Then
                Return True
            Else
                Return False
            End If

        End Function

    End Class

End Namespace

