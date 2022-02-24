Imports books_service

Public Class PrintProgress

    Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Sub New(VouchersForPrint As List(Of Model.Voucher))
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Vouchers = VouchersForPrint
    End Sub

    Public Vouchers As List(Of Model.Voucher)


    Private Sub PrintProgress_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        StartPrinting()
    End Sub

    Private Sub StartPrinting()
        DatabaseManager.Connection.Open()
        pbPrintProgress.Maximum = Vouchers.Count
        pbPrintProgress.Value = 0
        For Each voucher As Model.Voucher In Vouchers
            tbVoucherDetail.Text = String.Format("{0} - {1}", voucher.Voucher_No, voucher.Supplier_Payee)
            Controller.Voucher.Print(voucher, System.AppDomain.CurrentDomain.BaseDirectory)
            Controller.Voucher.ChangePrintStatus(voucher, Model.Voucher.PrintStatusChoices.PRINTED, DatabaseManager.Connection)
            pbPrintProgress.Value += 1
        Next
        DatabaseManager.Connection.Close()
        Close()
    End Sub

End Class
