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

Imports Jayrock.Json

Namespace uWiMP.TVServer

    Public Class MPClientRemoting

        Const useTCP As Boolean = False

        
        Public Shared Function CanConnect(ByVal friendly As String) As Boolean

            If useTCP Then
                Return MPClientTCPRemoting.CanConnect(friendly)
            Else
                Return MPClientHTTPRemoting.CanConnect(friendly)
            End If

        End Function

        Public Shared Function SendSyncMessage(ByVal friendly As String, ByVal mpRequest As uWiMP.TVServer.MPClient.Request) As String

            If useTCP Then
                Return MPClientTCPRemoting.SendSyncMessage(friendly, mpRequest)
            Else
                Return MPClientHTTPRemoting.SendHTTPPostMessage(friendly, mpRequest)
            End If

        End Function

        Public Shared Function SendAsyncMessage(ByVal friendly As String, ByVal mpRequest As uWiMP.TVServer.MPClient.Request) As String

            If useTCP Then
                Return MPClientTCPRemoting.SendSyncMessage(friendly, mpRequest)
            Else
                Return MPClientHTTPRemoting.SendHTTPPostMessage(friendly, mpRequest, False)
            End If

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
