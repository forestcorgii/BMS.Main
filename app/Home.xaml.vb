Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports books_service

Class Home


    Public Property VoucherSchedules As ObservableCollection(Of Model.VoucherScheduledEntry)
    Public Property VoucherReminders As New ObservableCollection(Of Model.VoucherReminder)


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' LoadVoucherSchedules()
    End Sub

    Private Sub LoadVoucherSchedules()
        DatabaseManager.Connection.Open()
        Try
            'Collect Voucher Schedules
            VoucherSchedules = Controller.VoucherScheduledEntry.LoadVoucherSchedules(DatabaseManager)

            'Generate Voucher Reminders based from Voucher Schedules
            For Each voucherSched As Model.VoucherScheduledEntry In VoucherSchedules
                If CheckSchedule(voucherSched) Then
                    Dim newReminder As New Model.VoucherReminder
                    With newReminder
                        .Schedule_Id = voucherSched.Id
                        .Visible_Until = Date.Parse(String.Format("{0}-{1}-{2}", Now.Year, Now.Month, voucherSched.Start_From.Day))

                        .ChangeState = States.ChangeState.Added
                        Controller.VoucherReminder.SaveVoucherScheduleReminder(newReminder, DatabaseManager.Connection)
                    End With
                End If
            Next

            'Collect Reminders after Generating
            VoucherReminders = Controller.VoucherReminder.LoadVoucherReminders(DatabaseManager)

            lstReminders.ItemsSource = VoucherReminders
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        DatabaseManager.Connection.Close()
    End Sub

    'checks if its time
    Private Function CheckSchedule(da As Model.VoucherScheduledEntry)
        'Return False
        Select Case da.Interval
            Case Model.VoucherScheduledEntry.IntervalChoices.Annual
                Return Now.ToString("MMdd") = da.Start_From.ToString("MMdd") And da.Last_Reminder_Generated.ToString("yyMMdd") <> Now.ToString("yyMMdd")
            Case Model.VoucherScheduledEntry.IntervalChoices.Bi_Annual
                Return da.Start_From.Day = Now.Day And (((Now - da.Start_From).TotalDays \ 30) Mod 6) = 0 And da.Last_Reminder_Generated.ToString("yyMMdd") <> Now.ToString("yyMMdd")
            Case Model.VoucherScheduledEntry.IntervalChoices.Monthly
                Return da.Start_From.Day = Now.Day And da.Last_Reminder_Generated.ToString("yyMMdd") <> Now.ToString("yyMMdd")
        End Select
        Return False
    End Function

    Private Sub btnGenerate_Click(sender As Object, e As RoutedEventArgs)
        Dim voucherReminder As Model.VoucherReminder = lstReminders.SelectedItem

        DatabaseManager.Connection.Open()
        Controller.VoucherReminder.CompleteVoucherReminderDetail(voucherReminder, DatabaseManager)
        DatabaseManager.Connection.Close()

        NavigationService.Navigate(New AddVoucher(voucherReminder.Schedule.Template.Voucher))
    End Sub

    'Private Sub lstReminders_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstReminders.SelectionChanged
    '    Dim voucherReminder As Model.VoucherReminder = lstReminders.SelectedItem
    '    voucherReminder.CompleteVoucherReminderDetail(DatabaseManager)
    '    NavigationService.Navigate(New AddVoucher(voucherReminder.Schedule.Template.Voucher))
    'End Sub

End Class
