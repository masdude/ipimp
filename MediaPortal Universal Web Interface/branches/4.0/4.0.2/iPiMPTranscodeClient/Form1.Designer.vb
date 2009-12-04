<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Label1 = New System.Windows.Forms.Label
        Me.FolderBrowser = New System.Windows.Forms.FolderBrowserDialog
        Me.FileBrowser = New System.Windows.Forms.OpenFileDialog
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.RecButton = New System.Windows.Forms.Button
        Me.RecPath = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.RecordingsBrowser = New System.Windows.Forms.OpenFileDialog
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        Me.iPiMPNotify = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.GoButton = New System.Windows.Forms.Button
        Me.txtProgress = New System.Windows.Forms.RichTextBox
        Me.cbxThumbs = New System.Windows.Forms.CheckBox
        Me.cbxMP4 = New System.Windows.Forms.CheckBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.numVideo = New System.Windows.Forms.NumericUpDown
        Me.numAudio = New System.Windows.Forms.NumericUpDown
        Me.Label6 = New System.Windows.Forms.Label
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.numVideo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numAudio, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoEllipsis = True
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(449, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "iPiMP transcode client allows you to convert previously recorded TV programs to i" & _
            "PiMP MP4   " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "format with thumbnail so that they can be viewed via the iPiMP appl" & _
            "ication."
        '
        'FolderBrowser
        '
        Me.FolderBrowser.SelectedPath = "C:\"
        '
        'FileBrowser
        '
        Me.FileBrowser.FileName = "ffmpeg.exe"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.RecButton)
        Me.GroupBox2.Controls.Add(Me.RecPath)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Location = New System.Drawing.Point(10, 38)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(451, 42)
        Me.GroupBox2.TabIndex = 7
        Me.GroupBox2.TabStop = False
        '
        'RecButton
        '
        Me.RecButton.Location = New System.Drawing.Point(367, 11)
        Me.RecButton.Name = "RecButton"
        Me.RecButton.Size = New System.Drawing.Size(75, 23)
        Me.RecButton.TabIndex = 2
        Me.RecButton.Text = "Browse"
        Me.RecButton.UseVisualStyleBackColor = True
        '
        'RecPath
        '
        Me.RecPath.Location = New System.Drawing.Point(130, 13)
        Me.RecPath.Multiline = True
        Me.RecPath.Name = "RecPath"
        Me.RecPath.Size = New System.Drawing.Size(231, 20)
        Me.RecPath.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 16)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(95, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Select recording(s)"
        '
        'RecordingsBrowser
        '
        Me.RecordingsBrowser.Multiselect = True
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = True
        '
        'iPiMPNotify
        '
        Me.iPiMPNotify.BalloonTipText = "iPiMP transcode client allows you to convert previously recorded TV programs to i" & _
            "PiMP MP4 format with thumbnail so that they can be viewed via the iPiMP applicat" & _
            "ion."
        Me.iPiMPNotify.BalloonTipTitle = "iPiMP"
        Me.iPiMPNotify.Icon = CType(resources.GetObject("iPiMPNotify.Icon"), System.Drawing.Icon)
        Me.iPiMPNotify.Text = "iPiMP Transcode client"
        '
        'GoButton
        '
        Me.GoButton.BackColor = System.Drawing.SystemColors.Control
        Me.GoButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GoButton.Location = New System.Drawing.Point(379, 179)
        Me.GoButton.Name = "GoButton"
        Me.GoButton.Size = New System.Drawing.Size(75, 23)
        Me.GoButton.TabIndex = 8
        Me.GoButton.Text = "Go Go Go"
        Me.GoButton.UseVisualStyleBackColor = False
        '
        'txtProgress
        '
        Me.txtProgress.BackColor = System.Drawing.SystemColors.Control
        Me.txtProgress.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtProgress.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtProgress.Location = New System.Drawing.Point(12, 134)
        Me.txtProgress.Name = "txtProgress"
        Me.txtProgress.ReadOnly = True
        Me.txtProgress.Size = New System.Drawing.Size(361, 68)
        Me.txtProgress.TabIndex = 9
        Me.txtProgress.Text = ""
        '
        'cbxThumbs
        '
        Me.cbxThumbs.AutoSize = True
        Me.cbxThumbs.Location = New System.Drawing.Point(390, 134)
        Me.cbxThumbs.Name = "cbxThumbs"
        Me.cbxThumbs.Size = New System.Drawing.Size(64, 17)
        Me.cbxThumbs.TabIndex = 10
        Me.cbxThumbs.Text = "Thumbs"
        Me.cbxThumbs.UseVisualStyleBackColor = True
        '
        'cbxMP4
        '
        Me.cbxMP4.AutoSize = True
        Me.cbxMP4.Location = New System.Drawing.Point(390, 157)
        Me.cbxMP4.Name = "cbxMP4"
        Me.cbxMP4.Size = New System.Drawing.Size(53, 17)
        Me.cbxMP4.TabIndex = 11
        Me.cbxMP4.Text = "MP4s"
        Me.cbxMP4.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.numAudio)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.numVideo)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Location = New System.Drawing.Point(10, 86)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(451, 42)
        Me.GroupBox3.TabIndex = 8
        Me.GroupBox3.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 16)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(66, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Video bitrate"
        '
        'numVideo
        '
        Me.numVideo.Increment = New Decimal(New Integer() {32, 0, 0, 0})
        Me.numVideo.Location = New System.Drawing.Point(100, 14)
        Me.numVideo.Maximum = New Decimal(New Integer() {1024, 0, 0, 0})
        Me.numVideo.Minimum = New Decimal(New Integer() {64, 0, 0, 0})
        Me.numVideo.Name = "numVideo"
        Me.numVideo.Size = New System.Drawing.Size(88, 20)
        Me.numVideo.TabIndex = 5
        Me.numVideo.Value = New Decimal(New Integer() {256, 0, 0, 0})
        '
        'numAudio
        '
        Me.numAudio.Increment = New Decimal(New Integer() {32, 0, 0, 0})
        Me.numAudio.Location = New System.Drawing.Point(323, 14)
        Me.numAudio.Maximum = New Decimal(New Integer() {256, 0, 0, 0})
        Me.numAudio.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
        Me.numAudio.Name = "numAudio"
        Me.numAudio.Size = New System.Drawing.Size(88, 20)
        Me.numAudio.TabIndex = 7
        Me.numAudio.Value = New Decimal(New Integer() {128, 0, 0, 0})
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(243, 16)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(66, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Audio bitrate"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(476, 214)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.cbxMP4)
        Me.Controls.Add(Me.cbxThumbs)
        Me.Controls.Add(Me.txtProgress)
        Me.Controls.Add(Me.GoButton)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(492, 250)
        Me.MinimumSize = New System.Drawing.Size(492, 250)
        Me.Name = "Form1"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "iPiMP Transcode client"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.numVideo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numAudio, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents FolderBrowser As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents FileBrowser As System.Windows.Forms.OpenFileDialog
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents RecButton As System.Windows.Forms.Button
    Friend WithEvents RecPath As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents RecordingsBrowser As System.Windows.Forms.OpenFileDialog
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents iPiMPNotify As System.Windows.Forms.NotifyIcon
    Friend WithEvents GoButton As System.Windows.Forms.Button
    Friend WithEvents txtProgress As System.Windows.Forms.RichTextBox
    Friend WithEvents cbxThumbs As System.Windows.Forms.CheckBox
    Friend WithEvents cbxMP4 As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents numVideo As System.Windows.Forms.NumericUpDown
    Friend WithEvents numAudio As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label6 As System.Windows.Forms.Label

End Class
