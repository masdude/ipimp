Imports Website

Module Module1

    Sub Main()

        Dim friendly As String = "diningroom"
        Dim request As New uWiMP.TVServer.MPClient.Request
        request.Action = "ping"
        
        For Each arg As String In My.Application.CommandLineArgs

            If arg.ToLower.StartsWith("/friendly=") Then
                friendly = arg.Remove(0, 10)
            Else
                friendly = "diningroom"
            End If

            If arg.ToLower.StartsWith("/action=") Then
                request.Action = arg.Remove(0, 8)
            Else
                request.Action = "ping"
            End If

            If arg.ToLower.StartsWith("/filter=") Then
                request.Filter = arg.Remove(0, 8)
            Else
                request.Filter = ""
            End If

            If arg.ToLower.StartsWith("/value=") Then
                request.Value = arg.Remove(0, 7)
            Else
                request.Value = ""
            End If

            If arg.ToLower.StartsWith("/start=") Then
                request.Start = CInt(arg.Remove(0, 7))
            Else
                request.Start = 0
            End If

            If arg.ToLower.StartsWith("/pagesize=") Then
                request.PageSize = CInt(arg.Remove(0, 10))
            Else
                request.PageSize = 0
            End If

            If arg.ToLower.StartsWith("/shuffle=") Then
                request.Shuffle = CBool(arg.Remove(0, 9))
            Else
                request.Shuffle = False
            End If

            If arg.ToLower.StartsWith("/enqueue=") Then
                request.Enqueue = CBool(arg.Remove(0, 9))
            Else
                request.Enqueue = False
            End If

            If arg.ToLower.StartsWith("/tracks=") Then
                request.Tracks = arg.Remove(0, 8)
            Else
                request.Tracks = ""
            End If

        Next

        SendReceiveMessage(friendly, request)

    End Sub

    Private Sub SendReceiveMessage(ByVal friendly As String, ByVal request As uWiMP.TVServer.MPClient.Request)
        Console.WriteLine(uWiMP.TVServer.MPClientRemoting.SendSyncMessage(friendly, request))
    End Sub

End Module
