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


Imports TvDatabase

Namespace uWiMP.TVServer

    Public Class RadioChannels

        Public Shared Function GetRadioChannelsByGroupId(ByVal idGroup As Integer) As List(Of Channel)

            Dim layer As New TvBusinessLayer()
            Dim allRadioChannels As New List(Of Channel)
            Dim groupRadioChannels As New List(Of Channel)

            Try
                allRadioChannels = layer.GetAllRadioChannels
            Catch ex As Exception
                Return Nothing
            End Try

            Dim group As RadioChannelGroup = RadioChannelGroups.GetRadioChannelGroupByGroupId(idGroup)
            Dim groupMaps As IList(Of RadioGroupMap) = group.ReferringRadioGroupMap()

            For Each channel As Channel In allRadioChannels
                For Each map As RadioGroupMap In groupMaps
                    If map.IdChannel = channel.IdChannel Then groupRadioChannels.Add(channel)
                Next
            Next

            Return groupRadioChannels

        End Function

        Public Shared Function GetRadioChannelByChannelId(ByVal idChannel As Integer) As Channel

            Dim layer As New TvBusinessLayer()
            Return layer.GetChannel(idChannel)

        End Function

        Public Shared Function GetRadioChannelNameByChannelId(ByVal idChannel As Integer) As String

            Dim layer As New TvBusinessLayer()
            Return layer.GetChannel(idChannel).Name

        End Function

    End Class

    Public Class RadioChannelNameComparer
        Implements IComparer(Of Channel)
        Public Overridable Overloads Function Compare(ByVal x As Channel, ByVal y As Channel) As Integer Implements IComparer(Of Channel).Compare
            Return x.Name.ToLower.CompareTo(y.Name.ToLower)
        End Function
    End Class

    Public Class RadioChannelSortOrderComparer
        Implements IComparer(Of Channel)
        Public Overridable Overloads Function Compare(ByVal x As Channel, ByVal y As Channel) As Integer Implements IComparer(Of Channel).Compare
            Return x.SortOrder.CompareTo(y.SortOrder)
        End Function
    End Class

End Namespace
