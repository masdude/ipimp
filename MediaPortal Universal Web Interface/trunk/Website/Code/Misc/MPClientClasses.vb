Namespace uWiMP.TVServer.MPClient

    Public Class Client
        Public Friendly As String
        Public Hostname As String
        Public MACAddress As String
        Public Port As String
    End Class

    Public Class Request
        Public Action As String
        Public Filter As String
        Public Value As String
        Public Start As Integer
        Public PageSize As Integer
        Public Shuffle As Boolean
        Public Enqueue As Boolean
        Public Tracks As String
    End Class

    Public Class SmallMovieInfo
        Public ID As String
        Public Title As String
    End Class

    Public Class SmallAlbumInfo
        Public Album As String
        Public Artist As String
    End Class

    Public Class AlbumTrack
        Public Title As String
        Public ID As Integer
        Public Artist As String
    End Class

    Public Class BigMovieInfo
        Public Title As String
        Public Tagline As String
        Public Plot As String
        Public Runtime As String
        Public Rating As String
        Public ThumbURL As String
        Public IMDBNumber As String
    End Class

End Namespace
