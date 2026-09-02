Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

' Modern ListBox class
Public Class ModernListBox
    Inherits ListBox

    Public Sub New()
        MyBase.New()
        Me.DrawMode = DrawMode.OwnerDrawFixed
        Me.ItemHeight = 45
        Me.BorderStyle = BorderStyle.None
        Me.BackColor = Color.FromArgb(45, 45, 48)
        Me.ForeColor = Color.White
        Me.Font = New Font("Segoe UI", 9, FontStyle.Regular)
    End Sub

    Protected Overrides Sub OnDrawItem(ByVal e As DrawItemEventArgs)
        If e.Index < 0 Then Return

        e.DrawBackground()
        e.DrawFocusRectangle()

        ' Colors for each segment
        Dim colors As Color() = {Color.FromArgb(65, 180, 90), Color.FromArgb(65, 130, 220), _
                                 Color.FromArgb(230, 150, 70), Color.FromArgb(180, 80, 180), _
                                 Color.FromArgb(70, 190, 190), Color.FromArgb(200, 200, 80)}
        Dim colorIndex As Integer = e.Index Mod colors.Length
        Dim segmentColor As Color = colors(colorIndex)

        ' Background
        Dim isSelected As Boolean = (e.State And DrawItemState.Selected) = DrawItemState.Selected
        If isSelected Then
            e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(60, segmentColor)), e.Bounds)
        Else
            e.Graphics.FillRectangle(New SolidBrush(If(e.Index Mod 2 = 0, Color.FromArgb(50, 50, 53), Color.FromArgb(55, 55, 58))), e.Bounds)
        End If

        ' Color bar on left side
        Dim colorBarRect As New Rectangle(e.Bounds.X, e.Bounds.Y, 5, e.Bounds.Height)
        e.Graphics.FillRectangle(New SolidBrush(segmentColor), colorBarRect)

        ' Icon/number circle
        Dim circleRect As New Rectangle(e.Bounds.X + 15, e.Bounds.Y + (e.Bounds.Height - 25) \ 2, 25, 25)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        e.Graphics.FillEllipse(New SolidBrush(segmentColor), circleRect)
        e.Graphics.DrawEllipse(New Pen(Color.White, 1), circleRect)

        ' Number text
        Dim numberText As String = (e.Index + 1).ToString()
        Dim numberFont As New Font("Segoe UI", 9, FontStyle.Bold)
        Dim numberBrush As New SolidBrush(Color.White)
        Dim numberFormat As New StringFormat()
        numberFormat.Alignment = StringAlignment.Center
        numberFormat.LineAlignment = StringAlignment.Center
        e.Graphics.DrawString(numberText, numberFont, numberBrush, circleRect, numberFormat)
        numberFont.Dispose()
        numberBrush.Dispose()
        numberFormat.Dispose()

        ' Segment text - use reflection to access properties
        If e.Index < Items.Count Then
            Dim item As Object = Items(e.Index)
            Dim segText As String = item.ToString()
            Dim textRect As New Rectangle(e.Bounds.X + 50, e.Bounds.Y + 8, e.Bounds.Width - 60, 18)
            Dim textFont As New Font("Segoe UI", 9, FontStyle.Bold)
            e.Graphics.DrawString(segText, textFont, New SolidBrush(Color.White), textRect)
            textFont.Dispose()

            ' Duration text - use reflection to access StartTime and EndTime properties
            Try
                Dim startTime As Double = CDbl(item.GetType().GetProperty("StartTime").GetValue(item, Nothing))
                Dim endTime As Double = CDbl(item.GetType().GetProperty("EndTime").GetValue(item, Nothing))
                Dim duration As Double = endTime - startTime
                Dim durationText As String = String.Format("Duration: {0:00}:{1:00}", TimeSpan.FromSeconds(duration).Minutes, TimeSpan.FromSeconds(duration).Seconds)
                Dim durationRect As New Rectangle(e.Bounds.X + 50, e.Bounds.Y + 26, e.Bounds.Width - 60, 15)
                Dim durationFont As New Font("Segoe UI", 8, FontStyle.Regular)
                e.Graphics.DrawString(durationText, durationFont, New SolidBrush(Color.FromArgb(180, 180, 180)), durationRect)
                durationFont.Dispose()
            Catch ex As Exception
                ' If properties not found, ignore
            End Try
        End If

        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.Default
    End Sub
End Class