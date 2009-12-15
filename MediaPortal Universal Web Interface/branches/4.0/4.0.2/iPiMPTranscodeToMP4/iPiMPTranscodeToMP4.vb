Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Collections.Specialized
Imports System.Configuration

Imports TvControl
Imports TvEngine
Imports TvEngine.Events
Imports TvEngine.Interfaces
Imports TvLibrary
Imports TvLibrary.Log
Imports TvLibrary.Interfaces
Imports TvDatabase

Namespace TVEngine

    Public Class iPiMPTranscodeToMP4
        Implements ITvServerPlugin

        Const DEFAULT_DELETE = True
        Const DEFAULT_TRANSCODE = True
        Const DEFAULT_STARTTIME = "01:00"
        Const DEFAULT_VIDEO = "256"
        Const DEFAULT_AUDIO = "128"
        Const DEFAULT_SAVEPATH = "C:\"
        Const DEFAULT_PROGRAM = "C:\Program Files\iPiMP\Utils\ffmpeg.exe"
        Const DEFAULT_PARAMS = "-threads 4 -re -vcodec libx264 -s 480x272 -flags +loop -cmp +chroma -deblockalpha 0 -deblockbeta 0 -crf 24 -bt {0}k -refs 1 -coder 0 -me_method umh -me_range 16 -subq 5 -partitions +parti4x4+parti8x8+partp8x8 -g 250 -keyint_min 25 -level 30 -qmin 10 -qmax 51 -trellis 2 -sc_threshold 40 -i_qfactor 0.71 -acodec libfaac -ab {1}k -ar 48000 -ac 2 -async 2"
        Const DEFAULT_PARAMS2 = "-ss 400 -vcodec png -vframes 1 -an -f rawvideo -s 88x50"

        Shared _transcode As Boolean = DEFAULT_TRANSCODE
        Shared _delete As Boolean = DEFAULT_DELETE
        Shared _savepath As String = DEFAULT_SAVEPATH
        Shared _program As String = DEFAULT_PROGRAM
        Shared _video As String = DEFAULT_VIDEO
        Shared _audio As String = DEFAULT_AUDIO
        Shared _start As String = DEFAULT_STARTTIME
        Shared _param As String = DEFAULT_PARAMS
        Shared _param2 As String = DEFAULT_PARAMS2

        Private Shared appSettings As NameValueCollection = ConfigurationManager.AppSettings

        Dim thread As Thread

        Public ReadOnly Property Name() As String Implements ITvServerPlugin.Name
            Get
                Return "iPiMPTranscodeToMP4"
            End Get
        End Property

        Public ReadOnly Property Version() As String Implements ITvServerPlugin.Version
            Get
                Return "4.0.2"
            End Get
        End Property

        Public ReadOnly Property Author() As String Implements ITvServerPlugin.Author
            Get
                Return "Cheezey"
            End Get
        End Property

        Public ReadOnly Property MasterOnly() As Boolean Implements ITvServerPlugin.MasterOnly
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Setup() As Global.SetupTv.SectionSettings Implements ITvServerPlugin.Setup
            Get
                Return New SetupTv.Sections.iPiMPTranscodeToMP4Setup
            End Get
        End Property

        Friend Shared Sub LoadSettings()

            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            Try
                Dim layer As TvBusinessLayer = New TvBusinessLayer()

                _delete = Convert.ToBoolean(layer.GetSetting("iPiMPTranscodeToMP4_Delete", DEFAULT_DELETE.ToString).Value)
                _transcode = Convert.ToBoolean(layer.GetSetting("iPiMPTranscodeToMP4_TranscodeNow", DEFAULT_TRANSCODE.ToString).Value)
                _savepath = layer.GetSetting("iPiMPTranscodeToMP4_SavePath", DEFAULT_SAVEPATH.ToString).Value
                _program = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegPath", DEFAULT_PROGRAM).Value
                _video = layer.GetSetting("iPiMPTranscodeToMP4_VideoBitrate", DEFAULT_VIDEO).Value
                _audio = layer.GetSetting("iPiMPTranscodeToMP4_AudioBitrate", DEFAULT_AUDIO).Value
                _start = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeTime", DEFAULT_STARTTIME).Value
                _param = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegParam", DEFAULT_PARAMS).Value
                _param2 = DEFAULT_PARAMS2

            Catch ex As Exception

                _delete = DEFAULT_DELETE
                _transcode = DEFAULT_TRANSCODE
                _savepath = DEFAULT_SAVEPATH
                _program = DEFAULT_PROGRAM
                _video = DEFAULT_VIDEO
                _audio = DEFAULT_AUDIO
                _start = DEFAULT_STARTTIME
                _param = DEFAULT_PARAMS
                _param2 = DEFAULT_PARAMS2

                Log.Error("plugin: iPiMPTranscodeToMP4 - LoadSettings(): {0}", ex.Message)

            End Try

        End Sub

        Public Sub Start(ByVal controller As IController) Implements ITvServerPlugin.Start

            LoadSettings()

            If _delete = True Then Watch_RecordedFolders()

            Dim events As ITvServerEvent = GlobalServiceProvider.Instance.Get(Of ITvServerEvent)()
            AddHandler events.OnTvServerEvent, New TvServerEventHandler(AddressOf iPiMP_OnTvServerEvent)

            'If _start.ToLower <> "never" Then
            If Mid(_start, 3, 1) <> ":" Then
                Try
                    thread = New Thread(AddressOf Me.ScheduledTranscode)
                    thread.Start()
                Catch ex As Exception
                    Log.Error("plugin: iPiMPTranscodeToMP4 - transcode thread start error : {0}", ex.Message)
                End Try
            End If


        End Sub

        Public Sub MyStop() Implements ITvServerPlugin.Stop

            Dim events As ITvServerEvent = GlobalServiceProvider.Instance.Get(Of ITvServerEvent)()
            RemoveHandler events.OnTvServerEvent, New TvServerEventHandler(AddressOf iPiMP_OnTvServerEvent)

            thread.Abort()

            Log.Info("plugin: iPiMPTranscodeToMP4 stopped")

        End Sub

        Private Sub iPiMP_OnTvServerEvent(ByVal sender As Object, ByVal eventArgs As EventArgs)

            Dim _params As String
            Try
                Dim tvEvent As TvServerEventArgs = CType(eventArgs, TvServerEventArgs)

                If tvEvent.EventType = TvServerEventType.RecordingEnded Then

                    If _transcode Then

                        'Dim channel As Channel = TvDatabase.Channel.Retrieve(tvEvent.Recording.IdChannel)
                        'Screenshot
                        _params = ProcessParameters(tvEvent.Recording.FileName, ".png")
                        LaunchProcess(_params)
                        Log.Info("plugin: iPiMPTranscodeToMP4 - screenshot " & tvEvent.Recording.Title)
                        'Transcode
                        _params = ProcessParameters(tvEvent.Recording.FileName, ".mp4")
                        LaunchProcess(_params)
                        Log.Info("plugin: iPiMPTranscodeToMP4 - transcoded " & tvEvent.Recording.Title)

                        'ElseIf _start.ToLower <> "never" Then
                    ElseIf Mid(_start, 3, 1) <> ":" Then
                        Dim path As String = _savepath & "\transcodeme.txt"
                        Dim sw As StreamWriter
                        If File.Exists(path) = False Then
                            sw = File.CreateText(path)
                            sw.WriteLine(tvEvent.Recording.FileName)
                            sw.Flush()
                            sw.Close()
                        Else
                            sw = File.AppendText(path)
                            sw.WriteLine(tvEvent.Recording.FileName)
                            sw.Flush()
                            sw.Close()
                        End If
                        Log.Info("plugin: iPiMPTranscodeToMP4 - scheduled " & tvEvent.Recording.Title)
                    End If

                End If

            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - iPiMP_OnTvServerEvent(): {0}", ex.Message)
            End Try

        End Sub

        Private Sub LaunchProcess(ByVal _params As String)

            Try
                Dim process As Process = New Process
                process.StartInfo.FileName = "" & _program & ""
                process.StartInfo.Arguments = _params
                process.StartInfo.WorkingDirectory = _savepath
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                Log.Info("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", process.StartInfo.FileName.ToString)
                Log.Info("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", process.StartInfo.Arguments.ToString)
                process.Start()
                Do Until process.HasExited
                    System.Threading.Thread.Sleep(1000)
                Loop

                Log.Info("plugin: iPiMPTranscodeToMP4 - process started")
            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - LaunchProcess(): {0}", ex.Message)
            End Try

        End Sub

        Private Function ProcessParameters(ByVal filename As String, ByVal extension As String) As String

            Dim output As String = ""
            Dim tmpParams As String = ""

            Select Case extension.ToLower
                Case ".mp4"
                    tmpParams = " " & String.Format(_param, _video, _audio) & " "
                Case ".png"
                    tmpParams = " " & _param2 & " "
                Case Else
                    Log.Error("plugin: iPiMPTranscodeToMP4 - ProcessParameters(): Unknown extension")
            End Select

            Try
                output = "-i " & Chr(34) & filename & Chr(34) & tmpParams & Chr(34) & _savepath & "\" & Path.GetFileNameWithoutExtension(filename) & extension & Chr(34)
            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - ProcessParameters(): {0}", ex.Message)
            End Try

            Return output

        End Function

        Private Sub Watch_RecordedFolders()

            Dim watchFolders As New ArrayList
            Dim watchFolder As String
            Dim used As Boolean = False

            Dim cards As IList = TvDatabase.Card.ListAll

            Dim card As Card

            For Each card In cards

                Dim cardPath As String = card.RecordingFolder.ToString

                For Each watchFolder In watchFolders
                    If watchFolder = cardPath Then used = True
                Next

                If Not used Then
                    Try
                        Dim fsWatcher As New FileSystemWatcher
                        fsWatcher.Path = cardPath
                        fsWatcher.IncludeSubdirectories = True
                        fsWatcher.Filter = "*.xml"
                        If Directory.Exists(cardPath) Then
                            AddHandler fsWatcher.Deleted, AddressOf FileDeleted
                            fsWatcher.EnableRaisingEvents = True
                            watchFolders.Add(cardPath)
                            Log.Info("plugin: iPiMPTranscodeToMP4 - watching: {0}", cardPath)
                        End If
                    Catch ex As Exception
                        Log.Error("plugin: iPiMPTranscodeToMP4 - watcher setup failed: {0}", ex.Message)
                    End Try
                End If

                used = False

            Next

        End Sub

        Private Shared Sub FileDeleted(ByVal source As Object, ByVal e As FileSystemEventArgs)

            Dim mp4File As String = _savepath & "\" & Path.GetFileNameWithoutExtension(e.Name) & ".mp4"
            Dim pngFile As String = _savepath & "\" & Path.GetFileNameWithoutExtension(e.Name) & ".png"

            Log.Info("plugin: iPiMPTranscodeToMP4 - file deletion: {0}", e.FullPath)

            If File.Exists(mp4File) Then
                Try
                    File.Delete(mp4File)
                    Log.Info("plugin: iPiMPTranscodeToMP4 - deleted: {0}", mp4File)
                Catch ex As Exception
                    Log.Error("plugin: iPiMPTranscodeToMP4 - deletion failed: {0}", ex.Message)
                End Try
            Else
                Log.Info("plugin: iPiMPTranscodeToMP4 - no MP4 file: {0}", mp4File)
            End If

            If File.Exists(pngFile) Then
                Try
                    File.Delete(pngFile)
                    Log.Info("plugin: iPiMPTranscodeToMP4 - deleted: {0}", pngFile)
                Catch ex As Exception
                    Log.Error("plugin: iPiMPTranscodeToMP4 - deletion failed: {0}", ex.Message)
                End Try
            Else
                Log.Info("plugin: iPiMPTranscodeToMP4 - no PNG file: {0}", pngFile)
            End If

        End Sub

        Private Sub ScheduledTranscode()
            Dim _params As String
            Dim path As String = _savepath & "\transcodeme.txt"
            Dim tmpPath As String = _savepath & "\transcode_working.txt"
            Dim recFile As String
            Dim sr As StreamReader
            Dim finished As Boolean = False
            Dim sleep As Integer = 10
            Dim hh, mm As String
            Dim xx() As String
            xx = Split(_start, ":")
            hh = xx(0)
            mm = xx(1)

            Try
                Do Until finished
                    If Now.ToString("HH") = hh Then
                        Log.Info("plugin: iPiMPTranscodeToMP4 - transcode thread time to work : " & Now.ToString("HH") & "=" & hh)
                        If File.Exists(path) Then
                            File.Copy(path, tmpPath, True)
                            File.Delete(path)
                            sr = File.OpenText(tmpPath)
                            Do Until sr.EndOfStream
                                recFile = sr.ReadLine()
                                If File.Exists(recFile) Then
                                    'Screenshot
                                    _params = ProcessParameters(recFile, ".png")
                                    Log.Info("plugin: iPiMPTranscodeToMP4 - screenshot " & recFile)
                                    LaunchProcess(_params)
                                    'Transcode
                                    _params = ProcessParameters(recFile, ".mp4")
                                    Log.Info("plugin: iPiMPTranscodeToMP4 - transcoded " & recFile)
                                    LaunchProcess(_params)
                                    'Transcode(recFile)
                                Else
                                    Log.Info("plugin: iPiMPTranscodeToMP4 - did not transcode {0} as the file was missing (already deleted?)" & recFile)
                                End If
                            Loop
                            sr.Close()
                            File.Delete(tmpPath)
                        End If
                    Else
                        Log.Info("plugin: iPiMPTranscodeToMP4 - transcode thread time to sleep : " & Now.ToString("HH") & "=" & hh)
                    End If
                    thread.Sleep(sleep * 60 * 1000)
                Loop
            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - scheduled transcode failed: {0}", ex.Message)
            End Try

            Log.Info("plugin: iPiMPTranscodeToMP4 - transcoding thread stopped.")

        End Sub

        Private Sub Transcode(ByVal recfile As String)

            Dim mp4File As String
            Dim pngFile As String
            Dim process As Process

            If File.Exists(recfile) Then
                Try
                    mp4File = _savepath & "\" & Path.GetFileNameWithoutExtension(recfile) & ".mp4"
                    pngFile = _savepath & "\" & Path.GetFileNameWithoutExtension(recfile) & ".png"

                    process = New Process
                    process.StartInfo.FileName = "" & _program & ""
                    process.StartInfo.Arguments = "-i """ & recfile & "" & _param2 & "" & pngFile & ""
                    process.StartInfo.WorkingDirectory = _savepath
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    Log.Info("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", process.StartInfo.FileName.ToString)
                    Log.Info("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", process.StartInfo.Arguments.ToString)
                    process.Start()
                    Do Until process.HasExited
                        System.Threading.Thread.Sleep(1000)
                    Loop

                    process = New Process
                    process.StartInfo.FileName = "" & _program & ""
                    process.StartInfo.Arguments = "-i """ & recfile & """ " & String.Format(_param, _video, _audio) & " """ & mp4File & ""
                    process.StartInfo.WorkingDirectory = _savepath
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    Log.Info("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", process.StartInfo.FileName.ToString)
                    Log.Info("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", process.StartInfo.Arguments.ToString)
                    process.Start()
                    process.WaitForExit()
                    Do Until process.HasExited
                        System.Threading.Thread.Sleep(1000)
                    Loop
                Catch ex As Exception
                    Log.Error("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", ex.Message)
                End Try
            End If
        End Sub

    End Class

End Namespace