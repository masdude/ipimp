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
        Const DEFAULT_TRANSCODER = "handbrake"
        Const DEFAULT_IPIMPPATH = "C:\Program Files\iPiMP\Utilities"
        Const DEFAULT_PRESET = "iPhone & iPod Touch"
        Const DEFAULT_CUSTOM = ""

        Friend Shared _transcodeNow As Boolean = DEFAULT_TRANSCODE
        Friend Shared _deleteWithRecording As Boolean = DEFAULT_DELETE
        Friend Shared _folderPath As String = DEFAULT_SAVEPATH
        Friend Shared _transcoder As String = DEFAULT_TRANSCODER
        Friend Shared _videoBitrate As String = DEFAULT_VIDEO
        Friend Shared _audioBitrate As String = DEFAULT_AUDIO
        Friend Shared _transcodeTime As String = DEFAULT_STARTTIME
        Friend Shared _preset As String = String.Empty
        Friend Shared _custom As String = String.Empty

        Private Shared _iPiMPPath As String = DEFAULT_IPIMPPATH
        Private Shared _transcoderPath As String = String.Empty
        Private Shared _mtnPath As String = String.Empty
        Private Shared appSettings As NameValueCollection = ConfigurationManager.AppSettings
        Private Shared _preInterval As Integer = 0
        Private Shared _postInterval As Integer = 0

        Dim timerThread As Thread

#Region "ITVServer"

        Public ReadOnly Property Name() As String Implements ITvServerPlugin.Name
            Get
                Return "iPiMPTranscodeToMP4"
            End Get
        End Property

        Public ReadOnly Property Version() As String Implements ITvServerPlugin.Version
            Get
                Return "4.2.0"
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
                Return New SetupTv.Sections.PluginSetup
            End Get
        End Property

#End Region

        Friend Shared Sub LoadSettings()

            appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

            Try
                Dim layer As TvBusinessLayer = New TvBusinessLayer()

                _deleteWithRecording = Convert.ToBoolean(layer.GetSetting("iPiMPTranscodeToMP4_Delete", DEFAULT_DELETE.ToString).Value)
                _transcodeNow = Convert.ToBoolean(layer.GetSetting("iPiMPTranscodeToMP4_TranscodeNow", DEFAULT_TRANSCODE.ToString).Value)
                _folderPath = layer.GetSetting("iPiMPTranscodeToMP4_SavePath", DEFAULT_SAVEPATH.ToString).Value
                _transcoder = layer.GetSetting("iPiMPTranscodeToMP4_Transcoder", DEFAULT_TRANSCODER).Value
                _iPiMPPath = layer.GetSetting("iPiMPTranscodeToMP4_iPiMPPath", DEFAULT_IPIMPPATH).Value
                _videoBitrate = layer.GetSetting("iPiMPTranscodeToMP4_VideoBitrate", DEFAULT_VIDEO).Value
                _audioBitrate = layer.GetSetting("iPiMPTranscodeToMP4_AudioBitrate", DEFAULT_AUDIO).Value
                _transcodeTime = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeTime", DEFAULT_STARTTIME).Value
                _preset = layer.GetSetting("iPiMPTranscodeToMP4_Preset", DEFAULT_PRESET).Value
                _custom = layer.GetSetting("iPiMPTranscodeToMP4_Custom", DEFAULT_CUSTOM).Value
                _preInterval = Int32.Parse(layer.GetSetting("preRecordInterval", "5").Value)
                _postInterval = Int32.Parse(layer.GetSetting("postRecordInterval", "5").Value)

            Catch ex As Exception

                _deleteWithRecording = DEFAULT_DELETE
                _transcodeNow = DEFAULT_TRANSCODE
                _folderPath = DEFAULT_SAVEPATH
                _transcoder = DEFAULT_TRANSCODER
                _iPiMPPath = DEFAULT_IPIMPPATH
                _videoBitrate = DEFAULT_VIDEO
                _audioBitrate = DEFAULT_AUDIO
                _transcodeTime = DEFAULT_STARTTIME
                _preset = DEFAULT_PRESET
                _custom = DEFAULT_CUSTOM
                _preInterval = 5
                _postInterval = 5

                Log.Error("plugin: iPiMPTranscodeToMP4 - LoadSettings(): {0}", ex.Message)

            End Try

            _mtnPath = String.Format("{0}{1}", _iPiMPPath, "\MTN\mtn.exe")

            If _transcoder.ToLower = "handbrake" Then
                _transcoderPath = String.Format("{0}\HandBrake\HandBrakeCLI.exe", _iPiMPPath)
            Else
                _transcoderPath = String.Format("{0}\FFMpeg\FFMpeg.exe", _iPiMPPath)
            End If

        End Sub

        Public Sub Start(ByVal controller As IController) Implements ITvServerPlugin.Start

            LoadSettings()

            If _deleteWithRecording = True Then Watch_RecordedFolders()

            Dim events As ITvServerEvent = GlobalServiceProvider.Instance.Get(Of ITvServerEvent)()
            AddHandler events.OnTvServerEvent, New TvServerEventHandler(AddressOf iPiMP_OnTvServerEvent)

            If Mid(_transcodeTime, 3, 1) = ":" Then
                Try
                    timerThread = New Thread(AddressOf ScheduleTimer)
                    timerThread.Start()
                Catch ex As Exception
                    Log.Error("plugin: iPiMPTranscodeToMP4 - transcode thread start error : {0}", ex.Message)
                End Try
            End If


        End Sub

        Public Sub MyStop() Implements ITvServerPlugin.Stop

            Dim events As ITvServerEvent = GlobalServiceProvider.Instance.Get(Of ITvServerEvent)()
            RemoveHandler events.OnTvServerEvent, New TvServerEventHandler(AddressOf iPiMP_OnTvServerEvent)

            timerThread.Abort()

            Log.Info("plugin: iPiMPTranscodeToMP4 stopped")

        End Sub

        Private Sub iPiMP_OnTvServerEvent(ByVal sender As Object, ByVal eventArgs As EventArgs)

            Try
                Dim tvEvent As TvServerEventArgs = CType(eventArgs, TvServerEventArgs)

                If tvEvent.EventType = TvServerEventType.RecordingEnded Then

                    If _transcodeNow Then
                        Screenshot(tvEvent.Recording.FileName)
                        Transcode(tvEvent.Recording.FileName)
                    ElseIf Mid(_transcodeTime, 3, 1) = ":" Then
                        Dim path As String = _folderPath & "\transcodeme.txt"
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
                        Log.Info(String.Format("plugin: iPiMPTranscodeToMP4 - scheduled {0} to {1}", tvEvent.Recording.Title, path))
                    End If

                End If

            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - iPiMP_OnTvServerEvent(): {0}", ex.Message)
            End Try

        End Sub

        Private Shared Sub Screenshot(ByVal _recFilename As String)

            Dim _params As String = String.Format("-B {0} -E {1} -w 88 -h 50 -c 1 -r 1 -i -t -P -z -O ""{2}"" -o .jpg ""{3}""", _preInterval * 60, _postInterval * 60, _folderPath, _recFilename)

            LaunchProcess(String.Format("""{0}""", _mtnPath), _params, String.Format("""{0}""", _folderPath))

            Log.Debug("plugin: iPiMPTranscodeToMP4 - Screenshot: {0}", _recFilename)

        End Sub

        Private Shared Sub Transcode(ByVal _recFilename As String)

            Dim _outfile As String = String.Format("{0}\{1}.{2}", _folderPath, Path.GetFileNameWithoutExtension(_recFilename), "mp4")
            '{0} = input filename
            '{1} = output filename
            '{2} = preset
            '{3} = video bitrate
            '{4} = audio bitrate
            Dim _ffparams As String = String.Format("-i ""{0}"" -threads 4 -re -vcodec libx264 -vpre {2} -s 480x272 -bt {1}k -acodec libfaac -ab {4}k -ar 48000 -ac 2 -async 2 ""{1}""", _recFilename, _outfile, _preset, _videoBitrate, _audioBitrate)
            Dim _hbparams As String = String.Format("""{2}"" -i ""{0}"" -o ""{1}""", _recFilename, _outfile, _preset)
            Dim _params As String = IIf(_transcoder.ToLower = "ffmpeg", _ffparams, _hbparams)

            LaunchProcess(String.Format("""{0}""", _transcoderPath), _params, String.Format("""{0}""", _folderPath))

            Log.Debug("plugin: iPiMPTranscodeToMP4 - Transcode: {0}", _recFilename)

        End Sub

        Private Shared Sub LaunchProcess(ByVal _filename As String, ByVal _params As String, ByVal _workingFolder As String)

            Try
                Dim process As Process = New Process
                process.StartInfo.FileName = _filename
                process.StartInfo.Arguments = _params
                process.StartInfo.WorkingDirectory = _workingFolder
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                Log.Debug("plugin: iPiMPTranscodeToMP4 - Transcode: {0} {1}", _filename, _params)
                process.Start()
                Do Until process.HasExited
                    System.Threading.Thread.Sleep(1000)
                Loop
                Log.Debug("plugin: iPiMPTranscodeToMP4 - Transcode complete: {0} {1}", _filename, _params)
            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - LaunchProcess(): {0}", ex.Message)
            End Try

        End Sub

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

            Dim mp4File As String = _folderPath & "\" & Path.GetFileNameWithoutExtension(e.Name) & ".mp4"
            Dim jpgFile As String = _folderPath & "\" & Path.GetFileNameWithoutExtension(e.Name) & ".jpg"

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

            If File.Exists(jpgFile) Then
                Try
                    File.Delete(jpgFile)
                    Log.Info("plugin: iPiMPTranscodeToMP4 - deleted: {0}", jpgFile)
                Catch ex As Exception
                    Log.Error("plugin: iPiMPTranscodeToMP4 - deletion failed: {0}", ex.Message)
                End Try
            Else
                Log.Info("plugin: iPiMPTranscodeToMP4 - no JPG file: {0}", jpgFile)
            End If

        End Sub

        Private Sub ScheduleTimer()

            Log.Debug("plugin: iPiMPTranscodeToMP4 - scheduler timer thread started")

            'http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/ebe5f10b-286e-45f8-8b97-26b24ef55a5e
            Dim HH As DateTime = DateTime.Today.AddHours(CDbl(Left(_transcodeTime, 2)))

            If DateTime.Now > HH Then
                HH = HH.AddDays(1)
            End If

            Dim timeToFirstExecution As Integer = CInt(HH.Subtract(DateTime.Now).TotalMilliseconds)
            Dim timeBetweenCalls As Integer = CInt(New System.TimeSpan(24, 0, 0).TotalMilliseconds)
            Dim methodToExecute As TimerCallback = (AddressOf RunScheduledTranscode)
            Dim timer As New System.Threading.Timer(methodToExecute, Nothing, timeToFirstExecution, timeBetweenCalls)

            Log.Debug("plugin: iPiMPTranscodeToMP4 - timeToFirstExecution: {0}", timeToFirstExecution)
            Log.Debug("plugin: iPiMPTranscodeToMP4 - timeBetweenCalls: {0}", timeBetweenCalls)

            timerThread.Sleep(Timeout.Infinite)

        End Sub

        Public Shared Sub RunScheduledTranscode()

            Dim path As String = _folderPath & "\transcodeme.txt"
            Dim tmpPath As String = _folderPath & "\transcode_working.txt"
            Dim recFile As String = String.Empty
            Dim sr As StreamReader
            Dim finished As Boolean = False
            Dim sleep As Integer = 10
            Dim hh, mm As String
            Dim xx() As String
            xx = Split(_transcodeTime, ":")
            hh = xx(0)
            mm = xx(1)

            Try
                If File.Exists(path) Then
                    File.Copy(path, tmpPath, True)
                    File.Delete(path)
                    sr = File.OpenText(tmpPath)
                    Do Until sr.EndOfStream
                        recFile = sr.ReadLine()
                        If File.Exists(recFile) Then
                            Screenshot(recFile)
                            Transcode(recFile)
                        Else
                            Log.Info("plugin: iPiMPTranscodeToMP4 - did not transcode {0} as the file was missing (already deleted?)", recFile)
                        End If
                    Loop
                    sr.Close()
                    File.Delete(tmpPath)
                End If
            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - scheduled transcode failed: {0}", ex.Message)
            End Try

            Log.Debug("plugin: iPiMPTranscodeToMP4 - transcoding thread stopped.")

        End Sub

    End Class

End Namespace