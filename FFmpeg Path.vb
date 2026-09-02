Public Class FFmpeg_Path

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        OpenFileDialog1.Filter = "ffmpeg.exe|ffmpeg.exe"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            If String.Equals(System.IO.Path.GetFileName(OpenFileDialog1.FileName),
                             "ffmpeg.exe",
                             StringComparison.OrdinalIgnoreCase) Then

                TextBox1.Text = OpenFileDialog1.FileName

            Else
            End If
        End If
    End Sub

    Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        OpenFileDialog2.Filter = "ffprobe.exe|ffprobe.exe"

        If OpenFileDialog2.ShowDialog() = DialogResult.OK Then
            If String.Equals(System.IO.Path.GetFileName(OpenFileDialog2.FileName),
                             "ffprobe.exe",
                             StringComparison.OrdinalIgnoreCase) Then

                TextBox2.Text = OpenFileDialog2.FileName

            Else
            End If
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Form1.ffmpegPath = TextBox1.Text
        Form1.ffprobePath = TextBox2.Text
        My.Settings.ffmpegpath = TextBox1.Text
        My.Settings.ffprobepath = TextBox2.Text
        My.Settings.Save()
        Me.Close()
    End Sub
End Class