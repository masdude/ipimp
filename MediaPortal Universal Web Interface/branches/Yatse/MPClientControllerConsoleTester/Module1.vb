﻿Imports Website

Module Module1

    Sub Main()

        Dim friendly As String = "diningroom"
        Dim transport As String = "http"
        Dim request As New uWiMP.TVServer.MPClient.Request
        request.Action = "ping"

        For Each arg As String In My.Application.CommandLineArgs
            If arg.ToLower.StartsWith("/transport=") Then transport = arg.Remove(0, 11)
            If arg.ToLower.StartsWith("/friendly=") Then friendly = arg.Remove(0, 10)
            If arg.ToLower.StartsWith("/action=") Then request.Action = arg.Remove(0, 8)
            If arg.ToLower.StartsWith("/filter=") Then request.Filter = arg.Remove(0, 8)
            If arg.ToLower.StartsWith("/value=") Then request.Value = arg.Remove(0, 7)
            If arg.ToLower.StartsWith("/start=") Then request.Start = CInt(arg.Remove(0, 7))
            If arg.ToLower.StartsWith("/pagesize=") Then request.PageSize = CInt(arg.Remove(0, 10))
            If arg.ToLower.StartsWith("/shuffle=") Then request.Shuffle = CBool(arg.Remove(0, 9))
            If arg.ToLower.StartsWith("/enqueue=") Then request.Enqueue = CBool(arg.Remove(0, 9))
            If arg.ToLower.StartsWith("/tracks=") Then request.Tracks = arg.Remove(0, 8)
        Next

        If transport.ToLower <> "http" Then transport = "tcp"
        SendReceiveMessage(transport, friendly, request)

    End Sub

    Private Sub SendReceiveMessage(ByVal transport As String, ByVal friendly As String, ByVal request As uWiMP.TVServer.MPClient.Request)
        Console.WriteLine(String.Format("Start {0}", Now))
        If transport.ToLower = "tcp" Then
            Console.WriteLine(uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, request))
        Else
            Console.WriteLine(uWiMP.TVServer.MPClientRemoting.SendHTTPSyncMessage(friendly, request))
        End If
        Console.WriteLine(String.Format("End {0}", Now))
    End Sub

End Module
