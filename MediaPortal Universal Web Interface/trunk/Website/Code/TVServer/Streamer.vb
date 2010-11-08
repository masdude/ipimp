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

Imports System.IO
Imports uWiMP.TVServer.MPWebServices
Imports uWiMP.TVServer.MPWebServices.Classes
Imports TvDatabase
Imports TvLibrary.Log
Imports TvControl

Namespace uWiMP.TVServer

    Public Class Streamer

        Enum MediaType As Integer
            None = 0
            Tv = 1
            Radio = 2
            MyVideo = 3
            MovingPicture = 4
            TvSeries = 5
            Music = 6
            Recording = 7
        End Enum

        Private _taskprogress As String
        Private _delegate As AsyncTaskDelegate

        Private Shared _mediaStream As Stream
        Private Shared _encoder As EncoderWrapper

        Private Shared _mediaID As String
        Public Property MediaID() As String
            Get
                Return _mediaID
            End Get
            Set(ByVal value As String)
                _mediaID = value
            End Set
        End Property

        Private Shared _mediaType As MediaType = MediaType.None
        Public Property Media() As MediaType
            Get
                Return _mediaType
            End Get
            Set(ByVal value As MediaType)
                _mediaType = value
            End Set
        End Property

        'Private Shared _running As Boolean = False
        'Private Property IsRunning() As Boolean
        '    Get
        '        Return _running
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _running = value
        '    End Set
        'End Property

        Protected Delegate Sub AsyncTaskDelegate()

        Public Function GetAsyncTaskProgress() As String
            Return _taskprogress
        End Function

        Public Sub ExecuteAsyncTask()
            Stream(_mediaType, _mediaID)
        End Sub

        Public Function OnBegin(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal extraData As Object) As IAsyncResult
            _taskprogress = String.Format("Streaming started at: {0}{1}", DateTime.Now.ToLongTimeString, vbCrLf)
            _delegate = New AsyncTaskDelegate(AddressOf ExecuteAsyncTask)
            Dim result As IAsyncResult = _delegate.BeginInvoke(cb, extraData)
            Return result
        End Function

        Public Sub OnEnd(ByVal ar As IAsyncResult)
            _taskprogress += String.Format("Streaming finished at: {0}", DateTime.Now.ToLongTimeString)
            _delegate.EndInvoke(ar)
        End Sub

        Public Sub OnTimeout(ByVal ar As IAsyncResult)
            _taskprogress += String.Format("Streaming timed out at: {0}", DateTime.Now.ToLongTimeString)
        End Sub

        Public Shared Sub Stream(ByVal mediatype As MediaType, ByVal id As String)

            StopStreaming()

            Dim bufferSize As Integer = &H80000
            Dim usedChannel As Integer = -1
            Dim filename As String = ""
            Dim card As Integer = 0
            Dim userName As String = String.Empty
            Dim cfg As EncoderConfig = Nothing

            Select Case mediatype

                Case Streamer.MediaType.Tv, Streamer.MediaType.Radio

                    cfg = Utils.LoadConfig.Item(0)

                    If mediatype = Streamer.MediaType.Radio Then
                        Log.Info("iPiMPWeb - Radio stream requested")
                        bufferSize = &HA00
                    Else
                        Log.Info("iPiMPWeb - TV stream requested")
                    End If

                    Dim res As WebTvResult = uWiMP.TVServer.Cards.StartTimeshifting(CInt(id))
                    Log.Info("iPiMPWeb - StartTimeshifting result is {0}", res.result.ToString)
                    If res.result <> 0 Then Exit Sub

                    card = res.user.idCard
                    usedChannel = res.user.idChannel
                    userName = res.user.name

                    If cfg.inputMethod = TransportMethod.Filename Then
                        filename = res.rtspURL
                    Else
                        filename = res.timeshiftFile
                    End If

                    UpdateStreamTracker(mediatype, id, card, userName)

                Case Streamer.MediaType.Recording
                    cfg = Utils.LoadConfig.Item(1)
                    Dim recording As Recording = uWiMP.TVServer.Recordings.GetRecordingById(CInt(id))
                    filename = recording.FileName
                    UpdateStreamTracker(mediatype, id, "", "")

                Case Streamer.MediaType.TvSeries
                    cfg = Utils.LoadConfig.Item(2)
                    filename = id
                    UpdateStreamTracker(mediatype, id, "", "")

                Case Else
                    Exit Sub

            End Select

            If Not (File.Exists(filename) OrElse filename.StartsWith("rtsp://")) Then
                Log.Info("iPiMPWeb - StartTimeshifting StopStreaming file does not exist or starts with rtsp {0}", filename)
                StopStreaming()
                Exit Sub
            End If

            Try
                If (cfg.inputMethod <> TransportMethod.Filename) Then
                    If (mediatype = Streamer.MediaType.Tv) Or (mediatype = Streamer.MediaType.Radio) Then
                        _mediaStream = New TsBuffer(filename)
                    Else
                        _mediaStream = New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    End If
                    _encoder = New EncoderWrapper(_mediaStream, cfg)
                Else
                    _encoder = New EncoderWrapper(filename, cfg)
                End If

            Catch ex As Exception
                Log.Info("iPiMPWeb - StartTimeshifting exception {0}", ex.Message)
            End Try

        End Sub

        Public Shared Function StopStream() As Boolean

            Log.Info("iPiMPWeb - StopStream start")
            Dim result As Boolean = True

            Try
                If _encoder IsNot Nothing Then _encoder.StopProcess()
            Catch ex As Exception
                Log.Info("iPiMPWeb - StopStream StopProcess exception {0}", ex.Message)
                result = False
            End Try

            Try
                If _mediaStream IsNot Nothing Then _mediaStream.Close()
            Catch ex As Exception
                Log.Info("iPiMPWeb - StopStream mediaStream.Close exception {0}", ex.Message)
                result = False
            End Try
            
            Try
                Dim type As MediaType = MediaType.None
                Dim mediaID As String = String.Empty
                Dim cardID As String = String.Empty
                Dim userName As String

                Dim path As String = String.Format("{0}\\SmoothStream.isml\\Stream.txt", AppDomain.CurrentDomain.BaseDirectory)
                Using sr As StreamReader = File.OpenText(path)
                    type = sr.ReadLine
                    mediaID = sr.ReadLine
                    cardID = sr.ReadLine
                    userName = sr.ReadLine
                End Using

                If (type = MediaType.Tv) Or (type = MediaType.Radio) Then
                    Dim stopResult As Boolean = uWiMP.TVServer.Cards.StopTimeshifting(CInt(mediaID), CInt(cardID), userName)
                    result = stopResult
                    Log.Info("iPiMPWeb - StopStream StopTimeShifting result {0}", stopResult.ToString)
                End If

            Catch ex As Exception
                result = False
            Finally
                ClearStreamFiles()
            End Try

            Return result

        End Function

        Private Shared Sub StopStreaming()
            Dim results As Boolean = StopStream()
        End Sub

        Private Shared Sub ClearStreamFiles()

            Dim path As String = String.Format("{0}\\SmoothStream.isml\\", AppDomain.CurrentDomain.BaseDirectory)

            If Not Directory.Exists(path) Then Directory.CreateDirectory(path)

            Dim dir As DirectoryInfo = New DirectoryInfo(path)
            For Each f As FileInfo In dir.GetFiles
                Try
                    f.Delete()
                    Log.Info("iPiMPWeb - ClearSTreamFiles deleted {0}", f.FullName)
                Catch ex As Exception
                End Try
            Next

        End Sub

        Private Shared Sub UpdateStreamTracker(ByVal streamType As String, ByVal id As String, Optional ByVal cardID As String = "", Optional ByVal user As String = "")

            Dim path As String = String.Format("{0}\\SmoothStream.isml", AppDomain.CurrentDomain.BaseDirectory)
            If Not Directory.Exists(path) Then Directory.CreateDirectory(path)

            Dim filePath As String = String.Format("{0}\\Stream.txt", path)
            If File.Exists(filePath) = False Then
                Using sw As StreamWriter = File.CreateText(filePath)
                    sw.WriteLine(streamType)
                    sw.WriteLine(id)
                    sw.WriteLine(cardID)
                    sw.WriteLine(user)
                    sw.Flush()
                End Using
            End If

        End Sub

    End Class

End Namespace
