Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class Printer
    Public Vouchers As ObservableCollection(Of Model.Voucher)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        SetupAutoComplete()

        DatabaseManager.Connection.Open()
        LoadVouchers()
        DatabaseManager.Connection.Close()

        cbCompany.SelectedIndex = 0
        cbSupplier.SelectedIndex = 0
    End Sub
    Private Function SetupAutoComplete()
        DatabaseManager.Connection.Open()
        Try
            Using reader As MySqlDataReader = DatabaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.company;")
                If reader.HasRows Then
                    While reader.Read
                        cbCompany.Items.Add(reader.Item("name"))
                    End While
                Else : MessageBox.Show("There is no available Company to select, please finish setting up.", "", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End Using

            Dim autoCompleteSource As New Forms.AutoCompleteStringCollection
            Using reader As MySqlDataReader = DatabaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.supplier;")
                If reader.HasRows Then
                    While reader.Read
                        cbSupplier.Items.Add(reader.Item("payee"))
                    End While
                Else : MessageBox.Show("There is no available Supplier to select, please finish setting up.", "", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End Using
        Catch ex As Exception
            Return False
        End Try

        DatabaseManager.Connection.Close()

        Return True
    End Function
    Private Sub LoadVouchers()
        Dim query As String = "SELECT * FROM `voucher_entry`.voucher_complete WHERE print_status = 0;"
        If tbSearch.Text <> "" Or cbCompany.SelectedIndex > 0 Or cbSupplier.SelectedIndex > 0 Then
            Dim conjunction As String = ""
            If tbSearch.Text <> "" Then
                query = String.Format("SELECT * FROM `voucher_entry`.voucher_complete where voucher_no like '%{0}%' or supplier_account_number like '%{0}%' or bank_account_code like '%{0}%'", tbSearch.Text) : conjunction = "and"
            Else
                query = String.Format("SELECT * FROM `voucher_entry`.voucher_complete where", tbSearch.Text)
            End If

            If cbCompany.SelectedIndex > 0 Then
                query &= String.Format(" {0} company_name like '%{1}%'", conjunction, cbCompany.Text) : conjunction = "and"
            End If
            If cbSupplier.SelectedIndex > 0 Then
                query &= String.Format(" {0} supplier_payee like '%{1}%'", conjunction, cbSupplier.Text) : conjunction = "and"
            End If

            query &= String.Format(" {0} print_status = 0", conjunction)
        End If

        Vouchers = New ObservableCollection(Of Model.Voucher)
        Try
            Using reader As MySqlDataReader = DatabaseManager.ExecuteDataReader(query)
                If reader.HasRows Then
                    While reader.Read
                        Vouchers.Add(New Model.Voucher(reader))
                    End While
                Else 'prompt to add company
                End If
            End Using

            lstVoucher.ItemsSource = Vouchers
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub


    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub

    Private Sub chbSelectAll_Checked(sender As Object, e As RoutedEventArgs)
        lstVoucher.SelectAll()
    End Sub
    Private Sub chbSelectAll_UnChecked(sender As Object, e As RoutedEventArgs)
        lstVoucher.UnselectAll()
    End Sub
    Private Sub tbSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not DatabaseManager.Connection.State = System.Data.ConnectionState.Open Then DatabaseManager.Connection.Open()

        LoadVouchers()
    End Sub

    Private Sub tbSearch_LostFocus(sender As Object, e As RoutedEventArgs)
        If DatabaseManager.Connection.State = System.Data.ConnectionState.Open Then DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        LoadVouchers()
        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnPrintSelectedVoucher_Click(sender As Object, e As RoutedEventArgs)
        Dim _vouchers As List(Of Model.Voucher) = (From res In lstVoucher.SelectedItems Select DirectCast(res, Model.Voucher)).ToList
        Dim PrintProgressDlg As New PrintProgress(_vouchers)
        If PrintProgressDlg.ShowDialog() Then

        End If
    End Sub
End Class
