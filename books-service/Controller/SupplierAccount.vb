Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports utility_service

Namespace Controller
    Public Class SupplierAccount

        Public Shared Function SaveSupplierAccount(supplierAccount As Model.SupplierAccount, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If supplierAccount.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`supplier_account` (account_number,supplier_id)values(@account_number,@supplier_id)"
                ElseIf supplierAccount.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`supplier_account` where id=@id;"
                    command.Parameters.AddWithValue("id", supplierAccount.Id)
                ElseIf supplierAccount.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`supplier_account` (id,account_number,supplier_id)values(@id,@account_number,@supplier_id)"
                    command.Parameters.AddWithValue("id", supplierAccount.Id)
                End If

                If supplierAccount.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("account_number", supplierAccount.Account_Number)
                    command.Parameters.AddWithValue("supplier_id", supplierAccount.Supplier.Id)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveSupplierAccount", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Sub CompleteSupplierAccountDetail(supplierAccount As Model.SupplierAccount, databaseManager As utility_service.Manager.Mysql)
            Dim ConnectionIsOpenedFromOutside As Boolean = databaseManager.Connection.State = ConnectionState.Open
            If Not ConnectionIsOpenedFromOutside Then databaseManager.Connection.Open()

            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from `voucher_entry`.supplier where id={0}", supplierAccount.Supplier_Id))
                    If reader.HasRows Then
                        reader.Read()
                        supplierAccount.Supplier = New Model.Supplier(reader)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "CompleteSupplierAccountDetail", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            If Not ConnectionIsOpenedFromOutside Then databaseManager.Connection.Close()
        End Sub

        ''' <summary>
        ''' Load all Suppler Accounts.
        ''' 20220223 - Excluded default Supplier Accounts.
        ''' </summary>
        ''' <param name="databaseManager"></param>
        ''' <returns></returns>
        Public Shared Function LoadSupplierAccounts(databaseManager As Manager.Mysql, Optional completeDetail As Boolean = False) As ObservableCollection(Of Model.SupplierAccount)
            Dim supplier_accounts As New List(Of Model.SupplierAccount)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.supplier_account WHERE account_number<>'';")
                    If reader.HasRows Then
                        While reader.Read
                            Dim _supplier_account As New Model.SupplierAccount(reader)
                            supplier_accounts.Add(_supplier_account)
                        End While
                    End If
                End Using

                If completeDetail Then
                    supplier_accounts.ForEach(Sub(item As Model.SupplierAccount) CompleteSupplierAccountDetail(item, databaseManager))
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadSupplierAccounts", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return New ObservableCollection(Of Model.SupplierAccount)(supplier_accounts)
        End Function

        Public Shared Function GetSupplierAccount(databaseManager As Manager.Mysql, account_number As String, supplierId As Integer) As Model.SupplierAccount
            Dim supplierAccount As Model.SupplierAccount = Nothing
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("SELECT * FROM `voucher_entry`.supplier_account WHERE account_number='{0}' AND supplier_id={1} LIMIT 1;", account_number, supplierId))
                    If reader.HasRows Then
                        reader.Read()
                        supplierAccount = New Model.SupplierAccount(reader)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "GetSupplierAccount", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return supplierAccount
        End Function
    End Class
End Namespace
