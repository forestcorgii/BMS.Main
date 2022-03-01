Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports utility_service

Namespace Controller
    Public Class VoucherScheduledEntry

        Public Shared Function SaveVoucherScheduledEntry(voucherScheduledEntry As Model.VoucherScheduledEntry, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If voucherScheduledEntry.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`voucher_reminder_schedule` (voucher_id,`interval`,start_from,last_reminder_generated)values(@voucher_id,@interval,@start_from,@last_reminder_generated)"
                ElseIf voucherScheduledEntry.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`voucher_reminder_schedule` where id=@id;"
                    command.Parameters.AddWithValue("id", voucherScheduledEntry.Id)
                ElseIf voucherScheduledEntry.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`voucher_reminder_schedule` (id,voucher_id,`interval`,start_from,last_reminder_generated)values(@id,@voucher_id,@interval,@start_from,@last_reminder_generated)"
                    command.Parameters.AddWithValue("id", voucherScheduledEntry.Id)
                End If

                If voucherScheduledEntry.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("voucher_id", voucherScheduledEntry.Voucher_Id)
                    command.Parameters.AddWithValue("interval", voucherScheduledEntry.Interval)
                    command.Parameters.AddWithValue("start_from", voucherScheduledEntry.Start_From)
                    command.Parameters.AddWithValue("last_reminder_generated", voucherScheduledEntry.Start_From)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveVoucherScheduledEntry", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Sub CompleteVoucherScheduleDetail(voucherScheduledEntry As Model.VoucherScheduledEntry, databaseManager As utility_service.Manager.Mysql)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from voucher_entry.voucher where id={0}", voucherScheduledEntry.Voucher_Id))
                    If reader.HasRows Then
                        reader.Read()
                        voucherScheduledEntry.Voucher = New Model.Voucher(reader)
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show(ex.Message, "CompleteVoucherScheduleDetail", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

        End Sub

        Public Shared Function LoadVoucherSchedules(databaseManager As Manager.Mysql) As ObservableCollection(Of Model.VoucherScheduledEntry)
            Dim voucherSchedules As New ObservableCollection(Of Model.VoucherScheduledEntry)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.voucher_reminder_schedule_complete;")
                    If reader.HasRows Then
                        While reader.Read
                            voucherSchedules.Add(New Model.VoucherScheduledEntry(reader))
                        End While
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "CompleteVoucherScheduleDetail", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return voucherSchedules
        End Function

    End Class
End Namespace
