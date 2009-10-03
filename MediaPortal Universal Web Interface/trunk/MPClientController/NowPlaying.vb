Imports MediaPortal.Player
Imports MediaPortal.Music.Database
Imports MediaPortal.Video.Database
Imports MediaPortal.Util.Utils
Imports Jayrock.Json
Imports Jayrock.Json.Conversion

Namespace MPClientController


    Public Class NowPlaying

        Public Shared Function GetNowPlaying() As String

            Dim result As String = String.Empty

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()

            If g_Player.Playing Then

                If g_Player.IsMusic Then
                    Dim song As New Song
                    Dim musicdb As MusicDatabase = MusicDatabase.Instance
                    musicdb.GetSongByFileName(g_Player.Player.CurrentFile, song)
                    jw.WriteMember("media")
                    jw.WriteString("music")
                    jw.WriteMember("album")
                    jw.WriteString(song.Album)
                    jw.WriteMember("artist")
                    jw.WriteString(song.Artist)
                    jw.WriteMember("title")
                    jw.WriteString(song.Title)
                    jw.WriteMember("genre")
                    jw.WriteString(song.Genre)
                    jw.WriteMember("track")
                    jw.WriteString(song.Track.ToString)
                ElseIf g_Player.IsVideo Then
                    If MediaPortal.Util.Utils.IsDVD(g_Player.Player.CurrentFile) Then
                        jw.WriteMember("media")
                        jw.WriteString("dvd")
                    Else
                        Dim movie As New IMDBMovie
                        Dim movieID As Integer = VideoDatabase.GetMovieId(g_Player.Player.CurrentFile)
                        VideoDatabase.GetMovieInfoById(movieID, movie)
                        jw.WriteMember("media")
                        jw.WriteString("video")
                        jw.WriteMember("title")
                        jw.WriteString(movie.Title)
                        jw.WriteMember("tagline")
                        jw.WriteString(movie.TagLine)
                        jw.WriteMember("id")
                        jw.WriteString(movie.ID)
                        jw.WriteMember("genre")
                        jw.WriteString(movie.Genre)
                        jw.WriteMember("filename")
                        jw.WriteString(MediaPortal.Util.Utils.SplitFilename(g_Player.Player.CurrentFile.ToString))
                    End If
                ElseIf g_Player.IsTVRecording Then
                    jw.WriteMember("media")
                    jw.WriteString("recording")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                    jw.WriteMember("filename")
                    jw.WriteString(MediaPortal.Util.Utils.SplitFilename(g_Player.Player.CurrentFile.ToString))
                ElseIf g_Player.IsRadio Then
                    jw.WriteMember("media")
                    jw.WriteString("radio")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                ElseIf g_Player.IsTV Or (g_Player.IsTimeShifting) Then
                    jw.WriteMember("media")
                    jw.WriteString("tv")
                    jw.WriteMember("hostname")
                    jw.WriteString(System.Net.Dns.GetHostName.ToString)
                Else
                    jw.WriteMember("media")
                    jw.WriteString("unknown")
                End If
                jw.WriteMember("duration")
                jw.WriteString(g_Player.Player.Duration.ToString)
                jw.WriteMember("position")
                jw.WriteString(g_Player.Player.CurrentPosition.ToString)
            Else
                jw.WriteMember("media")
                jw.WriteString("nothing")
            End If

            jw.WriteEndObject()

            Return jw.ToString

        End Function

    End Class

End Namespace