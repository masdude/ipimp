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

Imports System.Net
Imports Jayrock.Json

Namespace uWiMP.TVServer

    Public Class MPClientTCPRemoting


        Private Shared client As Sockets.TcpClient
        Private Shared port As Integer
        Private Shared host As String

        Const BUFFER As Integer = 256
        Const DEFAULT_PORT As Integer = 55667

        Private Shared Function Connect(ByVal friendly As String) As Boolean

            Try
                client = New Sockets.TcpClient
                Dim mpclient As TVServer.MPClient.Client = uWiMP.TVServer.MPClientDatabase.GetClient(friendly)

                If mpclient.Hostname = String.Empty Then
                    host = friendly
                Else
                    host = mpclient.Hostname
                End If

                If mpclient.Port = String.Empty Then
                    port = DEFAULT_PORT
                Else
                    port = CInt(mpclient.Port)
                End If

                client.Connect(host, port)

            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

        Private Shared Sub Disconnect(ByVal friendly As String)
            Try
                client.Close()
            Catch ex As Exception

            End Try

        End Sub

        Public Shared Function CanConnect(ByVal friendly As String) As Boolean

            Dim result As Boolean = False
            Try
                result = Connect(friendly)
            Catch ex As Exception
                result = False
            Finally
                Disconnect(friendly)
            End Try

            Return result

        End Function

        Public Shared Function SendSyncMessage(ByVal friendly As String, ByVal mpRequest As uWiMP.TVServer.MPClient.Request) As String

            Dim response As [String] = [String].Empty

            Try
                Dim data As [Byte]() = System.Text.Encoding.UTF8.GetBytes(ConvertRequestToJson(mpRequest))

                Connect(friendly)

                Dim stream As Sockets.NetworkStream = client.GetStream()

                stream.Write(data, 0, data.Length)

                Dim timeout As Integer = 0
                Do Until stream.DataAvailable
                    timeout += 100
                    Threading.Thread.Sleep(100)
                    If timeout >= 10000 Then Throw New Exception
                Loop

                data = New [Byte](BUFFER) {}
                Dim bytes As Int32
                While stream.DataAvailable
                    bytes = stream.Read(data, 0, data.Length)
                    response += System.Text.Encoding.UTF8.GetString(data, 0, bytes)
                End While

                stream.Close()

            Catch ex As Exception
                Return "fail"
            Finally
                Disconnect(friendly)
            End Try

            Return response

        End Function

        Public Shared Function SendAsyncMessage(ByVal friendly As String, ByVal mpRequest As uWiMP.TVServer.MPClient.Request) As String

            Try
                Connect(friendly)

                Dim data As [Byte]() = System.Text.Encoding.UTF8.GetBytes(ConvertRequestToJson(mpRequest))
                Dim stream As Sockets.NetworkStream = client.GetStream()

                stream.Write(data, 0, data.Length)
                stream.Close()

            Catch ex As Exception
                Return ReturnBoolean(False)
            Finally
                Disconnect(friendly)
            End Try

            Return ReturnBoolean(True)

        End Function

        Friend Shared Function ReturnBoolean(ByVal value As Boolean) As String

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(value)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Friend Shared Function ConvertRequestToJson(ByVal request As uWiMP.TVServer.MPClient.Request) As String

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("action")
            jw.WriteString(request.Action)
            jw.WriteMember("filter")
            jw.WriteString(request.Filter)
            jw.WriteMember("value")
            jw.WriteString(request.Value)
            jw.WriteMember("start")
            jw.WriteString(request.Start)
            jw.WriteMember("pagesize")
            jw.WriteString(request.PageSize)
            jw.WriteMember("shuffle")
            jw.WriteBoolean(request.Shuffle)
            jw.WriteMember("enqueue")
            jw.WriteBoolean(request.Enqueue)
            jw.WriteMember("tracks")
            jw.WriteString(request.Tracks)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

    End Class

End Namespace
