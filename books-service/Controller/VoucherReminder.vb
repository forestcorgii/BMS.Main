Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports utility_service

Namespace Controller
    Public Class VoucherReminder

        Public Shared Function SaveVoucherScheduleReminder(voucherScheduleReminder As Model.VoucherReminder, conn As MySqlConnection)
            Try

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveVoucherScheduleReminder", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Dim query As String = ""
            Dim command As New MySqlCommand()
            If voucherScheduleReminder.ChangeState = States.ChangeState.Added Then
                query = "insert into `voucher_entry`.`voucher_schedule_reminder` (schedule_id,visible_until)values(@schedule_id,@visible_until)"
            ElseIf voucherScheduleReminder.ChangeState = States.ChangeState.Deleted Then
                query = "delete from `voucher_entry`.`voucher_schedule_reminder` where id=@id;"
                command.Parameters.AddWithValue("id", voucherScheduleReminder.Id)
            ElseIf voucherScheduleReminder.ChangeState = States.ChangeState.None Then
                query = "replace into `voucher_entry`.`voucher_schedule_reminder` (id,schedule_id,visible_until)values(@id,@schedule_id,@visible_until)"
                command.Parameters.AddWithValue("id", voucherScheduleReminder.Id)
            End If

            If voucherScheduleReminder.ChangeState <> States.ChangeState.Deleted Then
                command.Parameters.AddWithValue("schedule_id", voucherScheduleReminder.Schedule_Id)
                command.Parameters.AddWithValue("visible_until", voucherScheduleReminder.Visible_Until)
            End If

            command.CommandText = query
            command.Connection = conn
            command.ExecuteNonQuery()

            Return Nothing
        End Function

        Public Shared Sub CompleteVoucherReminderDetail(voucherScheduleReminder As Model.VoucherReminder, databaseManager As utility_service.Manager.Mysql)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from voucher_entry.voucher_scheduled_entry_complete where id={0}", voucherScheduleReminder.Schedule_Id))
                    If reader.HasRows Then
                        reader.Read()
                        voucherScheduleReminder.Schedule = New Model.VoucherScheduledEntry(reader)
                    End If
                End Using

                VoucherScheduledEntry.CompleteVoucherScheduleDetail(voucherScheduleReminder.Schedule, databaseManager)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "CompleteVoucherScheduleReminder", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

        End Sub

        Public Shared Function LoadVoucherReminders(databaseManager As Manager.Mysql) As ObservableCollection(Of Model.VoucherReminder)
            Dim voucherReminders As New ObservableCollection(Of Model.VoucherReminder)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.voucher_schedule_reminder_complete;")
                    If reader.HasRows Then
                        While reader.Read
                            voucherReminders.Add(New Model.VoucherReminder(reader))
                        End While
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadVoucherReminders", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return voucherReminders
        End Function

    End Class
End Namespace
