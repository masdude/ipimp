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
Imports System.IO
Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Namespace uWiMP.TVServer

    Public Class MPClientHTTPRemoting

        Private Shared port As Integer
        Private Shared host As String

        Const DEFAULT_PORT As Integer = 55667

        Public Shared Function CanConnect(ByVal friendly As String) As Boolean

            Dim result As Boolean = False

            Dim request As New uWiMP.TVServer.MPClient.Request
            request.Action = "ping"

            Dim response As String = SendHTTPPostMessage(friendly, request)
            Dim jo As JsonObject = CType(JsonConvert.Import(response), JsonObject)
            Dim success As Boolean = CType(jo("result"), Boolean)
            If success Then result = True

            Return result

        End Function

        Public Shared Function SendHTTPPostMessage(ByVal friendly As String, ByVal mpRequest As uWiMP.TVServer.MPClient.Request, Optional ByVal sync As Boolean = True) As String

            Dim response As String = String.Empty
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

            Dim data As String = MPClientRemoting.ConvertRequestToJson(mpRequest)

            Try
                Dim webRequest As WebRequest = webRequest.Create(String.Format("http://{0}:{1}/mpcc/", host, (port + 1).ToString))
                webRequest.Method = "POST"
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(data)
                webRequest.ContentType = "application/x-www-form-urlencoded"
                webRequest.ContentLength = byteArray.Length

                Dim datastream As Stream = webRequest.GetRequestStream
                datastream.Write(byteArray, 0, byteArray.Length)
                datastream.Close()

                Dim webResponse As WebResponse = webRequest.GetResponse()
                datastream = webResponse.GetResponseStream

                Dim reader As New StreamReader(datastream)
                response = reader.ReadToEnd

                reader.Close()
                datastream.Close()
                webResponse.Close()

            Catch ex As Exception
                Return MPClientRemoting.ReturnBoolean(False)
            End Try

            If sync Then
                Return response
            Else
                Return MPClientRemoting.ReturnBoolean(True)
            End If

        End Function

    End Class

End Namespace
