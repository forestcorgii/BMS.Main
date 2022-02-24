Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports utility_service
Imports books_service

Class Voucher


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Voucher_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        SetupAutoComplete()

        DatabaseManager.Connection.Open()
        LoadVouchers()
        DatabaseManager.Connection.Close()
        'cbCompany.SelectedIndex = 0
        'cbSupplier.SelectedIndex = 0
    End Sub
    Private Function SetupAutoComplete()
        'cbCompany.Items.Clear()
        'cbSupplier.Items.Clear()
        DatabaseManager.Connection.Open()
        Dim _companies As List(Of Model.Company) = Controller.Company.LoadCompanies(DatabaseManager).ToList
        Dim _suppliers As List(Of Model.Supplier) = Controller.Supplier.LoadSuppliers(DatabaseManager).ToList
        DatabaseManager.Connection.Close()

        Dim autoCompleteSource As New Forms.AutoCompleteStringCollection
        For Each _supplier As Model.Supplier In _suppliers
            autoCompleteSource.Add(_supplier.Name)
        Next
        For Each _company As Model.Company In _companies
            autoCompleteSource.Add(_company.Name)
        Next
        With tbSearch
            .AutoCompleteCustomSource = autoCompleteSource
            .AutoCompleteMode = Forms.AutoCompleteMode.SuggestAppend
            .AutoCompleteSource = Forms.AutoCompleteSource.CustomSource
        End With

        Return True
    End Function
    Private Sub LoadVouchers()
        lstVoucher.ItemsSource = Controller.Voucher.LoadVouchers(DatabaseManager, tbSearch.Text)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs)
        Console.WriteLine(lstVoucher.SelectedItem)
        Controller.Voucher.Print(lstVoucher.SelectedItem, System.AppDomain.CurrentDomain.BaseDirectory)
    End Sub

    Private Sub btnViewDetail_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedVoucher As Model.Voucher = DirectCast(lstVoucher.SelectedItem, Model.Voucher)
        Controller.Voucher.CompleteVoucherDetail(selectedVoucher, DatabaseManager)
        NavigationService.Navigate(New AddVoucher(selectedVoucher))
    End Sub

    Private Sub btnAddToTemplate_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedVoucher As Model.Voucher = DirectCast(lstVoucher.SelectedItem, Model.Voucher)
        Controller.Voucher.CompleteVoucherDetail(selectedVoucher, DatabaseManager)
        'selectedVoucher.CompleteVoucherDetail(DatabaseManager)

        DatabaseManager.Connection.Open()
        Try
            Dim newVoucherTemplate As New Model.VoucherTemplate
            With newVoucherTemplate
                selectedVoucher.Id = 0
                .Voucher = selectedVoucher
                .Remarks = ""
                .ChangeState = States.ChangeState.Added

                Controller.VoucherTemplate.SaveVoucherTemplate(newVoucherTemplate, DatabaseManager.Connection)
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        MessageBox.Show("done")
        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnAddVoucher_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New AddVoucher())
    End Sub
    Private Sub btnVoucherTemplate_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New VoucherTemplate())
    End Sub
    Private Sub btnVoucherSchedule_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New VoucherSchedule())
    End Sub

    Private Sub tbSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
        '     If Not DatabaseManager.Connection.State = System.Data.ConnectionState.Open Then DatabaseManager.Connection.Open()

        'LoadVouchers()
    End Sub

    Private Sub tbSearch_LostFocus(sender As Object, e As RoutedEventArgs)
        If DatabaseManager.Connection.State = System.Data.ConnectionState.Open Then DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnPrintVoucher_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New Printer)
    End Sub


    Private Sub btnSearch_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        LoadVouchers()
        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        lstVoucher.SelectedItem.ChangePrintStatus(Model.Voucher.PrintStatusChoices.CANCELLED, DatabaseManager.Connection)
        lstVoucher.SelectedItem.print_status = Model.Voucher.PrintStatusChoices.CANCELLED
        DatabaseManager.Connection.Close()
    End Sub
End Class
