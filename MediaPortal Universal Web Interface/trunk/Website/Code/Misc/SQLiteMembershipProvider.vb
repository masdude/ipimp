' 
'   Copyright (C) 2008-2009 Martin van der Boon
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
Imports System.Security.Cryptography

Namespace uWiMP.TVServer

    '
    ' SQLiteMembershipProvider code from http://www.eggheadcafe.com/articles/20051119.asp
    ' then converted to vb.net
    '
    Public Class SQLiteMembershipProvider

        Inherits MembershipProvider

        Private NewPasswordLength As Integer = 8
        Private eventSource As String = "SQLiteMembershipProvider"
        Private eventLog As String = "Application"
        Private exceptionMessage As String = "An exception occurred. Please check the Event Log."
        Private tableName As String = "Users"
        Private connectionString As String

        Private Const encryptionKey As String = "56A476BC8C921BC9"
        '
        ' If false, exceptions are thrown to the caller. If true,
        ' exceptions are written to the event log.
        '

        Private pWriteExceptionsToEventLog As Boolean

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

            If name = Nothing Or name.Length = 0 Then name = "SQLiteMembershipProvider"

            If String.IsNullOrEmpty(config("description")) Then
                config.Remove("description")
                config.Add("description", "Sample SQLite Membership provider")
            End If

            ' Initialize the abstract base class.
            MyBase.Initialize(name, config)

            pApplicationName = GetConfigValue(config("applicationName"), System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath)
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config("maxInvalidPasswordAttempts"), "5"))
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config("passwordAttemptWindow"), "10"))
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config("minRequiredNonAlphanumericCharacters"), "1"))
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config("minRequiredPasswordLength"), "7"))
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config("passwordStrengthRegularExpression"), ""))
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config("enablePasswordReset"), "true"))
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config("enablePasswordRetrieval"), "true"))
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config("requiresQuestionAndAnswer"), "false"))
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config("requiresUniqueEmail"), "false"))
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config("writeExceptionsToEventLog"), "true"))

            Dim temp_format As String = config("passwordFormat")

            If temp_format = Nothing Then
                temp_format = "Hashed"
            End If

            Select Case temp_format
                Case "Hashed"
                    pPasswordFormat = MembershipPasswordFormat.Hashed
                    Exit Select
                Case "Encrypted"
                    pPasswordFormat = MembershipPasswordFormat.Encrypted
                    Exit Select
                Case "Clear"
                    pPasswordFormat = MembershipPasswordFormat.Clear
                    Exit Select
                Case Else
                    Throw New ProviderException("Password format not supported.")
            End Select

            '
            ' Initialize SQLiteConnection.
            '

            Dim ConnectionStringSettings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(config("connectionStringName"))

            If ConnectionStringSettings Is Nothing Or ConnectionStringSettings.ConnectionString.Trim() = "" Then
                Throw New ProviderException("Connection String cannot be blank.")
            End If

            connectionString = ConnectionStringSettings.ConnectionString


        End Sub

        '
        ' A helper function to retrieve config values from the configuration file.
        '
        Private Function GetConfigValue(ByVal configValue As String, ByVal defaultValue As String) As String
            If String.IsNullOrEmpty(configValue) Then
                Return defaultValue
            Else
                Return configValue
            End If
        End Function

        '
        ' System.Web.Security.MembershipProvider properties.
        '
        Private pApplicationName As String
        Private pEnablePasswordReset As Boolean
        Private pEnablePasswordRetrieval As Boolean
        Private pRequiresQuestionAndAnswer As Boolean
        Private pRequiresUniqueEmail As Boolean
        Private pMaxInvalidPasswordAttempts As Integer
        Private pPasswordAttemptWindow As Integer
        Private pPasswordFormat As MembershipPasswordFormat

        Public Overrides Property ApplicationName() As String
            Get
                Return pApplicationName
            End Get
            Set(ByVal Value As String)
                pApplicationName = Value
            End Set
        End Property

        Public Overrides ReadOnly Property EnablePasswordReset() As Boolean
            Get
                Return pEnablePasswordReset
            End Get
        End Property


        Public Overrides ReadOnly Property EnablePasswordRetrieval() As Boolean
            Get
                Return pEnablePasswordRetrieval
            End Get
        End Property


        Public Overrides ReadOnly Property RequiresQuestionAndAnswer() As Boolean
            Get
                Return pRequiresQuestionAndAnswer
            End Get
        End Property


        Public Overrides ReadOnly Property RequiresUniqueEmail() As Boolean
            Get
                Return pRequiresUniqueEmail
            End Get
        End Property


        Public Overrides ReadOnly Property MaxInvalidPasswordAttempts() As Integer
            Get
                Return pMaxInvalidPasswordAttempts
            End Get
        End Property


        Public Overrides ReadOnly Property PasswordAttemptWindow() As Integer
            Get
                Return pPasswordAttemptWindow
            End Get
        End Property


        Public Overrides ReadOnly Property PasswordFormat() As MembershipPasswordFormat
            Get
                Return pPasswordFormat
            End Get
        End Property

        Private pMinRequiredNonAlphanumericCharacters As Integer

        Public Overrides ReadOnly Property MinRequiredNonAlphanumericCharacters() As Integer
            Get
                Return pMinRequiredNonAlphanumericCharacters
            End Get
        End Property

        Private pMinRequiredPasswordLength As Integer

        Public Overrides ReadOnly Property MinRequiredPasswordLength() As Integer
            Get
                Return pMinRequiredPasswordLength
            End Get
        End Property

        Private pPasswordStrengthRegularExpression As String

        Public Overrides ReadOnly Property PasswordStrengthRegularExpression() As String
            Get
                Return pPasswordStrengthRegularExpression
            End Get
        End Property

        '
        ' System.Web.Security.MembershipProvider methods.
        '

        '
        ' MembershipProvider.ChangePassword
        '
        Public Overrides Function ChangePassword(ByVal username As String, ByVal oldPwd As String, ByVal NewPwd As String) As Boolean

            If Not ValidateUser(username.ToLower, oldPwd) Then Return False
            Dim args As ValidatePasswordEventArgs = New ValidatePasswordEventArgs(username.ToLower, NewPwd, True)

            OnValidatingPassword(args)

            If args.Cancel Then
                If Not args.FailureInformation Is Nothing Then
                    Throw args.FailureInformation
                Else
                    Throw New MembershipPasswordException("Change password canceled due to New password validation failure.")

                End If
            End If

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "`SET Password = $Password, LastPasswordChangedDate = $LastPasswordChangedDate WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Password", DbType.String, 255).Value = EncodePassword(NewPwd)
            cmd.Parameters.Add("$LastPasswordChangedDate", DbType.DateTime).Value = DateTime.Now
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim rowsAffected As Integer = 0

            Try
                conn.Open()
                rowsAffected = cmd.ExecuteNonQuery()

            Catch e As SQLiteException
                If (WriteExceptionsToEventLog) Then

                    WriteToEventLog(e, "ChangePassword")

                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            If rowsAffected > 0 Then
                Return True
            Else
                Return False
            End If

        End Function


        '
        ' MembershipProvider.ChangePasswordQuestionAndAnswer
        '

        Public Overrides Function ChangePasswordQuestionAndAnswer(ByVal username As String, ByVal password As String, ByVal newPwdQuestion As String, ByVal newPwdAnswer As String) As Boolean

            If Not ValidateUser(username.ToLower, password) Then Return False

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As New SQLiteCommand("UPDATE `" & tableName & "` SET PasswordQuestion = $PasswordQuestion, PasswordAnswer = $PasswordAnswer WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Question", DbType.String, 255).Value = newPwdQuestion
            cmd.Parameters.Add("$Answer", DbType.String, 255).Value = EncodePassword(newPwdAnswer)
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim rowsAffected As Integer = 0

            Try
                conn.Open()
                rowsAffected = cmd.ExecuteNonQuery

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            If rowsAffected > 0 Then
                Return True
            Else
                Return False
            End If

        End Function




        '
        ' MembershipProvider.CreateUser
        '

        Public Overloads Overrides Function CreateUser(ByVal username As String, ByVal password As String, ByVal email As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String, ByVal isApproved As Boolean, ByVal providerUserKey As Object, ByRef status As MembershipCreateStatus) As MembershipUser

            Dim args As ValidatePasswordEventArgs = New ValidatePasswordEventArgs(username.ToLower, password, True)
            OnValidatingPassword(args)

            If args.Cancel Then
                status = MembershipCreateStatus.InvalidPassword
                Return Nothing
            End If

            If RequiresUniqueEmail And Not GetUserNameByEmail(email) = "" Then
                status = MembershipCreateStatus.DuplicateEmail
                Return Nothing
            End If

            Dim u As MembershipUser = GetUser(username.ToLower, False)
            If u Is Nothing Then
                Dim createdate As DateTime = DateTime.Now

                If providerUserKey Is Nothing Then
                    providerUserKey = Guid.NewGuid()
                Else
                    If Not (TypeOf providerUserKey Is Guid) Then
                        status = MembershipCreateStatus.InvalidProviderUserKey
                        Return Nothing
                    End If
                End If

                Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
                Dim cmd As SQLiteCommand = New SQLiteCommand("INSERT INTO `" & tableName & "`" & _
                      " (PKID, Username, Password, Email, PasswordQuestion, " & _
                      " PasswordAnswer, IsApproved," & _
                      " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," & _
                      " ApplicationName, IsLockedOut, LastLockedOutDate," & _
                      " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " & _
                      " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" & _
                      " Values($PKID, $Username, $Password, $Email, $PasswordQuestion, " & _
                      " $PasswordAnswer, $IsApproved, $Comment, $CreationDate, $LastPasswordChangedDate, " & _
                      " $LastActivityDate, $ApplicationName, $IsLockedOut, $LastLockedOutDate, " & _
                      " $FailedPasswordAttemptCount, $FailedPasswordAttemptWindowStart, " & _
                      " $FailedPasswordAnswerAttemptCount, $FailedPasswordAnswerAttemptWindowStart)", conn)

                cmd.Parameters.Add("$PKID", DbType.String).Value = providerUserKey.ToString()
                cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                cmd.Parameters.Add("$Password", DbType.String, 255).Value = EncodePassword(password)
                cmd.Parameters.Add("$Email", DbType.String, 128).Value = email
                cmd.Parameters.Add("$PasswordQuestion", DbType.String, 255).Value = passwordQuestion
                cmd.Parameters.Add("$PasswordAnswer", DbType.String, 255).Value = EncodePassword(passwordAnswer)
                cmd.Parameters.Add("$IsApproved", DbType.Boolean).Value = isApproved
                cmd.Parameters.Add("$Comment", DbType.String, 255).Value = ""
                cmd.Parameters.Add("$CreationDate", DbType.DateTime).Value = createdate
                cmd.Parameters.Add("$LastPasswordChangedDate", DbType.DateTime).Value = createdate
                cmd.Parameters.Add("$LastActivityDate", DbType.DateTime).Value = createdate
                cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName
                cmd.Parameters.Add("$IsLockedOut", DbType.Boolean).Value = False
                cmd.Parameters.Add("$LastLockedOutDate", DbType.DateTime).Value = createdate
                cmd.Parameters.Add("$FailedPasswordAttemptCount", DbType.Int32).Value = 0
                cmd.Parameters.Add("$FailedPasswordAttemptWindowStart", DbType.DateTime).Value = createdate
                cmd.Parameters.Add("$FailedPasswordAnswerAttemptCount", DbType.Int32).Value = 0
                cmd.Parameters.Add("$FailedPasswordAnswerAttemptWindowStart", DbType.DateTime).Value = createdate

                Try
                    conn.Open()
                    Dim recAdded As Integer = cmd.ExecuteNonQuery()
                    If recAdded > 0 Then
                        status = MembershipCreateStatus.Success
                    Else
                        status = MembershipCreateStatus.UserRejected
                    End If
                Catch e As SQLiteException

                    If WriteExceptionsToEventLog Then
                        WriteToEventLog(e, "CreateUser")
                    End If
                    status = MembershipCreateStatus.ProviderError
                Finally
                    conn.Close()
                End Try
                Return GetUser(username.ToLower, False)
            Else
                status = MembershipCreateStatus.DuplicateUserName
            End If

            Return Nothing

        End Function




        '
        ' MembershipProvider.DeleteUser
        '

        Public Overrides Function DeleteUser(ByVal username As String, ByVal deleteAllRelatedData As Boolean) As Boolean

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("DELETE FROM `" + tableName + "` WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim rowsAffected As Integer = 0

            Try
                conn.Open()

                rowsAffected = cmd.ExecuteNonQuery()

                If deleteAllRelatedData Then
                    ' Process commands to delete all data for the user in the database.
                End If
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "DeleteUser")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            If (rowsAffected > 0) Then
                Return True
            Else
                Return False
            End If


        End Function


        '
        ' MembershipProvider.GetAllUsers
        '

        Public Overrides Function GetAllUsers(ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As MembershipUserCollection

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` WHERE ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim users As MembershipUserCollection = New MembershipUserCollection()

            Dim reader As SQLiteDataReader = Nothing
            totalRecords = 0

            Try
                conn.Open()
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar())

                If totalRecords <= 0 Then Return users

                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," & _
                         " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," & _
                         " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " & _
                         " FROM `" + tableName + "` " & _
                         " WHERE ApplicationName = $ApplicationName " & _
                         " ORDER BY Username Asc"

                reader = cmd.ExecuteReader()

                Dim counter As Integer = 0
                Dim startIndex As Integer = pageSize * pageIndex
                Dim endIndex As Integer = startIndex + pageSize - 1

                While reader.Read()
                    If counter >= startIndex Then
                        Dim u As MembershipUser = GetUserFromReader(reader)
                        users.Add(u)
                    End If

                    If counter >= endIndex Then cmd.Cancel()

                    counter = counter + 1
                End While
            Catch e As SQLiteException
                If (WriteExceptionsToEventLog) Then
                    WriteToEventLog(e, "GetAllUsers")

                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

            Return users

        End Function

        '
        ' MembershipProvider.GetNumberOfUsersOnline
        '

        Public Overrides Function GetNumberOfUsersOnline() As Integer

            Dim onlineSpan As TimeSpan = New TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0)
            Dim compareTime As DateTime = DateTime.Now.Subtract(onlineSpan)

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` WHERE LastActivityDate > $LastActivityDate AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$CompareDate", DbType.DateTime).Value = compareTime
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim numOnline As Integer = 0

            Try
                conn.Open()

                numOnline = Convert.ToInt32(cmd.ExecuteScalar())
            Catch e As SQLiteException
                If (WriteExceptionsToEventLog) Then
                    WriteToEventLog(e, "GetNumberOfUsersOnline")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            Return numOnline

        End Function

        '
        ' MembershipProvider.GetPassword
        '
        Public Overrides Function GetPassword(ByVal username As String, ByVal answer As String) As String

            If Not EnablePasswordRetrieval Then Throw New ProviderException("Password Retrieval Not Enabled.")

            If PasswordFormat = MembershipPasswordFormat.Hashed Then Throw New ProviderException("Cannot retrieve Hashed passwords.")

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Password, PasswordAnswer, IsLockedOut FROM `" + tableName + "` WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim password As String = ""
            Dim passwordAnswer As String = ""
            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow)

                If reader.HasRows Then
                    reader.Read()
                    If reader.GetBoolean(2) Then
                        Throw New MembershipPasswordException("The supplied user is locked .")
                        password = reader.GetString(0)
                        passwordAnswer = reader.GetString(1)
                    Else
                        Throw New MembershipPasswordException("The supplied user name is not found.")
                    End If
                End If
            Catch e As SQLiteException
                If (WriteExceptionsToEventLog) Then
                    WriteToEventLog(e, "GetPassword")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
                conn.Close()
            End Try


            If RequiresQuestionAndAnswer And Not CheckPassword(answer, passwordAnswer) Then
                UpdateFailureCount(username, "passwordAnswer")
                Throw New MembershipPasswordException("Incorrect password answer.")
            End If

            If (PasswordFormat = MembershipPasswordFormat.Encrypted) Then
                password = UnEncodePassword(password)
            End If

            Return password
        End Function


        '
        ' MembershipProvider.GetUser(string, bool)
        '

        Public Overrides Function GetUser(ByVal username As String, ByVal userIsOnline As Boolean) As MembershipUser

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT PKID, Username, Email, PasswordQuestion," & _
                     " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," & _
                     " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" & _
                     " FROM `" + tableName + "` WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim u As MembershipUser = Nothing
            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()

                reader = cmd.ExecuteReader()

                If reader.HasRows Then

                    reader.Read()
                    u = GetUserFromReader(reader)

                    If userIsOnline Then

                        Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "` " & _
                                  "SET LastActivityDate = $LastActivityDate " & _
                                  "WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

                        updateCmd.Parameters.Add("$LastActivityDate", DbType.DateTime).Value = DateTime.Now
                        updateCmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                        updateCmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

                        updateCmd.ExecuteNonQuery()
                    End If
                End If

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "GetUser(String, Boolean)")
                    Throw New ProviderException(exceptionMessage)

                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

            Return u

        End Function


        '
        ' MembershipProvider.GetUser(object, bool)
        '

        Public Overrides Function GetUser(ByVal providerUserKey As Object, ByVal userIsOnline As Boolean) As MembershipUser

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT PKID, Username, Email, PasswordQuestion," & _
                  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," & _
                  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" & _
                  " FROM `" + tableName + "` WHERE PKID = $PKID", conn)

            cmd.Parameters.Add("$PKID", DbType.String).Value = providerUserKey

            Dim u As MembershipUser = Nothing
            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()

                reader = cmd.ExecuteReader()

                If reader.HasRows Then

                    reader.Read()
                    u = GetUserFromReader(reader)

                    If userIsOnline Then
                        Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "` " & _
                                     "SET LastActivityDate = $LastActivityDate " & _
                                     "WHERE PKID = $PKID", conn)

                        updateCmd.Parameters.Add("$LastActivityDate", DbType.DateTime).Value = DateTime.Now
                        updateCmd.Parameters.Add("$PKID", DbType.String).Value = providerUserKey

                        updateCmd.ExecuteNonQuery()
                    End If
                End If

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "GetUser(Object, Boolean)")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If

            Finally

                If Not reader Is Nothing Then reader.Close()

                conn.Close()

            End Try

            Return u

        End Function


        '
        ' GetUserFromReader
        '    A helper function that takes the current row from the SQLiteDataReader
        ' and hydrates a MembershipUser from the values. Called by the 
        ' MembershipUser.GetUser implementation.
        '

        Private Function GetUserFromReader(ByVal reader As SQLiteDataReader) As MembershipUser

            If reader.GetString(1) = "" Then Return Nothing

            Dim providerUserKey As Object = Nothing
            Dim strGooid As String = Guid.NewGuid().ToString()
            If reader.GetValue(0).ToString().Length > 0 Then

                providerUserKey = New Guid(reader.GetValue(0).ToString())
            Else
                providerUserKey = New Guid(strGooid)
            End If

            Dim username As String = reader.GetString(1).ToLower
            Dim email As String = reader.GetString(2)

            Dim passwordQuestion As String = ""
            If Not reader.GetValue(3) Is DBNull.Value Then passwordQuestion = reader.GetString(3)

            Dim comment As String = ""
            If Not reader.GetValue(4) Is DBNull.Value Then comment = reader.GetString(4)

            Dim tmpApproved As Boolean = (reader.GetValue(5) = Nothing)
            Dim isApproved As Boolean = False
            If tmpApproved Then isApproved = reader.GetBoolean(5)

            Dim tmpLockedOut As Boolean = (reader.GetValue(6) = Nothing)
            Dim isLockedOut As Boolean = False
            If tmpLockedOut Then isLockedOut = reader.GetBoolean(6)

            Dim creationDate As DateTime = DateTime.Now
            Try
                If Not reader.GetValue(6) Is DBNull.Value Then creationDate = reader.GetDateTime(7)
            Catch
            End Try

            Dim lastLoginDate As DateTime = DateTime.Now
            Try
                If Not reader.GetValue(8) Is DBNull.Value Then lastLoginDate = reader.GetDateTime(8)
            Catch
            End Try

            Dim lastActivityDate As DateTime = DateTime.Now
            Try
                If Not reader.GetValue(9) Is DBNull.Value Then lastActivityDate = reader.GetDateTime(9)
            Catch
            End Try

            Dim lastPasswordChangedDate As DateTime = DateTime.Now
            Try
                If Not reader.GetValue(10) Is DBNull.Value Then lastPasswordChangedDate = reader.GetDateTime(10)
            Catch
            End Try

            Dim lastLockedOutDate As DateTime = DateTime.Now
            Try
                If Not reader.GetValue(11) Is DBNull.Value Then lastLockedOutDate = reader.GetDateTime(11)
            Catch
            End Try

            Dim u As MembershipUser = New MembershipUser(Me.Name, username.ToLower, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockedOutDate)

            Return u

        End Function


        '
        ' MembershipProvider.UnlockUser
        '

        Public Overrides Function UnlockUser(ByVal username As String) As Boolean

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "` " & _
                    " SET IsLockedOut = False, LastLockedOutDate = $LastLockedOutDate " & _
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$LastLockedOutDate", DbType.DateTime).Value = DateTime.Now
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim rowsAffected As Integer = 0

            Try
                conn.Open()
                rowsAffected = cmd.ExecuteNonQuery()
            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "UnlockUser")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            If rowsAffected > 0 Then
                Return True
            Else
                Return False
            End If

        End Function

        '
        ' MembershipProvider.GetUserNameByEmail
        '

        Public Overrides Function GetUserNameByEmail(ByVal email As String) As String

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Username FROM `" + tableName + "` WHERE Email = $Email AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Email", DbType.String, 128).Value = email
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim username As String = ""

            Try
                conn.Open()
                Dim o As Object = cmd.ExecuteScalar()
                If Not o Is Nothing Then username = Convert.ToString(o)
            Catch e As SQLiteException
                If (WriteExceptionsToEventLog) Then
                    WriteToEventLog(e, "GetUserNameByEmail")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

            If username = Nothing Then username = ""

            Return username.ToLower

        End Function

        '
        ' MembershipProvider.ResetPassword
        '

        Public Overrides Function ResetPassword(ByVal username As String, ByVal answer As String) As String

            If Not EnablePasswordReset Then Throw New NotSupportedException("Password reset is not enabled.")

            If answer = Nothing And RequiresQuestionAndAnswer Then
                UpdateFailureCount(username.ToLower, "passwordAnswer")
                Throw New ProviderException("Password answer required for password reset.")
            End If

            Dim NewPassword As String = System.Web.Security.Membership.GeneratePassword(NewPasswordLength, MinRequiredNonAlphanumericCharacters)

            Dim args As ValidatePasswordEventArgs = New ValidatePasswordEventArgs(username.ToLower, NewPassword, True)

            OnValidatingPassword(args)

            If args.Cancel Then
                If Not args.FailureInformation Is Nothing Then
                    Throw args.FailureInformation
                Else
                    Throw New MembershipPasswordException("Reset password canceled due to password validation failure.")
                End If
            End If

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT PasswordAnswer, IsLockedOut FROM `" + tableName + "`" & _
                        " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim rowsAffected As Integer = 0
            Dim passwordAnswer As String = ""
            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow)

                Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "`" & _
                " SET Password = $Password, LastPasswordChangedDate = $LastPasswordChangedDate" & _
                " WHERE Username = $Username AND ApplicationName = $ApplicationName AND IsLockedOut = 0", conn)

                If reader.HasRows Then

                    reader.Read()

                    'object val = reader.GetValue(1);

                    If (Convert.ToBoolean(reader.GetValue(1))) Then
                        Throw New MembershipPasswordException("The supplied user is locked .")
                        passwordAnswer = reader.GetString(0)
                    Else
                        Throw New MembershipPasswordException("The supplied user name is not found.")
                    End If

                    If RequiresQuestionAndAnswer And Not CheckPassword(answer, passwordAnswer) Then
                        UpdateFailureCount(username.ToLower, "passwordAnswer")

                        Throw New MembershipPasswordException("Incorrect password answer.")
                    End If

                    reader.Close()

                    updateCmd.Parameters.Add("$Password", DbType.String, 255).Value = EncodePassword(NewPassword)
                    updateCmd.Parameters.Add("$LastPasswordChangedDate", DbType.DateTime).Value = DateTime.Now
                    updateCmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                    updateCmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

                    rowsAffected = updateCmd.ExecuteNonQuery()
                End If

            Catch e As SQLiteException

                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "ResetPassword")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If

            Finally

                If Not reader Is Nothing Then reader.Close()
                conn.Close()

            End Try

            If rowsAffected > 0 Then
                Return NewPassword
            Else
                Throw New MembershipPasswordException("User not found, or user is locked . Password not Reset.")
            End If

        End Function



        '
        ' MembershipProvider.UpdateUser
        '

        Public Overrides Sub UpdateUser(ByVal user As MembershipUser)

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "`" & _
                    " SET Email = $Email, Comment = $Comment," & _
                    " IsApproved = $IsApproved" & _
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Email", DbType.String, 128).Value = user.Email
            cmd.Parameters.Add("$Comment", DbType.String, 255).Value = user.Comment
            cmd.Parameters.Add("$IsApproved", DbType.Boolean).Value = user.IsApproved
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = user.UserName.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Try
                conn.Open()
                cmd.ExecuteNonQuery()

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "UpdateUser")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                conn.Close()
            End Try

        End Sub


        '
        ' MembershipProvider.ValidateUser
        '

        Public Overrides Function ValidateUser(ByVal username As String, ByVal password As String) As Boolean

            Dim isValid As Boolean = False

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Password, IsApproved FROM `" + tableName + "`" & _
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName AND IsLockedOut = 0", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim reader As SQLiteDataReader = Nothing
            Dim isApproved As Boolean = False
            Dim pwd As String = ""

            Try
                conn.Open()
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow)

                If reader.HasRows Then

                    reader.Read()
                    pwd = reader.GetString(0)
                    Dim iApp As Integer = Convert.ToInt32(reader.GetValue(1))
                    If iApp = 1 Then isApproved = True
                Else
                    Return False
                End If

                reader.Close()

                If CheckPassword(password, pwd) Then

                    If isApproved Then

                        isValid = True
                        Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE `" + tableName + "` SET LastLoginDate = $LastLoginDate" & _
                                " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

                        updateCmd.Parameters.Add("$LastLoginDate", DbType.DateTime).Value = DateTime.Now
                        updateCmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                        updateCmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

                        updateCmd.ExecuteNonQuery()
                    End If

                Else

                    conn.Close()
                    UpdateFailureCount(username.ToLower, "password")

                End If

            Catch e As SQLiteException
                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "ValidateUser")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally

                If Not reader Is Nothing Then reader.Close()
                conn.Close()

            End Try

            Return isValid

        End Function


        '
        ' UpdateFailureCount
        '   A helper method that performs the checks and updates associated with
        ' password failure tracking.
        '

        Private Sub UpdateFailureCount(ByVal username As String, ByVal failureType As String)

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT FailedPasswordAttemptCount, " & _
                                                  "  FailedPasswordAttemptWindowStart, " & _
                                                  "  FailedPasswordAnswerAttemptCount, " & _
                                                  "  FailedPasswordAnswerAttemptWindowStart " & _
                                                  "  FROM `" + tableName + "` " & _
                                                  "  WHERE Username = $Username AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim reader As SQLiteDataReader = Nothing
            Dim windowStart As DateTime = New DateTime()
            Dim failureCount As Integer = 0

            Try
                conn.Open()
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow)

                If reader.HasRows Then

                    reader.Read()

                    If failureType = "password" Then
                        failureCount = reader.GetInt32(0)
                        Try
                            windowStart = reader.GetDateTime(1)
                        Catch
                            windowStart = DateTime.Now
                        End Try
                    End If

                    If failureType = "passwordAnswer" Then

                        failureCount = reader.GetInt32(2)
                        windowStart = reader.GetDateTime(3)
                    End If
                End If

                reader.Close()

                Dim windowEnd As DateTime = windowStart.AddMinutes(PasswordAttemptWindow)

                If failureCount = 0 Or DateTime.Now > windowEnd Then

                    ' First password failure or outside of PasswordAttemptWindow. 
                    ' Start a new password failure count from 1 and a new window starting now.

                    If failureType = "password" Then cmd.CommandText = "UPDATE `" + tableName + "` " & _
                                          "  SET FailedPasswordAttemptCount = $Count, " & _
                                          "      FailedPasswordAttemptWindowStart = $WindowStart " & _
                                          "  WHERE Username = $Username AND ApplicationName = $ApplicationName"

                    If failureType = "passwordAnswer" Then cmd.CommandText = "UPDATE `" + tableName + "` " & _
                                          "  SET FailedPasswordAnswerAttemptCount = $Count, " & _
                                          "      FailedPasswordAnswerAttemptWindowStart = $WindowStart " & _
                                          "  WHERE Username = $Username AND ApplicationName = $ApplicationName"

                    cmd.Parameters.Clear()

                    cmd.Parameters.Add("$Count", DbType.Int32).Value = 1
                    cmd.Parameters.Add("$WindowStart", DbType.DateTime).Value = DateTime.Now
                    cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                    cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

                    If cmd.ExecuteNonQuery() < 0 Then Throw New ProviderException("Unable to update failure count and window start.")

                Else
                    If failureCount + 1 >= MaxInvalidPasswordAttempts Then

                        ' Password attempts have exceeded the failure threshold. Lock out
                        ' the user.

                        cmd.CommandText = "UPDATE `" + tableName + "` " & _
                                          "  SET IsLockedOut = $IsLockedOut, LastLockedOutDate = $LastLockedOutDate " & _
                                          "  WHERE Username = $Username AND ApplicationName = $ApplicationName"

                        cmd.Parameters.Clear()

                        cmd.Parameters.Add("$IsLockedOut", DbType.Boolean).Value = True
                        cmd.Parameters.Add("$LastLockedOutDate", DbType.DateTime).Value = DateTime.Now
                        cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                        cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

                        If (cmd.ExecuteNonQuery() < 0) Then Throw New ProviderException("Unable to lock  user.")

                    Else

                        ' Password attempts have not exceeded the failure threshold. Update
                        ' the failure counts. Leave the window the same.

                        If failureType = "password" Then cmd.CommandText = "UPDATE `" + tableName + "` " & _
                                      "  SET FailedPasswordAttemptCount = $Count" & _
                                      "  WHERE Username = $Username AND ApplicationName = $ApplicationName"

                        If (failureType = "passwordAnswer") Then cmd.CommandText = "UPDATE `" + tableName + "` " & _
                                      "  SET FailedPasswordAnswerAttemptCount = $Count" & _
                                      "  WHERE Username = $Username AND ApplicationName = $ApplicationName"

                        cmd.Parameters.Clear()

                        cmd.Parameters.Add("$Count", DbType.Int32).Value = failureCount
                        cmd.Parameters.Add("$Username", DbType.String, 255).Value = username.ToLower
                        cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

                        If cmd.ExecuteNonQuery() < 0 Then Throw New ProviderException("Unable to update failure count.")
                    End If
                End If

            Catch e As SQLiteException

                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "UpdateFailureCount")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If

            Finally

                If Not reader Is Nothing Then reader.Close()
                conn.Close()

            End Try

        End Sub


        '
        ' CheckPassword
        '   Compares password values based on the MembershipPasswordFormat.
        '

        Private Function CheckPassword(ByVal password As String, ByVal dbpassword As String) As Boolean

            Dim pass1 As String = password
            Dim pass2 As String = dbpassword

            Select Case PasswordFormat
                Case MembershipPasswordFormat.Encrypted
                    pass2 = UnEncodePassword(dbpassword)
                    Exit Select
                Case MembershipPasswordFormat.Hashed
                    pass1 = EncodePassword(password)
                    Exit Select
                Case Else
                    Exit Select
            End Select

            If (pass1 = pass2) Then
                Return True
            Else
                Return False
            End If

        End Function

        '
        ' EncodePassword
        '   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        '

        Private Function EncodePassword(ByVal password As String) As String

            If password = Nothing Then password = ""
            Dim encodedPassword As String = password

            Select Case PasswordFormat
                Case MembershipPasswordFormat.Clear
                    Exit Select
                Case MembershipPasswordFormat.Encrypted
                    encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)))
                    Exit Select
                Case MembershipPasswordFormat.Hashed
                    Dim hash As HMACSHA1 = New HMACSHA1()
                    hash.Key = HexToByte(encryptionKey)
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)))
                    Exit Select
                Case Else
                    Throw New ProviderException("Unsupported password format.")
            End Select

            Return encodedPassword

        End Function


        '
        ' UnEncodePassword
        '   Decrypts or leaves the password clear based on the PasswordFormat.
        '

        Private Function UnEncodePassword(ByVal encodedPassword As String) As String

            Dim password As String = encodedPassword

            Select Case PasswordFormat
                Case MembershipPasswordFormat.Clear
                    Exit Select
                Case MembershipPasswordFormat.Encrypted
                    password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)))
                    Exit Select
                Case MembershipPasswordFormat.Hashed
                    Throw New ProviderException("Cannot unencode a hashed password.")
                Case Else
            End Select

            Return password
        End Function

        Private Function HexToByte(ByVal hexString As String) As Byte()
            Dim returnBytes As Byte() = New Byte(hexString.Length / 2 - 1) {}
            For i As Integer = 0 To returnBytes.Length - 1
                returnBytes(i) = Convert.ToByte(hexString.Substring(i * 2, 2), 16)
            Next
            Return returnBytes
        End Function



        '
        ' MembershipProvider.FindUsersByName
        '

        Public Overrides Function FindUsersByName(ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As MembershipUserCollection


            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` " & _
                      "WHERE Username LIKE $UsernameSearch AND ApplicationName = $ApplicationName", conn)

            cmd.Parameters.Add("$UsernameSearch", DbType.String, 255).Value = usernameToMatch.ToLower
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName

            Dim users As MembershipUserCollection = New MembershipUserCollection()

            Dim reader As SQLiteDataReader = Nothing

            Try
                conn.Open()
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar())

                If totalRecords <= 0 Then Return users

                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," & _
                                  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," & _
                                  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " & _
                                  " FROM `" + tableName + "` " & _
                                  " WHERE Username LIKE $UsernameSearch AND ApplicationName = $ApplicationName " & _
                                  " ORDER BY Username Asc"

                reader = cmd.ExecuteReader()

                Dim counter As Integer = 0
                Dim startIndex As Integer = pageSize * pageIndex
                Dim endIndex As Integer = startIndex + pageSize - 1

                While reader.Read()
                    If counter >= startIndex Then

                        Dim u As MembershipUser = GetUserFromReader(reader)
                        users.Add(u)
                    End If

                    If counter >= endIndex Then

                        cmd.Cancel()
                    End If

                    counter = counter + 1
                End While

            Catch e As SQLiteException

                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "FindUsersByName")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally

                If Not reader Is Nothing Then reader.Close()
                conn.Close()

            End Try

            Return users
        End Function

        '
        ' MembershipProvider.FindUsersByEmail
        '

        Public Overrides Function FindUsersByEmail(ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As MembershipUserCollection

            Dim conn As SQLiteConnection = New SQLiteConnection(connectionString)
            Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` WHERE Email LIKE $EmailSearch AND ApplicationName = $ApplicationName", conn)
            cmd.Parameters.Add("$EmailSearch", DbType.String, 255).Value = emailToMatch
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName

            Dim users As MembershipUserCollection = New MembershipUserCollection()

            Dim reader As SQLiteDataReader = Nothing
            totalRecords = 0

            Try
                conn.Open()
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar())

                If totalRecords <= 0 Then Return users

                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," & _
                         " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," & _
                         " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " & _
                         " FROM `" + tableName + "` " & _
                         " WHERE Email LIKE $Username AND ApplicationName = $ApplicationName " & _
                         " ORDER BY Username Asc"

                reader = cmd.ExecuteReader()

                Dim counter As Integer = 0
                Dim startIndex As Integer = pageSize * pageIndex
                Dim endIndex As Integer = startIndex + pageSize - 1

                While reader.Read()
                    If counter >= startIndex Then
                        Dim u As MembershipUser = GetUserFromReader(reader)
                        users.Add(u)
                    End If

                    If counter >= endIndex Then cmd.Cancel()

                    counter = counter + 1
                End While

            Catch e As SQLiteException

                If WriteExceptionsToEventLog Then
                    WriteToEventLog(e, "FindUsersByEmail")
                    Throw New ProviderException(exceptionMessage)
                Else
                    Throw e
                End If
            Finally
                If Not reader Is Nothing Then reader.Close()

                conn.Close()
            End Try

            Return users
        End Function

        '
        ' WriteToEventLog
        '   A helper function that writes exception detail to the event log. Exceptions
        ' are written to the event log as a security measure to avoid private database
        ' details from being returned to the browser. If a method does not return a status
        ' or boolean indicating the action succeeded or failed, a generic exception is also 
        ' thrown by the caller.
        '

        Private Sub WriteToEventLog(ByVal e As Exception, ByVal action As String)

            Dim log As EventLog = New EventLog()
            log.Source = eventSource
            log.Log = eventLog

            Dim message As String = "An exception occurred communicating with the data source.\n\n"
            message += "Action: " + action + "\n\n"
            message += "Exception: " + e.ToString()

            log.WriteEntry(message)
        End Sub

    End Class

End Namespace
