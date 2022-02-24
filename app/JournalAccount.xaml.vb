Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class JournalAccount

    Public Property JournalAccounts As New ObservableCollection(Of Model.JournalAccount)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LoadJournalAccounts()
    End Sub

    Private Sub LoadJournalAccounts()
        DatabaseManager.Connection.Open()

        JournalAccounts = Controller.JournalAccount.LoadJournalAccounts(DatabaseManager)
        lstJournalAccount.ItemsSource = JournalAccounts

        DatabaseManager.Connection.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        For Each journalAccount As Model.JournalAccount In JournalAccounts
            Controller.JournalAccount.SaveJournalAccount(journalAccount, DatabaseManager.Connection)
        Next
        DatabaseManager.Connection.Close()

        LoadJournalAccounts()
        MessageBox.Show("Changes has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub lstCompany_KeyDown(sender As Object, e As KeyEventArgs) Handles lstJournalAccount.PreviewKeyDown
        If e.Key = Key.Delete Then
            lstJournalAccount.SelectedItem.changestate = 2
            lstJournalAccount.Items.Refresh()
        End If
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub
End Class
