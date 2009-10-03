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
                cg.Sort(New ChannelGroupsGroupNameComparer)
                Return cg
            Else
                Return Nothing
            End If

        End Function

        Public Shared Function GetChannelGroupByGroupId(ByVal idGroup As Integer) As ChannelGroup

            Return ChannelGroup.Retrieve(idGroup)

        End Function

    End Class

    Public Class ChannelGroupsGroupNameComparer
        Implements IComparer(Of ChannelGroup)
        Public Overridable Overloads Function Compare(ByVal x As ChannelGroup, ByVal y As ChannelGroup) As Integer Implements IComparer(Of ChannelGroup).Compare
            Return x.GroupName.CompareTo(y.GroupName)
        End Function
    End Class

End Namespace
