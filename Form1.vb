Imports System.IO
Imports System.Diagnostics

Public Class Form1
    ' Deklarasi variabel global
    Public ffmpegPath As String
    Public ffprobePath As String
    Private inputFile As String = ""
    Private outputFile As String = ""
    Private isUpdatingTrackbar As Boolean = False
    Private videoDuration As Double = 0
    Private currentPosition As Double = 0
    Private isPreviewPlaying As Boolean = False
    Private videoPlayer As VideoPlayer
    Private customTrackBar As CustomTrackBar
    Private isSettingStart As Boolean = False
    Private currentStartTime As Double = -1
    Private currentEndTime As Double = -1
    Private isPlayingSegment As Boolean = False
    Private segmentPlayStart As Double = 0
    Private segmentPlayEnd As Double = 0
    Private isUpdatingSegmentUI As Boolean = False
    Private isProcessingFile As Boolean = False

    ' Class untuk menyimpan segmen
    Private Class VideoSegment
        Public StartTime As Double
        Public EndTime As Double
        Public SegmentIndex As Integer

        Public Sub New()
            StartTime = 0
            EndTime = 0
            SegmentIndex = 0
        End Sub

        Public Sub New(ByVal start As Double, ByVal endT As Double, ByVal index As Integer)
            StartTime = start
            EndTime = endT
            SegmentIndex = index
        End Sub

        Public Overrides Function ToString() As String
            Dim startTs As TimeSpan = TimeSpan.FromSeconds(StartTime)
            Dim endTs As TimeSpan = TimeSpan.FromSeconds(EndTime)
            Dim durTs As TimeSpan = TimeSpan.FromSeconds(EndTime - StartTime)

            Dim startStr As String = String.Format("{0:00}:{1:00}:{2:00}", startTs.Hours, startTs.Minutes, startTs.Seconds)
            Dim endStr As String = String.Format("{0:00}:{1:00}:{2:00}", endTs.Hours, endTs.Minutes, endTs.Seconds)
            Dim durStr As String = String.Format("{0:00}:{1:00}:{2:00}", durTs.Hours, durTs.Minutes, durTs.Seconds)

            Return String.Format("{1} - {2}", SegmentIndex, startStr, endStr, durStr)
        End Function
    End Class
    Private Sub UpdateFormTitle()
        If String.IsNullOrEmpty(inputFile) Then
            Me.Text = "Pongo Video Cutter"
        Else
            Me.Text = "Pongo Video Cutter - " & Path.GetFileName(inputFile)
        End If
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "Pongo Video Cutter"
        Me.KeyPreview = True

        ' ===== DRAG & DROP SETUP =====
        Me.AllowDrop = True
        AddHandler Me.DragEnter, AddressOf Form1_DragEnter
        AddHandler Me.DragDrop, AddressOf Form1_DragDrop
        AddHandler Me.DragLeave, AddressOf Form1_DragLeave
        If String.IsNullOrEmpty(My.Settings.ffmpegpath) Then
            ffmpegPath = Application.StartupPath & "\ffmpeg.exe"
        Else
            ffmpegPath = My.Settings.ffmpegpath
        End If
        If String.IsNullOrEmpty(My.Settings.ffprobepath) Then
            ffprobePath = Application.StartupPath & "\ffprobe.exe"
        Else
            ffprobePath = My.Settings.ffprobepath
        End If
        ' Also enable drag & drop on all child controls
        EnableDragDropRecursively(Me)

        ' Setup Timer
        Timer3.Interval = 100


        ' Sembunyikan TrackBar bawaan
        If TrackBar1 IsNot Nothing Then
            TrackBar1.Visible = False
        End If

        customTrackBar = New CustomTrackBar()
        customTrackBar.Minimum = 0
        customTrackBar.Maximum = 1000
        customTrackBar.Width = TrackBar1.Width
        customTrackBar.Height = 80
        customTrackBar.Left = TrackBar1.Left
        customTrackBar.Top = TrackBar1.Top - 10
        customTrackBar.Anchor = TrackBar1.Anchor
        customTrackBar.BackColor = Me.BackColor
        customTrackBar.Enabled = True
        Panel5.Controls.Add(customTrackBar)  ' <-- Perubahan di sini
        customTrackBar.BringToFront()

        ' Setup event handlers
        AddHandler customTrackBar.ValueChanged, AddressOf CustomTrackBar_ValueChanged
        AddHandler customTrackBar.Scroll, AddressOf CustomTrackBar_Scroll
        AddHandler customTrackBar.SegmentClicked, AddressOf CustomTrackBar_SegmentClicked
        AddHandler customTrackBar.SegmentChanged, AddressOf CustomTrackBar_SegmentChanged
        AddHandler lstSegments.SelectedIndexChanged, AddressOf lstSegments_SelectedIndexChanged

        ' Setup Panel1
        If Panel1 IsNot Nothing Then
            Panel1.BackColor = Color.Black
            Panel1.BorderStyle = BorderStyle.None
            videoPlayer = New VideoPlayer()
            videoPlayer.Dock = DockStyle.Fill
            videoPlayer.BackColor = Color.Black
            AddHandler videoPlayer.PositionChanged, AddressOf VideoPlayer_PositionChanged
            AddHandler videoPlayer.PlaybackEnded, AddressOf VideoPlayer_PlaybackEnded
            Panel1.Controls.Add(videoPlayer)
        End If

        ' Setup ListBox - Ganti dengan ModernListBox
        If lstSegments IsNot Nothing Then
            ' Simpan referensi ke ListBox lama
            Dim oldListBox As ListBox = lstSegments
            Dim parentControl As Control = oldListBox.Parent
            Dim oldLocation As Point = oldListBox.Location
            Dim oldSize As Size = oldListBox.Size
            Dim oldAnchor As AnchorStyles = oldListBox.Anchor

            ' Buat ModernListBox baru
            Dim modernListBox As New ModernListBox()
            modernListBox.Location = oldLocation
            modernListBox.Size = oldSize
            modernListBox.Anchor = oldAnchor
            modernListBox.Name = "lstSegmentsModern"
            modernListBox.Items.Clear()

            ' Hapus ListBox lama dari form
            parentControl.Controls.Remove(oldListBox)
            oldListBox.Dispose()

            ' Tambahkan ModernListBox ke form
            parentControl.Controls.Add(modernListBox)

            ' Update referensi
            lstSegments = modernListBox

            ' Re-attach event handler
            AddHandler lstSegments.SelectedIndexChanged, AddressOf lstSegments_SelectedIndexChanged
        End If

        ' Setup CheckBox
        If chkMergeOutput IsNot Nothing Then
            chkMergeOutput.Checked = False
        End If

        ' Setup file info textboxes as read-only
        SetupFileInfoTextBoxes()

        ' Cek FFmpeg
        If Not System.IO.File.Exists(ffmpegPath) Then
            If lblStatus IsNot Nothing Then
                lblStatus.Text = "Status: FFmpeg NOT FOUND!"
                'lblStatus.ForeColor = Color.Yellow
            End If
            MessageBox.Show("FFmpeg was not found in the application directory!" & vbNewLine & vbNewLine & _
                          "Please download FFmpeg and place ffmpeg.exe in:" & vbNewLine & _
                          Application.StartupPath & vbNewLine & vbNewLine & _
                          "Download from: https://ffmpeg.org/download.html", _
                          "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            If lblStatus IsNot Nothing Then
                lblStatus.Text = "Status: Ready - Drag & Drop video or click Browse | Press SPACE to Play/Pause"
                'lblStatus.ForeColor = Color.White
            End If
        End If
    End Sub
    Private Sub VideoPlayer_PositionChanged(ByVal position As Double)
        Try
            If Me.InvokeRequired Then
                Me.BeginInvoke(New MethodInvoker(Sub() VideoPlayer_PositionChanged(position)))
                Return
            End If

            currentPosition = position

            If Not isUpdatingTrackbar AndAlso customTrackBar IsNot Nothing AndAlso videoDuration > 0 Then
                isUpdatingTrackbar = True
                customTrackBar.Value = CInt((position / videoDuration) * 1000)
                isUpdatingTrackbar = False
            End If

            If isPlayingSegment AndAlso position >= segmentPlayEnd Then
                StopPreview()
                isPlayingSegment = False
                lblStatus.Text = "Status: Segment playback finished"
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Sub VideoPlayer_PlaybackEnded()
        Try
            If Me.InvokeRequired Then
                Me.BeginInvoke(New MethodInvoker(AddressOf VideoPlayer_PlaybackEnded))
                Return
            End If

            If isPreviewPlaying Then
                StopPreview()
                lblStatus.Text = "Status: Playback finished"
            End If
        Catch ex As Exception
        End Try
    End Sub
    ' ==================== SETUP FILE INFO TEXTBOXES ====================
    Private Sub SetupFileInfoTextBoxes()
        ' Set file info textboxes as read-only
        If txtFilename IsNot Nothing Then
            txtFilename.ReadOnly = True
            txtFilename.Text = ""
            txtFilename.BackColor = Color.FromArgb(60, 60, 65)
            txtFilename.ForeColor = Color.White
        End If

        If txtFilesize IsNot Nothing Then
            txtFilesize.ReadOnly = True
            txtFilesize.Text = ""
            txtFilesize.BackColor = Color.FromArgb(60, 60, 65)
            txtFilesize.ForeColor = Color.White
        End If

        If txtResolution IsNot Nothing Then
            txtResolution.ReadOnly = True
            txtResolution.Text = ""
            txtResolution.BackColor = Color.FromArgb(60, 60, 65)
            txtResolution.ForeColor = Color.White
        End If

        If txtDuration IsNot Nothing Then
            txtDuration.ReadOnly = True
            txtDuration.Text = ""
            txtDuration.BackColor = Color.FromArgb(60, 60, 65)
            txtDuration.ForeColor = Color.White
        End If
    End Sub

    Private Sub UpdateFileInfo(ByVal filePath As String)
        Try
            ' Update filename
            If txtFilename IsNot Nothing Then
                txtFilename.Text = Path.GetFileName(filePath)
            End If

            ' Update filesize
            If txtFilesize IsNot Nothing Then
                Dim fileInfo As New FileInfo(filePath)
                txtFilesize.Text = FormatFileSize(fileInfo.Length)
            End If

            ' Get resolution and duration from ffprobe
            GetVideoInfoFromFFprobe(filePath)

            ' Get and display metadata
            If txtMetadata IsNot Nothing Then
                ' Show loading status
                txtMetadata.Text = "Loading metadata..." & vbNewLine & _
                                  "File: " & Path.GetFileName(filePath)
                txtMetadata.Refresh()
                Application.DoEvents()

                ' Get metadata
                Dim metadata As String = GetVideoMetadata(filePath)
                txtMetadata.Text = metadata
                txtMetadata.SelectionStart = 0
                txtMetadata.ScrollToCaret()
            End If

        Catch ex As Exception
            ' Silently handle file info errors
            Try
                System.IO.File.WriteAllText(Application.StartupPath & "\fileinfo_error.log", _
                    "Error: " & ex.Message & vbNewLine & ex.StackTrace)
            Catch
            End Try

            If txtMetadata IsNot Nothing Then
                txtMetadata.Text = "Error loading metadata: " & ex.Message
            End If
        End Try
    End Sub

    ' ==================== GET VIDEO INFO FROM FFPROBE ====================
    Private Sub GetVideoInfoFromFFprobe(ByVal filePath As String)
        Try
            Dim ffprobe As New Process()
            ffprobe.StartInfo.FileName = ffprobePath
            ffprobe.StartInfo.Arguments = String.Format( _
                "-v error -select_streams v:0 -show_entries stream=width,height,duration -show_entries format=duration -of default=noprint_wrappers=1 ""{0}""", _
                filePath _
            )
            ffprobe.StartInfo.UseShellExecute = False
            ffprobe.StartInfo.RedirectStandardOutput = True
            ffprobe.StartInfo.RedirectStandardError = True
            ffprobe.StartInfo.CreateNoWindow = True

            ffprobe.Start()
            Dim output As String = ffprobe.StandardOutput.ReadToEnd()
            Dim errorOutput As String = ffprobe.StandardError.ReadToEnd()
            ffprobe.WaitForExit()

            ' Parse width and height
            Dim width As Integer = 0
            Dim height As Integer = 0

            Dim lines As String() = output.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
            For Each line As String In lines
                If line.StartsWith("width=") Then
                    Integer.TryParse(line.Substring(6), width)
                ElseIf line.StartsWith("height=") Then
                    Integer.TryParse(line.Substring(7), height)
                End If
            Next

            ' Update resolution
            If txtResolution IsNot Nothing Then
                If width > 0 AndAlso height > 0 Then
                    txtResolution.Text = String.Format("{0} x {1}", width, height)
                Else
                    txtResolution.Text = "Unknown"
                End If
            End If

            ' Update duration
            If txtDuration IsNot Nothing Then
                If videoDuration > 0 Then
                    txtDuration.Text = FormatTime(videoDuration)
                Else
                    txtDuration.Text = "Unknown"
                End If
            End If

        Catch ex As Exception
            If txtResolution IsNot Nothing Then
                txtResolution.Text = "Unknown"
            End If
            If txtDuration IsNot Nothing Then
                txtDuration.Text = "Unknown"
            End If
        End Try
    End Sub

    ' ==================== FORMAT FILE SIZE ====================
    Private Function FormatFileSize(ByVal bytes As Long) As String
        If bytes >= 1073741824 Then
            Return String.Format("{0:F2} GB", bytes / 1073741824)
        ElseIf bytes >= 1048576 Then
            Return String.Format("{0:F2} MB", bytes / 1048576)
        ElseIf bytes >= 1024 Then
            Return String.Format("{0:F2} KB", bytes / 1024)
        Else
            Return String.Format("{0} Bytes", bytes)
        End If
    End Function

    ' ==================== DRAG & DROP METHODS ====================

    ' Enable drag & drop recursively on all controls
    Private Sub EnableDragDropRecursively(ByVal parent As Control)
        For Each ctrl As Control In parent.Controls
            ctrl.AllowDrop = True
            AddHandler ctrl.DragEnter, AddressOf Form1_DragEnter
            AddHandler ctrl.DragDrop, AddressOf Form1_DragDrop
            AddHandler ctrl.DragLeave, AddressOf Form1_DragLeave

            ' Recursively enable on child controls
            If ctrl.HasChildren Then
                EnableDragDropRecursively(ctrl)
            End If
        Next
    End Sub

    ' Drag enter event - show visual feedback
    Private Sub Form1_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
        If Not isProcessingFile AndAlso e.Data.GetDataPresent(DataFormats.FileDrop) Then
            ' Get the dropped files
            Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())

            ' Check if at least one file is a supported video file
            If files IsNot Nothing AndAlso files.Length > 0 Then
                Dim fileExt As String = Path.GetExtension(files(0)).ToLower()
                Dim supportedExts As String() = {".mp4", ".avi", ".mkv", ".mov", ".flv", ".wmv", ".ts", ".m4v", ".webm"}

                If Array.IndexOf(supportedExts, fileExt) >= 0 Then
                    e.Effect = DragDropEffects.Copy

                    ' Visual feedback - change panel color
                    If Panel1 IsNot Nothing Then
                        Panel1.BackColor = Color.FromArgb(30, 60, 30)
                    End If

                    ' Update status
                    If lblStatus IsNot Nothing Then
                        lblStatus.Text = "Status: Drop video file to load..."
                        lblStatus.ForeColor = Color.LightGreen
                    End If
                Else
                    e.Effect = DragDropEffects.None
                End If
            Else
                e.Effect = DragDropEffects.None
            End If
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    ' Drag leave event - reset visual feedback
    Private Sub Form1_DragLeave(ByVal sender As Object, ByVal e As System.EventArgs)
        ' Reset panel color
        If Panel1 IsNot Nothing Then
            Panel1.BackColor = Color.Black
        End If

        ' Reset status if not processing
        If Not isProcessingFile AndAlso lblStatus IsNot Nothing Then
            If String.IsNullOrEmpty(inputFile) Then
                lblStatus.Text = "Status: Ready - Drag & Drop video or click Browse | Press SPACE to Play/Pause"
            Else
                lblStatus.Text = "Status: Ready - Press SPACE to Play/Pause"
            End If
            lblStatus.ForeColor = Color.White
        End If
    End Sub

    ' Drag drop event - handle dropped file
    Private Sub Form1_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
        ' Reset panel color
        If Panel1 IsNot Nothing Then
            Panel1.BackColor = Color.Black
        End If

        If isProcessingFile Then
            MessageBox.Show("Please wait for the current video to finish loading...", _
                          "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())

            If files IsNot Nothing AndAlso files.Length > 0 Then
                Dim droppedFile As String = files(0)
                Dim fileExt As String = Path.GetExtension(droppedFile).ToLower()
                Dim supportedExts As String() = {".mp4", ".avi", ".mkv", ".mov", ".flv", ".wmv", ".ts", ".m4v", ".webm"}

                ' Check if file extension is supported
                If Array.IndexOf(supportedExts, fileExt) < 0 Then
                    MessageBox.Show("Unsupported file format!" & vbNewLine & vbNewLine & _
                                  "Supported formats:" & vbNewLine & _
                                  "• MP4 (.mp4)" & vbNewLine & _
                                  "• AVI (.avi)" & vbNewLine & _
                                  "• MKV (.mkv)" & vbNewLine & _
                                  "• MOV (.mov)" & vbNewLine & _
                                  "• FLV (.flv)" & vbNewLine & _
                                  "• WMV (.wmv)" & vbNewLine & _
                                  "• TS (.ts)" & vbNewLine & _
                                  "• M4V (.m4v)" & vbNewLine & _
                                  "• WebM (.webm)", _
                                  "Pongo Video Cutter - Unsupported Format", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Check if file exists
                If Not System.IO.File.Exists(droppedFile) Then
                    MessageBox.Show("The dropped file does not exist or is not accessible.", _
                                  "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' Load the dropped video file
                LoadDroppedVideo(droppedFile)
            End If
        End If
    End Sub

    ' Method to load dropped video file (same as browse functionality)
    Private Sub LoadDroppedVideo(ByVal filePath As String)
        Try
            ' Set processing flag
            isProcessingFile = True

            ' Stop preview
            StopPreview()

            ' Set input file
            inputFile = filePath
            txtInputFile.Text = inputFile

            ' Update form title
            UpdateFormTitle()

            ' Update file info
            UpdateFileInfo(inputFile)

            ' Auto-fill output file
            Dim ext As String = Path.GetExtension(inputFile)
            Dim dir As String = Path.GetDirectoryName(inputFile)
            Dim filename As String = Path.GetFileNameWithoutExtension(inputFile)
            txtOutputFile.Text = Path.Combine(dir, filename & "_cut" & ext)
            outputFile = txtOutputFile.Text

            ' Clear segments
            lstSegments.Items.Clear()
            If customTrackBar IsNot Nothing Then
                customTrackBar.ClearSegments()
            End If
            isSettingStart = False
            currentStartTime = -1
            currentEndTime = -1
            isPlayingSegment = False
            isPreviewPlaying = False

            ' Reset position
            currentPosition = 0
            videoDuration = 0

            ' Reset textbox
            txtStartTime.Text = "00:00:00"
            txtEndTime.Text = "00:00:00"

            lblStatus.Text = "Status: Loading video..."
            lblStatus.ForeColor = Color.White
            Application.DoEvents()

            ' Load video
            LoadVideo()

        Catch ex As Exception
            MessageBox.Show("Failed to load the dropped video file." & vbNewLine & vbNewLine & _
                          "Please try again or use the Browse button.", _
                          "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Reset processing flag
            isProcessingFile = False

            ' Reset panel color
            If Panel1 IsNot Nothing Then
                Panel1.BackColor = Color.Black
            End If
        End Try
    End Sub

    ' ==================== KEYBOARD SHORTCUT ====================
    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        ' Cek jika tombol SPACE ditekan
        If e.KeyCode = Keys.Space Then
            ' Cegah Space untuk mengaktifkan button yang fokus
            e.SuppressKeyPress = True

            ' Toggle play/pause
            TogglePreviewPlay()
        End If
    End Sub

    ' ==================== CUSTOM TRACKBAR EVENTS ====================
    Private Sub CustomTrackBar_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        If isUpdatingTrackbar Then Return

        Try
            If videoDuration > 0 Then
                currentPosition = (customTrackBar.Value / 1000.0) * videoDuration

                ' Seek video
                If videoPlayer IsNot Nothing Then
                    videoPlayer.Seek(currentPosition)
                End If
            End If
        Catch ex As Exception
            ' Ignore seek errors during initialization
        End Try
    End Sub

    Private Sub CustomTrackBar_Scroll(ByVal sender As Object, ByVal e As EventArgs)
        ' This is called when user finishes dragging
    End Sub

    Private Sub CustomTrackBar_SegmentClicked(ByVal sender As Object, ByVal segmentIndex As Integer)
        If segmentIndex >= 0 AndAlso segmentIndex < lstSegments.Items.Count Then
            ' Set selected index without triggering play (use flag)
            isUpdatingSegmentUI = True
            lstSegments.SelectedIndex = segmentIndex
            isUpdatingSegmentUI = False

            Dim seg As VideoSegment = DirectCast(lstSegments.Items(segmentIndex), VideoSegment)
            txtStartTime.Text = FormatTime(seg.StartTime)
            txtEndTime.Text = FormatTime(seg.EndTime)

            ' Update trackbar selection
            If customTrackBar IsNot Nothing Then
                customTrackBar.SelectedSegmentIndexValue = segmentIndex
            End If

            ' Play the clicked segment directly
            PlaySegment(segmentIndex)

            lblStatus.Text = String.Format("Status: Segment {0} selected and playing", segmentIndex + 1)
        End If
    End Sub

    Private Sub CustomTrackBar_SegmentChanged(ByVal sender As Object, ByVal segmentIndex As Integer, ByVal startValue As Double, ByVal endValue As Double)
        If segmentIndex >= 0 AndAlso segmentIndex < lstSegments.Items.Count Then
            ' Set flag to prevent triggering SelectedIndexChanged
            isUpdatingSegmentUI = True

            ' Update segment in listbox
            Dim seg As VideoSegment = DirectCast(lstSegments.Items(segmentIndex), VideoSegment)
            seg.StartTime = (startValue / 1000.0) * videoDuration
            seg.EndTime = (endValue / 1000.0) * videoDuration
            lstSegments.Items(segmentIndex) = seg

            ' Reset flag
            isUpdatingSegmentUI = False

            ' Update textbox
            txtStartTime.Text = FormatTime(seg.StartTime)
            txtEndTime.Text = FormatTime(seg.EndTime)

            lblStatus.Text = String.Format("Status: Segment {0} updated", segmentIndex + 1)

            ' If currently playing this segment, update playback boundaries
            If isPlayingSegment AndAlso segmentIndex = lstSegments.SelectedIndex Then
                segmentPlayStart = seg.StartTime
                segmentPlayEnd = seg.EndTime

                ' Adjust position if outside segment boundaries
                If currentPosition < segmentPlayStart Then
                    currentPosition = segmentPlayStart
                    If videoPlayer IsNot Nothing Then
                        videoPlayer.Seek(currentPosition)
                    End If
                ElseIf currentPosition > segmentPlayEnd Then
                    currentPosition = segmentPlayEnd
                    If videoPlayer IsNot Nothing Then
                        videoPlayer.Seek(currentPosition)
                    End If
                End If
            End If
        End If
    End Sub

    ' ==================== BROWSE INPUT FILE ====================
    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Me.ActiveControl = Nothing
        ' Stop preview
        StopPreview()

        OpenFileDialog1.Title = "Select Video File"
        OpenFileDialog1.Filter = "Video Files|*.mp4;*.avi;*.mkv;*.mov;*.flv;*.wmv;*.ts;*.m4v;*.webm|All Files|*.*"
        OpenFileDialog1.FileName = ""

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            inputFile = OpenFileDialog1.FileName
            txtInputFile.Text = inputFile
            ' Update form title
            UpdateFormTitle()
            ' Update file info
            UpdateFileInfo(inputFile)

            ' Auto-fill output file
            Dim ext As String = Path.GetExtension(inputFile)
            Dim dir As String = Path.GetDirectoryName(inputFile)
            Dim filename As String = Path.GetFileNameWithoutExtension(inputFile)
            txtOutputFile.Text = Path.Combine(dir, filename & "_cut" & ext)
            outputFile = txtOutputFile.Text

            ' Clear segments
            lstSegments.Items.Clear()
            If customTrackBar IsNot Nothing Then
                customTrackBar.ClearSegments()
            End If
            isSettingStart = False
            currentStartTime = -1
            currentEndTime = -1
            isPlayingSegment = False
            isPreviewPlaying = False

            ' Reset position
            currentPosition = 0
            videoDuration = 0

            ' Reset textbox
            txtStartTime.Text = "00:00:00"
            txtEndTime.Text = "00:00:00"

            lblStatus.Text = "Status: Loading video..."
            Application.DoEvents()

            ' Load video
            LoadVideo()
        End If
    End Sub

    ' ==================== LOAD VIDEO ====================
    Private Sub LoadVideo()
        Try
            ' Ensure Panel1 handle is created
            If Not Panel1.IsHandleCreated Then
                Panel1.CreateControl()
            End If

            ' Create VideoPlayer if not exists
            If videoPlayer Is Nothing Then
                videoPlayer = New VideoPlayer()
                videoPlayer.Dock = DockStyle.Fill
                videoPlayer.Visible = True
                Panel1.Controls.Add(videoPlayer)
            End If

            ' Ensure VideoPlayer handle is created
            If Not videoPlayer.IsHandleCreated Then
                videoPlayer.CreateControl()
            End If

            ' Ensure VideoPlayer is visible
            videoPlayer.Visible = True
            videoPlayer.BringToFront()

            ' Update UI
            Panel1.Refresh()
            Application.DoEvents()

            ' Load video
            Dim loadSuccess As Boolean = videoPlayer.LoadVideo(inputFile)
            UpdateFormTitle()
            ' Get duration from ffprobe
            GetDurationFromFFprobe()

            ' Update UI
            currentPosition = 0
            txtStartTime.Text = "00:00:00"
            txtEndTime.Text = FormatTime(videoDuration)

            If customTrackBar IsNot Nothing Then
                customTrackBar.Value = 0
            End If

            ' Update duration textbox
            If txtDuration IsNot Nothing Then
                If videoDuration > 0 Then
                    txtDuration.Text = FormatTime(videoDuration)
                Else
                    txtDuration.Text = "Unknown"
                End If
            End If

            ' Update resolution from ffprobe
            GetVideoInfoFromFFprobe(inputFile)

            If loadSuccess Then
                lblStatus.Text = "Status: Video loaded - " & Path.GetFileName(inputFile) & " | Press SPACE to Play/Pause"
            Else
                lblStatus.Text = "Status: Video loaded (preview may not be available)"
            End If

            ' Create automatic segment
            'CreateAutoSegment()
            AutoPlayVideo()
        Catch ex As Exception
            lblStatus.Text = "Status: Error loading video"
            MessageBox.Show("Unable to load the selected video file." & vbNewLine & vbNewLine & _
                          "The file may be corrupted or in an unsupported format." & vbNewLine & _
                          "Please try another video file.", _
                          "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    ' ===== Method untuk auto play video tanpa membuat segmen =====
    Private Sub AutoPlayVideo()
        Try
            ' Ensure videoDuration is valid
            If videoDuration <= 0 Then
                Return
            End If

            ' Reset state
            isSettingStart = False
            currentStartTime = -1
            currentEndTime = -1

            ' Update textbox - kosongkan karena belum ada segmen
            txtStartTime.Text = "00:00:00"
            txtEndTime.Text = "00:00:00"

            ' Auto play
            System.Threading.Thread.Sleep(200)

            ' Set position to beginning
            currentPosition = 0
            If videoPlayer IsNot Nothing Then
                videoPlayer.Seek(0)
                Application.DoEvents()
                System.Threading.Thread.Sleep(100)
            End If

            ' Start playback
            isPreviewPlaying = True
            isPlayingSegment = False ' Normal playback, not segment playback

            If videoPlayer IsNot Nothing Then
                videoPlayer.Play()
            End If

            Timer3.Start()
            If btnPlayPause IsNot Nothing Then
                btnPlayPause.Image = My.Resources.bpause
            End If
            lblStatus.Text = "Status: Video playing... | Click 'Set Start' to mark segment beginning | Press SPACE to Play/Pause"

        Catch ex As Exception
            ' Silently handle auto-play errors
            System.IO.File.WriteAllText(Application.StartupPath & "\autoplay_error.log", _
                "Error: " & ex.Message & vbNewLine & ex.StackTrace)
        End Try
    End Sub
    ' ===== Method to create automatic segment =====
    Private Sub CreateAutoSegment()
        Try
            ' Ensure videoDuration is valid
            If videoDuration <= 0 Then
                Return
            End If

            ' Clear existing segments
            lstSegments.Items.Clear()
            If customTrackBar IsNot Nothing Then
                customTrackBar.ClearSegments()
            End If

            ' Set start time to 0 (beginning of video)
            currentStartTime = 0
            currentEndTime = videoDuration
            isSettingStart = True

            ' Update textbox
            txtStartTime.Text = FormatTime(0)
            txtEndTime.Text = FormatTime(videoDuration)

            ' Add segment to trackbar
            If customTrackBar IsNot Nothing Then
                Dim startValue As Integer = CInt((currentStartTime / videoDuration) * 1000)
                Dim endValue As Integer = CInt((currentEndTime / videoDuration) * 1000)
                customTrackBar.AddSegment(startValue, endValue)
            End If

            ' Create segment and add to listbox
            Dim seg As New VideoSegment(currentStartTime, currentEndTime, 1)
            lstSegments.Items.Add(seg)

            ' Reset state
            isSettingStart = False
            currentStartTime = -1
            currentEndTime = -1

            ' Update status
            lblStatus.Text = "Status: Segment 1 automatically created (00:00:00 - " & FormatTime(videoDuration) & ")"

            ' Auto play
            System.Threading.Thread.Sleep(200)

            ' Set position to beginning
            currentPosition = 0
            If videoPlayer IsNot Nothing Then
                videoPlayer.Seek(0)
                Application.DoEvents()
                System.Threading.Thread.Sleep(100)
            End If

            ' Start playback
            isPreviewPlaying = True
            isPlayingSegment = False ' Normal playback, not segment playback

            If videoPlayer IsNot Nothing Then
                videoPlayer.Play()
            End If

            Timer3.Start()
            btnPlayPause.Image = My.Resources.bpause
            lblStatus.Text = "Status: Video playing automatically... | Press SPACE to Play/Pause"

        Catch ex As Exception
            ' Silently handle auto-segment errors
            System.IO.File.WriteAllText(Application.StartupPath & "\autosegment_error.log", _
                "Error: " & ex.Message & vbNewLine & ex.StackTrace)
        End Try
    End Sub

    ' ==================== GET DURATION FROM FFPROBE ====================
    Private Sub GetDurationFromFFprobe()
        Try
            Dim ffprobe As New Process()
            ffprobe.StartInfo.FileName = ffprobePath
            ffprobe.StartInfo.Arguments = String.Format( _
                "-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 ""{0}""", _
                inputFile _
            )
            ffprobe.StartInfo.UseShellExecute = False
            ffprobe.StartInfo.RedirectStandardOutput = True
            ffprobe.StartInfo.RedirectStandardError = True
            ffprobe.StartInfo.CreateNoWindow = True

            ffprobe.Start()
            Dim output As String = ffprobe.StandardOutput.ReadToEnd().Trim()
            Dim errorOutput As String = ffprobe.StandardError.ReadToEnd()
            ffprobe.WaitForExit()

            If Double.TryParse(output, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, videoDuration) Then
                ' Update duration in trackbar
                If customTrackBar IsNot Nothing Then
                    customTrackBar.VideoDuration = videoDuration
                End If

                ' Update UI with duration
                If txtEndTime IsNot Nothing Then
                    txtEndTime.Text = FormatTime(videoDuration)
                End If

                ' Update duration textbox
                If txtDuration IsNot Nothing Then
                    txtDuration.Text = FormatTime(videoDuration)
                End If
            Else
                ' Fallback if ffprobe fails
                videoDuration = 0
                MessageBox.Show("Unable to determine video duration." & vbNewLine & _
                              "The video file may be corrupted or unsupported.", _
                              "Pongo Video Cutter - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            videoDuration = 0
            System.IO.File.WriteAllText(Application.StartupPath & "\ffprobe_error.log", _
                "Error: " & ex.Message & vbNewLine & ex.StackTrace)
        End Try
    End Sub

    ' ==================== UPDATE TIMELINE ====================
    Private Sub UpdateTimeline()
        Try
            If videoPlayer IsNot Nothing AndAlso videoPlayer.IsVideoPlaying() Then
                currentPosition = videoPlayer.GetCurrentPosition()
            End If

            ' Update trackbar
            If videoDuration > 0 AndAlso customTrackBar IsNot Nothing Then
                isUpdatingTrackbar = True
                customTrackBar.Value = CInt((currentPosition / videoDuration) * 1000)
                isUpdatingTrackbar = False
            End If
        Catch ex As Exception
            ' Ignore update errors
        End Try
    End Sub

    ' ==================== PLAY/PAUSE PREVIEW ====================
    Private Sub btnPlayPause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlayPause.Click
        Me.ActiveControl = Nothing
        TogglePreviewPlay()
    End Sub

    ' ==================== TOGGLE PREVIEW PLAY ====================
    Private Sub TogglePreviewPlay()
        If String.IsNullOrEmpty(inputFile) Then
            MessageBox.Show("Please select a video file first!", "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If isPreviewPlaying Then
            ' Pause
            isPreviewPlaying = False
            If videoPlayer IsNot Nothing Then
                videoPlayer.Pause()
            End If
            Timer3.Stop()
            btnPlayPause.Image = My.Resources.bplay
            lblStatus.Text = "Status: Preview paused at " & FormatTime(currentPosition)
        Else
            ' Reset isPlayingSegment if playing from beginning
            isPlayingSegment = False

            ' Play
            If currentPosition >= videoDuration Then
                currentPosition = 0
                If videoPlayer IsNot Nothing Then
                    videoPlayer.Seek(0)
                End If
            End If

            isPreviewPlaying = True
            If videoPlayer IsNot Nothing Then
                videoPlayer.Play()
            End If
            Timer3.Start()
            btnPlayPause.Image = My.Resources.bpause
            lblStatus.Text = "Status: Preview playing..."
        End If
    End Sub

    ' ==================== STOP PREVIEW ====================
    Private Sub StopPreview()
        isPreviewPlaying = False
        isPlayingSegment = False
        Timer3.Stop()
        If videoPlayer IsNot Nothing Then
            videoPlayer.StopVideo()
        End If
        If btnPlayPause IsNot Nothing Then
            btnPlayPause.Image = My.Resources.bplay
        End If
    End Sub

    ' ==================== SET START TIME ====================
    Private Sub btnSetStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetStart.Click
        Me.ActiveControl = Nothing
        txtStartTime.Text = FormatTime(currentPosition)
        currentStartTime = currentPosition
        isSettingStart = True

        ' Show temporary marker on trackbar
        If customTrackBar IsNot Nothing AndAlso videoDuration > 0 Then
            customTrackBar.TempStartMarker = CInt((currentPosition / videoDuration) * 1000)
            customTrackBar.TempEndMarker = -1
        End If

        lblStatus.Text = "Status: Start time set to " & txtStartTime.Text & " | Now click 'Set End'"
    End Sub

    ' ==================== SET END TIME ====================
    Private Sub btnSetEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetEnd.Click
        Me.ActiveControl = Nothing
        If Not isSettingStart OrElse currentStartTime < 0 Then
            MessageBox.Show("Please click 'Set Start' first to mark the beginning of the segment!", _
                          "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        txtEndTime.Text = FormatTime(currentPosition)
        currentEndTime = currentPosition

        ' Validate
        If currentEndTime <= currentStartTime Then
            MessageBox.Show("End time must be greater than start time!", _
                          "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Add segment to trackbar
        If customTrackBar IsNot Nothing AndAlso videoDuration > 0 Then
            Dim startValue As Integer = CInt((currentStartTime / videoDuration) * 1000)
            Dim endValue As Integer = CInt((currentEndTime / videoDuration) * 1000)
            customTrackBar.AddSegment(startValue, endValue)
        End If

        ' Create segment and add to listbox
        Dim seg As New VideoSegment(currentStartTime, currentEndTime, lstSegments.Items.Count + 1)
        lstSegments.Items.Add(seg)

        ' Reset
        isSettingStart = False
        currentStartTime = -1
        currentEndTime = -1

        ' Update status
        lblStatus.Text = String.Format("Status: Segment {0} added! Click 'Set Start' to create another segment", lstSegments.Items.Count)
    End Sub

    ' ==================== REMOVE SEGMENT ====================
    Private Sub btnRemoveSegment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveSegment.Click
        Me.ActiveControl = Nothing
        If lstSegments.SelectedIndex >= 0 Then
            Dim index As Integer = lstSegments.SelectedIndex
            lstSegments.Items.RemoveAt(index)
            If customTrackBar IsNot Nothing Then
                customTrackBar.RemoveSegment(index)
            End If

            ' Update index for remaining segments
            For i As Integer = 0 To lstSegments.Items.Count - 1
                Dim seg As VideoSegment = DirectCast(lstSegments.Items(i), VideoSegment)
                seg.SegmentIndex = i + 1
                lstSegments.Items(i) = seg
            Next

            lblStatus.Text = "Status: Segment removed - Total: " & lstSegments.Items.Count & " segments"
        Else
            MessageBox.Show("Please select a segment to remove!", _
                          "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' ==================== CLEAR SEGMENTS ====================
    Private Sub btnClearSegments_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearSegments.Click
        Me.ActiveControl = Nothing
        lstSegments.Items.Clear()
        If customTrackBar IsNot Nothing Then
            customTrackBar.ClearSegments()
        End If
        isSettingStart = False
        currentStartTime = -1
        currentEndTime = -1
        lblStatus.Text = "Status: All segments cleared"
    End Sub

    ' ==================== SAVE OUTPUT FILE ====================
    Private Sub btnSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveAs.Click
        Me.ActiveControl = Nothing
        If String.IsNullOrEmpty(inputFile) Then
            MessageBox.Show("Please select an input video file first!", _
                          "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        SaveFileDialog1.Title = "Save Video As"
        Dim ext As String = Path.GetExtension(inputFile)
        SaveFileDialog1.Filter = "Video Files|*" & ext & "|MP4|*.mp4|MKV|*.mkv|AVI|*.avi|All Files|*.*"
        SaveFileDialog1.FileName = Path.GetFileNameWithoutExtension(inputFile) & "_cut" & ext

        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            outputFile = SaveFileDialog1.FileName
            txtOutputFile.Text = outputFile
            lblStatus.Text = "Status: Output file set"
        End If
    End Sub

    ' ==================== CUT VIDEO (MULTI SEGMENT) ====================
    Private Sub btnCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCut.Click
        Me.ActiveControl = Nothing
        If isPreviewPlaying Then
            TogglePreviewPlay()
        End If

        If String.IsNullOrEmpty(inputFile) Then
            MessageBox.Show("Please select an input video file first!", _
                          "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Check if there are segments in the list
        If lstSegments.Items.Count = 0 Then
            MessageBox.Show("Please add at least 1 segment to cut!" & vbNewLine & vbNewLine & _
                          "Steps:" & vbNewLine & _
                          "1. Click 'Set Start' at the beginning position" & vbNewLine & _
                          "2. Click 'Set End' at the ending position", _
                          "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Confirmation
        Dim mode As String = IIf(chkMergeOutput.Checked, "Merge into 1 file", "Split per segment")
        Dim result As DialogResult = MessageBox.Show("Process " & lstSegments.Items.Count & " segments?" & vbNewLine & vbNewLine & _
            "Mode: " & mode & vbNewLine & _
            "Input: " & Path.GetFileName(inputFile), _
            "Pongo Video Cutter - Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.No Then Return

        ' Process all segments
        ProcessAllSegments()
    End Sub

    ' ==================== PROCESS ALL SEGMENTS ====================
    ' ==================== PROCESS ALL SEGMENTS ====================
    Private Sub ProcessAllSegments()
        Try
            ' Disable buttons
            SetControlsEnabled(False)

            ' Setup progress bar
            ProgressBar1.Style = ProgressBarStyle.Continuous
            ProgressBar1.Minimum = 0
            ProgressBar1.Maximum = lstSegments.Items.Count
            ProgressBar1.Value = 0

            Dim segmentFiles As New List(Of String)()
            Dim ext As String = Path.GetExtension(inputFile)
            Dim baseDir As String = Path.GetDirectoryName(inputFile)
            Dim baseName As String = Path.GetFileNameWithoutExtension(inputFile)

            ' Determine output file if not set
            If String.IsNullOrEmpty(outputFile) Then
                If chkMergeOutput.Checked OrElse lstSegments.Items.Count > 1 Then
                    outputFile = Path.Combine(baseDir, baseName & "_merged" & ext)
                Else
                    outputFile = Path.Combine(baseDir, baseName & "_cut" & ext)
                End If
                txtOutputFile.Text = outputFile
            End If

            ' ===== SCENARIO 1: SINGLE SEGMENT WITHOUT MERGE =====
            If lstSegments.Items.Count = 1 AndAlso Not chkMergeOutput.Checked Then
                Dim seg As VideoSegment = DirectCast(lstSegments.Items(0), VideoSegment)

                lblStatus.Text = "Status: Processing 1 segment..."
                Application.DoEvents()

                ' Format time
                Dim startTimeStr As String = FormatTimeForFFmpeg(seg.StartTime)
                Dim duration As Double = seg.EndTime - seg.StartTime
                Dim durationStr As String = FormatTimeForFFmpeg(duration)

                ' FFmpeg command to cut directly to output file
                Dim args As String = String.Format( _
                    "-ss {0} -i ""{1}"" -t {2} -c copy -avoid_negative_ts make_zero -y ""{3}""", _
                    startTimeStr, _
                    inputFile, _
                    durationStr, _
                    outputFile _
                )

                ' Execute FFmpeg
                If Not ExecuteFFmpegSilent(args) Then
                    Throw New Exception("Failed to process segment")
                End If

                ProgressBar1.Value = 1
                lblStatus.Text = "Status: COMPLETE!"

                MessageBox.Show("Video cut successfully!" & vbNewLine & vbNewLine & _
                              "Output: " & outputFile, _
                              "Pongo Video Cutter - Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Buka folder lokasi file output
                OpenOutputFolder(outputFile)
                Return
            End If

            ' ===== SCENARIO 2 & 3: MULTIPLE SEGMENTS (MERGE OR NOT) =====
            ' Process each segment to temporary file
            For i As Integer = 0 To lstSegments.Items.Count - 1
                Dim seg As VideoSegment = DirectCast(lstSegments.Items(i), VideoSegment)

                lblStatus.Text = String.Format("Status: Processing segment {0}/{1}...", i + 1, lstSegments.Items.Count)
                Application.DoEvents()

                ' Create temporary file name for each segment
                Dim segFile As String = Path.Combine(baseDir, String.Format("{0}_seg{1:00}_temp{2}", baseName, i + 1, ext))
                segmentFiles.Add(segFile)

                ' Format time
                Dim startTimeStr As String = FormatTimeForFFmpeg(seg.StartTime)
                Dim duration As Double = seg.EndTime - seg.StartTime
                Dim durationStr As String = FormatTimeForFFmpeg(duration)

                ' FFmpeg command to cut segment to temporary file
                Dim args As String = String.Format( _
                    "-ss {0} -i ""{1}"" -t {2} -c copy -avoid_negative_ts make_zero -y ""{3}""", _
                    startTimeStr, _
                    inputFile, _
                    durationStr, _
                    segFile _
                )

                ' Execute FFmpeg
                If Not ExecuteFFmpegSilent(args) Then
                    Throw New Exception("Failed to process segment " & (i + 1))
                End If

                ' Check if temporary file was created
                If Not System.IO.File.Exists(segFile) Then
                    Throw New Exception("Segment " & (i + 1) & " file was not found after processing")
                End If

                ProgressBar1.Value = i + 1
                Application.DoEvents()
            Next

            ' ===== SCENARIO 2: MULTIPLE SEGMENTS WITH MERGE =====
            If chkMergeOutput.Checked Then
                lblStatus.Text = "Status: Merging segments..."
                Application.DoEvents()

                ' Create file list for concat
                Dim listFile As String = Path.Combine(baseDir, "filelist.txt")
                Dim sb As New System.Text.StringBuilder()

                For Each file As String In segmentFiles
                    Dim safeFile As String = file.Replace("'", "'\''")
                    sb.AppendLine("file '" & safeFile & "'")
                Next

                System.IO.File.WriteAllText(listFile, sb.ToString())

                ' FFmpeg command to merge
                Dim mergeArgs As String = String.Format( _
                    "-f concat -safe 0 -i ""{0}"" -c copy -y ""{1}""", _
                    listFile, _
                    outputFile _
                )

                Dim mergeSuccess As Boolean = ExecuteFFmpegSilent(mergeArgs)

                ' Delete list file
                If System.IO.File.Exists(listFile) Then
                    System.IO.File.Delete(listFile)
                End If

                ' Delete temporary segment files
                For Each file As String In segmentFiles
                    If System.IO.File.Exists(file) Then
                        System.IO.File.Delete(file)
                    End If
                Next

                If Not mergeSuccess Then
                    Throw New Exception("Failed to merge segments")
                End If

                ' Check if output file was created
                If Not System.IO.File.Exists(outputFile) Then
                    Throw New Exception("Output file not found after merge process")
                End If

            Else
                ' ===== SCENARIO 3: MULTIPLE SEGMENTS WITHOUT MERGE =====
                ' Rename temporary files to final files
                For i As Integer = 0 To segmentFiles.Count - 1
                    Dim finalFile As String = Path.Combine(baseDir, String.Format("{0}_seg{1:00}{2}", baseName, i + 1, ext))

                    ' Delete final file if exists
                    If System.IO.File.Exists(finalFile) Then
                        System.IO.File.Delete(finalFile)
                    End If

                    ' Rename temporary file to final
                    If System.IO.File.Exists(segmentFiles(i)) Then
                        System.IO.File.Move(segmentFiles(i), finalFile)
                    End If
                Next

                ' Set output file to first file
                outputFile = Path.Combine(baseDir, String.Format("{0}_seg{1:00}{2}", baseName, 1, ext))
                txtOutputFile.Text = outputFile
            End If

            ProgressBar1.Value = ProgressBar1.Maximum
            lblStatus.Text = "Status: COMPLETE!"

            ' Show success message based on mode
            If chkMergeOutput.Checked Then
                MessageBox.Show("All segments merged successfully!" & vbNewLine & vbNewLine & _
                              "Total segments: " & lstSegments.Items.Count & vbNewLine & _
                              "Output: " & outputFile, _
                              "Pongo Video Cutter - Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Buka folder lokasi file output
                OpenOutputFolder(outputFile)
            Else
                Dim fileNames As New System.Text.StringBuilder()
                For Each file As String In segmentFiles
                    Dim finalName As String = file.Replace("_temp", "")
                    fileNames.AppendLine(Path.GetFileName(finalName))
                Next

                MessageBox.Show("All segments cut successfully!" & vbNewLine & vbNewLine & _
                              "Total segments: " & lstSegments.Items.Count & vbNewLine & vbNewLine & _
                              "Output files:" & vbNewLine & _
                              fileNames.ToString(), _
                              "Pongo Video Cutter - Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Buka folder lokasi file output
                OpenOutputFolder(outputFile)
            End If

        Catch ex As Exception
            lblStatus.Text = "Status: ERROR!"
            MessageBox.Show("An error occurred while processing the video." & vbNewLine & vbNewLine & _
                          "Please check that:" & vbNewLine & _
                          "• The input file is valid and not corrupted" & vbNewLine & _
                          "• You have enough disk space" & vbNewLine & _
                          "• The output directory is writable" & vbNewLine & vbNewLine & _
                          "If the problem persists, try reinstalling FFmpeg.", _
                          "Pongo Video Cutter - Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Reset progress bar ke 0
            If ProgressBar1 IsNot Nothing Then
                ProgressBar1.Value = 0
            End If

            ' Aktifkan kembali semua control
            SetControlsEnabled(True)

            ' Reset cursor
            Me.Cursor = Cursors.Default
        End Try
    End Sub

   

    ' ==================== EXECUTE FFMPEG SILENT ====================
    Private Function ExecuteFFmpegSilent(ByVal args As String) As Boolean
        Try
            Dim ffmpeg As New Process()
            ffmpeg.StartInfo.FileName = ffmpegPath
            ffmpeg.StartInfo.Arguments = args
            ffmpeg.StartInfo.UseShellExecute = False
            ffmpeg.StartInfo.CreateNoWindow = True
            ffmpeg.StartInfo.RedirectStandardError = True
            ffmpeg.StartInfo.RedirectStandardOutput = True
            ffmpeg.StartInfo.WorkingDirectory = Application.StartupPath

            ffmpeg.Start()

            ' Read output asynchronously to prevent deadlock
            Dim errorOutput As String = ffmpeg.StandardError.ReadToEnd()
            Dim standardOutput As String = ffmpeg.StandardOutput.ReadToEnd()

            ffmpeg.WaitForExit()

            ' Log error if any (for debugging)
            If ffmpeg.ExitCode <> 0 Then
                System.IO.File.WriteAllText(Application.StartupPath & "\ffmpeg_error.log", _
                    "Command: " & args & vbNewLine & _
                    "Error: " & errorOutput & vbNewLine & _
                    "Output: " & standardOutput)
            End If

            Return ffmpeg.ExitCode = 0

        Catch ex As Exception
            System.IO.File.WriteAllText(Application.StartupPath & "\ffmpeg_exception.log", _
                "Exception: " & ex.Message & vbNewLine & _
                "Stack: " & ex.StackTrace)
            Return False
        End Try
    End Function

    ' ==================== SET CONTROLS ENABLED ====================
    ' ==================== SET CONTROLS ENABLED ====================
    Private Sub SetControlsEnabled(ByVal enabled As Boolean)
        ' Nonaktifkan semua button
        btnCut.Enabled = enabled
        btnBrowse.Enabled = enabled
        btnSaveAs.Enabled = enabled
        btnSetStart.Enabled = enabled
        btnSetEnd.Enabled = enabled
        btnPlayPause.Enabled = enabled
        btnRemoveSegment.Enabled = enabled
        btnClearSegments.Enabled = enabled

        ' Nonaktifkan trackbar
        If customTrackBar IsNot Nothing Then
            customTrackBar.Enabled = enabled
        End If

        ' Nonaktifkan checkbox
        chkMergeOutput.Enabled = enabled

        ' Nonaktifkan listbox segments
        If lstSegments IsNot Nothing Then
            lstSegments.Enabled = enabled
        End If

        ' Nonaktifkan textbox input/output
        If txtInputFile IsNot Nothing Then
            txtInputFile.Enabled = enabled
        End If

        If txtOutputFile IsNot Nothing Then
            txtOutputFile.Enabled = enabled
        End If

        ' Nonaktifkan textbox start/end time
        If txtStartTime IsNot Nothing Then
            txtStartTime.Enabled = enabled
        End If

        If txtEndTime IsNot Nothing Then
            txtEndTime.Enabled = enabled
        End If

        ' Nonaktifkan textbox file info
        If txtFilename IsNot Nothing Then
            txtFilename.Enabled = enabled
        End If

        If txtFilesize IsNot Nothing Then
            txtFilesize.Enabled = enabled
        End If

        If txtResolution IsNot Nothing Then
            txtResolution.Enabled = enabled
        End If

        If txtDuration IsNot Nothing Then
            txtDuration.Enabled = enabled
        End If

        ' Nonaktifkan zoom buttons
        If zoomInButton IsNot Nothing Then
            zoomInButton.Enabled = enabled
        End If

        If zoomOutButton IsNot Nothing Then
            zoomOutButton.Enabled = enabled
        End If

        If ZoomResetButton IsNot Nothing Then
            ZoomResetButton.Enabled = enabled
        End If

        ' Nonaktifkan menu items jika ada
        If ZoomInTimelineToolStripMenuItem IsNot Nothing Then
            ZoomInTimelineToolStripMenuItem.Enabled = enabled
        End If

        If ZoomOutTimelineToolStripMenuItem IsNot Nothing Then
            ZoomOutTimelineToolStripMenuItem.Enabled = enabled
        End If

        If ResetZoomToolStripMenuItem IsNot Nothing Then
            ResetZoomToolStripMenuItem.Enabled = enabled
        End If

        If StartTimeToolStripMenuItem IsNot Nothing Then
            StartTimeToolStripMenuItem.Enabled = enabled
        End If

        If EndTimeToolStripMenuItem IsNot Nothing Then
            EndTimeToolStripMenuItem.Enabled = enabled
        End If

        If RemoveSegmentToolStripMenuItem IsNot Nothing Then
            RemoveSegmentToolStripMenuItem.Enabled = enabled
        End If

        If ClearAllSegmentToolStripMenuItem IsNot Nothing Then
            ClearAllSegmentToolStripMenuItem.Enabled = enabled
        End If

        If OpenVideoFilesToolStripMenuItem IsNot Nothing Then
            OpenVideoFilesToolStripMenuItem.Enabled = enabled
        End If

        If ExportToolStripMenuItem IsNot Nothing Then
            ExportToolStripMenuItem.Enabled = enabled
        End If

        ' Nonaktifkan drag & drop saat proses
        Me.AllowDrop = enabled

        ' Update cursor
        If enabled Then
            Me.Cursor = Cursors.Default
        Else
            Me.Cursor = Cursors.WaitCursor
        End If
    End Sub

    ' ==================== FORMAT TIME ====================
    Private Function FormatTime(ByVal seconds As Double) As String
        Dim ts As TimeSpan = TimeSpan.FromSeconds(seconds)
        Return String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds)
    End Function

    Private Function FormatTimeForFFmpeg(ByVal seconds As Double) As String
        Dim time As TimeSpan = TimeSpan.FromSeconds(seconds)
        Return String.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds)
    End Function

    ' ==================== OPEN OUTPUT FOLDER ====================
    Private Sub OpenOutputFolder(ByVal filePath As String)
        Try
            If String.IsNullOrEmpty(filePath) Then
                Return
            End If

            ' Dapatkan folder dari file output
            Dim folderPath As String = Path.GetDirectoryName(filePath)

            ' Cek jika folder exists
            If System.IO.Directory.Exists(folderPath) Then
                ' Buka folder menggunakan Windows Explorer
                Process.Start("explorer.exe", """" & folderPath & """")
            Else
                ' Jika folder tidak ada, coba buka folder dari input file
                Dim inputFolder As String = Path.GetDirectoryName(inputFile)
                If System.IO.Directory.Exists(inputFolder) Then
                    Process.Start("explorer.exe", """" & inputFolder & """")
                End If
            End If

        Catch ex As Exception
            ' Silently handle folder opening errors
            System.IO.File.WriteAllText(Application.StartupPath & "\folderopen_error.log", _
                "Error: " & ex.Message & vbNewLine & ex.StackTrace)
        End Try
    End Sub

    ' ==================== CLEANUP ====================
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        StopPreview()

        If videoPlayer IsNot Nothing Then
            videoPlayer.Cleanup()
            videoPlayer.Dispose()
            videoPlayer = Nothing
        End If

        ' Delete list file if exists
        Dim listFile As String = Application.StartupPath & "\filelist.txt"
        If System.IO.File.Exists(listFile) Then
            System.IO.File.Delete(listFile)
        End If
        Application.Exit()
    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        If isPreviewPlaying Then
            ' Pause temporarily if dragging segment
            If customTrackBar IsNot Nothing AndAlso customTrackBar.IsDraggingSegment Then
                Return
            End If

            UpdateTimeline()

            ' Check if playing segment
            If isPlayingSegment Then
                ' Check if reached segment end
                If currentPosition >= segmentPlayEnd Then
                    StopPreview()
                    isPlayingSegment = False
                    lblStatus.Text = String.Format("Status: Segment playback finished")

                    ' Return to segment end position
                    currentPosition = segmentPlayEnd

                    If customTrackBar IsNot Nothing Then
                        customTrackBar.Value = CInt((currentPosition / videoDuration) * 1000)
                    End If
                End If
            Else
                ' Check if playback finished (normal playback)
                If currentPosition >= videoDuration AndAlso videoDuration > 0 Then
                    StopPreview()
                    lblStatus.Text = "Status: Preview finished"

                    ' Reset position to beginning for easy replay
                    currentPosition = 0
                    If customTrackBar IsNot Nothing Then
                        customTrackBar.Value = 0
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub lblStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStatus.Click

    End Sub

    Private Sub lstSegments_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstSegments.SelectedIndexChanged
        If isUpdatingSegmentUI Then Return

        If lstSegments.SelectedIndex >= 0 AndAlso lstSegments.SelectedIndex < lstSegments.Items.Count Then
            ' Update trackbar selection
            If customTrackBar IsNot Nothing Then
                customTrackBar.SelectedSegmentIndexValue = lstSegments.SelectedIndex
            End If

            ' Play selected segment only if not from trackbar click
            If Not isUpdatingSegmentUI Then
                PlaySegment(lstSegments.SelectedIndex)
            End If
        End If
    End Sub

    Private Sub PlaySegment(ByVal segmentIndex As Integer)
        If segmentIndex < 0 OrElse segmentIndex >= lstSegments.Items.Count Then
            Return
        End If

        If String.IsNullOrEmpty(inputFile) Then
            MessageBox.Show("Please select a video file first!", "Pongo Video Cutter - Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Stop current preview
        If isPreviewPlaying Then
            isPreviewPlaying = False
            isPlayingSegment = False
            Timer3.Stop()
            If videoPlayer IsNot Nothing Then
                videoPlayer.Pause()
            End If
            Application.DoEvents()
            System.Threading.Thread.Sleep(50)
        End If

        ' Get segment
        Dim seg As VideoSegment = DirectCast(lstSegments.Items(segmentIndex), VideoSegment)

        ' Set position to segment start
        currentPosition = seg.StartTime
        segmentPlayStart = seg.StartTime
        segmentPlayEnd = seg.EndTime

        ' Update UI without triggering events
        isUpdatingTrackbar = True
        If customTrackBar IsNot Nothing Then
            customTrackBar.Value = CInt((currentPosition / videoDuration) * 1000)
        End If
        isUpdatingTrackbar = False

        ' Seek to start position
        If videoPlayer IsNot Nothing Then
            videoPlayer.Seek(currentPosition)
            Application.DoEvents()
            System.Threading.Thread.Sleep(100)
        End If

        ' Start playback
        isPreviewPlaying = True
        isPlayingSegment = True

        If videoPlayer IsNot Nothing Then
            videoPlayer.Play()
        End If

        Timer3.Start()
        btnPlayPause.Image = My.Resources.bpause
        lblStatus.Text = String.Format("Status: Playing Segment {0} ({1} - {2})", segmentIndex + 1, FormatTime(seg.StartTime), FormatTime(seg.EndTime))
    End Sub

    Private Sub zoomInButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles zoomInButton.Click
        Me.ActiveControl = Nothing
        If customTrackBar IsNot Nothing Then
            customTrackBar.ZoomIn()
        End If
    End Sub

    Private Sub zoomOutButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles zoomOutButton.Click
        Me.ActiveControl = Nothing
        If customTrackBar IsNot Nothing Then
            customTrackBar.ZoomOut()
        End If
    End Sub

    Private Sub ZoomResetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ZoomResetButton.Click
        Me.ActiveControl = Nothing
        If customTrackBar IsNot Nothing Then
            customTrackBar.ResetZoom()
        End If
    End Sub

    Private Sub ZoomInTimelineToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ZoomInTimelineToolStripMenuItem.Click
        zoomInButton.PerformClick()
    End Sub

    Private Sub ZoomOutTimelineToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ZoomOutTimelineToolStripMenuItem.Click
        zoomOutButton.PerformClick()
    End Sub

    Private Sub ResetZoomToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetZoomToolStripMenuItem.Click
        ZoomResetButton.PerformClick()
    End Sub

    Private Sub StartTimeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartTimeToolStripMenuItem.Click
        btnSetStart.PerformClick()
    End Sub

    Private Sub EndTimeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EndTimeToolStripMenuItem.Click
        btnSetEnd.PerformClick()
    End Sub

    Private Sub RemoveSegmentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveSegmentToolStripMenuItem.Click
        btnRemoveSegment.PerformClick()
    End Sub

    Private Sub ClearAllSegmentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearAllSegmentToolStripMenuItem.Click
        btnClearSegments.PerformClick()
    End Sub

    Private Sub OpenVideoFilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenVideoFilesToolStripMenuItem.Click
        btnBrowse.PerformClick()
    End Sub

    Private Sub ExportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportToolStripMenuItem.Click
        btnCut.PerformClick()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            RadioButton1.ForeColor = Color.Yellow
        Else
            RadioButton1.ForeColor = Color.White
        End If
    End Sub

    Private Sub chkMergeOutput_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMergeOutput.CheckedChanged
        If chkMergeOutput.Checked Then
            chkMergeOutput.ForeColor = Color.Yellow
        Else
            chkMergeOutput.ForeColor = Color.White
        End If
    End Sub

    Private Sub FFmpegPathToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FFmpegPathToolStripMenuItem.Click
        FFmpeg_Path.TextBox1.Text = ffmpegPath
        FFmpeg_Path.TextBox2.Text = ffprobePath
        FFmpeg_Path.ShowDialog()
    End Sub


    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        Process.Start("https://www.ffmpeg.org/legal.html")
    End Sub

    Private Sub PanelLeftToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLeftToolStripMenuItem.Click
        If PanelLeftToolStripMenuItem.Checked Then
            Panel2.Visible = False
            PanelLeftToolStripMenuItem.Checked = False
        Else
            Panel2.Visible = True
            PanelLeftToolStripMenuItem.Checked = True
        End If
    End Sub

    Private Sub PanelRightToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelRightToolStripMenuItem.Click
        If PanelRightToolStripMenuItem.Checked Then
            Panel3.Visible = False
            PanelRightToolStripMenuItem.Checked = False
        Else
            Panel3.Visible = True
            PanelRightToolStripMenuItem.Checked = True
        End If
    End Sub

    Private Sub DonateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DonateToolStripMenuItem.Click
        Try
            Process.Start("https://pongo.my.id/donate.htm")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub SendFeedbackToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendFeedbackToolStripMenuItem.Click
        Try
            Process.Start("mailto:arisohandriputra@gmail.com")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
   ' ==================== GET METADATA FROM FFPROBE ====================
    Private Function GetVideoMetadata(ByVal filePath As String) As String
        Try
            Dim ffprobe As New Process()
            ffprobe.StartInfo.FileName = ffprobePath
            ffprobe.StartInfo.Arguments = String.Format( _
                "-v error -show_format -show_streams ""{0}""", _
                filePath _
            )
            ffprobe.StartInfo.UseShellExecute = False
            ffprobe.StartInfo.RedirectStandardOutput = True
            ffprobe.StartInfo.RedirectStandardError = True
            ffprobe.StartInfo.CreateNoWindow = True
            ffprobe.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8

            ffprobe.Start()
            Dim output As String = ffprobe.StandardOutput.ReadToEnd()
            Dim errorOutput As String = ffprobe.StandardError.ReadToEnd()
            ffprobe.WaitForExit()

            If String.IsNullOrEmpty(output) Then
                Return "No metadata available"
            End If

            Return ParseMetadataSimple(output)

        Catch ex As Exception
            Return "Error getting metadata: " & ex.Message
        End Try
    End Function

    ' ==================== PARSE METADATA SEDERHANA ====================
    Private Function ParseMetadataSimple(ByVal output As String) As String
        Dim sb As New System.Text.StringBuilder()

        Try
            Dim lines As String() = output.Split(New Char() {ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries)

            Dim inFormat As Boolean = False
            Dim inStream As Boolean = False
            Dim streamIndex As Integer = 0
            Dim currentStreamInfo As String = ""
            Dim bitrate As String = ""
            Dim formatName As String = ""
            Dim formatLongName As String = ""
            Dim metadataList As New List(Of String)()
            Dim streamList As New List(Of String)()

            For Each line As String In lines
                Dim cleanLine As String = line.Trim()

                ' Skip empty lines
                If cleanLine.Length = 0 Then Continue For

                ' Check section headers
                If cleanLine = "[FORMAT]" Then
                    inFormat = True
                    inStream = False
                    Continue For
                ElseIf cleanLine = "[/FORMAT]" Then
                    inFormat = False
                    Continue For
                ElseIf cleanLine = "[STREAM]" Then
                    inFormat = False
                    inStream = True
                    streamIndex += 1
                    currentStreamInfo = "Stream #" & (streamIndex - 1) & ": "
                    Continue For
                ElseIf cleanLine = "[/STREAM]" Then
                    inStream = False
                    If currentStreamInfo.Length > 0 Then
                        streamList.Add(currentStreamInfo)
                        currentStreamInfo = ""
                    End If
                    Continue For
                End If

                ' Parse format section
                If inFormat Then
                    Dim eqIndex As Integer = cleanLine.IndexOf("=")
                    If eqIndex > 0 Then
                        Dim key As String = cleanLine.Substring(0, eqIndex)
                        Dim value As String = cleanLine.Substring(eqIndex + 1)

                        ' HAPUS: duration tidak lagi diproses
                        ' If key = "duration" Then
                        '     duration = value
                        If key = "bit_rate" Then
                            bitrate = value
                        ElseIf key = "format_name" Then
                            formatName = value
                        ElseIf key = "format_long_name" Then
                            formatLongName = value
                        ElseIf key.StartsWith("TAG:") Then
                            Dim tagName As String = key.Substring(4)
                            metadataList.Add(tagName & " : " & value)
                        End If
                    End If
                End If

                ' Parse stream section
                If inStream Then
                    Dim eqIndex As Integer = cleanLine.IndexOf("=")
                    If eqIndex > 0 Then
                        Dim key As String = cleanLine.Substring(0, eqIndex)
                        Dim value As String = cleanLine.Substring(eqIndex + 1)

                        Select Case key
                            Case "codec_type"
                                currentStreamInfo &= value
                            Case "codec_name"
                                currentStreamInfo &= " (" & value & ")"
                            Case "width"
                                currentStreamInfo &= ", " & value & "x"
                            Case "height"
                                currentStreamInfo &= value
                            Case "r_frame_rate"
                                If value.Contains("/") Then
                                    Dim parts As String() = value.Split("/"c)
                                    If parts.Length = 2 Then
                                        Dim num As Double, den As Double
                                        If Double.TryParse(parts(0), num) AndAlso Double.TryParse(parts(1), den) AndAlso den > 0 Then
                                            currentStreamInfo &= ", " & (num / den).ToString("F2") & " fps"
                                        End If
                                    End If
                                End If
                            Case "channels"
                                currentStreamInfo &= ", " & value & " ch"
                            Case "sample_rate"
                                currentStreamInfo &= ", " & value & " Hz"
                            Case "bit_rate"
                                Dim rate As Integer
                                If Integer.TryParse(value, rate) Then
                                    currentStreamInfo &= ", " & FormatBitrate(rate)
                                End If
                        End Select
                    End If
                End If
            Next

            ' HAPUS: Bagian Duration tidak lagi ditambahkan
            ' Duration sudah dihapus dari sini

            ' Bitrate
            If bitrate.Length > 0 Then
                Dim rate As Integer
                If Integer.TryParse(bitrate, rate) Then
                    sb.AppendLine("Bitrate: " & FormatBitrate(rate))
                End If
            End If

            ' Format
            If formatName.Length > 0 Then
                sb.AppendLine("Format: " & formatName)
            End If

            If formatLongName.Length > 0 Then
                sb.AppendLine("Format Long Name: " & formatLongName)
            End If

            ' Metadata tags
            If metadataList.Count > 0 Then
                sb.AppendLine()
                sb.AppendLine("Metadata:")
                For Each tag As String In metadataList
                    sb.AppendLine("  " & tag)
                Next
            End If

            ' Streams
            If streamList.Count > 0 Then
                sb.AppendLine()
                sb.AppendLine("Streams:")
                For Each info As String In streamList
                    sb.AppendLine("  " & info)
                Next
            End If

            Return sb.ToString()

        Catch ex As Exception
            Return "Error parsing metadata: " & ex.Message
        End Try
    End Function
    ' ==================== FORMAT BITRATE ====================
    Private Function FormatBitrate(ByVal bitrate As Integer) As String
        If bitrate >= 1000000 Then
            Return (bitrate / 1000000).ToString("F1") & " Mb/s"
        ElseIf bitrate >= 1000 Then
            Return (bitrate / 1000).ToString("F0") & " kb/s"
        Else
            Return bitrate.ToString() & " b/s"
        End If
    End Function

    Private Sub LicenseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LicenseToolStripMenuItem.Click
        Try
            Process.Start("https://pongo.my.id/license.htm")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub AboutPongoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutPongoToolStripMenuItem.Click
        frmAbout.ShowDialog()
    End Sub

    Private Sub TutorialsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TutorialsToolStripMenuItem.Click
        Try
            Process.Start("https://pongo.my.id/tutorials.htm")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class