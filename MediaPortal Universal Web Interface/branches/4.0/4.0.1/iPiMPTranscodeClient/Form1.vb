Imports System.IO
Imports TvDatabase
Imports System.Collections.Specialized
Imports System.Configuration

Public Class Form1

    Dim MP4Path, FFPath As String

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim layer As New TvBusinessLayer
        Dim setting As Setting

        Dim appSettings As NameValueCollection = ConfigurationManager.AppSettings
        appSettings.Set("GentleConfigFile", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\Team MediaPortal\MediaPortal TV Server\gentle.config")

        setting = layer.GetSetting("iPiMPTranscodeToMP4_SavePath")
        MP4Path = setting.Value.ToString

        setting = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegPath")
        FFPath = setting.Value.ToString

    End Sub

    Private Sub RecButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RecButton.Click
        RecordingsBrowser.ShowDialog()
        RecPath.Lines = RecordingsBrowser.FileNames
        txtProgress.Text = ""
    End Sub

    Private Sub Minimise(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            Me.Hide()
            iPiMPNotify.Visible = True
            iPiMPNotify.ShowBalloonTip(5)
        End If
    End Sub

    Private Sub iPiMPNotify_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles iPiMPNotify.MouseDoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
        iPiMPNotify.Visible = False
    End Sub

    Private Sub GoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoButton.Click

        If MP4Path.Length = 0 Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "No MP4 path provided."
            Exit Sub
        End If

        If Not Directory.Exists(MP4Path) Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "Invalid MP4 path."
            Exit Sub
        End If

        If FFPath = "" Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "No FFmpeg path provided."
            Exit Sub
        End If

        If Not File.Exists(FFPath) Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "Invalid FFmpeg path."
            Exit Sub
        End If

        If Path.GetFileName(FFPath).ToLower <> "ffmpeg.exe" Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "Invalid FFmpeg file."
            Exit Sub
        End If

        If RecPath.Text = "" Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "No recordings selected."
            Exit Sub
        End If

        If (cbxThumbs.Checked = False) And (cbxMP4.Checked = False) Then
            txtProgress.ForeColor = Color.Red
            txtProgress.Text = "Nothing to do."
            Exit Sub
        End If

        txtProgress.ForeColor = Color.Black
        txtProgress.Text = "Progress..." & vbCrLf
        Me.WindowState = FormWindowState.Minimized

        Dim recFile As String
        Dim mp4File As String
        Dim pngFile As String
        Dim process As Process

        For Each recFile In RecPath.Lines
            If File.Exists(recFile) Then
                Try
                    mp4File = MP4Path & "\" & Path.GetFileNameWithoutExtension(recFile) & ".mp4"
                    pngFile = MP4Path & "\" & Path.GetFileNameWithoutExtension(recFile) & ".png"

                    If cbxThumbs.Checked = True Then
                        txtProgress.Text += "Thumbnail " & Path.GetFileNameWithoutExtension(recFile) & vbCrLf
                        iPiMPNotify.BalloonTipText = "Generating thumbnail for " & Path.GetFileNameWithoutExtension(recFile)
                        If Me.WindowState = FormWindowState.Minimized Then iPiMPNotify.ShowBalloonTip(5)
                        process = New Process
                        process.StartInfo.FileName = "" & FFPath & ""
                        process.StartInfo.Arguments = "-i """ & recFile & """ -ss 330 -vcodec png -vframes 1 -an -f rawvideo -s 88x50 -y """ & pngFile & ""
                        process.StartInfo.WorkingDirectory = MP4Path
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                        Application.DoEvents()
                        process.Start()
                        Do Until process.HasExited
                            Application.DoEvents()
                            System.Threading.Thread.Sleep(250)
                        Loop
                    End If

                    If cbxMP4.Checked = True Then
                        txtProgress.Text += "Transcode " & Path.GetFileNameWithoutExtension(recFile) & vbCrLf
                        iPiMPNotify.BalloonTipText = "Transcoding " & Path.GetFileNameWithoutExtension(recFile)
                        If Me.WindowState = FormWindowState.Minimized Then iPiMPNotify.ShowBalloonTip(5)
                        process = New Process
                        process.StartInfo.FileName = "" & FFPath & ""
                        process.StartInfo.Arguments = "-i """ & recFile & """ -threads 4 -re -vcodec libx264 -s 480x272 -flags +loop -cmp +chroma -deblockalpha 0 -deblockbeta 0 -crf 24 -bt " & numVideo.Value.ToString & "k -refs 1 -coder 0 -me_method umh -me_range 16 -subq 5 -partitions +parti4x4+parti8x8+partp8x8 -g 250 -keyint_min 25 -level 30 -qmin 10 -qmax 51 -trellis 2 -sc_threshold 40 -i_qfactor 0.71 -acodec libfaac -ab " & numAudio.Value.ToString & "k -ar 48000 -ac 2 -y """ & mp4File & ""
                        process.StartInfo.WorkingDirectory = MP4Path
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                        Application.DoEvents()
                        process.Start()
                        process.WaitForExit()
                        Do Until process.HasExited
                            Application.DoEvents()
                            System.Threading.Thread.Sleep(250)
                        Loop
                    End If
                Catch ex As Exception
                    txtProgress.ForeColor = Color.Red
                    txtProgress.Text = "Error processing " & Path.GetFileNameWithoutExtension(recFile) & vbCrLf
                    txtProgress.ForeColor = Color.Black
                End Try
            End If
        Next

        txtProgress.Text += "...Finished"

        If Me.WindowState = FormWindowState.Minimized Then Me.WindowState = FormWindowState.Maximized

    End Sub

End Class
