Public Partial Class DefaultLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim UA As String = Request.UserAgent

        If UA.ToLower.Contains("webkit") Then
            Response.Redirect("/iPhone/Login.aspx")
        Else
            Response.Redirect("/iPhone/Login.aspx")
        End If

    End Sub

End Class