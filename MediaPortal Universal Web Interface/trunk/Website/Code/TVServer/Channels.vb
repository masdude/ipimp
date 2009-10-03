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


Imports TvDatabase

Namespace uWiMP.TVServer

    Public Class Channels

        Public Shared Function GetChannelsByGroupId(ByVal idGroup As Integer) As List(Of Channel)

            Dim layer As New TvBusinessLayer()
            Return layer.GetTVGuideChannelsForGroup(idGroup)

        End Function

        Public Shared Function GetChannelByChannelId(ByVal idChannel As Integer) As Channel

            Dim layer As New TvBusinessLayer()
            Return layer.GetChannel(idChannel)

        End Function

        Public Shared Function GetChannelNameByChannelId(ByVal idChannel As Integer) As String

            Dim layer As New TvBusinessLayer()
            Return layer.GetChannel(idChannel).Name

        End Function

    End Class

    Public Class ChannelNameComparer
        Implements IComparer(Of Channel)
        Public Overridable Overloads Function Compare(ByVal x As Channel, ByVal y As Channel) As Integer Implements IComparer(Of Channel).Compare
            Return x.Name.ToLower.CompareTo(y.Name.ToLower)
        End Function
    End Class

    Public Class ChannelSortOrderComparer
        Implements IComparer(Of Channel)
        Public Overridable Overloads Function Compare(ByVal x As Channel, ByVal y As Channel) As Integer Implements IComparer(Of Channel).Compare
            Return x.SortOrder.CompareTo(y.SortOrder)
        End Function
    End Class

End Namespace
