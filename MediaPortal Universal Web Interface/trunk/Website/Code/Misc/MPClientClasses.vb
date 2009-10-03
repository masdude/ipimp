' 
'   Copyright (C) 2008-2009 Martin van der Boon
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
