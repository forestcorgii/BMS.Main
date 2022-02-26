Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class SupplierAccount
    Public Property DefaultParticulars As New ObservableCollection(Of Model.ParticularsItem)

    Private Mode As ProcessTypeChoices

#Region "Properties"

    Private Property Supplier As Model.Supplier
        Get
            Return cbSupplier.SelectedItem
        End Get
        Set(value As Model.Supplier)
            For i As Integer = 0 To cbSupplier.Items.Count - 1
                If value.Id = cbSupplier.Items(i).id Then
                    cbSupplier.SelectedIndex = i
                End If
            Next
        End Set
    End Property

    'Private Property Company As Model.Company
    '    Get
    '        Return cbCompany.SelectedItem
    '    End Get
    '    Set(value As Model.Company)
    '        For i As Integer = 0 To cbCompany.Items.Count - 1
    '            If value.Id = cbCompany.Items(i).id Then
    '                cbCompany.SelectedIndex = i
    '            End If
    '        Next
    '    End Set
    'End Property

    'Private Property Company_Bank_Account As Model.CompanyBankAccount
    '    Get
    '        Return cbBankAccount.SelectedItem
    '    End Get
    '    Set(value As Model.CompanyBankAccount)
    '        For i As Integer = 0 To cbBankAccount.Items.Count - 1
    '            If value.Id = cbBankAccount.Items(i).id Then
    '                cbBankAccount.SelectedIndex = i
    '            End If
    '        Next
    '    End Set
    'End Property

#End Region


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Mode = ProcessTypeChoices.SETUP
        ' Add any initialization after the InitializeComponent() call.
        SetupAutoComplete()

        'lstParticulars.ItemsSource = DefaultParticulars
        lstSupplierAccounts.SelectedIndex = 0

        Mode = ProcessTypeChoices.STAND_BY
    End Sub

    Public Sub SetupAutoComplete()
        DatabaseManager.Connection.Open()

        'cbCompany.ItemsSource = Controller.Company.LoadCompanies(DatabaseManager)

        cbSupplier.ItemsSource = Controller.Supplier.LoadSuppliers(DatabaseManager)

        lstSupplierAccounts.ItemsSource = Controller.SupplierAccount.LoadSupplierAccounts(DatabaseManager, completeDetail:=True)

        DatabaseManager.Connection.Close()
    End Sub

    'Private Sub cbCompany_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCompany.SelectionChanged
    '    If e.AddedItems.Count > 0 Then
    '        Dim ConnectionIsOpenedFromOutside As Boolean = DatabaseManager.Connection.State = System.Data.ConnectionState.Open
    '        If Not ConnectionIsOpenedFromOutside Then DatabaseManager.Connection.Open()

    '        Dim company As Model.Company = DirectCast(e.AddedItems(0), Model.Company)

    '        cbBankAccount.ItemsSource = Controller.CompanyBankAccount.LoadCompanyBankAccounts(DatabaseManager, companyId:=company.Id)

    '        If Not ConnectionIsOpenedFromOutside Then DatabaseManager.Connection.Close()
    '    End If
    'End Sub

    Private Sub btnAddSupplierAccount_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        Dim newSupplierAccount As New Model.SupplierAccount
        With newSupplierAccount
            .Supplier = Supplier
            '.Company = Company
            '.Company_Bank_Account = Company_Bank_Account

            .Account_Number = tbAccountNumber.Text
            '.Default_Particulars = DefaultParticulars
            .ChangeState = States.ChangeState.Added

            If lbId.Text <> "" Then
                .Id = lbId.Text
                .ChangeState = States.ChangeState.None
            End If

            Controller.SupplierAccount.SaveSupplierAccount(newSupplierAccount, DatabaseManager.Connection)
        End With
        DatabaseManager.Connection.Close()

        ClearSupplierAccountFields()
        SetupAutoComplete()
        MessageBox.Show("Supplier Account has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub FillSupplierAccountFields(supplierAccount As Model.SupplierAccount)
        lbId.Text = supplierAccount.Id
        Supplier = supplierAccount.Supplier
        'Company = supplierAccount.Company
        'Company_Bank_Account = supplierAccount.Company_Bank_Account
        tbAccountNumber.Text = supplierAccount.Account_Number
        'DefaultParticulars = supplierAccount.Default_Particulars

        'lstParticulars.ItemsSource = DefaultParticulars
    End Sub

    Private Sub ClearSupplierAccountFields()
        lbId.Text = ""
        cbSupplier.SelectedItem = Nothing
        tbAccountNumber.Text = ""
        DefaultParticulars = New ObservableCollection(Of Model.ParticularsItem)

        'lstParticulars.ItemsSource = DefaultParticulars
    End Sub

    Private Sub lstSupplierAccounts_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstSupplierAccounts.SelectionChanged
        If lstSupplierAccounts.SelectedItem IsNot Nothing Then FillSupplierAccountFields(lstSupplierAccounts.SelectedItem)
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub
End Class
