Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports utility_service

Namespace Controller

    Public Class JournalAccount


        Public Shared Function SaveJournalAccount(journalAccount As Model.JournalAccount, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If journalAccount.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`journal_account` (name,rate)values(@name,@rate)"
                ElseIf journalAccount.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`journal_account` where id=@id;"
                    command.Parameters.AddWithValue("id", journalAccount.Id)
                ElseIf journalAccount.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`journal_account` (id,name,rate)values(@id,@name,@rate)"
                    command.Parameters.AddWithValue("id", journalAccount.Id)
                End If

                If journalAccount.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("name", journalAccount.Name)
                    command.Parameters.AddWithValue("rate", journalAccount.Rate)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveJournalAccount", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return Nothing
        End Function

        Public Shared Function LoadJournalAccounts(databaseManager As Manager.Mysql) As ObservableCollection(Of Model.JournalAccount)
            Dim journalAccounts As New ObservableCollection(Of Model.JournalAccount)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.journal_account;")
                    If reader.HasRows Then
                        While reader.Read
                            journalAccounts.Add(New Model.JournalAccount(reader))
                        End While
                    Else : MessageBox.Show("There is no available Journal Account to select, please finish setting up.", "", MessageBoxButton.OK, MessageBoxImage.Error)
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadJournalAccounts", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return journalAccounts
        End Function

    End Class
End Namespace
