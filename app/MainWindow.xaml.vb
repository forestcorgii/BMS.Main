Imports utility_service

Class MainWindow

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DatabaseConfiguration = New utility_service.Configuration.MysqlConfiguration()
        DatabaseConfiguration.Setup("ACCOUNTING_DB_URL")

        DatabaseManager = New Manager.Mysql
        DatabaseManager.Connection = DatabaseConfiguration.ToMysqlConnection

        SetupUserAuthentication()

        tbProfile.Text = User.Fullname
    End Sub

    Private Sub btnJournalEntry_Checked(sender As Object, e As RoutedEventArgs)
        If frmMain Is Nothing Then Exit Sub
        Select Case sender.name
            Case btnTitle.Name
                UncheckAllMenu()
                frmMain.Navigate(New Home)

            Case btnVoucher.Name
                frmMain.Navigate(New Voucher)

            Case btnSettings.Name
                frmMain.Navigate(New Configuration)
        End Select

        e.Handled = True
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        frmMain.Navigate(New Home)
    End Sub

    Private Sub UncheckAllMenu()
        btnVoucher.IsChecked = False
        btnSettings.IsChecked = False
    End Sub

    Private Sub mnChangePassword_Click(sender As Object, e As RoutedEventArgs)
        Dim ChangePasswordForm As New ChangePassword(User, DatabaseManager)
        If ChangePasswordForm.ShowDialog() Then
            'Show a Message Box
        End If
    End Sub

    Private Sub mnLogout_Click(sender As Object, e As RoutedEventArgs)
        Environment.SetEnvironmentVariable("PAYABLE_SYSTEM_AUTH_TOKEN", "")
        If SetupUserAuthentication() Then Close()
    End Sub
    Private Sub mnExit_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub
End Class
