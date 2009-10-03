Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Imports MediaPortal.Video.Database
Imports MediaPortal.Player

Namespace MPClientController

    Public Class MyVideos

        Private Class MPClientSmallMovieInfo
            Public ID As String
            Public Title As String
        End Class

        Private Class MPClientSmallMovieInfoComparer
            Implements IComparer(Of MPClientSmallMovieInfo)
            Public Overridable Overloads Function Compare(ByVal x As MPClientSmallMovieInfo, ByVal y As MPClientSmallMovieInfo) As Integer Implements IComparer(Of MPClientSmallMovieInfo).Compare
                Return x.Title.CompareTo(y.Title)
            End Function
        End Class

        ''' <summary>
        ''' Gets a list of videos from the native MediaPortal MyVideos database.
        ''' </summary>
        ''' <param name="filter">Optional: subset of videos to return, valid values are Genre or Year. Omit to return all videos.</param>
        ''' <param name="value">Optional: filter to be used if subset is required, e.g. a genre or year.</param>
        ''' <returns>A JSON list of video ID's and titles.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetMovies(Optional ByVal filter As String = "", _
                                         Optional ByVal value As String = "", _
                                         Optional ByVal start As Integer = 0, _
                                         Optional ByVal pagesize As Integer = 0) As String

            Dim allMovies As New ArrayList
            Select Case filter.ToLower
                Case "genre"
                    VideoDatabase.GetMoviesByGenre(value, allMovies)
                Case "year"
                    VideoDatabase.GetMoviesByYear(value, allMovies)
                Case Else
                    VideoDatabase.GetMovies(allMovies)
            End Select

            Dim movies As New List(Of MPClientSmallMovieInfo)
            For Each movie As IMDBMovie In allMovies
                Dim movieInfo As New MPClientSmallMovieInfo
                movieInfo.ID = movie.ID.ToString
                movieInfo.Title = movie.Title
                movies.Add(movieInfo)
            Next
            movies.Sort(New MPClientSmallMovieInfoComparer)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartArray()
            If pagesize = 0 Then
                For Each movieInfo As MPClientSmallMovieInfo In movies
                    jw.WriteStartObject()
                    jw.WriteMember("ID")
                    jw.WriteString(movieInfo.ID)
                    jw.WriteMember("Title")
                    jw.WriteString(movieInfo.Title)
                    jw.WriteEndObject()
                Next
            Else
                For i As Integer = start To (start + (pagesize - 1))
                    jw.WriteStartObject()
                    jw.WriteMember("ID")
                    jw.WriteString(movies(i).ID)
                    jw.WriteMember("Title")
                    jw.WriteString(movies(i).Title)
                    jw.WriteEndObject()
                Next
            End If
            jw.WriteEndArray()

            Return jw.ToString

        End Function

        ''' <summary>
        ''' Gets the values for a given filter.
        ''' </summary>
        ''' <param name="filter">The subset of videos, valid values are Genre or Decade.</param>
        ''' <param name="value">Optional, the decade to return years for.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetVideoFilters(ByVal filter As String, Optional ByVal value As String = "") As String

            Dim filters As New ArrayList

            Select Case filter.ToLower
                Case "genre"
                    VideoDatabase.GetGenres(filters)
                Case "year"
                    VideoDatabase.GetYears(filters)
                    Dim subset As New ArrayList
                    For Each year As String In filters
                        If Left(year.ToLower, 3) = Left(value.ToLower, 3) Then
                            If Not subset.Contains(year) Then subset.Add(year)
                        End If
                    Next
                    filters = subset
                Case "decade"
                    VideoDatabase.GetYears(filters)
                    Dim decades As New ArrayList
                    Dim decade As String = String.Empty
                    For Each year As String In filters
                        If year = "0" Then
                            decade = "0000"
                        Else
                            decade = Left(year, 3) & "0"
                        End If
                        If Not decades.Contains(decade) Then decades.Add(decade)
                    Next
                    filters = decades
                Case Else
            End Select

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember(filter.ToLower)
            jw.WriteStringArray(filters.ToArray)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        ''' <summary>
        ''' Gets video information from the native MediaPortal MyVideos database
        ''' </summary>
        ''' <param name="ID">The ID of the video required.</param>
        ''' <returns>Video title, tagline, plot, runtime, rating, thumbURL and IMDB number.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetVideoInfo(ByVal ID As Integer) As String

            Dim video As New IMDBMovie
            VideoDatabase.GetMovieInfoById(ID, video)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("Title")
            jw.WriteString(video.Title)
            jw.WriteMember("Tagline")
            jw.WriteString(video.TagLine)
            jw.WriteMember("Plot")
            jw.WriteString(video.Plot)
            jw.WriteMember("Runtime")
            jw.WriteString(video.RunTime)
            jw.WriteMember("Rating")
            jw.WriteString(video.Rating)
            jw.WriteMember("ThumbURL")
            jw.WriteString(video.ThumbURL)
            jw.WriteMember("IMDBNumber")
            jw.WriteString(video.IMDBNumber)
            jw.WriteEndObject()
            Return jw.ToString

        End Function

        Public Shared Function IsVideoIDPlaying(ByVal ID As Integer) As String

            Dim playingID As Integer = -1
            Try
                For i = 1 To 15
                    If (g_Player.Playing) And (g_Player.IsVideo) Then
                        playingID = VideoDatabase.GetMovieId(g_Player.Player.CurrentFile)
                        If playingID = ID Then Exit For
                    End If
                    System.Threading.Thread.Sleep(1000)
                Next
            Catch ex As Exception
            End Try

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            If ID = playingID Then
                jw.WriteBoolean(True)
            Else
                jw.WriteBoolean(False)
            End If
            jw.WriteEndObject()
            Return jw.ToString

        End Function

    End Class

End Namespace

