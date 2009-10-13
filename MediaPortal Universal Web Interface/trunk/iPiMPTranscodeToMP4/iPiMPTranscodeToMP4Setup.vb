Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Imports TvLibrary.Log
Imports TvEngine
Imports TvControl
Imports TvDatabase

Namespace SetupTv.Sections

    Partial Public Class iPiMPTranscodeToMP4Setup
        Inherits Global.SetupTv.SectionSettings

        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents DelYes As System.Windows.Forms.RadioButton
        Friend WithEvents DelNo As System.Windows.Forms.RadioButton
        Friend WithEvents MP4Group As System.Windows.Forms.GroupBox
        Friend WithEvents SaveBtn As System.Windows.Forms.Button
        Friend WithEvents MP4Path As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents FolderBrowser As System.Windows.Forms.FolderBrowserDialog
        Friend WithEvents FileBrowser As System.Windows.Forms.OpenFileDialog
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents FFButton As System.Windows.Forms.Button
        Friend WithEvents FFPath As System.Windows.Forms.TextBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents TrNo As System.Windows.Forms.RadioButton
        Friend WithEvents TrYes As System.Windows.Forms.RadioButton
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents cbStartTime As System.Windows.Forms.ComboBox
        Friend WithEvents lblStartTime As System.Windows.Forms.Label
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents numVideo As System.Windows.Forms.NumericUpDown
        Friend WithEvents numAudio As System.Windows.Forms.NumericUpDown
        Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
        Friend WithEvents FFParam As System.Windows.Forms.TextBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents btnFFParam As System.Windows.Forms.Button
        Friend WithEvents DelGroup As System.Windows.Forms.GroupBox

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Property Delete() As Boolean
            Get
                Return DelYes.Checked
            End Get
            Set(ByVal Value As Boolean)
                DelYes.Checked = Value
                DelNo.Checked = Not Value
            End Set
        End Property

        Public Property SavePath() As String
            Get
                Return MP4Path.Text
            End Get
            Set(ByVal Value As String)
                MP4Path.Text = Value
            End Set
        End Property

        Public Property FFmpegPath() As String
            Get
                Return FFPath.Text
            End Get
            Set(ByVal Value As String)
                FFPath.Text = Value
            End Set
        End Property


        Public Property FFmpegParam() As String
            Get
                Return FFParam.Text
            End Get
            Set(ByVal Value As String)
                FFParam.Text = Value
            End Set
        End Property

        Public Property TranscodeNow() As Boolean
            Get
                Return TrYes.Checked
            End Get
            Set(ByVal value As Boolean)
                TrYes.Checked = value
                TrNo.Checked = Not value
            End Set
        End Property

        Public Property TranscodeTime() As String
            Get
                Return cbStartTime.Text
            End Get
            Set(ByVal value As String)
                cbStartTime.Text = value
            End Set
        End Property

        Public Overrides Sub OnSectionActivated()
            MyBase.OnSectionActivated()
            Log.Info("iPiMPTranscodeToMP4: Configuration activated")
            LoadSettings()

            Dim layer As New TvBusinessLayer

            Dim testBool As Boolean = layer.GetSetting("iPiMPTranscodeToMP4_Delete", "true").Value
            If testBool Then
                DelYes.Checked = True
                DelNo.Checked = False
            Else
                DelYes.Checked = False
                DelNo.Checked = True
            End If

            Dim testTranscode As Boolean = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeNow", "true").Value
            If testTranscode Then
                TrYes.Checked = True
                TrNo.Checked = False
                lblStartTime.Visible = False
                cbStartTime.Visible = False
            Else
                TrYes.Checked = False
                TrNo.Checked = True
                lblStartTime.Visible = True
                cbStartTime.Visible = True
            End If

            MP4Path.Text = layer.GetSetting("iPiMPTranscodeToMP4_SavePath").Value

            FFPath.Text = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegPath").Value

            FFParam.Text = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegParam").Value

            cbStartTime.Text = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeTime").Value

            numVideo.Value = layer.GetSetting("iPiMPTranscodeToMP4_VideoBitrate").Value

            numAudio.Value = layer.GetSetting("iPiMPTranscodeToMP4_AudioBitrate").Value

            MyBase.OnSectionActivated()

        End Sub

        Public Overrides Sub OnSectionDeActivated()
            MyBase.OnSectionDeActivated()
            Log.Info("iPiMPTranscodeToMP4: Configuration deactivated")

            Dim layer As New TvBusinessLayer
            Dim setting As Setting
            Dim msg As String

            setting = layer.GetSetting("iPiMPTranscodeToMP4_Delete", "true")
            If DelYes.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeNow", "true")
            If TrYes.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            If Not Directory.Exists(MP4Path.Text) Then
                msg = "Invalid MP4 path."
                Log.Error("iPiMPTranscodeToMP4 - OnSectionDeActivated: {0}", msg)
            End If
            setting = layer.GetSetting("iPiMPTranscodeToMP4_SavePath")
            setting.Value = MP4Path.Text
            setting.Persist()

            If Not File.Exists(FFPath.Text) Then
                msg = "Invalid ffmpeg path."
                Log.Error("iPiMPTranscodeToMP4 - OnSectionDeActivated: {0}", msg)
            End If
            setting = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegPath")
            setting.Value = FFPath.Text
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_FFmpegParam")
            setting.Value = FFParam.Text
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeTime")
            setting.Value = cbStartTime.Text
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_VideoBitrate")
            setting.Value = numVideo.Value
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_AudioBitrate")
            setting.Value = numAudio.Value
            setting.Persist()

            MyBase.OnSectionDeActivated()

        End Sub



        Private Sub InitializeComponent()
            Me.DelGroup = New System.Windows.Forms.GroupBox
            Me.DelNo = New System.Windows.Forms.RadioButton
            Me.DelYes = New System.Windows.Forms.RadioButton
            Me.Label1 = New System.Windows.Forms.Label
            Me.MP4Group = New System.Windows.Forms.GroupBox
            Me.SaveBtn = New System.Windows.Forms.Button
            Me.MP4Path = New System.Windows.Forms.TextBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.FolderBrowser = New System.Windows.Forms.FolderBrowserDialog
            Me.FileBrowser = New System.Windows.Forms.OpenFileDialog
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.FFButton = New System.Windows.Forms.Button
            Me.FFPath = New System.Windows.Forms.TextBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.cbStartTime = New System.Windows.Forms.ComboBox
            Me.lblStartTime = New System.Windows.Forms.Label
            Me.TrNo = New System.Windows.Forms.RadioButton
            Me.TrYes = New System.Windows.Forms.RadioButton
            Me.Label4 = New System.Windows.Forms.Label
            Me.GroupBox3 = New System.Windows.Forms.GroupBox
            Me.numAudio = New System.Windows.Forms.NumericUpDown
            Me.numVideo = New System.Windows.Forms.NumericUpDown
            Me.Label5 = New System.Windows.Forms.Label
            Me.Label6 = New System.Windows.Forms.Label
            Me.GroupBox4 = New System.Windows.Forms.GroupBox
            Me.btnFFParam = New System.Windows.Forms.Button
            Me.FFParam = New System.Windows.Forms.TextBox
            Me.Label7 = New System.Windows.Forms.Label
            Me.DelGroup.SuspendLayout()
            Me.MP4Group.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            CType(Me.numAudio, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numVideo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox4.SuspendLayout()
            Me.SuspendLayout()
            '
            'DelGroup
            '
            Me.DelGroup.Controls.Add(Me.DelNo)
            Me.DelGroup.Controls.Add(Me.DelYes)
            Me.DelGroup.Controls.Add(Me.Label1)
            Me.DelGroup.Location = New System.Drawing.Point(15, 16)
            Me.DelGroup.Name = "DelGroup"
            Me.DelGroup.Size = New System.Drawing.Size(451, 42)
            Me.DelGroup.TabIndex = 0
            Me.DelGroup.TabStop = False
            '
            'DelNo
            '
            Me.DelNo.AutoSize = True
            Me.DelNo.Location = New System.Drawing.Point(179, 14)
            Me.DelNo.Name = "DelNo"
            Me.DelNo.Size = New System.Drawing.Size(39, 17)
            Me.DelNo.TabIndex = 2
            Me.DelNo.Text = "No"
            Me.DelNo.UseVisualStyleBackColor = True
            '
            'DelYes
            '
            Me.DelYes.AutoSize = True
            Me.DelYes.Location = New System.Drawing.Point(130, 14)
            Me.DelYes.Name = "DelYes"
            Me.DelYes.Size = New System.Drawing.Size(43, 17)
            Me.DelYes.TabIndex = 1
            Me.DelYes.Text = "Yes"
            Me.DelYes.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(6, 16)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(118, 13)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Delete with recordings?"
            '
            'MP4Group
            '
            Me.MP4Group.Controls.Add(Me.SaveBtn)
            Me.MP4Group.Controls.Add(Me.MP4Path)
            Me.MP4Group.Controls.Add(Me.Label3)
            Me.MP4Group.Location = New System.Drawing.Point(15, 64)
            Me.MP4Group.Name = "MP4Group"
            Me.MP4Group.Size = New System.Drawing.Size(451, 42)
            Me.MP4Group.TabIndex = 4
            Me.MP4Group.TabStop = False
            '
            'SaveBtn
            '
            Me.SaveBtn.Location = New System.Drawing.Point(367, 11)
            Me.SaveBtn.Name = "SaveBtn"
            Me.SaveBtn.Size = New System.Drawing.Size(75, 23)
            Me.SaveBtn.TabIndex = 2
            Me.SaveBtn.Text = "Browse"
            Me.SaveBtn.UseVisualStyleBackColor = True
            '
            'MP4Path
            '
            Me.MP4Path.Location = New System.Drawing.Point(130, 13)
            Me.MP4Path.Name = "MP4Path"
            Me.MP4Path.Size = New System.Drawing.Size(231, 20)
            Me.MP4Path.TabIndex = 1
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(6, 16)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(86, 13)
            Me.Label3.TabIndex = 0
            Me.Label3.Text = "Select MP4 path"
            '
            'FolderBrowser
            '
            Me.FolderBrowser.SelectedPath = "C:\"
            '
            'FileBrowser
            '
            Me.FileBrowser.FileName = "ffmpeg.exe"
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.FFButton)
            Me.GroupBox1.Controls.Add(Me.FFPath)
            Me.GroupBox1.Controls.Add(Me.Label2)
            Me.GroupBox1.Location = New System.Drawing.Point(15, 112)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(451, 42)
            Me.GroupBox1.TabIndex = 5
            Me.GroupBox1.TabStop = False
            '
            'FFButton
            '
            Me.FFButton.Location = New System.Drawing.Point(367, 11)
            Me.FFButton.Name = "FFButton"
            Me.FFButton.Size = New System.Drawing.Size(75, 23)
            Me.FFButton.TabIndex = 2
            Me.FFButton.Text = "Browse"
            Me.FFButton.UseVisualStyleBackColor = True
            '
            'FFPath
            '
            Me.FFPath.Location = New System.Drawing.Point(130, 13)
            Me.FFPath.Name = "FFPath"
            Me.FFPath.Size = New System.Drawing.Size(231, 20)
            Me.FFPath.TabIndex = 1
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(6, 16)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(96, 13)
            Me.Label2.TabIndex = 0
            Me.Label2.Text = "Select ffmpeg path"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.cbStartTime)
            Me.GroupBox2.Controls.Add(Me.lblStartTime)
            Me.GroupBox2.Controls.Add(Me.TrNo)
            Me.GroupBox2.Controls.Add(Me.TrYes)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Location = New System.Drawing.Point(15, 208)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(451, 42)
            Me.GroupBox2.TabIndex = 3
            Me.GroupBox2.TabStop = False
            '
            'cbStartTime
            '
            Me.cbStartTime.FormattingEnabled = True
            Me.cbStartTime.Items.AddRange(New Object() {"00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00", "never"})
            Me.cbStartTime.Location = New System.Drawing.Point(362, 13)
            Me.cbStartTime.Name = "cbStartTime"
            Me.cbStartTime.Size = New System.Drawing.Size(80, 21)
            Me.cbStartTime.TabIndex = 4
            '
            'lblStartTime
            '
            Me.lblStartTime.AutoSize = True
            Me.lblStartTime.Location = New System.Drawing.Point(274, 16)
            Me.lblStartTime.Name = "lblStartTime"
            Me.lblStartTime.Size = New System.Drawing.Size(82, 13)
            Me.lblStartTime.TabIndex = 3
            Me.lblStartTime.Text = "Select start time"
            '
            'TrNo
            '
            Me.TrNo.AutoSize = True
            Me.TrNo.Location = New System.Drawing.Point(179, 14)
            Me.TrNo.Name = "TrNo"
            Me.TrNo.Size = New System.Drawing.Size(39, 17)
            Me.TrNo.TabIndex = 2
            Me.TrNo.Text = "No"
            Me.TrNo.UseVisualStyleBackColor = True
            '
            'TrYes
            '
            Me.TrYes.AutoSize = True
            Me.TrYes.Checked = True
            Me.TrYes.Location = New System.Drawing.Point(130, 14)
            Me.TrYes.Name = "TrYes"
            Me.TrYes.Size = New System.Drawing.Size(43, 17)
            Me.TrYes.TabIndex = 1
            Me.TrYes.TabStop = True
            Me.TrYes.Text = "Yes"
            Me.TrYes.UseVisualStyleBackColor = True
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(6, 16)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(121, 13)
            Me.Label4.TabIndex = 0
            Me.Label4.Text = "Transcode immediately?"
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.numAudio)
            Me.GroupBox3.Controls.Add(Me.numVideo)
            Me.GroupBox3.Controls.Add(Me.Label5)
            Me.GroupBox3.Controls.Add(Me.Label6)
            Me.GroupBox3.Location = New System.Drawing.Point(15, 256)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(451, 42)
            Me.GroupBox3.TabIndex = 5
            Me.GroupBox3.TabStop = False
            '
            'numAudio
            '
            Me.numAudio.Increment = New Decimal(New Integer() {32, 0, 0, 0})
            Me.numAudio.Location = New System.Drawing.Point(354, 14)
            Me.numAudio.Maximum = New Decimal(New Integer() {256, 0, 0, 0})
            Me.numAudio.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
            Me.numAudio.Name = "numAudio"
            Me.numAudio.Size = New System.Drawing.Size(88, 20)
            Me.numAudio.TabIndex = 5
            Me.numAudio.Value = New Decimal(New Integer() {128, 0, 0, 0})
            '
            'numVideo
            '
            Me.numVideo.Increment = New Decimal(New Integer() {32, 0, 0, 0})
            Me.numVideo.Location = New System.Drawing.Point(130, 14)
            Me.numVideo.Maximum = New Decimal(New Integer() {1024, 0, 0, 0})
            Me.numVideo.Minimum = New Decimal(New Integer() {64, 0, 0, 0})
            Me.numVideo.Name = "numVideo"
            Me.numVideo.Size = New System.Drawing.Size(88, 20)
            Me.numVideo.TabIndex = 4
            Me.numVideo.Value = New Decimal(New Integer() {256, 0, 0, 0})
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(274, 16)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(66, 13)
            Me.Label5.TabIndex = 3
            Me.Label5.Text = "Audio bitrate"
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(6, 16)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(66, 13)
            Me.Label6.TabIndex = 0
            Me.Label6.Text = "Video bitrate"
            '
            'GroupBox4
            '
            Me.GroupBox4.Controls.Add(Me.btnFFParam)
            Me.GroupBox4.Controls.Add(Me.FFParam)
            Me.GroupBox4.Controls.Add(Me.Label7)
            Me.GroupBox4.Location = New System.Drawing.Point(15, 160)
            Me.GroupBox4.Name = "GroupBox4"
            Me.GroupBox4.Size = New System.Drawing.Size(451, 42)
            Me.GroupBox4.TabIndex = 6
            Me.GroupBox4.TabStop = False
            '
            'btnFFParam
            '
            Me.btnFFParam.Location = New System.Drawing.Point(367, 10)
            Me.btnFFParam.Name = "btnFFParam"
            Me.btnFFParam.Size = New System.Drawing.Size(75, 23)
            Me.btnFFParam.TabIndex = 3
            Me.btnFFParam.Text = "Default"
            Me.btnFFParam.UseVisualStyleBackColor = True
            '
            'FFParam
            '
            Me.FFParam.Location = New System.Drawing.Point(130, 13)
            Me.FFParam.Name = "FFParam"
            Me.FFParam.Size = New System.Drawing.Size(231, 20)
            Me.FFParam.TabIndex = 1
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Location = New System.Drawing.Point(6, 16)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(113, 13)
            Me.Label7.TabIndex = 0
            Me.Label7.Text = "Set ffmpeg parameters"
            '
            'iPiMPTranscodeToMP4Setup
            '
            Me.Controls.Add(Me.GroupBox4)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.MP4Group)
            Me.Controls.Add(Me.DelGroup)
            Me.Name = "iPiMPTranscodeToMP4Setup"
            Me.Size = New System.Drawing.Size(481, 322)
            Me.DelGroup.ResumeLayout(False)
            Me.DelGroup.PerformLayout()
            Me.MP4Group.ResumeLayout(False)
            Me.MP4Group.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            CType(Me.numAudio, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numVideo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox4.ResumeLayout(False)
            Me.GroupBox4.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Private Sub SaveBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveBtn.Click
            FolderBrowser.SelectedPath = "C:\"
            FolderBrowser.ShowDialog()
            MP4Path.Text = FolderBrowser.SelectedPath
        End Sub

        Private Sub FFButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FFButton.Click
            FileBrowser.ShowDialog()
            FileBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\iPiMP\Utils"
            FFPath.Text = FileBrowser.FileName
        End Sub

        Private Sub TrYes_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrYes.CheckedChanged
            If TrYes.Checked Then
                lblStartTime.Visible = False
                cbStartTime.Visible = False
            Else
                lblStartTime.Visible = True
                cbStartTime.Visible = True
            End If
        End Sub

        Private Sub TrNo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrNo.CheckedChanged
            If TrNo.Checked Then
                lblStartTime.Visible = True
                cbStartTime.Visible = True
            Else
                lblStartTime.Visible = False
                cbStartTime.Visible = False
            End If
        End Sub

        
        Private Sub btnFFParam_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFFParam.Click

            FFParam.Text = "-threads 4 -re -vcodec libx264 -s 480x272 -flags +loop -cmp +chroma -deblockalpha 0 -deblockbeta 0 -crf 24 -bt {0}k -refs 1 -coder 0 -me_method umh -me_range 16 -subq 5 -partitions +parti4x4+parti8x8+partp8x8 -g 250 -keyint_min 25 -level 30 -qmin 10 -qmax 51 -trellis 2 -sc_threshold 40 -i_qfactor 0.71 -acodec libfaac -ab {1}k -ar 48000 -ac 2 -async 2"

        End Sub

    End Class

End Namespace

