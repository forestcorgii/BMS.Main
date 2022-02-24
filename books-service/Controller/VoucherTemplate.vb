Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports utility_service

Namespace Controller
    Public Class VoucherTemplate

        Public Shared Function SaveVoucherTemplate(voucherTemplate As Model.VoucherTemplate, conn As MySqlConnection)
            Try
                Dim _name As String = String.Format("{0}({1})", voucherTemplate.Voucher.Supplier.Payee, voucherTemplate.Voucher.Supplier_Account.Account_Number)
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If voucherTemplate.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`voucher_template` (name,voucher_json,remarks)values(@name,@voucher_json,@remarks)"
                ElseIf voucherTemplate.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`voucher_template` where id=@id;"
                    command.Parameters.AddWithValue("id", voucherTemplate.Id)
                ElseIf voucherTemplate.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`voucher_template` (id,name,voucher_json,remarks)values(@id,@name,@voucher_json,@remarks)"
                    command.Parameters.AddWithValue("id", voucherTemplate.Id)
                End If

                If voucherTemplate.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("name", _name)
                    command.Parameters.AddWithValue("remarks", voucherTemplate.Remarks)

                    voucherTemplate.Voucher.Id = 0
                    command.Parameters.AddWithValue("voucher_json", JsonConvert.SerializeObject(voucherTemplate.Voucher, Formatting.Indented))
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveVoucherTemplate", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Function LoadVoucherTemplates(databaseManager As Manager.Mysql) As ObservableCollection(Of Model.VoucherTemplate)
            Dim voucherTemplates = New ObservableCollection(Of Model.VoucherTemplate)

            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.voucher_template;")
                    If reader.HasRows Then
                        While reader.Read
                            voucherTemplates.Add(New Model.VoucherTemplate(reader))
                        End While
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadVoucherTemplates", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return voucherTemplates
        End Function
    End Class
End Namespace