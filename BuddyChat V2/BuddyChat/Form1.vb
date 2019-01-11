Imports System.Net
Imports System.Text
Imports System.Net.Sockets
Imports System.Threading

Public Class Form1
    Dim UdpClient As New UdpClient
    Dim GLOIP As IPAddress
    Dim GLOINTPORT As Integer
    Dim bytCommand As Byte() = New Byte() {}
    Dim returnData As String

    Public receivingUdpClient As UdpClient
    Public RemoteIpEndPoint As New IPEndPoint(IPAddress.Any, 0)
    Public ThreadReceive As Thread
    Public ReceivedMessage As String
    'flag to check if a new message comes
    Public Received As Boolean
    Public ListenPort As Integer
    Public run_thread As Boolean = True

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ' Get Buddy's IP
        GLOIP = IPAddress.Parse(BuddyIPBox.Text)
        ' Default port is 12345
        GLOINTPORT = BuddyPort.Text
        ' Connect
        UdpClient.Connect(GLOIP, GLOINTPORT)
        ' Get message from I say box, encode it
        bytCommand = Encoding.ASCII.GetBytes(ISay_Box.Text)
        'Send
        UdpClient.Send(bytCommand, bytCommand.Length)
        'add your message to the conversation box
        Conversation_ListBox.Items.Add("I Say: " + ISay_Box.Text)
        'Clear the I say box
        ISay_Box.Text = " "


    End Sub

    Private Sub ReceiveMessages()
        'Creates a UdpClient for reading incoming data
        Dim receivingUdpClient As New UdpClient(ListenPort)

        'Loop forever in order to receive message from internet host.
        Do While run_thread
            Try
                'blocks until a message returns on this socket from a remote host.
                'Creates an IPEndPoint to record the IP address and port number of the sender
                'The IPEndPoint will allow you to read datagrams sent from any source
                Dim receiveBytes As [Byte]() = receivingUdpClient.Receive(RemoteIpEndPoint)
                'convert the received message to human readable format
                returnData = Encoding.ASCII.GetString(receiveBytes)
                'RemoteIdEndPoint is your buddy's IP address
                ReceivedMessage = RemoteIpEndPoint.Address.ToString() + "says: " + returnData.ToString()
                'a message is received, set flag to true
                Received = True
                'Ask program to update the output
                Me.Invalidate()
            Catch e As Exception
                If returnData = "monkey" Then
                    bytCommand = Encoding.ASCII.GetBytes(ISay_Box.Text)
                    Conversation_ListBox.Items.Add("I Say: banana")
                    UdpClient.Send(bytCommand, bytCommand.Length)
                End If
                ReceivedMessage = e.ToString()
                Me.Invalidate()
            End Try
        Loop



    End Sub

    Private Sub Form1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        'We will update the list box only when a new message comes
        If (Received) Then
            'Reset the received message to false after we display it!
            Received = False
            Conversation_ListBox.Items.Add(ReceivedMessage)
        End If

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If (ListenPortBox.Text <> "") Then
            ListenPort = ListenPortBox.Text
            'Create a receive thread
            ThreadReceive = New Thread(AddressOf ReceiveMessages)
            Received = False
            ThreadReceive.Start()
            Button2.Enabled = False
        Else
            Conversation_ListBox.Items.Add("Must set proper listening port!!")
        End If
    End Sub
End Class
