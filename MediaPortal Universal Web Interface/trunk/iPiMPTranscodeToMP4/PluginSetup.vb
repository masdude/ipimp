' 
'   Copyright (C) 2008-2010 Martin van der Boon
' 
'  This program is free software: you can redistribute it and/or modify 
'  it under the terms of the GNU General Public License as published by 
'  the Free Software Foundation, either version 3 of the License, or 
'  (at your option) any later version. 
' 
'   This program is distributed in the hope that it will be useful, 
'   but WITHOUT ANY WARRANTY; without even the implied warranty of 
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
'   GNU General Public License for more details. 
' 
'   You should have received a copy of the GNU General Public License 
'   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
' 

Imports System
Imports System.ComponentModel
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

    Partial Public Class PluginSetup
        Inherits Global.SetupTv.SectionSettings

#Region "Controls"
        Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
        Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents FFMpeg As System.Windows.Forms.RadioButton
        Friend WithEvents Handbrake As System.Windows.Forms.RadioButton
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents RichTextBox4 As System.Windows.Forms.RichTextBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
        Friend WithEvents Starttime As System.Windows.Forms.ComboBox
        Friend WithEvents lblStartTime As System.Windows.Forms.Label
        Friend WithEvents TrNo As System.Windows.Forms.RadioButton
        Friend WithEvents TrYes As System.Windows.Forms.RadioButton
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents MP4Group As System.Windows.Forms.GroupBox
        Friend WithEvents RichTextBox3 As System.Windows.Forms.RichTextBox
        Friend WithEvents Browse As System.Windows.Forms.Button
        Friend WithEvents folder As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents DelGroup As System.Windows.Forms.GroupBox
        Friend WithEvents RichTextBox2 As System.Windows.Forms.RichTextBox
        Friend WithEvents DelNo As System.Windows.Forms.RadioButton
        Friend WithEvents DelYes As System.Windows.Forms.RadioButton
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
        Friend WithEvents Link As System.Windows.Forms.LinkLabel
        Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
        Friend WithEvents Custom As System.Windows.Forms.RichTextBox
        Friend WithEvents RichTextBox8 As System.Windows.Forms.RichTextBox
        Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
        Friend WithEvents Preset As System.Windows.Forms.ComboBox
        Friend WithEvents Label12 As System.Windows.Forms.Label
        Friend WithEvents Label13 As System.Windows.Forms.Label
        Friend WithEvents GroupBox9 As System.Windows.Forms.GroupBox
        Friend WithEvents audioBitrate As System.Windows.Forms.NumericUpDown
        Friend WithEvents videoBitrate As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label16 As System.Windows.Forms.Label
        Friend WithEvents Label17 As System.Windows.Forms.Label
        Friend WithEvents numAudio As System.Windows.Forms.NumericUpDown
        Friend WithEvents numVideo As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label10 As System.Windows.Forms.Label
        Friend WithEvents Label15 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents Label11 As System.Windows.Forms.Label
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents CheckedListBox1 As System.Windows.Forms.CheckedListBox
        Friend WithEvents RichTextBox5 As System.Windows.Forms.RichTextBox
        Friend WithEvents RichTextBox6 As System.Windows.Forms.RichTextBox
#End Region

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PluginSetup))
            Me.TabControl1 = New System.Windows.Forms.TabControl
            Me.TabPage1 = New System.Windows.Forms.TabPage
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.FFMpeg = New System.Windows.Forms.RadioButton
            Me.Handbrake = New System.Windows.Forms.RadioButton
            Me.Label5 = New System.Windows.Forms.Label
            Me.RichTextBox4 = New System.Windows.Forms.RichTextBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.RichTextBox1 = New System.Windows.Forms.RichTextBox
            Me.Starttime = New System.Windows.Forms.ComboBox
            Me.lblStartTime = New System.Windows.Forms.Label
            Me.TrNo = New System.Windows.Forms.RadioButton
            Me.TrYes = New System.Windows.Forms.RadioButton
            Me.Label4 = New System.Windows.Forms.Label
            Me.MP4Group = New System.Windows.Forms.GroupBox
            Me.RichTextBox3 = New System.Windows.Forms.RichTextBox
            Me.Browse = New System.Windows.Forms.Button
            Me.folder = New System.Windows.Forms.TextBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.DelGroup = New System.Windows.Forms.GroupBox
            Me.RichTextBox2 = New System.Windows.Forms.RichTextBox
            Me.DelNo = New System.Windows.Forms.RadioButton
            Me.DelYes = New System.Windows.Forms.RadioButton
            Me.Label1 = New System.Windows.Forms.Label
            Me.TabPage3 = New System.Windows.Forms.TabPage
            Me.PictureBox1 = New System.Windows.Forms.PictureBox
            Me.Label11 = New System.Windows.Forms.Label
            Me.GroupBox9 = New System.Windows.Forms.GroupBox
            Me.Label6 = New System.Windows.Forms.Label
            Me.audioBitrate = New System.Windows.Forms.NumericUpDown
            Me.videoBitrate = New System.Windows.Forms.NumericUpDown
            Me.Label16 = New System.Windows.Forms.Label
            Me.Label17 = New System.Windows.Forms.Label
            Me.Link = New System.Windows.Forms.LinkLabel
            Me.GroupBox8 = New System.Windows.Forms.GroupBox
            Me.RichTextBox6 = New System.Windows.Forms.RichTextBox
            Me.Custom = New System.Windows.Forms.RichTextBox
            Me.RichTextBox8 = New System.Windows.Forms.RichTextBox
            Me.GroupBox7 = New System.Windows.Forms.GroupBox
            Me.Preset = New System.Windows.Forms.ComboBox
            Me.Label12 = New System.Windows.Forms.Label
            Me.Label13 = New System.Windows.Forms.Label
            Me.TabPage2 = New System.Windows.Forms.TabPage
            Me.GroupBox3 = New System.Windows.Forms.GroupBox
            Me.RichTextBox5 = New System.Windows.Forms.RichTextBox
            Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox
            Me.numAudio = New System.Windows.Forms.NumericUpDown
            Me.numVideo = New System.Windows.Forms.NumericUpDown
            Me.Label10 = New System.Windows.Forms.Label
            Me.Label15 = New System.Windows.Forms.Label
            Me.TabControl1.SuspendLayout()
            Me.TabPage1.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.MP4Group.SuspendLayout()
            Me.DelGroup.SuspendLayout()
            Me.TabPage3.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox9.SuspendLayout()
            CType(Me.audioBitrate, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.videoBitrate, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.GroupBox8.SuspendLayout()
            Me.GroupBox7.SuspendLayout()
            Me.TabPage2.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            CType(Me.numAudio, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numVideo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TabControl1
            '
            Me.TabControl1.Controls.Add(Me.TabPage1)
            Me.TabControl1.Controls.Add(Me.TabPage3)
            Me.TabControl1.Controls.Add(Me.TabPage2)
            Me.TabControl1.Location = New System.Drawing.Point(12, 13)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            Me.TabControl1.ShowToolTips = True
            Me.TabControl1.Size = New System.Drawing.Size(462, 369)
            Me.TabControl1.TabIndex = 1
            '
            'TabPage1
            '
            Me.TabPage1.Controls.Add(Me.GroupBox1)
            Me.TabPage1.Controls.Add(Me.Label2)
            Me.TabPage1.Controls.Add(Me.GroupBox2)
            Me.TabPage1.Controls.Add(Me.MP4Group)
            Me.TabPage1.Controls.Add(Me.DelGroup)
            Me.TabPage1.Location = New System.Drawing.Point(4, 22)
            Me.TabPage1.Name = "TabPage1"
            Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage1.Size = New System.Drawing.Size(454, 343)
            Me.TabPage1.TabIndex = 0
            Me.TabPage1.Text = "Transcode settings"
            Me.TabPage1.UseVisualStyleBackColor = True
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.FFMpeg)
            Me.GroupBox1.Controls.Add(Me.Handbrake)
            Me.GroupBox1.Controls.Add(Me.Label5)
            Me.GroupBox1.Controls.Add(Me.RichTextBox4)
            Me.GroupBox1.Location = New System.Drawing.Point(6, 265)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(442, 67)
            Me.GroupBox1.TabIndex = 9
            Me.GroupBox1.TabStop = False
            '
            'FFMpeg
            '
            Me.FFMpeg.AutoSize = True
            Me.FFMpeg.Location = New System.Drawing.Point(130, 40)
            Me.FFMpeg.Name = "FFMpeg"
            Me.FFMpeg.Size = New System.Drawing.Size(64, 17)
            Me.FFMpeg.TabIndex = 13
            Me.FFMpeg.Text = "FFMpeg"
            Me.FFMpeg.UseVisualStyleBackColor = True
            '
            'Handbrake
            '
            Me.Handbrake.AutoSize = True
            Me.Handbrake.Location = New System.Drawing.Point(200, 40)
            Me.Handbrake.Name = "Handbrake"
            Me.Handbrake.Size = New System.Drawing.Size(79, 17)
            Me.Handbrake.TabIndex = 11
            Me.Handbrake.Text = "HandBrake"
            Me.Handbrake.UseVisualStyleBackColor = True
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(6, 42)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(90, 13)
            Me.Label5.TabIndex = 10
            Me.Label5.Text = "Select transcoder"
            '
            'RichTextBox4
            '
            Me.RichTextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox4.Location = New System.Drawing.Point(9, 19)
            Me.RichTextBox4.Name = "RichTextBox4"
            Me.RichTextBox4.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
            Me.RichTextBox4.Size = New System.Drawing.Size(409, 20)
            Me.RichTextBox4.TabIndex = 12
            Me.RichTextBox4.Text = "HandBrake and FFMpeg are both supported as transcoding utilities."
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(6, 7)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(0, 13)
            Me.Label2.TabIndex = 8
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.RichTextBox1)
            Me.GroupBox2.Controls.Add(Me.Starttime)
            Me.GroupBox2.Controls.Add(Me.lblStartTime)
            Me.GroupBox2.Controls.Add(Me.TrNo)
            Me.GroupBox2.Controls.Add(Me.TrYes)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Location = New System.Drawing.Point(6, 162)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(442, 97)
            Me.GroupBox2.TabIndex = 6
            Me.GroupBox2.TabStop = False
            '
            'RichTextBox1
            '
            Me.RichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox1.Location = New System.Drawing.Point(6, 20)
            Me.RichTextBox1.Name = "RichTextBox1"
            Me.RichTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
            Me.RichTextBox1.Size = New System.Drawing.Size(412, 44)
            Me.RichTextBox1.TabIndex = 9
            Me.RichTextBox1.Text = resources.GetString("RichTextBox1.Text")
            '
            'Starttime
            '
            Me.Starttime.FormattingEnabled = True
            Me.Starttime.Items.AddRange(New Object() {"00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00", "never"})
            Me.Starttime.Location = New System.Drawing.Point(362, 69)
            Me.Starttime.Name = "Starttime"
            Me.Starttime.Size = New System.Drawing.Size(71, 21)
            Me.Starttime.TabIndex = 4
            '
            'lblStartTime
            '
            Me.lblStartTime.AutoSize = True
            Me.lblStartTime.Location = New System.Drawing.Point(279, 72)
            Me.lblStartTime.Name = "lblStartTime"
            Me.lblStartTime.Size = New System.Drawing.Size(82, 13)
            Me.lblStartTime.TabIndex = 3
            Me.lblStartTime.Text = "Select start time"
            '
            'TrNo
            '
            Me.TrNo.AutoSize = True
            Me.TrNo.Location = New System.Drawing.Point(200, 70)
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
            Me.TrYes.Location = New System.Drawing.Point(130, 70)
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
            Me.Label4.Location = New System.Drawing.Point(6, 72)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(121, 13)
            Me.Label4.TabIndex = 0
            Me.Label4.Text = "Transcode immediately?"
            '
            'MP4Group
            '
            Me.MP4Group.Controls.Add(Me.RichTextBox3)
            Me.MP4Group.Controls.Add(Me.Browse)
            Me.MP4Group.Controls.Add(Me.folder)
            Me.MP4Group.Controls.Add(Me.Label3)
            Me.MP4Group.Location = New System.Drawing.Point(6, 76)
            Me.MP4Group.Name = "MP4Group"
            Me.MP4Group.Size = New System.Drawing.Size(442, 80)
            Me.MP4Group.TabIndex = 7
            Me.MP4Group.TabStop = False
            '
            'RichTextBox3
            '
            Me.RichTextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox3.Location = New System.Drawing.Point(9, 19)
            Me.RichTextBox3.Name = "RichTextBox3"
            Me.RichTextBox3.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
            Me.RichTextBox3.Size = New System.Drawing.Size(409, 26)
            Me.RichTextBox3.TabIndex = 11
            Me.RichTextBox3.Text = "Transcoded files are stored in a single folder to allow them to be streamed from " & _
                "the iPiMP web application."
            '
            'Browse
            '
            Me.Browse.Location = New System.Drawing.Point(367, 49)
            Me.Browse.Name = "Browse"
            Me.Browse.Size = New System.Drawing.Size(66, 23)
            Me.Browse.TabIndex = 2
            Me.Browse.Text = "Browse"
            Me.Browse.UseVisualStyleBackColor = True
            '
            'folder
            '
            Me.folder.Location = New System.Drawing.Point(130, 51)
            Me.folder.Name = "folder"
            Me.folder.Size = New System.Drawing.Size(231, 20)
            Me.folder.TabIndex = 1
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(6, 54)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(66, 13)
            Me.Label3.TabIndex = 0
            Me.Label3.Text = "Select folder"
            '
            'DelGroup
            '
            Me.DelGroup.Controls.Add(Me.RichTextBox2)
            Me.DelGroup.Controls.Add(Me.DelNo)
            Me.DelGroup.Controls.Add(Me.DelYes)
            Me.DelGroup.Controls.Add(Me.Label1)
            Me.DelGroup.Location = New System.Drawing.Point(6, 3)
            Me.DelGroup.Name = "DelGroup"
            Me.DelGroup.Size = New System.Drawing.Size(442, 67)
            Me.DelGroup.TabIndex = 5
            Me.DelGroup.TabStop = False
            '
            'RichTextBox2
            '
            Me.RichTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox2.Location = New System.Drawing.Point(9, 14)
            Me.RichTextBox2.Name = "RichTextBox2"
            Me.RichTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
            Me.RichTextBox2.Size = New System.Drawing.Size(392, 17)
            Me.RichTextBox2.TabIndex = 10
            Me.RichTextBox2.Text = "Transcoded files can be deleted when the original recording is deleted."
            '
            'DelNo
            '
            Me.DelNo.AutoSize = True
            Me.DelNo.Location = New System.Drawing.Point(200, 37)
            Me.DelNo.Name = "DelNo"
            Me.DelNo.Size = New System.Drawing.Size(39, 17)
            Me.DelNo.TabIndex = 2
            Me.DelNo.Text = "No"
            Me.DelNo.UseVisualStyleBackColor = True
            '
            'DelYes
            '
            Me.DelYes.AutoSize = True
            Me.DelYes.Location = New System.Drawing.Point(130, 37)
            Me.DelYes.Name = "DelYes"
            Me.DelYes.Size = New System.Drawing.Size(43, 17)
            Me.DelYes.TabIndex = 1
            Me.DelYes.Text = "Yes"
            Me.DelYes.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(6, 39)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(118, 13)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Delete with recordings?"
            '
            'TabPage3
            '
            Me.TabPage3.Controls.Add(Me.PictureBox1)
            Me.TabPage3.Controls.Add(Me.Label11)
            Me.TabPage3.Controls.Add(Me.GroupBox9)
            Me.TabPage3.Controls.Add(Me.Link)
            Me.TabPage3.Controls.Add(Me.GroupBox8)
            Me.TabPage3.Controls.Add(Me.GroupBox7)
            Me.TabPage3.Location = New System.Drawing.Point(4, 22)
            Me.TabPage3.Name = "TabPage3"
            Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage3.Size = New System.Drawing.Size(454, 343)
            Me.TabPage3.TabIndex = 2
            Me.TabPage3.Text = "Transcoder settings"
            Me.TabPage3.UseVisualStyleBackColor = True
            '
            'PictureBox1
            '
            Me.PictureBox1.Location = New System.Drawing.Point(6, 4)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(50, 50)
            Me.PictureBox1.TabIndex = 8
            Me.PictureBox1.TabStop = False
            '
            'Label11
            '
            Me.Label11.AutoSize = True
            Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 28.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label11.Location = New System.Drawing.Point(62, 10)
            Me.Label11.Name = "Label11"
            Me.Label11.Size = New System.Drawing.Size(162, 44)
            Me.Label11.TabIndex = 6
            Me.Label11.Text = "Label11"
            '
            'GroupBox9
            '
            Me.GroupBox9.Controls.Add(Me.Label6)
            Me.GroupBox9.Controls.Add(Me.audioBitrate)
            Me.GroupBox9.Controls.Add(Me.videoBitrate)
            Me.GroupBox9.Controls.Add(Me.Label16)
            Me.GroupBox9.Controls.Add(Me.Label17)
            Me.GroupBox9.Location = New System.Drawing.Point(6, 248)
            Me.GroupBox9.Name = "GroupBox9"
            Me.GroupBox9.Size = New System.Drawing.Size(442, 64)
            Me.GroupBox9.TabIndex = 7
            Me.GroupBox9.TabStop = False
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(6, 16)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(375, 13)
            Me.Label6.TabIndex = 4
            Me.Label6.Text = "You can amend the video and audio bitrates used by the libx264-iPiMP preset."
            '
            'audioBitrate
            '
            Me.audioBitrate.Increment = New Decimal(New Integer() {32, 0, 0, 0})
            Me.audioBitrate.Location = New System.Drawing.Point(343, 36)
            Me.audioBitrate.Maximum = New Decimal(New Integer() {256, 0, 0, 0})
            Me.audioBitrate.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
            Me.audioBitrate.Name = "audioBitrate"
            Me.audioBitrate.Size = New System.Drawing.Size(88, 20)
            Me.audioBitrate.TabIndex = 5
            Me.audioBitrate.Value = New Decimal(New Integer() {128, 0, 0, 0})
            '
            'videoBitrate
            '
            Me.videoBitrate.Increment = New Decimal(New Integer() {32, 0, 0, 0})
            Me.videoBitrate.Location = New System.Drawing.Point(130, 36)
            Me.videoBitrate.Maximum = New Decimal(New Integer() {1024, 0, 0, 0})
            Me.videoBitrate.Minimum = New Decimal(New Integer() {64, 0, 0, 0})
            Me.videoBitrate.Name = "videoBitrate"
            Me.videoBitrate.Size = New System.Drawing.Size(88, 20)
            Me.videoBitrate.TabIndex = 4
            Me.videoBitrate.Value = New Decimal(New Integer() {256, 0, 0, 0})
            '
            'Label16
            '
            Me.Label16.AutoSize = True
            Me.Label16.Location = New System.Drawing.Point(259, 38)
            Me.Label16.Name = "Label16"
            Me.Label16.Size = New System.Drawing.Size(66, 13)
            Me.Label16.TabIndex = 3
            Me.Label16.Text = "Audio bitrate"
            '
            'Label17
            '
            Me.Label17.AutoSize = True
            Me.Label17.Location = New System.Drawing.Point(6, 38)
            Me.Label17.Name = "Label17"
            Me.Label17.Size = New System.Drawing.Size(66, 13)
            Me.Label17.TabIndex = 0
            Me.Label17.Text = "Video bitrate"
            '
            'Link
            '
            Me.Link.AutoSize = True
            Me.Link.Location = New System.Drawing.Point(306, 315)
            Me.Link.Name = "Link"
            Me.Link.Size = New System.Drawing.Size(27, 13)
            Me.Link.TabIndex = 6
            Me.Link.TabStop = True
            Me.Link.Text = "Link"
            '
            'GroupBox8
            '
            Me.GroupBox8.Controls.Add(Me.RichTextBox6)
            Me.GroupBox8.Controls.Add(Me.Custom)
            Me.GroupBox8.Controls.Add(Me.RichTextBox8)
            Me.GroupBox8.Location = New System.Drawing.Point(6, 127)
            Me.GroupBox8.Name = "GroupBox8"
            Me.GroupBox8.Size = New System.Drawing.Size(442, 115)
            Me.GroupBox8.TabIndex = 5
            Me.GroupBox8.TabStop = False
            '
            'RichTextBox6
            '
            Me.RichTextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox6.Location = New System.Drawing.Point(10, 48)
            Me.RichTextBox6.Name = "RichTextBox6"
            Me.RichTextBox6.Size = New System.Drawing.Size(151, 61)
            Me.RichTextBox6.TabIndex = 6
            Me.RichTextBox6.Text = "Enter command line:" & Global.Microsoft.VisualBasic.ChrW(10) & "Use the following variables:" & Global.Microsoft.VisualBasic.ChrW(10) & "    """"{0}"""" = input filename" & Global.Microsoft.VisualBasic.ChrW(10) & "   " & _
                " """"{1}"""" = output filename"
            '
            'Custom
            '
            Me.Custom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.Custom.Location = New System.Drawing.Point(167, 48)
            Me.Custom.Name = "Custom"
            Me.Custom.Size = New System.Drawing.Size(264, 61)
            Me.Custom.TabIndex = 5
            Me.Custom.Text = ""
            '
            'RichTextBox8
            '
            Me.RichTextBox8.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox8.Location = New System.Drawing.Point(9, 13)
            Me.RichTextBox8.Name = "RichTextBox8"
            Me.RichTextBox8.Size = New System.Drawing.Size(422, 29)
            Me.RichTextBox8.TabIndex = 4
            Me.RichTextBox8.Text = "RichTextBox8"
            '
            'GroupBox7
            '
            Me.GroupBox7.Controls.Add(Me.Preset)
            Me.GroupBox7.Controls.Add(Me.Label12)
            Me.GroupBox7.Controls.Add(Me.Label13)
            Me.GroupBox7.Location = New System.Drawing.Point(6, 51)
            Me.GroupBox7.Name = "GroupBox7"
            Me.GroupBox7.Size = New System.Drawing.Size(442, 70)
            Me.GroupBox7.TabIndex = 4
            Me.GroupBox7.TabStop = False
            '
            'Preset
            '
            Me.Preset.FormattingEnabled = True
            Me.Preset.Location = New System.Drawing.Point(277, 37)
            Me.Preset.Name = "Preset"
            Me.Preset.Size = New System.Drawing.Size(157, 21)
            Me.Preset.TabIndex = 3
            '
            'Label12
            '
            Me.Label12.AutoSize = True
            Me.Label12.Location = New System.Drawing.Point(6, 40)
            Me.Label12.Name = "Label12"
            Me.Label12.Size = New System.Drawing.Size(69, 13)
            Me.Label12.TabIndex = 2
            Me.Label12.Text = "Select preset"
            '
            'Label13
            '
            Me.Label13.AutoSize = True
            Me.Label13.Location = New System.Drawing.Point(6, 16)
            Me.Label13.Name = "Label13"
            Me.Label13.Size = New System.Drawing.Size(45, 13)
            Me.Label13.TabIndex = 1
            Me.Label13.Text = "Label13"
            '
            'TabPage2
            '
            Me.TabPage2.Controls.Add(Me.GroupBox3)
            Me.TabPage2.Location = New System.Drawing.Point(4, 22)
            Me.TabPage2.Name = "TabPage2"
            Me.TabPage2.Size = New System.Drawing.Size(454, 343)
            Me.TabPage2.TabIndex = 3
            Me.TabPage2.Text = "Channel Groups"
            Me.TabPage2.UseVisualStyleBackColor = True
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.RichTextBox5)
            Me.GroupBox3.Controls.Add(Me.CheckedListBox1)
            Me.GroupBox3.Location = New System.Drawing.Point(4, 4)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(447, 154)
            Me.GroupBox3.TabIndex = 1
            Me.GroupBox3.TabStop = False
            '
            'RichTextBox5
            '
            Me.RichTextBox5.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.RichTextBox5.Location = New System.Drawing.Point(6, 19)
            Me.RichTextBox5.Name = "RichTextBox5"
            Me.RichTextBox5.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
            Me.RichTextBox5.Size = New System.Drawing.Size(409, 26)
            Me.RichTextBox5.TabIndex = 12
            Me.RichTextBox5.Text = "Transcoding takes time and disk space, to reduce this select which Channel groups" & _
                " you do NOT want to automatically transcode."
            '
            'CheckedListBox1
            '
            Me.CheckedListBox1.CheckOnClick = True
            Me.CheckedListBox1.FormattingEnabled = True
            Me.CheckedListBox1.Location = New System.Drawing.Point(6, 51)
            Me.CheckedListBox1.MultiColumn = True
            Me.CheckedListBox1.Name = "CheckedListBox1"
            Me.CheckedListBox1.Size = New System.Drawing.Size(435, 94)
            Me.CheckedListBox1.TabIndex = 0
            Me.CheckedListBox1.ThreeDCheckBoxes = True
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
            'Label10
            '
            Me.Label10.AutoSize = True
            Me.Label10.Location = New System.Drawing.Point(274, 16)
            Me.Label10.Name = "Label10"
            Me.Label10.Size = New System.Drawing.Size(66, 13)
            Me.Label10.TabIndex = 3
            Me.Label10.Text = "Audio bitrate"
            '
            'Label15
            '
            Me.Label15.AutoSize = True
            Me.Label15.Location = New System.Drawing.Point(6, 16)
            Me.Label15.Name = "Label15"
            Me.Label15.Size = New System.Drawing.Size(66, 13)
            Me.Label15.TabIndex = 0
            Me.Label15.Text = "Video bitrate"
            '
            'PluginSetup
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TabControl1)
            Me.Name = "PluginSetup"
            Me.Size = New System.Drawing.Size(486, 395)
            Me.TabControl1.ResumeLayout(False)
            Me.TabPage1.ResumeLayout(False)
            Me.TabPage1.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.MP4Group.ResumeLayout(False)
            Me.MP4Group.PerformLayout()
            Me.DelGroup.ResumeLayout(False)
            Me.DelGroup.PerformLayout()
            Me.TabPage3.ResumeLayout(False)
            Me.TabPage3.PerformLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox9.ResumeLayout(False)
            Me.GroupBox9.PerformLayout()
            CType(Me.audioBitrate, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.videoBitrate, System.ComponentModel.ISupportInitialize).EndInit()
            Me.GroupBox8.ResumeLayout(False)
            Me.GroupBox7.ResumeLayout(False)
            Me.GroupBox7.PerformLayout()
            Me.TabPage2.ResumeLayout(False)
            Me.GroupBox3.ResumeLayout(False)
            CType(Me.numAudio, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numVideo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Public Overrides Sub OnSectionActivated()

            MyBase.OnSectionActivated()
            Log.Info("iPiMPTranscodeToMP4: Configuration activated")

            iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4.LoadSettings()

            PopulateChannelGroups()

            If iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._deleteWithRecording Then
                DelYes.Checked = True
                DelNo.Checked = False
            Else
                DelYes.Checked = False
                DelNo.Checked = True
            End If

            folder.Text = iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._folderPath

            Starttime.Text = iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._transcodeTime

            If iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._transcodeNow Then
                TrYes.Checked = True
                TrNo.Checked = False
                lblStartTime.Visible = False
                Starttime.Visible = False
            Else
                TrYes.Checked = False
                TrNo.Checked = True
                lblStartTime.Visible = True
                Starttime.Visible = True
            End If

            If iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._transcoder.ToLower = "ffmpeg" Then
                Handbrake.Checked = False
                FFMpeg.Checked = True
            Else
                Handbrake.Checked = True
                FFMpeg.Checked = False
            End If

            Preset.Text = iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._preset
            Custom.Text = iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._custom

            videoBitrate.Value = iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._videoBitrate
            audioBitrate.Value = iPiMPTranscodeToMP4.TVEngine.iPiMPTranscodeToMP4._audioBitrate

        End Sub

        Public Overrides Sub OnSectionDeActivated()

            MyBase.OnSectionDeActivated()
            Log.Info("iPiMPTranscodeToMP4: Configuration deactivated")

            Dim layer As New TvBusinessLayer
            Dim setting As Setting
            Dim msg As String = String.Empty

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

            If Not Directory.Exists(folder.Text) Then
                msg = "Invalid MP4 path."
                Log.Error("iPiMPTranscodeToMP4 - OnSectionDeActivated: {0}", msg)
            End If
            setting = layer.GetSetting("iPiMPTranscodeToMP4_SavePath")
            setting.Value = folder.Text
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_Transcoder")
            If FFMpeg.Checked Then
                setting.Value = "ffmpeg"
            Else
                setting.Value = "handbrake"
            End If
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_Preset")
            setting.Value = Preset.Text
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_Custom")
            setting.Value = Custom.Text
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_TranscodeTime")
            setting.Value = Starttime.Text
            If TrYes.Checked Then setting.Value = ""
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_VideoBitrate")
            setting.Value = numVideo.Value.ToString
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_AudioBitrate")
            setting.Value = numAudio.Value.ToString
            setting.Persist()

            setting = layer.GetSetting("iPiMPTranscodeToMP4_Groups")
            Dim checkedGroups As CheckedListBox.CheckedItemCollection = CheckedListBox1.CheckedItems
            Dim groups As String = String.Empty
            For Each group As String In checkedGroups
                groups += String.Format("{0},", group)
            Next
            setting.Value = groups
            setting.Persist()

            MyBase.OnSectionDeActivated()

        End Sub

        Private Sub PopulateChannelGroups()

            CheckedListBox1.Items.Clear()
            For Each Group As ChannelGroup In TvDatabase.ChannelGroup.ListAll
                CheckedListBox1.Items.Add(Group.GroupName)
            Next

            For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                For Each group As String In TVEngine.iPiMPTranscodeToMP4._groups
                    If CheckedListBox1.Items.Item(i).ToString.ToLower = group.ToLower Then
                        CheckedListBox1.SetItemChecked(i, True)
                    End If
                Next
            Next

        End Sub

        Private Sub FFMpeg_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FFMpeg.CheckedChanged
            UpdateTranscoderSettingsTab()
        End Sub

        Private Sub Handbrake_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Handbrake.CheckedChanged
            UpdateTranscoderSettingsTab()
        End Sub

        Private Sub UpdateTranscoderSettingsTab()

            If FFMpeg.Checked Then
                TabControl1.TabPages(1).Text = "FFMpeg settings"
                PictureBox1.Image = My.Resources.ffmpeg
                Label11.Text = "FFMpeg settings"
                Label11.ForeColor = Color.Green
                Label13.Text = "FFMpeg has a number of preset transcoding settings."
                Preset.Items.Clear()
                Preset.Items.Add("libx264-iPiMP")
                Preset.Items.Add("libx264-baseline")
                Preset.Items.Add("libx264-default")
                Preset.Items.Add("libx264-fastfirstpass")
                Preset.Items.Add("libx264-hq")
                Preset.Items.Add("libx264-ipod320")
                Preset.Items.Add("libx264-ipod640")
                Preset.Items.Add("libx264-lossless_fast")
                Preset.Items.Add("libx264-lossless_max")
                Preset.Items.Add("libx264-lossless_medium")
                Preset.Items.Add("libx264-lossless_slow")
                Preset.Items.Add("libx264-lossless_slower")
                Preset.Items.Add("libx264-lossless_ultrafast")
                Preset.Items.Add("libx264-main")
                Preset.Items.Add("libx264-max")
                Preset.Items.Add("libx264-normal")
                Preset.Items.Add("libx264-slowfirstpass")
                RichTextBox8.Text = "You can override the available presets by entering your own FFMpeg command line parameters.  This requires knowledge of the available settings and transcoding options."
                Link.Text = "Read more about FFMpeg"
                Preset.Text = "libx264-iPiMP"
                GroupBox9.Visible = True
            Else
                TabControl1.TabPages(1).Text = "HandBrake settings"
                PictureBox1.Image = My.Resources.handbrake
                Label11.Text = "HandBrake settings"
                Label11.ForeColor = Color.CornflowerBlue
                Label13.Text = "HandBrake has a number of preset transcoding settings."
                Preset.Items.Clear()
                Preset.Items.Add("iPhone & iPod Touch")
                Preset.Items.Add("Universal")
                Preset.Items.Add("iPod")
                Preset.Items.Add("AppleTV")
                Preset.Items.Add("Normal")
                Preset.Items.Add("High Profile")
                Preset.Items.Add("Classic")
                Preset.Items.Add("AppleTV Legacy")
                Preset.Items.Add("iPhone Legacy")
                Preset.Items.Add("iPod Legacy")
                RichTextBox8.Text = "You can override the available presets by entering your own HandBrake command line parameters.  This requires knowledge of the available settings and transcoding options."
                GroupBox9.Visible = False
                Link.Text = "Read more about HandBrake"
                Preset.Text = "iPhone & iPod Touch"
            End If
        End Sub

        Private Sub ffLink_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles Link.LinkClicked
            Link.LinkVisited = True
            If FFMpeg.Checked Then
                System.Diagnostics.Process.Start("http://ffmpeg.org/")
            Else
                System.Diagnostics.Process.Start("http://handbrake.fr/")
            End If
        End Sub

        Private Sub TrYes_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrYes.CheckedChanged
            ShowHideStarttime()
        End Sub

        Private Sub TrNo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrNo.CheckedChanged
            ShowHideStarttime()
        End Sub

        Private Sub ShowHideStarttime()
            If TrYes.Checked Then
                lblStartTime.Visible = False
                Starttime.Visible = False
            Else
                lblStartTime.Visible = True
                Starttime.Visible = True
            End If
        End Sub

    End Class

End Namespace
