Imports System.Web.SessionState
Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Const listenport As Integer = 55667
    Dim listener As UdpClient
    Dim listenerThread As Threading.Thread

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
        Dim running As Boolean = False
        If Not running Then
            listener = New UdpClient(listenport)
            listenerThread = New System.Threading.Thread(AddressOf StartUDPListener)
            listenerThread.IsBackground = True
            listenerThread.Start()
            running = True
        End If
    End Sub

    Private Sub StartUDPListener()

        'Listen for iPiMP process plugin broadcasts
        Dim done As Boolean = False

        listener.EnableBroadcast = True
        Dim groupEP As New IPEndPoint(IPAddress.Any, listenport)

        Try
            While Not done
                Dim bytes As Byte() = listener.Receive(groupEP)
                UpdateOrAddMPClient(Encoding.ASCII.GetString(bytes, 0, bytes.Length))
            End While
        Catch ex As Exception
        Finally
            listener.Close()
        End Try

    End Sub
    Private Sub UpdateOrAddMPClient(ByVal broadcastMessage As String)

        Dim clientInfo() As String = Split(broadcastMessage, ",")
        Dim client As New uWiMP.TVServer.MPClient.Client
        client.Friendly = clientInfo(0)
        client.Hostname = clientInfo(0)
        client.MACAddress = clientInfo(1)
        client.Port = clientInfo(2)
        client.usesMovingPictures = clientInfo(3)
        client.usesTVSeries = clientInfo(4)

        If uWiMP.TVServer.MPClientDatabase.IsExistingClientByHostname(client.Hostname) Then
            Dim oldClient As uWiMP.TVServer.MPClient.Client = uWiMP.TVServer.MPClientDatabase.GetClientByHostname(client.Hostname)
            client.Friendly = oldClient.Friendly
            Dim result As Boolean = uWiMP.TVServer.MPClientDatabase.ManageClient(client, "update")
        Else
            Dim result As Boolean = uWiMP.TVServer.MPClientDatabase.ManageClient(client, "insert")
        End If

        End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
        Dim ex As Exception = Server.GetLastError.GetBaseException()
        Application("fubar") = ex
        Server.ClearError()
        Dim UA As String = Request.UserAgent

        If UA.ToLower.Contains("webkit") Then
            Server.Transfer("~/iPhone/Error.aspx")
        Else
            Server.Transfer("~/iPhone/Error.aspx")
        End If

    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
        listener.Close()
        listenerThread.Abort()
    End Sub

End Class