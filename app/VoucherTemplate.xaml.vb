Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class VoucherTemplate

    Public Property VoucherTemplates As ObservableCollection(Of Model.VoucherTemplate)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LoadVoucherTemplates()
    End Sub

    Private Sub LoadVoucherTemplates()
        DatabaseManager.Connection.Open()
        VoucherTemplates = Controller.VoucherTemplate.LoadVoucherTemplates(DatabaseManager)
        DatabaseManager.Connection.Close()

        lstVoucherTemplates.ItemsSource = VoucherTemplates
    End Sub
    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        For Each voucherTemplate In VoucherTemplates
            Controller.VoucherTemplate.SaveVoucherTemplate(voucherTemplate, DatabaseManager.Connection)
        Next
        DatabaseManager.Connection.Close()

        LoadVoucherTemplates()
        MessageBox.Show("Changes has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub lstVoucherTemplates_KeyDown(sender As Object, e As KeyEventArgs) Handles lstVoucherTemplates.PreviewKeyDown
        If e.Key = Key.Delete Then
            lstVoucherTemplates.SelectedItem.changestate = 2
            lstVoucherTemplates.Items.Refresh()
        End If
    End Sub

    Private Sub btnAddCompanyBankAccount_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New CompanyBankAccount)
    End Sub
    Private Sub btnViewDetail_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedVoucherTemplate As Model.VoucherTemplate = DirectCast(lstVoucherTemplates.SelectedItem, Model.VoucherTemplate)

        Controller.Voucher.CompleteVoucherDetail(selectedVoucherTemplate.Voucher, DatabaseManager)

        NavigationService.Navigate(New AddVoucher(selectedVoucherTemplate))
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub
End Class
