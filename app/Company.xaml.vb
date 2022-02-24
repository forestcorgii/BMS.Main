Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class Company

    Public Property Companies As New ObservableCollection(Of Model.Company)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LoadCompanies()
    End Sub

    Private Sub LoadCompanies()
        DatabaseManager.Connection.Open()
        Companies = Controller.Company.LoadCompanies(DatabaseManager)
        lstCompany.ItemsSource = Companies
        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        For Each company In Companies
            Controller.Company.SaveCompany(company, DatabaseManager.Connection)
        Next
        DatabaseManager.Connection.Close()

        LoadCompanies()
        MessageBox.Show("Changes has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub lstCompany_KeyDown(sender As Object, e As KeyEventArgs) Handles lstCompany.PreviewKeyDown
        If e.Key = Key.Delete Then
            lstCompany.SelectedItem.changestate = 2
            lstCompany.Items.Refresh()
        End If
    End Sub

    Private Sub btnAddCompanyBankAccount_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New CompanyBankAccount)
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub
End Class
