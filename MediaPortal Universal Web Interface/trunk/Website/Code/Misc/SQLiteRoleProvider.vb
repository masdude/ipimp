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


Imports System.Configuration.Provider
Imports System.Data.SQLite

Namespace uWiMP.TVServer

    '
    ' SQLiteRoleProvider code from http://www.eggheadcafe.com/articles/20051119.asp
    ' then converted to vb.net
    Public Class SQLiteRoleProvider

        Inherits RoleProvider

        '
        ' Global connection string, generic exception message, event log info.
        '

        Private rolesTable As String = "Roles"
        Private usersInRolesTable As String = "UsersInRoles"

        Private eventSource As String = "SQLiteRoleProvider"
        Private eventLog As String = "Application"
        Private exceptionMessage As String = "An exception occurred. Please check the Event Log."

        Private pConnectionStringSettings As ConnectionStringSettings
        Private connectionString As String


        '
        ' If false, exceptions are thrown to the caller. If true,
        ' exceptions are written to the event log.
        '

        Private pWriteExceptionsToEventLog As Boolean = False

        Public Property WriteExceptionsToEventLog() As Boolean
            Get
                Return pWriteExceptionsToEventLog
            End Get
            Set(ByVal Value As Boolean)
                pWriteExceptionsToEventLog = Value
            End Set
        End Property



        '
        ' System.Configuration.Provider.ProviderBase.Initialize Method
        '

        Public Overrides Sub Initialize(ByVal name As String, ByVal config As NameValueCollection)


            '
            ' Initialize values from web.config.
            '

            If config Is Nothing Then Throw New ArgumentNullException("config")

            If (name = Nothing Or name.Length = 0) Then name = "SQLiteRoleProvider"

            If (String.IsNullOrEmpty(config("description"))) Then

                config.Remove("description")
                config.Add("description", "Sample SQLite Role provider")

            End If

            ' Initialize the abstract base class.
            MyBase.Initialize(name, config)


            If config("applicationName") Is Nothing Or config("applicationName").Trim() = "" Then
                pApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath
            Else
                pApplicationName = config("applicationName")
            End If


            If Not config("writeExceptionsToEventLog") Is Nothing Then
                If config("writeExceptionsToEventLog").ToUpper() = "TRUE" Then
                    pWriteExceptionsToEventLog = True
                End If
            End If

            '
            ' Initialize SQLiteConnection.
            '

            pConnectionStringSettings = ConfigurationManager.ConnectionStrings(config("connectionStringName"))

            If pConnectionStringSettings Is Nothing Or pConnectionStringSettings.ConnectionString.Trim() = "" Then
                Throw New ProviderException("Connection String cannot be blank.")
            End If

            connectionString = pConnectionStringSettings.ConnectionString

        End Sub

        '
        ' System.Web.Security.RoleProvider properties.
        '


        Private pApplicationName As String


        Public Overrides Property ApplicationName() As String
            Get
                Return pApplicationName
            End Get
            Set(ByVal Value As String)
                pApplicationName = Value
            End Set
        End Property

        '
        ' System.Web.Security.RoleProvider methods.
        '

        '
        ' RoleProvider.AddUsersToRoles
        '

        Public Overrides Sub AddUsersToRoles(ByVal usernames As String(), ByVal rolenames As String())

            Dim rolename As String
            For Each rolename In rolenames
                If Not RoleExists(rolename) Then Throw New ProviderException("Role name not found.")
            Next

            Dim username As String
            For Each username In usernames
                If username.IndexOf(","c) > 0 Then Throw New ArgumentException("User names cannot contain commas.")

                For Each rolename In rolenames
                    If IsUserInRole(username.ToLower, rolename) Then Throw New ProviderException("User is already in role.")
                Next
            Next


            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("INSERT INTO `" + usersInRolesTable + "`" & _
                    " (Username, Rolename, ApplicationName) " & _
                    " Values($Username, $Rolename, $ApplicationName)", conn)


            Dim userParm As SQLiteParameter = cmd.Parameters.Add("$Username", DbType.String, 255)
            Dim roleParm As SQLiteParameter = cmd.Parameters.Add("$Rolename", DbType.String, 255)
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim tran As SQLiteTransaction = Nothing

            Try
                conn.Open()
                tran = conn.BeginTransaction()
                cmd.Transaction = tran

                For Each username In usernames
                    For Each rolename In rolenames
                        userParm.Value = username.ToLower
                        roleParm.Value = rolename
                        cmd.ExecuteNonQuery()
                    Next
                Next

                tran.Commit()
            Catch e As SQLiteException
                Try
                    tran.Rollback()
                Catch
                End Try

                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "AddUsersToRoles")
                Else
                    Throw e
                End If

            Finally
                conn.Close()
            End Try
        End Sub


        '
        ' RoleProvider.CreateRole
        '

        Public Overrides Sub CreateRole(ByVal rolename As String)

            If rolename.IndexOf(",") > 0 Then Throw New ArgumentException("Role names cannot contain commas.")

            If RoleExists(rolename) Then Throw New ProviderException("Role name already exists.")

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("INSERT INTO `" + rolesTable + "`" & _
                    " (Rolename, ApplicationName) " & _
                    " Values($Rolename, $ApplicationName)", conn)

            cmd.Parameters.Add("$Rolename", DbType.String, 255).Value = rolename
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Try
                conn.Open()
                cmd.ExecuteNonQuery()
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "CreateRole")
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try
        End Sub


        '
        ' RoleProvider.DeleteRole
        '

        Public Overrides Function DeleteRole(ByVal rolename As String, ByVal throwOnPopulatedRole As Boolean) As Boolean

            Dim usersInRole() As String = GetUsersInRole(rolename)

            If Not RoleExists(rolename) Then Throw New ProviderException("Role does not exist.")

            If throwOnPopulatedRole And usersInRole(0) Is Nothing Then Throw New ProviderException("Cannot delete a populated role.")

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("DELETE FROM `" + rolesTable + "`" & _
                    " WHERE Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Rolename", DbType.String, 255).Value = rolename
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName


            Dim cmd2 As SQLiteCommand = New SQLiteCommand("DELETE FROM `" + usersInRolesTable + "`" & _
                    " WHERE Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            cmd2.Parameters.Add("$Rolename", DbType.String, 255).Value = rolename
            cmd2.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim tran As SQLiteTransaction = Nothing

            Try
                conn.Open()
                tran = conn.BeginTransaction()
                cmd.Transaction = tran
                cmd2.Transaction = tran

                cmd2.ExecuteNonQuery()
                cmd.ExecuteNonQuery()

                tran.Commit()
            Catch e As SQLiteException
                Try
                    tran.Rollback()
                Catch
                End Try


                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "DeleteRole")
                    Return False
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            Return True
        End Function


        '
        ' RoleProvider.GetAllRoles
        '

        Public Overrides Function GetAllRoles() As String()

            Dim tmpRoleNames As String = ""

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Rolename FROM `" + rolesTable + "` WHERE ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()

                reader = cmd.ExecuteReader()

                While reader.Read()
                    tmpRoleNames += reader.GetString(0) + ","
                End While
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "GetAllRoles")
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

            If tmpRoleNames.Length > 0 Then
                ' Remove trailing comma.
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1)
                Return tmpRoleNames.Split(","c)
            End If

            Return New String(0) {}
        End Function


        '
        ' RoleProvider.GetRolesForUser
        '

        Public Overrides Function GetRolesForUser(ByVal username As String) As String()

            Dim tmpRoleNames As String = ""

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Rolename FROM `" + usersInRolesTable + "` WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()

                reader = cmd.ExecuteReader()

                While reader.Read()
                    tmpRoleNames += reader.GetString(0) + ","
                End While
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "GetRolesForUser")
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

            If tmpRoleNames.Length > 0 Then
                ' Remove trailing comma.
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1)
                Return tmpRoleNames.Split(","c)
            End If
            Return New String(0) {}
        End Function

        '
        ' RoleProvider.GetUsersInRole
        '

        Public Overrides Function GetUsersInRole(ByVal rolename As String) As String()

            Dim tmpUserNames As String = ""

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Username FROM `" + usersInRolesTable + "` WHERE Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            Dim param1 As SQLiteParameter = New SQLiteParameter("$Rolename", DbType.String, 255).Value = rolename
            cmd.Parameters.Add(param1)
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()

                reader = cmd.ExecuteReader()

                While reader.Read()
                    tmpUserNames += reader.GetString(0).ToLower + ","
                End While
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "GetUsersInRole")
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

            If tmpUserNames.Length > 0 Then
                ' Remove trailing comma.
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1)
                Return tmpUserNames.Split(","c)
            End If
            Return New String(0) {}
        End Function


        '
        ' RoleProvider.IsUserInRole
        '

        Public Overrides Function IsUserInRole(ByVal username As String, ByVal rolename As String) As Boolean

            Dim userIsInRole As Boolean = False

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT COUNT(*) FROM `" + usersInRolesTable + "` WHERE Username = $Username AND Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$Rolename", DbType.String, 255).Value = rolename
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Try
                conn.Open()

                Dim numRecs As Long = CType(cmd.ExecuteScalar(), Long)

                If numRecs > 0 Then userIsInRole = True

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "IsUserInRole")
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            Return userIsInRole
        End Function


        '
        ' RoleProvider.RemoveUsersFromRoles
        '

        Public Overrides Sub RemoveUsersFromRoles(ByVal usernames As String(), ByVal rolenames As String())

            Dim rolename As String
            For Each rolename In rolenames
                If Not RoleExists(rolename) Then Throw New ProviderException("Role name not found.")
            Next

            Dim username As String
            For Each username In usernames
                For Each rolename In rolenames
                    If Not IsUserInRole(username.ToLower, rolename) Then Throw New ProviderException("User is not in role.")
                Next
            Next


            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("DELETE FROM `" + usersInRolesTable + "` WHERE Username = $Username AND Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            Dim userParm As SQLiteParameter = cmd.Parameters.Add("$Username", DbType.String, 255)
            Dim roleParm As SQLiteParameter = cmd.Parameters.Add("$Rolename", DbType.String, 255)
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim tran As SQLiteTransaction = Nothing

            Try
                conn.Open()
                tran = conn.BeginTransaction()
                cmd.Transaction = tran

                For Each username In usernames
                    For Each rolename In rolenames
                        userParm.Value = username.ToLower
                        roleParm.Value = rolename
                        cmd.ExecuteNonQuery()
                    Next
                Next

                tran.Commit()
            Catch e As SQLiteException
                Try
                    tran.Rollback()
                Catch
                End Try

                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "RemoveUsersFromRoles")
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try
        End Sub


        '
        ' RoleProvider.RoleExists
        '

        Public Overrides Function RoleExists(ByVal rolename As String) As Boolean

            Dim exists As Boolean = False

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT COUNT(*) FROM `" + rolesTable + "` WHERE Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Rolename", DbType.String, 255).Value = rolename
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Try
                conn.Open()

                Dim numRecs As Long = CType(cmd.ExecuteScalar(), Long)

                If numRecs > 0 Then exists = True

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "RoleExists")
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            Return exists
        End Function

        '
        ' RoleProvider.FindUsersInRole
        '

        Public Overrides Function FindUsersInRole(ByVal rolename As String, ByVal usernameToMatch As String) As String()

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Username FROM `" + usersInRolesTable + "` WHERE Username LIKE $UsernameSearch AND Rolename = $Rolename AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$UsernameSearch", DbType.String, 255).Value = usernameToMatch.ToLower
            cmd.Parameters.Add("$RoleName", DbType.String, 255).Value = rolename
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim tmpUserNames As String = ""
            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()

                reader = cmd.ExecuteReader()

                While reader.Read()
                    tmpUserNames += reader.GetString(0).ToLower + ","
                End While
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "FindUsersInRole")
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

            If tmpUserNames.Length > 0 Then
                ' Remove trailing comma.
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1)
                Return tmpUserNames.Split(","c)
            End If

            Return New String(0) {}
        End Function

        '
        ' WriteToEventLog
        '   A helper function that writes exception detail to the event log. Exceptions
        ' are written to the event log as a security measure to avoid private database
        ' details from being returned to the browser. If a method does not return a status
        ' or boolean indicating the action succeeded or failed, a generic exception is also 
        ' thrown by the caller.
        '


        Private Sub WriteToEventLog(ByVal e As SQLiteException, ByVal action As String)

            Dim log As EventLog = New EventLog()
            log.Source = eventSource
            log.Log = eventLog

            Dim message As String = exceptionMessage + "\n\n"
            message += "Action: " + action + "\n\n"
            message += "Exception: " + e.ToString()

            log.WriteEntry(message)
        End Sub

    End Class
End Namespace

