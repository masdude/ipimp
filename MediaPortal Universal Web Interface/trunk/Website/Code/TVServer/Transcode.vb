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


Imports System
Imports System.IO
Imports System.Threading
Imports System.ComponentModel
Imports TvDatabase
Imports TvLibrary.Log

Namespace uWiMP.TVServer

    Public Class Transcode

        ' property to indicate if task has run at least once.
        Private Shared _firstRunComplete As Boolean = False
        Public ReadOnly Property firstRunComplete() As Boolean
            Get
                Return _firstRunComplete
            End Get
        End Property

        ' property to indicate iftask is running.
        Private Shared _running As Boolean = False
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

        ' property containing the transcode progress
        Private _progress As Integer = 0
        Public ReadOnly Property TranscodeProgress() As String
            Get
                Return _progress
            End Get
        End Property

        'property to indicate whether the last task succeeded.
        Public Shared _lastTaskSuccess As Boolean = True
        Public ReadOnly Property LastTaskSuccess() As Boolean
            Get
                If _lastFinishTime = DateTime.MinValue Then
                    Throw New InvalidOperationException("The task has never completed.")
                End If
                Return _lastTaskSuccess
            End Get
        End Property

        'store any exception generated during the task.
        Private Shared _exceptionOccured As Exception = Nothing
        Public ReadOnly Property ExceptionOccured() As Exception
            Get
                Return _exceptionOccured
            End Get
        End Property

        Private Shared _lastStartTime As DateTime = DateTime.MinValue
        Public ReadOnly Property LastStartTime() As DateTime
            Get
                If _lastStartTime = DateTime.MinValue Then
                    Throw New InvalidOperationException("The task has never started.")
                End If
                Return _lastStartTime
            End Get
        End Property

        Private Shared _lastFinishTime As DateTime = DateTime.MinValue
        Public ReadOnly Property LastFinishTime() As DateTime
            Get
                If _lastFinishTime = DateTime.MinValue Then
                    Throw New InvalidOperationException("The task has never completed.")
                End If
                Return _lastFinishTime
            End Get
        End Property

#Region "Transcoding settings"

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

        Private Shared _transcodeNow As Boolean = DEFAULT_TRANSCODE
        Private Shared _deleteWithRecording As Boolean = DEFAULT_DELETE
        Private Shared _folderPath As String = DEFAULT_SAVEPATH
        Private Shared _transcoder As String = DEFAULT_TRANSCODER
        Private Shared _transcodeTime As String = DEFAULT_STARTTIME
        Private Shared _preset As String = String.Empty
        Private Shared _custom As String = String.Empty
        Private Shared _priority As String = DEFAULT_PRIORITY
        Private Shared _groups As New List(Of String)

        Private Shared _iPiMPPath As String = DEFAULT_IPIMPPATH
        Private Shared _transcoderPath As String = String.Empty
        Private Shared _presetPath As String = String.Empty
        Private Shared _mtnPath As String = String.Empty
        Private Shared _preInterval As Integer = 0
        Private Shared _postInterval As Integer = 0

        Private Shared appSettings As NameValueCollection = ConfigurationManager.AppSettings

        Private Sub LoadSettings()

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

            _mtnPath = String.Format("{0}{1}", _iPiMPPath, "\MTN\mtn.exe")

            If _transcoder.ToLower = "handbrake" Then
                _transcoderPath = String.Format("{0}\HandBrake\HandBrakeCLI.exe", _iPiMPPath)
            Else
                _transcoderPath = String.Format("{0}\FFMpeg\FFMpeg.exe", _iPiMPPath)
                _presetPath = String.Format("{0}\FFMpeg\FFPresets", _iPiMPPath)
            End If

        End Sub

#End Region

        ' Start the task
        Public Sub RunTask()

            SyncLock Me
                If Not _running Then
                    _running = True
                    _lastStartTime = DateTime.Now
                    Dim t As New Thread(New ThreadStart(AddressOf Me.DoWork))
                    t.Start()
                Else
                    Log.Debug("uWiMP.Transcode - The task is already running!")
                    Throw New InvalidOperationException("The task is already running!")
                End If
            End SyncLock

        End Sub

        Public Sub DoWork()

            Log.Debug("uWiMP.Transcode: Transcoding started.")

            LoadSettings()

            If _recid <> 0 Then

                Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(_recid))
                Dim recFile As String = recording.FileName

                Screenshot(recFile)
                Transcode(recFile)

            End If

        End Sub

        Private Shared Sub Screenshot(ByVal _recFilename As String)

            Dim _params As String = String.Format("-B {0} -E {1} -w 88 -h 50 -c 1 -r 1 -i -t -P -z -O ""{2}"" -o .jpg ""{3}""", _preInterval * 60, _postInterval * 60, _folderPath, _recFilename)

            LaunchProcess(String.Format("""{0}""", _mtnPath), _params, String.Format("""{0}""", _folderPath), False)

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

            LaunchProcess(String.Format("""{0}""", _transcoderPath), params, String.Format("""{0}""", _folderPath), True)

        End Sub

        Private Shared Sub LaunchProcess(ByVal _filename As String, ByVal _params As String, ByVal _workingFolder As String, ByVal reportSuccess As Boolean)

            Log.Debug("uWiMP.Transcode: LaunchProcess start {0} {1} {2}", _filename, _params, _workingFolder)

            Try
                Dim process As Process = New Process

                With process.StartInfo
                    .FileName = _filename
                    .Arguments = _params
                    '.WorkingDirectory = _workingFolder
                    .WindowStyle = ProcessWindowStyle.Hidden
                    .UseShellExecute = False
                    .RedirectStandardOutput = True
                End With

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

                process.WaitForExit()

                If reportSuccess Then
                    If process.ExitCode = 0 Then
                        _lastTaskSuccess = True
                    Else
                        _lastTaskSuccess = False
                    End If
                End If

            Catch ex As Exception

                If reportSuccess Then _lastTaskSuccess = False
                _exceptionOccured = ex
                Log.Debug("uWiMP.Transcode: LaunchProcess exception: {0}", ex.ToString)

            Finally

                If reportSuccess Then
                    _running = False
                    _lastFinishTime = DateTime.Now
                    If Not _firstRunComplete Then _firstRunComplete = True
                End If
                
            End Try

            Log.Debug("uWiMP.Transcode: LaunchProcess end")

        End Sub

        Public Shared Function GetProgress() As String
            Return _lastStartTime
        End Function

        Public Shared Function IsTaskRunning() As Boolean
            Return _running
        End Function

    End Class

End Namespace

