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

    Private SupplierAccount As Model.SupplierAccount


    Private Companies As List(Of Model.Company)
    Private SupplierAccounts As List(Of Model.SupplierAccount)
    Private Suppliers As List(Of Model.Supplier)
    Private JournalAccounts As List(Of Model.JournalAccount)

#Region "Properties"
    Private __Supplier As Model.Supplier
    Private Property Supplier As Model.Supplier
        Get
            Return __Supplier
        End Get
        Set(value As Model.Supplier)
            __Supplier = value
            tbSupplier.Text = value.ToString
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
            .AutoCompleteMode = Forms.AutoCompleteMode.SuggestAppend
            .AutoCompleteSource = Forms.AutoCompleteSource.CustomSource
        End With

        autoCompleteSource = New Forms.AutoCompleteStringCollection
        SupplierAccounts = Controller.SupplierAccount.LoadSupplierAccounts(DatabaseManager).ToList
        For Each _supplierAccount As Model.SupplierAccount In SupplierAccounts
            autoCompleteSource.Add(_supplierAccount.Account_Number & " - " & _supplierAccount.Supplier_Name & " - " & _supplierAccount.Supplier_Id)
        Next

        With tbSupplierAccount
            .AutoCompleteCustomSource = autoCompleteSource
            .AutoCompleteMode = Forms.AutoCompleteMode.SuggestAppend
            .AutoCompleteSource = Forms.AutoCompleteSource.CustomSource
        End With

        SetupVAT()

        DatabaseManager.Connection.Close()
        Return True
    End Function

    Private Sub SetupVAT()
        Dim vatRate As Double = 0.12
        cbVATType.Items.Add(New Model.VAT() With {.Name = "NON_VAT", .Rate = 0})
        cbVATType.Items.Add(New Model.VAT() With {.Name = "VAT_INC", .Rate = 0})
        cbVATType.Items.Add(New Model.VAT() With {.Name = "VAT_EX", .Rate = vatRate})
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

        tbSupplierAccount.Text = voucherForEdit.Supplier_Account.Account_Number

        Particulars = New ObservableCollection(Of Model.ParticularsItem)(voucherForEdit.Particulars)
        lstParticulars.ItemsSource = Particulars
        JournalAccountDistributions = New ObservableCollection(Of Model.JournalAccountDistributionItem)(voucherForEdit.Journal_Account_Distributions)
        lstJournalAccounts.ItemsSource = JournalAccountDistributions

        tbRemarks.Text = voucherForEdit.Remarks
    End Sub

    Private Sub cbCompany_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCompany.SelectionChanged
        If e.AddedItems.Count > 0 Then 'And Mode = ProcessTypeChoices.STAND_BY Then
            Dim ConnectionIsOpenedFromOutside As Boolean = DatabaseManager.Connection.State = System.Data.ConnectionState.Open
            DatabaseManager.Connection.Open()
            Try
                Dim company As Model.Company = DirectCast(e.AddedItems(0), Model.Company)
                cbBankAccount.ItemsSource = Controller.CompanyBankAccount.LoadCompanyBankAccounts(DatabaseManager, company.Id)

            Catch ex As Exception
                MessageBox.Show(ex.Message, "cbCompany_SelectionChanged", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            DatabaseManager.Connection.Close()
        End If
    End Sub

    Private Sub tbSupplierAccount_TextChanged(sender As Object, e As EventArgs) Handles tbSupplierAccount.LostFocus
        If tbSupplierAccount.TextLength > 5 And Mode = ProcessTypeChoices.STAND_BY Then
            DatabaseManager.Connection.Open()
            Try
                Dim supplierAccount_args As String() = tbSupplierAccount.Text.Split("-")

                SupplierAccount = Controller.SupplierAccount.GetSupplierAccount(DatabaseManager, supplierAccount_args(0).Trim, supplierAccount_args(2).Trim)

                Particulars = New ObservableCollection(Of Model.ParticularsItem)
                For Each i As Model.ParticularsItem In SupplierAccount.Default_Particulars
                    Particulars.Add(i)
                Next
                lstParticulars.ItemsSource = Particulars

                Controller.SupplierAccount.CompleteSupplierAccountDetail(SupplierAccount, DatabaseManager)

                Company = SupplierAccount.Company
                Company_Bank_Account = SupplierAccount.Company_Bank_Account

                Supplier = SupplierAccount.Supplier
            Catch ex As Exception
                MessageBox.Show(ex.Message, "tbSupplierAccount_TextChanged", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            DatabaseManager.Connection.Close()
        End If
    End Sub
    Private Sub tbSupplier_TextChanged(sender As Object, e As EventArgs) Handles tbSupplier.LostFocus
        If tbSupplierAccount.TextLength > 5 And Mode = ProcessTypeChoices.STAND_BY Then
            DatabaseManager.Connection.Open()
            Try
                Dim supplier_args As String() = tbSupplier.Text.Split("-")
                Supplier = Controller.Supplier.GetSupplier(supplier_args(0).Trim, supplier_args(1).Trim, DatabaseManager)
                If Supplier IsNot Nothing Then
                    'SupplierAccount = Controller.SupplierAccount.GetSupplierAccount(DatabaseManager, "", Supplier.Id)

                    'Particulars = New ObservableCollection(Of Model.ParticularsItem)
                    'For Each i As Model.ParticularsItem In SupplierAccount.Default_Particulars
                    '    Particulars.Add(i)
                    'Next
                    'lstParticulars.ItemsSource = Particulars
                End If

                Company = SupplierAccount.Company
                Company_Bank_Account = SupplierAccount.Company_Bank_Account

                Supplier = SupplierAccount.Supplier
            Catch ex As Exception
                MessageBox.Show(ex.Message, "tbSupplier_TextChanged", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            DatabaseManager.Connection.Close()
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
                .VAT = cbVATType.SelectedItem.name
            End With

            JournalAccountDistributions.Add(newJournalAccountDistributionItem)
            lstJournalAccounts.ItemsSource = JournalAccountDistributions

            cbJournalAccount.SelectedItem = Nothing
            tbAmount.Text = ""
            tbWTaxRate.Text = ""
            cbVATType.SelectedItem = Nothing
        Else
            MessageBox.Show("All Fields are Reqiured, input all Fields.", MessageBoxButton.OK, MessageBoxImage.Error)
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
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

            If EditMode = EditModeChoices.Template Then
                With VoucherTemplate
                    .Voucher = newVoucher
                    Controller.VoucherTemplate.SaveVoucherTemplate(VoucherTemplate, DatabaseManager.Connection)
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

    Public Enum EditModeChoices
        Voucher
        Template
    End Enum

    Private Sub chbIsIrregular_Checked(sender As Object, e As RoutedEventArgs)
        If chbIsIrregular.IsChecked Then
            tbSupplier.ReadOnly = False
            tbSupplierAccount.ReadOnly = True
        Else
            tbSupplier.ReadOnly = True
            tbSupplierAccount.ReadOnly = False
        End If
    End Sub
End Class
