Imports System.Net.NetworkInformation
Imports System.Threading
Imports System.IO

Public Class Form1
    Dim t As Thread
    Dim tstop As Boolean



    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        tstop = False
        If Not IsNumeric(TBIP.Text.Trim().Replace(".", "").Replace(" ", "")) Then
            Try
                TBIP.Text = System.Net.Dns.GetHostAddresses(TBIP.Text.Trim())(0).ToString
            Catch ex As Exception
                MessageBox.Show("Not a valid Domain")
                Return
            End Try

        End If

        t = New Thread(AddressOf Pings)
        ListBox1.Items.Clear()
        t.Start()

    End Sub

    Private Delegate Sub DelAddItem(ByVal ItemTitle As String)

    Private Sub AddItem(ByVal ItemTitle As String)
        ListBox1.Items.Insert(0, ItemTitle)
    End Sub

    Public Sub Pings()


        Dim myIP As String = TBIP.Text.Trim().Replace(" ", "")
        Dim timeoutvalue As Integer = 120
        Dim ips As System.Net.IPAddress
        Dim pingComputer As Ping = New Ping()
        Dim Cuenta As Integer = 1
        Dim Buenos As Integer = 0
        Dim Malos As Integer = 0
        Dim Miliseguntos As Integer = 0
        If System.Net.IPAddress.TryParse(myIP, ips) AndAlso myIP.Trim.IndexOf("0.0.0.0") = -1 Then
            Try
                If CheckBox1.Checked Then
                    writeFile("--------------------------------------------------------")
                    writeFile("")
                End If
                Do

                    Dim theReply As PingReply = pingComputer.Send(ips)

                    If theReply.Status = IPStatus.Success Then
                        If ListBox1.InvokeRequired Then
                            ListBox1.Invoke(New DelAddItem(AddressOf AddItem), Cuenta.ToString(" 0:") & theReply.Status.ToString() & " " & theReply.RoundtripTime & " Milisecond - " & DateAndTime.Now.ToString("h:mm:ss.fff tt"))
                        Else
                            ListBox1.Items.Add(Cuenta.ToString(" 0:") & theReply.Status.ToString() & " " & theReply.RoundtripTime & " Milisecond - " & DateAndTime.Now.ToString("h:mm:ss.fff tt"))
                        End If
                        If CheckBox1.Checked Then
                            writeFile(Cuenta.ToString(" 0:") & theReply.Status.ToString() & " " & theReply.RoundtripTime & " Milisecond - " & DateAndTime.Now.ToString("h:mm:ss.fff tt"))
                        End If
                        Buenos = Buenos + 1
                        Miliseguntos = Miliseguntos + theReply.RoundtripTime
                    Else
                        If ListBox1.InvokeRequired Then
                            ListBox1.Invoke(New DelAddItem(AddressOf AddItem), Cuenta.ToString(" 0:") & theReply.Status.ToString() & " - " & DateAndTime.Now.ToString("h:mm:ss.fff tt"))
                        Else
                            ListBox1.Items.Add(Cuenta.ToString(" 0:") & theReply.Status.ToString() & " - " & DateAndTime.Now.ToString("h:mm:ss.fff tt"))
                        End If
                        If CheckBox1.Checked Then
                            writeFile(Cuenta.ToString(" 0:") & theReply.Status.ToString() & " - " & DateAndTime.Now.ToString("h:mm:ss.fff tt"))
                        End If

                        Malos = Malos + 1


                    End If

                    Cuenta = Cuenta + 1
                    If tstop Then
                        Exit Do
                    End If
                    If NUDPING.Value > 0 Then
                        If Cuenta > NUDPING.Value Then
                            Exit Do
                        End If
                    End If


                    System.Threading.Thread.Sleep(NumericUpDown1.Value * 1000)
                Loop
            Catch ex As Exception
                ' MessageBox.Show(ex.Message)
            End Try


            ListBox1.Invoke(New DelAddItem(AddressOf AddItem), "")
            ListBox1.Invoke(New DelAddItem(AddressOf AddItem), "Send:" & Cuenta - 1 & "  Received:" & Buenos & "  Lost:" & Malos & "  (" & ((Malos / (Cuenta - 1))).ToString("#0%") & " Loss)")
            ListBox1.Invoke(New DelAddItem(AddressOf AddItem), "Average:" & (Miliseguntos / Buenos).ToString("0"))
            If CheckBox1.Checked Then
                writeFile("")
                writeFile("Send:" & Cuenta - 1 & "  Received:" & Buenos & "  Lost:" & Malos & "  (" & ((Malos / (Cuenta - 1))).ToString("#0%") & " Loss)")
                writeFile("Average:" & (Miliseguntos / Buenos).ToString("0"))
                writeFile("")
            End If
        Else
            MessageBox.Show("IP Not Valid")
        End If

    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If tstop = True Then
            t.Abort()
        End If
        If IsNothing(t) = False AndAlso t.IsAlive() Then
            Button2.Text = "Wait"
            tstop = True

        End If


    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        readfile()


    End Sub

    Private Sub TBIP_Enter(sender As Object, e As EventArgs) Handles TBIP.Enter
        If TBIP.Text.Trim = "0.0.0.0 or Domain" Then
            TBIP.Text = ""
            TBIP.ForeColor = Color.Black

        End If
    End Sub

    Private Sub TBIP_Leave(sender As Object, e As EventArgs) Handles TBIP.Leave
        If TBIP.Text.Trim = "0.0.0.0 or Domain" Or TBIP.Text.Trim = "" Then
            TBIP.Text = "0.0.0.0 or Domain"
            TBIP.ForeColor = Color.Gray

        End If
    End Sub

    Private Sub readfile()
        Dim ips As System.Net.IPAddress
        If File.Exists("ipfile.txt") Then

            Dim ReadFile As StreamReader = New StreamReader("ipfile.txt")
            Dim ip As String = ReadFile.ReadLine.Trim
            If ip.IndexOf("0.0.0.0") = -1 And System.Net.IPAddress.TryParse(ip, ips) Then
                TBIP.Text = ip
            Else
                TBIP.Text = "0.0.0.0 or Domain"
                TBIP.ForeColor = Color.Gray
            End If
            ReadFile.Close()
            ReadFile.Dispose()
        Else
            TBIP.Text = "0.0.0.0 or Domain"
            TBIP.ForeColor = Color.Gray
        End If

    End Sub


    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Dim ips As System.Net.IPAddress
        Dim WriteFile As StreamWriter = New StreamWriter("ipfile.txt", False)
        If TBIP.Text.Trim <> "0.0.0.0 or Domain" And System.Net.IPAddress.TryParse(TBIP.Text.Trim, ips) Then

            WriteFile.WriteLine(TBIP.Text.Trim)
        Else
            WriteFile.WriteLine("0.0.0.0 or Domain")
        End If
        WriteFile.Flush()
        WriteFile.Close()
        WriteFile.Dispose()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If IsNothing(t) = False AndAlso t.IsAlive Then
            TBIP.Enabled = False
            NUDPING.Enabled = False
            NumericUpDown1.Enabled = False
            Button1.Enabled = False
            Button2.Enabled = True
        Else
            TBIP.Enabled = True
            NUDPING.Enabled = True
            NumericUpDown1.Enabled = True
            Button1.Enabled = True
            Button2.Enabled = False
            Button2.Text = "Stop"
        End If
    End Sub


    Private Sub writeFile(Linea As String)
        If tbFile.Text IsNot "" Then

            Dim write As IO.StreamWriter = New IO.StreamWriter(tbFile.Text, True)
            write.WriteLine(Linea)
            write.Flush()
            write.Close()
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            btDialog.Enabled = True
        Else
            btDialog.Enabled = False
        End If
    End Sub

    Private Sub BtDialog_Click(sender As Object, e As EventArgs) Handles btDialog.Click

        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            tbFile.Text = SaveFileDialog1.FileName

        End If
    End Sub

    Private Sub NUDPING_ValueChanged(sender As Object, e As EventArgs) Handles NUDPING.ValueChanged
        If NUDPING.Value = 0 Then
            Label5.Visible = True
        Else
            Label5.Visible = False
        End If
    End Sub
End Class
