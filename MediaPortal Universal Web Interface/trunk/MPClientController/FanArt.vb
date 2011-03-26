' 
'   Copyright (C) 2008-2011 Martin van der Boon
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


Imports FanartHandler
Imports MediaPortal.Music.Database

Namespace MPClientController

    Public Class MPCCFanArt

        Friend Shared Function GetFanArtForSong(ByVal song As Song)

            Dim Result As Hashtable = FanartHandler.Utils.GetDbm().GetFanart(song.Artist.ToLowerInvariant(), "MusicFanart", 0)

            Dim Found As Boolean = False
            For Each DictionaryEntry As DictionaryEntry In Result
                If (DirectCast(DictionaryEntry.Value, FanartImage).Type = "MusicFanart") Then
                    Found = True
                End If
            Next

            If Found Then
                Return String.Format("{0}:{1}", "musicfanart", song.Artist.ToLowerInvariant())
            Else
                Return ""
            End If

        End Function

    End Class

End Namespace


