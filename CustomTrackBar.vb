Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Collections.Generic

Public Class CustomTrackBar
    Inherits Control
    Private _playheadPriority As Boolean = True
    ' Properties
    Private _minimum As Integer = 0
    Private _maximum As Integer = 1000
    Private _value As Integer = 0
    Private _isDragging As Boolean = False
    Private _segments As New List(Of TrackSegment)
    Private _selectedSegmentIndex As Integer = -1
    Private _draggingMarker As Integer = -1 ' -1 = none, 0 = thumb/playhead, 1 = start marker, 2 = end marker
    Private _tempStartMarker As Double = -1
    Private _tempEndMarker As Double = -1
    Private _isDraggingSegment As Boolean = False
    Private _isDraggingPlayhead As Boolean = False
    Private _lastMouseX As Integer = 0
    Private _isDraggingScrollbar As Boolean = False
    Private _drawnLabels As New List(Of Rectangle)
    ' Zoom properties
    Private _zoomLevel As Double = 1.0 ' 1.0 = 100% (full view)
    Private _zoomCenter As Double = 0.5 ' Center of zoom (0.0 to 1.0)
    Private _minZoomLevel As Double = 1.0 ' Minimum zoom (100%)
    Private _maxZoomLevel As Double = 200.0 ' Maximum zoom (20000%)
    Private _autoScrollEnabled As Boolean = True ' Auto-scroll during playback
    Private _autoScrollMargin As Double = 0.15 ' Margin 15% from edge before scrolling
    Private _zoomStep As Double = 1.5 ' Zoom step multiplier
    Private _isZooming As Boolean = False ' Flag to prevent recursive zoom

    ' Events
    Public Event ValueChanged As EventHandler
    Public Event Scroll As EventHandler
    Public Event SegmentClicked As SegmentClickedEventHandler
    Public Event SegmentChanged As SegmentChangedEventHandler
    Public Event PlayheadDragging As EventHandler
    Public Event PlayheadDragStart As EventHandler
    Public Event PlayheadDragEnd As EventHandler
    Public Event ZoomChanged As EventHandler

    ' Delegates
    Public Delegate Sub SegmentClickedEventHandler(ByVal sender As Object, ByVal segmentIndex As Integer)
    Public Delegate Sub SegmentChangedEventHandler(ByVal sender As Object, ByVal segmentIndex As Integer, ByVal startValue As Double, ByVal endValue As Double)

    ' Structure for segment
    Public Structure TrackSegment
        Dim StartValue As Double
        Dim EndValue As Double
        Dim Color As Color

        Public Sub New(ByVal start As Double, ByVal endVal As Double, ByVal segColor As Color)
            StartValue = start
            EndValue = endVal
            Color = segColor
        End Sub
    End Structure
    Public Property PlayheadPriority() As Boolean
        Get
            Return _playheadPriority
        End Get
        Set(ByVal value As Boolean)
            _playheadPriority = value
        End Set
    End Property
    ' Constructor
    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or _
                   ControlStyles.UserPaint Or _
                   ControlStyles.OptimizedDoubleBuffer Or _
                   ControlStyles.ResizeRedraw Or _
                   ControlStyles.Selectable, True) ' Tambahkan Selectable
        Me.Size = New Size(400, 100)
        Me.BackColor = Color.FromArgb(45, 45, 48)
        Me.TabStop = True
        Me.Cursor = Cursors.Hand
    End Sub

    ' ==================== ZOOM PROPERTIES ====================
    Public Property ZoomLevel() As Double
        Get
            Return _zoomLevel
        End Get
        Set(ByVal value As Double)
            If value < _minZoomLevel Then value = _minZoomLevel
            If value > _maxZoomLevel Then value = _maxZoomLevel
            If _zoomLevel <> value Then
                _zoomLevel = value
                Invalidate()
                RaiseEvent ZoomChanged(Me, EventArgs.Empty)
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentZoomLevel() As Double
        Get
            Return _zoomLevel
        End Get
    End Property

    Public Property AutoScrollEnabled() As Boolean
        Get
            Return _autoScrollEnabled
        End Get
        Set(ByVal value As Boolean)
            _autoScrollEnabled = value
        End Set
    End Property

    ' ==================== ZOOM METHODS ====================
    Public Sub ZoomIn()
        If _isZooming Then Return
        _isZooming = True
        Try
            ' Simpan posisi playhead sebelum zoom
            Dim playheadValue As Double = _value

            ' Hitung zoom baru
            Dim newZoom As Double = _zoomLevel * _zoomStep
            If newZoom > _maxZoomLevel Then newZoom = _maxZoomLevel

            ' Jika sudah maksimal, keluar
            If newZoom = _zoomLevel Then
                _isZooming = False
                Return
            End If

            ' Update zoom level
            _zoomLevel = newZoom

            ' Pastikan playhead tetap visible
            EnsureValueVisible(playheadValue)

            ' Update zoom center berdasarkan posisi playhead
            If _zoomLevel > 1.0 Then
                _zoomCenter = playheadValue / (_maximum - _minimum)
                _zoomCenter = Math.Max(0.0, Math.Min(1.0, _zoomCenter))
            Else
                _zoomCenter = 0.5
            End If

            Invalidate()
            RaiseEvent ZoomChanged(Me, EventArgs.Empty)
        Finally
            _isZooming = False
        End Try
    End Sub

    Public Sub ZoomOut()
        If _isZooming Then Return
        _isZooming = True
        Try
            ' Simpan posisi playhead sebelum zoom
            Dim playheadValue As Double = _value

            ' Hitung zoom baru
            Dim newZoom As Double = _zoomLevel / _zoomStep
            If newZoom < _minZoomLevel Then newZoom = _minZoomLevel

            ' Jika sudah minimal, keluar
            If newZoom = _zoomLevel Then
                _isZooming = False
                Return
            End If

            ' Update zoom level
            _zoomLevel = newZoom

            ' Jika kembali ke zoom penuh, reset center
            If _zoomLevel <= 1.0 Then
                _zoomCenter = 0.5
            Else
                ' Pastikan playhead tetap visible
                EnsureValueVisible(playheadValue)

                ' Update zoom center berdasarkan posisi playhead
                _zoomCenter = playheadValue / (_maximum - _minimum)
                _zoomCenter = Math.Max(0.0, Math.Min(1.0, _zoomCenter))
            End If

            Invalidate()
            RaiseEvent ZoomChanged(Me, EventArgs.Empty)
        Finally
            _isZooming = False
        End Try
    End Sub

    Public Sub ResetZoom()
        If _isZooming Then Return
        _isZooming = True
        Try
            _zoomLevel = 1.0
            _zoomCenter = 0.5
            Invalidate()
            RaiseEvent ZoomChanged(Me, EventArgs.Empty)
        Finally
            _isZooming = False
        End Try
    End Sub

    Public Function GetVisibleStartValue() As Double
        If _zoomLevel <= 1.0 Then
            Return _minimum
        End If

        Dim totalRange As Double = _maximum - _minimum
        Dim visibleRangeCalc As Double = totalRange / _zoomLevel

        ' Hitung start berdasarkan zoom center
        Dim startValue As Double = (_zoomCenter * totalRange) - (visibleRangeCalc / 2.0)

        ' Clamp ke range yang valid
        If startValue < _minimum Then startValue = _minimum
        If startValue > _maximum - visibleRangeCalc Then startValue = _maximum - visibleRangeCalc

        Return startValue
    End Function

    Public Function GetVisibleEndValue() As Double
        If _zoomLevel <= 1.0 Then
            Return _maximum
        End If

        Dim visibleStart As Double = GetVisibleStartValue()
        Dim totalRange As Double = _maximum - _minimum
        Dim visibleRangeCalc As Double = totalRange / _zoomLevel

        Return visibleStart + visibleRangeCalc
    End Function

    Public Sub EnsureValueVisible(ByVal value As Double)
        If _zoomLevel <= 1.0 Then Return

        Dim totalRange As Double = _maximum - _minimum
        Dim visibleRangeCalc As Double = totalRange / _zoomLevel
        Dim visibleStart As Double = GetVisibleStartValue()
        Dim visibleEnd As Double = visibleStart + visibleRangeCalc

        ' Hitung margin dalam nilai aktual
        Dim marginValue As Double = visibleRangeCalc * _autoScrollMargin

        ' Jika value di luar area visible (dengan margin)
        If value < visibleStart + marginValue OrElse value > visibleEnd - marginValue Then
            ' Hitung center baru agar value berada di tengah area visible
            Dim newCenter As Double = value / totalRange
            _zoomCenter = Math.Max(0.0, Math.Min(1.0, newCenter))
            Invalidate()
            RaiseEvent ZoomChanged(Me, EventArgs.Empty)
        End If
    End Sub

    Public Sub ScrollToValue(ByVal value As Double)
        If _zoomLevel <= 1.0 Then Return

        Dim totalRange As Double = _maximum - _minimum
        If totalRange <= 0 Then Return

        ' Hitung center baru
        Dim newCenter As Double = value / totalRange
        _zoomCenter = Math.Max(0.0, Math.Min(1.0, newCenter))
        Invalidate()
        RaiseEvent ZoomChanged(Me, EventArgs.Empty)
    End Sub

    Public Sub ScrollView(ByVal delta As Double)
        If _zoomLevel <= 1.0 Then Return

        Dim totalRange As Double = _maximum - _minimum
        If totalRange <= 0 Then Return

        ' Hitung scroll amount dalam nilai aktual
        Dim visibleRange As Double = totalRange / _zoomLevel
        Dim scrollAmount As Double = visibleRange * delta

        ' Hitung center baru
        Dim newCenterValue As Double = (_zoomCenter * totalRange) + scrollAmount

        ' Clamp center ke range yang valid
        Dim minCenter As Double = (visibleRange / 2) / totalRange
        Dim maxCenter As Double = 1.0 - minCenter

        _zoomCenter = Math.Max(minCenter, Math.Min(maxCenter, newCenterValue / totalRange))

        Invalidate()
        RaiseEvent ZoomChanged(Me, EventArgs.Empty)
    End Sub

    ' ==================== DRAGGING PROPERTIES ====================
    Public ReadOnly Property IsDraggingPlayhead() As Boolean
        Get
            Return _isDraggingPlayhead
        End Get
    End Property

    Public ReadOnly Property IsDraggingSegment() As Boolean
        Get
            Return _isDraggingSegment
        End Get
    End Property

    ' ==================== SELECTED SEGMENT ====================
    Public Property SelectedSegmentIndexValue() As Integer
        Get
            Return _selectedSegmentIndex
        End Get
        Set(ByVal value As Integer)
            If value >= -1 AndAlso value < _segments.Count Then
                _selectedSegmentIndex = value
                Invalidate()
            End If
        End Set
    End Property

    Public ReadOnly Property SelectedSegmentIndex() As Integer
        Get
            Return _selectedSegmentIndex
        End Get
    End Property

    ' ==================== MINIMUM/MAXIMUM/VALUE ====================
    Public Property Minimum() As Integer
        Get
            Return _minimum
        End Get
        Set(ByVal value As Integer)
            _minimum = value
            If _value < _minimum Then _value = _minimum
            Invalidate()
        End Set
    End Property

    Public Property Maximum() As Integer
        Get
            Return _maximum
        End Get
        Set(ByVal value As Integer)
            _maximum = value
            If _value > _maximum Then _value = _maximum
            Invalidate()
        End Set
    End Property

    Public Property Value() As Integer
        Get
            Return _value
        End Get
        Set(ByVal value As Integer)
            If value < _minimum Then value = _minimum
            If value > _maximum Then value = _maximum
            If _value <> value Then
                _value = value
                Invalidate()
                RaiseEvent ValueChanged(Me, EventArgs.Empty)

                ' Auto-scroll saat playback jika zoom aktif
                If _autoScrollEnabled AndAlso _zoomLevel > 1.0 Then
                    EnsureValueVisible(value)
                End If
            End If
        End Set
    End Property

    ' ==================== TEMP MARKERS ====================
    Public Property TempStartMarker() As Double
        Get
            Return _tempStartMarker
        End Get
        Set(ByVal value As Double)
            _tempStartMarker = value
            Invalidate()
        End Set
    End Property

    Public Property TempEndMarker() As Double
        Get
            Return _tempEndMarker
        End Get
        Set(ByVal value As Double)
            _tempEndMarker = value
            Invalidate()
        End Set
    End Property

    ' ==================== SEGMENT METHODS ====================
    Public Sub AddSegment(ByVal startValue As Double, ByVal endValue As Double)
        Dim colors As Color() = {Color.FromArgb(65, 180, 90), Color.FromArgb(65, 130, 220), Color.FromArgb(230, 150, 70), _
                                 Color.FromArgb(180, 80, 180), Color.FromArgb(70, 190, 190), Color.FromArgb(200, 200, 80)}
        Dim colorIndex As Integer = _segments.Count Mod colors.Length
        _segments.Add(New TrackSegment(startValue, endValue, colors(colorIndex)))
        _tempStartMarker = -1
        _tempEndMarker = -1
        Invalidate()
    End Sub

    Public Sub RemoveSegment(ByVal index As Integer)
        If index >= 0 AndAlso index < _segments.Count Then
            _segments.RemoveAt(index)
            If _selectedSegmentIndex >= _segments.Count Then
                _selectedSegmentIndex = -1
            End If
            Invalidate()
        End If
    End Sub

    Public Sub ClearSegments()
        _segments.Clear()
        _selectedSegmentIndex = -1
        _tempStartMarker = -1
        _tempEndMarker = -1
        Invalidate()
    End Sub

    Public Sub UpdateSegment(ByVal index As Integer, ByVal startValue As Double, ByVal endValue As Double)
        If index >= 0 AndAlso index < _segments.Count Then
            Dim seg As TrackSegment = _segments(index)
            seg.StartValue = startValue
            seg.EndValue = endValue
            _segments(index) = seg
            Invalidate()
        End If
    End Sub

    Public Function GetSegmentCount() As Integer
        Return _segments.Count
    End Function

    Public Function GetSegment(ByVal index As Integer) As TrackSegment
        If index >= 0 AndAlso index < _segments.Count Then
            Return _segments(index)
        End If
        Return Nothing
    End Function
    ' Helper to draw time ruler - Professional ruler style
    Private Sub DrawTimeRuler(ByVal g As Graphics, ByVal trackLeft As Integer, ByVal trackY As Integer, ByVal trackWidth As Integer, ByVal visibleStart As Double, ByVal visibleEnd As Double)
        Dim visibleRange As Double = visibleEnd - visibleStart

        ' Hitung nilai waktu dalam detik
        Dim startTimeInSeconds As Double = 0
        Dim endTimeInSeconds As Double = 0

        If _videoDuration > 0 Then
            startTimeInSeconds = (visibleStart / 1000.0) * _videoDuration
            endTimeInSeconds = (visibleEnd / 1000.0) * _videoDuration
        Else
            startTimeInSeconds = visibleStart
            endTimeInSeconds = visibleEnd
        End If

        Dim visibleTimeRange As Double = endTimeInSeconds - startTimeInSeconds

        ' Hitung interval berdasarkan pixel per detik
        Dim pixelsPerSecond As Double = trackWidth / visibleTimeRange

        ' Tentukan interval label (mayor) berdasarkan zoom
        ' Target: label setiap 80-100 pixels
        Dim targetLabelSpacing As Double = 85 ' pixels antara label
        Dim idealLabelInterval As Double = targetLabelSpacing / pixelsPerSecond

        ' Bulatkan ke interval yang bagus
        Dim majorInterval As Double = GetNiceInterval(idealLabelInterval)

        ' Interval minor: 10 ticks per major interval
        Dim minorInterval As Double = majorInterval / 10

        ' Interval medium: 5 ticks per major interval
        Dim mediumInterval As Double = majorInterval / 5

        ' Font untuk label waktu
        Dim timeFont As New Font("Segoe UI", 7, FontStyle.Regular)
        Dim timeBrush As New SolidBrush(Color.FromArgb(140, 140, 140))

        ' Pens untuk berbagai tingkat ticks
        Dim minorTickPen As New Pen(Color.FromArgb(60, 60, 60), 1)
        Dim mediumTickPen As New Pen(Color.FromArgb(80, 80, 80), 1)
        Dim majorTickPen As New Pen(Color.FromArgb(120, 120, 120), 1)

        ' Background ruler
        Dim rulerRect As New Rectangle(trackLeft, trackY - 22, trackWidth, 22)
        Using rulerBgBrush As New SolidBrush(Color.FromArgb(35, 35, 38))
            g.FillRectangle(rulerBgBrush, rulerRect)
        End Using

        ' Garis bawah ruler
        Using rulerBorderPen As New Pen(Color.FromArgb(60, 60, 60), 1)
            g.DrawLine(rulerBorderPen, trackLeft, trackY - 1, trackLeft + trackWidth, trackY - 1)
        End Using

        ' Draw minor ticks (terpendek, paling rapat)
        Dim startMinorTick As Double = Math.Floor(startTimeInSeconds / minorInterval) * minorInterval
        For tickTime As Double = startMinorTick To endTimeInSeconds Step minorInterval
            If tickTime >= startTimeInSeconds AndAlso tickTime <= endTimeInSeconds Then
                Dim tickX As Integer = trackLeft + CInt(((tickTime - startTimeInSeconds) / visibleTimeRange) * trackWidth)

                If tickX >= trackLeft AndAlso tickX <= trackLeft + trackWidth Then
                    ' Minor tick - garis pendek 4px
                    g.DrawLine(minorTickPen, tickX, trackY - 4, tickX, trackY - 1)
                End If
            End If
        Next

        ' Draw medium ticks (sedang)
        Dim startMediumTick As Double = Math.Floor(startTimeInSeconds / mediumInterval) * mediumInterval
        For tickTime As Double = startMediumTick To endTimeInSeconds Step mediumInterval
            If tickTime >= startTimeInSeconds AndAlso tickTime <= endTimeInSeconds Then
                Dim tickX As Integer = trackLeft + CInt(((tickTime - startTimeInSeconds) / visibleTimeRange) * trackWidth)

                If tickX >= trackLeft AndAlso tickX <= trackLeft + trackWidth Then
                    ' Medium tick - garis sedang 7px
                    g.DrawLine(mediumTickPen, tickX, trackY - 7, tickX, trackY - 1)
                End If
            End If
        Next

        ' Draw major ticks dengan label (paling panjang)
        Dim startMajorTick As Double = Math.Floor(startTimeInSeconds / majorInterval) * majorInterval
        Dim lastLabelX As Integer = -1000 ' Untuk mencegah overlap

        For tickTime As Double = startMajorTick To endTimeInSeconds Step majorInterval
            If tickTime >= startTimeInSeconds AndAlso tickTime <= endTimeInSeconds Then
                Dim tickX As Integer = trackLeft + CInt(((tickTime - startTimeInSeconds) / visibleTimeRange) * trackWidth)

                If tickX >= trackLeft AndAlso tickX <= trackLeft + trackWidth Then
                    ' Major tick - garis panjang 10px
                    g.DrawLine(majorTickPen, tickX, trackY - 10, tickX, trackY - 1)

                    ' Format waktu untuk label
                    Dim timeLabel As String = FormatTimeLabel(tickTime, majorInterval)

                    ' Ukur teks
                    Dim textSize As SizeF = g.MeasureString(timeLabel, timeFont)
                    Dim labelX As Integer = tickX + 2 ' Label sedikit ke kanan dari tick
                    Dim labelY As Integer = trackY - 19

                    ' Pastikan label tidak keluar dari batas
                    If labelX < trackLeft Then labelX = trackLeft
                    If labelX + CInt(textSize.Width) > trackLeft + trackWidth Then
                        labelX = trackLeft + trackWidth - CInt(textSize.Width) - 1
                    End If

                    ' Cek overlap dengan label sebelumnya
                    If labelX > lastLabelX + 50 Then ' Minimal jarak 50px antara label
                        ' Gambar label waktu
                        g.DrawString(timeLabel, timeFont, timeBrush, labelX, labelY)
                        lastLabelX = labelX
                    End If
                End If
            End If
        Next

        ' Dispose resources
        timeFont.Dispose()
        timeBrush.Dispose()
        minorTickPen.Dispose()
        mediumTickPen.Dispose()
        majorTickPen.Dispose()
    End Sub

    ' Helper untuk mendapatkan interval yang "bagus"
    Private Function GetNiceInterval(ByVal rawInterval As Double) As Double
        ' Array interval yang "bagus" dalam detik
        Dim niceIntervals As Double() = { _
            0.001, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.5, _
            1, 2, 5, 10, 15, 30, _
            60, 120, 300, 600, 900, 1800, 3600, 7200 _
        }

        ' Cari interval terdekat yang lebih besar atau sama dengan rawInterval
        For Each interval As Double In niceIntervals
            If interval >= rawInterval Then
                Return interval
            End If
        Next

        ' Jika rawInterval lebih besar dari semua, gunakan yang terbesar
        Return niceIntervals(niceIntervals.Length - 1)
    End Function

    ' Helper untuk format label waktu
    Private Function FormatTimeLabel(ByVal seconds As Double, ByVal interval As Double) As String
        ' Bulatkan ke interval terdekat untuk menghindari floating point errors
        Dim roundedSeconds As Double = Math.Round(seconds / interval) * interval
        Dim ts As TimeSpan = TimeSpan.FromSeconds(roundedSeconds)

        If interval < 0.1 Then
            ' Tampilkan dengan milliseconds (00:00.000)
            Return String.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds)
        ElseIf interval < 1 Then
            ' Tampilkan dengan 1 decimal (00:00.0)
            Return String.Format("{0:00}:{1:00}.{2:0}", ts.Minutes, ts.Seconds, ts.Milliseconds / 100)
        ElseIf interval < 60 Then
            ' Tampilkan menit:detik (00:00)
            If ts.Hours > 0 Then
                Return String.Format("{0}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds)
            Else
                Return String.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds)
            End If
        Else
            ' Tampilkan jam:menit:detik (0:00:00)
            Return String.Format("{0}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds)
        End If
    End Function
    ' ==================== ON PAINT ====================
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        _drawnLabels.Clear()

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        ' Clear background with gradient
        Using bgBrush As New LinearGradientBrush(Me.ClientRectangle, Color.FromArgb(50, 50, 53), Color.FromArgb(40, 40, 43), LinearGradientMode.Vertical)
            g.FillRectangle(bgBrush, Me.ClientRectangle)
        End Using

        ' Calculate track area - GESER KE BAWAH untuk memberi ruang ruler
        Dim trackY As Integer = 42 ' Diubah dari 40 ke 42
        Dim trackHeight As Integer = 12
        Dim trackLeft As Integer = 15
        Dim trackRight As Integer = Me.Width - 15
        Dim trackWidth As Integer = trackRight - trackLeft

        ' Calculate visible range based on zoom
        Dim visibleStartValue As Double = GetVisibleStartValue()
        Dim visibleEndValue As Double = GetVisibleEndValue()
        Dim visibleRange As Double = visibleEndValue - visibleStartValue

        ' Draw time ruler
        DrawTimeRuler(g, trackLeft, trackY, trackWidth, visibleStartValue, visibleEndValue)

        ' Draw track background
        Using shadowBrush As New SolidBrush(Color.FromArgb(60, 0, 0, 0))
            g.FillRectangle(shadowBrush, trackLeft + 1, trackY + 2, trackWidth, trackHeight)
        End Using

        Using trackBrush As New SolidBrush(Color.FromArgb(70, 70, 75))
            g.FillRectangle(trackBrush, trackLeft, trackY, trackWidth, trackHeight)
        End Using

        Using trackPen As New Pen(Color.FromArgb(100, 100, 105), 1)
            g.DrawRectangle(trackPen, trackLeft, trackY, trackWidth, trackHeight)
        End Using

        ' Draw grid lines with interval adjusted for zoom
        DrawGridLines(g, trackLeft, trackY, trackWidth, trackHeight, visibleStartValue, visibleEndValue)

        ' Draw all segments
        For i As Integer = 0 To _segments.Count - 1
            Dim seg As TrackSegment = _segments(i)

            ' Check if segment is visible
            If seg.EndValue >= visibleStartValue AndAlso seg.StartValue <= visibleEndValue Then
                ' Convert value to X position with zoom
                Dim startX As Integer = trackLeft + CInt(((seg.StartValue - visibleStartValue) / visibleRange) * trackWidth)
                Dim endX As Integer = trackLeft + CInt(((seg.EndValue - visibleStartValue) / visibleRange) * trackWidth)

                ' Clamp to track area
                If startX < trackLeft Then startX = trackLeft
                If endX > trackRight Then endX = trackRight

                Dim isSelected As Boolean = (i = _selectedSegmentIndex)

                ' Segment area with rounded corners
                If endX > startX Then
                    Dim segRect As New Rectangle(startX, trackY + 2, endX - startX, trackHeight - 4)
                    Dim segPath As GraphicsPath = CreateRoundedRectangle(segRect, 3)

                    Dim alpha As Integer = If(isSelected, 200, 150)

                    Using segBrush As New SolidBrush(Color.FromArgb(alpha, seg.Color))
                        g.FillPath(segBrush, segPath)
                    End Using

                    Using highlightBrush As New SolidBrush(Color.FromArgb(60, 255, 255, 255))
                        Dim highlightRect As New Rectangle(startX + 2, trackY + 3, Math.Max(1, endX - startX - 4), (trackHeight - 6) \ 2)
                        g.FillRectangle(highlightBrush, highlightRect)
                    End Using

                    Dim borderColor As Color = If(isSelected, Color.White, Color.FromArgb(180, seg.Color))
                    Dim borderWidth As Single = If(isSelected, 2.0F, 1.0F)

                    Using segPen As New Pen(borderColor, borderWidth)
                        g.DrawPath(segPen, segPath)
                    End Using

                    segPath.Dispose()
                End If

                ' Draw markers only if in view
                If seg.StartValue >= visibleStartValue AndAlso seg.StartValue <= visibleEndValue Then
                    DrawMarker(g, startX, trackY + trackHeight \ 2, seg.Color, isSelected, True)
                End If

                If seg.EndValue >= visibleStartValue AndAlso seg.EndValue <= visibleEndValue Then
                    DrawMarker(g, endX, trackY + trackHeight \ 2, seg.Color, isSelected, False)
                End If

                ' Segment label
                If endX - startX > 20 Then
                    Dim labelText As String = If(endX - startX > 60, "Seg " & (i + 1).ToString(), (i + 1).ToString())
                    Dim labelFont As New Font("Segoe UI", 8, FontStyle.Bold)
                    Dim textSize As SizeF = g.MeasureString(labelText, labelFont)
                    Dim labelWidth As Integer = CInt(textSize.Width) + 14
                    Dim labelHeight As Integer = 20
                    Dim labelX As Integer = startX + (endX - startX - labelWidth) \ 2

                    If labelX < 0 Then labelX = 0
                    If labelX + labelWidth > Me.Width Then labelX = Me.Width - labelWidth - 1

                    Dim labelY As Integer = trackY - labelHeight - 5
                    Dim labelRect As New Rectangle(labelX, labelY, labelWidth, labelHeight)
                    DrawSegmentLabel(g, labelRect, labelText, seg.Color, isSelected)
                    labelFont.Dispose()
                End If
            End If
        Next

        ' Draw temp markers
        If _tempStartMarker >= visibleStartValue AndAlso _tempStartMarker <= visibleEndValue Then
            Dim tempStartX As Integer = trackLeft + CInt(((_tempStartMarker - visibleStartValue) / visibleRange) * trackWidth)
            DrawTempMarker(g, tempStartX, trackY, trackHeight, Color.FromArgb(0, 220, 0), "START")
        End If

        If _tempEndMarker >= visibleStartValue AndAlso _tempEndMarker <= visibleEndValue Then
            Dim tempEndX As Integer = trackLeft + CInt(((_tempEndMarker - visibleStartValue) / visibleRange) * trackWidth)
            DrawTempMarker(g, tempEndX, trackY, trackHeight, Color.FromArgb(255, 60, 60), "END")
        End If

        ' Draw playhead
        If _value >= visibleStartValue AndAlso _value <= visibleEndValue Then
            Dim playheadX As Integer = trackLeft + CInt(((_value - visibleStartValue) / visibleRange) * trackWidth)
            Dim playheadHeight As Integer = If(_isDraggingPlayhead, 20, 16)

            Using playheadPen As New Pen(Color.FromArgb(255, 80, 80), If(_isDraggingPlayhead, 3.0F, 2.0F))
                g.DrawLine(playheadPen, playheadX, trackY - playheadHeight \ 2, playheadX, trackY + trackHeight + playheadHeight \ 2)
            End Using

            Dim handleSize As Integer = If(_isDraggingPlayhead, 8, 6)
            Dim playheadPath As New GraphicsPath()
            playheadPath.AddPolygon(New Point() { _
                New Point(playheadX - handleSize, trackY - playheadHeight \ 2), _
                New Point(playheadX + handleSize, trackY - playheadHeight \ 2), _
                New Point(playheadX, trackY - 2) _
            })
            Using playheadBrush As New SolidBrush(Color.FromArgb(255, 80, 80))
                g.FillPath(playheadBrush, playheadPath)
            End Using
            playheadPath.Dispose()

            Dim handleCircleSize As Integer = If(_isDraggingPlayhead, 8, 6)
            Dim handleCircleY As Integer = trackY + trackHeight + playheadHeight \ 2
            Using handleCircleBrush As New SolidBrush(Color.FromArgb(255, 80, 80))
                g.FillEllipse(handleCircleBrush, playheadX - handleCircleSize \ 2, handleCircleY - handleCircleSize \ 2, handleCircleSize, handleCircleSize)
            End Using
            Using handleCirclePen As New Pen(Color.White, 1.5F)
                g.DrawEllipse(handleCirclePen, playheadX - handleCircleSize \ 2, handleCircleY - handleCircleSize \ 2, handleCircleSize, handleCircleSize)
            End Using
        End If

        ' Draw time at bottom left of trackbar
        Dim currentTime As Double = 0
        If _videoDuration > 0 Then
            currentTime = (_value / 1000.0) * _videoDuration
        End If

        Dim timeText As String = FormatTime(currentTime)
        Dim timeFont As New Font("Arial", 8, FontStyle.Bold)
        Dim timeBrush As New SolidBrush(Color.White)

        Dim timeBgWidth As Integer = 70
        Dim timeBgHeight As Integer = 18
        Dim timeBgX As Integer = 5
        Dim timeBgY As Integer = trackY + trackHeight + 10

        Dim timeBgRect As New Rectangle(timeBgX, timeBgY, timeBgWidth, timeBgHeight)
        Dim timeBgPath As GraphicsPath = CreateRoundedRectangle(timeBgRect, 3)
        Using timeBgBrush As New SolidBrush(Color.FromArgb(220, 60, 60, 60))
            g.FillPath(timeBgBrush, timeBgPath)
        End Using

        Using timeBgPen As New Pen(Color.FromArgb(100, 100, 100), 1)
            g.DrawPath(timeBgPen, timeBgPath)
        End Using
        timeBgPath.Dispose()

        Dim timeRect As New Rectangle(timeBgX, timeBgY, timeBgWidth, timeBgHeight)
        Dim timeFormat As New StringFormat()
        timeFormat.Alignment = StringAlignment.Center
        timeFormat.LineAlignment = StringAlignment.Center
        g.DrawString(timeText, timeFont, timeBrush, timeRect, timeFormat)
        timeFont.Dispose()
        timeBrush.Dispose()
        timeFormat.Dispose()

        ' Draw total duration at bottom right
        If _videoDuration > 0 Then
            Dim totalTimeText As String = FormatTime(_videoDuration)
            Dim totalFont As New Font("Arial", 8, FontStyle.Regular)
            Dim totalBrush As New SolidBrush(Color.FromArgb(180, 180, 180))

            Dim totalBgWidth As Integer = 70
            Dim totalBgHeight As Integer = 18
            Dim totalBgX As Integer = Me.Width - totalBgWidth - 5
            Dim totalBgY As Integer = trackY + trackHeight + 10

            Dim totalBgRect As New Rectangle(totalBgX, totalBgY, totalBgWidth, totalBgHeight)
            Dim totalBgPath As GraphicsPath = CreateRoundedRectangle(totalBgRect, 3)
            Using totalBgBrush As New SolidBrush(Color.FromArgb(220, 60, 60, 60))
                g.FillPath(totalBgBrush, totalBgPath)
            End Using

            Using totalBgPen As New Pen(Color.FromArgb(100, 100, 100), 1)
                g.DrawPath(totalBgPen, totalBgPath)
            End Using
            totalBgPath.Dispose()

            Dim totalRect As New Rectangle(totalBgX, totalBgY, totalBgWidth, totalBgHeight)
            Dim totalFormat As New StringFormat()
            totalFormat.Alignment = StringAlignment.Center
            totalFormat.LineAlignment = StringAlignment.Center
            g.DrawString(totalTimeText, totalFont, totalBrush, totalRect, totalFormat)
            totalFont.Dispose()
            totalBrush.Dispose()
            totalFormat.Dispose()
        End If

        ' Draw zoom controls
        DrawZoomControls(g, trackY, trackHeight, trackLeft, trackWidth)
    End Sub

    ' Helper to draw grid lines with zoom
    ' Helper to draw grid lines with zoom
    Private Sub DrawGridLines(ByVal g As Graphics, ByVal trackLeft As Integer, ByVal trackY As Integer, ByVal trackWidth As Integer, ByVal trackHeight As Integer, ByVal visibleStart As Double, ByVal visibleEnd As Double)
        Dim visibleRange As Double = visibleEnd - visibleStart

        ' Determine grid interval based on zoom level
        Dim gridInterval As Double
        If _zoomLevel >= 200 Then
            gridInterval = 1
        ElseIf _zoomLevel >= 100 Then
            gridInterval = 2
        ElseIf _zoomLevel >= 50 Then
            gridInterval = 5
        ElseIf _zoomLevel >= 20 Then
            gridInterval = 10
        ElseIf _zoomLevel >= 10 Then
            gridInterval = 20
        ElseIf _zoomLevel >= 5 Then
            gridInterval = 50
        ElseIf _zoomLevel >= 2 Then
            gridInterval = 100
        Else
            gridInterval = 200
        End If

        Using gridPen As New Pen(Color.FromArgb(30, 255, 255, 255), 1)
            Dim startGridValue As Double = Math.Ceiling(visibleStart / gridInterval) * gridInterval
            For gridValue As Double = startGridValue To visibleEnd Step gridInterval
                Dim gridX As Integer = trackLeft + CInt(((gridValue - visibleStart) / visibleRange) * trackWidth)
                If gridX >= trackLeft AndAlso gridX <= trackLeft + trackWidth Then
                    g.DrawLine(gridPen, gridX, trackY + 2, gridX, trackY + trackHeight - 2)
                End If
            Next
        End Using
    End Sub

    ' Helper to draw zoom controls
    Private Sub DrawZoomControls(ByVal g As Graphics, ByVal trackY As Integer, ByVal trackHeight As Integer, ByVal trackLeft As Integer, ByVal trackWidth As Integer)
        ' Draw zoom level indicator
        Dim zoomText As String = String.Format("Zoom: {0}%", CInt(_zoomLevel * 100))
        Dim zoomFont As New Font("Arial", 8, FontStyle.Bold)
        Dim zoomBrush As New SolidBrush(Color.FromArgb(200, 200, 200))

        Dim zoomTextSize As SizeF = g.MeasureString(zoomText, zoomFont)
        Dim zoomX As Integer = trackLeft + (trackWidth - CInt(zoomTextSize.Width)) \ 2
        Dim zoomY As Integer = trackY + trackHeight + 8

        g.DrawString(zoomText, zoomFont, zoomBrush, zoomX, zoomY)

        ' Draw mini scrollbar for navigation
        If _zoomLevel > 1.0 Then
            Dim scrollBarY As Integer = trackY + trackHeight + 30
            Dim scrollBarHeight As Integer = 6

            ' Background scrollbar
            Using scrollBgBrush As New SolidBrush(Color.FromArgb(60, 60, 65))
                g.FillRectangle(scrollBgBrush, trackLeft, scrollBarY, trackWidth, scrollBarHeight)
            End Using

            ' Thumb scrollbar
            Dim thumbWidth As Integer = CInt(trackWidth / _zoomLevel)
            If thumbWidth < 20 Then thumbWidth = 20

            Dim visibleStartValue As Double = GetVisibleStartValue()
            Dim thumbX As Integer = trackLeft + CInt((visibleStartValue / (_maximum - _minimum)) * trackWidth)

            If thumbX < trackLeft Then thumbX = trackLeft
            If thumbX + thumbWidth > trackLeft + trackWidth Then thumbX = trackLeft + trackWidth - thumbWidth

            Using thumbBrush As New SolidBrush(Color.FromArgb(150, 150, 155))
                g.FillRectangle(thumbBrush, thumbX, scrollBarY, thumbWidth, scrollBarHeight)
            End Using

            Using thumbPen As New Pen(Color.FromArgb(200, 200, 200), 1)
                g.DrawRectangle(thumbPen, thumbX, scrollBarY, thumbWidth, scrollBarHeight)
            End Using
        End If

        zoomFont.Dispose()
        zoomBrush.Dispose()
    End Sub

    ' Helper function to format time
    Private Function FormatTime(ByVal seconds As Double) As String
        If seconds < 0 Then seconds = 0
        Dim ts As TimeSpan = TimeSpan.FromSeconds(seconds)
        Return String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds)
    End Function

    ' Helper to create rounded rectangle
    Private Function CreateRoundedRectangle(ByVal rect As Rectangle, ByVal radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d As Integer = radius * 2
        path.AddArc(rect.X, rect.Y, d, d, 180, 90)
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    ' Helper to draw marker
    ' Helper to draw marker
    Private Sub DrawMarker(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal color As Color, ByVal isSelected As Boolean, ByVal isStart As Boolean)
        ' Tinggi marker tetap (vertikal)
        Dim markerHeight As Integer = If(isSelected, 12, 10)
        ' Lebar marker dikurangi (horizontal) - ubah dari 12/10 menjadi 4/3
        Dim markerWidth As Integer = If(isSelected, 4, 3)

        Dim markerPath As New GraphicsPath()

        If isStart Then
            ' Marker start: menempel di sisi kiri segmen
            markerPath.AddPolygon(New Point() { _
                New Point(x - markerWidth, y - markerHeight), _
                New Point(x, y - markerHeight), _
                New Point(x, y + markerHeight), _
                New Point(x - markerWidth, y + markerHeight) _
            })
        Else
            ' Marker end: menempel di sisi kanan segmen
            markerPath.AddPolygon(New Point() { _
                New Point(x, y - markerHeight), _
                New Point(x + markerWidth, y - markerHeight), _
                New Point(x + markerWidth, y + markerHeight), _
                New Point(x, y + markerHeight) _
            })
        End If

        Using shadowBrush As New SolidBrush(Color.FromArgb(80, 0, 0, 0))
            g.TranslateTransform(1, 1)
            g.FillPath(shadowBrush, markerPath)
            g.TranslateTransform(-1, -1)
        End Using

        Using markerBrush As New SolidBrush(color)
            g.FillPath(markerBrush, markerPath)
        End Using

        Dim markerColor As Color = If(isSelected, Color.White, Color.FromArgb(200, color))
        Using markerPen As New Pen(markerColor, 1.5F)
            g.DrawPath(markerPen, markerPath)
        End Using

        markerPath.Dispose()
    End Sub

    ' Helper to draw segment label
    Private Sub DrawSegmentLabel(ByVal g As Graphics, ByVal rect As Rectangle, ByVal text As String, ByVal color As Color, ByVal isSelected As Boolean)
        Dim labelPath As GraphicsPath = CreateRoundedRectangle(rect, 5)
        Dim alpha As Integer = If(isSelected, 220, 180)

        Dim shadowRect As New Rectangle(rect.X + 1, rect.Y + 1, rect.Width, rect.Height)
        Dim shadowPath As GraphicsPath = CreateRoundedRectangle(shadowRect, 5)
        Using shadowBrush As New SolidBrush(Color.FromArgb(100, 0, 0, 0))
            g.FillPath(shadowBrush, shadowPath)
        End Using
        shadowPath.Dispose()

        Using labelBgBrush As New SolidBrush(Color.FromArgb(alpha, color))
            g.FillPath(labelBgBrush, labelPath)
        End Using

        Dim labelColor As Color = If(isSelected, Color.White, Color.FromArgb(150, 0, 0, 0))
        Using labelPen As New Pen(labelColor, 1.0F)
            g.DrawPath(labelPen, labelPath)
        End Using

        Using labelBrush As New SolidBrush(Color.White)
            Using labelFont As New Font("Segoe UI", 8, FontStyle.Bold)
                Dim sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoClip
                g.DrawString(text, labelFont, labelBrush, rect, sf)
                sf.Dispose()
            End Using
        End Using

        labelPath.Dispose()
    End Sub

    ' Helper to draw temp marker
    Private Sub DrawTempMarker(ByVal g As Graphics, ByVal x As Integer, ByVal trackY As Integer, ByVal trackHeight As Integer, ByVal color As Color, ByVal label As String)
        ' Garis vertikal tipis
        Using tempPen As New Pen(color, 1.5F)
            g.DrawLine(tempPen, x, trackY - 10, x, trackY + trackHeight + 10)
        End Using

        ' Marker tipis di track (seperti marker segmen)
        Dim markerWidth As Integer = 3
        Dim markerHeight As Integer = 10
        Dim markerY As Integer = trackY + (trackHeight - markerHeight) \ 2

        Dim markerRect As New Rectangle(x - markerWidth, markerY, markerWidth * 2, markerHeight)

        ' Shadow tipis
        Using shadowBrush As New SolidBrush(Color.FromArgb(80, 0, 0, 0))
            g.FillRectangle(shadowBrush, markerRect.X + 1, markerRect.Y + 1, markerRect.Width, markerRect.Height)
        End Using

        ' Marker utama
        Using markerBrush As New SolidBrush(Color.FromArgb(200, color))
            g.FillRectangle(markerBrush, markerRect)
        End Using

        ' Border tipis
        Using markerPen As New Pen(Color.White, 1.0F)
            g.DrawRectangle(markerPen, markerRect)
        End Using

        ' Label di atas marker
        If Not String.IsNullOrEmpty(label) Then
            Dim labelFont As New Font("Segoe UI", 7, FontStyle.Bold)
            Dim textSize As SizeF = g.MeasureString(label, labelFont)
            Dim labelWidth As Integer = CInt(textSize.Width) + 12
            Dim labelHeight As Integer = CInt(textSize.Height) + 6
            Dim labelX As Integer = x - labelWidth \ 2
            Dim labelY As Integer = trackY - labelHeight - 8

            If labelX < 0 Then labelX = 0
            If labelX + labelWidth > Me.Width Then labelX = Me.Width - labelWidth - 1

            ' Label background dengan sudut membulat
            Dim labelRect As New Rectangle(labelX, labelY, labelWidth, labelHeight)
            Dim labelPath As GraphicsPath = CreateRoundedRectangle(labelRect, 3)

            ' Shadow label
            Dim shadowRect As New Rectangle(labelRect.X + 1, labelRect.Y + 1, labelRect.Width, labelRect.Height)
            Dim shadowPath As GraphicsPath = CreateRoundedRectangle(shadowRect, 3)
            Using shadowBrush As New SolidBrush(Color.FromArgb(80, 0, 0, 0))
                g.FillPath(shadowBrush, shadowPath)
            End Using
            shadowPath.Dispose()

            ' Background label
            Using labelBgBrush As New SolidBrush(Color.FromArgb(220, color))
                g.FillPath(labelBgBrush, labelPath)
            End Using

            ' Border label
            Using labelPen As New Pen(Color.White, 1.0F)
                g.DrawPath(labelPen, labelPath)
            End Using

            ' Teks label
            Using labelBrush As New SolidBrush(Color.White)
                Dim sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoClip
                g.DrawString(label, labelFont, labelBrush, labelRect, sf)
                sf.Dispose()
            End Using

            labelPath.Dispose()
            labelFont.Dispose()
        End If
    End Sub

    ' ==================== MOUSE EVENTS ====================
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = MouseButtons.Left Then
            Dim trackLeft As Integer = 15
            Dim trackRight As Integer = Me.Width - 15
            Dim trackWidth As Integer = trackRight - trackLeft
            Dim trackY As Integer = 42
            Dim trackHeight As Integer = 12

            Dim clickedX As Integer = e.X
            Dim clickedY As Integer = e.Y

            ' Calculate visible range ONCE at start
            Dim visibleStartValue As Double = GetVisibleStartValue()
            Dim visibleEndValue As Double = GetVisibleEndValue()
            Dim visibleRange As Double = visibleEndValue - visibleStartValue

            ' Check scrollbar area for zoom navigation
           ' Check scrollbar area for zoom navigation
            If _zoomLevel > 1.0 Then
                Dim scrollBarY As Integer = trackY + trackHeight + 30
                Dim scrollBarHeight As Integer = 6

                If clickedY >= scrollBarY - 5 AndAlso clickedY <= scrollBarY + scrollBarHeight + 5 Then
                    _isDraggingScrollbar = True
                    _lastMouseX = clickedX
                    Me.Capture = True
                    Me.Cursor = Cursors.SizeWE

                    ' Hitung center berdasarkan posisi klik di scrollbar
                    Dim scrollPercentage As Double = (clickedX - trackLeft) / trackWidth
                    scrollPercentage = Math.Max(0.0, Math.Min(1.0, scrollPercentage))

                    ' Sesuaikan center agar thumb scrollbar berada di posisi klik
                    Dim totalRange As Double = _maximum - _minimum
                    _zoomCenter = scrollPercentage
                    _zoomCenter = Math.Max(0.0, Math.Min(1.0, _zoomCenter))

                    Invalidate()
                    RaiseEvent ZoomChanged(Me, EventArgs.Empty)
                    Return
                End If
            End If

            ' Track interaction area
            Dim trackAreaTop As Integer = trackY - 35
            Dim trackAreaBottom As Integer = trackY + trackHeight + 25

            ' ============ CEK PLAYHEAD DULU (PERBAIKAN) ============
            Dim playheadX As Integer = trackLeft + CInt(((_value - visibleStartValue) / visibleRange) * trackWidth)
            Dim isNearPlayhead As Boolean = Math.Abs(clickedX - playheadX) <= 15 AndAlso
                                            clickedY >= trackAreaTop AndAlso clickedY <= trackAreaBottom

            If isNearPlayhead Then
                ' Drag playhead
                _draggingMarker = 0
                _isDraggingPlayhead = True
                _lastMouseX = clickedX
                Me.Capture = True
                UpdateValueFromMouse(e.X)
                RaiseEvent PlayheadDragStart(Me, EventArgs.Empty)
                Me.Focus()
                Return
            End If

            ' ============ BARU CEK SEGMENT MARKERS ============
            For i As Integer = _segments.Count - 1 To 0 Step -1
                Dim seg As TrackSegment = _segments(i)

                If seg.EndValue >= visibleStartValue AndAlso seg.StartValue <= visibleEndValue Then
                    Dim startX As Integer = trackLeft + CInt(((seg.StartValue - visibleStartValue) / visibleRange) * trackWidth)
                    Dim endX As Integer = trackLeft + CInt(((seg.EndValue - visibleStartValue) / visibleRange) * trackWidth)

                    If startX < trackLeft Then startX = trackLeft
                    If endX > trackRight Then endX = trackRight

                    ' Check start marker
                    If Math.Abs(clickedX - startX) <= 15 AndAlso clickedY >= trackAreaTop AndAlso clickedY <= trackAreaBottom Then
                        _draggingMarker = 1
                        _isDraggingSegment = True
                        _selectedSegmentIndex = i
                        _lastMouseX = clickedX
                        Me.Cursor = Cursors.SizeWE
                        Me.Capture = True
                        Invalidate()
                        Return
                    End If

                    ' Check end marker
                    If Math.Abs(clickedX - endX) <= 15 AndAlso clickedY >= trackAreaTop AndAlso clickedY <= trackAreaBottom Then
                        _draggingMarker = 2
                        _isDraggingSegment = True
                        _selectedSegmentIndex = i
                        _lastMouseX = clickedX
                        Me.Cursor = Cursors.SizeWE
                        Me.Capture = True
                        Invalidate()
                        Return
                    End If
                End If
            Next

            ' ============ CEK SEGMENT AREA ============
            For i As Integer = _segments.Count - 1 To 0 Step -1
                Dim seg As TrackSegment = _segments(i)

                If seg.EndValue >= visibleStartValue AndAlso seg.StartValue <= visibleEndValue Then
                    Dim startX As Integer = trackLeft + CInt(((seg.StartValue - visibleStartValue) / visibleRange) * trackWidth)
                    Dim endX As Integer = trackLeft + CInt(((seg.EndValue - visibleStartValue) / visibleRange) * trackWidth)

                    If startX < trackLeft Then startX = trackLeft
                    If endX > trackRight Then endX = trackRight

                    If clickedX >= startX + 15 AndAlso clickedX <= endX - 15 AndAlso _
                       clickedY >= trackAreaTop AndAlso clickedY <= trackAreaBottom Then
                        _selectedSegmentIndex = i
                        Invalidate()
                        RaiseEvent SegmentClicked(Me, i)
                        Return
                    End If
                End If
            Next

            ' ============ KLIK DI TRACK AREA UNTUK PINDAHKAN PLAYHEAD ============
            If clickedY >= trackAreaTop AndAlso clickedY <= trackAreaBottom Then
                _draggingMarker = 0
                _isDraggingPlayhead = True
                _lastMouseX = clickedX
                Me.Capture = True
                UpdateValueFromMouse(e.X)
                RaiseEvent PlayheadDragStart(Me, EventArgs.Empty)
                RaiseEvent PlayheadDragging(Me, EventArgs.Empty)
                Me.Focus()
            End If
        ElseIf e.Button = MouseButtons.Right Then
            ZoomIn()
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)

        Dim trackLeft As Integer = 15
        Dim trackRight As Integer = Me.Width - 15
        Dim trackWidth As Integer = trackRight - trackLeft
        Dim trackY As Integer = 42
        Dim trackHeight As Integer = 12

        Dim trackAreaTop As Integer = trackY - 35
        Dim trackAreaBottom As Integer = trackY + trackHeight + 25

        Dim visibleStartValue As Double = GetVisibleStartValue()
        Dim visibleEndValue As Double = GetVisibleEndValue()
        Dim visibleRange As Double = visibleEndValue - visibleStartValue

        If e.Button = MouseButtons.Left Then
            If _isDraggingScrollbar Then
                Dim totalRange As Double = _maximum - _minimum
                Dim scrollPercentage As Double = (e.X - trackLeft) / trackWidth
                scrollPercentage = Math.Max(0.0, Math.Min(1.0, scrollPercentage))
                _zoomCenter = scrollPercentage
                _lastMouseX = e.X
                Invalidate()
                RaiseEvent ZoomChanged(Me, EventArgs.Empty)
                Return
            End If

            If _isDraggingPlayhead Then
                UpdateValueFromMouse(e.X)
                _lastMouseX = e.X
                RaiseEvent PlayheadDragging(Me, EventArgs.Empty)
                Return
            ElseIf _draggingMarker = 1 AndAlso _selectedSegmentIndex >= 0 AndAlso _selectedSegmentIndex < _segments.Count Then
                Dim percentage As Double = (e.X - trackLeft) / trackWidth
                If percentage < 0 Then percentage = 0
                If percentage > 1 Then percentage = 1

                Dim newValue As Double = visibleStartValue + percentage * visibleRange
                Dim seg As TrackSegment = _segments(_selectedSegmentIndex)
                If newValue < seg.EndValue - Math.Max(1, visibleRange / trackWidth * 5) Then
                    seg.StartValue = newValue
                    _segments(_selectedSegmentIndex) = seg
                    _lastMouseX = e.X
                    Invalidate()
                    RaiseEvent SegmentChanged(Me, _selectedSegmentIndex, seg.StartValue, seg.EndValue)
                End If
                Return
            ElseIf _draggingMarker = 2 AndAlso _selectedSegmentIndex >= 0 AndAlso _selectedSegmentIndex < _segments.Count Then
                Dim percentage As Double = (e.X - trackLeft) / trackWidth
                If percentage < 0 Then percentage = 0
                If percentage > 1 Then percentage = 1

                Dim newValue As Double = visibleStartValue + percentage * visibleRange
                Dim seg As TrackSegment = _segments(_selectedSegmentIndex)
                If newValue > seg.StartValue + Math.Max(1, visibleRange / trackWidth * 5) Then
                    seg.EndValue = newValue
                    _segments(_selectedSegmentIndex) = seg
                    _lastMouseX = e.X
                    Invalidate()
                    RaiseEvent SegmentChanged(Me, _selectedSegmentIndex, seg.StartValue, seg.EndValue)
                End If
                Return
            End If
        Else
            ' Update cursor - PRIORITASKAN PLAYHEAD
            Dim mouseX As Integer = e.X
            Dim mouseY As Integer = e.Y

            Dim isOverPlayhead As Boolean = False
            Dim isOverMarker As Boolean = False
            Dim isOverScrollbar As Boolean = False

            ' Check scrollbar
            If _zoomLevel > 1.0 Then
                Dim scrollBarY As Integer = trackY + trackHeight + 30
                Dim scrollBarHeight As Integer = 6
                If mouseY >= scrollBarY - 5 AndAlso mouseY <= scrollBarY + scrollBarHeight + 5 Then
                    isOverScrollbar = True
                End If
            End If

            ' CEK PLAYHEAD DULU (PRIORITAS)
            If Not isOverScrollbar Then
                Dim playheadX As Integer = trackLeft + CInt(((_value - visibleStartValue) / visibleRange) * trackWidth)
                If Math.Abs(mouseX - playheadX) <= 15 AndAlso mouseY >= trackAreaTop AndAlso mouseY <= trackAreaBottom Then
                    isOverPlayhead = True
                End If
            End If

            ' CEK MARKER (HANYA JIKA TIDAK OVER PLAYHEAD)
            If Not isOverScrollbar AndAlso Not isOverPlayhead Then
                For i As Integer = 0 To _segments.Count - 1
                    Dim seg As TrackSegment = _segments(i)
                    If seg.EndValue >= visibleStartValue AndAlso seg.StartValue <= visibleEndValue Then
                        Dim startX As Integer = trackLeft + CInt(((seg.StartValue - visibleStartValue) / visibleRange) * trackWidth)
                        Dim endX As Integer = trackLeft + CInt(((seg.EndValue - visibleStartValue) / visibleRange) * trackWidth)

                        If startX < trackLeft Then startX = trackLeft
                        If endX > trackRight Then endX = trackRight

                        If (Math.Abs(mouseX - startX) <= 15 OrElse Math.Abs(mouseX - endX) <= 15) AndAlso _
                           mouseY >= trackAreaTop AndAlso mouseY <= trackAreaBottom Then
                            isOverMarker = True
                            Exit For
                        End If
                    End If
                Next
            End If

            ' Set cursor berdasarkan prioritas
            If isOverScrollbar Then
                Me.Cursor = Cursors.SizeWE
            ElseIf isOverPlayhead Then
                Me.Cursor = Cursors.SizeWE
            ElseIf isOverMarker Then
                Me.Cursor = Cursors.SizeWE
            Else
                Me.Cursor = Cursors.Hand
            End If
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)

        If _isDraggingScrollbar Then
            _isDraggingScrollbar = False
            Me.Capture = False
            Me.Cursor = Cursors.Hand
            Invalidate()
            Return
        End If

        If _isDraggingPlayhead Then
            _isDraggingPlayhead = False
            Me.Capture = False
            Me.Cursor = Cursors.Hand
            RaiseEvent PlayheadDragEnd(Me, EventArgs.Empty)
            RaiseEvent Scroll(Me, EventArgs.Empty)
            Invalidate()
        End If

        If _draggingMarker >= 0 Then
            _draggingMarker = -1
            _isDraggingSegment = False
            Me.Capture = False
            Me.Cursor = Cursors.Hand
            RaiseEvent Scroll(Me, EventArgs.Empty)
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        MyBase.OnMouseWheel(e)

        ' Cek apakah mouse berada di area trackbar
        Dim trackLeft As Integer = 15
        Dim trackRight As Integer = Me.Width - 15
        Dim trackWidth As Integer = trackRight - trackLeft
        Dim trackY As Integer = 42
        Dim trackHeight As Integer = 12

        ' Cek apakah mouse berada di dalam control
        Dim mouseX As Integer = e.X
        Dim mouseY As Integer = e.Y

        If mouseX < 0 OrElse mouseX > Me.Width OrElse mouseY < 0 OrElse mouseY > Me.Height Then
            Return ' Mouse di luar control, abaikan
        End If

        ' Hitung posisi mouse relatif terhadap track
        Dim percentage As Double = (mouseX - trackLeft) / trackWidth
        percentage = Math.Max(0.0, Math.Min(1.0, percentage))

        ' Hitung value di posisi mouse
        Dim visibleStartValue As Double = GetVisibleStartValue()
        Dim visibleEndValue As Double = GetVisibleEndValue()
        Dim currentVisibleRange As Double = visibleEndValue - visibleStartValue
        Dim valueAtMouse As Double = visibleStartValue + percentage * currentVisibleRange

        ' Simpan zoom level lama
        Dim oldZoomLevel As Double = _zoomLevel

        ' Zoom in/out dengan step
        If e.Delta > 0 Then
            ' Zoom in (scroll ke atas)
            _zoomLevel = Math.Min(_maxZoomLevel, _zoomLevel * _zoomStep)
        Else
            ' Zoom out (scroll ke bawah)
            _zoomLevel = Math.Max(_minZoomLevel, _zoomLevel / _zoomStep)
        End If

        ' Jika zoom level berubah
        If _zoomLevel <> oldZoomLevel Then
            If _zoomLevel <= 1.0 Then
                ' Reset center jika zoom penuh
                _zoomCenter = 0.5
            Else
                ' Hitung center baru agar value di mouse tetap di posisi yang sama
                Dim totalRange As Double = _maximum - _minimum
                If totalRange > 0 Then
                    Dim newVisibleRange As Double = totalRange / _zoomLevel
                    Dim newCenterValue As Double = valueAtMouse - (percentage - 0.5) * newVisibleRange
                    _zoomCenter = newCenterValue / totalRange
                    _zoomCenter = Math.Max(0.0, Math.Min(1.0, _zoomCenter))
                End If
            End If

            ' Update tampilan
            Invalidate()
            RaiseEvent ZoomChanged(Me, EventArgs.Empty)
        End If
    End Sub
    Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
        ' Mendukung arrow keys untuk scroll saat zoom
        If keyData = Keys.Left OrElse keyData = Keys.Right OrElse _
           keyData = Keys.Up OrElse keyData = Keys.Down Then
            Return True
        End If
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        If _zoomLevel > 1.0 Then
            Select Case e.KeyCode
                Case Keys.Left
                    ' Scroll ke kiri
                    ScrollView(-0.1)
                    e.Handled = True
                Case Keys.Right
                    ' Scroll ke kanan
                    ScrollView(0.1)
                    e.Handled = True
                Case Keys.Up
                    ' Zoom in
                    ZoomIn()
                    e.Handled = True
                Case Keys.Down
                    ' Zoom out
                    ZoomOut()
                    e.Handled = True
                Case Keys.Home
                    ' Scroll ke awal
                    ScrollToValue(_minimum)
                    e.Handled = True
                Case Keys.End
                    ' Scroll ke akhir
                    ScrollToValue(_maximum)
                    e.Handled = True
            End Select
        Else
            ' Jika tidak zoom, arrow keys untuk navigasi playhead
            Select Case e.KeyCode
                Case Keys.Left
                    Value = Math.Max(_minimum, Value - 1)
                    e.Handled = True
                Case Keys.Right
                    Value = Math.Min(_maximum, Value + 1)
                    e.Handled = True
            End Select
        End If
    End Sub

    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        MyBase.OnMouseEnter(e)

        ' Pastikan control mendapatkan focus untuk menerima scroll events
        If Not Me.Focused Then
            Me.Focus()
        End If
    End Sub
    Protected Overrides Sub OnMouseHover(ByVal e As EventArgs)
        MyBase.OnMouseHover(e)

        ' Pastikan control dalam keadaan aktif
        If Me.CanFocus AndAlso Not Me.Focused Then
            Me.Focus()
        End If
    End Sub
    ' Update value from mouse position considering zoom
    Private Sub UpdateValueFromMouse(ByVal mouseX As Integer)
        Dim trackLeft As Integer = 15
        Dim trackRight As Integer = Me.Width - 15
        Dim trackWidth As Integer = trackRight - trackLeft

        If trackWidth > 0 Then
            Dim percentage As Double = (mouseX - trackLeft) / trackWidth
            If percentage < 0 Then percentage = 0
            If percentage > 1 Then percentage = 1

            ' Convert percentage to value considering zoom
            Dim visibleStartValue As Double = GetVisibleStartValue()
            Dim visibleEndValue As Double = GetVisibleEndValue()
            Dim visibleRange As Double = visibleEndValue - visibleStartValue

            Value = CInt(visibleStartValue + percentage * visibleRange)
        End If
    End Sub

    ' Add property for video duration
    Private _videoDuration As Double = 0

    Public Property VideoDuration() As Double
        Get
            Return _videoDuration
        End Get
        Set(ByVal value As Double)
            _videoDuration = value
            Invalidate()
        End Set
    End Property
End Class