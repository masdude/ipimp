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
