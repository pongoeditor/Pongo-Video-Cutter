Imports System.Runtime.InteropServices
Imports DirectShowLib
Imports System.IO
Imports System.Security.Principal

Public Class VideoPlayer
    Inherits System.Windows.Forms.Control

    ' DirectShow interfaces
    Private graphBuilder As IGraphBuilder = Nothing
    Private mediaControl As IMediaControl = Nothing
    Private mediaPosition As IMediaPosition = Nothing
    Private mediaEvent As IMediaEvent = Nothing
    Private videoWindow As IVideoWindow = Nothing
    Private basicVideo As IBasicVideo = Nothing
    Private mediaSeeking As IMediaSeeking = Nothing
    Private WithEvents logoPictureBox As New PictureBox()
    ' Variabel untuk aspect ratio
    Private videoWidth As Integer = 0
    Private videoHeight As Integer = 0
    Private aspectRatio As Double = 1.0

    Private videoFile As String = ""
    Private _isPlaying As Boolean = False
    Private hasVideo As Boolean = False
    Private _currentPosition As Double = 0
    Private _videoDuration As Double = 0

    ' Timer untuk update posisi
    Private WithEvents positionTimer As New System.Windows.Forms.Timer()

    ' Timer untuk animasi warna
    Private WithEvents colorTimer As New System.Windows.Forms.Timer()
    Private colorIndex As Integer = 0

    ' Array warna untuk tulisan
    Private colors As Color() = { _
        Color.Red, Color.Orange, Color.Yellow, Color.Green, _
        Color.Cyan, Color.Blue, Color.Magenta, Color.Pink _
    }

    ' Events
    Public Event PositionChanged(ByVal position As Double)
    Public Event PlaybackEnded()

    ' Path untuk LAV Filters
    Private lavFiltersPath As String = ""

    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.BackColor = Color.Black
        Me.Size = New Size(640, 360)

        ' ============ INISIALISASI LOGO PICTUREBOX ============
        Try
            ' Setel gambar dari resource
            logoPictureBox.Image = My.Resources.animated ' Ganti dengan nama resource Anda
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom
            logoPictureBox.BackColor = Color.Transparent
            logoPictureBox.Visible = False ' Awalnya disembunyikan
            Me.Controls.Add(logoPictureBox)
        Catch ex As Exception
            Debug.WriteLine("Failed to load animated logo: " & ex.Message)
        End Try

        ' Setup timer untuk animasi warna
        colorTimer.Interval = 300
        colorTimer.Enabled = True
        colorTimer.Start()

        ' Setup timer untuk update posisi
        positionTimer.Interval = 100
        AddHandler positionTimer.Tick, AddressOf PositionTimer_Tick

        ' Cek dan install LAV Filters jika perlu
        CheckAndInstallLAVFilters()
    End Sub

    ' Cek apakah LAV Filters sudah terdaftar
    Private Function IsLAVFiltersRegistered() As Boolean
        Try
            ' Cek CLSID LAV Splitter di registry
            Dim key As Microsoft.Win32.RegistryKey = _
                Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID\{171252A0-8820-4AFE-9DF8-5C92B2D66B04}")

            If key IsNot Nothing Then
                key.Close()
                Debug.WriteLine("LAV Filters already registered")
                Return True
            End If

            Debug.WriteLine("LAV Filters not registered")
            Return False

        Catch ex As Exception
            Debug.WriteLine("IsLAVFiltersRegistered error: " & ex.Message)
            Return False
        End Try
    End Function

    ' Cek dan install LAV Filters
    Private Sub CheckAndInstallLAVFilters()
        Try
            ' Cek apakah LAV Filters sudah terdaftar
            If IsLAVFiltersRegistered() Then
                Debug.WriteLine("LAV Filters already installed, no need to register")
                Return
            End If

            ' Cari folder LAV Filters
            Dim lavFolder As String = FindLAVFiltersFolder()

            If String.IsNullOrEmpty(lavFolder) Then
                Debug.WriteLine("LAV Filters folder not found!")
                Return
            End If

            Debug.WriteLine("LAV Filters found at: " & lavFolder)
            Debug.WriteLine("LAV Filters not registered, requesting admin rights...")

            ' Buat batch file untuk registrasi
            Dim batchFile As String = Path.Combine(Path.GetTempPath(), "register_lav_filters.bat")
            Dim sb As New System.Text.StringBuilder()

            sb.AppendLine("@echo off")
            sb.AppendLine("cd /d """ & lavFolder & """")

            ' Cari semua file .ax
            Dim axFiles As String() = Directory.GetFiles(lavFolder, "*.ax")

            If axFiles.Length = 0 Then
                Debug.WriteLine("No .ax files found in: " & lavFolder)
                Return
            End If

            ' Tambahkan perintah regsvr32 untuk setiap file
            For Each axFile As String In axFiles
                sb.AppendLine("regsvr32 /s """ & axFile & """")
                Debug.WriteLine("Will register: " & Path.GetFileName(axFile))
            Next

            sb.AppendLine("exit")

            ' Tulis batch file
            File.WriteAllText(batchFile, sb.ToString())

            ' Jalankan batch file sebagai administrator
            Dim psi As New ProcessStartInfo()
            psi.FileName = batchFile
            psi.UseShellExecute = True
            psi.Verb = "runas" ' Ini yang meminta hak administrator
            psi.WorkingDirectory = lavFolder
            psi.CreateNoWindow = False

            Debug.WriteLine("Requesting administrator privileges...")

            Try
                Dim proc As Process = Process.Start(psi)
                If proc IsNot Nothing Then
                    proc.WaitForExit()
                    proc.Close()
                    Debug.WriteLine("LAV Filters registration completed")

                    ' Hapus batch file
                    Try
                        File.Delete(batchFile)
                    Catch ex As Exception
                    End Try

                    ' Tampilkan pesan sukses
                    MessageBox.Show("Video codec (LAVFilters) successfully applied!" & vbNewLine & vbNewLine & _
                                  "Find more information at https://pongo.my.id/info.htm", _
                                  "Video codec (LAVFilters) applied", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                Debug.WriteLine("User declined admin rights: " & ex.Message)

                ' Hapus batch file
                Try
                    File.Delete(batchFile)
                Catch ex2 As Exception
                End Try

                ' Tampilkan pesan warning
                MessageBox.Show("LAV Filters belum terinstall." & vbNewLine & vbNewLine & _
                              "Untuk menginstall LAV Filters, silakan jalankan file .bat" & vbNewLine & _
                              "di folder LAVFilters sebagai Administrator.", _
                              "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try

        Catch ex As Exception
            Debug.WriteLine("CheckAndInstallLAVFilters error: " & ex.Message)
        End Try
    End Sub

    ' Cari folder LAV Filters
    Private Function FindLAVFiltersFolder() As String
        Try
            Dim possiblePaths As New List(Of String)()

            ' 1. Di folder aplikasi
            Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory
            possiblePaths.Add(appPath & "LAVFilters")
            possiblePaths.Add(appPath & "LAVFilters\x86")
            possiblePaths.Add(appPath & "LAVFilters\x64")

            ' 2. Di folder project (untuk debugging)
            Try
                Dim projectPath As String = Directory.GetParent(Directory.GetParent(appPath).FullName).FullName
                possiblePaths.Add(projectPath & "\LAVFilters")
                possiblePaths.Add(projectPath & "\LAVFilters\x86")
                possiblePaths.Add(projectPath & "\LAVFilters\x64")
            Catch ex As Exception
            End Try

            ' 3. Cari di seluruh folder aplikasi
            For Each path As String In possiblePaths
                If Directory.Exists(path) Then
                    ' Cek jika ada file .ax
                    Dim axFiles As String() = Directory.GetFiles(path, "*.ax", SearchOption.TopDirectoryOnly)
                    If axFiles.Length > 0 Then
                        Debug.WriteLine("Found LAV Filters in: " & path)
                        Return path
                    End If
                End If
            Next

            ' 4. Cari file .ax di folder aplikasi
            If Directory.Exists(appPath) Then
                Dim axFiles As String() = Directory.GetFiles(appPath, "*.ax", SearchOption.AllDirectories)
                If axFiles.Length > 0 Then
                    Dim foundFolder As String = Path.GetDirectoryName(axFiles(0))
                    Debug.WriteLine("Found .ax files in: " & foundFolder)
                    Return foundFolder
                End If
            End If

            Debug.WriteLine("LAV Filters folder not found!")
            Return ""

        Catch ex As Exception
            Debug.WriteLine("FindLAVFiltersFolder error: " & ex.Message)
            Return ""
        End Try
    End Function

    ' Load video
    Public Function LoadVideo(ByVal filePath As String) As Boolean
        Try
            Cleanup()

            If Not Me.IsHandleCreated Then
                Me.CreateControl()
            End If

            videoFile = filePath
            hasVideo = False

            ' Buat Filter Graph
            graphBuilder = DirectCast(New FilterGraph(), IGraphBuilder)

            ' Render file
            Dim hr As Integer = -1
            Try
                hr = graphBuilder.RenderFile(filePath, Nothing)
                Debug.WriteLine("RenderFile result: 0x" & hr.ToString("X8"))
            Catch ex As Exception
                Debug.WriteLine("RenderFile exception: " & ex.Message)
            End Try

            If hr < 0 Then
                Debug.WriteLine("Failed to render file: 0x" & hr.ToString("X8"))

                ' Cek jika LAV Filters belum terinstall
                If Not IsLAVFiltersRegistered() Then
                    MessageBox.Show("LAV Filters belum terinstall!" & vbNewLine & vbNewLine & _
                                  "Silakan restart aplikasi dan izinkan instalasi LAV Filters.", _
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                hasVideo = False
                Me.Invalidate()
                Return False
            End If

            ' Dapatkan interfaces
            Try
                mediaControl = DirectCast(graphBuilder, IMediaControl)
                mediaPosition = DirectCast(graphBuilder, IMediaPosition)
                mediaEvent = DirectCast(graphBuilder, IMediaEvent)
                videoWindow = DirectCast(graphBuilder, IVideoWindow)
                basicVideo = DirectCast(graphBuilder, IBasicVideo)
                mediaSeeking = DirectCast(graphBuilder, IMediaSeeking)
            Catch ex As Exception
                Debug.WriteLine("Failed to get interfaces: " & ex.Message)
            End Try

            ' Dapatkan informasi video
            GetVideoDimensions()
            GetDurationFromGraph()
            SetupVideoWindow()

            hasVideo = True
            positionTimer.Start()

            Me.Invalidate()
            Application.DoEvents()

            Debug.WriteLine("Video loaded successfully")
            Return True

        Catch ex As Exception
            Debug.WriteLine("LoadVideo error: " & ex.Message)
            hasVideo = False
            Me.Invalidate()
            Return False
        End Try
    End Function

    ' Get video dimensions
    Private Sub GetVideoDimensions()
        Try
            If basicVideo IsNot Nothing Then
                Dim width As Integer = 0
                Dim height As Integer = 0
                Dim hr As Integer = basicVideo.GetVideoSize(width, height)

                If hr >= 0 AndAlso width > 0 AndAlso height > 0 Then
                    videoWidth = width
                    videoHeight = height
                    aspectRatio = width / height
                    Debug.WriteLine("Video size: " & width & "x" & height)
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine("GetVideoDimensions error: " & ex.Message)
        End Try
    End Sub

    ' Get duration
    Private Sub GetDurationFromGraph()
        Try
            If mediaPosition IsNot Nothing Then
                Dim duration As Double = 0
                Dim hr As Integer = mediaPosition.get_Duration(duration)
                If hr >= 0 AndAlso duration > 0 Then
                    _videoDuration = duration
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' Setup video window
    Private Sub SetupVideoWindow()
        Try
            If videoWindow Is Nothing Then Return
            If Not Me.IsHandleCreated Then Me.CreateControl()

            Dim handle As IntPtr = Me.Handle
            Dim videoRect As Rectangle = CalculateVideoRect()

            videoWindow.put_Owner(handle)
            videoWindow.put_WindowStyle(WS_CHILD Or WS_CLIPCHILDREN)
            videoWindow.SetWindowPosition(videoRect.X, videoRect.Y, videoRect.Width, videoRect.Height)
            videoWindow.put_MessageDrain(handle)
            videoWindow.put_Visible(OABool.True)

            Me.Refresh()
            Application.DoEvents()

        Catch ex As Exception
            Debug.WriteLine("SetupVideoWindow error: " & ex.Message)
        End Try
    End Sub

    ' Calculate video rectangle
    Private Function CalculateVideoRect() As Rectangle
        If videoWidth = 0 OrElse videoHeight = 0 Then
            Return New Rectangle(0, 0, Me.Width, Me.Height)
        End If

        Dim controlRatio As Double = Me.Width / Me.Height
        Dim videoRatio As Double = aspectRatio

        If controlRatio > videoRatio Then
            Dim newHeight As Integer = Me.Height
            Dim newWidth As Integer = CInt(Me.Height * videoRatio)
            Return New Rectangle((Me.Width - newWidth) \ 2, 0, newWidth, newHeight)
        Else
            Dim newWidth As Integer = Me.Width
            Dim newHeight As Integer = CInt(Me.Width / videoRatio)
            Return New Rectangle(0, (Me.Height - newHeight) \ 2, newWidth, newHeight)
        End If
    End Function

    ' Position timer tick
    Private Sub PositionTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If hasVideo AndAlso _isPlaying AndAlso mediaPosition IsNot Nothing Then
                Dim pos As Double = 0
                Dim hr As Integer = mediaPosition.get_CurrentPosition(pos)

                If hr >= 0 AndAlso pos >= 0 Then
                    _currentPosition = pos
                    RaiseEvent PositionChanged(pos)

                    If _videoDuration > 0 AndAlso pos >= _videoDuration - 0.1 Then
                        _isPlaying = False
                        positionTimer.Stop()
                        RaiseEvent PlaybackEnded()
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' Play
    Public Sub Play()
        Try
            If mediaControl IsNot Nothing AndAlso hasVideo Then
                SetupVideoWindow()
                Dim hr As Integer = mediaControl.Run()

                If hr >= 0 Then
                    _isPlaying = True
                    positionTimer.Start()
                    Debug.WriteLine("Playback started")
                End If

                Me.Invalidate()
                Application.DoEvents()
            End If
        Catch ex As Exception
            Debug.WriteLine("Play error: " & ex.Message)
        End Try
    End Sub

    ' Pause
    Public Sub Pause()
        Try
            If mediaControl IsNot Nothing Then
                mediaControl.Pause()
                _isPlaying = False
                positionTimer.Stop()
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' Stop
    Public Sub StopVideo()
        Try
            If mediaControl IsNot Nothing Then
                mediaControl.Stop()
                _isPlaying = False
                positionTimer.Stop()

                If mediaPosition IsNot Nothing Then
                    mediaPosition.put_CurrentPosition(0)
                End If

                _currentPosition = 0
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' Seek
    Public Sub Seek(ByVal seconds As Double)
        Try
            If mediaPosition IsNot Nothing Then
                mediaPosition.put_CurrentPosition(seconds)
                _currentPosition = seconds
                Me.Invalidate()
                Application.DoEvents()
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' Get current position
    Public Function GetCurrentPosition() As Double
        Try
            If mediaPosition IsNot Nothing Then
                Dim pos As Double = 0
                mediaPosition.get_CurrentPosition(pos)
                Return pos
            End If
        Catch ex As Exception
        End Try
        Return _currentPosition
    End Function

    ' Get duration
    Public Function GetDuration() As Double
        Return _videoDuration
    End Function

    ' Is playing
    Public Function IsVideoPlaying() As Boolean
        Return _isPlaying
    End Function

    ' Cleanup
    Public Sub Cleanup()
        Try
            positionTimer.Stop()

            If mediaControl IsNot Nothing Then
                Try
                    mediaControl.Stop()
                Catch ex As Exception
                End Try
            End If

            If videoWindow IsNot Nothing Then
                Try
                    videoWindow.put_Visible(OABool.False)
                    videoWindow.put_Owner(IntPtr.Zero)
                Catch ex As Exception
                End Try
            End If

            ' Release COM objects
            If mediaControl IsNot Nothing Then
                Marshal.ReleaseComObject(mediaControl)
                mediaControl = Nothing
            End If
            If mediaPosition IsNot Nothing Then
                Marshal.ReleaseComObject(mediaPosition)
                mediaPosition = Nothing
            End If
            If mediaEvent IsNot Nothing Then
                Marshal.ReleaseComObject(mediaEvent)
                mediaEvent = Nothing
            End If
            If videoWindow IsNot Nothing Then
                Marshal.ReleaseComObject(videoWindow)
                videoWindow = Nothing
            End If
            If basicVideo IsNot Nothing Then
                Marshal.ReleaseComObject(basicVideo)
                basicVideo = Nothing
            End If
            If mediaSeeking IsNot Nothing Then
                Marshal.ReleaseComObject(mediaSeeking)
                mediaSeeking = Nothing
            End If
            If graphBuilder IsNot Nothing Then
                Marshal.ReleaseComObject(graphBuilder)
                graphBuilder = Nothing
            End If

            _isPlaying = False
            hasVideo = False
            _currentPosition = 0
            _videoDuration = 0

            Me.Invalidate()

        Catch ex As Exception
            Debug.WriteLine("Cleanup error: " & ex.Message)
        End Try
    End Sub

    ' Color timer tick
    Private Sub colorTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles colorTimer.Tick
        If Not hasVideo Then
            colorIndex += 1
            If colorIndex >= colors.Length Then colorIndex = 0
            Me.Invalidate()
        End If
    End Sub

    ' OnResize
    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        MyBase.OnResize(e)
        Try
            If videoWindow IsNot Nothing AndAlso hasVideo Then
                Dim videoRect As Rectangle = CalculateVideoRect()
                videoWindow.SetWindowPosition(videoRect.X, videoRect.Y, videoRect.Width, videoRect.Height)
            End If
        Catch ex As Exception
        End Try
        Me.Invalidate()
    End Sub

    ' OnPaint
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.Clear(Color.Black)

        If Not hasVideo Then
            DrawLogo(e.Graphics)
        End If
    End Sub

    ' Draw logo
    Private Sub DrawLogo(ByVal g As Graphics)
        Try
            ' Set kualitas rendering
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            ' ============ GAMBAR LOGO ANIMASI ============
            Dim logoSize As Integer = 80 ' Ukuran logo
            Dim logoX As Single = (Me.Width - logoSize) / 2
            Dim logoY As Single = (Me.Height / 2) - 80 ' Posisi di atas teks

            ' Gambar logo dari PictureBox
            If logoPictureBox.Image IsNot Nothing Then
                ' Buat rectangle untuk logo
                Dim logoRect As New RectangleF(logoX, logoY, logoSize, logoSize)

                ' Gambar logo dengan efek bayangan
                Using shadowBrush As New SolidBrush(Color.FromArgb(80, 0, 0, 0))
                    Dim shadowRect As New RectangleF(logoX + 3, logoY + 3, logoSize, logoSize)
                    g.FillEllipse(shadowBrush, shadowRect)
                End Using

                ' Gambar logo (lingkaran dengan gambar di dalamnya)
                Using path As New Drawing2D.GraphicsPath()
                    ' Buat lingkaran untuk clip
                    path.AddEllipse(logoX, logoY, logoSize, logoSize)
                    g.SetClip(path)

                    ' Gambar gambar
                    g.DrawImage(logoPictureBox.Image, logoX, logoY, logoSize, logoSize)

                    ' Reset clip
                    g.ResetClip()
                End Using

                ' Gambar border lingkaran
                Using borderPen As New Pen(Color.FromArgb(100, 255, 255, 255), 2)
                    g.DrawEllipse(borderPen, logoX, logoY, logoSize, logoSize)
                End Using

                ' Gambar efek glow - PERBAIKAN: Gunakan titik-titik untuk PathGradientBrush
                Dim glowPoints As New List(Of PointF)()
                Dim glowRadius As Integer = 50
                Dim centerX As Single = logoX + logoSize / 2
                Dim centerY As Single = logoY + logoSize / 2

                ' Buat titik-titik untuk membentuk lingkaran glow
                For i As Integer = 0 To 360 Step 10
                    Dim angle As Double = i * Math.PI / 180.0
                    Dim px As Single = centerX + CSng(Math.Cos(angle) * glowRadius)
                    Dim py As Single = centerY + CSng(Math.Sin(angle) * glowRadius)
                    glowPoints.Add(New PointF(px, py))
                Next

                Using glowBrush As New Drawing2D.PathGradientBrush(glowPoints.ToArray())
                    Dim centerColor As Color = Color.FromArgb(50, colors(colorIndex))
                    Dim surroundColor As Color = Color.FromArgb(0, colors(colorIndex))
                    glowBrush.CenterColor = centerColor
                    glowBrush.SurroundColors = New Color() {surroundColor}
                    glowBrush.CenterPoint = New PointF(centerX, centerY)
                    g.FillEllipse(glowBrush, logoX - 15, logoY - 15, logoSize + 30, logoSize + 30)
                End Using
            Else
                ' Fallback: jika gambar tidak ada, tampilkan ikon lingkaran
                Dim fallbackSize As Integer = 60
                Dim fallbackX As Single = (Me.Width - fallbackSize) / 2
                Dim fallbackY As Single = (Me.Height / 2) - 70

                Using circleBrush As New SolidBrush(Color.FromArgb(80, colors(colorIndex)))
                    g.FillEllipse(circleBrush, fallbackX, fallbackY, fallbackSize, fallbackSize)
                End Using

                Using circlePen As New Pen(colors(colorIndex), 3)
                    g.DrawEllipse(circlePen, fallbackX, fallbackY, fallbackSize, fallbackSize)
                End Using

                ' Gambar ikon play di dalam lingkaran
                Using playBrush As New SolidBrush(Color.White)
                    Dim points As Point() = {
                        New Point(CInt(fallbackX + 20), CInt(fallbackY + 15)),
                        New Point(CInt(fallbackX + 20), CInt(fallbackY + fallbackSize - 15)),
                        New Point(CInt(fallbackX + fallbackSize - 15), CInt(fallbackY + fallbackSize / 2))
                    }
                    g.FillPolygon(playBrush, points)
                End Using
            End If

            ' ============ GAMBAR TEKS ============
            ' Font untuk judul
            Dim titleFont As New Font("Arial Black", 28, FontStyle.Bold)
            Dim subtitleFont As New Font("Arial", 12, FontStyle.Regular)

            ' Ukuran teks
            Dim titleText As String = "Pongo Video Cutter"
            Dim subtitleText As String = "Drag & Drop Video Here..."

            Dim titleSize As SizeF = g.MeasureString(titleText, titleFont)
            Dim subtitleSize As SizeF = g.MeasureString(subtitleText, subtitleFont)

            ' Posisi teks (di bawah logo)
            Dim titleX As Single = (Me.Width - titleSize.Width) / 2
            Dim titleY As Single = (Me.Height / 2) + 10 ' Posisi di bawah logo
            Dim subtitleX As Single = (Me.Width - subtitleSize.Width) / 2
            Dim subtitleY As Single = titleY + titleSize.Height + 10

            ' Warna utama (berubah-ubah)
            Dim mainColor As Color = colors(colorIndex)

            ' Gradient brush untuk judul
            Dim rect As New RectangleF(titleX - 10, titleY - 10, titleSize.Width + 20, titleSize.Height + 20)
            Dim brush As New Drawing2D.LinearGradientBrush(rect, mainColor, Color.White, 45)

            ' Gambar bayangan teks - PERBAIKAN: Ganti nama variabel
            Using textShadowBrush As New SolidBrush(Color.FromArgb(100, 0, 0, 0))
                g.DrawString(titleText, titleFont, textShadowBrush, titleX + 3, titleY + 3)
            End Using

            ' Gambar judul dengan gradient
            g.DrawString(titleText, titleFont, brush, titleX, titleY)

            ' Gambar subtitle
            Dim subtitleBrush As New SolidBrush(Color.FromArgb(180, 255, 255, 255))
            g.DrawString(subtitleText, subtitleFont, subtitleBrush, subtitleX, subtitleY)

            ' Gambar garis dekoratif
            Dim linePen As New Pen(mainColor, 2)
            Dim lineY As Single = titleY + titleSize.Height + 5
            Dim lineWidth As Single = Math.Min(300, titleSize.Width)
            Dim lineX As Single = (Me.Width - lineWidth) / 2

            g.DrawLine(linePen, lineX, lineY, lineX + lineWidth, lineY)

            ' Cleanup
            titleFont.Dispose()
            subtitleFont.Dispose()
            brush.Dispose()
            subtitleBrush.Dispose()
            linePen.Dispose()

        Catch ex As Exception
            ' Fallback sederhana jika terjadi error
            Try
                Dim fallbackBrush As New SolidBrush(Color.White)
                Dim fallbackFont As New Font("Arial", 14, FontStyle.Bold)
                g.DrawString("Pongo Video Cutter", fallbackFont, fallbackBrush, 10, 10)
                fallbackBrush.Dispose()
                fallbackFont.Dispose()
            Catch
            End Try
        End Try
    End Sub

    ' Constants
    Private Const WS_CHILD As Integer = &H40000000
    Private Const WS_CLIPCHILDREN As Integer = &H2000000

    ' Dispose
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            colorTimer.Stop()
            colorTimer.Dispose()
            positionTimer.Stop()
            positionTimer.Dispose()
            Cleanup()
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class