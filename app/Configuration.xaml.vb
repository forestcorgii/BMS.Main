
Class Configuration
    Private Sub tlCompany_Click(sender As Object, e As RoutedEventArgs) Handles tlCompany.Click
        NavigationService.Navigate(New Company)
    End Sub
    Private Sub tlJournalAccount_Click(sender As Object, e As RoutedEventArgs) Handles tlJournalAccount.Click
        NavigationService.Navigate(New JournalAccount)
    End Sub
    Private Sub tlSupplier_Click(sender As Object, e As RoutedEventArgs) Handles tlSupplier.Click
        NavigationService.Navigate(New Supplier)
    End Sub

End Class
