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
    End Sub
    Private Function SetupAutoComplete()
        DatabaseManager.Connection.Open()
        Dim _companies As List(Of Model.Company) = Controller.Company.LoadCompanies(DatabaseManager).ToList
        Dim _suppliers As List(Of Model.Supplier) = Controller.Supplier.LoadSuppliers(DatabaseManager).ToList
        DatabaseManager.Connection.Close()

        Dim autoCompleteSource As New Forms.AutoCompleteStringCollection
        For Each _supplier As Model.Supplier In _suppliers
            autoCompleteSource.Add(_supplier.Payee)
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
        lstVoucher.ItemsSource = Controller.Voucher.LoadVouchers(DatabaseManager, tbSearch.Text, completeDetail:=True)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs)
        Console.WriteLine(lstVoucher.SelectedItem)
        Controller.Voucher.Print(lstVoucher.SelectedItem, New IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName & "\Data\")
    End Sub

    Private Sub btnViewDetail_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedVoucher As Model.Voucher = DirectCast(lstVoucher.SelectedItem, Model.Voucher)
        Controller.Voucher.CompleteVoucherDetail(selectedVoucher, DatabaseManager)
        NavigationService.Navigate(New AddVoucher(selectedVoucher))
    End Sub


    Private Sub btnAddVoucher_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New AddVoucher())
    End Sub
    Private Sub btnVoucherSchedule_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New VoucherSchedule())
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
