Imports System.Data
Imports System.Data.SQLite

Namespace uWiMP.TVServer

    Public Class MPClientDatabase

        Private Shared Function IsExistingClient(ByVal friendly As String) As Boolean

            Dim clientExists As Boolean = False

            Dim connStr As String = ConfigurationManager.ConnectionStrings("uWiMPConnString").ConnectionString
            Dim conn As SQLiteConnection = New SQLiteConnection(connStr)
            Dim reader As SQLiteDataReader

            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Friendly FROM `MPClients` WHERE Friendly = $Friendly", conn)
            cmd.Parameters.Add("$Friendly", DbType.String, 255).Value = friendly.ToLower

            Try
                conn.Open()
                reader = cmd.ExecuteReader(CommandBehavior.Default)
                If reader.HasRows Then clientExists = True
            Catch ex As SQLiteException
                Return False
            Finally
                conn.Close()
            End Try

            Return clientExists

        End Function

        Public Shared Function GetClients() As List(Of uWiMP.TVServer.MPClient.Client)

            Dim mpclients As New List(Of uWiMP.TVServer.MPClient.Client)


            Dim connStr As String = ConfigurationManager.ConnectionStrings("uWiMPConnString").ConnectionString
            Dim conn As SQLiteConnection = New SQLiteConnection(connStr)
            Dim reader As SQLiteDataReader

            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Friendly FROM `MPClients`", conn)

            Try
                conn.Open()
                reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        Dim mpclient As New uWiMP.TVServer.MPClient.Client
                        mpclient.Friendly = reader.GetString(0)
                        mpclients.Add(mpclient)
                    End While
                End If
                reader.Close()
            Catch ex As SQLiteException
                Return Nothing
            Finally
                conn.Close()
            End Try

            Return mpclients

        End Function

        Public Shared Function GetClient(ByVal friendly As String) As uWiMP.TVServer.MPClient.Client

            Dim MPClient As New uWiMP.TVServer.MPClient.Client

            Dim connStr As String = ConfigurationManager.ConnectionStrings("uWiMPConnString").ConnectionString
            Dim conn As SQLiteConnection = New SQLiteConnection(connStr)
            Dim reader As SQLiteDataReader

            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Hostname, MACAddress, Port FROM `MPClients` WHERE Friendly = $Friendly", conn)
            cmd.Parameters.Add("$Friendly", DbType.String, 255).Value = friendly.ToLower

            Try
                conn.Open()
                reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        MPClient.Friendly = friendly
                        MPClient.Hostname = reader.GetString(0)
                        MPClient.MACAddress = reader.GetString(1)
                        MPClient.Port = reader.GetString(2)
                    End While
                End If
                reader.Close()
            Catch ex As SQLiteException
                Return MPClient
            Finally
                conn.Close()
            End Try

            Return MPClient

        End Function

        Public Shared Function ManageClient(ByVal client As uWiMP.TVServer.MPClient.Client, ByVal action As String) As Boolean

            If (action.ToLower = "insert") And (IsExistingClient(client.Friendly)) Then Return False

            Dim connStr As String = ConfigurationManager.ConnectionStrings("uWiMPConnString").ConnectionString
            Dim conn As SQLiteConnection = New SQLiteConnection(connStr)

            Dim cmd As New SQLiteCommand
            cmd.Connection = conn
            cmd.Parameters.Add("$Friendly", DbType.String, 255).Value = client.Friendly.ToLower

            Select Case action.ToLower
                Case "delete"
                    cmd.CommandText = "DELETE FROM `MPClients` WHERE Friendly = $Friendly"
                Case "insert"
                    cmd.CommandText = "INSERT INTO `MPClients` (Friendly, Hostname, Port, MACAddress) VALUES ($Friendly, $Hostname, $Port, $MACAddress)"
                Case "update"
                    cmd.CommandText = "UPDATE `MPClients` SET Hostname = $Hostname, Port = $Port, MACAddress = $MACAddress WHERE Friendly = $Friendly"
            End Select

            If (action.ToLower = "insert") Or (action.ToLower = "update") Then
                cmd.Parameters.Add("$Hostname", DbType.String, 255).Value = client.Hostname.ToLower
                cmd.Parameters.Add("$Port", DbType.String, 255).Value = client.Port.ToString
                cmd.Parameters.Add("$MACAddress", DbType.String, 255).Value = client.MACAddress.ToString
            End If

            Dim rows As Integer = 0

            Try
                conn.Open()
                rows = cmd.ExecuteNonQuery
            Catch ex As SQLiteException
                Return False
            Finally
                conn.Close()
            End Try

            If rows > 0 Then
                Return True
            Else
                Return False
            End If

        End Function

    End Class

End Namespace
