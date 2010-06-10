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


Imports TvDatabase
Imports TvControl
Imports uWiMP.TVServer.MPWebServices.Classes

Namespace uWiMP.TVServer

    Public Class Cards

        Private Shared isConnected As Boolean = False

        ''' <summary>
        ''' Card statuses
        ''' </summary>
        ''' <remarks></remarks>
        Enum Status
            idle = 0
            grabbing = 1
            scanning = 2
            timeshifting = 3
            recording = 4
            unknown = 9
        End Enum

        ''' <summary>
        ''' Connects to the TV server.
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub SetupConnection()
            Try
                If Not isConnected Then
                    RemoteControl.Clear()
                    RemoteControl.HostName = Utilities.GetAppConfig("HOSTNAME")
                    Dim connString As String = String.Empty
                    Dim dataProvider As String = String.Empty
                    RemoteControl.Instance.GetDatabaseConnectionString(connString, dataProvider)
                    Gentle.Framework.ProviderFactory.SetDefaultProviderConnectionString(connString)
                    isConnected = True
                End If
            Catch ex As Exception
                RemoteControl.Clear()
                isConnected = False
            End Try
        End Sub

        ''' <summary>
        ''' Retrieves a list of installed TV cards.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCards() As List(Of Card)

            Return Card.ListAll

        End Function

        ''' <summary>
        ''' Retrieves a specific TV card.
        ''' </summary>
        ''' <param name="idCard"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCard(ByVal idCard As Integer) As Card

            Return Card.Retrieve(idCard)

        End Function

        Public Shared Function GetClients() As List(Of TvLibrary.Streaming.RtspClient)

            SetupConnection()

            Return RemoteControl.Instance.StreamingClients()

        End Function

        ''' <summary>
        ''' Retrieves the status of a TV card.
        ''' </summary>
        ''' <param name="card">The required TV card.</param>
        ''' <returns>uWiMP.TVServer.Cards.Status</returns>
        ''' <remarks></remarks>
        Public Shared Function GetCardStatus(ByVal card As Card) As Status

            SetupConnection()

            Dim user As User = New User()
            Dim vcard As VirtualCard
            user.CardId = card.IdCard
            Dim usersForCard As User() = RemoteControl.Instance.GetUsersForCard(card.IdCard)
            Dim status As Status = Cards.Status.idle

            If usersForCard.Length = 0 Then
                vcard = New VirtualCard(user, RemoteControl.HostName)
                Try
                    If vcard.IsScanning Then status = Cards.Status.scanning
                    If vcard.IsRecording Then status = Cards.Status.recording
                    If vcard.IsGrabbingEpg Then status = Cards.Status.grabbing
                    If vcard.IsTimeShifting Then status = Cards.Status.timeshifting
                Catch ex As Exception

                End Try
            Else
                Dim i As Integer = 0
                For i = 0 To usersForCard.Length - 1
                    vcard = New VirtualCard(usersForCard(i), RemoteControl.HostName)
                    Try
                        If vcard.IsScanning Then status = Cards.Status.scanning
                        If vcard.IsRecording Then status = Cards.Status.recording
                        If vcard.IsGrabbingEpg Then status = Cards.Status.grabbing
                        If vcard.IsTimeShifting Then status = Cards.Status.timeshifting
                    Catch ex As Exception

                    End Try
                Next

            End If

            Return status

        End Function

        ''' <summary>
        ''' Retrieves additional status information for an busy TV card.
        ''' </summary>
        ''' <param name="card">The required TV card.</param>
        ''' <param name="status">The status of the card. (uWiMP.TVServer.Cards.Status)</param>
        ''' <returns></returns>
        ''' <remarks>Returns the recording ID or channel ID if recording or timeshifting.</remarks>
        Public Shared Function GetCardUsageStatus(ByVal card As Card, ByVal status As Status) As String

            SetupConnection()

            Dim user As User = New User()
            Dim vcard As VirtualCard
            user.CardId = card.IdCard
            Dim usersForCard As User() = RemoteControl.Instance.GetUsersForCard(card.IdCard)
            Dim id As Integer = 0
            Dim username As String = "unknown"

            If usersForCard.Length = 0 Then
                vcard = New VirtualCard(user, RemoteControl.HostName)
                Try
                    If vcard.IsRecording And status = Cards.Status.recording Then id = vcard.RecordingScheduleId
                    If vcard.IsTimeShifting And status = Cards.Status.timeshifting Then id = vcard.IdChannel
                Catch ex As Exception
                    id = 0
                End Try
            Else
                Dim i As Integer = 0
                For i = 0 To usersForCard.Length - 1
                    vcard = New VirtualCard(usersForCard(i), RemoteControl.HostName)
                    username = vcard.User.Name
                    Try
                        If vcard.IsRecording And status = Cards.Status.recording Then id = vcard.RecordingScheduleId
                        If vcard.IsTimeShifting And status = Cards.Status.timeshifting Then id = vcard.IdChannel
                    Catch ex As Exception
                        id = 0
                    End Try
                Next
            End If

            Return String.Format("{0},{1}", username, id)

        End Function

        ''' <summary>
        ''' Retrieves the program ID for a specific MPuWiMP.TVServer.MPClient.Client.
        ''' </summary>
        ''' <param name="clientName">The name of the requireduWiMP.TVServer.MPClient.Client.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProgramIDForClient(ByVal clientName As String) As Integer

            SetupConnection()

            Dim user As User = New User()
            Dim vcard As VirtualCard

            For Each card As Card In GetCards()

                user.CardId = card.IdCard
                Dim usersForCard As User() = RemoteControl.Instance.GetUsersForCard(card.IdCard)
                If Not usersForCard Is Nothing Then
                    If usersForCard.Length > 0 Then
                        For i As Integer = 0 To usersForCard.Length - 1
                            vcard = New VirtualCard(usersForCard(i), RemoteControl.HostName)
                            If vcard.User.Name.ToLower = clientName.ToLower Then
                                Try
                                    Return Channels.GetChannelByChannelId(vcard.IdChannel).CurrentProgram.IdProgram
                                Catch ex As Exception
                                    Return -1
                                End Try
                            End If
                        Next
                    End If
                End If
            Next

            Return -1

        End Function

        ''' <summary>
        ''' Stops and deletes an active recording.
        ''' </summary>
        ''' <param name="card"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function StopRecording(ByVal card As Card) As Boolean

            SetupConnection()

            Dim user As User
            Dim vcard As VirtualCard
            Dim usersForCard As User() = RemoteControl.Instance.GetUsersForCard(card.IdCard)

            For Each user In usersForCard
                vcard = New VirtualCard(user, RemoteControl.HostName)
                Try
                    If vcard.IsRecording Then
                        Dim recording As Recording = recording.Retrieve(vcard.RecordingFileName)
                        vcard.StopRecording()
                        recording.Delete()
                        Return True
                    End If
                Catch ex As Exception
                    Return False
                End Try
            Next

        End Function

        ''' <summary>
        ''' Kicks a user from a TV card if timeshifting.
        ''' </summary>
        ''' <param name="card"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function StopTimeshifting(ByVal card As Card) As Boolean

            SetupConnection()

            Dim user As User
            Dim vcard As VirtualCard
            Dim usersForCard As User() = RemoteControl.Instance.GetUsersForCard(card.IdCard)

            For Each user In usersForCard
                vcard = New VirtualCard(user, RemoteControl.HostName)
                Try
                    If vcard.IsTimeShifting Then
                        vcard.StopTimeShifting()
                        Return True
                    End If
                Catch ex As Exception
                    Return False
                End Try
            Next

        End Function

        Public Shared Function StopTimeshifting(ByVal idChannel As Integer, ByVal idCard As Integer, ByVal userName As String) As Boolean

            SetupConnection()

            Dim user As New User(userName, False, idCard)
            user.IdChannel = idChannel
            Return RemoteControl.Instance.StopTimeShifting((user))

        End Function

        Public Shared Function StartTimeshifting(ByVal idChannel As Integer) As WebTvResult

            SetupConnection()

            Dim vcard As VirtualCard
            Dim result As TvResult
            Dim rtspURL As String = String.Empty
            Dim timeshiftFilename As String = String.Empty
            Dim userId As New TvControl.User(System.Guid.NewGuid().ToString("B"), False)
            Dim uWiMPTVResult As New WebTvResult
            Try
                result = RemoteControl.Instance.StartTimeShifting(userId, idChannel, vcard)
            Catch generatedExceptionName As Exception
                Return New WebTvResult
            End Try

            Dim r As New WebTvResult

            If result = TvResult.Succeeded Then
                userId.IdChannel = idChannel
                userId.CardId = vcard.Id
                rtspURL = vcard.RTSPUrl
                timeshiftFilename = vcard.TimeShiftFileName

                Dim u As New WebTvServerUser
                u.idCard = vcard.Id
                u.idChannel = idChannel
                u.name = userId.Name

                r.result = CInt(result)
                r.rtspURL = vcard.RTSPUrl
                r.timeshiftFile = vcard.TimeShiftFileName
                r.user = u

                Return r
            Else
                Return r
            End If

        End Function

        Public Shared Sub SendHeartbeat(ByVal idChannel As Integer, ByVal idCard As Integer, ByVal userID As String)

            SetupConnection()
            Dim user As New User(userID, False, idCard)
            user.IdChannel = idChannel
            Try
                RemoteControl.Instance.HeartBeat(user)
            Catch generatedExceptionName As Exception
            End Try

        End Sub

    End Class

End Namespace
