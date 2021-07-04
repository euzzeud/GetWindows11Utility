Imports System.Net, System.IO, System.Web
Public Class Form1
    Dim wc As New WebClient()
    Dim filename As String = "\Windows_11.iso"
    Dim requiredSpace As Long = 4874553344 / 1000000000
    Dim downloadInProgress As Boolean = False
    Function GetFileContent(link)
        Dim sharingURL As String = link
        Dim base64Value As String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sharingURL))
        Dim encodedURL As String = "u!" & base64Value.TrimEnd("="c).Replace("/"c, "_"c).Replace("+"c, "-"c)
        Dim resultURL As String = String.Format("https://api.onedrive.com/v1.0/shares/{0}/root/content", encodedURL)
        Return resultURL
    End Function

    Public Sub DownloadISOFile()
        Try
            Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(GetFileContent("https://1drv.ms/t/s!At2YLE8H_9mBhHwfJbur_mP3IfnX?e=vk9Iu3"))
            Dim response As System.Net.HttpWebResponse = request.GetResponse()
            Dim sr As New System.IO.StreamReader(response.GetResponseStream())

            AddHandler wc.DownloadProgressChanged, AddressOf ProgChanged
            wc.DownloadFileAsync(New Uri(GetFileContent(sr.ReadToEnd.ToString())), FolderBrowserDialog1.SelectedPath + filename)
            sr.Close()
        Catch ex As Exception
            If MessageBox.Show("Cannot find the URL of the ISO file on the server!" + vbNewLine + "Get Windows 11 utility will close...", "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error) = DialogResult.OK Then
                End
            End If
        End Try
    End Sub

    Sub DownloadCompleted()
        Me.Text = "Get Windows 11"
        ProgressBar1.Value = 0
        GroupBox1.Enabled = True
        GroupBox2.Enabled = False
        Button2.Enabled = True
        TextBox1.Text = "[none]"
        If MessageBox.Show("Download completed.", "Get Windows 11", MessageBoxButtons.OK, MessageBoxIcon.Information) = DialogResult.OK Then
            End
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
            GroupBox2.Enabled = True
            GroupBox1.Enabled = False

            Dim availableSpace As Long
            availableSpace = My.Computer.FileSystem.GetDriveInfo(FolderBrowserDialog1.SelectedPath).AvailableFreeSpace / 1000000000
            Label3.Text = "Required disk space: " + requiredSpace.ToString() + ".0 GB"
            Label4.Text = "Available disk space: " + availableSpace.ToString() + ".0 GB"
        Else
            Return
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If (My.Computer.FileSystem.GetDriveInfo(FolderBrowserDialog1.SelectedPath).AvailableFreeSpace / 1000000000 < requiredSpace) Then
            If MessageBox.Show("There is not enough storage space on the selected disk." + vbNewLine + "Get Windows 11 utility will close...", "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error) = DialogResult.OK Then
                End
            End If
        End If

            GroupBox1.Enabled = False
        Button2.Enabled = False
        DownloadISOFile()
    End Sub

    Private Sub ProgChanged(sender As Object, e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
        downloadInProgress = True
        Me.Text = "Get Windows 11 | " + e.ProgressPercentage.ToString() + "%"

        If (e.ProgressPercentage = 100) Then
            downloadInProgress = False
            DownloadCompleted()
        End If
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        MessageBox.Show("Created by Euzzeud#6040 on Discord.", "About Get Windows 11", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If (downloadInProgress = True) Then
            If (System.IO.File.Exists(FolderBrowserDialog1.SelectedPath + filename)) Then
                wc.CancelAsync()
                System.IO.File.Delete(FolderBrowserDialog1.SelectedPath + filename)
                End
            End If
        Else
            End
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        downloadInProgress = False
        If (My.Computer.Network.IsAvailable = False) Then
            If MessageBox.Show("You are not connected to the Internet... Make sure you have an active and stable internet connection to use Get Windows 11.", "Connection failed", MessageBoxButtons.OK, MessageBoxIcon.Error) = DialogResult.OK Then
                End
            End If
        End If
    End Sub
End Class