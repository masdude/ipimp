Imports System.IO
Imports System.Xml

Partial Public Class MPClientMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "text/xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim wa As String = "waMPClient"
        Dim friendly As String = Request.QueryString("friendly")

        Dim tw As TextWriter = New StreamWriter(Response.OutputStream, Encoding.UTF8)
        Dim xw As XmlWriter = New XmlTextWriter(tw)

        'start doc
        xw.WriteStartDocument()

        'start root
        xw.WriteStartElement("root")

        'go
        xw.WriteStartElement("go")
        xw.WriteAttributeString("to", wa)
        xw.WriteEndElement()
        'end go

        'start title
        xw.WriteStartElement("title")
        xw.WriteAttributeString("set", wa)
        xw.WriteEndElement()
        'end title

        'start dest
        xw.WriteStartElement("destination")
        xw.WriteAttributeString("mode", "replace")
        xw.WriteAttributeString("zone", wa)
        xw.WriteAttributeString("create", "true")
        xw.WriteEndElement()
        'end dest

        'start data
        xw.WriteStartElement("data")
        xw.WriteCData(DisplayMPClientMenu(wa, friendly))
        xw.WriteEndElement()
        'end data

        'end root
        xw.WriteEndElement()

        'end doc
        xw.WriteEndDocument()
        xw.Close()

    End Sub

    Private Function DisplayMPClientMenu(ByVal wa As String, ByVal friendly As String) As String

        Dim client As uWiMP.TVServer.MPClient.Client
        client = uWiMP.TVServer.MPClientDatabase.GetClient(friendly)

        Dim markup As String = String.Empty

        markup += String.Format("<div class=""iMenu"" id=""{0}"">", wa)
        markup += String.Format("<h3>{0}</h3>", friendly)
        markup += "<ul class=""iArrow"">"

        If uWiMP.TVServer.MPClientRemoting.CanConnect(client.Friendly) Then
            markup += String.Format("<li><a href=""MPClient/MCERemoteControl.aspx?friendly={0}#_Remote1"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "remote_control"))
            markup += String.Format("<li><a href=""MPClient/MyVideosMenu.aspx?friendly={0}#_MyVideos"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "my_videos"))
            markup += String.Format("<li><a href=""MPClient/MyMusicMenu.aspx?friendly={0}#_MyMusic"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "my_music"))
            markup += String.Format("<li><a href=""MPClient/NowPlaying.aspx?friendly={0}#_MPClientNowPlaying"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "now_playing"))
            markup += String.Format("<li><a href=""MPClient/MPClientSendMessage.aspx?friendly={0}#_MPClientSendMessage"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "send_message"))
            markup += String.Format("<li><a href=""MPClient/MPClientPowerOptions.aspx?friendly={0}#_MPClientPowerOptions"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "power_options"))
        ElseIf client.MACAddress <> "" Then
            markup += String.Format("<li style=""color:red""><a href=""MPClient/MPClientWOL.aspx?friendly={0}#_MPClientWOL"" rev=""async"">{1}</a></li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "could_not_connect_wol"))
        Else
            markup += String.Format("<li style=""color:red"">{1}</li>", client.Friendly, GetGlobalResourceObject("uWiMPStrings", "could_not_connect"))
        End If

        markup += "</ul>"
        markup += "</div>"

        Return markup

    End Function

End Class