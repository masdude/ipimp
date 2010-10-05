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

Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports System.IO

Imports Jayrock.Json.Conversion

Imports MediaPortal.Configuration
Imports MediaPortal.GUI.Library
Imports MediaPortal.InputDevices

Namespace MPClientController

    Public Class ClientPlugin
        Implements ISetupForm
        Implements IPlugin

        Private tcpListener As Sockets.TcpListener
        Private tcpClient As Sockets.TcpClient
        Private tcpThread As Thread
        Private httpListener As HttpListener
        Private httpContext As HttpListenerContext
        Private httpThread As Thread
        Private broadcastThread As Thread
        Private remoteHandler As InputHandler
        Private keyboardHandler As MediaPortal.Hooks.KeyboardHook
        Private isMovingPicturesPresent As Boolean = Nothing
        Private isTVSeriesPresent As Boolean = Nothing

        Const DEFAULT_PORT As Integer = 55667
        Const SUPPORTED_MOVING_PICTURES_MINVERSION As String = "1.0.6.1116"
        Const SUPPORTED_TV_SERIES_MINVERSION As String = "2.6.2.1231"
        Private port As Integer = DEFAULT_PORT

#Region "ISetupForm members"

        Public Function Author() As String Implements ISetupForm.Author
            Return "Cheezey"
        End Function

        Public Function CanEnable() As Boolean Implements ISetupForm.CanEnable
            Return True
        End Function

        Public Function DefaultEnabled() As Boolean Implements ISetupForm.DefaultEnabled
            Return True
        End Function

        Public Function Description() As String Implements ISetupForm.Description
            Return "Provides a remoting interface to a MediaPortal client."
        End Function

        Public Function GetHome(ByRef strButtonText As String, ByRef strButtonImage As String, ByRef strButtonImageFocus As String, ByRef strPictureImage As String) As Boolean Implements ISetupForm.GetHome
            strButtonText = String.Empty
            strButtonImage = String.Empty
            strButtonImageFocus = String.Empty
            strPictureImage = String.Empty
            Return False
        End Function

        Public Function GetWindowId() As Integer Implements ISetupForm.GetWindowId
            Return -1
        End Function

        Public Function HasSetup() As Boolean Implements ISetupForm.HasSetup
            Return True
        End Function

        Public Function PluginName() As String Implements ISetupForm.PluginName
            Return "MPClientController"
        End Function

        Public Sub ShowPlugin() Implements ISetupForm.ShowPlugin
            Dim setupForm As Form = New SetupForm
            setupForm.ShowDialog()
        End Sub
#End Region

#Region "IPlugin members"

        Public Sub Start() Implements MediaPortal.GUI.Library.IPlugin.Start

            remoteHandler = New InputHandler("iPiMP")
            DoStart()

        End Sub

        Public Sub [Stop]() Implements MediaPortal.GUI.Library.IPlugin.Stop
            DoStop()
        End Sub

#End Region

#Region "Thread handling"

        Private Sub DoStart()

            Dim xmlReader As MediaPortal.Profile.Settings = New MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))
            port = xmlReader.GetValueAsInt("MPClientController", "TCPPort", DEFAULT_PORT)

            If isMovingPicturesPresent = Nothing Then
                isMovingPicturesPresent = iPiMPUtils.IsPluginLoaded("MovingPictures.dll", SUPPORTED_MOVING_PICTURES_MINVERSION)
                Log.Info("plugin: MPClientController - MovingPictures detected version, min version: {0} iPiMP supported: {1}", SUPPORTED_MOVING_PICTURES_MINVERSION, isMovingPicturesPresent)
            End If

            If isTVSeriesPresent = Nothing Then
                isTVSeriesPresent = iPiMPUtils.IsPluginLoaded("MP-TVSeries.dll", SUPPORTED_TV_SERIES_MINVERSION)
                Log.Info("plugin: MPClientController - TVSeries detected version, min version: {0} iPiMP supported: {1}", SUPPORTED_TV_SERIES_MINVERSION, isTVSeriesPresent.ToString)
            End If

            tcpThread = New System.Threading.Thread(AddressOf DoTCPListen)
            tcpThread.IsBackground = True
            tcpThread.Start()
            Log.Info("plugin: MPClientController - started TCP listening on port {0}", port.ToString)

            httpThread = New System.Threading.Thread(AddressOf DoHTTPListen)
            httpThread.IsBackground = True
            httpThread.Start()
            Log.Info("plugin: MPClientController - started HTTP listening on port {0}", (port + 1).ToString)

            broadcastThread = New System.Threading.Thread(AddressOf DoBroadcast)
            broadcastThread.IsBackground = True
            broadcastThread.Start()
            Log.Info("plugin: MPClientController - started broadcasting on UDP")

        End Sub

        Private Sub DoStop()
            Try
                tcpListener.Stop()
                tcpThread.Abort()
                Log.Info("plugin: MPClientController - stopped TCP listening")
            Catch ex As Exception
            End Try

            Try
                httpContext = Nothing
                httpListener.Stop()
                httpThread.Abort()
                Log.Info("plugin: MPClientController - stopped HTTP listening")
            Catch ex As Exception
            End Try

            Try
                broadcastThread.Abort()
                Log.Info("plugin: MPClientController - stopped broadcasting")
            Catch ex As Exception
            End Try

        End Sub

#End Region

#Region "Listeners"

        Private Sub DoTCPListen()

            tcpListener = New System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port)
            tcpListener.Start()

            Do
                tcpClient = tcpListener.AcceptTcpClient
                TCPInputReceived()
            Loop

        End Sub

        Private Sub DoHTTPListen()

            httpListener = New HttpListener
            httpListener.Prefixes.Add(String.Format("http://*:{0}/mpcc/", (port + 1).ToString))
            httpListener.Start()

            Do
                httpContext = httpListener.GetContext
                HTTPPostInputReceived()
            Loop

        End Sub

#End Region

#Region "Broadcast"

        Private Sub DoBroadcast()

            Dim hostname As String = Dns.GetHostName
            Dim MACAddress As String = String.Empty
            Dim broadcastAddresses As List(Of String) = Nothing
            Try
                For Each nic As NetworkInformation.NetworkInterface In NetworkInformation.NetworkInterface.GetAllNetworkInterfaces
                    MACAddress = nic.GetPhysicalAddress.ToString
                    If MACAddress.Length = 12 Then MACAddress = String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Left(MACAddress, 2), Mid(MACAddress, 3, 2), Mid(MACAddress, 5, 2), Mid(MACAddress, 7, 2), Mid(MACAddress, 9, 2), Right(MACAddress, 2))
                    Exit For 'Just get the first MAC Address
                Next
            Catch ex As Exception
            End Try

            Dim socket As New Sockets.Socket(Sockets.AddressFamily.InterNetwork, Sockets.SocketType.Dgram, Sockets.ProtocolType.Udp)
            socket.EnableBroadcast = True
            Dim broadcastAddress As IPAddress
            Dim sendbuf As Byte() = Encoding.ASCII.GetBytes(String.Format("{0},{1},{2},{3},{4}", hostname, MACAddress, port, isMovingPicturesPresent, isTVSeriesPresent))

            Do
                broadcastAddresses = GetDirectBroadcastAddresses()
                For Each address As String In broadcastAddresses
                    broadcastAddress = IPAddress.Parse(address)
                    Dim endpoint As New IPEndPoint(broadcastAddress, DEFAULT_PORT)
                    socket.SendTo(sendbuf, endpoint)
                    Log.Debug("plugin: MPClientController - iPiMP ping on {0}", address)
                Next
                Log.Debug("plugin: MPClientController - iPiMP ping data {0}", Encoding.ASCII.GetString(sendbuf))
                System.Threading.Thread.Sleep(1000 * 60) 'sleep for one minute
            Loop

        End Sub

        Private Function GetDirectBroadcastAddresses() As List(Of String)

            Dim broadcastAddresses As New List(Of String)
            Dim IPAddress As String
            Dim SubnetMask As String

            For Each nic As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces
                If nic.Supports(NetworkInterfaceComponent.IPv4) Then
                    Dim nicProperties As IPInterfaceProperties = nic.GetIPProperties
                    Dim addresses As UnicastIPAddressInformationCollection = nicProperties.UnicastAddresses
                    For Each address In addresses
                        If (address.Address.AddressFamily = Net.Sockets.AddressFamily.InterNetwork) And
                            (address.Address IsNot Nothing) And
                            (address.IPv4Mask IsNot Nothing) Then
                            IPAddress = address.Address.ToString
                            SubnetMask = address.IPv4Mask.ToString
                            broadcastAddresses.Add(DirectBroadcastAddress(IPAddress, SubnetMask))
                        End If
                    Next
                End If
            Next

            Return broadcastAddresses

        End Function

        Private Function DirectBroadcastAddress(ByVal IPAddress As String, ByVal SubnetMask As String) As String

            Dim temp() As String
            Dim ip(0 To 3) As Integer
            Dim nm(0 To 3) As Integer
            Dim i As Integer

            temp = Split(SubnetMask, ".", -1, vbTextCompare)
            For i = 0 To 3
                nm(i) = 255 - Val(temp(i))
            Next i

            temp = Split(IPAddress, ".", -1, vbTextCompare)
            For i = 0 To 3
                ip(i) = Val(temp(i))
                ip(i) = ip(i) Or nm(i)
            Next i

            Return ip(0) & "." & ip(1) & "." & ip(2) & "." & ip(3)

        End Function

#End Region

        Private Sub TCPInputReceived()

            Log.Debug("plugin: iPiMPClient - TCPInputReceived")

            Dim results As String = String.Empty
            Dim response As Byte() = Nothing
            Dim data As String = String.Empty
            Dim stream As Sockets.NetworkStream = tcpClient.GetStream
            Dim myCompleteMessage As StringBuilder = New StringBuilder()

            If stream.CanRead Then
                Dim myReadBuffer(1024) As Byte
                Dim numberOfBytesRead As Integer = 0
                Do
                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length)
                    myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead))
                Loop While stream.DataAvailable
            End If
            data = myCompleteMessage.ToString

            Dim request As MPClientRequest
            If data.Length > 0 Then
                Log.Debug("plugin: iPiMPClient - Raw: {0}", data)
                Try
                    request = DirectCast(JsonConvert.Import(GetType(MPClientRequest), data), MPClientRequest)
                Catch ex As Exception
                    response = System.Text.Encoding.UTF8.GetBytes(iPiMPUtils.SendError(1, "Bad data"))
                    stream.Write(response, 0, response.Length)
                    Log.Debug("plugin: iPiMPClient - Sent: {0}", System.Text.Encoding.UTF8.GetString(response))
                    Exit Sub
                End Try
            Else
                response = System.Text.Encoding.UTF8.GetBytes(iPiMPUtils.SendError(2, "No data"))
                stream.Write(response, 0, response.Length)
                Log.Debug("plugin: iPiMPClient - Sent: {0}", System.Text.Encoding.UTF8.GetString(response))
                Exit Sub
            End If

            Select Case request.Action.ToLower
                Case "unsupported"
                    response = System.Text.Encoding.UTF8.GetBytes(iPiMPUtils.SendError(6, "HTTP only"))
                    stream.Write(response, 0, response.Length)
                    Log.Debug("plugin: iPiMPClient - Sent: {0}", System.Text.Encoding.UTF8.GetString(response))
                Case Else
                    results = GetResults(request)
            End Select

            If results = String.Empty Then results = iPiMPUtils.SendError(3, "No result")
            response = System.Text.Encoding.UTF8.GetBytes(results)

            stream.Write(response, 0, response.Length)
            stream.Close()

            Log.Debug("plugin: iPiMPClient - Sent: {0}", System.Text.Encoding.UTF8.GetString(response))

        End Sub

        Private Sub HTTPPostInputReceived()

            Log.Debug("plugin: iPiMPClient - HTTPPostInputReceived")

            Dim httpRequest As HttpListenerRequest = httpContext.Request
            Dim httpResponse As HttpListenerResponse = httpContext.Response
            Dim inputText As String = String.Empty
            Dim results As String = String.Empty

            Dim request As MPClientRequest = Nothing

            If httpRequest.ContentLength64 > 0 Then
                If (httpRequest.ContentType = "application/x-www-form-urlencoded") Then
                    Dim datastream As Stream = httpRequest.InputStream
                    Dim reader As New StreamReader(datastream)
                    inputText = reader.ReadToEnd
                    Log.Debug("plugin: iPiMPClient - Raw: {0}", inputText)
                    Try
                        request = DirectCast(JsonConvert.Import(GetType(MPClientRequest), inputText), MPClientRequest)
                    Catch ex As Exception
                        results = iPiMPUtils.SendError(1, "Bad data")
                    End Try
                Else
                    results = iPiMPUtils.SendError(8, "Unknown content type")
                End If
            Else
                results = iPiMPUtils.SendError(2, "No data")
            End If

            If results = String.Empty Then results = GetResults(request)

            Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes(results)
            httpResponse.ContentLength64 = buffer.Length

            Dim outputStream As Stream = httpResponse.OutputStream
            outputStream.Write(buffer, 0, buffer.Length)
            outputStream.Close()

            Log.Debug("plugin: iPiMPClient - Sent: {0}", System.Text.Encoding.UTF8.GetString(buffer))

        End Sub

        Private Function GetResults(ByVal request As MPClientRequest) As String

            Dim results As String = String.Empty

            Log.Debug("plugin: iPiMPClient - Received: Action {0} Filter {1} Value {2} Start {3} PageSize {4} Shuffle {5} Enqueue {6} Tracks {7}", _
                                        request.Action, _
                                        request.Filter, _
                                        request.Value, _
                                        request.Start.ToString, _
                                        request.PageSize.ToString, _
                                        request.Shuffle, _
                                        request.Enqueue, _
                                        request.Tracks)

            Select Case request.Action.ToLower

                'TVSeries
                Case "getallseries"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetAllSeriesDetails()
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getallseasons"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetAllSeasons()
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getallepisodes"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetAllEpisodes()
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getserieslist"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetAllSeries()
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getseries"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetSeries(request.Value)
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getseasons"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetSeasons(request.Value)
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getseason"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetSeason(request.Value)
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getepisodes"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetEpisodes(request.Value)
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "getepisode"
                    If isTVSeriesPresent Then
                        results = TVSeries.GetEpisode(request.Value)
                    Else
                        results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If
                Case "playepisode"
                    If isTVSeriesPresent Then
                        Dim episode As New TVEpisode
                        episode.compositeID = request.Value
                        episode.PlayEpisode()
                        results = TVSeries.IsEpisodeIDPlaying(request.Value)
                    Else
                    results = iPiMPUtils.SendString("warning", "TVSeries not loaded or wrong version")
                    End If


                    'MyVideos
                Case "getmoviefilter"
                    results = MyVideos.GetVideoFilters(request.Filter, request.Value)
                Case "getmovies"
                    If (request.Filter <> String.Empty) And (request.Value <> String.Empty) Then
                        results = MyVideos.GetMovies(request.Filter, request.Value, request.Start, request.PageSize)
                    Else
                        results = MyVideos.GetMovies()
                    End If
                Case "getmovie"
                    If Not request.Filter = String.Empty Then results = MyVideos.GetVideoInfo(request.Filter)
                Case "playmovie"
                    If Not request.Filter = String.Empty Then
                        Dim video As New MPClientController.Video
                        video.movieID = CInt(request.Filter)
                        If request.Value.ToLower = "force" Then
                            video.RemoveStopTime()
                        End If
                        video.PlayVideo()
                        results = MyVideos.IsVideoIDPlaying(CInt(request.Filter))
                    End If
                Case "getallmovies"
                    results = MyVideos.GetAllMovies()

                    'Moving Pictures
                Case "getallmovingpicture"
                    If isMovingPicturesPresent Then
                        results = MovingPictures.GetAllMovies()
                    Else
                        results = iPiMPUtils.SendString("warning", "MovingPictures not loaded or wrong version")
                    End If
                Case "getmovingpicturefilter"
                    If isMovingPicturesPresent Then
                        results = MovingPictures.GetVideoFilters(request.Filter, request.Value)
                    Else
                        results = iPiMPUtils.SendString("warning", "MovingPictures not loaded or wrong version")
                    End If
                Case "getmovingpictures"
                    If isMovingPicturesPresent Then
                        If (request.Filter <> String.Empty) And (request.Value <> String.Empty) Then
                            results = MovingPictures.GetMovies(request.Filter, request.Value, request.Start, request.PageSize)
                        Else
                            results = MovingPictures.GetMovies()
                        End If
                    Else
                        results = iPiMPUtils.SendString("warning", "MovingPictures not loaded or wrong version")
                    End If
                Case "getmovingpicture"
                    If isMovingPicturesPresent Then
                        If Not request.Filter = String.Empty Then results = MovingPictures.GetVideoInfo(request.Filter)
                    Else
                        results = iPiMPUtils.SendString("warning", "MovingPictures not loaded or wrong version")
                    End If
                Case "playmovingpicture"
                    If isMovingPicturesPresent Then
                        If Not request.Filter = String.Empty Then
                            MovingPictures.PlayMovie(CInt(request.Filter))
                            results = MovingPictures.IsVideoIDPlaying(CInt(request.Filter))
                        End If
                    Else
                        results = iPiMPUtils.SendString("warning", "MovingPictures not loaded or wrong version")
                    End If

                    'My Music
                Case "getallmusicartists"
                    results = MyMusic.GetAllArtists()
                Case "getallmusicgenres"
                    results = MyMusic.GetAllGenres()
                Case "getallmusicalbums"
                    results = MyMusic.GetAllAlbums()
                Case "getallmusicsongs"
                    results = MyMusic.GetAllSongs()
                Case "getmusicfilter"
                    results = MyMusic.GetMusicFilters(request.Filter, request.Value)
                Case "getalbums"
                    If Not (request.Filter = String.Empty) Or (request.Value = String.Empty) Then
                        results = MyMusic.GetMusicAlbums(request.Filter, request.Value, request.Start, request.PageSize)
                    Else
                        results = MyMusic.GetMusicAlbums()
                    End If
                Case "getalbum"
                    If Not request.Filter = String.Empty Then results = MyMusic.GetAlbum(request.Filter, request.Value)
                Case "gettracks"
                    If Not (request.Filter = String.Empty) Or (request.Value = String.Empty) Then
                        results = MyMusic.GetMusicTracks(request.Filter, request.Value, request.Start, request.PageSize)
                    Else
                        results = MyMusic.GetMusicTracks()
                    End If
                Case "getplaylists"
                    results = MyMusic.GetPlaylists()
                Case "getrandomplaylist"
                    results = MyMusic.GetPlaylists(True)
                Case "playrandommusic"
                    results = MyMusic.PlayRandom(request.Filter, request.Value)
                Case "playmusic"
                    results = MyMusic.PlayMusic(request.Filter, request.Value, request.Tracks, request.Shuffle, request.Enqueue)
                Case "playtracks"
                    results = MyMusic.PlayTracks(request.Tracks, request.Shuffle, request.Enqueue)
                Case "reorgmusic"

                    'MyMusic.UpdateMusicDB()
                Case "getmusiccoverart"
                    results = MyMusic.GetCoverArt(request.Filter, request.Value)
                Case "saveplaylist"
                    results = MyMusic.SaveCurrentPlaylist(request.Filter)

                    'MyTV
                Case "startchannel"
                    results = MyTV.StartChannel(request.Filter)

                Case "sendmessage"
                    Dim message As New Message
                    message.heading = request.Filter
                    message.message = request.Value
                    results = message.SendMessage()

                Case "poweroption"
                    results = PowerOptions.DoPowerOption(request.Filter)

                Case "button"
                    If Left(request.Filter.ToLower, 10) = "keystring_" Then
                        results = SendKeystring(Split(request.Filter, "_")(1))
                    Else
                        results = SendButton(request.Filter)
                    End If

                Case "ping"
                    results = iPiMPUtils.SendString("data", "pong")

                Case "version"
                    Dim VersionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
                    Dim ver As String = String.Format("{0}.{1}.{2}.{3}",
                                                       VersionInfo.Major.ToString,
                                                       VersionInfo.Minor.ToString,
                                                       VersionInfo.Build.ToString,
                                                       VersionInfo.Revision.ToString)
                    results = iPiMPUtils.SendString("version", ver)

                Case "mpversion"
                    Dim ver As String = String.Format("{0}.{1}.{2}.{3}",
                                                       My.Application.Info.Version.Major.ToString,
                                                       My.Application.Info.Version.Minor.ToString,
                                                       My.Application.Info.Version.Build.ToString,
                                                       My.Application.Info.Version.Revision.ToString)
                    results = iPiMPUtils.SendString("version", ver)

                Case "nowplaying"
                    results = NowPlaying.GetNowPlaying(isMovingPicturesPresent, isTVSeriesPresent)

                Case "volume"
                    results = iPiMPUtils.SetVolume(request.Value)

                Case "seekpercentage"
                    results = iPiMPUtils.SeekPercentage(request.Value)

                Case "getimage"
                    results = iPiMPUtils.GetImage(request.Value)

                Case "getfile"
                    results = iPiMPUtils.GetFile(request.Value, request.Filter)

                Case Else
                    results = iPiMPUtils.SendError(4, "Unknown action")

            End Select

            Return results

        End Function


#Region "Remote Control"

        Private Enum RemoteButton
            'Same as MCE buttons
            None = 0
            Power1 = 165
            Power2 = 12
            PowerTV = 101
            Record = 23
            Pause = 24
            [Stop] = 25
            Rewind = 21
            Play = 22
            Forward = 20
            Replay = 27
            Skip = 26
            Back = 35
            Up = 30
            Info = 15
            Left = 32
            Ok = 34
            Right = 33
            Down = 31
            VolumeUp = 16
            VolumeDown = 17
            Start = 13
            ChannelUp = 18
            ChannelDown = 19
            Mute = 14
            RecordedTV = 72
            Guide = 38
            LiveTV = 37
            DVDMenu = 36
            NumPad1 = 1
            NumPad2 = 2
            NumPad3 = 3
            NumPad4 = 4
            NumPad5 = 5
            NumPad6 = 6
            NumPad7 = 7
            NumPad8 = 8
            NumPad9 = 9
            NumPad0 = 0
            Oem8 = 29
            OemGate = 28
            Clear = 10
            Enter = 11
            Teletext = 90
            Red = 91
            Green = 92
            Yellow = 93
            Blue = 94

            ' MCE keyboard specific
            MyTV = 70
            MyMusic = 71
            MyPictures = 73
            MyVideos = 74
            MyRadio = 80
            Messenger = 105

            ' Special OEM buttons
            AspectRatio = 39 ' FIC Spectra
            Print = 78 ' Hewlett Packard MCE Edition

            'MPClientController specific mappings
            Home = 800
            BasicHome = 801
            Weather = 802
            Plugins = 803
            Star = 804
            Hash = 805
            TVSeries = 806
            MovingPictures = 807
            NowPlaying = 808
            PlayDVD = 809
            MyPlaylists = 810

            'Try some keyboard keys
            F2 = 820

        End Enum

        Private Function SendButton(ByVal request As String) As String

            Dim btn As RemoteButton

            Select Case request.ToLower
                Case "stop"
                    btn = RemoteButton.Stop
                Case "record"
                    btn = RemoteButton.Record
                Case "pause"
                    btn = RemoteButton.Pause
                Case "play"
                    btn = RemoteButton.Play
                Case "rewind"
                    btn = RemoteButton.Rewind
                Case "forward"
                    btn = RemoteButton.Forward
                Case "replay"
                    btn = RemoteButton.Replay
                Case "skip"
                    btn = RemoteButton.Skip
                Case "back"
                    btn = RemoteButton.Back
                Case "info"
                    btn = RemoteButton.Info
                Case "up"
                    btn = RemoteButton.Up
                Case "down"
                    btn = RemoteButton.Down
                Case "left"
                    btn = RemoteButton.Left
                Case "right"
                    btn = RemoteButton.Right
                Case "ok"
                    btn = RemoteButton.Ok
                Case "volup"
                    btn = RemoteButton.VolumeUp
                Case "voldown"
                    btn = RemoteButton.VolumeDown
                Case "volmute"
                    btn = RemoteButton.Mute
                Case "chup"
                    btn = RemoteButton.ChannelUp
                Case "chdown"
                    btn = RemoteButton.ChannelDown
                Case "dvdmenu"
                    btn = RemoteButton.DVDMenu
                Case "0"
                    btn = RemoteButton.NumPad0
                Case "1"
                    btn = RemoteButton.NumPad1
                Case "2"
                    btn = RemoteButton.NumPad2
                Case "3"
                    btn = RemoteButton.NumPad3
                Case "4"
                    btn = RemoteButton.NumPad4
                Case "5"
                    btn = RemoteButton.NumPad5
                Case "6"
                    btn = RemoteButton.NumPad6
                Case "7"
                    btn = RemoteButton.NumPad7
                Case "8"
                    btn = RemoteButton.NumPad8
                Case "9"
                    btn = RemoteButton.NumPad9
                Case "*"
                    btn = RemoteButton.Star
                Case "#"
                    btn = RemoteButton.Hash
                Case "clear"
                    btn = RemoteButton.Clear
                Case "enter"
                    btn = RemoteButton.Enter
                Case "teletext"
                    btn = RemoteButton.Teletext
                Case "red"
                    btn = RemoteButton.Red
                Case "blue"
                    btn = RemoteButton.Blue
                Case "yellow"
                    btn = RemoteButton.Yellow
                Case "green"
                    btn = RemoteButton.Green
                Case "home"
                    btn = RemoteButton.Home
                Case "basichome"
                    btn = RemoteButton.BasicHome
                Case "plugins"
                    btn = RemoteButton.Plugins
                Case "pictures"
                    btn = RemoteButton.MyPictures
                Case "music"
                    btn = RemoteButton.MyMusic
                Case "nowplaying"
                    btn = RemoteButton.NowPlaying
                Case "radio"
                    btn = RemoteButton.MyRadio
                Case "tv"
                    btn = RemoteButton.MyTV
                Case "tvguide"
                    btn = RemoteButton.Guide
                Case "tvrecs"
                    btn = RemoteButton.RecordedTV
                Case "videos"
                    btn = RemoteButton.MyVideos
                Case "tvseries"
                    btn = RemoteButton.TVSeries
                Case "weather"
                    btn = RemoteButton.Weather
                Case "movingpictures"
                    btn = RemoteButton.MovingPictures
                Case "dvd"
                    btn = RemoteButton.PlayDVD
                Case "playlists"
                    btn = RemoteButton.MyPlaylists
                Case "f2"
                    btn = RemoteButton.F2
                Case Else
                    btn = RemoteButton.Ok
            End Select

            remoteHandler.MapAction(btn)
            System.Threading.Thread.Sleep(100)
            Log.Debug("plugin: iPiMPClient - Pressed button {0}", btn.ToString)

            Return iPiMPUtils.SendBool(True)

        End Function

        Private Function SendKeystring(ByVal request As String) As String

            Dim keystring As Integer = 0
            Dim modifiers As Integer = 0
            Dim modifier As String = String.Empty

            If InStr(request, "+") > 0 Then
                modifier = Split(request, "+")(0)
                request = Split(request, "+")(1)
            End If

            Select Case modifier.ToLower
                Case "ctrl"
                    modifier = 2
                Case "shift"
                    modifier = 1
                Case "alt"
                    modifier = 0
                Case Else
                    modifier = 0
            End Select

            Select Case request.ToLower
                Case "0"
                    keystring = Keys.D0
                Case "1"
                    keystring = Keys.D1
                Case "2"
                    keystring = Keys.D2
                Case "3"
                    keystring = Keys.D3
                Case "4"
                    keystring = Keys.D4
                Case "5"
                    keystring = Keys.D5
                Case "6"
                    keystring = Keys.D6
                Case "7"
                    keystring = Keys.D7
                Case "8"
                    keystring = Keys.D8
                Case "9"
                    keystring = Keys.D9

                Case "a"
                    keystring = Keys.A
                Case "b"
                    keystring = Keys.B
                Case "c"
                    keystring = Keys.C
                Case "d"
                    keystring = Keys.D
                Case "e"
                    keystring = Keys.E
                Case "f"
                    keystring = Keys.F
                Case "g"
                    keystring = Keys.G
                Case "h"
                    keystring = Keys.H
                Case "i"
                    keystring = Keys.I
                Case "j"
                    keystring = Keys.J
                Case "k"
                    keystring = Keys.K
                Case "l"
                    keystring = Keys.L
                Case "m"
                    keystring = Keys.M
                Case "n"
                    keystring = Keys.N
                Case "o"
                    keystring = Keys.O
                Case "p"
                    keystring = Keys.P
                Case "q"
                    keystring = Keys.Q
                Case "r"
                    keystring = Keys.R
                Case "s"
                    keystring = Keys.S
                Case "t"
                    keystring = Keys.T
                Case "u"
                    keystring = Keys.U
                Case "v"
                    keystring = Keys.V
                Case "w"
                    keystring = Keys.W
                Case "x"
                    keystring = Keys.X
                Case "y"
                    keystring = Keys.Y
                Case "z"
                    keystring = Keys.Z

                Case "f1"
                    keystring = Keys.F1
                Case "f2"
                    keystring = Keys.F2
                Case "f3"
                    keystring = Keys.F3
                Case "f4"
                    keystring = Keys.F4
                Case "f5"
                    keystring = Keys.F5
                Case "f6"
                    keystring = Keys.F6
                Case "f7"
                    keystring = Keys.F7
                Case "f8"
                    keystring = Keys.F8
                Case "f9"
                    keystring = Keys.F9
                Case "f10"
                    keystring = Keys.F10
                Case "f11"
                    keystring = Keys.F11
                Case "f12"
                    keystring = Keys.F12

                Case "pageup"
                    keystring = Keys.PageUp
                Case "pagedown"
                    keystring = Keys.PageDown
                Case "tab"
                    keystring = Keys.Tab
                Case "esc", "escape"
                    keystring = Keys.Escape
                Case "home"
                    keystring = Keys.Home
                Case "end"
                    keystring = Keys.End
                Case "del", "delete"
                    keystring = Keys.Delete
                Case "enter", "return", "rtn"
                    keystring = Keys.Enter
                Case " ", "space"
                    keystring = Keys.Space

                Case Else
                    keystring = 0

            End Select

            Dim key As MediaPortal.GUI.Library.Key = New MediaPortal.GUI.Library.Key(keystring + (modifiers * 32), 0)
            Dim action As MediaPortal.GUI.Library.Action = New Action(key, MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED, 0, 0)

            GUIWindowManager.OnAction(action)

            'SendKeys.Send(String.Format("{0}{1}", modifier, keystring))
            System.Threading.Thread.Sleep(100)
            Log.Debug("plugin: iPiMPClient - Sent keystring - modifier {0}, keychar {1}", modifier, keystring)
            Log.Debug("plugin: iPiMPClient - {0}", action.ToString)

            Return iPiMPUtils.SendBool(True)

        End Function

#End Region

        Public Class MPClientRequest
            Public Action As String
            Public Filter As String
            Public Value As String
            Public Start As Integer
            Public PageSize As Integer
            Public Shuffle As Boolean
            Public Enqueue As Boolean
            Public Tracks As String
        End Class

    End Class

End Namespace

