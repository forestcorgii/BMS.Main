Imports utility_service

Imports MySql.Data.MySqlClient
Imports books_service
Imports System.Collections.ObjectModel

Class AddVoucher

    Private Mode As ProcessTypeChoices

    Public Property Particulars As ObservableCollection(Of Model.ParticularsItem)
    Public Property JournalAccountDistributions As ObservableCollection(Of Model.JournalAccountDistributionItem)

    Private EditMode As EditModeChoices

    Private Voucher As Model.Voucher
    Private VoucherTemplate As Model.VoucherTemplate



    Private Companies As List(Of Model.Company)
    Private SupplierAccounts As List(Of Model.SupplierAccount)
    Private Suppliers As List(Of Model.Supplier)
    Private JournalAccounts As List(Of Model.JournalAccount)

#Region "Properties"
    Private __SupplierAccount As Model.SupplierAccount
    Private Property SupplierAccount As Model.SupplierAccount
        Get
            Return __SupplierAccount
        End Get
        Set(value As Model.SupplierAccount)
            __SupplierAccount = value
            If value.Account_Number <> "" Then tbSupplierAccount.Text = value.ToString
        End Set
    End Property

    Private __Supplier As Model.Supplier
    Private Property Supplier As Model.Supplier
        Get
            Return __Supplier
        End Get
        Set(value As Model.Supplier)
            __Supplier = value
            If value IsNot Nothing AndAlso value.Name <> "" Then tbSupplier.Text = value.ToString
        End Set
    End Property

    Private Property Company As Model.Company
        Get
            Return cbCompany.SelectedItem
        End Get
        Set(value As Model.Company)
            For i As Integer = 0 To cbCompany.Items.Count - 1
                If value.Id = cbCompany.Items(i).id Then
                    cbCompany.SelectedIndex = i
                End If
            Next
        End Set
    End Property

    Private Property Company_Bank_Account As Model.CompanyBankAccount
        Get
            Return cbBankAccount.SelectedItem
        End Get
        Set(value As Model.CompanyBankAccount)
            For i As Integer = 0 To cbBankAccount.Items.Count - 1
                If value.Id = cbBankAccount.Items(i).id Then
                    cbBankAccount.SelectedIndex = i
                End If
            Next
        End Set
    End Property

#End Region

    Sub New()
        InitializeComponent()

        Mode = ProcessTypeChoices.SETUP

        GeneralSetup()

        Mode = ProcessTypeChoices.STAND_BY
    End Sub

    Sub New(voucherForEdit As Model.Voucher)
        InitializeComponent()

        Mode = ProcessTypeChoices.SETUP

        GeneralSetup()
        Voucher = voucherForEdit
        PopulateVoucher(voucherForEdit)
        EditMode = EditModeChoices.Voucher

        Mode = ProcessTypeChoices.STAND_BY
    End Sub

    Sub New(template As Model.VoucherTemplate)
        InitializeComponent()

        Mode = ProcessTypeChoices.SETUP

        GeneralSetup()
        VoucherTemplate = template

        PopulateVoucher(template.Voucher)
        EditMode = EditModeChoices.Template

        Mode = ProcessTypeChoices.STAND_BY
    End Sub

    Private Sub GeneralSetup()

        JournalAccountDistributions = New ObservableCollection(Of Model.JournalAccountDistributionItem)

        dtEntryDate.SelectedDate = Now

        SetupAutoComplete()

    End Sub

    Private Function SetupAutoComplete()
        DatabaseManager.Connection.Open()

        cbCompany.ItemsSource = Controller.Company.LoadCompanies(DatabaseManager)
        cbJournalAccount.ItemsSource = Controller.JournalAccount.LoadJournalAccounts(DatabaseManager)

        Dim autoCompleteSource As New Forms.AutoCompleteStringCollection
        Suppliers = Controller.Supplier.LoadSuppliers(DatabaseManager).ToList
        For Each _supplier As Model.Supplier In Suppliers
            autoCompleteSource.Add(_supplier.Name & " - " & _supplier.TIN)
        Next

        With tbSupplier
            .AutoCompleteCustomSource = autoCompleteSource
            .AutoCompleteMode = Forms.AutoCompleteMode.Suggest
            .AutoCompleteSource = Forms.AutoCompleteSource.CustomSource
        End With

        autoCompleteSource = New Forms.AutoCompleteStringCollection
        SupplierAccounts = Controller.SupplierAccount.LoadSupplierAccounts(DatabaseManager).ToList
        For Each _supplierAccount As Model.SupplierAccount In SupplierAccounts
            autoCompleteSource.Add(_supplierAccount.Account_Number & " - " & _supplierAccount.Supplier_Id)
        Next

        With tbSupplierAccount
            .AutoCompleteCustomSource = autoCompleteSource
            .AutoCompleteMode = Forms.AutoCompleteMode.Suggest
            .AutoCompleteSource = Forms.AutoCompleteSource.CustomSource
        End With

        SetupVAT()

        DatabaseManager.Connection.Close()
        Return True
    End Function

    Private Sub SetupVAT()
        Dim vatRate As Double = 0.12

        cbVATType.ItemsSource = [Enum].GetValues(GetType(Model.JournalAccountDistributionItem.VATTypeChoices))
    End Sub

    Private Sub PopulateVoucher(voucherForEdit As Model.Voucher)
        If voucherForEdit.Entry_Date <> Nothing Then
            dtEntryDate.SelectedDate = voucherForEdit.Entry_Date
        End If

        lbId.Text = voucherForEdit.Id

        Company = voucherForEdit.Company

        Company_Bank_Account = voucherForEdit.Bank_Account

        Supplier = voucherForEdit.Supplier
        SupplierAccount = voucherForEdit.Supplier_Account

        'tbSupplierAccount.Text = voucherForEdit.Supplier_Account.Account_Number

        Particulars = New ObservableCollection(Of Model.ParticularsItem)(voucherForEdit.Particulars)
        lstParticulars.ItemsSource = Particulars
        JournalAccountDistributions = New ObservableCollection(Of Model.JournalAccountDistributionItem)(voucherForEdit.Journal_Account_Distributions)
        lstJournalAccounts.ItemsSource = JournalAccountDistributions

        tbRemarks.Text = voucherForEdit.Remarks
    End Sub

    Private Sub cbCompany_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCompany.SelectionChanged
        If e.AddedItems.Count > 0 Then
            Dim ConnectionIsOpenedFromOutside As Boolean = DatabaseManager.Connection.State = System.Data.ConnectionState.Open
            If Not ConnectionIsOpenedFromOutside Then DatabaseManager.Connection.Open()

            Try
                Dim company As Model.Company = DirectCast(e.AddedItems(0), Model.Company)
                cbBankAccount.ItemsSource = Controller.CompanyBankAccount.LoadCompanyBankAccounts(DatabaseManager, company.Id)

            Catch ex As Exception
                MessageBox.Show(ex.Message, "cbCompany_SelectionChanged", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            If Not ConnectionIsOpenedFromOutside Then DatabaseManager.Connection.Close()
        End If
    End Sub

    Private Sub tbSupplierAccount_TextChanged(sender As Object, e As EventArgs) Handles tbSupplierAccount.TextChanged
        If tbSupplierAccount.TextLength > 5 And Mode <> ProcessTypeChoices.BUSY Then
            Mode = ProcessTypeChoices.BUSY
            Dim supplierAccount_args As String() = tbSupplierAccount.Text.Split("-")
            If supplierAccount_args.Length > 1 Then 'tbSupplierAccount format for getting supplier detail is not yet valid.
                DatabaseManager.Connection.Open()
                Dim template As Model.Voucher = Nothing
                Try

                    SupplierAccount = Controller.SupplierAccount.GetSupplierAccount(DatabaseManager, supplierAccount_args(0).Trim, supplierAccount_args(1).Trim)

                    If SupplierAccount IsNot Nothing Then
                        template = Controller.VoucherTemplate.GetTemplate(DatabaseManager, SupplierAccount.Supplier_Id, SupplierAccount.Id)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "tbSupplierAccount_TextChanged", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try


                If template IsNot Nothing Then
                    Controller.Voucher.CompleteVoucherDetail(template, DatabaseManager)
                    PopulateVoucher(template)
                End If

                DatabaseManager.Connection.Close()
            End If
            Mode = ProcessTypeChoices.STAND_BY
        End If
    End Sub
    Private Sub tbSupplier_TextChanged(sender As Object, e As EventArgs) Handles tbSupplier.TextChanged
        If tbSupplier.TextLength > 5 And Mode <> ProcessTypeChoices.BUSY Then
            Mode = ProcessTypeChoices.BUSY
            Dim supplier_args As String() = tbSupplier.Text.Split("-")
            If supplier_args.Length > 1 Then  'tbSupplier format for getting supplier detail is not yet valid.
                DatabaseManager.Connection.Open()

                Dim template As Model.Voucher = Nothing
                Try
                    Supplier = Controller.Supplier.GetSupplier(supplier_args(0).Trim, supplier_args(1).Trim, DatabaseManager)
                    If Supplier IsNot Nothing Then
                        template = Controller.VoucherTemplate.GetTemplate(DatabaseManager, Supplier.Id, 0)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "tbSupplier_TextChanged", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try


                If template IsNot Nothing Then
                    Controller.Voucher.CompleteVoucherDetail(template, DatabaseManager)

                    tbSupplierAccount.Text = ""
                    PopulateVoucher(template)
                End If

                DatabaseManager.Connection.Close()
            End If
            Mode = ProcessTypeChoices.STAND_BY
        End If
    End Sub

    Private Sub btnAddJournalAccount_Click(sender As Object, e As RoutedEventArgs)
        If cbJournalAccount.SelectedItem IsNot Nothing And tbAmount.Text <> "" And
            tbWTaxRate.Text <> "" And
            cbVATType.SelectedItem IsNot Nothing Then

            Dim newJournalAccountDistributionItem As New Model.JournalAccountDistributionItem
            With newJournalAccountDistributionItem
                .Journal_Account = cbJournalAccount.SelectedItem
                .Amount = CInt(tbAmount.Text)
                .W_TAX = tbWTaxRate.Text
                .VAT = cbVATType.SelectedItem
            End With

            JournalAccountDistributions.Add(newJournalAccountDistributionItem)
            lstJournalAccounts.ItemsSource = JournalAccountDistributions

            cbJournalAccount.SelectedItem = Nothing
            tbAmount.Text = ""
            tbWTaxRate.Text = ""
            cbVATType.SelectedItem = 0
        Else
            MessageBox.Show("All Fields are Reqiured, input all Fields.", MessageBoxButton.OK, MessageBoxImage.Error)
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()

        If Supplier IsNot Nothing And SupplierAccount.Account_Number Is Nothing And tbSupplierAccount.Text <> "" Then
            If MessageBox.Show(String.Format("Account Number does not exist under {0}. It will be saved if You proceed, Proceed?", Supplier.Name), "Save as new Supplier Account", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) Then
                SupplierAccount = New Model.SupplierAccount() With {.Supplier = Supplier, .Account_Number = tbSupplierAccount.Text}
                Controller.SupplierAccount.SaveSupplierAccount(SupplierAccount, DatabaseManager.Connection)
                SupplierAccount = Controller.SupplierAccount.GetSupplierAccount(DatabaseManager, SupplierAccount.Account_Number, SupplierAccount.Supplier.Id)
            End If
        End If

        Try
            Dim newVoucher As New Model.Voucher
            With newVoucher
                .Id = lbId.Text

                .Company = DirectCast(cbCompany.SelectedItem, Model.Company)

                .Supplier = Supplier
                .Supplier_Account = SupplierAccount

                .Bank_Account = DirectCast(cbBankAccount.SelectedItem, Model.CompanyBankAccount)

                .Journal_Account_Distributions = JournalAccountDistributions.ToList
                .Particulars = Particulars.ToList

                .Remarks = tbRemarks.Text
                .Entry_Date = dtEntryDate.SelectedDate

                .Print_Status = Model.Voucher.PrintStatusChoices.NOT_PRINTED

                .ChangeState = States.ChangeState.Added
                If lbId.Text <> "" And lbId.Text <> "0" Then
                    .Id = lbId.Text
                    .ChangeState = States.ChangeState.None
                End If

                If EditMode = EditModeChoices.Voucher Then Controller.Voucher.SaveVoucher(newVoucher, DatabaseManager.Connection, User)
            End With

            If MessageBox.Show("Add Voucher as Template?", "Save as Template", MessageBoxButton.YesNo, MessageBoxImage.Question) Then
                Dim newVoucherTemplate As New Model.VoucherTemplate
                With newVoucherTemplate
                    .Voucher = Controller.Voucher.ToTemplate(newVoucher)
                    .Supplier_Id = newVoucher.Supplier_Id
                    .Supplier_Account_Id = newVoucher.Supplier_Account_Id
                    .ChangeState = States.ChangeState.Added

                    Controller.VoucherTemplate.SaveVoucherTemplate(newVoucherTemplate, DatabaseManager.Connection)
                End With
            End If

            NavigationService.GoBack()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "btnSave_Click", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub

    Private Sub cbJournalAccount_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbJournalAccount.SelectionChanged
        Try
            If e.AddedItems.Count > 0 Then
                tbWTaxRate.Text = e.AddedItems(0).Rate
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "cbJournalAccount_SelectionChanged", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub chkSaveAsTemplate_Checked(sender As Object, e As RoutedEventArgs) Handles chkSaveAsTemplate.Checked, chkSaveAsTemplate.Unchecked
        If chkSaveAsTemplate.IsChecked Then
            pnlTemplateName.Visibility = Visibility.Visible
        Else : pnlTemplateName.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub AddVoucher_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        SupplierAccount = New Model.SupplierAccount

        Particulars = New ObservableCollection(Of books_service.Model.ParticularsItem)
        lstParticulars.ItemsSource = Particulars
    End Sub

    Public Enum EditModeChoices
        Voucher
        Template
    End Enum

End Class
