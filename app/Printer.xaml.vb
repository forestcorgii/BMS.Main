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

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub

    Private Sub chbSelectAll_Checked(sender As Object, e As RoutedEventArgs)
        lstVoucher.SelectAll()
    End Sub
    Private Sub chbSelectAll_UnChecked(sender As Object, e As RoutedEventArgs)
        lstVoucher.UnselectAll()
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
