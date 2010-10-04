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


Imports System.IO
Imports Jayrock.Json
Imports WindowPlugins.GUITVSeries
Imports MediaPortal.Player


Namespace MPClientController

    Public Class TVSeries

        Public Shared Function GetSeriesPoster(ByVal seriesID As Integer) As String

            Dim series As DBSeries = DBSeries.Get(seriesID)
            Dim filename As String = WindowPlugins.GUITVSeries.ImageAllocator.GetSeriesPosterAsFilename(series)

            Return iPiMPUtils.GetImage(filename)

        End Function

        Public Shared Function GetSeriesFanart(ByVal seriesID As Integer) As String

            Dim filename As String = Nothing
            filename = Fanart.getFanart(seriesID).FanartFilename

            Return iPiMPUtils.GetImage(filename)

        End Function

        Public Shared Function GetSeasonPoster(ByVal seriesID As Integer, ByVal index As Integer) As String

            Dim season As DBSeason = DBSeason.getRaw(seriesID, index)
            Dim filename As String = WindowPlugins.GUITVSeries.ImageAllocator.GetSeasonBannerAsFilename(season)

            Return iPiMPUtils.GetImage(filename)

        End Function

        Public Shared Function GetSeasonFanart(ByVal seriesID As Integer, ByVal index As Integer) As String

            Dim filename As String = Fanart.getFanart(seriesID, index).FanartFilename

            Return iPiMPUtils.GetImage(filename)

        End Function

        Public Shared Function GetEpisodeThumb(ByVal episodeID As String) As String

            Dim sqlCondition As New SQLCondition
            sqlCondition.Add(New DBOnlineEpisode(), DBOnlineEpisode.cCompositeID, episodeID, SQLConditionType.Equal)

            Dim episodeList As List(Of DBEpisode) = DBEpisode.Get(sqlCondition)
            Dim filename As String = Nothing

            For Each episode As DBEpisode In episodeList
                filename = episode.Image
            Next

            Return iPiMPUtils.GetImage(filename)

        End Function

        Public Shared Function GetAllEpisodes() As String

            Dim sqlCondition As New SQLCondition
            sqlCondition.Add(New DBOnlineSeries(), DBOnlineSeries.cViewTags, "", SQLConditionType.Like)

            Dim seriesList As List(Of DBSeries) = DBSeries.Get(sqlCondition)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("episodes")
            jw.WriteStartArray()

            For Each series As DBSeries In seriesList

                Dim sqlCondition2 As New SQLCondition
                sqlCondition2.Add(New DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series.Item("ID"), SQLConditionType.Equal)

                Dim episodeList As List(Of DBEpisode) = DBEpisode.Get(sqlCondition2)

                For Each episode As DBEpisode In episodeList
                    jw.WriteStartObject()
                    jw.WriteMember("id")
                    jw.WriteString(episode.Item(DBOnlineEpisode.cEpisodeIndex))
                    jw.WriteMember("show")
                    jw.WriteString(series.Item("Pretty_Name"))
                    jw.WriteMember("idshow")
                    jw.WriteString(series.Item("ID"))
                    jw.WriteMember("path")
                    jw.WriteString(episode.Item(DBEpisode.cCompositeID))
                    jw.WriteMember("episode")
                    jw.WriteString(episode.Item(DBEpisode.cEpisodeIndex))
                    jw.WriteMember("season")
                    jw.WriteString(episode.Item(DBEpisode.cSeasonIndex))
                    jw.WriteMember("name")
                    jw.WriteString(episode.Item(DBEpisode.cEpisodeName))
                    jw.WriteMember("plot")
                    jw.WriteString(episode.Item(DBOnlineEpisode.cEpisodeSummary))
                    jw.WriteMember("aired")
                    jw.WriteString(episode.Item(DBOnlineEpisode.cFirstAired))
                    jw.WriteMember("director")
                    jw.WriteString(episode.Item(DBOnlineEpisode.cDirector))
                    jw.WriteMember("rating")
                    jw.WriteString(episode.Item(DBOnlineEpisode.cRating))
                    jw.WriteMember("filename")
                    jw.WriteString(episode.Item(DBEpisode.cFilename))
                    jw.WriteMember("watched")
                    jw.WriteString(episode.Item(DBOnlineEpisode.cWatched))
                    jw.WriteMember("studio")
                    jw.WriteString(series.Item("Network"))
                    jw.WriteMember("thumb")
                    If (File.Exists(episode.Image)) Then
                        jw.WriteString(String.Format("{0}:{1}", "tvepisodethumb", episode.Item(DBOnlineEpisode.cCompositeID)))
                    Else
                        jw.WriteString("")
                    End If
                    jw.WriteMember("fanart")
                    jw.WriteString("")
                    jw.WriteEndObject()
                Next
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetAllSeasons() As String

            Dim sqlCondition As New SQLCondition
            sqlCondition.Add(New DBOnlineSeries(), DBOnlineSeries.cViewTags, "", SQLConditionType.Like)

            Dim seriesList As List(Of DBSeries) = DBSeries.Get(sqlCondition)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("seasons")
            jw.WriteStartArray()
            For Each series As DBSeries In seriesList

                Dim seasonList As List(Of DBSeason) = DBSeason.Get(CInt(series.Item("ID")))

                For Each season As DBSeason In seasonList
                    jw.WriteStartObject()
                    jw.WriteMember("id")
                    jw.WriteString(season.Item(DBSeason.cID))
                    jw.WriteMember("show")
                    jw.WriteString(series.Item("Pretty_Name"))
                    jw.WriteMember("seasonnumber")
                    jw.WriteString(season.Item(DBSeason.cIndex))
                    jw.WriteMember("episodecount")
                    jw.WriteString(season.Item(DBSeason.cEpisodeCount))
                    jw.WriteMember("thumb")
                    If (File.Exists(WindowPlugins.GUITVSeries.ImageAllocator.GetSeasonBannerAsFilename(season))) Then
                        jw.WriteString(String.Format("{0}:{1}:{2}", "tvseasonposter", series.Item("ID"), season.Item(DBSeason.cIndex)))
                    Else
                        jw.WriteString("")
                    End If

                    jw.WriteMember("fanart")
                    If (File.Exists(Fanart.getFanart(series.Item("ID"), season.Item(DBSeason.cIndex)).FanartFilename)) Then
                        jw.WriteString(String.Format("{0}:{1}:{2}", "tvseasonfanart", series.Item("ID"), season.Item(DBSeason.cIndex)))
                    Else
                        jw.WriteString("")
                    End If
                    jw.WriteEndObject()
                Next
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function


        Public Shared Function GetAllSeriesDetails() As String

            Dim sqlCondition As New SQLCondition
            sqlCondition.Add(New DBOnlineSeries(), DBOnlineSeries.cViewTags, "", SQLConditionType.Like)

            Dim seriesList As List(Of DBSeries) = DBSeries.Get(sqlCondition)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("series")
            jw.WriteStartArray()
            For Each series As DBSeries In seriesList
                jw.WriteStartObject()
                jw.WriteMember("id")
                jw.WriteString(series.Item("ID"))
                jw.WriteMember("name")
                jw.WriteString(series.Item("Pretty_Name"))
                jw.WriteMember("genre")
                jw.WriteString(series.Item("Genre"))
                jw.WriteMember("plot")
                jw.WriteString(series.Item("Summary"))
                jw.WriteMember("episodecount")
                jw.WriteString(series.Item("EpisodeCount"))
                jw.WriteMember("firstaired")
                jw.WriteString(series.Item("FirstAired"))
                jw.WriteMember("mpaa")
                jw.WriteString(series.Item("ContentRating"))
                jw.WriteMember("studio")
                jw.WriteString(series.Item("Network"))
                jw.WriteMember("rating")
                jw.WriteString(series.Item("Rating"))

                jw.WriteMember("thumb")
                If (File.Exists(WindowPlugins.GUITVSeries.ImageAllocator.GetSeriesPosterAsFilename(series))) Then
                    jw.WriteString(String.Format("{0}:{1}", "tvseriesposter", series.Item("ID")))
                Else
                    jw.WriteString("")
                End If

                jw.WriteMember("fanart")
                If (File.Exists(Fanart.getFanart(series.Item("ID")).FanartFilename)) Then
                    jw.WriteString(String.Format("{0}:{1}", "tvseriesfanart", series.Item("ID")))
                Else
                    jw.WriteString("")
                End If

                jw.WriteEndObject()
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetAllSeries() As String

            Dim sqlCondition As New SQLCondition
            sqlCondition.Add(New DBOnlineSeries(), DBOnlineSeries.cViewTags, "", SQLConditionType.Like)
            sqlCondition.AddOrderItem(DBOnlineSeries.Q(DBOnlineSeries.cPrettyName), sqlCondition.orderType.Ascending)

            Dim seriesList As List(Of DBSeries) = DBSeries.Get(sqlCondition)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("series")
            jw.WriteStartArray()
            For Each series As DBSeries In seriesList
                jw.WriteStartObject()
                jw.WriteMember("id")
                jw.WriteString(series.Item("ID"))
                jw.WriteMember("name")
                jw.WriteString(series.Item("Pretty_Name"))
                jw.WriteEndObject()
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetSeries(ByVal seriesID As String) As String

            Dim series As DBSeries = DBSeries.Get(CInt(seriesID))

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("name")
            jw.WriteString(series.Item("Pretty_Name"))
            jw.WriteMember("summary")
            jw.WriteString(series.Item("Summary"))
            jw.WriteMember("banner")
            jw.WriteString(series.Banner)
            jw.WriteMember("poster")
            jw.WriteString(series.Poster)
            jw.WriteEndObject()

            Return jw.ToString

        End Function


        Public Shared Function GetSeasons(ByVal seriesID As String) As String

            Dim seasonList As List(Of DBSeason) = DBSeason.Get(CInt(seriesID))

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("seasons")
            jw.WriteStartArray()
            For Each season As DBSeason In seasonList
                jw.WriteStartObject()
                jw.WriteMember("id")
                jw.WriteString(season.Item(DBSeason.cID))
                jw.WriteEndObject()
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetSeason(ByVal compositeID As String) As String

            Dim seriesID As Integer = Split(compositeID, "_")(0)
            Dim seasonIndex As Integer = Split(compositeID, "_")(1)

            Dim seasonList As List(Of DBSeason) = DBSeason.Get(seriesID)
            Dim season As DBSeason = Nothing
            For Each season In seasonList
                If CInt(season.Item(DBSeason.cIndex)) = seasonIndex Then
                    Exit For
                End If
            Next

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("id")
            jw.WriteString(season.Item(DBSeason.cID))
            jw.WriteMember("index")
            jw.WriteString(season.Item(DBSeason.cIndex))
            jw.WriteMember("banner")
            jw.WriteString(season.Item(DBSeason.cCurrentBannerFileName))
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetEpisodes(ByVal compositeID As String) As String

            Dim seriesID As Integer = Split(compositeID, "_")(0)
            Dim seasonIndex As Integer = Split(compositeID, "_")(1)

            Dim episodeList As List(Of DBEpisode) = DBEpisode.Get(seriesID, seasonIndex)

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("episodes")
            jw.WriteStartArray()
            For Each episode As DBEpisode In episodeList
                jw.WriteStartObject()
                jw.WriteMember("id")
                jw.WriteString(episode.Item(DBEpisode.cCompositeID))
                jw.WriteMember("index")
                jw.WriteString(episode.Item(DBEpisode.cEpisodeIndex))
                jw.WriteMember("name")
                jw.WriteString(episode.Item(DBEpisode.cEpisodeName))
                jw.WriteEndObject()
            Next
            jw.WriteEndArray()
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function GetEpisode(ByVal compositeID As String) As String

            Dim seriesID As Integer = Split(compositeID, "_")(0)
            Dim seasonIndex As Integer = Split(Split(compositeID, "_")(1), "x")(0)
            Dim episodeIndex As Integer = Split(Split(compositeID, "_")(1), "x")(1)

            Dim episodeList As List(Of DBEpisode) = DBEpisode.Get(seriesID, seasonIndex)

            Dim episode As DBEpisode = Nothing
            For Each episode In episodeList
                If CInt(episode.Item(DBEpisode.cEpisodeIndex)) = episodeIndex Then
                    Exit For
                End If
            Next

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteMember("compositeid")
            jw.WriteString(episode.Item(DBEpisode.cCompositeID))
            jw.WriteMember("name")
            jw.WriteString(episode.Item(DBEpisode.cEpisodeName))
            jw.WriteMember("summary")
            jw.WriteString(episode.Item("Summary"))
            jw.WriteMember("watched")
            jw.WriteString(episode.Item("Watched"))
            jw.WriteMember("image")
            jw.WriteString(episode.Image)
            jw.WriteMember("filename")
            jw.WriteString(episode(DBEpisode.cFilename))
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Public Shared Function IsEpisodeIDPlaying(ByVal compositeID As String) As String

            Dim seriesID As Integer = Split(compositeID, "_")(0)
            Dim seasonIndex As Integer = Split(Split(compositeID, "_")(1), "x")(0)
            Dim episodeIndex As Integer = Split(Split(compositeID, "_")(1), "x")(1)

            Dim episodeList As List(Of DBEpisode) = DBEpisode.Get(seriesID, seasonIndex)

            Dim episode As DBEpisode = Nothing
            For Each episode In episodeList
                If CInt(episode.Item(DBEpisode.cEpisodeIndex)) = episodeIndex Then
                    Exit For
                End If
            Next

            Try
                For i = 1 To 15
                    If (g_Player.Playing) And (g_Player.currentFileName.ToLower = episode.Item(DBEpisode.cFilename).ToString.ToLower) Then
                        Return iPiMPUtils.SendBool(True)
                    Else
                        System.Threading.Thread.Sleep(1000)
                    End If
                Next
            Catch ex As Exception
            End Try

            Return iPiMPUtils.SendBool(False)

        End Function

    End Class

End Namespace
