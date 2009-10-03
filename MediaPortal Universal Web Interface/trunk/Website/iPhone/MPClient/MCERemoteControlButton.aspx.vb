Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports System.IO
Imports System.Xml

Partial Public Class MCERemoteControlButton
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim friendly As String = Request.QueryString("friendly")
        Dim button As String = Request.QueryString("button")
        DoButton(friendly, button)

        
    End Sub

    Private Sub DoButton(ByVal friendly As String, ByVal button As String)

        Dim mpRequest As New uWiMP.TVServer.MPClient.Request
        mpRequest.Action = "button"
        mpRequest.Filter = button

        Dim response As String = uWiMP.TVServer.MPClientRemoting.SendAsyncMessage(friendly, mpRequest)

    End Sub

End Class