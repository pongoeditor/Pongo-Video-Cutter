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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtDuration = New System.Windows.Forms.TextBox()
        Me.txtFormatlabel = New System.Windows.Forms.Label()
        Me.txtResolution = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtFilesize = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtFilename = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.txtInputFile = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btnSaveAs = New System.Windows.Forms.Button()
        Me.txtOutputFile = New System.Windows.Forms.TextBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtEndTime = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtStartTime = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.btnClearSegments = New System.Windows.Forms.Button()
        Me.btnRemoveSegment = New System.Windows.Forms.Button()
        Me.lstSegments = New System.Windows.Forms.ListBox()
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer4 = New System.Windows.Forms.Timer(Me.components)
        Me.TrackBar1 = New System.Windows.Forms.TrackBar()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenVideoFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartTimeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EndTimeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.RemoveSegmentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearAllSegmentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FFmpegPathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ZoomInTimelineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ZoomOutTimelineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ResetZoomToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.PanelLeftToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PanelRightToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DonateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SendFeedbackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.LicenseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutPongoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.chkMergeOutput = New System.Windows.Forms.RadioButton()
        Me.ZoomResetButton = New System.Windows.Forms.Button()
        Me.zoomOutButton = New System.Windows.Forms.Button()
        Me.zoomInButton = New System.Windows.Forms.Button()
        Me.btnCut = New System.Windows.Forms.Button()
        Me.btnPlayPause = New System.Windows.Forms.Button()
        Me.btnSetEnd = New System.Windows.Forms.Button()
        Me.btnSetStart = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.txtMetadata = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.TutorialsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VideoPlayer1 = New Pongo_Video_Cutter.VideoPlayer()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtDuration)
        Me.GroupBox1.Controls.Add(Me.txtFormatlabel)
        Me.GroupBox1.Controls.Add(Me.txtResolution)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtFilesize)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.txtFilename)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.btnBrowse)
        Me.GroupBox1.Controls.Add(Me.txtInputFile)
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.GroupBox1.Location = New System.Drawing.Point(6, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(215, 263)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Import Media"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(6, 45)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(61, 15)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "File Path :"
        '
        'txtDuration
        '
        Me.txtDuration.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(65, Byte), Integer))
        Me.txtDuration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtDuration.Location = New System.Drawing.Point(6, 233)
        Me.txtDuration.Name = "txtDuration"
        Me.txtDuration.ReadOnly = True
        Me.txtDuration.Size = New System.Drawing.Size(203, 20)
        Me.txtDuration.TabIndex = 9
        Me.txtDuration.TabStop = False
        '
        'txtFormatlabel
        '
        Me.txtFormatlabel.AutoSize = True
        Me.txtFormatlabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFormatlabel.Location = New System.Drawing.Point(6, 214)
        Me.txtFormatlabel.Name = "txtFormatlabel"
        Me.txtFormatlabel.Size = New System.Drawing.Size(60, 15)
        Me.txtFormatlabel.TabIndex = 8
        Me.txtFormatlabel.Text = "Duration :"
        '
        'txtResolution
        '
        Me.txtResolution.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(65, Byte), Integer))
        Me.txtResolution.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtResolution.Location = New System.Drawing.Point(6, 191)
        Me.txtResolution.Name = "txtResolution"
        Me.txtResolution.ReadOnly = True
        Me.txtResolution.Size = New System.Drawing.Size(203, 20)
        Me.txtResolution.TabIndex = 7
        Me.txtResolution.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(6, 172)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(72, 15)
        Me.Label5.TabIndex = 6
        Me.Label5.Text = "Resolution :"
        '
        'txtFilesize
        '
        Me.txtFilesize.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(65, Byte), Integer))
        Me.txtFilesize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtFilesize.Location = New System.Drawing.Point(6, 149)
        Me.txtFilesize.Name = "txtFilesize"
        Me.txtFilesize.ReadOnly = True
        Me.txtFilesize.Size = New System.Drawing.Size(203, 20)
        Me.txtFilesize.TabIndex = 5
        Me.txtFilesize.TabStop = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(6, 130)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(60, 15)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "File Size :"
        '
        'txtFilename
        '
        Me.txtFilename.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(65, Byte), Integer))
        Me.txtFilename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtFilename.Location = New System.Drawing.Point(6, 107)
        Me.txtFilename.Name = "txtFilename"
        Me.txtFilename.ReadOnly = True
        Me.txtFilename.Size = New System.Drawing.Size(203, 20)
        Me.txtFilename.TabIndex = 3
        Me.txtFilename.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(6, 88)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(65, 15)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Filename :"
        '
        'btnBrowse
        '
        Me.btnBrowse.BackColor = System.Drawing.Color.White
        Me.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBrowse.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnBrowse.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.folder
        Me.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBrowse.Location = New System.Drawing.Point(6, 17)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(85, 23)
        Me.btnBrowse.TabIndex = 1
        Me.btnBrowse.TabStop = False
        Me.btnBrowse.Text = "Browse..."
        Me.btnBrowse.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnBrowse.UseVisualStyleBackColor = False
        '
        'txtInputFile
        '
        Me.txtInputFile.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(65, Byte), Integer))
        Me.txtInputFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtInputFile.ForeColor = System.Drawing.SystemColors.Window
        Me.txtInputFile.Location = New System.Drawing.Point(6, 65)
        Me.txtInputFile.Name = "txtInputFile"
        Me.txtInputFile.ReadOnly = True
        Me.txtInputFile.Size = New System.Drawing.Size(203, 20)
        Me.txtInputFile.TabIndex = 0
        Me.txtInputFile.TabStop = False
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.Controls.Add(Me.btnSaveAs)
        Me.GroupBox3.Controls.Add(Me.txtOutputFile)
        Me.GroupBox3.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.GroupBox3.Location = New System.Drawing.Point(263, 451)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(258, 51)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Output Path"
        Me.GroupBox3.Visible = False
        '
        'btnSaveAs
        '
        Me.btnSaveAs.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnSaveAs.Location = New System.Drawing.Point(177, 17)
        Me.btnSaveAs.Name = "btnSaveAs"
        Me.btnSaveAs.Size = New System.Drawing.Size(75, 23)
        Me.btnSaveAs.TabIndex = 2
        Me.btnSaveAs.TabStop = False
        Me.btnSaveAs.Text = "Path"
        Me.btnSaveAs.UseVisualStyleBackColor = True
        '
        'txtOutputFile
        '
        Me.txtOutputFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtOutputFile.Location = New System.Drawing.Point(6, 19)
        Me.txtOutputFile.Name = "txtOutputFile"
        Me.txtOutputFile.ReadOnly = True
        Me.txtOutputFile.Size = New System.Drawing.Size(165, 20)
        Me.txtOutputFile.TabIndex = 3
        Me.txtOutputFile.TabStop = False
        '
        'lblStatus
        '
        Me.lblStatus.BackColor = System.Drawing.Color.White
        Me.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblStatus.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblStatus.Location = New System.Drawing.Point(0, 724)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(1057, 18)
        Me.lblStatus.TabIndex = 6
        Me.lblStatus.Text = "Ready"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ProgressBar1.ForeColor = System.Drawing.Color.Lime
        Me.ProgressBar1.Location = New System.Drawing.Point(5, 650)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(255, 10)
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar1.TabIndex = 7
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.VideoPlayer1)
        Me.Panel1.Controls.Add(Me.GroupBox2)
        Me.Panel1.Controls.Add(Me.GroupBox3)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(558, 526)
        Me.Panel1.TabIndex = 15
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.txtEndTime)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.txtStartTime)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.GroupBox2.Location = New System.Drawing.Point(32, 41)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(215, 103)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Visible = False
        '
        'txtEndTime
        '
        Me.txtEndTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtEndTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEndTime.Location = New System.Drawing.Point(9, 72)
        Me.txtEndTime.Name = "txtEndTime"
        Me.txtEndTime.ReadOnly = True
        Me.txtEndTime.Size = New System.Drawing.Size(199, 21)
        Me.txtEndTime.TabIndex = 3
        Me.txtEndTime.TabStop = False
        Me.txtEndTime.Text = "00:00:00"
        Me.txtEndTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 56)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(118, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "End Time (HH:MM:SS):"
        '
        'txtStartTime
        '
        Me.txtStartTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtStartTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStartTime.Location = New System.Drawing.Point(9, 32)
        Me.txtStartTime.Name = "txtStartTime"
        Me.txtStartTime.ReadOnly = True
        Me.txtStartTime.Size = New System.Drawing.Size(199, 21)
        Me.txtStartTime.TabIndex = 1
        Me.txtStartTime.TabStop = False
        Me.txtStartTime.Text = "00:00:00"
        Me.txtStartTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(121, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Start Time (HH:MM:SS):"
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.Controls.Add(Me.btnClearSegments)
        Me.GroupBox4.Controls.Add(Me.btnRemoveSegment)
        Me.GroupBox4.Controls.Add(Me.lstSegments)
        Me.GroupBox4.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.GroupBox4.Location = New System.Drawing.Point(7, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(258, 586)
        Me.GroupBox4.TabIndex = 18
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Segment"
        '
        'btnClearSegments
        '
        Me.btnClearSegments.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearSegments.BackColor = System.Drawing.Color.White
        Me.btnClearSegments.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClearSegments.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClearSegments.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnClearSegments.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.close
        Me.btnClearSegments.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnClearSegments.Location = New System.Drawing.Point(135, 557)
        Me.btnClearSegments.Name = "btnClearSegments"
        Me.btnClearSegments.Size = New System.Drawing.Size(108, 23)
        Me.btnClearSegments.TabIndex = 3
        Me.btnClearSegments.TabStop = False
        Me.btnClearSegments.Text = "Clear Segment"
        Me.btnClearSegments.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnClearSegments.UseVisualStyleBackColor = False
        '
        'btnRemoveSegment
        '
        Me.btnRemoveSegment.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveSegment.BackColor = System.Drawing.Color.White
        Me.btnRemoveSegment.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRemoveSegment.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveSegment.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnRemoveSegment.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.delete
        Me.btnRemoveSegment.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRemoveSegment.Location = New System.Drawing.Point(6, 557)
        Me.btnRemoveSegment.Name = "btnRemoveSegment"
        Me.btnRemoveSegment.Size = New System.Drawing.Size(123, 23)
        Me.btnRemoveSegment.TabIndex = 2
        Me.btnRemoveSegment.TabStop = False
        Me.btnRemoveSegment.Text = "Remove Segment"
        Me.btnRemoveSegment.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnRemoveSegment.UseVisualStyleBackColor = False
        '
        'lstSegments
        '
        Me.lstSegments.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstSegments.BackColor = System.Drawing.SystemColors.Control
        Me.lstSegments.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lstSegments.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstSegments.FormattingEnabled = True
        Me.lstSegments.ItemHeight = 17
        Me.lstSegments.Location = New System.Drawing.Point(6, 17)
        Me.lstSegments.Name = "lstSegments"
        Me.lstSegments.Size = New System.Drawing.Size(246, 527)
        Me.lstSegments.TabIndex = 0
        '
        'Timer3
        '
        '
        'Timer4
        '
        Me.Timer4.Enabled = True
        Me.Timer4.Interval = 120000
        '
        'TrackBar1
        '
        Me.TrackBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TrackBar1.Location = New System.Drawing.Point(7, 2)
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(546, 45)
        Me.TrackBar1.TabIndex = 21
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.SetToolStripMenuItem, Me.SettingsToolStripMenuItem, Me.ViewToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1057, 24)
        Me.MenuStrip1.TabIndex = 22
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenVideoFilesToolStripMenuItem, Me.ToolStripSeparator1, Me.ExportToolStripMenuItem, Me.ToolStripSeparator2, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'OpenVideoFilesToolStripMenuItem
        '
        Me.OpenVideoFilesToolStripMenuItem.Name = "OpenVideoFilesToolStripMenuItem"
        Me.OpenVideoFilesToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenVideoFilesToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.OpenVideoFilesToolStripMenuItem.Text = "Open Video Files..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(211, 6)
        '
        'ExportToolStripMenuItem
        '
        Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
        Me.ExportToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.ExportToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ExportToolStripMenuItem.Text = "Export"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(211, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'SetToolStripMenuItem
        '
        Me.SetToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartTimeToolStripMenuItem, Me.EndTimeToolStripMenuItem, Me.ToolStripSeparator3, Me.RemoveSegmentToolStripMenuItem, Me.ClearAllSegmentToolStripMenuItem})
        Me.SetToolStripMenuItem.Name = "SetToolStripMenuItem"
        Me.SetToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.SetToolStripMenuItem.Text = "&Option"
        '
        'StartTimeToolStripMenuItem
        '
        Me.StartTimeToolStripMenuItem.Name = "StartTimeToolStripMenuItem"
        Me.StartTimeToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
                    Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.StartTimeToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.StartTimeToolStripMenuItem.Text = "Start Time"
        '
        'EndTimeToolStripMenuItem
        '
        Me.EndTimeToolStripMenuItem.Name = "EndTimeToolStripMenuItem"
        Me.EndTimeToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
                    Or System.Windows.Forms.Keys.E), System.Windows.Forms.Keys)
        Me.EndTimeToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.EndTimeToolStripMenuItem.Text = "End Time"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(188, 6)
        '
        'RemoveSegmentToolStripMenuItem
        '
        Me.RemoveSegmentToolStripMenuItem.Name = "RemoveSegmentToolStripMenuItem"
        Me.RemoveSegmentToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.RemoveSegmentToolStripMenuItem.Text = "Remove Segment"
        '
        'ClearAllSegmentToolStripMenuItem
        '
        Me.ClearAllSegmentToolStripMenuItem.Name = "ClearAllSegmentToolStripMenuItem"
        Me.ClearAllSegmentToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
        Me.ClearAllSegmentToolStripMenuItem.Text = "Clear All Segment"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FFmpegPathToolStripMenuItem})
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.SettingsToolStripMenuItem.Text = "&Settings"
        '
        'FFmpegPathToolStripMenuItem
        '
        Me.FFmpegPathToolStripMenuItem.Name = "FFmpegPathToolStripMenuItem"
        Me.FFmpegPathToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.FFmpegPathToolStripMenuItem.Text = "FFmpeg Path"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ZoomInTimelineToolStripMenuItem, Me.ZoomOutTimelineToolStripMenuItem, Me.ResetZoomToolStripMenuItem, Me.ToolStripSeparator5, Me.PanelLeftToolStripMenuItem, Me.PanelRightToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem.Text = "&View"
        '
        'ZoomInTimelineToolStripMenuItem
        '
        Me.ZoomInTimelineToolStripMenuItem.Name = "ZoomInTimelineToolStripMenuItem"
        Me.ZoomInTimelineToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.T), System.Windows.Forms.Keys)
        Me.ZoomInTimelineToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.ZoomInTimelineToolStripMenuItem.Text = "Zoom In Timeline"
        '
        'ZoomOutTimelineToolStripMenuItem
        '
        Me.ZoomOutTimelineToolStripMenuItem.Name = "ZoomOutTimelineToolStripMenuItem"
        Me.ZoomOutTimelineToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.U), System.Windows.Forms.Keys)
        Me.ZoomOutTimelineToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.ZoomOutTimelineToolStripMenuItem.Text = "Zoom Out Timeline"
        '
        'ResetZoomToolStripMenuItem
        '
        Me.ResetZoomToolStripMenuItem.Name = "ResetZoomToolStripMenuItem"
        Me.ResetZoomToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.ResetZoomToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.ResetZoomToolStripMenuItem.Text = "Reset Zoom"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(217, 6)
        '
        'PanelLeftToolStripMenuItem
        '
        Me.PanelLeftToolStripMenuItem.Checked = True
        Me.PanelLeftToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.PanelLeftToolStripMenuItem.Name = "PanelLeftToolStripMenuItem"
        Me.PanelLeftToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.PanelLeftToolStripMenuItem.Text = "Panel Left"
        '
        'PanelRightToolStripMenuItem
        '
        Me.PanelRightToolStripMenuItem.Checked = True
        Me.PanelRightToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.PanelRightToolStripMenuItem.Name = "PanelRightToolStripMenuItem"
        Me.PanelRightToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.PanelRightToolStripMenuItem.Text = "Panel Right"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TutorialsToolStripMenuItem, Me.DonateToolStripMenuItem, Me.SendFeedbackToolStripMenuItem, Me.ToolStripSeparator4, Me.LicenseToolStripMenuItem, Me.AboutPongoToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'DonateToolStripMenuItem
        '
        Me.DonateToolStripMenuItem.Name = "DonateToolStripMenuItem"
        Me.DonateToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.DonateToolStripMenuItem.Text = "Donate"
        '
        'SendFeedbackToolStripMenuItem
        '
        Me.SendFeedbackToolStripMenuItem.Name = "SendFeedbackToolStripMenuItem"
        Me.SendFeedbackToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.SendFeedbackToolStripMenuItem.Text = "Send Feedback"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(151, 6)
        '
        'LicenseToolStripMenuItem
        '
        Me.LicenseToolStripMenuItem.Name = "LicenseToolStripMenuItem"
        Me.LicenseToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.LicenseToolStripMenuItem.Text = "License"
        '
        'AboutPongoToolStripMenuItem
        '
        Me.AboutPongoToolStripMenuItem.Name = "AboutPongoToolStripMenuItem"
        Me.AboutPongoToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.AboutPongoToolStripMenuItem.Text = "About Pongo..."
        '
        'RadioButton1
        '
        Me.RadioButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Checked = True
        Me.RadioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.RadioButton1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadioButton1.ForeColor = System.Drawing.Color.Yellow
        Me.RadioButton1.Location = New System.Drawing.Point(82, 600)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(101, 19)
        Me.RadioButton1.TabIndex = 26
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "Sparate Video"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'chkMergeOutput
        '
        Me.chkMergeOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkMergeOutput.AutoSize = True
        Me.chkMergeOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.chkMergeOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkMergeOutput.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.chkMergeOutput.Location = New System.Drawing.Point(82, 625)
        Me.chkMergeOutput.Name = "chkMergeOutput"
        Me.chkMergeOutput.Size = New System.Drawing.Size(115, 19)
        Me.chkMergeOutput.TabIndex = 27
        Me.chkMergeOutput.Text = "Combined Video"
        Me.chkMergeOutput.UseVisualStyleBackColor = True
        '
        'ZoomResetButton
        '
        Me.ZoomResetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ZoomResetButton.BackColor = System.Drawing.Color.White
        Me.ZoomResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ZoomResetButton.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.zoom
        Me.ZoomResetButton.Location = New System.Drawing.Point(303, 85)
        Me.ZoomResetButton.Name = "ZoomResetButton"
        Me.ZoomResetButton.Size = New System.Drawing.Size(32, 23)
        Me.ZoomResetButton.TabIndex = 25
        Me.ZoomResetButton.TabStop = False
        Me.ZoomResetButton.UseVisualStyleBackColor = False
        '
        'zoomOutButton
        '
        Me.zoomOutButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.zoomOutButton.BackColor = System.Drawing.Color.White
        Me.zoomOutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.zoomOutButton.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.zoom_out
        Me.zoomOutButton.Location = New System.Drawing.Point(265, 85)
        Me.zoomOutButton.Name = "zoomOutButton"
        Me.zoomOutButton.Size = New System.Drawing.Size(32, 23)
        Me.zoomOutButton.TabIndex = 24
        Me.zoomOutButton.TabStop = False
        Me.zoomOutButton.UseVisualStyleBackColor = False
        '
        'zoomInButton
        '
        Me.zoomInButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.zoomInButton.BackColor = System.Drawing.Color.White
        Me.zoomInButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.zoomInButton.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.zoom_in
        Me.zoomInButton.Location = New System.Drawing.Point(227, 85)
        Me.zoomInButton.Name = "zoomInButton"
        Me.zoomInButton.Size = New System.Drawing.Size(32, 23)
        Me.zoomInButton.TabIndex = 23
        Me.zoomInButton.TabStop = False
        Me.zoomInButton.UseVisualStyleBackColor = False
        '
        'btnCut
        '
        Me.btnCut.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCut.BackColor = System.Drawing.Color.PaleGreen
        Me.btnCut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCut.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.diskette
        Me.btnCut.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnCut.Location = New System.Drawing.Point(5, 600)
        Me.btnCut.Name = "btnCut"
        Me.btnCut.Size = New System.Drawing.Size(71, 44)
        Me.btnCut.TabIndex = 3
        Me.btnCut.TabStop = False
        Me.btnCut.Text = "Export"
        Me.btnCut.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnCut.UseVisualStyleBackColor = False
        '
        'btnPlayPause
        '
        Me.btnPlayPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnPlayPause.BackColor = System.Drawing.Color.White
        Me.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPlayPause.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.bplay
        Me.btnPlayPause.Location = New System.Drawing.Point(243, 114)
        Me.btnPlayPause.Name = "btnPlayPause"
        Me.btnPlayPause.Size = New System.Drawing.Size(75, 36)
        Me.btnPlayPause.TabIndex = 14
        Me.btnPlayPause.TabStop = False
        Me.btnPlayPause.UseVisualStyleBackColor = False
        '
        'btnSetEnd
        '
        Me.btnSetEnd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSetEnd.BackColor = System.Drawing.Color.White
        Me.btnSetEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSetEnd.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.timeend
        Me.btnSetEnd.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSetEnd.Location = New System.Drawing.Point(447, 115)
        Me.btnSetEnd.Name = "btnSetEnd"
        Me.btnSetEnd.Size = New System.Drawing.Size(103, 36)
        Me.btnSetEnd.TabIndex = 13
        Me.btnSetEnd.TabStop = False
        Me.btnSetEnd.Text = "Set End"
        Me.btnSetEnd.UseVisualStyleBackColor = False
        '
        'btnSetStart
        '
        Me.btnSetStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSetStart.BackColor = System.Drawing.Color.White
        Me.btnSetStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSetStart.Image = Global.Pongo_Video_Cutter.My.Resources.Resources.timestart
        Me.btnSetStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSetStart.Location = New System.Drawing.Point(7, 114)
        Me.btnSetStart.Name = "btnSetStart"
        Me.btnSetStart.Size = New System.Drawing.Size(105, 36)
        Me.btnSetStart.TabIndex = 12
        Me.btnSetStart.TabStop = False
        Me.btnSetStart.Text = "Set Start"
        Me.btnSetStart.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.GroupBox5)
        Me.Panel2.Controls.Add(Me.GroupBox1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel2.Location = New System.Drawing.Point(0, 24)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(227, 700)
        Me.Panel2.TabIndex = 31
        '
        'GroupBox5
        '
        Me.GroupBox5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox5.Controls.Add(Me.txtMetadata)
        Me.GroupBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.GroupBox5.Location = New System.Drawing.Point(6, 272)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(214, 388)
        Me.GroupBox5.TabIndex = 1
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "RAW Metadata"
        '
        'txtMetadata
        '
        Me.txtMetadata.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtMetadata.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(65, Byte), Integer))
        Me.txtMetadata.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtMetadata.ForeColor = System.Drawing.SystemColors.Info
        Me.txtMetadata.Location = New System.Drawing.Point(6, 16)
        Me.txtMetadata.Multiline = True
        Me.txtMetadata.Name = "txtMetadata"
        Me.txtMetadata.ReadOnly = True
        Me.txtMetadata.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtMetadata.Size = New System.Drawing.Size(203, 366)
        Me.txtMetadata.TabIndex = 12
        Me.txtMetadata.TabStop = False
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.GroupBox4)
        Me.Panel3.Controls.Add(Me.btnCut)
        Me.Panel3.Controls.Add(Me.chkMergeOutput)
        Me.Panel3.Controls.Add(Me.ProgressBar1)
        Me.Panel3.Controls.Add(Me.RadioButton1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel3.Location = New System.Drawing.Point(787, 24)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(270, 700)
        Me.Panel3.TabIndex = 3
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.Panel1)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(227, 24)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(560, 528)
        Me.Panel4.TabIndex = 4
        '
        'Panel5
        '
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.TrackBar1)
        Me.Panel5.Controls.Add(Me.btnSetStart)
        Me.Panel5.Controls.Add(Me.btnSetEnd)
        Me.Panel5.Controls.Add(Me.btnPlayPause)
        Me.Panel5.Controls.Add(Me.ZoomResetButton)
        Me.Panel5.Controls.Add(Me.zoomInButton)
        Me.Panel5.Controls.Add(Me.zoomOutButton)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel5.Location = New System.Drawing.Point(227, 552)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(560, 172)
        Me.Panel5.TabIndex = 32
        '
        'TutorialsToolStripMenuItem
        '
        Me.TutorialsToolStripMenuItem.Name = "TutorialsToolStripMenuItem"
        Me.TutorialsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.TutorialsToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.TutorialsToolStripMenuItem.Text = "Tutorials"
        '
        'VideoPlayer1
        '
        Me.VideoPlayer1.BackColor = System.Drawing.Color.Black
        Me.VideoPlayer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VideoPlayer1.Location = New System.Drawing.Point(0, 0)
        Me.VideoPlayer1.Name = "VideoPlayer1"
        Me.VideoPlayer1.Size = New System.Drawing.Size(558, 526)
        Me.VideoPlayer1.TabIndex = 0
        Me.VideoPlayer1.Text = "VideoPlayer1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1057, 742)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Pongo Video Cutter"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents txtInputFile As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSaveAs As System.Windows.Forms.Button
    Friend WithEvents txtOutputFile As System.Windows.Forms.TextBox
    Friend WithEvents btnCut As System.Windows.Forms.Button
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnSetStart As System.Windows.Forms.Button
    Friend WithEvents btnSetEnd As System.Windows.Forms.Button
    Friend WithEvents btnPlayPause As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents VideoPlayer1 As Pongo_Video_Cutter.VideoPlayer
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents lstSegments As System.Windows.Forms.ListBox
    Friend WithEvents btnClearSegments As System.Windows.Forms.Button
    Friend WithEvents btnRemoveSegment As System.Windows.Forms.Button
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents Timer3 As System.Windows.Forms.Timer
    Friend WithEvents Timer4 As System.Windows.Forms.Timer
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtStartTime As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtEndTime As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents zoomInButton As System.Windows.Forms.Button
    Friend WithEvents zoomOutButton As System.Windows.Forms.Button
    Friend WithEvents ZoomResetButton As System.Windows.Forms.Button
    Friend WithEvents OpenVideoFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartTimeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EndTimeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents RemoveSegmentToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ClearAllSegmentToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents txtResolution As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtFilesize As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtFilename As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtDuration As System.Windows.Forms.TextBox
    Friend WithEvents txtFormatlabel As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ZoomInTimelineToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ZoomOutTimelineToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResetZoomToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents chkMergeOutput As System.Windows.Forms.RadioButton
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DonateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SendFeedbackToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents LicenseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutPongoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FFmpegPathToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents PanelLeftToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PanelRightToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents txtMetadata As System.Windows.Forms.TextBox
    Friend WithEvents TutorialsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
