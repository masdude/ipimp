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
Imports MediaPortal.Plugins.MovingPictures.Database
Imports MediaPortal.Player
Imports MediaPortal.Plugins.MovingPictures
Imports MediaPortal.GUI.Library
Imports System.IO

Namespace MPClientController

    Public NotInheritable Class MovingPictures

        Private Sub New()
        End Sub

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

        Public Shared Function FillNowPlaying(ByRef jw As JsonTextWriter) As Boolean

            Try
                Dim allMovies As New List(Of DBMovieInfo)
                allMovies = DBMovieInfo.GetAll
                Dim Found As Boolean = False

                For Each movieInfo As DBMovieInfo In allMovies
                    For Each mediaFile As DBLocalMedia In movieInfo.LocalMedia
                        If mediaFile.FullPath.ToLower = g_Player.Player.CurrentFile.ToLower Then
                            jw.WriteString("movingpicture")
                            jw.WriteMember("title")
                            jw.WriteString(movieInfo.Title)
                            jw.WriteMember("fanart")
                            jw.WriteString(String.Format("movingpicturefanart:{0}", Path.GetFileName(movieInfo.BackdropFullPath)))
                            jw.WriteMember("thumb")
                            jw.WriteString(String.Format("movingpicturethumb:{0}", Path.GetFileName(movieInfo.CoverFullPath)))
                            jw.WriteMember("tagline")
                            jw.WriteString(movieInfo.Tagline)
                            jw.WriteMember("id")
                            jw.WriteString(movieInfo.ID)
                            jw.WriteMember("genre")
                            jw.WriteString(Join(movieInfo.Genres.ToArray, " /"))
                            jw.WriteMember("filename")
                            jw.WriteString(g_Player.Player.CurrentFile)
                            jw.WriteMember("plot")
                            jw.WriteString(movieInfo.Summary)
                            jw.WriteMember("director")
                            jw.WriteString(Join(movieInfo.Directors.ToArray(), " / "))
                            jw.WriteMember("year")
                            jw.WriteString(movieInfo.Year)
                            jw.WriteMember("rating")
                            jw.WriteString(movieInfo.Score)
                            Found = True
                            Exit For
                        End If
                    Next
                    If Found Then
                        Exit For
                    End If
                Next
                Return Found
            Catch ex As Exception
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Gets a list of videos from the MovingPictures database.
        ''' </summary>
        ''' <param name="filter">Optional: subset of videos to return, valid values are Genre, Certification, Recent or Year. Omit to return all videos.</param>
        ''' <param name="value">Optional: filter to be used if subset is required, e.g. a genre, certification, period(days) or year.</param>
        ''' <returns>A JSON list of video ID's and titles.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetMovies(Optional ByVal filter As String = "", _
                                         Optional ByVal value As String = "", _
                                         Optional ByVal start As Integer = 0, _
                                         Optional ByVal pagesize As Integer = 0) As String

            Dim allMovies As New List(Of DBMovieInfo)
            allMovies = DBMovieInfo.GetAll

            Dim filteredMovies As New List(Of DBMovieInfo)
            Select Case filter.ToLower
                Case "genre"
                    For Each movie As DBMovieInfo In allMovies
                        For Each genre As String In movie.Genres
                            If genre.ToLower = value.ToLower Then filteredMovies.Add(movie)
                        Next
                    Next
                Case "year"
                    For Each movie As DBMovieInfo In allMovies
                        If movie.Year.ToString.ToLower = value.ToLower Then filteredMovies.Add(movie)
                    Next
                Case "certification"
                    If value = "-" Then value = " "
                    For Each movie As DBMovieInfo In allMovies
                        If movie.Certification.ToLower = value.ToLower Then filteredMovies.Add(movie)
                    Next
                Case "recent"
                    For Each movie As DBMovieInfo In allMovies
                        If movie.DateAdded > Now.AddDays(-CInt(value)) Then filteredMovies.Add(movie)
                    Next
                Case Else
                    filteredMovies = allMovies
            End Select

            Dim movies As New List(Of MPClientSmallMovieInfo)
            For Each movie As DBMovieInfo In filteredMovies
                Dim movieInfo As New MPClientSmallMovieInfo
                movieInfo.ID = movie.ID.ToString
                movieInfo.Title = movie.Title
                movies.Add(movieInfo)
            Next
            movies.Sort(New MPClientSmallMovieInfoComparer)

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(True)
                jw.WriteMember("movingpictures")
                jw.WriteStartArray()
                If pagesize = 0 Then
                    For Each movieInfo As MPClientSmallMovieInfo In movies
                        jw.WriteStartObject()
                        jw.WriteMember("id")
                        jw.WriteString(movieInfo.ID)
                        jw.WriteMember("title")
                        jw.WriteString(movieInfo.Title)
                        jw.WriteEndObject()
                    Next
                Else
                    For i As Integer = start To (start + (pagesize - 1))
                        jw.WriteStartObject()
                        jw.WriteMember("id")
                        jw.WriteString(movies(i).ID)
                        jw.WriteMember("title")
                        jw.WriteString(movies(i).Title)
                        jw.WriteEndObject()
                    Next
                End If
                jw.WriteEndArray()
                jw.WriteEndObject()

                Return jw.ToString
            End Using
        End Function


        ''' <summary>
        ''' Gets a list of videos from the Moving Pictures database.
        ''' </summary>
        ''' <returns>A JSON list of all movies with extended information.</returns>
        ''' <remarks></remarks>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")>
        Public Shared Function GetAllMovies() As String
            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(True)
                jw.WriteMember("movies")
                jw.WriteStartArray()
                Dim allMovies As New List(Of DBMovieInfo)
                allMovies = DBMovieInfo.GetAll

                For Each movieInfo As DBMovieInfo In allMovies
                    Dim file As String = ""
                    For Each mediaFile As DBLocalMedia In movieInfo.LocalMedia
                        file = file & mediaFile.FullPath & ";"
                    Next
                    jw.WriteStartObject()
                    jw.WriteMember("title")
                    jw.WriteString(movieInfo.Title)
                    jw.WriteMember("fanart")
                    Try
                        If (movieInfo.BackdropFullPath.Length < 2) Then
                            jw.WriteString("")
                        Else
                            jw.WriteString(String.Format("movingpicturefanart:{0}", Path.GetFileName(movieInfo.BackdropFullPath)))
                        End If
                    Catch ex As Exception
                        Log.Info("plugin: iPiMPClient - movingpicturefanart exception for {0}", movieInfo.Title)
                        jw.WriteString("")
                    End Try
                    jw.WriteMember("thumb")
                    Try
                        If (movieInfo.CoverFullPath.Length < 2) Then
                            jw.WriteString("")
                        Else
                            jw.WriteString(String.Format("movingpicturethumb:{0}", Path.GetFileName(movieInfo.CoverFullPath)))
                        End If
                    Catch ex As Exception
                        Log.Info("plugin: iPiMPClient - movingpicturethumb exception for {0}", movieInfo.Title)
                        jw.WriteString("")
                    End Try
                    jw.WriteMember("tagline")
                    jw.WriteString(movieInfo.Tagline)
                    jw.WriteMember("id")
                    jw.WriteString(movieInfo.ID)
                    jw.WriteMember("genre")
                    jw.WriteString(Join(movieInfo.Genres.ToArray, " /"))
                    jw.WriteMember("file")
                    jw.WriteString(file)
                    jw.WriteMember("path")
                    Try
                        jw.WriteString(Path.GetDirectoryName(file))
                    Catch ex As Exception
                        Log.Info("plugin: iPiMPClient - GetAllMovies - path exception for {0}", movieInfo.Title)
                        jw.WriteString("")
                    End Try
                    jw.WriteMember("plot")
                    jw.WriteString(movieInfo.Summary)
                    jw.WriteMember("director")
                    jw.WriteString(Join(movieInfo.Directors.ToArray(), " / "))
                    jw.WriteMember("year")
                    jw.WriteString(movieInfo.Year)
                    jw.WriteMember("votes")
                    jw.WriteString(movieInfo.Popularity)
                    jw.WriteMember("rating")
                    jw.WriteString(movieInfo.Score)
                    jw.WriteMember("mpaa")
                    jw.WriteString("")
                    jw.WriteMember("imdbnumber")
                    jw.WriteString(movieInfo.ImdbID)
                    jw.WriteMember("runtime")
                    jw.WriteString(movieInfo.Runtime)
                    jw.WriteMember("watched")
                    jw.WriteString(movieInfo.WatchedHistory.Count)
                    jw.WriteEndObject()
                Next

                jw.WriteEndArray()
                jw.WriteEndObject()
                Return jw.ToString()
            End Using
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="filter"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetVideoFilters(ByVal filter As String, Optional ByVal value As String = "") As String

            Dim allMovies As New List(Of DBMovieInfo)
            Dim filterResults As New List(Of String)
            allMovies = DBMovieInfo.GetAll

            Select Case filter.ToLower
                Case "genre"
                    For Each movie As DBMovieInfo In allMovies
                        For Each genre As String In movie.Genres
                            If Not filterResults.Contains(genre) Then filterResults.Add(genre)
                        Next
                    Next
                Case "certification"
                    Dim cert As String = String.Empty
                    For Each movie As DBMovieInfo In allMovies
                        cert = IIf(movie.Certification = " ", "-", movie.Certification)
                        If Not filterResults.Contains(cert) Then filterResults.Add(cert)
                    Next
                Case "year"
                    For Each movie As DBMovieInfo In allMovies
                        If Left(movie.Year.ToString.ToLower, 3) = Left(value.ToLower, 3) Then
                            If Not filterResults.Contains(movie.Year.ToString) Then filterResults.Add(movie.Year.ToString)
                        End If
                    Next
                Case "decade"
                    Dim decade As String = String.Empty
                    For Each movie As DBMovieInfo In allMovies
                        decade = IIf(movie.Year.ToString = "0", "0000", String.Format("{0}0", Left(movie.Year.ToString, 3)))
                        If Not filterResults.Contains(decade) Then filterResults.Add(decade)

                    Next
                Case Else
            End Select

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(True)
                jw.WriteMember(filter.ToLower)
                jw.WriteStringArray(filterResults.ToArray)
                jw.WriteEndObject()

                Return jw.ToString
            End Using
        End Function

        ''' <summary>
        ''' Gets video information from the native MediaPortal MovingPictures database
        ''' </summary>
        ''' <param name="ID">The ID of the video required.</param>
        ''' <returns>Video title, tagline, plot, runtime, rating, thumbURL and IMDB number.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetVideoInfo(ByVal ID As Integer) As String

            Dim movie As New DBMovieInfo
            movie = DBMovieInfo.Get(ID)

            Using jw As New JsonTextWriter
                jw.PrettyPrint = True
                jw.WriteStartObject()
                jw.WriteMember("result")
                jw.WriteBoolean(True)
                jw.WriteMember("title")
                jw.WriteString(movie.Title)
                jw.WriteMember("tagline")
                jw.WriteString(movie.Tagline)
                jw.WriteMember("plot")
                jw.WriteString(movie.Summary)
                jw.WriteMember("runtime")
                jw.WriteString(movie.Runtime)
                jw.WriteMember("Rating")
                jw.WriteString(movie.Score)
                jw.WriteMember("thumburl")
                jw.WriteString(movie.CoverThumbFullPath)
                jw.WriteMember("imdbnumber")
                jw.WriteString(movie.ImdbID)
                jw.WriteEndObject()
                Return jw.ToString
            End Using
        End Function

        Public Shared Sub PlayMovie(ByVal ID As Integer)

            Dim movieInfo As New DBMovieInfo
            movieInfo = DBMovieInfo.Get(ID)
            Dim movie As New MPClientController.MovingPicture
            movie.movieInfo = movieInfo
            movie.PlayVideo()

        End Sub

        Public Shared Function IsVideoIDPlaying(ByVal ID As Integer) As String

            Dim movieInfo As New DBMovieInfo
            movieInfo = DBMovieInfo.Get(ID)
            Dim match As Boolean = False

            Dim i As Integer = 0
            Do Until i = 15
                i += 1
                If (g_Player.Playing) And (g_Player.IsVideo) Then
                    For Each mediaFile As DBLocalMedia In movieInfo.LocalMedia
                        If mediaFile.FullPath.ToLower = g_Player.Player.CurrentFile.ToLower Then
                            match = True
                            Exit Do
                        End If
                    Next
                End If
                Threading.Thread.Sleep(1000)
            Loop

            Return iPiMPUtils.SendBool(match)

        End Function

        Public Shared Function GetThumb(ByVal fileName As String, ByVal isMovingPicturesPresent As Boolean) As String
            If isMovingPicturesPresent Then
                Return iPiMPUtils.GetImage(String.Format("{0}\{1}", MovingPicturesCore.Settings.CoverArtFolder, fileName))
            End If
            Return iPiMPUtils.GetImage(Nothing)

        End Function

        Public Shared Function GetFanart(ByVal fileName As String, ByVal isMovingPicturesPresent As Boolean) As String
            If isMovingPicturesPresent Then
                Return iPiMPUtils.GetImage(String.Format("{0}\{1}", MovingPicturesCore.Settings.BackdropFolder, fileName))
            End If
            Return iPiMPUtils.GetImage(Nothing)

        End Function

    End Class

End Namespace

