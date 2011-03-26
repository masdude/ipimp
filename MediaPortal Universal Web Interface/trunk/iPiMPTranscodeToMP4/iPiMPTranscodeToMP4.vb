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
        Const DEFAULT_SAVEPATH = "C:\"
        Const DEFAULT_TRANSCODER = "handbrake"
        Const DEFAULT_IPIMPPATH = "C:\Program Files\iPiMP\Utilities"
        Const DEFAULT_PRESET = "iPhone & iPod Touch"
        Const DEFAULT_CUSTOM = ""
        Const DEFAULT_PRIORITY = "Normal"
        Const DEFAULT_GROUPS = ""

        Friend Shared _transcodeNow As Boolean = DEFAULT_TRANSCODE
        Friend Shared _deleteWithRecording As Boolean = DEFAULT_DELETE
        Friend Shared _folderPath As String = DEFAULT_SAVEPATH
        Friend Shared _transcoder As String = DEFAULT_TRANSCODER
        Friend Shared _transcodeTime As String = DEFAULT_STARTTIME
        Friend Shared _preset As String = String.Empty
        Friend Shared _custom As String = String.Empty
        Friend Shared _priority As String = DEFAULT_PRIORITY
        Friend Shared _groups As New List(Of String)
        Friend Shared _iPiMPPath As String = DEFAULT_IPIMPPATH

        Private Shared _transcoderPath As String = String.Empty
        Private Shared _presetPath As String = String.Empty
        Private Shared _preInterval As Integer = 0
        Private Shared _postInterval As Integer = 0

        Private Shared appSettings As NameValueCollection = ConfigurationManager.AppSettings

        Private period As Integer = 24
        Private timer As System.Threading.Timer

#Region "ITVServer"

        Public ReadOnly Property Name() As String Implements ITvServerPlugin.Name
            Get
                Return "iPiMPTranscodeToMP4"
            End Get
        End Property

        Public ReadOnly Property Version() As String Implements ITvServerPlugin.Version
            Get
                Return "5.1.0"
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
                _transcodeTime = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeTime", DEFAULT_STARTTIME).Value
                _preset = layer.GetSetting("iPiMPTranscodeToMP4_Preset", DEFAULT_PRESET).Value
                _priority = layer.GetSetting("iPiMPTranscodeToMP4_Priority", DEFAULT_PRIORITY).Value
                _custom = layer.GetSetting("iPiMPTranscodeToMP4_Custom", DEFAULT_CUSTOM).Value
                _preInterval = Int32.Parse(layer.GetSetting("preRecordInterval", "5").Value)
                _postInterval = Int32.Parse(layer.GetSetting("postRecordInterval", "5").Value)
                Dim groups As String = layer.GetSetting("iPiMPTranscodeToMP4_Groups", DEFAULT_GROUPS).Value
                If groups <> "" Then
                    For Each group As String In Split(groups, ",")
                        If group <> "" Then _groups.Add(group)
                    Next
                End If

            Catch ex As Exception

                _deleteWithRecording = DEFAULT_DELETE
                _transcodeNow = DEFAULT_TRANSCODE
                _folderPath = DEFAULT_SAVEPATH
                _transcoder = DEFAULT_TRANSCODER
                _iPiMPPath = DEFAULT_IPIMPPATH
                _transcodeTime = DEFAULT_STARTTIME
                _preset = DEFAULT_PRESET
                _priority = DEFAULT_PRIORITY
                _custom = DEFAULT_CUSTOM
                _preInterval = 5
                _postInterval = 5

                Log.Error("plugin: iPiMPTranscodeToMP4 - LoadSettings(): {0}", ex.Message)

            End Try

            If _transcoder.ToLower = "handbrake" Then
                _transcoderPath = String.Format("{0}\HandBrake\HandBrakeCLI.exe", _iPiMPPath)
            Else
                _transcoderPath = String.Format("{0}\FFMpeg\FFMpeg.exe", _iPiMPPath)
                _presetPath = String.Format("{0}\FFMpeg\FFPresets", _iPiMPPath)
            End If

        End Sub

        Public Sub Start(ByVal controller As IController) Implements ITvServerPlugin.Start

            LoadSettings()

            If _deleteWithRecording = True Then Watch_RecordedFolders()

            Dim events As ITvServerEvent = GlobalServiceProvider.Instance.Get(Of ITvServerEvent)()
            AddHandler events.OnTvServerEvent, New TvServerEventHandler(AddressOf OnTvServerEvent)

            If Mid(_transcodeTime, 3, 1) = ":" Then ScheduleTimer()

            Log.Info("plugin: iPiMPTranscodeToMP4 - started")

        End Sub

        Public Sub MyStop() Implements ITvServerPlugin.Stop

            If _deleteWithRecording = True Then StopWatching_RecordedFolders()

            Dim events As ITvServerEvent = GlobalServiceProvider.Instance.Get(Of ITvServerEvent)()
            RemoveHandler events.OnTvServerEvent, New TvServerEventHandler(AddressOf OnTvServerEvent)

            If Mid(_transcodeTime, 3, 1) = ":" Then StopScheduleTimer()

            Log.Info("plugin: iPiMPTranscodeToMP4 stopped")

        End Sub

        Private Sub OnTvServerEvent(ByVal sender As Object, ByVal eventArgs As EventArgs)

            Try
                Dim tvEvent As TvServerEventArgs = CType(eventArgs, TvServerEventArgs)

                If tvEvent.EventType = TvServerEventType.RecordingEnded Then

                    Dim layer As New TvBusinessLayer
                    Dim doTranscode As Boolean = True
                    For Each group As String In _groups
                        Dim channelGroup As ChannelGroup = layer.GetGroupByName(group)
                        Dim channels As List(Of Channel) = layer.GetTVGuideChannelsForGroup(channelGroup.IdGroup)
                        For Each channel As Channel In channels
                            If channel.IdChannel = tvEvent.Recording.IdChannel Then doTranscode = False
                        Next
                    Next

                    If doTranscode Then
                        If _transcodeNow Then
                            Screenshot(tvEvent.Recording.FileName)
                            Transcode(tvEvent.Recording.FileName)
                        ElseIf Mid(_transcodeTime, 3, 1) = ":" Then
                            Dim path As String = _folderPath & "\transcodeme.txt"
                            Dim sw As StreamWriter
                            If File.Exists(path) Then
                                sw = File.AppendText(path)
                            Else
                                sw = File.CreateText(path)
                            End If
                            sw.WriteLine(tvEvent.Recording.FileName)
                            sw.Flush()
                            sw.Close()
                            Log.Info(String.Format("plugin: iPiMPTranscodeToMP4 - scheduled {0} to {1}", tvEvent.Recording.Title, path))
                        End If
                    Else
                        Log.Info(String.Format("plugin: iPiMPTranscodeToMP4 - excluded {0} ", tvEvent.Recording.Title))
                    End If
                End If

            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - iPiMP_OnTvServerEvent(): {0}", ex.Message)
            End Try

        End Sub

        Private Shared Sub Screenshot(ByVal _recFilename As String)

            Dim params As String = String.Format("-i {0} -ss {1} -vcodec mjpeg -vframes 1 -an -f rawvideo -s 88x50 ""{2}\{3}.jpg""", _recFilename, (_preInterval * 60) + 10, _folderPath, Path.GetFileNameWithoutExtension(_recFilename))
            
            LaunchProcess(String.Format("{0}\FFMpeg\FFMpeg.exe", _iPiMPPath), params, String.Format("""{0}""", _folderPath))

        End Sub

        Private Shared Sub Transcode(ByVal _recFilename As String)

            Dim _outfile As String = String.Format("{0}\{1}.{2}", _folderPath, Path.GetFileNameWithoutExtension(_recFilename), "mp4")
            '{0} = input filename
            '{1} = output filename
            '{2} = preset
            Dim params As String = String.Empty
            If _custom.Length > 0 Then
                params = String.Format(_custom, _recFilename, _outfile)
            ElseIf _transcoder.ToLower = "ffmpeg" Then
                params = String.Format("-i ""{0}"" -threads 4 -re -vcodec libx264 -fpre ""{2}\{3}.ffpreset"" -s 480x272 -bt 256k -acodec libfaac -ab 128k -ar 48000 -ac 2 -async 2 ""{1}""", _recFilename, _outfile, _presetPath, _preset)
            ElseIf _transcoder.ToLower = "handbrake" Then
                If _preset.ToLower = "nexus one" Then
                    params = String.Format("-i ""{0}"" -t 1 -c 1 -o ""{1}"" -f mp4 -I  -O  -X 800 -e x264 -b 1024 -a 1 -E faac -6 dpl2 -R 48 -B 128 -D 0.0 -x cabac=0:ref=1:me=umh:bframes=0:subq=6:8x8dct=0:trellis=0:weightb=0:mixed-refs=0:no-fast-pskip=1:analyse=all -v 1", _recFilename, _outfile)
                Else
                    params = String.Format("-Z ""{0}"" -i ""{1}"" -o ""{2}""", _preset, _recFilename, _outfile)
                End If
            Else
                Log.Info("plugin: iPiMPTranscodeToMP4 - could not determine transcoding options.")
            End If

            LaunchProcess(String.Format("""{0}""", _transcoderPath), params, String.Format("""{0}""", _folderPath))

        End Sub

        Private Shared Sub LaunchProcess(ByVal _filename As String, ByVal _params As String, ByVal _workingFolder As String)

            Try
                Dim process As Process = New Process
                process.StartInfo.FileName = _filename
                process.StartInfo.Arguments = _params
                process.StartInfo.WorkingDirectory = _workingFolder
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                Log.Info("plugin: iPiMPTranscodeToMP4 - LaunchProcess: {0} {1}", _filename, _params)
                process.Start()
                Select Case _priority.ToLower
                    Case "normal"
                        process.PriorityClass = ProcessPriorityClass.Normal
                    Case "belownormal"
                        process.PriorityClass = ProcessPriorityClass.BelowNormal
                    Case "idle"
                        process.PriorityClass = ProcessPriorityClass.Idle
                    Case Else
                        process.PriorityClass = ProcessPriorityClass.Normal
                End Select
                Do Until process.HasExited
                    System.Threading.Thread.Sleep(1000)
                Loop
                Log.Info("plugin: iPiMPTranscodeToMP4 - LaunchProcess complete: {0} {1}", _filename, _params)
            Catch ex As Exception
                Log.Error("plugin: iPiMPTranscodeToMP4 - LaunchProcess: {0}", ex.Message)
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

        Private Sub StopWatching_RecordedFolders()

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
                            RemoveHandler fsWatcher.Deleted, AddressOf FileDeleted
                            Log.Info("plugin: iPiMPTranscodeToMP4 - stopped watching: {0}", cardPath)
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
            Dim timeBetweenCalls As Integer = CInt(New System.TimeSpan(period, 0, 0).TotalMilliseconds)
            Dim methodToExecute As TimerCallback = (AddressOf RunScheduledTranscode)
            timer = New System.Threading.Timer(methodToExecute, Nothing, timeToFirstExecution, timeBetweenCalls)

            Log.Info("plugin: iPiMPTranscodeToMP4 - timeToFirstExecution: {0}", timeToFirstExecution)
            Log.Info("plugin: iPiMPTranscodeToMP4 - timeBetweenCalls: {0}", timeBetweenCalls)

        End Sub

        Private Sub StopScheduleTimer()

            timer.Dispose()

        End Sub

        Public Shared Sub RunScheduledTranscode()

            Dim path As String = _folderPath & "\transcodeme.txt"
            Dim tmpPath As String = _folderPath & "\transcode_working.txt"
            Dim recFile As String = String.Empty
            Dim sr As StreamReader

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

            Log.Info("plugin: iPiMPTranscodeToMP4 - transcoding thread stopped.")

        End Sub

    End Class

End Namespace