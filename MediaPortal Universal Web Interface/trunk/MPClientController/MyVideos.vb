' 
'   Copyright (C) 2008-2010 Martin van der Boon
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


Imports Jayrock.Json
Imports MediaPortal.Video.Database
Imports MediaPortal.Player
Imports MediaPortal.Util
Imports System.IO

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
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("movies")
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
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        ''' <summary>
        ''' Gets a list of videos from the native MediaPortal MyVideos database.
        ''' </summary>
        ''' <returns>A JSON list of all movies with extended information.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllMovies() As String

            Dim allMovies As New ArrayList
            VideoDatabase.GetMovies(allMovies)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("movies")
            jw.WriteStartArray()

            For Each movie As IMDBMovie In allMovies

                Dim fileList As New ArrayList
                VideoDatabase.GetFiles(movie.ID, fileList)

                Dim files As String = Join(fileList.ToArray, ";")
                Dim crc As New CRCTool
                crc.Init(CRCTool.CRCCode.CRC32)

                jw.WriteStartObject()
                jw.WriteMember("Thumb")

                If (File.Exists(String.Format("{0}\{1}L{2}", Thumbs.MovieTitle, MediaPortal.Util.Utils.MakeFileName(movie.Title), MediaPortal.Util.Utils.GetThumbExtension()))) Then
                    jw.WriteString(String.Format("videotitle:{0}", MediaPortal.Util.Utils.MakeFileName(movie.Title)))
                ElseIf (File.Exists(String.Format("{0}\{1}L{2}", Thumbs.Videos, crc.calc(files).ToString, MediaPortal.Util.Utils.GetThumbExtension()))) Then
                    jw.WriteString(String.Format("videothumb:{0}", crc.calc(files).ToString))
                Else
                    jw.WriteString("NONE")
                End If

                jw.WriteMember("Fanart")
                jw.WriteString("NONE")
                jw.WriteMember("File")
                jw.WriteString(files)
                jw.WriteMember("Title")
                jw.WriteString(movie.Title)
                jw.WriteMember("Id")
                jw.WriteString(movie.ID)
                jw.WriteMember("Year")
                jw.WriteString(movie.Year)
                jw.WriteMember("Tagline")
                jw.WriteString(movie.TagLine)
                jw.WriteMember("Plot")
                jw.WriteString(movie.Plot)
                jw.WriteMember("Runtime")
                jw.WriteString(movie.RunTime)
                jw.WriteMember("Rating")
                jw.WriteString(movie.Rating)
                jw.WriteMember("ThumbURL")
                jw.WriteString(movie.ThumbURL)
                jw.WriteMember("IMDBNumber")
                jw.WriteString(movie.IMDBNumber)
                jw.WriteMember("Path")
                jw.WriteString(movie.Path)
                jw.WriteMember("Director")
                jw.WriteString(movie.Director)
                jw.WriteMember("Genre")
                jw.WriteString(movie.Genre)
                jw.WriteMember("Mpaa")
                jw.WriteString(movie.MPARating)
                jw.WriteMember("Votes")
                jw.WriteString(movie.Votes)
                jw.WriteMember("Watched")
                jw.WriteString(movie.Watched)
                jw.WriteEndObject()
            Next
            jw.WriteEndArray()

            jw.WriteEndObject()
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
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember(filter.ToLower)
            jw.WriteStringArray(filters.ToArray)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        ''' <summary>
        ''' Gets video information from the native MediaPortal MyVideos database
        ''' </summary>
        ''' <param name="ID">The ID of the video required.</param>
        ''' <returns>Video title, tagline, plot, runtime, rating, thumbURL, IMDB number, file and path.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetVideoInfo(ByVal ID As Integer) As String

            Dim video As New IMDBMovie
            VideoDatabase.GetMovieInfoById(ID, video)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
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
            jw.WriteMember("File")
            jw.WriteString(video.File)
            jw.WriteMember("Path")
            jw.WriteString(video.Path)
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

            If ID = playingID Then
                Return iPiMPUtils.SendBool(True)
            Else
                Return iPiMPUtils.SendBool(False)
            End If

        End Function

        Public Shared Function GetVideoTitle(ByVal title As String, ByVal size As String) As String

            Dim filename As String = Nothing
            filename = String.Format("{0}\{1}{2}{3}", Thumbs.MovieTitle, title, IIf(size.ToLower = "large", "L", ""), Utils.GetThumbExtension())

            Return iPiMPUtils.GetImage(filename)

        End Function

        Public Shared Function GetVideoThumb(ByVal title As String, ByVal size As String) As String

            Dim filename As String = Nothing
            filename = String.Format("{0}\{1}{2}{3}", Thumbs.Videos, title, IIf(size.ToLower = "large", "L", ""), Utils.GetThumbExtension())

            Return iPiMPUtils.GetImage(filename)

        End Function

    End Class

End Namespace

