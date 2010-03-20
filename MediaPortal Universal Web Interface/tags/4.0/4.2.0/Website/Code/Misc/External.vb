Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Runtime.InteropServices

Namespace uWiMP.TVServer.External

    Public Class DiskSpace

        '
        ' Drive space code from http://addressof.com/blog/posts/267.aspx
        '
        Private Declare Auto Function GetDiskFreeSpaceEx Lib "kernel32.dll" ( _
            ByVal drive As String, _
            ByRef freeBytesAvailableToCaller As System.UInt64, _
            ByRef totalNumberOfBytes As System.UInt64, _
            ByRef totalNumberOfFreeBytes As System.UInt64) As Integer

        Public Shared Function AvailableSpace(ByVal rootPath As String) As Int64
            '  receives the total number of free bytes on the disk that are available to the user associated with the calling thread. 
            '  If per-user quotas are in use, this value may be less than the total number of free bytes on the disk.
            If Not rootPath Is Nothing Then
                Dim uavailable As System.UInt64
                Dim utotal As System.UInt64
                Dim ufree As System.UInt64
                If GetDiskFreeSpaceEx(rootPath, uavailable, utotal, ufree) <> 0 Then
                    Return System.Convert.ToInt64(uavailable)
                Else
                    Dim e As Integer = Marshal.GetLastWin32Error
                    If e = 21 Then ' device not ready.
                        Return 0
                    Else
                        Throw New System.IO.IOException("Win32 error # " & e & " occured.")
                    End If
                End If
            Else
                Throw New System.IO.DirectoryNotFoundException("Invalid path specified.")
            End If
        End Function

    End Class

    '
    ' WOL code from http://www.lucamauri.com/snippet/snip.aspx?id=6
    '
    Public Class WOL
        Const lenHeader As Integer = 6
        Const lenMAC As Integer = 6
        Const repMAC As Integer = 16

        Private Shared mEndPoint As IPEndPoint
        Private Shared mMACAddress As Byte()
        Private Shared mBytesSent As Integer = 0
        Private Shared mPacketSent As String

        ''' <summary>
        ''' The IPEndPoint object that will act as a trasport for the packet.
        ''' It is automatically created by New statement, but you can modify it or read it.
        ''' </summary>
        ''' <value>An IPEndPoint object</value>
        ''' <returns>An IPEndPoint object</returns>
        ''' <remarks>Normally there is no need to change this manually.</remarks>
        Public Property endPoint() As IPEndPoint
            Get
                Return mEndPoint
            End Get
            Set(ByVal value As IPEndPoint)
                mEndPoint = value
            End Set
        End Property

        ''' <summary>
        ''' The target machine Network Interface Card MAC address.
        ''' It must be dash-separated, i.e. in the 11-22-33-44-55-66 form
        ''' </summary>
        ''' <value>A string with dash-separated values</value>
        ''' <returns>A string with dash-separated values</returns>
        ''' <remarks>The standard (IEEE 802) separator for cotet are dash (-) and semicolon (:). I resolved to use dashes only in order to avoid any possible confusion and misunderstanding with upcoming IPv6 addressing space.</remarks>
        Public Shared Property macAddress() As String
            Get
                Dim textMAC As New StringBuilder

                For Each currByte As Byte In mMACAddress
                    textMAC.Append("-")
                    textMAC.Append(currByte.ToString("X2"))
                Next

                Return textMAC.ToString.Substring(1)
            End Get
            Set(ByVal value As String)
                Dim values As Byte() = Nothing

                For Each currByte As String In value.Split("-")
                    If values Is Nothing Then
                        ReDim values(0)
                    Else
                        ReDim Preserve values(values.GetUpperBound(0) + 1)
                    End If
                    values(values.GetUpperBound(0)) = Byte.Parse(currByte, Globalization.NumberStyles.HexNumber)
                Next

                mMACAddress = values
            End Set
        End Property

        ''' <summary>
        ''' Total bytes sent by WakeIt method. It is 0 until the method is called at least once for this class instance.
        ''' </summary>
        ''' <returns>Integer value, total bytes trasmitted</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property bytesSent() As Integer
            Get
                Return mBytesSent
            End Get
        End Property

        ''' <summary>
        ''' It represent the Magic Packet broadcasted.
        ''' </summary>
        ''' <returns>String containing the text parsing of the Magic Packet</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property packetSent() As String
            Get
                Return mPacketSent
            End Get
        End Property

        ''' <summary>
        ''' Creates a WOL Magic Packet, the datagram that will awake the target PC updon broadcast on the network.
        ''' </summary>
        ''' <param name="macAddress">An array of byte representing the target machine Network Interface Card MAC address</param>
        ''' <returns>An array of byte representing the Magic Packet</returns>
        ''' <remarks>This method can be used indipendently from the rest of the class. If necessary it can create a Magic Packet just providing the MAC address.</remarks>
        Private Shared Function magicPacket(ByVal macAddress As Byte()) As Byte()
            Dim payloadData As Byte() = Nothing
            Dim packet As New StringBuilder

            Try
                ReDim payloadData(lenHeader + lenMAC * repMAC)

                For i As Integer = 0 To lenHeader - 1
                    payloadData(i) = Byte.Parse("FF", Globalization.NumberStyles.HexNumber)
                Next
                For i As Integer = 0 To repMAC - 1
                    For j As Integer = 0 To lenMAC - 1
                        payloadData(lenHeader + i * lenMAC + j) = macAddress(j)
                    Next
                Next

                For Each currLoad As Byte In payloadData
                    packet.Append("-")
                    packet.Append(currLoad.ToString("X2"))
                Next

                mPacketSent = packet.ToString.Substring(1)
            Catch ex As Exception
                mPacketSent = "EXCEPTION: " & ex.ToString
            End Try

            Return payloadData
        End Function

        Private Shared Function sendUDP(ByVal payload As Byte(), ByVal endPoint As IPEndPoint) As Integer
            Dim byteSend As Integer
            Dim socketClient As Socket

            If (payload IsNot Nothing) AndAlso (endPoint IsNot Nothing) Then
                socketClient = New Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
                socketClient.Connect(endPoint)
                byteSend = socketClient.Send(payload, 0, payload.Length, SocketFlags.None)

                socketClient.Close()
            Else
                byteSend = 0
            End If

            Return byteSend
        End Function

        ''' <summary>
        ''' It is the main method of the class. It must be called after the MAC address has been set. It does not return any code, you can see the result of the operation with the bytesSent and packetSent properties of this class.
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub wakeIt()
            mBytesSent = sendUDP(magicPacket(mMACAddress), mEndPoint)
        End Sub

        ''' <summary>
        ''' No parameter is required. The new statement just created the IPEndPoint object.
        ''' </summary>
        ''' <remarks>The default IPEndPoint transmit on port 7. Other choices for WOL are port 0 or 9</remarks>
        Sub New()
            mEndPoint = New IPEndPoint(IPAddress.Broadcast, 7)
        End Sub

        ''' <summary>
        ''' The IPEndPoint object is created to the specified port
        ''' </summary>
        ''' <param name="epPort">A valid port number</param>
        ''' <remarks>If the port number is invalid, the IPEndPoint is created to port 7. Ports normally used for WOL are 0, 7 or 9.</remarks>
        Public Shared Sub Init(ByVal epPort As Integer)
            If epPort >= 0 AndAlso epPort < 65535 Then
                mEndPoint = New IPEndPoint(IPAddress.Broadcast, epPort)
            Else
                mEndPoint = New IPEndPoint(IPAddress.Broadcast, 7)
            End If
        End Sub
    End Class

End Namespace

