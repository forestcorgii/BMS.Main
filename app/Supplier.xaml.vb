Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient
Imports books_service

Class Supplier

    Public Property Suppliers As New ObservableCollection(Of Model.Supplier)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        DatabaseManager.Connection.Open()
        SetupAutoComplete()
        LoadSuppliers()
        DatabaseManager.Connection.Close()
    End Sub
    Private Function SetupAutoComplete()
        Dim _suppliers As List(Of Model.Supplier) = Controller.Supplier.GroupSuppliers(DatabaseManager).ToList

        Dim autoCompleteSource As New Forms.AutoCompleteStringCollection
        For Each _supplier As Model.Supplier In _suppliers
            autoCompleteSource.Add(_supplier.Name)
        Next

        With tbSearch
            .AutoCompleteCustomSource = autoCompleteSource
            .AutoCompleteMode = Forms.AutoCompleteMode.SuggestAppend
            .AutoCompleteSource = Forms.AutoCompleteSource.CustomSource
        End With

        Return True
    End Function

    Private Sub LoadSuppliers()
        lstSupplier.ItemsSource = Controller.Supplier.LoadSuppliers(DatabaseManager, tbSearch.Text)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        For Each supplier In Suppliers
            Controller.Supplier.SaveSupplier(supplier, DatabaseManager.Connection)
        Next

        LoadSuppliers()
        DatabaseManager.Connection.Close()

        MessageBox.Show("Changes has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub lstCompany_KeyDown(sender As Object, e As KeyEventArgs) Handles lstSupplier.PreviewKeyDown
        If e.Key = Key.Delete Then
            lstSupplier.SelectedItem.changestate = 2
            lstSupplier.Items.Refresh()
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        LoadSuppliers()
        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnAddSupplierAccount_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New SupplierAccount)
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub

    Private Sub btnImportSupplier_Click(sender As Object, e As RoutedEventArgs)
        Dim openFile As New Forms.OpenFileDialog
        openFile.Filter = "CSV Files | *.csv"
        openFile.Multiselect = True
        If openFile.ShowDialog() = Forms.DialogResult.OK Then
            DatabaseManager.Connection.Open()
            Controller.Supplier.ImportSuppliers(DatabaseManager, openFile.FileNames)

            LoadSuppliers()
            DatabaseManager.Connection.Close()
        End If
    End Sub

End Class
