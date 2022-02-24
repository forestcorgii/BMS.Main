Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows
Imports MySql.Data.MySqlClient

Namespace Controller


    Public Class Company

        Public Shared Function SaveCompany(company As Model.Company, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If company.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`company` (name,address,tin)values(@name,@address,@tin)"
                ElseIf company.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`company` where id=@id;"
                    command.Parameters.AddWithValue("id", company.Id)
                ElseIf company.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`company` (id,name,address,tin)values(@id,@name,@address,@tin)"
                    command.Parameters.AddWithValue("id", company.Id)
                End If

                If company.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("name", company.Name)
                    command.Parameters.AddWithValue("address", company.Address)
                    command.Parameters.AddWithValue("tin", company.TIN)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveCompany", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Function LoadCompanies(databasemanager As utility_service.Manager.Mysql) As ObservableCollection(Of Model.Company)
            Dim companies As New ObservableCollection(Of Model.Company)
            Using reader As MySqlDataReader = databasemanager.ExecuteDataReader("SELECT * FROM `voucher_entry`.company;")
                If reader.HasRows Then
                    While reader.Read
                        companies.Add(New Model.Company(reader))
                    End While
                Else : MessageBox.Show("There is no available Company to select, please finish setting up.", "", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End Using
            Return companies
        End Function

    End Class
End Namespace
