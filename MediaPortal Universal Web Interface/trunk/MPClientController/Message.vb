Imports Jayrock.Json
Imports Jayrock.Json.Conversion
Imports MediaPortal.GUI.Library
Imports MediaPortal.Dialogs
Imports System.Threading


Namespace MPClientController

    Public Class Message
        Inherits GUIWindow

        ''' <summary>
        ''' Dialog box header
        ''' </summary>
        ''' <remarks></remarks>
        Private _heading As String = String.Empty
        Public Property heading() As String
            Get
                Return _heading
            End Get
            Set(ByVal value As String)
                _heading = value
            End Set
        End Property

        ''' <summary>
        ''' Dialog box message
        ''' </summary>
        ''' <remarks></remarks>
        Private _message As String = String.Empty
        Public Property message() As String
            Get
                Return _message
            End Get
            Set(ByVal value As String)
                _message = value
            End Set
        End Property

        ''' <summary>
        ''' Pops up a dialog box on the MediaPortal client with a header and message
        ''' </summary>
        ''' <remarks>Must set heading and message properties first.</remarks>
        Public Function SendMessage() As String
            SyncLock Me
                Dim thread As New Thread(New ThreadStart(AddressOf Me.PopupModal))
                thread.Start()
            End SyncLock

            Dim jw As New JsonTextWriter
            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(True)
            jw.WriteEndObject()

            Return jw.ToString

        End Function

        Private Sub PopupModal()
            If (_heading = String.Empty) Or (_message = String.Empty) Then Exit Sub
            Dim dialog As GUIDialogText = CType(GUIWindowManager.GetWindow(CType(Window.WINDOW_DIALOG_TEXT, Integer)), GUIDialogText)
            dialog.Reset()
            dialog.SetHeading(_heading)
            dialog.SetText(_message)
            dialog.FontSize = 24
            dialog.DoModal(GUIWindowManager.ActiveWindow())
        End Sub

    End Class

End Namespace
