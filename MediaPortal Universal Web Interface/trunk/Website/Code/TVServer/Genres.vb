Imports TvDatabase

Namespace uWiMP.TVServer

    Public Class Genres

        Public Shared Function GetGenres() As List(Of String)

            Dim layer As New TvBusinessLayer
            Return layer.GetGenres

        End Function

    End Class

End Namespace


