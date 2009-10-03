Public Partial Class _Default1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
        appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

        Dim UA As String = Request.UserAgent

        If UA.ToLower.Contains("webkit") Then
            Response.Redirect("/iPhone/Default.aspx")
        Else
            Response.Redirect("/iPhone/Default.aspx")
        End If

    End Sub

End Class