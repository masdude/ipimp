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
        Private _delegate As AsyncTaskDelegate

        Private Shared _card As Integer
        Private Shared _mediaStream As Stream
        Private Shared _encoder As EncoderWrapper

        Private _channelID As Integer = 0
        Public Property ChannelID() As String
            Get
                Return _channelID
            End Get
            Set(ByVal value As String)
                _channelID = value
            End Set
        End Property

        Private _mediaType As MediaType = MediaType.None
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

        Public Shared Function Stream(ByVal mediatype As MediaType, ByVal id As Integer) As Boolean

            Dim bufferSize As Integer = &H80000
            Dim tvServerUsername As String = ""
            Dim usedChannel As Integer = -1
            Dim filename As String = ""

            Dim cfg As EncoderConfig = Utils.LoadConfig.Item(0)

            Select Case mediatype
                Case Streamer.MediaType.Tv, Streamer.MediaType.Radio
                    If mediatype = Streamer.MediaType.Radio Then bufferSize = &HA00
                    Dim res As WebTvResult = uWiMP.TVServer.Cards.StartTimeshifting(id)
                    If res.result <> 0 Then Return False
                    _card = res.user.idCard
                    usedChannel = res.user.idChannel
                    tvServerUsername = res.user.name

                    If (cfg.inputMethod = TransportMethod.Filename) Then
                        filename = res.rtspURL
                    Else
                        filename = res.timeshiftFile
                    End If

                    'ElseIf (Not MyBase.Request.QueryString.Item("idRecording") Is Nothing) Then
                    '    filename = server.GetRecording(Integer.Parse(MyBase.Request.QueryString.Item("idRecording"))).fileName
                    'ElseIf (Not MyBase.Request.QueryString.Item("idMovie") Is Nothing) Then
                    '    filename = server.GetMovie(Integer.Parse(MyBase.Request.QueryString.Item("idMovie"))).file
                    'ElseIf (Not MyBase.Request.QueryString.Item("idMusicTrack") Is Nothing) Then
                    '    filename = server.GetMusicTrack(Integer.Parse(MyBase.Request.QueryString.Item("idMusicTrack"))).file
                    'ElseIf (Not MyBase.Request.QueryString.Item("idTvSeries") Is Nothing) Then
                    '    filename = server.GetTvSeries(MyBase.Request.QueryString.Item("idTvSeries")).filename
                    'ElseIf (Not MyBase.Request.QueryString.Item("idMovingPicture") Is Nothing) Then
                    '    filename = server.GetMovingPicture(Integer.Parse(MyBase.Request.QueryString.Item("idMovingPicture"))).filename
                    'End If

            End Select

            If Not (File.Exists(filename) OrElse filename.StartsWith("rtsp://")) Then Return False

            If (cfg.inputMethod <> TransportMethod.Filename) Then
                If mediatype = Streamer.MediaType.Tv Then
                    _mediaStream = New TsBuffer(filename)
                Else
                    _mediaStream = New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                End If
                _encoder = New EncoderWrapper(_mediaStream, cfg)
            Else
                _encoder = New EncoderWrapper(filename, cfg)
            End If

        End Function

        Public Shared Function StopStream(ByVal mediatype As MediaType) As Boolean
            If (Not _mediaStream Is Nothing) Then
                _mediaStream.Close()
            End If
            If MediaType = Streamer.MediaType.Tv Then
                uWiMP.TVServer.Cards.StopTimeshifting(uWiMP.TVServer.Cards.GetCard(_card))
            End If
            _encoder.StopProcess()
        End Function

        Private Function ClearLiveFiles() As Boolean

            Return True

        End Function
    End Class

End Namespace
