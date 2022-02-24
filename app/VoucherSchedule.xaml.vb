Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class VoucherSchedule
    Public Property VoucherSchedules As ObservableCollection(Of Model.VoucherScheduledEntry)
    Public Property VoucherTemplates As ObservableCollection(Of Model.VoucherTemplate)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LoadVoucherSchedules()
    End Sub

    Private Sub LoadVoucherSchedules()
        VoucherSchedules = New ObservableCollection(Of Model.VoucherScheduledEntry)
        DatabaseManager.Connection.Open()
        VoucherSchedules = Controller.VoucherScheduledEntry.LoadVoucherSchedules(DatabaseManager)

        lstVoucherSchedules.ItemsSource = VoucherSchedules

        cbVoucherTemplates.ItemsSource = Controller.VoucherTemplate.LoadVoucherTemplates(DatabaseManager)


        DatabaseManager.Connection.Close()
    End Sub


    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        For Each voucherTemplate In VoucherSchedules
            Controller.VoucherScheduledEntry.SaveVoucherScheduledEntry(voucherTemplate, DatabaseManager.Connection)
        Next
        DatabaseManager.Connection.Close()

        LoadVoucherSchedules()
        MessageBox.Show("Changes has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub lstVoucherTemplates_KeyDown(sender As Object, e As KeyEventArgs) Handles lstVoucherSchedules.PreviewKeyDown
        If e.Key = Key.Delete Then
            lstVoucherSchedules.SelectedItem.changestate = 2
            lstVoucherSchedules.Items.Refresh()
        End If
    End Sub
    Private Sub btnAddVoucherSchedule_Click(sender As Object, e As RoutedEventArgs)
        DatabaseManager.Connection.Open()
        Dim newVoucherScheduleEntry As New Model.VoucherScheduledEntry
        With newVoucherScheduleEntry
            .Template_Id = cbVoucherTemplates.SelectedItem.id
            .Interval = cbInterval.SelectedIndex

            .Start_From = dtFrom.SelectedDate
            .End_To = dtTo.SelectedDate

            .ChangeState = States.ChangeState.Added
            If lbId.Text <> "" And lbId.Text <> "0" Then
                .Id = lbId.Text
                .ChangeState = States.ChangeState.None
            End If
            Controller.VoucherScheduledEntry.SaveVoucherScheduledEntry(newVoucherScheduleEntry, DatabaseManager.Connection)
            '.SaveVoucherScheduledEntry(DatabaseManager.Connection)
        End With
        DatabaseManager.Connection.Close()

        ClearVoucherScheduleEntry()
        LoadVoucherSchedules()
        MessageBox.Show("Supplier Account has been saved.", "", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub FillVoucherScheduleEntry(_voucherSchedule As Model.VoucherScheduledEntry)
        lbId.Text = _voucherSchedule.Id
        For i As Integer = 0 To cbVoucherTemplates.Items.Count - 1
            If _voucherSchedule.Template_Id = cbVoucherTemplates.Items(i).id Then
                cbVoucherTemplates.SelectedIndex = i
            End If
        Next

        cbInterval.SelectedIndex = _voucherSchedule.Interval

        dtFrom.SelectedDate = _voucherSchedule.Start_From
        dtTo.SelectedDate = _voucherSchedule.End_To

    End Sub

    Private Sub ClearVoucherScheduleEntry()
        lbId.Text = ""
        cbVoucherTemplates.SelectedItem = Nothing
        cbInterval.SelectedItem = Nothing

        dtFrom.SelectedDate = Now
        dtTo.SelectedDate = Now

    End Sub

    Private Sub lstVoucherSchedules_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstVoucherSchedules.SelectionChanged
        If lstVoucherSchedules.SelectedItem IsNot Nothing Then FillVoucherScheduleEntry(lstVoucherSchedules.SelectedItem)
    End Sub

    Private Sub btnGoBack_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.GoBack()
    End Sub
End Class
