Imports System.Threading
Imports MediaPortal.GUI.Video

Namespace MPClientController

    Public Class Video

        Private _movieid As Integer = 0

        Public Property movieID() As Integer
            Get
                Return _movieid
            End Get
            Set(ByVal value As Integer)
                _movieid = value
            End Set
        End Property

        Public Sub PlayVideo()
            SyncLock Me
                Dim t As New Thread(New ThreadStart(AddressOf Me.StartPlaying))
                t.Start()
            End SyncLock
        End Sub

        Private Sub StartPlaying()
            GUIVideoFiles.PlayMovie(_movieid)
        End Sub

    End Class

End Namespace
