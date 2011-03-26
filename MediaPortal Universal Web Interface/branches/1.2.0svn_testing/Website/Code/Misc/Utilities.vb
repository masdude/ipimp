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


Imports System.IO
Imports System.Net
Imports System.Math
Imports System.Web.Configuration
Imports System.ServiceProcess
Imports TvLibrary.Log

Namespace uWiMP.TVServer

    Public Class Utilities

        Private Shared resource As System.Resources.ResourceManager
        Private Shared textInfo As System.Globalization.TextInfo

        Public Shared Function GetAppConfig(ByVal setting As String) As String

            Return ConfigurationManager.AppSettings(setting)

        End Function

        Public Shared Function SetAppConfig(ByVal setting As String, ByVal value As String) As Boolean

            Dim config As System.Configuration.Configuration = WebConfigurationManager.OpenWebConfiguration("~")
            Dim appSetting As AppSettingsSection = config.GetSection("appSettings")

            Try
                appSetting.Settings(setting).Value = value
                config.Save()
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        Public Shared Function DoesFileExist(ByVal fileName As String) As Boolean

            If File.Exists(fileName) Then
                Return True
            Else
                Return False
            End If

        End Function


        Public Shared Function DoesHTTPResourceExist(ByVal URI As String) As Boolean

            Dim request As HttpWebRequest
            Dim response As HttpWebResponse

            Try
                request = WebRequest.Create(URI)
                request.Method = "HEAD"
                response = request.GetResponse()
                If response.StatusCode = HttpStatusCode.OK Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try

        End Function

        Public Shared Function ConvertToHHMMSS(ByVal secs As String) As String

            Dim iSec As Double = CInt(secs)
            Dim iHH, iMM, iSS As Double
            Dim sHH, sMM, sSS As String

            iHH = Floor(iSec / 3600)
            iMM = Floor((iSec - (iHH * 3600)) / 60)
            iSS = Floor(iSec - (iHH * 3600) - (iMM * 60))

            sHH = iHH.ToString

            If (iMM < 10) And (iHH > 0) Then
                sMM = "0" & iMM.ToString
            Else
                sMM = iMM.ToString
            End If

            If iSS < 10 Then
                sSS = "0" & iSS.ToString
            Else
                sSS = iSS.ToString
            End If

            If iHH > 0 Then
                Return sHH & ":" & sMM & ":" & sSS
            Else
                Return sMM & ":" & sSS
            End If

        End Function

        Public Shared Function GetServiceStatus(ByVal service As String) As String

            Dim sc As New ServiceController

            sc.MachineName = GetAppConfig("HOSTNAME")
            sc.ServiceName = service

            Try
                Return sc.Status.ToString
            Catch ex As Exception
                Return ""
            End Try

        End Function

        Public Shared Function ModifyService(ByVal service As String, ByVal action As String) As Boolean

            Dim sc As New ServiceController

            sc.MachineName = GetAppConfig("HOSTNAME")
            sc.ServiceName = service

            Select Case action.ToLower
                Case "restart"
                    Try
                        sc.Stop()
                        sc.WaitForStatus(ServiceControllerStatus.Stopped)
                        sc.Start()
                        sc.WaitForStatus(ServiceControllerStatus.Running)
                        Return True
                    Catch
                        Return False
                    End Try

                Case "stop"
                    Try
                        sc.Stop()
                        sc.WaitForStatus(ServiceControllerStatus.Stopped)
                        Return True
                    Catch
                        Return False
                    End Try

                Case "start"
                    Try
                        sc.Start()
                        sc.WaitForStatus(ServiceControllerStatus.Running)
                        Return True
                    Catch
                        Return False
                    End Try

            End Select

        End Function

        Public Shared Sub TVLog(ByVal logMessage As String)

            Try
                TvLibrary.Log.Log.Info(logMessage)
            Catch ex As Exception

            End Try

        End Sub

        Public Shared Function GetMPSafeFilename(ByVal sFilename) As String

            Dim sFName As String = sFilename.Replace(":"c, "_"c)
            sFName = sFName.Replace("/"c, "_"c)
            sFName = sFName.Replace("\"c, "_"c)
            sFName = sFName.Replace("*"c, "_"c)
            sFName = sFName.Replace("?"c, "_"c)
            sFName = sFName.Replace(""""c, "_"c)
            sFName = sFName.Replace("<"c, "_"c)
            sFName = sFName.Replace(">"c, "_"c)
            sFName = sFName.Replace("|"c, "_"c)

            Return sFilename

        End Function

    End Class

End Namespace
