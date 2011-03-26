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

    Public Class ChannelGroups

        Public Shared Function GetChannelGroups() As List(Of ChannelGroup)

            Dim cg As New List(Of ChannelGroup)

            Try
                cg = ChannelGroup.ListAll
            Catch ex As Exception
                Return Nothing
            End Try

            If cg.Count > 1 Then
                If CBool(uWiMP.TVServer.Utilities.GetAppConfig("SORTLISTSBYNAME")) = True Then cg.Sort(New ChannelGroupsGroupNameComparer)
            End If

            Return cg

        End Function

        Public Shared Function GetChannelGroupByGroupId(ByVal idGroup As Integer) As ChannelGroup

            Return ChannelGroup.Retrieve(idGroup)

        End Function

        Public Shared Function GetFirstChannelGroupName() As String

            Dim cg As New List(Of ChannelGroup)
            cg = ChannelGroup.ListAll

            Return cg(0).IdGroup

        End Function

    End Class

    Public Class ChannelGroupsGroupNameComparer
        Implements IComparer(Of ChannelGroup)
        Public Overridable Overloads Function Compare(ByVal x As ChannelGroup, ByVal y As ChannelGroup) As Integer Implements IComparer(Of ChannelGroup).Compare
            Return x.GroupName.CompareTo(y.GroupName)
        End Function
    End Class

End Namespace
