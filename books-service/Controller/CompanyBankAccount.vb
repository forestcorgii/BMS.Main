Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports utility_service

Namespace Controller

    Public Class CompanyBankAccount

        Public Shared Function SaveCompanyBankAccount(companyBankAccount As Model.CompanyBankAccount, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If companyBankAccount.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`company_bank_account` (name,account_number,code,company_id)values(@name,@account_number,@code,@company_id)"
                ElseIf companyBankAccount.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`company_bank_account` where id=@id;"
                    command.Parameters.AddWithValue("id", companyBankAccount.Id)
                ElseIf companyBankAccount.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`company_bank_account` (id,name,account_number,code,company_id)values(@id,@name,@account_number,@code,@company_id)"
                    command.Parameters.AddWithValue("id", companyBankAccount.Id)
                End If

                If companyBankAccount.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("name", companyBankAccount.Name)
                    command.Parameters.AddWithValue("account_number", companyBankAccount.Account_Number)
                    command.Parameters.AddWithValue("code", companyBankAccount.Code)
                    command.Parameters.AddWithValue("company_id", companyBankAccount.Company.Id)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveCompanyBankAccount", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Function LoadCompanyBankAccounts(databaseManager As Manager.Mysql, Optional companyId As Integer = 0) As ObservableCollection(Of Model.CompanyBankAccount)
            Dim bankAccounts As New ObservableCollection(Of Model.CompanyBankAccount)
            Dim query As String = "SELECT * FROM `voucher_entry`.company_bank_account_complete;"
            If companyId > 0 Then query = String.Format("SELECT * FROM `voucher_entry`.company_bank_account_complete WHERE company_id = {0};", companyId)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(query)
                If reader.HasRows Then
                    While reader.Read
                        bankAccounts.Add(New Model.CompanyBankAccount(reader))
                    End While
                End If
            End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadCompanyBankAccounts", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return bankAccounts
        End Function

    End Class
End Namespace


