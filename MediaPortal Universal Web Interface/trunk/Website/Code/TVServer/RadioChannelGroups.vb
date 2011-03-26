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

    Public Class RadioChannelGroups

        Public Shared Function GetRadioChannelGroups() As List(Of RadioChannelGroup)

            Dim cg As New List(Of RadioChannelGroup)

            Try
                cg = RadioChannelGroup.ListAll
            Catch ex As Exception
                Return Nothing
            End Try

            If cg.Count > 1 Then
                cg.Sort(New RadioChannelGroupsGroupNameComparer)
            End If

            Return cg

        End Function

        Public Shared Function GetRadioChannelGroupByGroupId(ByVal idGroup As Integer) As RadioChannelGroup

            Return RadioChannelGroup.Retrieve(idGroup)

        End Function

    End Class

    Public Class RadioChannelGroupsGroupNameComparer
        Implements IComparer(Of RadioChannelGroup)
        Public Overridable Overloads Function Compare(ByVal x As RadioChannelGroup, ByVal y As RadioChannelGroup) As Integer Implements IComparer(Of RadioChannelGroup).Compare
            Return x.GroupName.CompareTo(y.GroupName)
        End Function
    End Class

End Namespace
