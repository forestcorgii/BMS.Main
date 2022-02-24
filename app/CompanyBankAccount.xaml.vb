Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class CompanyBankAccount

    Public Property CompanyBankAccounts As New ObservableCollection(Of Model.CompanyBankAccount)


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LoadCompanyBankAccounts()

    End Sub

    Public Sub LoadCompanyBankAccounts()
        DatabaseManager.Connection.Open()
        cbCompany.ItemsSource = Controller.Company.LoadCompanies(DatabaseManager)
        CompanyBankAccounts = Controller.CompanyBankAccount.LoadCompanyBankAccounts(DatabaseManager)
        DatabaseManager.Connection.Close()

        lstCompanyBankAccounts.ItemsSource = CompanyBankAccounts
    End Sub


    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        For Each bankAccount As Model.CompanyBankAccount In CompanyBankAccounts
            Controller.CompanyBankAccount.SaveCompanyBankAccount(bankAccount, DatabaseManager.Connection)
        Next
        DatabaseManager.Connection.Close()

        LoadCompanyBankAccounts()
        MessageBox.Show("Changes has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub btnAddBankAccount_Click(sender As Object, e As RoutedEventArgs)
        Dim newBankAccount As New Model.CompanyBankAccount
        With newBankAccount
            .Company = cbCompany.SelectedItem
            .Name = tbName.Text
            .Code = tbCode.Text
            .Account_Number = tbAccountNumber.Text

            .ChangeState = States.ChangeState.Added
        End With
        CompanyBankAccounts.Add(newBankAccount)
        MessageBox.Show("Bank Account has been Added.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub lstCompanyBankAccounts_KeyDown(sender As Object, e As KeyEventArgs) Handles lstCompanyBankAccounts.PreviewKeyDown
        If e.Key = Key.Delete Then
            lstCompanyBankAccounts.SelectedItem.changestate = 2
            lstCompanyBankAccounts.Items.Refresh()
        End If
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub
End Class
