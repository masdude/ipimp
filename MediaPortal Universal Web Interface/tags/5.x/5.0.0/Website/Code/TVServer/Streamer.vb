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
Imports TvControl
Imports TvLibrary
Imports uWiMP.TVServer.MPWebServices
Imports uWiMP.TVServer.MPWebServices.Classes

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
        End Enum

        Private _taskprogress As String
        Private Shared _isStreaming As Boolean
        Private _delegate As AsyncTaskDelegate

        Private Shared _card As Integer
        Private Shared _mediaStream As Stream
        Private Shared _encoder As EncoderWrapper
        Private Shared _userName As String
        Private Shared _channelID As Integer = 0

        Public Property ChannelID() As String
            Get
                Return _channelID
            End Get
            Set(ByVal value As String)
                _channelID = value
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

        ' Create delegate.
        Protected Delegate Sub AsyncTaskDelegate()

        Public Function GetAsyncTaskProgress() As String
            Return _taskprogress
        End Function

        Public Function IsRunning() As Boolean
            Return _isStreaming
        End Function

        Public Sub ExecuteAsyncTask()
            Stream(_mediaType, _channelID)
        End Sub

        ' Define the method that will get called to
        ' start the asynchronous task.
        Public Function OnBegin(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal extraData As Object) As IAsyncResult
            _taskprogress = String.Format("Streaming started at: {0}{1}", DateTime.Now.ToLongTimeString, vbCrLf)
            _delegate = New AsyncTaskDelegate(AddressOf ExecuteAsyncTask)
            Dim result As IAsyncResult = _delegate.BeginInvoke(cb, extraData)

            Return result
        End Function

        ' Define the method that will get called when
        ' the asynchronous task is ended.
        Public Sub OnEnd(ByVal ar As IAsyncResult)
            _taskprogress += String.Format("Streaming finished at: {0}", DateTime.Now.ToLongTimeString)
            _delegate.EndInvoke(ar)
        End Sub

        ' Define the method that will get called if the task
        ' is not completed within the asynchronous timeout interval.
        Public Sub OnTimeout(ByVal ar As IAsyncResult)
            _taskprogress += String.Format("Streaming timed out at: {0}", DateTime.Now.ToLongTimeString)
        End Sub

        Public Shared Sub Stream(ByVal mediatype As MediaType, ByVal id As Integer)

            
            Dim bufferSize As Integer = &H80000
            'Dim tvServerUsername As String = ""
            Dim usedChannel As Integer = -1
            Dim filename As String = ""
            Dim type As String = ""

            Dim cfg As EncoderConfig = Utils.LoadConfig.Item(0)

            Select Case mediatype
                Case Streamer.MediaType.Tv, Streamer.MediaType.Radio
                    type = "tv"
                    If mediatype = Streamer.MediaType.Radio Then
                        bufferSize = &HA00
                        type = "radio"
                    End If

                    Dim res As WebTvResult = uWiMP.TVServer.Cards.StartTimeshifting(id)
                    If res.result <> 0 Then Exit Sub
                    _card = res.user.idCard
                    usedChannel = res.user.idChannel
                    _userName = res.user.name

                    If (cfg.inputMethod = TransportMethod.Filename) Then
                        filename = res.rtspURL
                    Else
                        filename = res.timeshiftFile
                    End If

            End Select

            If Not (File.Exists(filename) OrElse filename.StartsWith("rtsp://")) Then Exit Sub

            ClearStreamFiles(type, id)

            If (cfg.inputMethod <> TransportMethod.Filename) Then
                If (mediatype = Streamer.MediaType.Tv) Or (mediatype = Streamer.MediaType.Radio) Then
                    _mediaStream = New TsBuffer(filename)
                Else
                    _mediaStream = New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                End If
                _encoder = New EncoderWrapper(_mediaStream, cfg)
                _isStreaming = True
            Else
                _encoder = New EncoderWrapper(filename, cfg)
                _isStreaming = True
            End If

        End Sub

        Public Shared Function StopStream() As Boolean
            Try
                If (Not _mediaStream Is Nothing) Then
                    _mediaStream.Close()
                End If
                If _mediaType = Streamer.MediaType.Tv Or _mediaType = Streamer.MediaType.Radio Then
                    uWiMP.TVServer.Cards.StopTimeshifting(_channelID, _card, _userName)
                End If
                _encoder.StopProcess()
                _isStreaming = False
                Dim path As String = String.Format("{0}\SmoothStream\Channel.txt", AppDomain.CurrentDomain.BaseDirectory)
                If File.Exists(path) Then File.Delete(path)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        Private Shared Sub ClearStreamFiles(ByVal type As String, ByVal channelID As Integer)

            Dim dir As DirectoryInfo = New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory & "\\SmoothStream")
            For Each f As FileInfo In dir.GetFiles
                Try
                    f.Delete()
                Catch ex As Exception
                End Try
            Next

            Dim path As String = String.Format("{0}\SmoothStream\Channel.txt", AppDomain.CurrentDomain.BaseDirectory)
            If File.Exists(path) = False Then
                ' Create a file to write to.
                Using sw As StreamWriter = File.CreateText(path)
                    sw.WriteLine(type)
                    sw.WriteLine(channelID.ToString)
                    sw.Flush()
                End Using
            End If

        End Sub

    End Class

End Namespace
